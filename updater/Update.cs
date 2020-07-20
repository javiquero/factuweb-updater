using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace updaterFactuWeb {
	public class UpdateTaskResponse {
		private readonly string message;
		private readonly Int32 value;

		public UpdateTaskResponse(string msg, Int32 v = 0) {
			this.message = msg;
			this.value = v;
		}

		public string Message { get { return message; } }
		public Int32 Value { get { return value; } }
	}

	class Update {
		
		private readonly Mdb dbFactu;
		private readonly Mdb dbCache;
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

		public Update() {
			this.dbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension);

			if (!File.Exists(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension))
				createLocalCacheDb();

			this.dbCache = new Mdb(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension);

			this.API = new apiRest();

			SyncContext = AsyncOperationManager.SynchronizationContext;
		}

		// Método para iniciar el proceso
		public void Start() {
			Thread thread = new Thread(Run);
			thread.IsBackground = true;
			thread.Start();
		}
		public void StartJson() {
			Thread thread = new Thread(RunCreateJson);
			thread.IsBackground = true;
			thread.Start();
		}

		public void StartClear() {
			Thread thread = new Thread(RunClear);
			thread.IsBackground = true;
			thread.Start();
		}

		// Método para detener el proceso
		public void Stop() {
			UpdateProcessStopped = true;
			SyncContext.Post(e => triggerOnFinishUpdate(new UpdateTaskResponse("")), null);
		}

		// Método donde la lógica principal de tu tarea se ejecuta
		private void Run() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);
				List<string> tables = dbFactu.GetTablesNames();
				DataTable dtFactu;

				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}

				//foreach (string table in tables) {

				//	DataRow[] found = tablesForSync.Select("name='" + table + "'");
				//	if (found.Length > 0) {
				//		SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Actualizando " + table)), null);
				//		Debug("Init update table - " + table + "!");
				//		bool y = false;
				//		if (found[0]["byYears"].ToString() == "1")
				//			y = true;



				//		bool existsTable = dbCache.ExistsTable(table);
				//		if (!existsTable)
				//			copySchemaTable(table, y);

				//		dtFactu = this.dbFactu.Query("SELECT * FROM " + table + ";");

				//		string querySELECT = "SELECT * FROM " + table;
				//		if (y)
				//			querySELECT += " WHERE YEAR='" + Properties.Settings.Default.year + "'";
				//		DataTable dtCache = this.dbCache.Query(querySELECT + ";");

				//		List<String> indexes = this.dbFactu.GetIndexes(table);
				//		string indWhere = this.dbFactu.GetWhereStringConditionByIndexes(table);
				//		int it = 0;
				//		foreach (DataRow dtaRow in dtFactu.Rows) {
				//			// Rows

				//			it++;
				//			string[] indexItems = new string[indexes.Count];
				//			for (var i = 0; i < indexes.Count; i++) {
				//				indexItems[i] = dtaRow[indexes[i]].ToString();
				//			}

				//			int percent = it * 100 / dtFactu.Rows.Count;
				//			triggerOnPercentChange(new UpdateTaskResponse("", percent));

				//			string where = string.Format(indWhere, indexItems).Replace("WHERE ", "");

				//			DataRow[] result = dtCache.Select(where);
				//			if (result.Length > 0) {
				//				var r = dtaRow.ItemArray.Except(result[0].ItemArray).ToArray();
				//				string updateQuery = "";
				//				if (r.Length != 0) {
				//					for (var i = 0; i < dtaRow.ItemArray.Length; i++) {
				//						//Columns

				//						bool isDiferent = false;
				//						int ii = y == true ? i + 1 : i;

				//						string columnName = dtaRow.Table.Columns[i].ColumnName;
				//						Type columnType = dtaRow.Table.Columns[i].DataType;

				//						if (columnType.Name == "Decimal") {
				//							if (Decimal.Parse(dtaRow.ItemArray[i].ToString()) != Decimal.Parse(result[0].ItemArray[ii].ToString())) {
				//								isDiferent = true;
				//								updateQuery += "`" + columnName + "`= " + dtaRow[columnName].ToString().Replace(",", ".") + ", ";
				//							}
				//						} else if (columnType.Name == "Byte[]") {
				//							Debug("De momento pasamos del tipo Byte");
				//						} else {
				//							if (dtaRow[columnName].ToString().Replace("'", "`").Replace("\"", "`") != Convert.ChangeType(result[0][columnName], dtaRow.Table.Columns[i].DataType).ToString().Replace("'", "`").Replace("\"", "`")) {
				//								isDiferent = true;
				//								updateQuery += "`" + columnName + "`= '" + dtaRow[columnName].ToString() + "', ";
				//							}
				//						}
				//						//if (isDiferent == true) {
				//						//	Debug("Diferente campo " + columnName + " " + where);
				//						//	try {
				//						//		string replaceJson = this.dbFactu.GetStringJson(table, dtaRow, y);
				//						//		this.API.REPLACE(table, where.Replace("`", "").Replace("'", ""), replaceJson);
				//						//	} catch (Exception ex) {
				//						//		SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//						//		Stop();
				//						//		return;
				//						//	}

				//						//	string updateQuery = this.dbFactu.GetStringUpdate(table, dtaRow, y);
				//						//	try {
				//						//		this.dbCache.Execute(updateQuery);
				//						//	} catch (Exception ex) {
				//						//		SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//						//		Stop();
				//						//		return;
				//						//	}

				//						//	break;
				//						//}
				//					}
				//					if (updateQuery != "") {
				//						try {
				//							string replaceJson = this.dbFactu.GetStringJson(table, dtaRow, y);
				//							this.API.REPLACE(table, where.Replace("`", "").Replace("'", ""), replaceJson);
				//						} catch (Exception ex) {
				//							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//							Stop();
				//							return;
				//						}

				//						updateQuery = "UPDATE " + table + " Set " + updateQuery.Remove(updateQuery.Length - 2) + " WHERE " + where + ";";
				//						try {
				//							this.dbCache.Execute(updateQuery);
				//						} catch (Exception ex) {
				//							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//							Stop();
				//							return;
				//						}
				//					}
				//				}
				//			} else {
				//				// No existe el registro
				//				string insertJson = this.dbFactu.GetStringJson(table, dtaRow, y);
				//				bool apiResponse = false;
				//				try {
				//					apiResponse = this.API.INSERT(table, insertJson);
				//				} catch (Exception ex) {
				//					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//					Stop();
				//					return;
				//				}

				//				string insertQuery = this.dbFactu.GetStringInsert(table, dtaRow, y);
				//				try {
				//					this.dbCache.Execute(insertQuery);
				//				} catch (Exception ex) {
				//					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//					Stop();
				//					return;
				//				}
				//			}
				//		}

				//		Debug("Init update table - " + table + "!");
				//		foreach (DataRow dtaRow in dtCache.Rows) {

				//			string[] indexItems = new string[indexes.Count];
				//			for (var i = 0; i < indexes.Count; i++) {
				//				indexItems[i] = dtaRow[indexes[i]].ToString();
				//			}
				//			string where = string.Format(indWhere, indexItems).Replace("WHERE ", "");

				//			DataRow[] result = dtFactu.Select(where);
				//			if (result.Length == 0) {
				//				Debug("DELETE " + where);
				//			}
				//		}
				//	}
				//}

				dtFactu = this.dbFactu.Query("SELECT CODART, IMGART FROM F_ART WHERE FAMART<>'';");
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
								Debug(ex.Message);
							}

						}
					}
				}
				Stop();
			}
		}

		private void RunCreateJson() {
			while (!UpdateProcessStopped) {
				SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse("")), null);

				List<string> tables = dbFactu.GetTablesNames();
				DataTable dtFactu;

				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
					Stop();
					return;
				}

				//foreach (DataRow table in tablesForSync.Rows) {
				//	try {
				//		SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Eliminando " + table["name"].ToString())), null);
				//		this.API.DESTROY(table["name"].ToString());
				//	} catch (Exception ex) {
				//		SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
				//		Stop();
				//		return;
				//	}
				//}
				//if (System.IO.File.Exists(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension)) {
				//	System.IO.File.Delete(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension);
				//	createLocalCacheDb();


				//}

				if (System.IO.File.Exists(@"data.json")) {
					System.IO.File.Delete(@"data.json");
				}
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"data.json", true)) { 
					
				file.WriteLine("{");
					int itt = 0;
					//string[] tabless = { "F_AGE", "F_ART", "F_CLI" };

					foreach (string table in tables) {
					//string table = "F_AGE";

					DataRow[] found = tablesForSync.Select("name='" + table + "'");
					if (found.Length > 0) {
						itt++;
						string tx = '"' + table + '"' + " :[";

						if (itt > 1)
							tx = ", " + tx;
						file.WriteLine(tx);

						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Actualizando " + table)), null);
						Debug("Init update table - " + table + "!");
						bool y = false;
						if (found[0]["byYears"].ToString() == "1")
							y = true;

						bool existsTable = dbCache.ExistsTable(table);
						if (!existsTable)
							copySchemaTable(table, y);

						dtFactu = this.dbFactu.Query("SELECT * FROM " + table + ";");
						List<String> indexes = this.dbFactu.GetIndexes(table);

						int it = 0;
							List<string> insertQuery = new List<string>();
						foreach (DataRow dtaRow in dtFactu.Rows) {
							// Rows
							it++;

							//string[] indexItems = new string[indexes.Count];
							//for (var i = 0; i < indexes.Count; i++) {
							//	indexItems[i] = dtaRow[indexes[i]].ToString();
							//}

							int percent = it * 100 / dtFactu.Rows.Count;
							triggerOnPercentChange(new UpdateTaskResponse("", percent));

							string insertJson = this.dbFactu.GetStringJson2(table, dtaRow, y);
							
							if (it > 1)
								insertJson = ", " + insertJson;
							file.WriteLine(insertJson);

							 insertQuery.Add( this.dbFactu.GetStringInsert(table, dtaRow, y));
							
						}
							SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Realizando cambios en " + table)), null);
							try {
								this.dbCache.Execute(insertQuery);
							} catch (Exception ex) {
								SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(ex.Message)), null);
								Stop();
								return;
							}

							file.WriteLine("]");
							file.Flush();
					}

				}


				file.WriteLine("}");
			}
				//dtFactu = this.dbFactu.Query("SELECT CODART, IMGART FROM F_ART WHERE FAMART<>'';");
				//SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse("Actualizando Fotos")), null);
				//int itt = 0;
				//foreach (DataRow dtaRow in dtFactu.Rows) {
				//	int percent = itt * 100 / dtFactu.Rows.Count;
				//	triggerOnPercentChange(new UpdateTaskResponse("", percent));
				//	itt++;
				//	if (dtaRow[1].ToString() != "") {
				//		if (File.Exists(dtaRow[1].ToString())) {
				//			try {
				//				this.API.UPLOAD(dtaRow[0].ToString(), dtaRow[1].ToString());
				//			} catch (Exception ex) {
				//				Debug(ex.Message);
				//			}

				//		}
				//	}
				//}
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
				if (System.IO.File.Exists(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension))
					System.IO.File.Delete(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension);

				Stop();
			}
		}

		public void RunClearAsync() {			
				DataTable tablesForSync = new DataTable();
				try {
					tablesForSync = API.listModels();
				} catch (Exception ex) {
					return;
				}


				foreach (DataRow table in tablesForSync.Rows) {
					try {
						this.API.DESTROY(table["name"].ToString());
					} catch (Exception ex) {
						return;
					}
				}
				if (System.IO.File.Exists(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension))
					System.IO.File.Delete(Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension);


		}

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

		private void copySchemaTable(string tableName, bool y ) {
			string createQuery = dbFactu.GetStringCreateTable(tableName,y);
			if (createQuery != "") dbCache.Execute(createQuery);

			string primaryQuery = dbFactu.GetStringPrimaryKeys(tableName);
			if (primaryQuery != "") dbCache.Execute(primaryQuery);

			List<string> keysQuery = dbFactu.GetStringKeys(tableName);
			if (keysQuery.Count > 0) dbCache.Execute(keysQuery);

		}

		private void Debug(string message) {
			if (this.debugLevel > 0) {
				string TXTmessage = string.Format("{0} - Info - {1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), message);
				System.Diagnostics.Debug.WriteLine(TXTmessage);
				Console.WriteLine(TXTmessage);
			}
		}

		private bool createLocalCacheDb() {
			string filePath = Properties.Settings.Default.dbCacheName + Properties.Settings.Default.dbExtension;
			ADOX.Catalog catalog = new ADOX.Catalog();
			if (!File.Exists(filePath)) {
				try {
					//catalog.Create(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Engine Type=5", filePath));
					catalog.Create("Provider = Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";;Jet OLEDB:Engine Type=5;");
				} catch (System.Exception) {
					return false;
				}
			}
			return true;
		}

	}
}
