using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace updaterFactuWeb {
	public class UpdateSqLiteTaskResponse {
		private readonly string message;
		private readonly Int32 value;

		public UpdateSqLiteTaskResponse(string msg, Int32 v = 0) {
			this.message = msg;
			this.value = v;
		}

		public string Message { get { return message; } }
		public Int32 Value { get { return value; } }
	}

	class UpdateSqLite {
		
		//private readonly DataSet dbFactu;
		private readonly LocalDB dbSqlite;
		private readonly apiRest API;
		private readonly int debugLevel = 1;

		// Bandera booleana que indica cuando el proceso está siendo ejecutado o ha sido detenido
		private bool UpdateProcessStopped;

		// Expone el contexto de sincronización en la clase entera 
		private readonly SynchronizationContext SyncContext;

		// Crear los 2 contenedores de callbacks
		public event EventHandler<UpdateTaskResponse> CallbackOnInitUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnFinishUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnStatusChange;
		public event EventHandler<UpdateTaskResponse> CallbackOnError;
		public event EventHandler<UpdateTaskResponse> CallbackOnPercentChange;

		public UpdateSqLite(bool reset = false) {
			
			this.dbSqlite = new LocalDB(reset);
			this.API = new apiRest();
			SyncContext = AsyncOperationManager.SynchronizationContext;
		}

		// Método para iniciar el proceso
		public void StartIncremental() {
			Thread thread = new Thread(RunIncremental);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartTotal() {
			Thread thread = new Thread(RunTotal);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartPhotos() {
			Thread thread = new Thread(RunPhotos);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartClear() {
			Thread thread = new Thread(RunClear);
			thread.IsBackground = true;
			thread.Start();
		}
		public void StartClearOnlyCache() {
			Thread thread = new Thread(RunClearOnlyCache);
			thread.IsBackground = true;
			thread.Start();
		}




		// Método para detener el proceso
		public void Stop() {
			UpdateProcessStopped = true;
			SyncContext.Post(e => triggerOnFinishUpdate(new UpdateTaskResponse("")), null);
		}
		
		private DataSet LoadFromFile(string fileName, DataTable tablesForSync, bool isCurrent = true) {
			DataSet result = new DataSet();

			// For convenience, the DataSet is identified by the name of the loaded file (without extension).
			result.DataSetName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

			// Compute the ConnectionString (using the OLEDB v12.0 driver compatible with ACCDB and MDB files)
			fileName = Path.GetFullPath(fileName);

			//string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\"{0}\"; Jet OLEDB:Database Password={1};", fileName, password);
			string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\"; Persist Security Info=False;", fileName);

			// Opening the Access connection
			using (OleDbConnection conn = new OleDbConnection(connString)) {
				conn.Open();

				// Getting all user tables present in the Access file (Msys* tables are system thus useless for us)
				DataTable dt = conn.GetSchema("Tables");
				List<string> tablesName = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).Where(dr => !dr.StartsWith("MSys")).ToList();

				// Getting the data for every user tables
				foreach (string tableName in tablesName) {
					DataRow[] found = tablesForSync.Select("name='" + tableName.Replace("[", "").Replace("]", "") + "'");
					if (found.Length > 0) {
						if (isCurrent || found[0]["byYears"].ToString() == "1")
							using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", tableName), conn)) {
							using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd)) {
								// Saving all tables in our result DataSet.
								DataTable buf = new DataTable("[" + tableName + "]");
								adapter.Fill(buf);
								result.Tables.Add(buf);
							} // adapter
						} // cmd
					}
				} // tableName
			} // conn

			// Return the filled DataSet
			return result;
		}


		// Método donde la lógica principal de la tarea se ejecuta
		private void RunIncremental() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);

				#region "Obtenemos datos de los modelos a actualizar"
				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}
				#endregion

				DateTime now = DateTime.Today;
				Mdb mdbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension);
				List<string> tables = mdbFactu.GetTablesNames();

				//DataSet dbFactu = this.LoadFromFile(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension, tablesForSync,true);

				//foreach (DataTable table in dbFactu.Tables.Cast<DataTable>()) {
				//foreach (DataTable table in dbFactu.Tables) {
				foreach (string table in tables) {
					//string tableName = table.TableName.Replace("[", "").Replace("]", "");

					DataRow[] found = tablesForSync.Select("name='" + table + "'");
					if (found.Length > 0) {

						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("(" + Properties.Settings.Default.year + ") Actualizando " + table)), null);
						Debug("Init update table - " + table + "!");

						DataTable dtFactu = mdbFactu.Query("SELECT * FROM " + table + ";");
						dtFactu.TableName = table;

						if (found[0]["cols"].ToString() !="") {
							List<DataColumn> r = new List<DataColumn>();
							string[] columnsForSync = ((string[])found[0]["cols"]);
							foreach (var dataColumn in dtFactu.Columns.Cast<DataColumn>()) {
								if (columnsForSync.Length > 0) {
									if (!columnsForSync.Contains(dataColumn.ColumnName)) {
										r.Add(dataColumn);
									}
								}
							}
							foreach (var dt in r)
								dtFactu.Columns.Remove(dt);
						}
						string y = null;
						if (found[0]["byYears"].ToString() == "1")
							y = Properties.Settings.Default.year;

						string indWhere = mdbFactu.GetWhereStringConditionByIndexes(table);
						List<String> indexes = mdbFactu.GetIndexes(table);
						
						bool createtableSql = dbSqlite.ImportTable(dtFactu, indexes, y);
						
						bool st = this.incrementalSyncDataTable(dtFactu, indexes, indWhere, y);
						if (!st) {
							Stop();
							return;
						}
					}
				}

				Stop();
			}
		}

		private bool incrementalSyncDataTable(DataTable table, List<String> indexes, string indWhere, string y = null) {
			if (table is null)
				return false;
			apiRest API = new apiRest();
			var columns = table.Columns.Cast<DataColumn>();
			var columnSql = string.Join(", ", columns.Select(x => dbSqlite.FixedColumnName(x)));
			if (y != null) { columnSql = "YEAR, " + columnSql; }
			var paramSql = string.Join(", ", columns.Select(x => "@" + dbSqlite.FixedColumnName(x)));
			if (y != null) { paramSql = "@YEAR, " + paramSql; }
			var tableName = dbSqlite.FixedTableName(table);
			var insertSql = $"INSERT OR REPLACE INTO {tableName} ({columnSql}) VALUES ({paramSql})";

			using (var conn = dbSqlite.GetConnection()) {
				string where = "";
				
				if (y!= null) where = "WHERE `YEAR`='" + y + "'";

				string query = $"SELECT * FROM {tableName} {where};";
				DataTable sqlItems = dbSqlite.Select(query);

				int it = 0;
				foreach (DataRow row in table.Rows) {
					it++;
					int percent = it * 100 / table.Rows.Count;
					triggerOnPercentChange(new UpdateTaskResponse("", percent));

					string[] indexItems = new string[indexes.Count];
					for (var i = 0; i < indexes.Count; i++) {
						indexItems[i] = row[indexes[i]].ToString();
					}

					 where = string.Format(indWhere, indexItems).Replace("WHERE ", "");
					if (y != null)
						where += " AND `YEAR`='" + y + "'";
					DataRow[] result = sqlItems.Select(where);


					bool inse = false;
					if (result.Length > 0) {
						// **** COMPARE
						foreach (var c in columns) {
							string columnName = dbSqlite.FixedColumnName(c);
							var val = row[c];
							if (c.AllowDBNull && val == DBNull.Value) {
								val = null;
							} else if (val.GetType().Name == "Byte[]") {
								val = "";
							}

							if (!result[0].Table.Columns.Contains(columnName)) {
								string o = dbSqlite.ImportColumn(c);
								dbSqlite.Select($"ALTER TABLE {table.TableName} ADD COLUMN {o};");
								sqlItems = dbSqlite.Select(query);
								result[0].Table.Columns.Add(columnName, c.DataType);
							}

							var valsqlite = result[0][columnName];
							if (c.AllowDBNull && valsqlite == DBNull.Value)
								valsqlite = null;

							if (val == null && valsqlite == null) { } else {
								if (columnName!="YEAR" && val.ToString().All(char.IsNumber)) valsqlite = val;
								if (dbSqlite.FixedColumnName(c) != "YEAR" && !val.Equals(Convert.ChangeType(valsqlite, val.GetType()))) {
									//if ((dynamic)val != (dynamic)valsqlite) { 
									//if (!val.Equals(valsqlite)) {
									inse = true;
									try {
										if (y != null) {
											if (!row.Table.Columns.Contains("YEAR"))
												row.Table.Columns.Add("YEAR");
											row["YEAR"] = Properties.Settings.Default.year;
										}
										bool resp = API.REPLACE(table.TableName, where.Replace("`", "").Replace("'", "").Replace(" AND ", "&"), row);
										if (!resp)
											API.INSERT(table.TableName, row);

									} catch (Exception ex) {
										return false;
									}
									break;
								}
							}
						}
					} else {
						try {
							if (y != null) {
								if (!row.Table.Columns.Contains("YEAR"))
									row.Table.Columns.Add("YEAR");
								row["YEAR"] = Properties.Settings.Default.year;
							}
							API.INSERT(table.TableName, row);
						} catch (Exception ex) {
							return false;
						}
					}

					if (inse) {
						// **** INSERT
						using (var tx = conn.BeginTransaction()) {
							using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn, tx)) {

								if (y != null) {
									var p = cmd.CreateParameter();
									p.ParameterName = "YEAR";
									p.Value = y;
									cmd.Parameters.Add(p);
								}
								foreach (var c in columns) {
									var val = row[c];
									if (c.AllowDBNull && val == DBNull.Value)
										val = null;

									var p = cmd.CreateParameter();
									p.ParameterName = dbSqlite.FixedColumnName(c);
									p.Value = val;
									cmd.Parameters.Add(p);
								}

								cmd.ExecuteNonQuery();
							}
							tx.Commit();
						}
					}
				}
			}
			triggerOnPercentChange(new UpdateTaskResponse("", 0));
			return true;
		}

		private bool incrementalSyncDataTable2(DataTable table, List<String> indexes, string indWhere, string y = null) {
			if (table is null)
				return false;
			apiRest API = new apiRest();
			var columns = table.Columns.Cast<DataColumn>();
			var columnSql = string.Join(", ", columns.Select(x => dbSqlite.FixedColumnName(x)));
			if (y != null) { columnSql = "YEAR, " + columnSql; }
			var paramSql = string.Join(", ", columns.Select(x => "@" + dbSqlite.FixedColumnName(x)));
			if (y != null) { paramSql = "@YEAR, " + paramSql; }
			var tableName = dbSqlite.FixedTableName(table);
			var insertSql = $"INSERT OR REPLACE INTO {tableName} ({columnSql}) VALUES ({paramSql})";

			using (var conn = dbSqlite.GetConnection()) {
				int it = 0;
				foreach (DataRow row in table.Rows) {
					it++;
					int percent = it * 100 / table.Rows.Count;
					triggerOnPercentChange(new UpdateTaskResponse("", percent));

					string[] indexItems = new string[indexes.Count];
					for (var i = 0; i < indexes.Count; i++) {
						indexItems[i] = row[indexes[i]].ToString();
					}

					string where = string.Format(indWhere, indexItems).Replace("WHERE ", "");
					if (y != null)
						where += " AND `YEAR`='" + y + "'";

					string query = $"SELECT * FROM {tableName} WHERE {where};";
					//var cmd = new SQLiteCommand(stm, conn);
					//int RowCount = Convert.ToInt32(cmd.ExecuteScalar());

					DataTable sqlItems = dbSqlite.Select(query);

					bool inse = false;
					if (sqlItems.Rows.Count > 0) {
						// **** COMPARE
						foreach (var c in columns) {
							string columnName = dbSqlite.FixedColumnName(c);
							var val = row[c];
							if (c.AllowDBNull && val == DBNull.Value) {
								val = null;
							} else if (val.GetType().Name == "Byte[]") {
								val = "";
							}
							var valsqlite = sqlItems.Rows[0][columnName];
							if (c.AllowDBNull && valsqlite == DBNull.Value)
								valsqlite = null;

							if (val == null && valsqlite == null) { } else {
								if (dbSqlite.FixedColumnName(c) != "YEAR" && !val.Equals(Convert.ChangeType(valsqlite, val.GetType()))) {
									//if ((dynamic)val != (dynamic)valsqlite) { 
									//if (!val.Equals(valsqlite)) {
									inse = true;
									try {
										if (y != null) {
											if (!row.Table.Columns.Contains("YEAR"))
												row.Table.Columns.Add("YEAR");
											row["YEAR"] = Properties.Settings.Default.year;
										}
										bool resp = API.REPLACE(table.TableName, where.Replace("`", "").Replace("'", ""), row);
										if (!resp)
											API.INSERT(table.TableName, row);

									} catch (Exception ex) {
										return false;
									}
									break;
								}
							}
						}
					} else {
						inse = true;
						try {
							if (y != null) {
								if (!row.Table.Columns.Contains("YEAR"))
									row.Table.Columns.Add("YEAR");
								row["YEAR"] = Properties.Settings.Default.year;
							}
							API.INSERT(table.TableName, row);
						} catch (Exception ex) {
							return false;
						}
					}

					if (inse) {
						// **** INSERT
						using (var tx = conn.BeginTransaction()) {
							using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn, tx)) {

								if (y != null) {
									var p = cmd.CreateParameter();
									p.ParameterName = "YEAR";
									p.Value = y;
									cmd.Parameters.Add(p);
								}
								foreach (var c in columns) {
									var val = row[c];
									if (c.AllowDBNull && val == DBNull.Value)
										val = null;

									var p = cmd.CreateParameter();
									p.ParameterName = dbSqlite.FixedColumnName(c);
									p.Value = val;
									cmd.Parameters.Add(p);
								}

								cmd.ExecuteNonQuery();
							}
							tx.Commit();
						}
					}
				}
			}
			triggerOnPercentChange(new UpdateTaskResponse("", 0));
			return true;
		}

		private void RunPhotos() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);

				Mdb dbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension);
				DataTable dtFactu = dbFactu.Query("SELECT CODART, IMGART FROM F_ART WHERE FAMART<>'';");
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Actualizando Fotos")), null);
				int itt = 0;
				foreach (DataRow dtaRow in dtFactu.Rows) {
					int percent = itt * 100 / dtFactu.Rows.Count;
					triggerOnPercentChange(new UpdateTaskResponse("", percent));
					itt++;
					if (dtaRow[1].ToString() != "") {
						if (File.Exists(dtaRow[1].ToString())) {
							try {
								this.API.UPLOAD(dtaRow[0].ToString(), dtaRow[1].ToString());
							} catch (Exception ex) {
								SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
								Debug(ex.Message);
							}

						}
					}
				}
				triggerOnPercentChange(new UpdateTaskResponse("", 0));
				Stop();
			}
		}
		
		private bool importDataTable(DataTable table, string y = null) {
			if (table is null)
				return false;
			apiRest API = new apiRest();
			var columns = table.Columns.Cast<DataColumn>();
			var columnSql = string.Join(", ", columns.Select(x => dbSqlite.FixedColumnName(x)));
			if (y != null) { columnSql = "YEAR, " + columnSql; }
			var paramSql = string.Join(", ", columns.Select(x => "@" + dbSqlite.FixedColumnName(x)));
			if (y != null) { paramSql = "@YEAR, " + paramSql; }
			var tableName = dbSqlite.FixedTableName(table);
			var insertSql = $"INSERT OR REPLACE INTO {tableName} ({columnSql}) VALUES ({paramSql})";

			using (var conn = dbSqlite.GetConnection()) {
				
				int it = 0;
				using (var tx = conn.BeginTransaction()) {
					using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn, tx)) {


						foreach (DataRow row in table.Rows) {
							it++;
							int percent = it * 100 / table.Rows.Count;
							triggerOnPercentChange(new UpdateTaskResponse("", percent));


								//try {
								//	if (y != null) {
								//		if (!row.Table.Columns.Contains("YEAR"))
								//			row.Table.Columns.Add("YEAR");
								//		row["YEAR"] = Properties.Settings.Default.year;
								//	}
								//} catch (Exception ex) {
								//	return false;
								//}
					

								// **** INSERT
						

								if (y != null) {
									var p = cmd.CreateParameter();
									p.ParameterName = "YEAR";
									p.Value = y;
									cmd.Parameters.Add(p);
								}
								foreach (var c in columns) {
									var val = row[c];
									if (c.AllowDBNull && val == DBNull.Value)
										val = null;
										if (c.DataType.FullName.Contains("Byte")) {
											val = "";
										}
									var p = cmd.CreateParameter();
									p.ParameterName = dbSqlite.FixedColumnName(c);
									p.Value = val;
									cmd.Parameters.Add(p);
								}

								cmd.ExecuteNonQuery();
							}
							tx.Commit();
						}
				}
				conn.Close();
			}
			triggerOnPercentChange(new UpdateTaskResponse("", 0));
			return true;
		}
		private void RunTotal() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);

				#region "Obtenemos datos de los modelos a actualizar"
				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}
				#endregion

				#region "Buscamos los años de Factusol de los que disponemos"
				List<string> years = new List<string>();
				DateTime now = DateTime.Today;
				for (var x = int.Parse(now.ToString("yyyy")); x > 2000; x--) {
					string fileName = Properties.Settings.Default.path + Properties.Settings.Default.company + x.ToString() + Properties.Settings.Default.dbExtension;
					if (File.Exists(fileName))
						years.Add(x.ToString());
				}
				#endregion

				#region "Pasamos por años -> tablas para actualizar la base de datos local"
				years.ForEach(delegate (String year) {
					SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("(" + year + ") Leyendo tablas.")), null);
					//DataSet dbFactu = this.LoadFromFile(Properties.Settings.Default.path + Properties.Settings.Default.company + year + Properties.Settings.Default.dbExtension, tablesForSync, year == now.ToString("yyyy"));

					Mdb mdbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + year + Properties.Settings.Default.dbExtension);
					List<string> tables = mdbFactu.GetTablesNames();

					foreach (string table in tables) {

						DataRow[] found = tablesForSync.Select("name='" + table + "'");
						if (found.Length > 0) {
							SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("(" + year + ") Leyendo " + table)), null);

							Debug("Init update table - " + table + "!");

							string y = null;
							if (found[0]["byYears"].ToString() == "1")
								y = year;

							if (year == years[0] || y != null) {
								DataTable dtFactu = mdbFactu.Query("SELECT * FROM " + table + ";");
								dtFactu.TableName = table;

								if (year == years[0]) {
									bool droptableSql = dbSqlite.dropDataTable(dtFactu);
								}
								SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("(" + year + ") Actualizando " + table)), null);
								if (found[0]["cols"].ToString() != "") {
									List<DataColumn> r = new List<DataColumn>();
									string[] columnsForSync = ((string[])found[0]["cols"]);
									foreach (var dataColumn in dtFactu.Columns.Cast<DataColumn>()) {
										if (columnsForSync.Length > 0) {
											if (!columnsForSync.Contains(dataColumn.ColumnName)) {
												r.Add(dataColumn);
											}
										}
									}
									foreach (var dt in r)
										dtFactu.Columns.Remove(dt);
								}

								bool createtableSql = dbSqlite.ImportTable(dtFactu, ((string[])found[0]["index"]).ToList(), y);
								bool insert = this.importDataTable(dtFactu, y);
							}
						}
					}
				});
				#endregion

				#region "Compresión de la base de datos en ZIP"
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Preparando datos")), null);
				var zipFile = @"db.zip";
				if (File.Exists(zipFile))
					File.Delete(zipFile);
				try {
					using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
						string fileName = Directory.GetCurrentDirectory() + "\\db.sqlite";
						//FileInfo fi = new FileInfo(fileName);
						
						string fileName2 = fileName.Replace("db.sqlite", "db2.sqlite");
						if (File.Exists(fileName2)) {
							File.Delete(fileName2);
						}
						File.Copy(fileName,fileName2);
						archive.CreateEntryFromFile(fileName2, "db.sqlite");
						File.Delete(fileName2);
						
					}
				} catch (Exception ex) {
					//System.Windows.Forms.MessageBox.Show("Ha sido imposible tener acceso al fichero.\r\nComrpueba que el fichero no está abierto en modo exlusivo por otro programa.", "Error - Acceso al fichero", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					Debug(ex.Message);
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}
				
				#endregion

				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Envíando datos")), null);
				if (File.Exists(zipFile)) {
					try {
						this.API.UPLOAD_DB(zipFile);
					} catch (Exception ex) {
						Debug(ex.Message);
						SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
						Stop();
						return;
					}
				}
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("")), null);
				Stop();
			}
		}


		private void RunClear() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);

				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}

				foreach (DataRow table in tablesForSync.Rows) {
					try {
						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Eliminando " + table["name"].ToString() )), null);
						this.API.DESTROY(table["name"].ToString());
					} catch (Exception ex) {
						SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
						Stop();
						return;
					}
				}
				//if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\db.sqlite"))
				//	System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\db.sqlite");

				Stop();
			}
		}

		private void RunClearOnlyCache() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);
				if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\db.sqlite"))
					System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\db.sqlite");

				Stop();
			}
		}

		//public void RunClearAsync() {			
		//		DataTable tablesForSync = new DataTable();
		//		try {
		//			tablesForSync = API.listModels();
		//		} catch (Exception ex) {
		//			return;
		//		}


		//		foreach (DataRow table in tablesForSync.Rows) {
		//			try {
		//				this.API.DESTROY(table["name"].ToString());
		//			} catch (Exception ex) {
		//				return;
		//			}
		//		}
		//		if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\db.sqlite")
		//			System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\db.sqlite");


		//}

		// Métodos que ejecutan los callback si y solo si fueron declarados durante la instanciación de la clase HeavyTask
		
		
		
		
		
		
		private void triggerOnInitUpdate(UpdateTaskResponse response) {
			CallbackOnInitUpdate?.Invoke(this, response);
		}

		private void triggerOnFinishUpdate(UpdateTaskResponse response) {
			CallbackOnFinishUpdate?.Invoke(this, response);
		}

		private void triggerOnStatusChange(UpdateTaskResponse response) {
			CallbackOnStatusChange?.Invoke(this, response);
		}

		private void triggerOnPercentChange(UpdateTaskResponse response) {
			CallbackOnPercentChange?.Invoke(this, response);
		}

		private void triggerOnError(UpdateTaskResponse response) {
			CallbackOnError?.Invoke(this, response);
		}



		private void Debug(string message) {
			if (this.debugLevel > 0) {
				string TXTmessage = string.Format("{0} - Info - {1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), message);
				System.Diagnostics.Debug.WriteLine(TXTmessage);
				Console.WriteLine(TXTmessage);
			}
		}

		

	}
}
