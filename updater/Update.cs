
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace updaterFactuWeb {
	public class UpdateTaskResponse {
		private readonly string message;
		private readonly Int32 value;
		private readonly string type;

		public UpdateTaskResponse(string t = "", string msg="", Int32 v = 0) {
			this.message = msg;
			this.value = v;
			this.type = t;
		}

		public string Type { get { return type; } }
		public string Message { get { return message; } }
		public Int32 Value { get { return value; } }
	}

    internal class Update : IDisposable {
		
		private readonly Sqlite dbSqlite;
		private readonly apiRest API;
		private string type = "";

		// Bandera booleana que indica cuando el proceso está siendo ejecutado o ha sido detenido
		public bool UpdateProcessStopped = true;

		// Expone el contexto de sincronización en la clase entera 
		private readonly SynchronizationContext SyncContext;

		public event EventHandler<UpdateTaskResponse> CallbackOnInitUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnFinishUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnStatusChange;
		public event EventHandler<UpdateTaskResponse> CallbackOnError;
		public event EventHandler<UpdateTaskResponse> CallbackOnPercentChange;

		public Update(bool reset = false) {			
			this.dbSqlite = new Sqlite(reset);
			this.API = new apiRest();
			SyncContext = AsyncOperationManager.SynchronizationContext;
		}

		public void StartIncremental() {
			UpdateProcessStopped = false;
			type = "incremental";
			Thread thread = new Thread(RunIncremental);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartTotal() {
			UpdateProcessStopped = false;
			type = "total";
			Thread thread = new Thread(RunTotal);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartPhotos() {
			UpdateProcessStopped = false;
			type = "fotos";
			Thread thread = new Thread(RunPhotos);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartClear() {
			UpdateProcessStopped = false;
			type = "clear";
			Thread thread = new Thread(RunClear);
			thread.IsBackground = true;
			thread.Start();
		}
		public void StartClearOnlyCache() {
			UpdateProcessStopped = false;
			type = "clear";
			Thread thread = new Thread(RunClearOnlyCache);
			thread.IsBackground = true;
			thread.Start();
		}

		public void Stop() {
			UpdateProcessStopped = true;
			SyncContext.Post(e => triggerOnFinishUpdate(new UpdateTaskResponse(type)), null);
		}
		
		private void RunIncremental() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse(type)), null);

				#region "Obtenemos datos de los modelos a actualizar"
				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					Logger.log(ex);
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
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
					if (UpdateProcessStopped)
						break;
					//string tableName = table.TableName.Replace("[", "").Replace("]", "");

					DataRow[] found = tablesForSync.Select("name='" + table + "'");
					if (found.Length > 0) {

						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type,"(" + Properties.Settings.Default.year + ") Actualizando " + table)), null);
						Logger.log("Init update table - " + table + "!");

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
						
						string createtableSql = dbSqlite.ImportTable(dtFactu, indexes, y);
						if (createtableSql!="") {
							API.EXECUTE(createtableSql);
						}
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
			var tableName = Sqlite.FixedTableName(table);
			var insertSql = $"INSERT OR REPLACE INTO {tableName} ({columnSql}) VALUES ({paramSql})";

			using (var conn = dbSqlite.GetConnection()) {
				string where = "";

				if (y != null) where = "WHERE `YEAR`='" + y + "'";

				string query = $"SELECT * FROM {tableName} {where};";
				DataTable sqlItems = dbSqlite.Select(query);

				int it = 0;
				foreach (DataRow row in table.Rows) {
					if (UpdateProcessStopped)
						break;
					it++;
					int percent = it * 50 / table.Rows.Count;
					triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));

					string[] indexItems = new string[indexes.Count];
					for (var i = 0; i < indexes.Count; i++) {
						indexItems[i] = row[indexes[i]].ToString();
					}

					where = string.Format(indWhere, indexItems).Replace("WHERE ", "");

					//if (where == "`CODART`=2402"){ var t = where;}

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


							if (val != null || valsqlite != null) {
								if (columnName != "YEAR" && val.ToString().All(char.IsNumber)) valsqlite = val;
								if (dbSqlite.FixedColumnName(c) != "YEAR" && !val.Equals(Convert.ChangeType(valsqlite, val.GetType()))) {
									inse = true;
									break;
								}
							}
						}
					} else {
						inse = true;
					}

					if (inse) {
						// **** INSERT
						try {
							if (y != null) {
								if (!row.Table.Columns.Contains("YEAR"))
									row.Table.Columns.Add("YEAR");
								row["YEAR"] = Properties.Settings.Default.year;
							}
							API.INSERT(table.TableName, row);
						} catch (Exception ex) {
							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
							Logger.log(ex);
							return false;
						}

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

				DataRow[] sqliteItems = sqlItems.Select();
				if (y != null) 
					sqliteItems = sqlItems.Select("`YEAR`='" + y + "'");
				
				
				// DELETES
				it = 0;
				foreach (DataRow row in sqliteItems) {
					it++;
					int percent =(50)+( it * 50 / (table.Rows.Count));
					triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));

					string[] indexItems = new string[indexes.Count];
					for (var i = 0; i < indexes.Count; i++) {
						indexItems[i] = row[indexes[i]].ToString();
					}

					where = string.Format(indWhere, indexItems).Replace("WHERE ", "");
					DataRow[] result = table.Select(where);

					if (result.Length < 1) {
						// **** DELETE
						try {
							if (y != null) {
								if (!row.Table.Columns.Contains("YEAR"))
									row.Table.Columns.Add("YEAR");
								row["YEAR"] = Properties.Settings.Default.year;
							}
							API.DELETE(table.TableName, where);
						} catch (Exception ex) {
							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
							Logger.log(ex);
							return false;
						}
						using (var tx = conn.BeginTransaction()) {
							if (y != null)
								where = where + " AND `YEAR`='" + y + "'";
							using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM " + table.TableName + " WHERE " + where + ";", conn, tx)) {
								cmd.ExecuteNonQuery();
							}
							tx.Commit();
						}

					}
				}
			}		
			triggerOnPercentChange(new UpdateTaskResponse(type, "", 0));
			return true;
		}

		private void RunPhotos() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse(type, "")), null);

				Mdb dbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension);
				DataTable dtFactu = dbFactu.Query("SELECT CODART, IMGART FROM F_ART WHERE FAMART<>'';");
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Actualizando Fotos")), null);
				int itt = 0;
				foreach (DataRow dtaRow in dtFactu.Rows) {
					if (!UpdateProcessStopped) {
						int percent = itt * 100 / dtFactu.Rows.Count;
						triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));
						itt++;
						if (dtaRow[1].ToString() != "") {
							if (File.Exists(dtaRow[1].ToString())) {
								try {
									this.API.UPLOAD(dtaRow[0].ToString(), dtaRow[1].ToString());
								} catch (Exception ex) {
									SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
									Logger.log(ex);
									if (ex.Message.Contains("Forbidden") == true) {
										SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
										triggerOnPercentChange(new UpdateTaskResponse(type, "", 0));
										Stop();
									}
								}

							}
						}
					}
				}

				dtFactu = dbFactu.Query("SELECT CODIMG, IMGIMG FROM F_IMG WHERE FAMIMG<>'';");
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Actualizando Fotos con acabados")), null);
				itt = 0;
				foreach (DataRow dtaRow in dtFactu.Rows) {
					if (!UpdateProcessStopped) {
						int percent = itt * 100 / dtFactu.Rows.Count;
						triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));
						itt++;
						if (dtaRow[1].ToString() != "") {
							if (File.Exists(dtaRow[1].ToString())) {
								try {
									this.API.UPLOAD(dtaRow[0].ToString(), dtaRow[1].ToString());
								} catch (Exception ex) {
									SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
									Logger.log(ex);
									if (ex.Message.Contains("Forbidden") == true) {
										SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
										triggerOnPercentChange(new UpdateTaskResponse(type, "", 0));
										Stop();
									}
								}

							}
						}
					}
				}
				triggerOnPercentChange(new UpdateTaskResponse(type, "", 0));
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
			var tableName = Sqlite.FixedTableName(table);
			var insertSql = $"INSERT OR REPLACE INTO {tableName} ({columnSql}) VALUES ({paramSql})";

			using (var conn = dbSqlite.GetConnection()) {
				
				int it = 0;
				using (var tx = conn.BeginTransaction()) {
					using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn, tx)) {


						foreach (DataRow row in table.Rows) {
							if (!UpdateProcessStopped) {
								it++;
								int percent = it * 100 / table.Rows.Count;
								triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));

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
								try {
									cmd.ExecuteNonQuery();
								} catch (SQLiteException ex) {
									SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
									Stop();
									return false;
								}
							}
						}
						tx.Commit();
					}
				}
				conn.Close();
			}
			triggerOnPercentChange(new UpdateTaskResponse(type, "", 0));
			return true;
		}

		private void RunTotal() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse(type, "")), null);

				#region "Obtenemos datos de los modelos a actualizar"
				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					Logger.log(ex);
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
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
					if (!UpdateProcessStopped) {
						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "(" + year + ") Leyendo tablas.")), null);
						//DataSet dbFactu = this.LoadFromFile(Properties.Settings.Default.path + Properties.Settings.Default.company + year + Properties.Settings.Default.dbExtension, tablesForSync, year == now.ToString("yyyy"));

						Mdb mdbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + year + Properties.Settings.Default.dbExtension);
						List<string> tables = mdbFactu.GetTablesNames();

						foreach (string table in tables) {
							if (!UpdateProcessStopped) {
								DataRow[] found = tablesForSync.Select("name='" + table + "'");
								if (found.Length > 0) {
									SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "(" + year + ") Leyendo " + table)), null);

									Logger.log("Init update table - " + table + "!");

									string y = null;
									if (found[0]["byYears"].ToString() == "1")
										y = year;

									if (year == years[0] || y != null) {
										DataTable dtFactu = mdbFactu.Query("SELECT * FROM " + table + ";");
										dtFactu.TableName = table;

										if (year == years[0]) {
											bool droptableSql = dbSqlite.dropDataTable(dtFactu);
										}
										SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "(" + year + ") Actualizando " + table)), null);
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

										string createtableSql = dbSqlite.ImportTable(dtFactu, ((string[])found[0]["index"]).ToList(), y);
										bool insert = this.importDataTable(dtFactu, y);
									}
								}
							}
						}
					}

				});
				#endregion
				if (!UpdateProcessStopped) {
					#region "Compresión de la base de datos en ZIP"
					SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Preparando datos")), null);
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
							File.Copy(fileName, fileName2);
							archive.CreateEntryFromFile(fileName2, "db.sqlite");
							File.Delete(fileName2);

						}
					} catch (Exception ex) {
						//System.Windows.Forms.MessageBox.Show("Ha sido imposible tener acceso al fichero.\r\nComrpueba que el fichero no está abierto en modo exlusivo por otro programa.", "Error - Acceso al fichero", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
						Logger.log(ex);
						SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
						Stop();
						return;
					}
					#endregion

					SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Envíando datos")), null);
					if (File.Exists(zipFile)) {
						try {
							this.API.UPLOAD_DB(zipFile);
						} catch (Exception ex) {
							Logger.log(ex);
							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
							Stop();
							return;
						}
					}
				}
				SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "")), null);
				Stop();
			}
		}

		private void RunClear() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse(type, "")), null);

				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
					Stop();
					return;
				}

				foreach (DataRow table in tablesForSync.Rows) {
					try {
						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Eliminando " + table["name"].ToString() )), null);
						this.API.DESTROY(table["name"].ToString());
					} catch (Exception ex) {
						SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
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

        public void Dispose() {
            throw new NotImplementedException();
        }
	}
}
