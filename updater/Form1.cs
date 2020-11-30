using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace updaterFactuWeb {
	public partial class Form1 : Form {
		RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		Update updateTaskIncremental = new Update(false);
		Update updateTaskTotal = new Update(false);
		Update updateTaskFotos = new Update(false);
		Orders updateTaskPedidos = new Orders();

		public Form1() {
			InitializeComponent();
			loadConfig();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxInitOnStartUp.Checked) {
				rkApp.SetValue("updaterFactuWeb", Application.ExecutablePath);
			} else {
				rkApp.DeleteValue("updaterFactuWeb", false);
			}
		}

		private void loadConfig() {

			// Config Factusol
			this.textBoxRutaFactusol.Text = updaterFactuWeb.Properties.Settings.Default.path;
			this.textBoxEmpresaFactusol.Text = updaterFactuWeb.Properties.Settings.Default.company;
			this.textBoxYearFactusol.Text = updaterFactuWeb.Properties.Settings.Default.year;

			// Config API
			this.textBoxConfigDirApi.Text = updaterFactuWeb.Properties.Settings.Default.api;
			this.textBoxConfigTokenApi.Text = updaterFactuWeb.Properties.Settings.Default.token;

			// Config Incrementales
			this.textBoxTimeIntervalo.Text = updaterFactuWeb.Properties.Settings.Default.timeIncrementales.ToString();
			this.checkBoxAutoIncrementales.Checked = updaterFactuWeb.Properties.Settings.Default.autoIncrementales;
			this.textBoxTimeIntervalo.Enabled = updaterFactuWeb.Properties.Settings.Default.autoIncrementales;
			this.timerIncrementales.Interval = updaterFactuWeb.Properties.Settings.Default.timeIncrementales*60 * 1000;
			if (updaterFactuWeb.Properties.Settings.Default.autoIncrementales) {				
				this.timerIncrementales.Start();
			} else {
				this.timerIncrementales.Stop();
			}

			// Config Fotos
			this.textBoxTimeFotos.Text = updaterFactuWeb.Properties.Settings.Default.timeFotos.ToString();
			this.checkBoxAutoFotos.Checked = updaterFactuWeb.Properties.Settings.Default.autoFotos;
			this.textBoxTimeFotos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoFotos;
			this.timerFotos.Interval = updaterFactuWeb.Properties.Settings.Default.timeFotos*60 * 1000;
			if (updaterFactuWeb.Properties.Settings.Default.autoFotos) {
				this.timerFotos.Start();
			} else {
				this.timerFotos.Stop();
			}

			// Config Pedidos
			this.textBoxTimePedidos.Text = updaterFactuWeb.Properties.Settings.Default.timePedidos.ToString();
			this.checkBoxAutoPedidos.Checked = updaterFactuWeb.Properties.Settings.Default.autoPedidos;
			this.textBoxTimePedidos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoPedidos;
			this.timerPedidos.Interval = updaterFactuWeb.Properties.Settings.Default.timePedidos * 60 * 1000;
			if (updaterFactuWeb.Properties.Settings.Default.autoPedidos) {
				this.timerPedidos.Start();
			} else {
				this.timerPedidos.Stop();
			}

			// Config Total
			this.dateTimePickerTimeTotal.Value = Convert.ToDateTime(updaterFactuWeb.Properties.Settings.Default.timeTotal);
			this.checkBoxAutoTotal.Checked = updaterFactuWeb.Properties.Settings.Default.autoTotal;
			this.dateTimePickerTimeTotal.Enabled = updaterFactuWeb.Properties.Settings.Default.autoTotal;
			this.timerTotal.Interval = 3000;
			if (updaterFactuWeb.Properties.Settings.Default.autoTotal) {
				this.timerTotal.Start();
			} else {
				this.timerTotal.Stop();
			}


			if (updaterFactuWeb.Properties.Settings.Default.path != "" &&
				updaterFactuWeb.Properties.Settings.Default.company != "" &&
				updaterFactuWeb.Properties.Settings.Default.year != "" &&
				updaterFactuWeb.Properties.Settings.Default.api != "" &&
				updaterFactuWeb.Properties.Settings.Default.token != "") {
				buttonStartPhotos.Enabled = true;
				buttonStartTotal.Enabled = true;
				buttonStartIncremental.Enabled = true;
				toolStripMenuItem2.Enabled = true;
			} else {
				buttonStartPhotos.Enabled = false;
				buttonStartTotal.Enabled = false;
				buttonStartIncremental.Enabled = false;
				toolStripMenuItem2.Enabled = false;
			}

		}

		private void Form1_Load(object sender, EventArgs e) {
			if (rkApp.GetValue("updaterFactuWeb") == null) {
				checkBoxInitOnStartUp.Checked = false;
			} else {
				checkBoxInitOnStartUp.Checked = true;
			}
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e) {
			Application.Exit();
			Environment.Exit(0);
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e) {
			this.Show();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = true;
			this.Hide();
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				if (this.Visible) {
					this.Hide();
				} else {
					this.Show();
				}
			}
		}

		private void Form1_Shown(object sender, EventArgs e) {
			if (updaterFactuWeb.Properties.Settings.Default.path != "" &&
				updaterFactuWeb.Properties.Settings.Default.company != "" &&
				updaterFactuWeb.Properties.Settings.Default.year != "" &&
				updaterFactuWeb.Properties.Settings.Default.api != "" &&
				updaterFactuWeb.Properties.Settings.Default.token != "" &&
				updaterFactuWeb.Properties.Settings.Default.initmin
				)
				this.Hide();
		}


		void u_OnInitUpdate(object sender, UpdateTaskResponse response) {

			if (response.Type == "incremental") {
				buttonStartIncremental.Text = "Parar actualización";
				textBoxTimeIntervalo.Enabled = false;
				checkBoxAutoIncrementales.Enabled = false;
				if (updaterFactuWeb.Properties.Settings.Default.autoIncrementales) {
					timerIncrementales.Stop();
				}
				labelStatusIncremental.Text = "";
				progressBarIncremental.Value = 0;
			}

			if (response.Type == "fotos") {
				buttonStartPhotos.Text = "Parar actualización";
				textBoxTimeFotos.Enabled = false;
				checkBoxAutoFotos.Enabled = false;
				if (updaterFactuWeb.Properties.Settings.Default.autoFotos) {
					timerFotos.Stop();
				}
				labelStatusFotos.Text = "";
				progressBarFotos.Value = 0;
			}

			if (response.Type == "pedidos") {
				buttonStartOrders.Text = "Parar descarga";
				textBoxTimePedidos.Enabled = false;
				checkBoxAutoPedidos.Enabled = false;
				if (updaterFactuWeb.Properties.Settings.Default.autoPedidos) {
					timerPedidos.Stop();
				}
				labelStatusPedidos.Text = "";
				progressBarPedidos.Value = 0;
			}

			if (response.Type == "total") {
				buttonStartTotal.Text = "Parar actualización";
				dateTimePickerTimeTotal.Enabled = false;
				checkBoxAutoTotal.Enabled = false;
				if (updaterFactuWeb.Properties.Settings.Default.autoTotal) {
					timerTotal.Stop();
				}
				labelStatusTotal.Text = "";
				progressBarTotal.Value = 0;
			}


			groupBoxConfigApi.Enabled = false;
			groupBoxConfigFactusol.Enabled = false;

		}

		void u_ChangeStatus(object sender, UpdateTaskResponse response) {
			Label lbl = new Label();

			if (response.Type == "incremental")
				lbl = labelStatusIncremental;

			if (response.Type == "fotos")
				lbl = labelStatusFotos;

			if (response.Type == "pedidos")
				lbl = labelStatusPedidos;

			if (response.Type == "total")
				lbl = labelStatusTotal;

			lbl.Text = response.Message;
		}

		void u_ChangePercent(object sender, UpdateTaskResponse response) {
			ProgressBar progress = new ProgressBar();
			
			if (response.Type == "incremental") 
				progress = progressBarIncremental;

			if (response.Type == "fotos")
				progress = progressBarFotos;

			if (response.Type == "pedidos")
				progress = progressBarPedidos;

			if (response.Type == "total")
				progress = progressBarTotal;

			if (response.Value > progress.Maximum) {
				progress.Invoke((Action)(() => progress.Value = progress.Maximum));
			} else {
				progress.Invoke((Action)(() => progress.Value = response.Value));
			}
		}

		void u_OnFinishUpdate(object sender, UpdateTaskResponse response) {
			if (response.Type == "incremental") {
				buttonStartIncremental.Text = "Actualizar ahora";				
				checkBoxAutoIncrementales.Enabled = true;
				if (updaterFactuWeb.Properties.Settings.Default.autoIncrementales) {
					textBoxTimeIntervalo.Enabled = true;
					timerIncrementales.Start();
				}
				labelStatusIncremental.Text = "";
				progressBarIncremental.Value = 0;
			}

			if (response.Type == "fotos") {
				buttonStartPhotos.Text = "Actualizar ahora";
				checkBoxAutoFotos.Enabled = true;
				if (updaterFactuWeb.Properties.Settings.Default.autoFotos) {
					textBoxTimeFotos.Enabled = true;
					timerFotos.Start();
				}
				labelStatusFotos.Text = "";
				progressBarFotos.Value = 0;
			}

			if (response.Type == "pedidos") {
				buttonStartOrders.Text = "Descargar ahora";
				checkBoxAutoPedidos.Enabled = true;
				if (updaterFactuWeb.Properties.Settings.Default.autoPedidos) {
					textBoxTimePedidos.Enabled = true;
					timerPedidos.Start();
				}
				labelStatusPedidos.Text = "";
				progressBarPedidos.Value = 0;
			}

			if (response.Type == "total") {
				buttonStartTotal.Text = "Actualizar ahora";
				checkBoxAutoTotal.Enabled = true;
				if (updaterFactuWeb.Properties.Settings.Default.autoTotal) {
					dateTimePickerTimeTotal.Enabled = true;
					timerTotal.Start();
				}
				labelStatusTotal.Text = "";
				progressBarTotal.Value = 0;
			}

			if (checkBoxAutoFotos.Enabled && checkBoxAutoIncrementales.Enabled && checkBoxAutoPedidos.Enabled) {
				groupBoxConfigApi.Enabled = true;
				groupBoxConfigFactusol.Enabled = true;
			}

		}

		void u_OnError(object sender, UpdateTaskResponse response) {
			bool showError = false;

			if (showError)
				MessageBox.Show(response.Message, "Update Error", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);


		}

		private void ClearCache() {
			DialogResult result = MessageBox.Show("Deseas eliminar la cache y hacer la actualización de cero?", "Atención", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes) {
				DialogResult result2 = MessageBox.Show("Eliminar tambien todos los datos del servidor web?", "Atención", MessageBoxButtons.YesNo);

				Update updateTask = new Update(true);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnError += u_OnError;

				if (result2 == DialogResult.Yes) {
					updateTask.StartClear();
				} else {
					updateTask.StartClearOnlyCache();
				}
			}
		}

			


		private void buttonExploreRutaFactusol_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK) {
				textBoxRutaFactusol.Text = folderBrowserDialog1.SelectedPath;
			}
		}
		private void buttonRemoveCache_Click(object sender, EventArgs e) {
			ClearCache();
		}
		private void linkOpenApplicationFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("explorer.exe", @".");
		}
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process fileopener = new Process();
			fileopener.StartInfo.FileName = "explorer";
			fileopener.StartInfo.Arguments = "\"" + Logger.file_name + "\"";
			fileopener.Start();
		}

		#region Incrementales	
		private void buttonStartIncremental_Click(object sender, EventArgs e) {
				if (updateTaskIncremental.UpdateProcessStopped) {
					SyncIncremental();
				} else {
					updateTaskIncremental.Stop();
				}				
			}

			private void timerIncrementales_Tick(object sender, EventArgs e) {
				if (updateTaskIncremental.UpdateProcessStopped) 
					if (updateTaskTotal.UpdateProcessStopped) 
						SyncIncremental();
			}

			private void SyncIncremental() {
				if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
					updateTaskIncremental = new Update(false);

					updateTaskIncremental.CallbackOnInitUpdate += u_OnInitUpdate;
					updateTaskIncremental.CallbackOnFinishUpdate += u_OnFinishUpdate;
					updateTaskIncremental.CallbackOnStatusChange += u_ChangeStatus;
					updateTaskIncremental.CallbackOnPercentChange += u_ChangePercent;
					updateTaskIncremental.CallbackOnError += u_OnError;

					updateTaskIncremental.StartIncremental();
				} else {
					MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
				}
			}

			private void checkBoxAutoIncrementales_CheckedChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.autoIncrementales = checkBoxAutoIncrementales.Checked;
				updaterFactuWeb.Properties.Settings.Default.Save();

				textBoxTimeIntervalo.Enabled = updaterFactuWeb.Properties.Settings.Default.autoIncrementales;
				timerIncrementales.Enabled = updaterFactuWeb.Properties.Settings.Default.autoIncrementales;
				if (updaterFactuWeb.Properties.Settings.Default.autoIncrementales) {
					timerIncrementales.Start();
				} else {
					timerIncrementales.Stop();
				}					
			}

			private void textBoxTimeIntervalo_ValueChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.timeIncrementales = (int)textBoxTimeIntervalo.Value;
				updaterFactuWeb.Properties.Settings.Default.Save();

				timerIncrementales.Interval = updaterFactuWeb.Properties.Settings.Default.timeIncrementales *60* 1000;
				timerIncrementales.Stop();
				timerIncrementales.Start();
			}
		#endregion

		#region Fotos
			private void buttonStartPhotos_Click(object sender, EventArgs e) {
				if (updateTaskFotos.UpdateProcessStopped) {
					this.SyncPhotos();
				} else {
					updateTaskFotos.Stop();
				}
			}
			private void timerFotos_Tick(object sender, EventArgs e) {
				if (updateTaskFotos.UpdateProcessStopped)
					this.SyncPhotos();
			}
			private void SyncPhotos() {
				if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
					updateTaskFotos = new Update(false);

					updateTaskFotos.CallbackOnInitUpdate += u_OnInitUpdate;
					updateTaskFotos.CallbackOnFinishUpdate += u_OnFinishUpdate;
					updateTaskFotos.CallbackOnStatusChange += u_ChangeStatus;
					updateTaskFotos.CallbackOnPercentChange += u_ChangePercent;
					updateTaskFotos.CallbackOnError += u_OnError;

					updateTaskFotos.StartPhotos();
				} else {
					MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
				}
			}

			private void checkBoxAutoFotos_CheckedChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.autoFotos = checkBoxAutoFotos.Checked;
				updaterFactuWeb.Properties.Settings.Default.Save();

				textBoxTimeFotos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoFotos;
				timerFotos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoFotos;
				if (updaterFactuWeb.Properties.Settings.Default.autoFotos) {
					timerFotos.Start();
				} else {
					timerFotos.Stop();
				}
			}

			private void textBoxTimeFotos_ValueChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.timeFotos = (int)textBoxTimeFotos.Value;
				updaterFactuWeb.Properties.Settings.Default.Save();

				timerFotos.Interval = updaterFactuWeb.Properties.Settings.Default.timeFotos*60* 1000;
			timerFotos.Stop();
			timerFotos.Start();
			}
		#endregion

		#region Pedidos
			private void buttonStartOrders_Click(object sender, EventArgs e) {				
				if (updateTaskPedidos.UpdateProcessStopped) {
					GetOrders();
				} else {
					updateTaskPedidos.Stop();
				}
			}

			private void GetOrders() {
				if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
					updateTaskPedidos = new Orders();

					updateTaskPedidos.CallbackOnInitUpdate += u_OnInitUpdate;
					updateTaskPedidos.CallbackOnFinishUpdate += u_OnFinishUpdate;
					updateTaskPedidos.CallbackOnStatusChange += u_ChangeStatus;
					updateTaskPedidos.CallbackOnPercentChange += u_ChangePercent;
					updateTaskPedidos.CallbackOnError += u_OnError;

					updateTaskPedidos.Start();
				} else {
					MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
				}
			}

			private void checkBoxAutoPedidos_CheckedChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.autoPedidos = this.checkBoxAutoPedidos.Checked;
				updaterFactuWeb.Properties.Settings.Default.Save();
				
				textBoxTimePedidos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoPedidos;
				timerPedidos.Enabled = updaterFactuWeb.Properties.Settings.Default.autoPedidos;
				if (updaterFactuWeb.Properties.Settings.Default.autoPedidos) {
					timerPedidos.Start();
				} else {
					timerPedidos.Stop();
				}
			}

			private void textBoxTimePedidos_ValueChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.timePedidos = (int)this.textBoxTimePedidos.Value;
				updaterFactuWeb.Properties.Settings.Default.Save();
				timerPedidos.Interval = (int)textBoxTimePedidos.Value*60 * 1000;
				timerPedidos.Stop();
				timerPedidos.Start();

			}

			private void timerPedidos_Tick(object sender, EventArgs e) {
				if (updateTaskPedidos.UpdateProcessStopped) {
					GetOrders();
				}					
			}
		#endregion

		#region Total
			private void buttonStartTotal_Click(object sender, EventArgs e) {
				if (updateTaskTotal.UpdateProcessStopped) {
					if (!updateTaskIncremental.UpdateProcessStopped) {
						updateTaskIncremental.Stop();
					}
					SyncTotal();
				} else {
					updateTaskTotal.Stop();
				}		
			}

			private void SyncTotal() {
				if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
					updateTaskTotal = new Update(false);

					updateTaskTotal.CallbackOnInitUpdate += u_OnInitUpdate;
					updateTaskTotal.CallbackOnFinishUpdate += u_OnFinishUpdate;
					updateTaskTotal.CallbackOnStatusChange += u_ChangeStatus;
					updateTaskTotal.CallbackOnPercentChange += u_ChangePercent;
					updateTaskTotal.CallbackOnError += u_OnError;

					updateTaskTotal.StartTotal();
				} else {
					MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
				}
			}

			private void checkBoxAutoTotal_CheckedChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.autoTotal = this.checkBoxAutoTotal.Checked;
				updaterFactuWeb.Properties.Settings.Default.Save();

				dateTimePickerTimeTotal.Enabled = updaterFactuWeb.Properties.Settings.Default.autoTotal;
				timerTotal.Enabled = updaterFactuWeb.Properties.Settings.Default.autoTotal;
				if (updaterFactuWeb.Properties.Settings.Default.autoTotal) {
					timerTotal.Start();
				} else {
					timerTotal.Stop();
				}
			}

			private void dateTimePickerTimeTotal_ValueChanged(object sender, EventArgs e) {
				updaterFactuWeb.Properties.Settings.Default.timeTotal = this.dateTimePickerTimeTotal.Value.ToString();
				updaterFactuWeb.Properties.Settings.Default.Save();
				timerTotal.Interval = 3000;
				timerTotal.Stop();
				timerTotal.Start();
			}

			private void timerTotal_Tick(object sender, EventArgs e) {
				var now = DateTime.Now;
				if (dateTimePickerTimeTotal.Value.Hour == now.Hour && dateTimePickerTimeTotal.Value.Minute == now.Minute) {
					if (updateTaskTotal.UpdateProcessStopped)
						SyncTotal();
				}
			}

		#endregion


		private void onChangeMDBPath() {
			string path = Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + Properties.Settings.Default.dbExtension;
			if (System.IO.File.Exists(path)) {
				labelPathExists.Text = "(Ruta correcta)";
				labelPathExists.ForeColor = System.Drawing.Color.Green;
			} else {
				labelPathExists.Text = "(Sin acceso a los datos, revise los datos)";
				labelPathExists.ForeColor = System.Drawing.Color.Red;
			}
		}

		private void checkBoxInitMin_CheckedChanged(object sender, EventArgs e) {
			updaterFactuWeb.Properties.Settings.Default.initmin = checkBoxInitMin.Checked;
			updaterFactuWeb.Properties.Settings.Default.Save();
		}

        private void textBoxConfigTokenApi_TextChanged(object sender, EventArgs e) {
			updaterFactuWeb.Properties.Settings.Default.token = this.textBoxConfigTokenApi.Text;
			updaterFactuWeb.Properties.Settings.Default.Save();
		}

        private void textBoxConfigDirApi_TextChanged(object sender, EventArgs e) {
			string t = this.textBoxConfigDirApi.Text;
			if (t != "" && !t.EndsWith("/"))
				t += "/";
			updaterFactuWeb.Properties.Settings.Default.api = t;
			updaterFactuWeb.Properties.Settings.Default.Save();
		}

        private void textBoxRutaFactusol_TextChanged(object sender, EventArgs e) {			
			string t = this.textBoxRutaFactusol.Text;
			if (t != "" && !t.EndsWith("\\"))
				t += "\\";
			updaterFactuWeb.Properties.Settings.Default.path = t;
			updaterFactuWeb.Properties.Settings.Default.Save();
			this.onChangeMDBPath();
		}

       
        private void textBoxEmpresaFactusol_TextChanged(object sender, EventArgs e) {
			updaterFactuWeb.Properties.Settings.Default.company = this.textBoxEmpresaFactusol.Text;
			updaterFactuWeb.Properties.Settings.Default.Save();
			this.onChangeMDBPath();
		}

        private void textBoxYearFactusol_TextChanged(object sender, EventArgs e) {
			updaterFactuWeb.Properties.Settings.Default.year = this.textBoxYearFactusol.Text;
			updaterFactuWeb.Properties.Settings.Default.Save();
			this.onChangeMDBPath();
		}
    }
}
