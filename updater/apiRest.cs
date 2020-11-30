using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Net.NetworkInformation;

namespace updaterFactuWeb {
    internal class apiRest : IDisposable {
		private HttpClient _httpClient = new HttpClient();
		private string apiToken;
		private string apiURL;

		public apiRest() {

			apiToken = Properties.Settings.Default.token;
			apiURL = Properties.Settings.Default.api;

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
		}

		public DataTable listOrders() {
			try {
				string json = this.Get("getorders", "");
				json = @"{'ordersList': " + json + "}";
				DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
				DataTable dataTable = dataSet.Tables["ordersList"];




				return dataTable;
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}
		public string listOrders2() {
			try {
				string json = this.Get("getorders", "");
				json = @"{'data': " + json + "}";
				return json;
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}

		public DataTable listModels() {
			try {
				string json = this.Get("models/list", "");
				json = @"{'modelsList': " + json + "}";
				DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
				DataTable dataTable = dataSet.Tables["modelsList"];
				return dataTable;
			} catch (Exception ex) {
				Logger.log(ex);
				throw ;
			}
		}

		public bool EXECUTE(string query) {
			try {
				return this.Post("db/run", "{query: \"" + query + "\"}");
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}

		public bool INSERT(string model, DataRow data) {
			try {
				string json = DataRowToJSON(data);				
				return this.Post("db/add", "{model: \"" + model + "\", data:" + json+ "}");
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}
				
		public bool DESTROY(string model) {
			try {
				string json = this.Get("models/" + model + "/destroy", "");
				return true;
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}

		public bool DELETE(string model, string id) {
			try {
				return this.Post("db/del", "{model: \"" + model + "\", data:\"" + id + "\"}");
			} catch (Exception ex) {
				Logger.log(ex);
				throw;
			}
		}

		public bool UPLOAD_DB(string path) {
			if (path != "") {
				if (File.Exists(path)) {
					try {
						uploadfile("db/upload", path);
						return true;
					} catch (Exception ex) {
						Logger.log(ex);
						throw;
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
								uploadfile("image/upload/" + codart, md5, path);
							} catch (Exception ex) {
								Logger.log(ex);
								throw;
							}
						}
					} catch (Exception ex ) {
						Logger.log(ex);
						throw;
					}
				}
			}
			return false;
		}

		public static string CalculateMD5(string filename) {
			using (var md5 = System.Security.Cryptography.MD5.Create()) {
				using (var stream = File.OpenRead(filename)) {
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}

		private static string DataRowToJSON(DataRow row) {
			string[] types = new string[row.Table.Columns.Count];

			for (var i = 0; i < row.Table.Columns.Count; i++) {
				types[i] = row.Table.Columns[i].DataType.ToString();
			}
			string response = "{";


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
				var err = new Exception($"Failed to GET data: ({result.StatusCode}): {returnValue}");
				Logger.log(err);
				throw err;
			}
		}

		private bool Post( string url,string json) {
			string _url = $"{apiURL+url}";
			dynamic jsonObject = JsonConvert.DeserializeObject(json);
			using (var content = new StringContent(JsonConvert.SerializeObject(jsonObject), System.Text.Encoding.UTF8, "application/json")) {
				HttpResponseMessage result = _httpClient.PostAsync(_url, content).Result;
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
					return true;
				string returnValue = result.Content.ReadAsStringAsync().Result;
				var err = new Exception($"Failed to POST data: ({result.StatusCode}): {returnValue}");
				Logger.log(err);
				throw err;
			}
		}
		
		private void uploadfile(string address, string md5, string filePath) {
			string _url = $"{apiURL}{address}";
			using (WebClient client = new WebClient()) {
				client.Headers.Add("Authorization", "Bearer " + apiToken);
				client.Headers.Add("md5", md5);
				try {
					client.UploadFile(_url, filePath);
				} catch (Exception ex) {
					Logger.log(ex);
					throw;
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
					Logger.log(ex);
					throw;
				}

			}
		}

        public void Dispose() {
            throw new NotImplementedException();
        }

		//private string PostWithResponse(string url, string json) {
		//	string _url = $"{apiURL + url}";
		//	dynamic jsonObject = JsonConvert.DeserializeObject(json);
		//	using (var content = new StringContent(JsonConvert.SerializeObject(jsonObject), System.Text.Encoding.UTF8, "application/json")) {
		//		HttpResponseMessage result = _httpClient.PostAsync(_url, content).Result;
		//		string returnValue = result.Content.ReadAsStringAsync().Result;
		//		if (result.StatusCode == System.Net.HttpStatusCode.OK)
		//			return returnValue;
		//		throw new Exception($"Failed to POST data: ({result.StatusCode}): {returnValue}");
		//	}
		//}

		//private static string DataTableToJSON(DataTable dt) {
		//	string response = "[";
		//	foreach (DataRow dr in dt.Rows) {
		//		if (response != "[")
		//			response += ",";
		//		response += DataRowToJSON(dr);
		//	}
		//	response += "]";
		//	return response;
		//}

		//public string PHOTOS(DataTable data) {
		//	try {
		//		string json = this.DataTableToJSON(data);
		//		return this.PostWithResponse("db/photos", "{ data:" + json + "}");
		//	} catch (Exception ex) {
		//		throw (ex);
		//	}
		//}
	}
}
