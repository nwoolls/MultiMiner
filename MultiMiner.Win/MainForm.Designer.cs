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
            this.deviceGridView = new System.Windows.Forms.DataGridView();
            this.kindDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.strategiesButton = new System.Windows.Forms.ToolStripButton();
            this.detectDevicesButton = new System.Windows.Forms.ToolStripButton();
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
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            this.startupMiningPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.coinColumn,
            this.difficultyColumn,
            this.priceColumn,
            this.profitabilityColumn,
            this.temperatureColumn,
            this.hashRateColumn,
            this.acceptedColumn,
            this.rejectedColumn,
            this.errorsColumn});
            this.deviceGridView.DataSource = this.deviceBindingSource;
            this.deviceGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceGridView.Location = new System.Drawing.Point(0, 25);
            this.deviceGridView.Name = "deviceGridView";
            this.deviceGridView.RowHeadersVisible = false;
            this.deviceGridView.Size = new System.Drawing.Size(1127, 409);
            this.deviceGridView.TabIndex = 0;
            this.deviceGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.deviceGridView_CellValueChanged);
            this.deviceGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.deviceGridView_CurrentCellDirtyStateChanged);
            // 
            // kindDataGridViewTextBoxColumn
            // 
            this.kindDataGridViewTextBoxColumn.DataPropertyName = "Identifier";
            this.kindDataGridViewTextBoxColumn.HeaderText = "Identifier";
            this.kindDataGridViewTextBoxColumn.Name = "kindDataGridViewTextBoxColumn";
            this.kindDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Driver";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Driver";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // coinColumn
            // 
            this.coinColumn.HeaderText = "Coin";
            this.coinColumn.Items.AddRange(new object[] {
            "Configure Coins"});
            this.coinColumn.Name = "coinColumn";
            // 
            // difficultyColumn
            // 
            this.difficultyColumn.HeaderText = "Difficulty";
            this.difficultyColumn.Name = "difficultyColumn";
            this.difficultyColumn.ReadOnly = true;
            // 
            // priceColumn
            // 
            this.priceColumn.HeaderText = "Price";
            this.priceColumn.Name = "priceColumn";
            this.priceColumn.ReadOnly = true;
            // 
            // profitabilityColumn
            // 
            this.profitabilityColumn.HeaderText = "Profitability";
            this.profitabilityColumn.Name = "profitabilityColumn";
            this.profitabilityColumn.ReadOnly = true;
            // 
            // temperatureColumn
            // 
            this.temperatureColumn.HeaderText = "Temperature";
            this.temperatureColumn.Name = "temperatureColumn";
            this.temperatureColumn.ReadOnly = true;
            // 
            // hashRateColumn
            // 
            this.hashRateColumn.HeaderText = "Hashrate";
            this.hashRateColumn.Name = "hashRateColumn";
            this.hashRateColumn.ReadOnly = true;
            // 
            // acceptedColumn
            // 
            this.acceptedColumn.HeaderText = "Accepted";
            this.acceptedColumn.Name = "acceptedColumn";
            this.acceptedColumn.ReadOnly = true;
            // 
            // rejectedColumn
            // 
            this.rejectedColumn.HeaderText = "Rejected";
            this.rejectedColumn.Name = "rejectedColumn";
            this.rejectedColumn.ReadOnly = true;
            // 
            // errorsColumn
            // 
            this.errorsColumn.HeaderText = "HW Errors";
            this.errorsColumn.Name = "errorsColumn";
            this.errorsColumn.ReadOnly = true;
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.Xgminer.Device);
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
            this.startupMiningPanel.Location = new System.Drawing.Point(404, 199);
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
            this.settingsButton,
            this.strategiesButton,
            this.detectDevicesButton,
            this.toolStripSeparator1,
            this.saveButton,
            this.cancelButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1127, 25);
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
            // settingsButton
            // 
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(69, 22);
            this.settingsButton.Text = "Settings";
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
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
            // detectDevicesButton
            // 
            this.detectDevicesButton.Image = ((System.Drawing.Image)(resources.GetObject("detectDevicesButton.Image")));
            this.detectDevicesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.detectDevicesButton.Name = "detectDevicesButton";
            this.detectDevicesButton.Size = new System.Drawing.Size(104, 22);
            this.detectDevicesButton.Text = "Detect Devices";
            this.detectDevicesButton.Click += new System.EventHandler(this.detectDevicesButton_Click);
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
            this.statusStrip1.Size = new System.Drawing.Size(1127, 22);
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
            this.sha256RateLabel.Size = new System.Drawing.Size(462, 17);
            this.sha256RateLabel.Spring = true;
            this.sha256RateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // scryptRateLabel
            // 
            this.scryptRateLabel.AutoSize = false;
            this.scryptRateLabel.Name = "scryptRateLabel";
            this.scryptRateLabel.Size = new System.Drawing.Size(150, 17);
            // 
            // coinStatsCountdownTimer
            // 
            this.coinStatsCountdownTimer.Interval = 60000;
            this.coinStatsCountdownTimer.Tick += new System.EventHandler(this.coinStatsCountdownTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 434);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.startupMiningPanel);
            this.Controls.Add(this.deviceGridView);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "MultiMiner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            this.startupMiningPanel.ResumeLayout(false);
            this.startupMiningPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView deviceGridView;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn kindDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
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
        private System.Windows.Forms.ToolStripStatusLabel strategiesLabel;
        private System.Windows.Forms.ToolStripStatusLabel strategyCountdownLabel;
        private System.Windows.Forms.Timer coinStatsCountdownTimer;
    }
}

