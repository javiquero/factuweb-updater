using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

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
				Console.WriteLine("Error dbController.query:\n\t {0}\nin query: {1}", e.Message, queryString);
				//throw e;
			}
			return dt;
		}
		public bool Execute(string queryString) {
			try {
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					connection.Open();
					OleDbCommand command = new
						OleDbCommand(queryString, connection);
					command.ExecuteNonQuery();
					connection.Close();
				}
				return true;
			} catch (System.Exception e) {
				Console.WriteLine("Error Mdb.execute:\n\t {0}\nin query: {1}", e.Message, queryString);
				throw new Exception($"Error Mdb.execute:\n\t { e.Message}\nin query: {queryString}");
			}
		}
		public bool Execute(List<string> queryStrings) {
			try {
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					connection.Open();
					foreach (string item in queryStrings) {

						OleDbCommand command = new
							OleDbCommand(item, connection);
						command.ExecuteNonQuery();
					}

					connection.Close();
				}
					return true;
				
				//bool result = this.Execute(item);
				//if (!result) return false;
			} catch (System.Exception e) {
				Console.WriteLine("Error Mdb.execute:\n\t {0}\nin querys", e.Message);
				throw new Exception($"Error Mdb.execute:\n\t { e.Message}\nin querys");
			}
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
					this._GetTables = this.DataTableToList(schemaTable);
					return this._GetTables;
				}
			} catch (System.Exception e) {
				Console.WriteLine("Error Mdb.GetTables:\n\t {0}", e.Message);
				throw (e);
			}
		}
		public List<Dictionary<string, object>> DataTableToList(DataTable dt) {
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			foreach (DataRow dr in dt.Rows) {
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns) {
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			return rows;
		}
		

		
		
		private Dictionary<string, DataTable> _GetSchemaTable = new Dictionary<string, DataTable>();
		internal DataTable GetSchemaTable(string model) {
			try {
				if (_GetSchemaTable.ContainsKey(model)) {
					return _GetSchemaTable[model];
				}
				using (OleDbConnection connection = new
					OleDbConnection(this.connectionString)) {
					connection.Open();

					DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, model, null });
					DataView dv = schemaTable.DefaultView;
					dv.Sort = "ORDINAL_POSITION ASC";
					DataTable sortedDT = dv.ToTable();
					_GetSchemaTable.Add(model, sortedDT);
					return sortedDT;
				}
			} catch (System.Exception e) {
				Console.WriteLine("Error Mdb.GetSchemaTable:\n\t {0}\nin model: {1}", e.Message, model);
				return GetSchemaTable(model);
			}
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
				Console.WriteLine("Error dbController.GetSchemaIndexTable:\n\t {0}\nin model: {1}", e.Message, model);
				return GetSchemaIndexTable(model);
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
						string _type_ = this.DataType(c);
						_GetTypeColumn.Add(tableName + "|" + c.Name, _type_);
					}
					cn.Close();
					return _GetTypeColumn[tableName + "|" + columnName];
				}
			}
			return null;
		}

		private Dictionary<string, bool> _ExistsTable = new Dictionary<string, bool>();
		public bool ExistsTable(string tableName) {
			if (_ExistsTable.ContainsKey(tableName)) {
				return true;
			}
			List<string> tables = this.GetTablesNames();
			foreach (string table in tables) {
				if (tableName == table) {
					_ExistsTable.Add(tableName, true);
					return true;
				}
			}
			return false;
		}

		private string DataType(ADOX.Column colDef) {
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
		public string GetStringCreateTable(string tableName, bool y = false) {
			String createTable = "";
			DataTable Columns = this.GetSchemaTable(tableName);
			if (Columns.Rows.Count > 0) {
				createTable = "CREATE TABLE `" + tableName + "` ( ";
				if (y) createTable += "`YEAR` VARCHAR (4),";
				for (int colNum = 0; colNum < Columns.Rows.Count; colNum++) {
					createTable += "`" + Columns.Rows[colNum].ItemArray[3].ToString() + "` ";

					string _type = this.GetTypeColumn(tableName, Columns.Rows[colNum].ItemArray[3].ToString());
					createTable += _type;

					if (colNum + 1 < Columns.Rows.Count) {
						createTable += ",";
					}
				}
				createTable += ");";
			}
			return createTable;
		}
		public string GetStringJson(string model, DataRow row, bool y = false) {
			string[] types = new string[row.Table.Columns.Count];
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = this.GetTypeColumn(model, row.Table.Columns[i].ColumnName);
			}
			string response = "{";

			if (y) response += "YEAR: '" + Properties.Settings.Default.year + "'";
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				if (response != "{")  response += ", ";
				response +=  row.Table.Columns[i].ColumnName + ":";
				if (types[i] == "LONGTEXT" || types[i].StartsWith("VARCHAR")) {
					response += "'" + row.ItemArray[i].ToString().Replace(@"\", @"\\").Replace(@"'", @"\'") + "'";
				} else if (types[i] == "DATETIME") {
					DateTime t = DateTime.ParseExact(row.ItemArray[i].ToString(), "dd/MM/yyyy H:mm:ss",
						System.Globalization.CultureInfo.InvariantCulture);
					response += "'" + t.ToString("yyyy-MM-dd HH:mm:ss") + "'";
				} else if (types[i] == "oleobject") {
					response += "''";
				} else {
					response += row.ItemArray[i].ToString().Replace(",", ".");
				}
			}
			return  response + "}";
		}
		public string GetStringJson2(string model, DataRow row, bool y = false) {
			string[] types = new string[row.Table.Columns.Count];
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = this.GetTypeColumn(model, row.Table.Columns[i].ColumnName);
			}
			string response = "{";

			if (y)
				response += "\"YEAR\": \"" + Properties.Settings.Default.year + "\"";
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				if (response != "{")
					response += ", ";
				response += "\"" + row.Table.Columns[i].ColumnName + "\":";
				if (types[i] == "LONGTEXT" || types[i].StartsWith("VARCHAR")) {
					//response += "\"" + System.Text.RegularExpressions.Regex.Replace(row.ItemArray[i].ToString().Replace(@"\", @"\\").Replace(@"'", @"\'"), @"\r\n?|\n", "") + "\"";
					response += "\"" + System.Text.RegularExpressions.Regex.Replace(row.ItemArray[i].ToString().Replace(@"\", @"\\").Replace("\"", "\'").Replace(@"\'", @"\\\'"), @"\r\n?|\n", "") + '"';
				} else if (types[i] == "DATETIME") {
					DateTime t = DateTime.ParseExact(row.ItemArray[i].ToString(), "dd/MM/yyyy H:mm:ss",
						System.Globalization.CultureInfo.InvariantCulture);
					response += "\"" + t.ToString("yyyy-MM-dd HH:mm:ss") + "\"";
				} else if (types[i] == "oleobject") {
					response += "\"\"";
				} else {
					response += row.ItemArray[i].ToString().Replace(",", ".");
				}
			}
			return response + "}";
		}
		public string GetStringInsert(string model, DataRow row, bool y = false) {
			string[] types = new string[row.Table.Columns.Count];
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = this.GetTypeColumn(model, row.Table.Columns[i].ColumnName);
			}
			string vals = "(";
			if (y) vals += "'" + Properties.Settings.Default.year + "'";
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				if (vals!="(") vals += ", ";
				if (types[i] == "LONGTEXT" || types[i].StartsWith("VARCHAR")) {
					vals += "'" + row.ItemArray[i].ToString().Replace("'", "`").Replace("\"", "`") + "'";
				} else if (types[i] == "DATETIME") {
					DateTime t = DateTime.ParseExact(row.ItemArray[i].ToString(), "dd/MM/yyyy H:mm:ss",
						System.Globalization.CultureInfo.InvariantCulture);
					vals += "\"" + t.ToString("yyyy-MM-dd HH:mm:ss") + "\"";
				} else if (types[i] == "oleobject") {
					vals += "''";
				} else {
					if (row.ItemArray[i].ToString() == "") {
						vals += "NULL";
					} else {
						vals += row.ItemArray[i].ToString().Replace(",", ".");
					}
				}
			}
			return "INSERT INTO " + model + " VALUES " + vals + ");";
		}

		public static string EscapeString(string str) {
			return System.Text.RegularExpressions.Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
			delegate (System.Text.RegularExpressions.Match match) {
				string v = match.Value;
				switch (v) {
					case "\x00":            // ASCII NUL (0x00) character
			return "\\0";
					case "\b":              // BACKSPACE character
			return "\\b";
					case "\n":              // NEWLINE (linefeed) character
			return "\\n";
					case "\r":              // CARRIAGE RETURN character
			return "\\r";
					case "\t":              // TAB
			return "\\t";
					case "\u001A":          // Ctrl-Z
			return "\\Z";
					default:
						return "\\" + v;
				}
			});
		}


		public string GetStringUpdate(string model, DataRow row, bool y = false) {
			List<String> indexes = this.GetIndexes(model);
			string indWhere = this.GetWhereStringConditionByIndexes(model);

			string[] indexItems = new string[indexes.Count];
			for (var i = 0; i < indexes.Count; i++) {
				indexItems[i] = row[indexes[i]].ToString();
			}

			string where = string.Format(indWhere, indexItems);

			string[] types = new string[row.Table.Columns.Count];
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = this.GetTypeColumn(model, row.Table.Columns[i].ColumnName);
			}
			string vals = "";
			if (y) vals += "`YEAR` =\"" + Properties.Settings.Default.year + "\"";
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				if (!indexes.Contains(row.Table.Columns[i].ColumnName)) {
					if (vals!="") vals += ", ";
					if (types[i] == "LONGTEXT" || types[i].StartsWith("VARCHAR")) {
						vals += "`" + row.Table.Columns[i].ColumnName + "`='" + row.ItemArray[i].ToString().Replace("'","`").Replace("\"", "`") + "'";
					} else if (types[i] == "DATETIME") {
						DateTime t = DateTime.ParseExact(row.ItemArray[i].ToString(), "dd/MM/yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
						vals += "`" + row.Table.Columns[i].ColumnName + "`='" + t.ToString("yyyy-MM-dd HH:mm:ss") + "'";
					} else if (types[i] == "oleobject") {
						vals += "`" + row.Table.Columns[i].ColumnName + "`=''";
					} else {
						if (row.ItemArray[i].ToString() == "") {
							vals += "`" + row.Table.Columns[i].ColumnName + "`=NULL";
						} else {
							vals += "`" + row.Table.Columns[i].ColumnName + "`=" + row.ItemArray[i].ToString().Replace(",", ".");
						}
					}
				}
			}
			return "UPDATE " + model + " SET " + vals + " " + where + " ;";
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


		public string GetOrderStringByIndexes(string model) {
			List<string> keys = this.GetIndexes(model);
			if (keys.Count() == 0) { return ""; }

			string orderResult = "";
			foreach (string columnName in keys) {
				if (orderResult != "") { orderResult += ", "; }
				orderResult += "`" + columnName + "`";
			}
			return "ORDER BY " + orderResult + " ASC";
		}
		public string GetStringPrimaryKeys(string TableName) {
			string primaryKeys = "ALTER TABLE " + TableName + " ADD PRIMARY KEY(";
			DataTable keys = this.GetSchemaIndexTable(TableName);
			DataRow[] primary = keys.Select("PRIMARY_KEY = true");
			if (primary.Length > 0) {
				int numkeys = 0;
				foreach (DataRow dr in primary) {
					if (numkeys == 1) {
						primaryKeys = primaryKeys.Replace("PRIMARY KEY", "CONSTRAINT " + TableName + "_pkey PRIMARY KEY");
					}
					if (numkeys > 0) {
						primaryKeys += ", ";
					}
					primaryKeys += dr["COLUMN_NAME"];
					numkeys++;
				}
				primaryKeys += ");";
				return primaryKeys;
			}
			return "";
		}
		public List<string> GetStringKeys(string TableName) {
			List<string> response = new List<string>();
			DataTable keys = this.GetSchemaIndexTable(TableName);
			DataRow[] noPrimary = keys.Select("PRIMARY_KEY = false");
			if (noPrimary.Length > 0) {
				foreach (DataRow dr in noPrimary) {
					response.Add("CREATE INDEX " + TableName + "_" + dr["COLUMN_NAME"] + "_idx ON " + TableName + "(" + dr["COLUMN_NAME"] + ");");
				}
			}
			return response;
		}
	}
}
