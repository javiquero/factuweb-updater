using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace updaterFactuWeb {
	public class Mdb {
		private readonly string dbPath;
		private readonly string connectionString = "";

		public Mdb(string path) {
			if (File.Exists(path)) {
				this.dbPath = path;
				this.connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "; Persist Security Info=False;";
				//this.connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";";
				//"; Persist Security Info=False;";
			} else {
				Logger.log("The database file was not found or could not be accessed." +  path);
				throw new FileNotFoundException("The database file was not found or could not be accessed.", path);
			}
		}

		public DataTable Query(string queryString) {
			DataTable dt = new DataTable();
			try {
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					OleDbDataAdapter Accessda = new OleDbDataAdapter {
						SelectCommand = new OleDbCommand(queryString, connection)
					};
					Accessda.Fill(dt);
					connection.Close();
				}
			} catch (System.Exception e) {
				Exception ex = new Exception($"Error Mdb.execute:\n\t { e.Message}\nin query: {queryString}");
				Logger.log(ex);
				throw;
				//Console.WriteLine("Error dbController.query:\n\t {0}\nin query: {1}", e.Message, queryString);
			}
			return dt;
		}

		private List<string> _GetTablesNames = new List<string>();
		internal List<string> GetTablesNames() {
			if (_GetTablesNames.Count > 0)
				return _GetTablesNames;
			List<string> resp = new List<string>();
			List<Dictionary<string, object>> tables = this.GetTables();
			foreach (Dictionary<string, object> table in tables) {
				if (table["TABLE_NAME"].ToString().Substring(0, 1) != "~") {
					resp.Add(table["TABLE_NAME"].ToString());
				}
			}
			_GetTablesNames = resp;
			return _GetTablesNames;
		}

		private List<Dictionary<string, object>> _GetTables = new List<Dictionary<string, object>>();
		internal List<Dictionary<string, object>> GetTables() {
			try {
				if (_GetTables.Count > 0)
					return _GetTables;
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					connection.Open();

					DataTable schemaTable = connection.GetOleDbSchemaTable(
						OleDbSchemaGuid.Tables,
						new object[] { null, null, null, "TABLE" });
					this._GetTables = DataTableToList(schemaTable);
					return this._GetTables;
				}
			} catch (System.Exception e) {
				Logger.log(e);
				throw;
			}
		}

		public static List<Dictionary<string, object>> DataTableToList(DataTable dt) {
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			
			if (dt is null) {
				Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - dt no puede ser null.");
				return rows;
			}			
			
			foreach (DataRow dr in dt.Rows) {
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns) {
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			return rows;
		}
		
		private Dictionary<string, DataTable> _GetSchemaIndexTable = new Dictionary<string, DataTable>();
		internal DataTable GetSchemaIndexTable(string model) {
			try {
				if (_GetSchemaIndexTable.ContainsKey(model)) {
					return _GetSchemaIndexTable[model];
				}
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					connection.Open();

					DataTable schemaTable = connection.GetSchema("Indexes");
					for (int i = 0; i < schemaTable.Rows.Count; i++) {
						DataRow dr = schemaTable.Rows[i];
						if (dr["TABLE_NAME"].ToString() != model)
							dr.Delete();
					}
					schemaTable.AcceptChanges();
					DataView dv = schemaTable.DefaultView;
					dv.Sort = "PRIMARY_KEY DESC, ORDINAL_POSITION ASC";
					DataTable sortedDT = dv.ToTable();
					_GetSchemaIndexTable.Add(model, sortedDT);
					return sortedDT;
				}
			} catch (System.Exception e) {
				//Console.WriteLine("Error dbController.GetSchemaIndexTable:\n\t {0}\nin model: {1}", e.Message, model);
				Logger.log(e);
				throw;
			}
		}

		private Dictionary<string, List<string>> _GetIndexes = new Dictionary<string, List<string>>(); 
		internal List<string> GetIndexes(string model) {
			if (_GetIndexes.ContainsKey(model)) {
				return _GetIndexes[model];
			}
			List<string> response = new List<string>();

			DataTable keys = GetSchemaIndexTable(model);
			if (keys.Rows.Count == 0) { return response; }

			DataRow[] primary = keys.Select("PRIMARY_KEY = true");
			foreach (DataRow dr in primary) {
				response.Add(dr["COLUMN_NAME"].ToString());
			}
			_GetIndexes.Add(model, response);
			return response;
		}

		private Dictionary<string, string> _GetTypeColumn = new Dictionary<string, string>();
		internal string GetTypeColumn(string tableName, string columnName) {
			if (_GetTypeColumn.ContainsKey(tableName+"|"+columnName)) {
				return _GetTypeColumn[tableName + "|" + columnName];
			}

			ADOX.Catalog Cat = new ADOX.Catalog();
			ADODB.Connection cn = new ADODB.Connection();
			cn.ConnectionString = this.connectionString;
			cn.Open();
			Cat.ActiveConnection = cn;

			foreach (ADOX.Table item in Cat.Tables) {
				if (item.Name == tableName && item.Type == "TABLE") {
					foreach (ADOX.Column c in item.Columns) {
						string _type_ = DataType(c);
						_GetTypeColumn.Add(tableName + "|" + c.Name, _type_);
					}
					cn.Close();
					return _GetTypeColumn[tableName + "|" + columnName];
				}
			}
			return null;
		}

		private static string DataType(ADOX.Column colDef) {
			int intLength = 0;
			int intPrecision = 0;
			int intScale = 0;
			string strNewType = null;

			intLength = colDef.DefinedSize;
			intPrecision = colDef.Precision;
			intScale = colDef.NumericScale;
			strNewType = colDef.Name;
			switch (colDef.Type) {
				case ADOX.DataTypeEnum.adBigInt:
					strNewType = "BIGINT";
					break;
				case ADOX.DataTypeEnum.adBoolean:
					strNewType = "TINYINT(1)";
					break;
				case ADOX.DataTypeEnum.adDouble:
					strNewType = "FLOAT";
					break;
				case ADOX.DataTypeEnum.adInteger:
					strNewType = "INTEGER";
					break;
				case ADOX.DataTypeEnum.adNumeric:
					strNewType = "NUMERIC (" + intPrecision + ", " + intScale + ")";
					break;
				case ADOX.DataTypeEnum.adSingle:
					strNewType = "REAL";
					break;
				case ADOX.DataTypeEnum.adUnsignedTinyInt:
					strNewType = "TINYINT";
					break;
				case ADOX.DataTypeEnum.adSmallInt:
					strNewType = "SMALLINT";
					break;
				case ADOX.DataTypeEnum.adTinyInt:
					strNewType = "TINYINT";
					break;
				case ADOX.DataTypeEnum.adVarChar:
					strNewType = "VARCHAR (" + intLength + ")";
					break;
				case ADOX.DataTypeEnum.adVarWChar:
					strNewType = "VARCHAR (" + intLength + ")";
					break;
				case ADOX.DataTypeEnum.adLongVarWChar:
					strNewType = "LONGTEXT";
					break;
				case ADOX.DataTypeEnum.adDate:
					strNewType = "DATETIME";
					break;
				case ADOX.DataTypeEnum.adCurrency:
					strNewType = "FLOAT";
					break;
				case ADOX.DataTypeEnum.adLongVarBinary:
					strNewType = "oleobject";// "VARCHAR (255)";
					break;
				default:
					strNewType = "UNKNOWN";
					break;
			}
			return strNewType;
		}

		private Dictionary<string, string> _GetWhereStringConditionByIndexes = new Dictionary<string, string>();
		public string GetWhereStringConditionByIndexes(string model) {
			if (_GetWhereStringConditionByIndexes.ContainsKey(model)) {
				return _GetWhereStringConditionByIndexes[model];
			}
			List<string> keys = this.GetIndexes(model);
			if (keys.Count() == 0) { return ""; }

			string whereResult = "";
			int x = 0;
			foreach (string columnName in keys) {
				string tipo = this.GetTypeColumn(model, columnName);
				if (whereResult != "") { whereResult += " AND "; }
				whereResult += "`" + columnName + "`=";
				if (tipo.StartsWith("VARCHAR") || tipo == "DATETIME" || tipo == "LONGTEXT") {
					whereResult += "'{" + x + "}'";
				} else {
					whereResult += "{" + x + "}";
				}
				x++;
			}

			_GetWhereStringConditionByIndexes.Add(model, "WHERE " + whereResult);
			return "WHERE " + whereResult;
		}

		public bool Execute(List<string> querys) {
			if (querys == null || querys.Count == 0)
				return false;
			using (OleDbConnection connection = new OleDbConnection(this.connectionString)) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {					
					try {						
						foreach (string q in querys) {
							OleDbCommand command = new OleDbCommand(q, connection, transaction);
                            command.ExecuteNonQuery();
                        }
					} catch (Exception e) {
						Logger.log(e);
						Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - Error al ejecutar los comandos. " + string.Join(";", querys));
						transaction.Rollback();
						connection.Close();
						return false;
						throw;						
					}
					transaction.Commit();					
				}
				connection.Close();
			}
			return true;
		}

	}
}
