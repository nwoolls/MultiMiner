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
            this.vendorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.platformDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coinColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.difficultyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperatureColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hashRateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rejectedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.settingsButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.deviceStatsTimer = new System.Windows.Forms.Timer(this.components);
            this.coinStatsTimer = new System.Windows.Forms.Timer(this.components);
            this.startupMiningTimer = new System.Windows.Forms.Timer(this.components);
            this.countdownTimer = new System.Windows.Forms.Timer(this.components);
            this.startupMiningPanel = new System.Windows.Forms.Panel();
            this.cancelStartupMiningButton = new System.Windows.Forms.Button();
            this.countdownLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.startupMiningPanel.SuspendLayout();
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
            this.vendorDataGridViewTextBoxColumn,
            this.platformDataGridViewTextBoxColumn,
            this.versionDataGridViewTextBoxColumn,
            this.coinColumn,
            this.difficultyColumn,
            this.priceColumn,
            this.temperatureColumn,
            this.hashRateColumn,
            this.acceptedColumn,
            this.rejectedColumn,
            this.errorsColumn});
            this.deviceGridView.DataSource = this.deviceBindingSource;
            this.deviceGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceGridView.Location = new System.Drawing.Point(0, 0);
            this.deviceGridView.Name = "deviceGridView";
            this.deviceGridView.RowHeadersVisible = false;
            this.deviceGridView.Size = new System.Drawing.Size(1127, 393);
            this.deviceGridView.TabIndex = 0;
            this.deviceGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.deviceGridView_CellValueChanged);
            this.deviceGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.deviceGridView_CurrentCellDirtyStateChanged);
            // 
            // kindDataGridViewTextBoxColumn
            // 
            this.kindDataGridViewTextBoxColumn.DataPropertyName = "Kind";
            this.kindDataGridViewTextBoxColumn.HeaderText = "Kind";
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
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // vendorDataGridViewTextBoxColumn
            // 
            this.vendorDataGridViewTextBoxColumn.DataPropertyName = "Vendor";
            this.vendorDataGridViewTextBoxColumn.HeaderText = "Vendor";
            this.vendorDataGridViewTextBoxColumn.Name = "vendorDataGridViewTextBoxColumn";
            this.vendorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // platformDataGridViewTextBoxColumn
            // 
            this.platformDataGridViewTextBoxColumn.DataPropertyName = "Platform";
            this.platformDataGridViewTextBoxColumn.HeaderText = "Platform";
            this.platformDataGridViewTextBoxColumn.Name = "platformDataGridViewTextBoxColumn";
            this.platformDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // versionDataGridViewTextBoxColumn
            // 
            this.versionDataGridViewTextBoxColumn.DataPropertyName = "Version";
            this.versionDataGridViewTextBoxColumn.HeaderText = "Version";
            this.versionDataGridViewTextBoxColumn.Name = "versionDataGridViewTextBoxColumn";
            this.versionDataGridViewTextBoxColumn.ReadOnly = true;
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
            // panel1
            // 
            this.panel1.Controls.Add(this.settingsButton);
            this.panel1.Controls.Add(this.stopButton);
            this.panel1.Controls.Add(this.startButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 393);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1127, 41);
            this.panel1.TabIndex = 1;
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(176, 9);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(75, 23);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(95, 9);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(13, 9);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(1040, 9);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(959, 9);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // deviceStatsTimer
            // 
            this.deviceStatsTimer.Interval = 10000;
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
            // countdownTimer
            // 
            this.countdownTimer.Interval = 1000;
            this.countdownTimer.Tick += new System.EventHandler(this.countdownTimer_Tick);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 434);
            this.Controls.Add(this.startupMiningPanel);
            this.Controls.Add(this.deviceGridView);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "MultiMiner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.startupMiningPanel.ResumeLayout(false);
            this.startupMiningPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView deviceGridView;
        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Timer deviceStatsTimer;
        private System.Windows.Forms.Timer coinStatsTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn kindDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn platformDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn versionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn coinColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn difficultyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn priceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperatureColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hashRateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acceptedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rejectedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorsColumn;
        private System.Windows.Forms.Timer startupMiningTimer;
        private System.Windows.Forms.Timer countdownTimer;
        private System.Windows.Forms.Panel startupMiningPanel;
        private System.Windows.Forms.Button cancelStartupMiningButton;
        private System.Windows.Forms.Label countdownLabel;
    }
}

