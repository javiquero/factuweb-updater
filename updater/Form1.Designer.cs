namespace updaterFactuWeb {
	partial class Form1 {
		/// <summary>
		/// Variable del diseñador necesaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Limpiar los recursos que se estén usando.
		/// </summary>
		/// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Código generado por el Diseñador de Windows Forms

		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido de este método con el editor de código.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panelConfigGlobal = new System.Windows.Forms.Panel();
            this.buttonRemoveCache = new System.Windows.Forms.Button();
            this.checkBoxInitMin = new System.Windows.Forms.CheckBox();
            this.checkBoxInitOnStartUp = new System.Windows.Forms.CheckBox();
            this.dateTimePickerTimeTotal = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.labelIntervaloIncrementales = new System.Windows.Forms.Label();
            this.buttonStartPhotos = new System.Windows.Forms.Button();
            this.linkOpenApplicationFolder = new System.Windows.Forms.LinkLabel();
            this.buttonStartTotal = new System.Windows.Forms.Button();
            this.buttonStartIncremental = new System.Windows.Forms.Button();
            this.buttonStartOrders = new System.Windows.Forms.Button();
            this.groupIncrementales = new System.Windows.Forms.GroupBox();
            this.labelStatusIncremental = new System.Windows.Forms.Label();
            this.progressBarIncremental = new System.Windows.Forms.ProgressBar();
            this.textBoxTimeIntervalo = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoIncrementales = new System.Windows.Forms.CheckBox();
            this.groupBoxFotos = new System.Windows.Forms.GroupBox();
            this.labelStatusFotos = new System.Windows.Forms.Label();
            this.textBoxTimeFotos = new System.Windows.Forms.NumericUpDown();
            this.progressBarFotos = new System.Windows.Forms.ProgressBar();
            this.checkBoxAutoFotos = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxPedidos = new System.Windows.Forms.GroupBox();
            this.labelStatusPedidos = new System.Windows.Forms.Label();
            this.progressBarPedidos = new System.Windows.Forms.ProgressBar();
            this.textBoxTimePedidos = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoPedidos = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxConfigFactusol = new System.Windows.Forms.GroupBox();
            this.textBoxYearFactusol = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxEmpresaFactusol = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonExploreRutaFactusol = new System.Windows.Forms.Button();
            this.textBoxRutaFactusol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxConfigApi = new System.Windows.Forms.GroupBox();
            this.textBoxConfigTokenApi = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxConfigDirApi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxTotal = new System.Windows.Forms.GroupBox();
            this.labelStatusTotal = new System.Windows.Forms.Label();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.checkBoxAutoTotal = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.timerIncrementales = new System.Windows.Forms.Timer(this.components);
            this.timerTotal = new System.Windows.Forms.Timer(this.components);
            this.timerPedidos = new System.Windows.Forms.Timer(this.components);
            this.timerFotos = new System.Windows.Forms.Timer(this.components);
            this.labelPathExists = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panelConfigGlobal.SuspendLayout();
            this.groupIncrementales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimeIntervalo)).BeginInit();
            this.groupBoxFotos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimeFotos)).BeginInit();
            this.groupBoxPedidos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimePedidos)).BeginInit();
            this.groupBoxConfigFactusol.SuspendLayout();
            this.groupBoxConfigApi.SuspendLayout();
            this.groupBoxTotal.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 16);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1001, 190);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(13, 719);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(54, 18);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Ver log";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.toolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 76);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem1.Text = "Mostrar ventana";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem2.Text = "Actualizar ahora";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem3.Text = "Cerra programa";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Haz clic para modificar la configuración";
            this.notifyIcon1.BalloonTipTitle = "factuWeb Updater";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "factuWeb Updater";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Factusol Data folder";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // panelConfigGlobal
            // 
            this.panelConfigGlobal.Controls.Add(this.buttonRemoveCache);
            this.panelConfigGlobal.Controls.Add(this.checkBoxInitMin);
            this.panelConfigGlobal.Controls.Add(this.checkBoxInitOnStartUp);
            this.panelConfigGlobal.Location = new System.Drawing.Point(4, 644);
            this.panelConfigGlobal.Name = "panelConfigGlobal";
            this.panelConfigGlobal.Size = new System.Drawing.Size(994, 75);
            this.panelConfigGlobal.TabIndex = 24;
            // 
            // buttonRemoveCache
            // 
            this.buttonRemoveCache.BackColor = System.Drawing.Color.Red;
            this.buttonRemoveCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemoveCache.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveCache.ForeColor = System.Drawing.Color.White;
            this.buttonRemoveCache.Location = new System.Drawing.Point(856, 27);
            this.buttonRemoveCache.Name = "buttonRemoveCache";
            this.buttonRemoveCache.Size = new System.Drawing.Size(125, 38);
            this.buttonRemoveCache.TabIndex = 21;
            this.buttonRemoveCache.Text = "Borrar cache";
            this.buttonRemoveCache.UseVisualStyleBackColor = false;
            this.buttonRemoveCache.Click += new System.EventHandler(this.buttonRemoveCache_Click);
            // 
            // checkBoxInitMin
            // 
            this.checkBoxInitMin.AutoSize = true;
            this.checkBoxInitMin.Location = new System.Drawing.Point(12, 43);
            this.checkBoxInitMin.Name = "checkBoxInitMin";
            this.checkBoxInitMin.Size = new System.Drawing.Size(145, 22);
            this.checkBoxInitMin.TabIndex = 20;
            this.checkBoxInitMin.Text = "Iniciar minimizado";
            this.checkBoxInitMin.UseVisualStyleBackColor = true;
            this.checkBoxInitMin.CheckedChanged += new System.EventHandler(this.checkBoxInitMin_CheckedChanged);
            // 
            // checkBoxInitOnStartUp
            // 
            this.checkBoxInitOnStartUp.AutoSize = true;
            this.checkBoxInitOnStartUp.Location = new System.Drawing.Point(12, 15);
            this.checkBoxInitOnStartUp.Name = "checkBoxInitOnStartUp";
            this.checkBoxInitOnStartUp.Size = new System.Drawing.Size(270, 22);
            this.checkBoxInitOnStartUp.TabIndex = 18;
            this.checkBoxInitOnStartUp.Text = "Iniciar aplicación al inicio de windows";
            this.checkBoxInitOnStartUp.UseVisualStyleBackColor = true;
            this.checkBoxInitOnStartUp.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // dateTimePickerTimeTotal
            // 
            this.dateTimePickerTimeTotal.CustomFormat = "HH:mm";
            this.dateTimePickerTimeTotal.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerTimeTotal.Location = new System.Drawing.Point(74, 64);
            this.dateTimePickerTimeTotal.Name = "dateTimePickerTimeTotal";
            this.dateTimePickerTimeTotal.ShowUpDown = true;
            this.dateTimePickerTimeTotal.Size = new System.Drawing.Size(97, 24);
            this.dateTimePickerTimeTotal.TabIndex = 6;
            this.dateTimePickerTimeTotal.Value = new System.DateTime(2020, 7, 7, 20, 42, 0, 0);
            this.dateTimePickerTimeTotal.ValueChanged += new System.EventHandler(this.dateTimePickerTimeTotal_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(177, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "minutos.";
            // 
            // labelIntervaloIncrementales
            // 
            this.labelIntervaloIncrementales.AutoSize = true;
            this.labelIntervaloIncrementales.Location = new System.Drawing.Point(23, 67);
            this.labelIntervaloIncrementales.Name = "labelIntervaloIncrementales";
            this.labelIntervaloIncrementales.Size = new System.Drawing.Size(67, 18);
            this.labelIntervaloIncrementales.TabIndex = 5;
            this.labelIntervaloIncrementales.Text = "Intervalo:";
            // 
            // buttonStartPhotos
            // 
            this.buttonStartPhotos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartPhotos.Location = new System.Drawing.Point(302, 35);
            this.buttonStartPhotos.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStartPhotos.Name = "buttonStartPhotos";
            this.buttonStartPhotos.Size = new System.Drawing.Size(180, 50);
            this.buttonStartPhotos.TabIndex = 26;
            this.buttonStartPhotos.Text = "Actualizar ahora";
            this.buttonStartPhotos.UseVisualStyleBackColor = true;
            this.buttonStartPhotos.Click += new System.EventHandler(this.buttonStartPhotos_Click);
            // 
            // linkOpenApplicationFolder
            // 
            this.linkOpenApplicationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkOpenApplicationFolder.AutoSize = true;
            this.linkOpenApplicationFolder.Location = new System.Drawing.Point(427, 719);
            this.linkOpenApplicationFolder.Name = "linkOpenApplicationFolder";
            this.linkOpenApplicationFolder.Size = new System.Drawing.Size(196, 18);
            this.linkOpenApplicationFolder.TabIndex = 27;
            this.linkOpenApplicationFolder.TabStop = true;
            this.linkOpenApplicationFolder.Text = "Abrir carpeta de la aplicación";
            this.linkOpenApplicationFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOpenApplicationFolder_LinkClicked);
            // 
            // buttonStartTotal
            // 
            this.buttonStartTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartTotal.Location = new System.Drawing.Point(302, 33);
            this.buttonStartTotal.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStartTotal.Name = "buttonStartTotal";
            this.buttonStartTotal.Size = new System.Drawing.Size(180, 55);
            this.buttonStartTotal.TabIndex = 28;
            this.buttonStartTotal.Text = "Actualizar ahora";
            this.buttonStartTotal.UseVisualStyleBackColor = true;
            this.buttonStartTotal.Click += new System.EventHandler(this.buttonStartTotal_Click);
            // 
            // buttonStartIncremental
            // 
            this.buttonStartIncremental.Location = new System.Drawing.Point(302, 33);
            this.buttonStartIncremental.Name = "buttonStartIncremental";
            this.buttonStartIncremental.Size = new System.Drawing.Size(180, 52);
            this.buttonStartIncremental.TabIndex = 29;
            this.buttonStartIncremental.Text = "Actualizar ahora";
            this.buttonStartIncremental.UseVisualStyleBackColor = true;
            this.buttonStartIncremental.Click += new System.EventHandler(this.buttonStartIncremental_Click);
            // 
            // buttonStartOrders
            // 
            this.buttonStartOrders.Location = new System.Drawing.Point(302, 33);
            this.buttonStartOrders.Name = "buttonStartOrders";
            this.buttonStartOrders.Size = new System.Drawing.Size(180, 52);
            this.buttonStartOrders.TabIndex = 30;
            this.buttonStartOrders.Text = "Descargar ahora";
            this.buttonStartOrders.UseVisualStyleBackColor = true;
            this.buttonStartOrders.Click += new System.EventHandler(this.buttonStartOrders_Click);
            // 
            // groupIncrementales
            // 
            this.groupIncrementales.Controls.Add(this.labelStatusIncremental);
            this.groupIncrementales.Controls.Add(this.progressBarIncremental);
            this.groupIncrementales.Controls.Add(this.textBoxTimeIntervalo);
            this.groupIncrementales.Controls.Add(this.checkBoxAutoIncrementales);
            this.groupIncrementales.Controls.Add(this.label5);
            this.groupIncrementales.Controls.Add(this.buttonStartIncremental);
            this.groupIncrementales.Controls.Add(this.labelIntervaloIncrementales);
            this.groupIncrementales.Location = new System.Drawing.Point(2, 366);
            this.groupIncrementales.Name = "groupIncrementales";
            this.groupIncrementales.Size = new System.Drawing.Size(495, 121);
            this.groupIncrementales.TabIndex = 32;
            this.groupIncrementales.TabStop = false;
            this.groupIncrementales.Text = "Actualizaciones incrementales";
            // 
            // labelStatusIncremental
            // 
            this.labelStatusIncremental.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatusIncremental.AutoSize = true;
            this.labelStatusIncremental.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusIncremental.Location = new System.Drawing.Point(15, 92);
            this.labelStatusIncremental.Name = "labelStatusIncremental";
            this.labelStatusIncremental.Size = new System.Drawing.Size(0, 15);
            this.labelStatusIncremental.TabIndex = 32;
            // 
            // progressBarIncremental
            // 
            this.progressBarIncremental.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarIncremental.Location = new System.Drawing.Point(0, 111);
            this.progressBarIncremental.Name = "progressBarIncremental";
            this.progressBarIncremental.Size = new System.Drawing.Size(497, 10);
            this.progressBarIncremental.Step = 1;
            this.progressBarIncremental.TabIndex = 31;
            // 
            // textBoxTimeIntervalo
            // 
            this.textBoxTimeIntervalo.Location = new System.Drawing.Point(96, 64);
            this.textBoxTimeIntervalo.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textBoxTimeIntervalo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textBoxTimeIntervalo.Name = "textBoxTimeIntervalo";
            this.textBoxTimeIntervalo.Size = new System.Drawing.Size(75, 24);
            this.textBoxTimeIntervalo.TabIndex = 30;
            this.textBoxTimeIntervalo.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.textBoxTimeIntervalo.ValueChanged += new System.EventHandler(this.textBoxTimeIntervalo_ValueChanged);
            // 
            // checkBoxAutoIncrementales
            // 
            this.checkBoxAutoIncrementales.AutoSize = true;
            this.checkBoxAutoIncrementales.Location = new System.Drawing.Point(26, 33);
            this.checkBoxAutoIncrementales.Name = "checkBoxAutoIncrementales";
            this.checkBoxAutoIncrementales.Size = new System.Drawing.Size(209, 22);
            this.checkBoxAutoIncrementales.TabIndex = 0;
            this.checkBoxAutoIncrementales.Text = "Actualizar automáticamente";
            this.checkBoxAutoIncrementales.UseVisualStyleBackColor = true;
            this.checkBoxAutoIncrementales.CheckedChanged += new System.EventHandler(this.checkBoxAutoIncrementales_CheckedChanged);
            // 
            // groupBoxFotos
            // 
            this.groupBoxFotos.Controls.Add(this.labelStatusFotos);
            this.groupBoxFotos.Controls.Add(this.textBoxTimeFotos);
            this.groupBoxFotos.Controls.Add(this.progressBarFotos);
            this.groupBoxFotos.Controls.Add(this.buttonStartPhotos);
            this.groupBoxFotos.Controls.Add(this.checkBoxAutoFotos);
            this.groupBoxFotos.Controls.Add(this.label9);
            this.groupBoxFotos.Controls.Add(this.label10);
            this.groupBoxFotos.Location = new System.Drawing.Point(505, 366);
            this.groupBoxFotos.Name = "groupBoxFotos";
            this.groupBoxFotos.Size = new System.Drawing.Size(495, 121);
            this.groupBoxFotos.TabIndex = 33;
            this.groupBoxFotos.TabStop = false;
            this.groupBoxFotos.Text = "Fotos";
            // 
            // labelStatusFotos
            // 
            this.labelStatusFotos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatusFotos.AutoSize = true;
            this.labelStatusFotos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusFotos.Location = new System.Drawing.Point(15, 92);
            this.labelStatusFotos.Name = "labelStatusFotos";
            this.labelStatusFotos.Size = new System.Drawing.Size(0, 15);
            this.labelStatusFotos.TabIndex = 34;
            // 
            // textBoxTimeFotos
            // 
            this.textBoxTimeFotos.Location = new System.Drawing.Point(96, 64);
            this.textBoxTimeFotos.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textBoxTimeFotos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textBoxTimeFotos.Name = "textBoxTimeFotos";
            this.textBoxTimeFotos.Size = new System.Drawing.Size(75, 24);
            this.textBoxTimeFotos.TabIndex = 27;
            this.textBoxTimeFotos.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.textBoxTimeFotos.ValueChanged += new System.EventHandler(this.textBoxTimeFotos_ValueChanged);
            // 
            // progressBarFotos
            // 
            this.progressBarFotos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarFotos.Location = new System.Drawing.Point(0, 111);
            this.progressBarFotos.Name = "progressBarFotos";
            this.progressBarFotos.Size = new System.Drawing.Size(497, 10);
            this.progressBarFotos.Step = 1;
            this.progressBarFotos.TabIndex = 33;
            // 
            // checkBoxAutoFotos
            // 
            this.checkBoxAutoFotos.AutoSize = true;
            this.checkBoxAutoFotos.Location = new System.Drawing.Point(26, 33);
            this.checkBoxAutoFotos.Name = "checkBoxAutoFotos";
            this.checkBoxAutoFotos.Size = new System.Drawing.Size(209, 22);
            this.checkBoxAutoFotos.TabIndex = 0;
            this.checkBoxAutoFotos.Text = "Actualizar automáticamente";
            this.checkBoxAutoFotos.UseVisualStyleBackColor = true;
            this.checkBoxAutoFotos.CheckedChanged += new System.EventHandler(this.checkBoxAutoFotos_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(176, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 18);
            this.label9.TabIndex = 3;
            this.label9.Text = "minutos.";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(23, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 18);
            this.label10.TabIndex = 5;
            this.label10.Text = "Intervalo:";
            // 
            // groupBoxPedidos
            // 
            this.groupBoxPedidos.Controls.Add(this.labelStatusPedidos);
            this.groupBoxPedidos.Controls.Add(this.progressBarPedidos);
            this.groupBoxPedidos.Controls.Add(this.textBoxTimePedidos);
            this.groupBoxPedidos.Controls.Add(this.buttonStartOrders);
            this.groupBoxPedidos.Controls.Add(this.checkBoxAutoPedidos);
            this.groupBoxPedidos.Controls.Add(this.label11);
            this.groupBoxPedidos.Controls.Add(this.label12);
            this.groupBoxPedidos.Location = new System.Drawing.Point(505, 493);
            this.groupBoxPedidos.Name = "groupBoxPedidos";
            this.groupBoxPedidos.Size = new System.Drawing.Size(495, 134);
            this.groupBoxPedidos.TabIndex = 34;
            this.groupBoxPedidos.TabStop = false;
            this.groupBoxPedidos.Text = "Pedidos";
            // 
            // labelStatusPedidos
            // 
            this.labelStatusPedidos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatusPedidos.AutoSize = true;
            this.labelStatusPedidos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusPedidos.Location = new System.Drawing.Point(16, 105);
            this.labelStatusPedidos.Name = "labelStatusPedidos";
            this.labelStatusPedidos.Size = new System.Drawing.Size(0, 15);
            this.labelStatusPedidos.TabIndex = 34;
            // 
            // progressBarPedidos
            // 
            this.progressBarPedidos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarPedidos.Location = new System.Drawing.Point(0, 123);
            this.progressBarPedidos.Name = "progressBarPedidos";
            this.progressBarPedidos.Size = new System.Drawing.Size(497, 10);
            this.progressBarPedidos.Step = 1;
            this.progressBarPedidos.TabIndex = 33;
            // 
            // textBoxTimePedidos
            // 
            this.textBoxTimePedidos.Location = new System.Drawing.Point(96, 64);
            this.textBoxTimePedidos.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textBoxTimePedidos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textBoxTimePedidos.Name = "textBoxTimePedidos";
            this.textBoxTimePedidos.Size = new System.Drawing.Size(75, 24);
            this.textBoxTimePedidos.TabIndex = 31;
            this.textBoxTimePedidos.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.textBoxTimePedidos.ValueChanged += new System.EventHandler(this.textBoxTimePedidos_ValueChanged);
            // 
            // checkBoxAutoPedidos
            // 
            this.checkBoxAutoPedidos.AutoSize = true;
            this.checkBoxAutoPedidos.Location = new System.Drawing.Point(26, 33);
            this.checkBoxAutoPedidos.Name = "checkBoxAutoPedidos";
            this.checkBoxAutoPedidos.Size = new System.Drawing.Size(214, 22);
            this.checkBoxAutoPedidos.TabIndex = 0;
            this.checkBoxAutoPedidos.Text = "Descargar automáticamente";
            this.checkBoxAutoPedidos.UseVisualStyleBackColor = true;
            this.checkBoxAutoPedidos.CheckedChanged += new System.EventHandler(this.checkBoxAutoPedidos_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(177, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 18);
            this.label11.TabIndex = 3;
            this.label11.Text = "minutos.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 67);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 18);
            this.label12.TabIndex = 5;
            this.label12.Text = "Intervalo:";
            // 
            // groupBoxConfigFactusol
            // 
            this.groupBoxConfigFactusol.Controls.Add(this.textBoxYearFactusol);
            this.groupBoxConfigFactusol.Controls.Add(this.label8);
            this.groupBoxConfigFactusol.Controls.Add(this.textBoxEmpresaFactusol);
            this.groupBoxConfigFactusol.Controls.Add(this.label7);
            this.groupBoxConfigFactusol.Controls.Add(this.buttonExploreRutaFactusol);
            this.groupBoxConfigFactusol.Controls.Add(this.textBoxRutaFactusol);
            this.groupBoxConfigFactusol.Controls.Add(this.label1);
            this.groupBoxConfigFactusol.Controls.Add(this.labelPathExists);
            this.groupBoxConfigFactusol.Location = new System.Drawing.Point(2, 213);
            this.groupBoxConfigFactusol.Name = "groupBoxConfigFactusol";
            this.groupBoxConfigFactusol.Size = new System.Drawing.Size(495, 147);
            this.groupBoxConfigFactusol.TabIndex = 35;
            this.groupBoxConfigFactusol.TabStop = false;
            this.groupBoxConfigFactusol.Text = "Configuración de Factusol";
            // 
            // textBoxYearFactusol
            // 
            this.textBoxYearFactusol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxYearFactusol.Location = new System.Drawing.Point(287, 108);
            this.textBoxYearFactusol.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxYearFactusol.Name = "textBoxYearFactusol";
            this.textBoxYearFactusol.Size = new System.Drawing.Size(195, 24);
            this.textBoxYearFactusol.TabIndex = 32;
            this.textBoxYearFactusol.TextChanged += new System.EventHandler(this.textBoxYearFactusol_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(287, 86);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 18);
            this.label8.TabIndex = 33;
            this.label8.Text = "Año";
            // 
            // textBoxEmpresaFactusol
            // 
            this.textBoxEmpresaFactusol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEmpresaFactusol.Location = new System.Drawing.Point(26, 108);
            this.textBoxEmpresaFactusol.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxEmpresaFactusol.Name = "textBoxEmpresaFactusol";
            this.textBoxEmpresaFactusol.Size = new System.Drawing.Size(253, 24);
            this.textBoxEmpresaFactusol.TabIndex = 30;
            this.textBoxEmpresaFactusol.TextChanged += new System.EventHandler(this.textBoxEmpresaFactusol_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 86);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 18);
            this.label7.TabIndex = 31;
            this.label7.Text = "Empresa";
            // 
            // buttonExploreRutaFactusol
            // 
            this.buttonExploreRutaFactusol.Location = new System.Drawing.Point(444, 57);
            this.buttonExploreRutaFactusol.Name = "buttonExploreRutaFactusol";
            this.buttonExploreRutaFactusol.Size = new System.Drawing.Size(38, 26);
            this.buttonExploreRutaFactusol.TabIndex = 29;
            this.buttonExploreRutaFactusol.Text = "...";
            this.buttonExploreRutaFactusol.UseVisualStyleBackColor = true;
            this.buttonExploreRutaFactusol.Click += new System.EventHandler(this.buttonExploreRutaFactusol_Click);
            // 
            // textBoxRutaFactusol
            // 
            this.textBoxRutaFactusol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRutaFactusol.Location = new System.Drawing.Point(26, 58);
            this.textBoxRutaFactusol.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxRutaFactusol.Name = "textBoxRutaFactusol";
            this.textBoxRutaFactusol.Size = new System.Drawing.Size(419, 24);
            this.textBoxRutaFactusol.TabIndex = 27;
            this.textBoxRutaFactusol.TextChanged += new System.EventHandler(this.textBoxRutaFactusol_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 18);
            this.label1.TabIndex = 28;
            this.label1.Text = "Ruta datos Factusol";
            // 
            // groupBoxConfigApi
            // 
            this.groupBoxConfigApi.Controls.Add(this.textBoxConfigTokenApi);
            this.groupBoxConfigApi.Controls.Add(this.label3);
            this.groupBoxConfigApi.Controls.Add(this.textBoxConfigDirApi);
            this.groupBoxConfigApi.Controls.Add(this.label2);
            this.groupBoxConfigApi.Location = new System.Drawing.Point(505, 213);
            this.groupBoxConfigApi.Name = "groupBoxConfigApi";
            this.groupBoxConfigApi.Size = new System.Drawing.Size(495, 147);
            this.groupBoxConfigApi.TabIndex = 36;
            this.groupBoxConfigApi.TabStop = false;
            this.groupBoxConfigApi.Text = "Configuración de la API FactuWeb";
            // 
            // textBoxConfigTokenApi
            // 
            this.textBoxConfigTokenApi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxConfigTokenApi.Location = new System.Drawing.Point(26, 108);
            this.textBoxConfigTokenApi.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxConfigTokenApi.Name = "textBoxConfigTokenApi";
            this.textBoxConfigTokenApi.Size = new System.Drawing.Size(456, 24);
            this.textBoxConfigTokenApi.TabIndex = 18;
            this.textBoxConfigTokenApi.TextChanged += new System.EventHandler(this.textBoxConfigTokenApi_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 18);
            this.label3.TabIndex = 20;
            this.label3.Text = "Token API";
            // 
            // textBoxConfigDirApi
            // 
            this.textBoxConfigDirApi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxConfigDirApi.Location = new System.Drawing.Point(26, 57);
            this.textBoxConfigDirApi.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxConfigDirApi.Name = "textBoxConfigDirApi";
            this.textBoxConfigDirApi.Size = new System.Drawing.Size(456, 24);
            this.textBoxConfigDirApi.TabIndex = 17;
            this.textBoxConfigDirApi.TextChanged += new System.EventHandler(this.textBoxConfigDirApi_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 18);
            this.label2.TabIndex = 19;
            this.label2.Text = "Dirección API";
            // 
            // groupBoxTotal
            // 
            this.groupBoxTotal.Controls.Add(this.labelStatusTotal);
            this.groupBoxTotal.Controls.Add(this.buttonStartTotal);
            this.groupBoxTotal.Controls.Add(this.progressBarTotal);
            this.groupBoxTotal.Controls.Add(this.dateTimePickerTimeTotal);
            this.groupBoxTotal.Controls.Add(this.checkBoxAutoTotal);
            this.groupBoxTotal.Controls.Add(this.label14);
            this.groupBoxTotal.Location = new System.Drawing.Point(2, 493);
            this.groupBoxTotal.Name = "groupBoxTotal";
            this.groupBoxTotal.Size = new System.Drawing.Size(495, 134);
            this.groupBoxTotal.TabIndex = 35;
            this.groupBoxTotal.TabStop = false;
            this.groupBoxTotal.Text = "Actualización total";
            // 
            // labelStatusTotal
            // 
            this.labelStatusTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatusTotal.AutoSize = true;
            this.labelStatusTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusTotal.Location = new System.Drawing.Point(15, 105);
            this.labelStatusTotal.Name = "labelStatusTotal";
            this.labelStatusTotal.Size = new System.Drawing.Size(0, 15);
            this.labelStatusTotal.TabIndex = 34;
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTotal.Location = new System.Drawing.Point(0, 124);
            this.progressBarTotal.Name = "progressBarTotal";
            this.progressBarTotal.Size = new System.Drawing.Size(497, 10);
            this.progressBarTotal.Step = 1;
            this.progressBarTotal.TabIndex = 33;
            // 
            // checkBoxAutoTotal
            // 
            this.checkBoxAutoTotal.AutoSize = true;
            this.checkBoxAutoTotal.Location = new System.Drawing.Point(26, 33);
            this.checkBoxAutoTotal.Name = "checkBoxAutoTotal";
            this.checkBoxAutoTotal.Size = new System.Drawing.Size(209, 22);
            this.checkBoxAutoTotal.TabIndex = 0;
            this.checkBoxAutoTotal.Text = "Actualizar automáticamente";
            this.checkBoxAutoTotal.UseVisualStyleBackColor = true;
            this.checkBoxAutoTotal.CheckedChanged += new System.EventHandler(this.checkBoxAutoTotal_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(23, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 18);
            this.label14.TabIndex = 5;
            this.label14.Text = "Hora:";
            // 
            // timerIncrementales
            // 
            this.timerIncrementales.Tick += new System.EventHandler(this.timerIncrementales_Tick);
            // 
            // timerTotal
            // 
            this.timerTotal.Tick += new System.EventHandler(this.timerTotal_Tick);
            // 
            // timerPedidos
            // 
            this.timerPedidos.Tick += new System.EventHandler(this.timerPedidos_Tick);
            // 
            // timerFotos
            // 
            this.timerFotos.Tick += new System.EventHandler(this.timerFotos_Tick);
            // 
            // labelPathExists
            // 
            this.labelPathExists.AutoSize = true;
            this.labelPathExists.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPathExists.Location = new System.Drawing.Point(174, 36);
            this.labelPathExists.Name = "labelPathExists";
            this.labelPathExists.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelPathExists.Size = new System.Drawing.Size(0, 18);
            this.labelPathExists.TabIndex = 34;
            this.labelPathExists.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1004, 746);
            this.Controls.Add(this.groupBoxTotal);
            this.Controls.Add(this.groupBoxConfigApi);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkOpenApplicationFolder);
            this.Controls.Add(this.groupBoxConfigFactusol);
            this.Controls.Add(this.groupBoxPedidos);
            this.Controls.Add(this.groupBoxFotos);
            this.Controls.Add(this.groupIncrementales);
            this.Controls.Add(this.panelConfigGlobal);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "factuWeb Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panelConfigGlobal.ResumeLayout(false);
            this.panelConfigGlobal.PerformLayout();
            this.groupIncrementales.ResumeLayout(false);
            this.groupIncrementales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimeIntervalo)).EndInit();
            this.groupBoxFotos.ResumeLayout(false);
            this.groupBoxFotos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimeFotos)).EndInit();
            this.groupBoxPedidos.ResumeLayout(false);
            this.groupBoxPedidos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTimePedidos)).EndInit();
            this.groupBoxConfigFactusol.ResumeLayout(false);
            this.groupBoxConfigFactusol.PerformLayout();
            this.groupBoxConfigApi.ResumeLayout(false);
            this.groupBoxConfigApi.PerformLayout();
            this.groupBoxTotal.ResumeLayout(false);
            this.groupBoxTotal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.Panel panelConfigGlobal;
		private System.Windows.Forms.CheckBox checkBoxInitMin;
		private System.Windows.Forms.CheckBox checkBoxInitOnStartUp;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonRemoveCache;
        private System.Windows.Forms.Button buttonStartPhotos;
        private System.Windows.Forms.LinkLabel linkOpenApplicationFolder;
        private System.Windows.Forms.Button buttonStartTotal;
        private System.Windows.Forms.Button buttonStartIncremental;
        private System.Windows.Forms.DateTimePicker dateTimePickerTimeTotal;
        private System.Windows.Forms.Label labelIntervaloIncrementales;
        private System.Windows.Forms.Button buttonStartOrders;
        private System.Windows.Forms.GroupBox groupIncrementales;
        private System.Windows.Forms.CheckBox checkBoxAutoIncrementales;
        private System.Windows.Forms.GroupBox groupBoxFotos;
        private System.Windows.Forms.CheckBox checkBoxAutoFotos;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBoxPedidos;
        private System.Windows.Forms.CheckBox checkBoxAutoPedidos;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBoxConfigFactusol;
        private System.Windows.Forms.TextBox textBoxYearFactusol;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxEmpresaFactusol;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonExploreRutaFactusol;
        private System.Windows.Forms.TextBox textBoxRutaFactusol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxConfigApi;
        private System.Windows.Forms.TextBox textBoxConfigTokenApi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxConfigDirApi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxTotal;
        private System.Windows.Forms.CheckBox checkBoxAutoTotal;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown textBoxTimeIntervalo;
        private System.Windows.Forms.NumericUpDown textBoxTimeFotos;
        private System.Windows.Forms.NumericUpDown textBoxTimePedidos;
        private System.Windows.Forms.Timer timerIncrementales;
        private System.Windows.Forms.Timer timerTotal;
        private System.Windows.Forms.Timer timerPedidos;
        private System.Windows.Forms.Timer timerFotos;
        private System.Windows.Forms.Label labelStatusIncremental;
        private System.Windows.Forms.ProgressBar progressBarIncremental;
        private System.Windows.Forms.Label labelStatusFotos;
        private System.Windows.Forms.ProgressBar progressBarFotos;
        private System.Windows.Forms.Label labelStatusPedidos;
        private System.Windows.Forms.ProgressBar progressBarPedidos;
        private System.Windows.Forms.Label labelStatusTotal;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.Label labelPathExists;
    }
}

