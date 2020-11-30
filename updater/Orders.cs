using ADODB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace updaterFactuWeb {

	public class Price {
		public double clientPrice { get; set; }
		public double price { get; set; }
		public int dto { get; set; }
	}

	public class LineaPedido {
		public string CODART { get; set; }
		public string DESART { get; set; }
		public string DIMART { get; set; }
		public string OBSART { get; set; }
		public string IMGART { get; set; }
		public int PESART { get; set; }
		public string EANART { get; set; }
		public int UELART { get; set; }
		public int UPPART { get; set; }
		public string DEWART { get; set; }
		public string DLAART { get; set; }
		public string FAMART { get; set; }
		public int SUWART { get; set; }
		public int ORD { get; set; }
		public string CODCE1 { get; set; }
		public string CE1ART { get; set; }
		public Price price { get; set; }
		public int type { get; set; }
		public int qty { get; set; }
		public int dto { get; set; }
		public double clientPrice { get; set; }
	}

	public class infoPedido {
		public int ID { get; set; }
		public string CODCLI { get; set; }
		public List<LineaPedido> DATA { get; set; }
		public string COMENTARIOS { get; set; }
		public string FECHA { get; set; }
		public string HORA { get; set; }
		public int STATUS { get; set; }
	}

	public class OrdersStructure {
		public List<infoPedido> data { get; set; }
	}

	class Orders : IDisposable {

		public bool UpdateProcessStopped = true;

		private readonly SynchronizationContext SyncContext;

		public event EventHandler<UpdateTaskResponse> CallbackOnInitUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnFinishUpdate;
		public event EventHandler<UpdateTaskResponse> CallbackOnStatusChange;
		public event EventHandler<UpdateTaskResponse> CallbackOnError;
		public event EventHandler<UpdateTaskResponse> CallbackOnPercentChange;

		private string type = "pedidos";

		private readonly apiRest API;

		public Orders() {
			SyncContext = AsyncOperationManager.SynchronizationContext;
			this.API = new apiRest();
		}

		public void Start() {
			Thread thread = new Thread(Run);
			thread.IsBackground = true;
			thread.Start();
		}

		private void Run() {
			while (!UpdateProcessStopped) {
                SyncContext.Post(e => triggerOnInitUpdate(new UpdateTaskResponse(type,"")), null);

				OrdersStructure OrdersList = new OrdersStructure();
				try {
					SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, "Recibiendo lista de pedidos del servidor")), null);
					string ordersJSON = API.listOrders2();
					OrdersList = JsonConvert.DeserializeObject<OrdersStructure>(ordersJSON);
				} catch (Exception ex) {
					Logger.log(ex);
					SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
					Stop();
					return;
				}
				if (OrdersList.data.Count > 0) {
					Mdb mdbFactu = new Mdb(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension);

					int itt = 0;
					foreach (infoPedido order in OrdersList.data) {
						if (UpdateProcessStopped)
							break;
						int percent = itt * 100 / OrdersList.data.Count;
						triggerOnPercentChange(new UpdateTaskResponse(type, "", percent));

						List<string> orderQuerys = new List<string>();

						DataTable infoCli_DT = mdbFactu.Query("SELECT * FROM F_CLI WHERE CODCLI=" + order.CODCLI + ";");
						DataRow infoCli;
						if (infoCli_DT.Rows.Count > 0) {
							infoCli = infoCli_DT.Rows[0];
						} else {
							string msg = "Error al importar pedido. No se ha podido acceder a la información del cliente - " + order.CODCLI;
							Logger.log(msg);
							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, msg)), null);
							Stop();
							return;
						}
						SyncContext.Post(e => triggerOnStatusChange(new UpdateTaskResponse(type, itt + "/" + OrdersList.data.Count + " Traspasando pedido de " + infoCli["NOCCLI"])), null);

						DataTable numNewID_DT = mdbFactu.Query("SELECT TOP 1 CODPRE FROM F_PRE WHERE F_PRE.TIPPRE = '1' ORDER BY CODPRE DESC;");
						int numNewId = 0;
						if (numNewID_DT.Rows.Count > 0) {
							numNewId = Int32.Parse(numNewID_DT.Rows[0][0].ToString()) + 1;
						}

						int numLine = 1;
						decimal totPedBruto = 0;
						foreach (LineaPedido linea in order.DATA) {							
							string Description = linea.DESART.ToString().Replace("'", "´");
							if (linea.CE1ART != "") Description = linea.CE1ART + " - " + Description;

							DataTable infoArt_DT = mdbFactu.Query("SELECT * FROM F_ART WHERE CODART='" + linea.CODART + "';");
							DataRow infoArt;
							if (infoArt_DT.Rows.Count > 0) {
								infoArt = infoArt_DT.Rows[0];
							} else {
								string msg = "Error al importar pedido. No se ha podido acceder a la información del Artículo - " + linea.CODART;
								Logger.log(msg);
								SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, msg)), null);
								Stop();
								return;
							}

							decimal total = (decimal)linea.qty * (decimal)linea.price.price;
							totPedBruto += total;
							string query = "INSERT INTO `F_LPS` (`TIPLPS`, `CODLPS`, `POSLPS`, `ARTLPS`, `DESLPS`, `CANLPS`, `DT1LPS`, `DT2LPS`, `DT3LPS`, `PRELPS`, `TOTLPS`, `IVALPS`, `MEMLPS`, `ALTLPS`, `ANCLPS`, `FONLPS`, `FFALPS`, `FCOLPS`, `IINLPS`, `PIVLPS`, `TIVLPS`, `FIMLPS`, `COSLPS`, `CE1LPS`, `CE2LPS`, `IMALPS`, `BULLPS`, `SUMLPS`) VALUES('1', " + numNewId + ", " + numLine + ", '" + linea.CODART + "', '" + Description + "'," + linea.qty.ToString().Replace(",", ".") + ",0.00000000e+00,0.00000000e+00,0.00000000e+00," + linea.price.price.ToString().Replace(",", ".") + ", " + total.ToString().Replace(",", ".") + "," + infoArt["TIVART"].ToString().Replace(",", ".") + ",'',0.0000,0.0000,0.0000,'1900 - 01 - 01 00:00:00','1900 - 01 - 01 00:00:00',0,0.0000,0.0000,0," + infoArt["PCOART"].ToString().Replace(",", ".") + ",'" + linea.CODCE1 + "','','" + infoArt["IMGART"] + "',0.0000,NULL);";
							orderQuerys.Add(query);
							numLine++;
						}

						decimal PercentPP = (decimal)infoCli["PPACLI"];
						decimal totDtoPPPed = totPedBruto * (PercentPP / 100);
						decimal totBaseImponiblePed = totPedBruto - totDtoPPPed;
						double PercentRE = (short)infoCli["REQCLI"] == 1 ? 5.2 : 0;
						decimal totREPed = totBaseImponiblePed * ((decimal)PercentRE / 100);
						decimal PercentIva = (short)infoCli["IVACLI"] == 1 ? 0 : 21;
						decimal totIVAPed = totBaseImponiblePed * (PercentIva / 100);
						decimal totNetoPed = totBaseImponiblePed + totREPed + totIVAPed;
						string obsPedido = order.COMENTARIOS;
						string codFormaPago = infoCli["FPACLI"].ToString();
						int direccion = 0;

						string q = "INSERT INTO F_PRE(`TIPPRE`, `CODPRE`, `REFPRE`, `FECPRE`, `AGEPRE`, `PROPRE`, `CLIPRE`, `CNOPRE`, `CDOPRE`, `CPOPRE`, `CCPPRE`, `CPRPRE`, `CNIPRE`, `TIVPRE`, `REQPRE`, `TELPRE`, `ALMPRE`, `NET1PRE`, `NET2PRE`, `NET3PRE`, `PDTO1PRE`, `PDTO2PRE`, `PDTO3PRE`, `IDTO1PRE`, `IDTO2PRE`, `IDTO3PRE`, `PPPA1PRE`, `PPPA2PRE`, `PPPA3PRE`, `IPPA1PRE`, `IPPA2PRE`, `IPPA3PRE`, `PPOR1PRE`, `PPOR2PRE`, `PPOR3PRE`, `IPOR1PRE`, `IPOR2PRE`, `IPOR3PRE`, `PFIN1PRE`, `PFIN2PRE`, `PFIN3PRE`, `IFIN1PRE`, `IFIN2PRE`, `IFIN3PRE`, `BAS1PRE`, `BAS2PRE`, `BAS3PRE`, `PIVA1PRE`, `PIVA2PRE`, `PIVA3PRE`, `IIVA1PRE`, `IIVA2PRE`, `IIVA3PRE`, `PREC1PRE`, `PREC2PRE`, `PREC3PRE`, `IREC1PRE`, `IREC2PRE`, `IREC3PRE`, `PRET1PRE`, `IRET1PRE`, `TOTPRE`, `FOPPRE`, `PENPRE`, `TVAPRE`, `PRTPRE`, `TPOPRE`, `OB1PRE`, `OB2PRE`, `OBRPRE`, `ESTPRE`, `IPRPRE`, `I1HPRE`, `IFPPRE`, `I2HPRE`, `I1NPRE`, `I2NPRE`, `PRIPRE`, `ASOPRE`, `COMPRE`, `USUPRE`, `USMPRE`, `FAXPRE`, `IMGPRE`, `NET4PRE`, `PDTO4PRE`, `IDTO4PRE`, `PPPA4PRE`, `IPPA4PRE`, `PPOR4PRE`, `IPOR4PRE`, `PFIN4PRE`, `IFIN4PRE`, `BAS4PRE`, `EMAPRE`, `PASPRE`, `HORPRE`, `CARPRE`, `CEMPRE`, `CPAPRE`, `TIVA1PRE`, `TIVA2PRE`, `TIVA3PRE`, `TPVIDPRE`, `TERPRE`) VALUES ('1'," + numNewId + ", NULL,'" + order.FECHA + "'," + infoCli["AGECLI"] + ",NULL," + infoCli["CODCLI"] + ",'" + infoCli["NOFCLI"].ToString().Replace("'", "´") + "','" + infoCli["DOMCLI"].ToString().Replace("'", "´") + "','" + infoCli["POBCLI"].ToString().Replace("'", "´") + "','" + infoCli["CPOCLI"] + "','" + infoCli["PROCLI"].ToString().Replace("'", "´") + "','" + infoCli["NIFCLI"] + "'," + infoCli["IVACLI"] + "," + infoCli["REQCLI"] + ",'" + infoCli["TELCLI"].ToString().Substring(0, infoCli["TELCLI"].ToString().Length<=20 ? infoCli["TELCLI"].ToString().Length: 20) + "','GEN'," + Math.Round(totPedBruto, 2).ToString(new CultureInfo("en-US")) + ",0.0000,0.0000,0.0000,0.0000,0.0000,0.0000 ,0.0000,0.0000," + PercentPP.ToString(new CultureInfo("en-US")) + ",0.0000,0.0000," + totDtoPPPed.ToString(new CultureInfo("en-US")) + ",0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000," + Math.Round(totBaseImponiblePed, 2).ToString(new CultureInfo("en-US")) + ",0.0000,0.0000," + PercentIva.ToString(new CultureInfo("en-US")) + ",10.0000,4.0000," + Math.Round(totIVAPed, 2).ToString(new CultureInfo("en-US")) + ",0.0000,0.0000," + PercentRE.ToString(new CultureInfo("en-US")) + ",1.4000,0.5000," + Math.Round(totREPed, 2).ToString(new CultureInfo("en-US")) + ",0.0000,0.0000,0.0000,0.0000," + Math.Round(totNetoPed, 2).ToString(new CultureInfo("en-US")) + ",'" + codFormaPago + "',NULL,NULL,0,NULL,'" + order.COMENTARIOS + "',''," + direccion + ",0,0,0,0,0,NULL,NULL,'" + numNewId + "', NULL, '" + obsPedido + "',1,1,NULL,NULL,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0,'', now(), NULL,'" + infoCli["EMACLI"] + "','" + infoCli["PAICLI"] + "',0,1,2,NULL,0);";

						orderQuerys.Add(q);

						try {
							// Insert order in DB
							//mdbFactu.Execute(orderQuerys);

							// Download OrderPDF
							WebClient webClient = new WebClient();
							webClient.Headers[HttpRequestHeader.Authorization] = "Bearer " + Properties.Settings.Default.token;
							webClient.DownloadFile(Properties.Settings.Default.api + "order/" + order.ID, @"myfile.pdf");

							//Print PDF FILE
							Process p = new Process();
							p.StartInfo = new ProcessStartInfo() {
								CreateNoWindow = true,
								Verb = "print",
								FileName = @"myfile.pdf" //put the correct path here
							};
							p.Start();

							// SET order has been download.
							webClient.DownloadData(Properties.Settings.Default.api + "order/downloaded/" + order.ID);

						} catch (Exception ex) {
							Logger.log(ex.Message);
							SyncContext.Post(e => triggerOnError(new UpdateTaskResponse(type, ex.Message)), null);
							Stop();
							return;
						}

						itt++;
					}
					Stop();
				} else {
					Stop();
				}
			}
		}
		
		public void Stop() {
			UpdateProcessStopped = true;
			SyncContext.Post(e => triggerOnFinishUpdate(new UpdateTaskResponse(type, "")), null);
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
