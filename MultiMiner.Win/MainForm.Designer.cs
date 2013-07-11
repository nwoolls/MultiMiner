namespace MultiMiner.Win
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.deviceStatsTimer = new System.Windows.Forms.Timer(this.components);
            this.coinStatsTimer = new System.Windows.Forms.Timer(this.components);
            this.startupMiningTimer = new System.Windows.Forms.Timer(this.components);
            this.startupMiningCountdownTimer = new System.Windows.Forms.Timer(this.components);
            this.startupMiningPanel = new System.Windows.Forms.Panel();
            this.cancelStartupMiningButton = new System.Windows.Forms.Button();
            this.countdownLabel = new System.Windows.Forms.Label();
            this.crashRecoveryTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.coinsButton = new System.Windows.Forms.ToolStripButton();
            this.strategiesButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.detectDevicesButton = new System.Windows.Forms.ToolStripButton();
            this.apiMonitorButton = new System.Windows.Forms.ToolStripButton();
            this.desktopModeButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.backendLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.strategiesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.strategyCountdownLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sha256RateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.scryptRateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.coinStatsCountdownTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.coinStatsLabel = new System.Windows.Forms.Label();
            this.coinChooseSuffixLabel = new System.Windows.Forms.Label();
            this.coinChooseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.coinChoosePrefixLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.deviceGridView = new System.Windows.Forms.DataGridView();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.apiLogGridView = new System.Windows.Forms.DataGridView();
            this.CoinName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.closeApiButton = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showAppMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitAppMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requestDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.responseDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.apiLogEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kindDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coinColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.difficultyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profitabilityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperatureColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hashRateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rejectedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intensityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startupMiningPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.apiLogGridView)).BeginInit();
            this.panel2.SuspendLayout();
            this.notifyIconMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.apiLogEntryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // deviceStatsTimer
            // 
            this.deviceStatsTimer.Interval = 5000;
            this.deviceStatsTimer.Tick += new System.EventHandler(this.statsTimer_Tick);
            // 
            // coinStatsTimer
            // 
            this.coinStatsTimer.Enabled = true;
            this.coinStatsTimer.Interval = 900000;
            this.coinStatsTimer.Tick += new System.EventHandler(this.coinStatsTimer_Tick);
            // 
            // startupMiningTimer
            // 
            this.startupMiningTimer.Tick += new System.EventHandler(this.startupMiningTimer_Tick);
            // 
            // startupMiningCountdownTimer
            // 
            this.startupMiningCountdownTimer.Interval = 1000;
            this.startupMiningCountdownTimer.Tick += new System.EventHandler(this.countdownTimer_Tick);
            // 
            // startupMiningPanel
            // 
            this.startupMiningPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startupMiningPanel.Controls.Add(this.cancelStartupMiningButton);
            this.startupMiningPanel.Controls.Add(this.countdownLabel);
            this.startupMiningPanel.Location = new System.Drawing.Point(417, 199);
            this.startupMiningPanel.Name = "startupMiningPanel";
            this.startupMiningPanel.Size = new System.Drawing.Size(319, 37);
            this.startupMiningPanel.TabIndex = 6;
            this.startupMiningPanel.Visible = false;
            // 
            // cancelStartupMiningButton
            // 
            this.cancelStartupMiningButton.Location = new System.Drawing.Point(229, 8);
            this.cancelStartupMiningButton.Name = "cancelStartupMiningButton";
            this.cancelStartupMiningButton.Size = new System.Drawing.Size(75, 23);
            this.cancelStartupMiningButton.TabIndex = 5;
            this.cancelStartupMiningButton.Text = "Cancel";
            this.cancelStartupMiningButton.UseVisualStyleBackColor = true;
            this.cancelStartupMiningButton.Click += new System.EventHandler(this.cancelStartupMiningButton_Click);
            // 
            // countdownLabel
            // 
            this.countdownLabel.AutoSize = true;
            this.countdownLabel.Location = new System.Drawing.Point(3, 13);
            this.countdownLabel.Name = "countdownLabel";
            this.countdownLabel.Size = new System.Drawing.Size(220, 13);
            this.countdownLabel.TabIndex = 0;
            this.countdownLabel.Text = "Mining will start automatically in 45 seconds...";
            // 
            // crashRecoveryTimer
            // 
            this.crashRecoveryTimer.Interval = 30000;
            this.crashRecoveryTimer.Tick += new System.EventHandler(this.crashRecoveryTimer_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startButton,
            this.stopButton,
            this.toolStripSeparator2,
            this.coinsButton,
            this.strategiesButton,
            this.settingsButton,
            this.toolStripSeparator3,
            this.detectDevicesButton,
            this.apiMonitorButton,
            this.desktopModeButton,
            this.toolStripSeparator1,
            this.saveButton,
            this.cancelButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1153, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // startButton
            // 
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(51, 22);
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(51, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // coinsButton
            // 
            this.coinsButton.Image = ((System.Drawing.Image)(resources.GetObject("coinsButton.Image")));
            this.coinsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.coinsButton.Name = "coinsButton";
            this.coinsButton.Size = new System.Drawing.Size(57, 22);
            this.coinsButton.Text = "Coins";
            this.coinsButton.Click += new System.EventHandler(this.coinsButton_Click);
            // 
            // strategiesButton
            // 
            this.strategiesButton.Image = ((System.Drawing.Image)(resources.GetObject("strategiesButton.Image")));
            this.strategiesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.strategiesButton.Name = "strategiesButton";
            this.strategiesButton.Size = new System.Drawing.Size(78, 22);
            this.strategiesButton.Text = "Strategies";
            this.strategiesButton.Click += new System.EventHandler(this.strategiesButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(69, 22);
            this.settingsButton.Text = "Settings";
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // detectDevicesButton
            // 
            this.detectDevicesButton.Image = ((System.Drawing.Image)(resources.GetObject("detectDevicesButton.Image")));
            this.detectDevicesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.detectDevicesButton.Name = "detectDevicesButton";
            this.detectDevicesButton.Size = new System.Drawing.Size(104, 22);
            this.detectDevicesButton.Text = "Detect Devices";
            this.detectDevicesButton.Click += new System.EventHandler(this.detectDevicesButton_Click);
            // 
            // apiMonitorButton
            // 
            this.apiMonitorButton.CheckOnClick = true;
            this.apiMonitorButton.Image = ((System.Drawing.Image)(resources.GetObject("apiMonitorButton.Image")));
            this.apiMonitorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.apiMonitorButton.Name = "apiMonitorButton";
            this.apiMonitorButton.Size = new System.Drawing.Size(91, 22);
            this.apiMonitorButton.Text = "API Monitor";
            this.apiMonitorButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // desktopModeButton
            // 
            this.desktopModeButton.CheckOnClick = true;
            this.desktopModeButton.Image = ((System.Drawing.Image)(resources.GetObject("desktopModeButton.Image")));
            this.desktopModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.desktopModeButton.Name = "desktopModeButton";
            this.desktopModeButton.Size = new System.Drawing.Size(104, 22);
            this.desktopModeButton.Text = "Desktop Mode";
            this.desktopModeButton.Click += new System.EventHandler(this.desktopModeButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(51, 22);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Enabled = false;
            this.cancelButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton.Image")));
            this.cancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(63, 22);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backendLabel,
            this.strategiesLabel,
            this.strategyCountdownLabel,
            this.sha256RateLabel,
            this.scryptRateLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 412);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1153, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // backendLabel
            // 
            this.backendLabel.AutoSize = false;
            this.backendLabel.Name = "backendLabel";
            this.backendLabel.Size = new System.Drawing.Size(150, 17);
            this.backendLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // strategiesLabel
            // 
            this.strategiesLabel.AutoSize = false;
            this.strategiesLabel.Name = "strategiesLabel";
            this.strategiesLabel.Size = new System.Drawing.Size(150, 17);
            this.strategiesLabel.Text = "Strategies: enabled";
            this.strategiesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // strategyCountdownLabel
            // 
            this.strategyCountdownLabel.AutoSize = false;
            this.strategyCountdownLabel.Name = "strategyCountdownLabel";
            this.strategyCountdownLabel.Size = new System.Drawing.Size(200, 17);
            this.strategyCountdownLabel.Text = "Time until strategy check: 60s";
            this.strategyCountdownLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sha256RateLabel
            // 
            this.sha256RateLabel.Name = "sha256RateLabel";
            this.sha256RateLabel.Size = new System.Drawing.Size(488, 17);
            this.sha256RateLabel.Spring = true;
            this.sha256RateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // scryptRateLabel
            // 
            this.scryptRateLabel.AutoSize = false;
            this.scryptRateLabel.Name = "scryptRateLabel";
            this.scryptRateLabel.Size = new System.Drawing.Size(150, 17);
            this.scryptRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // coinStatsCountdownTimer
            // 
            this.coinStatsCountdownTimer.Interval = 60000;
            this.coinStatsCountdownTimer.Tick += new System.EventHandler(this.coinStatsCountdownTimer_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.coinStatsLabel);
            this.panel1.Controls.Add(this.coinChooseSuffixLabel);
            this.panel1.Controls.Add(this.coinChooseLinkLabel);
            this.panel1.Controls.Add(this.coinChoosePrefixLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 375);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1153, 37);
            this.panel1.TabIndex = 9;
            // 
            // coinStatsLabel
            // 
            this.coinStatsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.coinStatsLabel.Location = new System.Drawing.Point(941, 13);
            this.coinStatsLabel.Name = "coinStatsLabel";
            this.coinStatsLabel.Size = new System.Drawing.Size(200, 23);
            this.coinStatsLabel.TabIndex = 3;
            this.coinStatsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // coinChooseSuffixLabel
            // 
            this.coinChooseSuffixLabel.AutoSize = true;
            this.coinChooseSuffixLabel.Location = new System.Drawing.Point(454, 13);
            this.coinChooseSuffixLabel.Name = "coinChooseSuffixLabel";
            this.coinChooseSuffixLabel.Size = new System.Drawing.Size(27, 13);
            this.coinChooseSuffixLabel.TabIndex = 2;
            this.coinChooseSuffixLabel.Text = "API.";
            // 
            // coinChooseLinkLabel
            // 
            this.coinChooseLinkLabel.AutoSize = true;
            this.coinChooseLinkLabel.Location = new System.Drawing.Point(343, 13);
            this.coinChooseLinkLabel.Name = "coinChooseLinkLabel";
            this.coinChooseLinkLabel.Size = new System.Drawing.Size(87, 13);
            this.coinChooseLinkLabel.TabIndex = 1;
            this.coinChooseLinkLabel.TabStop = true;
            this.coinChooseLinkLabel.Text = "CoinChoose.com";
            this.coinChooseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.coinChooseLink_LinkClicked);
            // 
            // coinChoosePrefixLabel
            // 
            this.coinChoosePrefixLabel.AutoSize = true;
            this.coinChoosePrefixLabel.Location = new System.Drawing.Point(3, 13);
            this.coinChoosePrefixLabel.Name = "coinChoosePrefixLabel";
            this.coinChoosePrefixLabel.Size = new System.Drawing.Size(318, 13);
            this.coinChoosePrefixLabel.TabIndex = 0;
            this.coinChoosePrefixLabel.Text = "Crypto-currency pricing and profitability information powered by the";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.deviceGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.apiLogGridView);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(1153, 350);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.TabIndex = 10;
            // 
            // deviceGridView
            // 
            this.deviceGridView.AllowUserToAddRows = false;
            this.deviceGridView.AllowUserToDeleteRows = false;
            this.deviceGridView.AllowUserToResizeRows = false;
            this.deviceGridView.AutoGenerateColumns = false;
            this.deviceGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.deviceGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.deviceGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.kindDataGridViewTextBoxColumn,
            this.nameColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.coinColumn,
            this.difficultyColumn,
            this.priceColumn,
            this.profitabilityColumn,
            this.temperatureColumn,
            this.hashRateColumn,
            this.acceptedColumn,
            this.rejectedColumn,
            this.errorsColumn,
            this.intensityColumn});
            this.deviceGridView.DataSource = this.deviceBindingSource;
            this.deviceGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceGridView.Location = new System.Drawing.Point(0, 0);
            this.deviceGridView.Name = "deviceGridView";
            this.deviceGridView.RowHeadersVisible = false;
            this.deviceGridView.Size = new System.Drawing.Size(1153, 234);
            this.deviceGridView.TabIndex = 1;
            this.deviceGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.deviceGridView_CellValueChanged);
            this.deviceGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.deviceGridView_CurrentCellDirtyStateChanged);
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.Xgminer.Device);
            // 
            // apiLogGridView
            // 
            this.apiLogGridView.AllowUserToAddRows = false;
            this.apiLogGridView.AllowUserToDeleteRows = false;
            this.apiLogGridView.AllowUserToOrderColumns = true;
            this.apiLogGridView.AllowUserToResizeRows = false;
            this.apiLogGridView.AutoGenerateColumns = false;
            this.apiLogGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.apiLogGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dateTimeDataGridViewTextBoxColumn,
            this.CoinName,
            this.requestDataGridViewTextBoxColumn,
            this.responseDataGridViewTextBoxColumn});
            this.apiLogGridView.DataSource = this.apiLogEntryBindingSource;
            this.apiLogGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiLogGridView.Location = new System.Drawing.Point(0, 28);
            this.apiLogGridView.Name = "apiLogGridView";
            this.apiLogGridView.RowHeadersVisible = false;
            this.apiLogGridView.Size = new System.Drawing.Size(1153, 84);
            this.apiLogGridView.TabIndex = 12;
            // 
            // CoinName
            // 
            this.CoinName.DataPropertyName = "CoinName";
            this.CoinName.HeaderText = "Coin";
            this.CoinName.Name = "CoinName";
            this.CoinName.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.closeApiButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1153, 28);
            this.panel2.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "API Monitor";
            // 
            // closeApiButton
            // 
            this.closeApiButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeApiButton.Location = new System.Drawing.Point(1125, 3);
            this.closeApiButton.Name = "closeApiButton";
            this.closeApiButton.Size = new System.Drawing.Size(25, 23);
            this.closeApiButton.TabIndex = 0;
            this.closeApiButton.Text = "X";
            this.closeApiButton.UseVisualStyleBackColor = true;
            this.closeApiButton.Click += new System.EventHandler(this.closeApiButton_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.notifyIconMenuStrip;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "MultiMiner - Stopped";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // notifyIconMenuStrip
            // 
            this.notifyIconMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startMenuItem,
            this.stopMenuItem,
            this.toolStripSeparator4,
            this.showAppMenuItem,
            this.quitAppMenuItem});
            this.notifyIconMenuStrip.Name = "contextMenuStrip1";
            this.notifyIconMenuStrip.Size = new System.Drawing.Size(104, 98);
            // 
            // startMenuItem
            // 
            this.startMenuItem.Image = global::MultiMiner.Win.Properties.Resources.computer_control_play;
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(103, 22);
            this.startMenuItem.Text = "Start";
            this.startMenuItem.Click += new System.EventHandler(this.startMenuItem_Click);
            // 
            // stopMenuItem
            // 
            this.stopMenuItem.Image = global::MultiMiner.Win.Properties.Resources.computer_control_stop;
            this.stopMenuItem.Name = "stopMenuItem";
            this.stopMenuItem.Size = new System.Drawing.Size(103, 22);
            this.stopMenuItem.Text = "Stop";
            this.stopMenuItem.Click += new System.EventHandler(this.stopMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(100, 6);
            // 
            // showAppMenuItem
            // 
            this.showAppMenuItem.Image = global::MultiMiner.Win.Properties.Resources.application;
            this.showAppMenuItem.Name = "showAppMenuItem";
            this.showAppMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showAppMenuItem.Text = "Show";
            this.showAppMenuItem.Click += new System.EventHandler(this.showAppMenuItem_Click);
            // 
            // quitAppMenuItem
            // 
            this.quitAppMenuItem.Image = global::MultiMiner.Win.Properties.Resources.application_delete;
            this.quitAppMenuItem.Name = "quitAppMenuItem";
            this.quitAppMenuItem.Size = new System.Drawing.Size(103, 22);
            this.quitAppMenuItem.Text = "Quit";
            this.quitAppMenuItem.Click += new System.EventHandler(this.quitAppMenuItem_Click);
            // 
            // dateTimeDataGridViewTextBoxColumn
            // 
            this.dateTimeDataGridViewTextBoxColumn.DataPropertyName = "DateTime";
            this.dateTimeDataGridViewTextBoxColumn.HeaderText = "Date/Time";
            this.dateTimeDataGridViewTextBoxColumn.Name = "dateTimeDataGridViewTextBoxColumn";
            this.dateTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.dateTimeDataGridViewTextBoxColumn.Width = 125;
            // 
            // requestDataGridViewTextBoxColumn
            // 
            this.requestDataGridViewTextBoxColumn.DataPropertyName = "Request";
            this.requestDataGridViewTextBoxColumn.HeaderText = "Request";
            this.requestDataGridViewTextBoxColumn.Name = "requestDataGridViewTextBoxColumn";
            this.requestDataGridViewTextBoxColumn.ReadOnly = true;
            this.requestDataGridViewTextBoxColumn.Width = 125;
            // 
            // responseDataGridViewTextBoxColumn
            // 
            this.responseDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.responseDataGridViewTextBoxColumn.DataPropertyName = "Response";
            this.responseDataGridViewTextBoxColumn.HeaderText = "Response";
            this.responseDataGridViewTextBoxColumn.Name = "responseDataGridViewTextBoxColumn";
            this.responseDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // apiLogEntryBindingSource
            // 
            this.apiLogEntryBindingSource.DataSource = typeof(MultiMiner.Win.ApiLogEntry);
            // 
            // kindDataGridViewTextBoxColumn
            // 
            this.kindDataGridViewTextBoxColumn.DataPropertyName = "Identifier";
            this.kindDataGridViewTextBoxColumn.HeaderText = "Identifier";
            this.kindDataGridViewTextBoxColumn.Name = "kindDataGridViewTextBoxColumn";
            this.kindDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameColumn
            // 
            this.nameColumn.DataPropertyName = "Name";
            this.nameColumn.FillWeight = 200F;
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.MinimumWidth = 85;
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Driver";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Driver";
            this.descriptionDataGridViewTextBoxColumn.MinimumWidth = 50;
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // coinColumn
            // 
            this.coinColumn.FillWeight = 150F;
            this.coinColumn.HeaderText = "Coin";
            this.coinColumn.Items.AddRange(new object[] {
            "Configure Coins"});
            this.coinColumn.MinimumWidth = 85;
            this.coinColumn.Name = "coinColumn";
            // 
            // difficultyColumn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.difficultyColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.difficultyColumn.HeaderText = "Difficulty";
            this.difficultyColumn.Name = "difficultyColumn";
            this.difficultyColumn.ReadOnly = true;
            // 
            // priceColumn
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.priceColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.priceColumn.HeaderText = "Price";
            this.priceColumn.Name = "priceColumn";
            this.priceColumn.ReadOnly = true;
            // 
            // profitabilityColumn
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.profitabilityColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.profitabilityColumn.HeaderText = "Profitability";
            this.profitabilityColumn.Name = "profitabilityColumn";
            this.profitabilityColumn.ReadOnly = true;
            // 
            // temperatureColumn
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.temperatureColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.temperatureColumn.HeaderText = "Temperature";
            this.temperatureColumn.Name = "temperatureColumn";
            this.temperatureColumn.ReadOnly = true;
            // 
            // hashRateColumn
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.hashRateColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.hashRateColumn.HeaderText = "Hashrate";
            this.hashRateColumn.Name = "hashRateColumn";
            this.hashRateColumn.ReadOnly = true;
            // 
            // acceptedColumn
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.acceptedColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.acceptedColumn.HeaderText = "Accepted";
            this.acceptedColumn.Name = "acceptedColumn";
            this.acceptedColumn.ReadOnly = true;
            // 
            // rejectedColumn
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.rejectedColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.rejectedColumn.HeaderText = "Rejected";
            this.rejectedColumn.Name = "rejectedColumn";
            this.rejectedColumn.ReadOnly = true;
            // 
            // errorsColumn
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.errorsColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.errorsColumn.HeaderText = "HW Errors";
            this.errorsColumn.Name = "errorsColumn";
            this.errorsColumn.ReadOnly = true;
            // 
            // intensityColumn
            // 
            this.intensityColumn.HeaderText = "Intensity";
            this.intensityColumn.Name = "intensityColumn";
            this.intensityColumn.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 434);
            this.Controls.Add(this.startupMiningPanel);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "MultiMiner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.startupMiningPanel.ResumeLayout(false);
            this.startupMiningPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.apiLogGridView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.notifyIconMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.apiLogEntryBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.Timer deviceStatsTimer;
        private System.Windows.Forms.Timer coinStatsTimer;
        private System.Windows.Forms.Timer startupMiningTimer;
        private System.Windows.Forms.Timer startupMiningCountdownTimer;
        private System.Windows.Forms.Panel startupMiningPanel;
        private System.Windows.Forms.Button cancelStartupMiningButton;
        private System.Windows.Forms.Label countdownLabel;
        private System.Windows.Forms.Timer crashRecoveryTimer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton startButton;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton cancelButton;
        private System.Windows.Forms.ToolStripButton detectDevicesButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel sha256RateLabel;
        private System.Windows.Forms.ToolStripStatusLabel scryptRateLabel;
        private System.Windows.Forms.ToolStripStatusLabel backendLabel;
        private System.Windows.Forms.ToolStripButton strategiesButton;
        private System.Windows.Forms.ToolStripStatusLabel strategiesLabel;
        private System.Windows.Forms.ToolStripStatusLabel strategyCountdownLabel;
        private System.Windows.Forms.Timer coinStatsCountdownTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label coinChooseSuffixLabel;
        private System.Windows.Forms.LinkLabel coinChooseLinkLabel;
        private System.Windows.Forms.Label coinChoosePrefixLabel;
        private System.Windows.Forms.ToolStripButton coinsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label coinStatsLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView deviceGridView;
        private System.Windows.Forms.DataGridView apiLogGridView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button closeApiButton;
        private System.Windows.Forms.ToolStripButton apiMonitorButton;
        private System.Windows.Forms.BindingSource apiLogEntryBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoinName;
        private System.Windows.Forms.DataGridViewTextBoxColumn requestDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn responseDataGridViewTextBoxColumn;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showAppMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitAppMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.ToolStripButton desktopModeButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn kindDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn coinColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn difficultyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn priceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn profitabilityColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperatureColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hashRateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acceptedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rejectedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intensityColumn;
    }
}

