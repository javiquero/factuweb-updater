using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace updaterFactuWeb {
	class apiRestOLD {
		private HttpClient _httpClient = new HttpClient();
		private string apiToken;
		private string apiURL;

		public apiRestOLD() {

			apiToken = Properties.Settings.Default.token;
			apiURL = Properties.Settings.Default.api;

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

			// CREATE
			//string _json = "{CODCLI: 556, NOCCLI:'NOMBRE DE PRUEBA 556'}";
			//bool response = Post("fcli", _json);

			// FIND
			//dynamic result = Get("fcli", "CODCLI=556");
			//Console.WriteLine(result);


			//POST /:model
			//DELETE /:model /:id
			//GET /:model /:id
			//GET /:model
			//	?where ={ "name":{ "contains":"theodore"} }
			//	?limit = 100
			//	?skip=30
			//	?sort=lastName%20ASC
			//	? select = name, age
			//	?omit=favoriteColor,address
			//PATCH /:model/:id

			// UPLOAD /image/upload/:codart 
		}

		public DataTable listModels() {
			try {
				string json = this.Get("models/list", "");
				json = @"{'modelsList': " + json + "}";
				DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
				DataTable dataTable = dataSet.Tables["modelsList"];
				return dataTable;
				//return JsontoDataTable(json);
			} catch (Exception ex) {
				throw (ex);
			}
		}

		public DataTable SELECT(string model, string query = "" ) {
			try {
				model = model.Replace("_", "").ToLower();
				if (query != "" && !query.StartsWith("?")) query = "?" + query;
				string json = this.Get(model, query);
				json = "{ \""+ model + "\": " + json + "}";
				DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json.Replace("\n",""));
				DataTable dataTable = dataSet.Tables[model];
				return dataTable;
				//return JsontoDataTable(json);
			} catch (Exception ex) {
				throw (ex);				
			}
		}
	public bool INSERT(string model, DataRow data) {
			try {
				//model = model.Replace("_", "").ToLower();
				string json = this.DataRowToJSON(data);
				//string json = "{";
				//foreach (DataColumn col in data.Table.Columns) {
				//	if (json != "{") json += ", ";
				//	json += "\"" + col.ColumnName + "\":";
				//	if (col.DataType == System.Type.GetType("System.String")) {
				//		json += "\"" + data[col.ColumnName] + "\"";
				//	} else {
				//		json += data[col.ColumnName].ToString().Replace(",",".");
				//	}
				//}

				//json += "}";
				//string json = JsonConvert.SerializeObject(data., Formatting.Indented);
				return this.Post("{model: \"" + model + "\", data:" + json+ "}");
			} catch (Exception ex) {
				throw(ex);
			}
		}

		public bool INSERT(string model, string json) {
			try {
				model = model.Replace("_", "").ToLower();
				return this.Post(model, json);
			} catch (Exception ex) {
				throw (ex);
			}
		}

		public bool DESTROY(string model) {
			try {
				model = model.Replace("_", "").ToLower();

				string json = this.Get("models/" + model + "/destroy", "");
				return true;
			} catch (Exception ex) {
				throw (ex);
			}
		}

		public bool DELETE(string model, string id) {
			try {
				//model = model.Replace("_", "").ToLower();
				bool result = this.Delete(model, id);
				return result;
			} catch (Exception ex) {
				throw (ex);
			}
		}

		public bool REPLACE(string model, string where, string json) {
			try {
				model = model.Replace("_", "").ToLower();
				DataTable dt = this.SELECT(model, where);
				if (dt.Rows.Count > 0) {
					bool result = this.Patch(model, dt.Rows[0][dt.Rows[0].Table.Columns["id"].Ordinal].ToString() , json);
					return result;
				}
								return false;
			} catch (Exception ex) {
				throw ex;
			}
		}
		public bool REPLACE(string model, string where, DataRow data) {
			try {
				model = model.Replace("_", "").ToLower();

				DataTable dt = this.SELECT(model, where);
				if (dt.Rows.Count > 0) {
					string json = this.DataRowToJSON(data);// "{";
					//foreach (DataColumn col in data.Table.Columns) {
					//	if (json != "{")
					//		json += ", ";
					//	json += "\"" + col.ColumnName + "\":";
					//	if (col.DataType == System.Type.GetType("System.String")) {
					//		json += "\"" + data[col.ColumnName] + "\"";
					//	} else {
					//		json += data[col.ColumnName].ToString().Replace(",", ".");
					//	}
					//}
					//json += "}";
					bool result = this.Patch(model, dt.Rows[0][dt.Rows[0].Table.Columns["id"].Ordinal].ToString(), json);
					return result;
				}
				return false;

			} catch (Exception ex) {
				throw (ex);
			}
		}

		public bool UPLOAD_DB(string path) {
			if (path != "") {
				if (File.Exists(path)) {
					try {
						uploadfile("db/upload", path);
						return true;
					} catch (Exception ex) {
						throw (ex);
					}				
				}
			}
			return false;
		}

		public bool UPLOAD(string codart, string path) {
			if (path != "") {
				if (File.Exists(path)) {
					string md5 = CalculateMD5(path);
					string extension = Path.GetExtension(path);
					try {
						string response = Get("image/upload/exists", "?md5=" + md5 + extension+"&codart=" + codart);

						if (response != "OK") {
							try {
								uploadfile("image/upload/" + codart, codart, md5, path);
							} catch (Exception ex) {
								throw (ex);
							}
						}
					} catch (Exception ex ) {
						throw ex;
					}
					
				}
			}
			return false;
		}

		private string CalculateMD5(string filename) {
			using (var md5 = System.Security.Cryptography.MD5.Create()) {
				using (var stream = File.OpenRead(filename)) {
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}
		private string DataRowToJSON(DataRow row) {
			string[] types = new string[row.Table.Columns.Count];

			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = row.Table.Columns[i].DataType.ToString();
				//types[i] = this.GetTypeColumn(model, row.Table.Columns[i].ColumnName);
			}
			string response = "{";

			//if (y)
			//	response += "YEAR: '" + Properties.Settings.Default.year + "'";
			for (var i = 0; i < row.Table.Columns.Count; i++) {
				if (response != "{")
					response += ", ";
				response += row.Table.Columns[i].ColumnName + ":";
				if (types[i].Contains("String")) {
					response += "'" + row.ItemArray[i].ToString().Replace(@"\", @"\\").Replace(@"'", @"\'") + "'";
				} else if (types[i].Contains("DateTime")) {
					DateTime t = DateTime.ParseExact(row.ItemArray[i].ToString(), "dd/MM/yyyy H:mm:ss",
						System.Globalization.CultureInfo.InvariantCulture);
					response += "'" + t.ToString("yyyy-MM-dd HH:mm:ss") + "'";
				} else if (types[i] == "oleobject" || types[i].Contains("Byte")) {
					response += "''";
				} else {
					response += row.ItemArray[i].ToString().Replace(",", ".");
				}
			}
			return response + "}";
		}
		private string Get(string model, string queryString) {
			string _url = $"{apiURL}{model}";
			//if (queryString != "") _url += "/find?" + queryString;
			using (var result = _httpClient.GetAsync($"{_url}{queryString}").Result) {
				if (result.StatusCode == System.Net.HttpStatusCode.OK) {
					string content = result.Content.ReadAsStringAsync().Result;
					return content;
					//return JsonConvert.DeserializeObject(content);
				}
				string returnValue = result.Content.ReadAsStringAsync().Result;
				throw new Exception($"Failed to GET data: ({result.StatusCode}): {returnValue}");
			}
		}
		private bool Post(string model, string json) {
			string _url = $"{apiURL}{model}";
			dynamic jsonObject = JsonConvert.DeserializeObject(json);
			using (var content = new StringContent(JsonConvert.SerializeObject(jsonObject), System.Text.Encoding.UTF8, "application/json")) {
				HttpResponseMessage result = _httpClient.PostAsync (_url, content).Result;
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
					return true;
				string returnValue = result.Content.ReadAsStringAsync().Result;
				throw new Exception($"Failed to POST data: ({result.StatusCode}): {returnValue}");
			}
		}
		private bool Post( string json) {
			string _url = $"{apiURL}db/add";
			dynamic jsonObject = JsonConvert.DeserializeObject(json);
			using (var content = new StringContent(JsonConvert.SerializeObject(jsonObject), System.Text.Encoding.UTF8, "application/json")) {
				HttpResponseMessage result = _httpClient.PostAsync(_url, content).Result;
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
					return true;
				string returnValue = result.Content.ReadAsStringAsync().Result;
				throw new Exception($"Failed to POST data: ({result.StatusCode}): {returnValue}");
			}
		}
		private bool Delete(string model, string id) {
			string _url = $"{apiURL}{model}/{id}";
			HttpResponseMessage result = _httpClient.DeleteAsync(_url).Result;
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
					return true;
				string returnValue = result.Content.ReadAsStringAsync().Result;
				throw new Exception($"Failed to DELETE data: ({result.StatusCode}): {returnValue}");			
		}
		private bool Patch(string model,string id, string json) {
			string _url = $"{apiURL}{model}/{id}";
			dynamic jsonObject = JsonConvert.DeserializeObject(json);
			using (var content = new StringContent(JsonConvert.SerializeObject(jsonObject), System.Text.Encoding.UTF8, "application/json")) {
				HttpResponseMessage result = PatchAsync(_url, content).Result;
				 if (result.StatusCode == System.Net.HttpStatusCode.OK)
					return true;
				string returnValue = result.Content.ReadAsStringAsync().Result;
				throw new Exception($"Failed to PATCH data: ({result.StatusCode}): {returnValue}");
			}
		}

		private async Task<HttpResponseMessage> PatchAsync( string UriString, HttpContent iContent) {
			Uri requestUri = new Uri(UriString);
			var method = new HttpMethod("PATCH");
			var request = new HttpRequestMessage(method, requestUri) {
				Content = iContent
			};

			HttpResponseMessage response = new HttpResponseMessage();
			try {
				response = await _httpClient.SendAsync(request);
			} catch (TaskCanceledException e) {
				Console.WriteLine("ERROR: " + e.ToString());
				throw e;
			}

			return response;
		}



		private void uploadfile(string address, string codart, string md5, string filePath) {
			string _url = $"{apiURL}{address}";
			using (WebClient client = new WebClient()) {
				client.Headers.Add("Authorization", "Bearer " + apiToken);
				client.Headers.Add("md5", md5);
				try {
					client.UploadFile(_url, filePath);
				} catch (Exception ex) {
					throw (ex);
				}
				
			}
		}

		private void uploadfile(string address, string filePath) {
			string _url = $"{apiURL}{address}";
			using (WebClient client = new WebClient()) {
				client.Headers.Add("Authorization", "Bearer " + apiToken);
				try {
					client.UploadFile(_url, filePath);
				} catch (Exception ex) {
					throw (ex);
				}

			}
		}

		//private async Task<System.IO.Stream> Upload(string actionUrl, string paramString, Stream paramFileStream, byte[] paramFileBytes) {
		//	HttpContent stringContent = new StringContent(paramString);
		//	HttpContent fileStreamContent = new StreamContent(paramFileStream);
		//	HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
		//	using (var client = new HttpClient())
		//	using (var formData = new MultipartFormDataContent()) {
		//		formData.Add(stringContent, "param1", "param1");
		//		formData.Add(fileStreamContent, "file1", "file1");
		//		formData.Add(bytesContent, "file2", "file2");
		//		var response = await client.PostAsync(actionUrl, formData);
		//		if (!response.IsSuccessStatusCode) {
		//			return null;
		//		}
		//		return await response.Content.ReadAsStreamAsync();
		//	}
		//}
	}
}
