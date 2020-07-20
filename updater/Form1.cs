using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace updaterFactuWeb {
	public partial class Form1 : Form {
		RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		public Form1() {
			InitializeComponent();
			loadConfig();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e) {
			if (checkBox2.Checked) {
				rkApp.SetValue("updaterFactuWeb", Application.ExecutablePath);
			} else {
				rkApp.DeleteValue("updaterFactuWeb", false);
			}
		}

		private void loadConfig() {
			this.textBox1.Text = updaterFactuWeb.Properties.Settings.Default.path;
			this.textBox2.Text = updaterFactuWeb.Properties.Settings.Default.api;
			this.textBox3.Text = updaterFactuWeb.Properties.Settings.Default.token;
			this.textBox5.Text = updaterFactuWeb.Properties.Settings.Default.company;
			this.textBox6.Text = updaterFactuWeb.Properties.Settings.Default.year;
			this.checkBox1.Checked = updaterFactuWeb.Properties.Settings.Default.autoUpdate;

			this.textBox4.Text = updaterFactuWeb.Properties.Settings.Default.interval.ToString();
			timer1.Interval = updaterFactuWeb.Properties.Settings.Default.interval * 1000;

			if (updaterFactuWeb.Properties.Settings.Default.path != "" &&
				updaterFactuWeb.Properties.Settings.Default.company != "" &&
				updaterFactuWeb.Properties.Settings.Default.year != "" &&
				updaterFactuWeb.Properties.Settings.Default.api != "" &&
				updaterFactuWeb.Properties.Settings.Default.token != "") {
				button5.Enabled = true;
				button6.Enabled = true;
				button7.Enabled = true;
				toolStripMenuItem2.Enabled = true;
			} else {
				button5.Enabled = false;
				button6.Enabled = false;
				button7.Enabled = false;
				toolStripMenuItem2.Enabled = false;
			}

			if (updaterFactuWeb.Properties.Settings.Default.autoUpdate) {
				panel1.Enabled = true;
				timer1.Enabled = true;
				timer1.Start();
			} else {
				panel1.Enabled = false;
				timer1.Stop();
				timer1.Enabled = false;
			}

			this.saveStatus();
		}

		private void saveStatus() {
			if (this.textBox1.Text != updaterFactuWeb.Properties.Settings.Default.path ||
			this.textBox2.Text != updaterFactuWeb.Properties.Settings.Default.api ||
			this.textBox3.Text != updaterFactuWeb.Properties.Settings.Default.token ||
			this.textBox5.Text != updaterFactuWeb.Properties.Settings.Default.company ||
			this.textBox6.Text != updaterFactuWeb.Properties.Settings.Default.year ||
			this.checkBox3.Checked != updaterFactuWeb.Properties.Settings.Default.initmin ||
			this.checkBox1.Checked != updaterFactuWeb.Properties.Settings.Default.autoUpdate ||
			Int32.Parse(this.textBox4.Text) != updaterFactuWeb.Properties.Settings.Default.interval) {
				panelSave.Visible = true;
			} else {
				panelSave.Visible = false;
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e) {
			this.saveStatus();
			this.panel1.Enabled = this.checkBox1.Checked;

		}

		private void Form1_Load(object sender, EventArgs e) {
			if (rkApp.GetValue("updaterFactuWeb") == null) {
				checkBox2.Checked = false;
			} else {
				checkBox2.Checked = true;
			}
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e) {
			timer1.Stop();
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
			button2.Enabled = false;
			button5.Enabled = false;
			button6.Enabled = false;
			button7.Enabled = false;

			panel2.Enabled = false;
			panel3.Enabled = false;
			panel4.Enabled = false;

			progressBar1.Visible = true;
			progressBar1.Value = 0;
		}
		void u_ChangeStatus(object sender, UpdateTaskResponse response) {
			label6.Text = response.Message;
		}
		void u_ChangePercent(object sender, UpdateTaskResponse response) {
			//if (progressBar1.Value != response.Value) { progressBar1.Value = response.Value; }
			progressBar1.Invoke((Action)(() => progressBar1.Value = response.Value));
		}
		void u_OnFinishUpdate(object sender, UpdateTaskResponse response) {
			button7.Enabled = true;
			button2.Enabled = true;
			button5.Enabled = true;
			button6.Enabled = true;

			panel2.Enabled = true;
			panel3.Enabled = true;
			panel4.Enabled = true;

			label6.Text = "";
			progressBar1.Visible = false;
		}

		void u_OnError(object sender, UpdateTaskResponse response) {
			MessageBox.Show(response.Message, "Update Error", MessageBoxButtons.OK);
		}

		private void SyncIncremental() {
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartIncremental();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}

		}

		private void SyncTotal() {
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartTotal();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}
		}

		private void SyncPhotos() {
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartPhotos();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}

		}
	

	
		private void timer1_Tick(object sender, EventArgs e) {
			this.SyncIncremental();
		}

		private void button2_Click(object sender, EventArgs e) {
			if (this.textBox1.Text != "" && !this.textBox1.Text.EndsWith("\\")) this.textBox1.Text += "\\";
			if (this.textBox2.Text != "" && !this.textBox2.Text.EndsWith("/")) this.textBox2.Text += "/";

			updaterFactuWeb.Properties.Settings.Default.path = this.textBox1.Text;
			updaterFactuWeb.Properties.Settings.Default.company = this.textBox5.Text;
			updaterFactuWeb.Properties.Settings.Default.year = this.textBox6.Text;
			updaterFactuWeb.Properties.Settings.Default.api = this.textBox2.Text;
			updaterFactuWeb.Properties.Settings.Default.token = this.textBox3.Text;
			updaterFactuWeb.Properties.Settings.Default.autoUpdate = checkBox1.Checked;
			updaterFactuWeb.Properties.Settings.Default.interval = Int32.Parse(this.textBox4.Text);
			updaterFactuWeb.Properties.Settings.Default.initmin = checkBox3.Checked;
			updaterFactuWeb.Properties.Settings.Default.Save();
			this.saveStatus();
			this.loadConfig();
		}

		private void textBox4_KeyPress(object sender, KeyPressEventArgs e) {
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
				(e.KeyChar != '.')) {
				e.Handled = true;
			}

			// only allow one decimal point
			//if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) {
			//	e.Handled = true;
			//}
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}

		private void textBox2_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}

		private void textBox3_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}

		private void textBox4_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}

		private void textBox6_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}

		private void textBox5_TextChanged(object sender, EventArgs e) {
			this.saveStatus();
		}
		private void checkBox3_CheckedChanged(object sender, EventArgs e) {
			this.saveStatus();
		}
		private void button3_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK) {
				textBox1.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button4_Click(object sender, EventArgs e) {
			// CLEAR
			DialogResult result = MessageBox.Show("Deseas eliminar la cache y hacer la actualización de cero?", "Atención", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes) {
				DialogResult result2 = MessageBox.Show("Eliminar tambien todos los datos del servidor web?", "Atención", MessageBoxButtons.YesNo);
				
				UpdateSqLite updateTask = new UpdateSqLite(true);

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

		private void button5_Click(object sender, EventArgs e) {
			//PHOTOS
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {		
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartPhotos();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("explorer.exe", @".");
		}

		private void button6_Click(object sender, EventArgs e) {
			// TOTAL
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartTotal();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}
		}

		private void button7_Click(object sender, EventArgs e) {
			// INCREMENTAL
			if (System.IO.File.Exists(Properties.Settings.Default.path + Properties.Settings.Default.company + Properties.Settings.Default.year + ".accdb")) {
				UpdateSqLite updateTask = new UpdateSqLite(false);

				updateTask.CallbackOnInitUpdate += u_OnInitUpdate;
				updateTask.CallbackOnFinishUpdate += u_OnFinishUpdate;
				updateTask.CallbackOnStatusChange += u_ChangeStatus;
				updateTask.CallbackOnPercentChange += u_ChangePercent;
				updateTask.CallbackOnError += u_OnError;

				updateTask.StartIncremental();
			} else {
				MessageBox.Show("No se ha podido acceder al archivo. Por favor revisa que la configuración sea correcta", "Error de acceso", MessageBoxButtons.OK);
			}
		}

		private void panelSave_VisibleChanged(object sender, EventArgs e) {
			panelActions.Visible = !panelSave.Visible;
		}

	
	}
}
