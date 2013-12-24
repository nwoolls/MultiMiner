namespace MultiMiner.Win
{
    partial class DetailsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailsControl));
            this.closeDetailsButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.coinInformationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.hashrateLabel = new System.Windows.Forms.Label();
            this.deviceDetailsResponseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.deviceInformationResponseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cryptoCoinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kindDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.indexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperatureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fanSpeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fanPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpuClockDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.memoryClockDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpuVoltageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpuActivityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.powerTuneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.averageHashrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentHashrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptedSharesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rejectedSharesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareErrorsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.utilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intensityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.poolIndexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rejectedSharesPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareErrorsPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinInformationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceDetailsResponseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInformationResponseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // closeDetailsButton
            // 
            this.closeDetailsButton.AccessibleName = "Close";
            this.closeDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeDetailsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeDetailsButton.Location = new System.Drawing.Point(490, 3);
            this.closeDetailsButton.Name = "closeDetailsButton";
            this.closeDetailsButton.Size = new System.Drawing.Size(22, 22);
            this.closeDetailsButton.TabIndex = 1;
            this.closeDetailsButton.Text = "✖";
            this.closeDetailsButton.UseVisualStyleBackColor = true;
            this.closeDetailsButton.Click += new System.EventHandler(this.closeApiButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "hardware.png");
            this.imageList1.Images.SetKeyName(1, "usb_connector.png");
            this.imageList1.Images.SetKeyName(2, "link_network-list.png");
            this.imageList1.Images.SetKeyName(3, "cpu_front.png");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameLabel.AutoEllipsis = true;
            this.nameLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Name", true));
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(79, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(411, 22);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Name", true));
            this.label2.Location = new System.Drawing.Point(79, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // coinInformationBindingSource
            // 
            this.coinInformationBindingSource.DataSource = typeof(MultiMiner.Coin.Api.CoinInformation);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Symbol", true));
            this.label3.Location = new System.Drawing.Point(79, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Algorithm", true));
            this.label4.Location = new System.Drawing.Point(79, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "label4";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Processors:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "ProcessorCount", true));
            this.label5.Location = new System.Drawing.Point(79, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Driver", true));
            this.label6.Location = new System.Drawing.Point(79, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Driver:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Path", true));
            this.label8.Location = new System.Drawing.Point(79, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Path:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Serial", true));
            this.label10.Location = new System.Drawing.Point(79, 113);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 113);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Serial:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 126);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Hashrate:";
            // 
            // hashrateLabel
            // 
            this.hashrateLabel.AutoSize = true;
            this.hashrateLabel.Location = new System.Drawing.Point(79, 126);
            this.hashrateLabel.Name = "hashrateLabel";
            this.hashrateLabel.Size = new System.Drawing.Size(53, 13);
            this.hashrateLabel.TabIndex = 16;
            this.hashrateLabel.Text = "Hashrate:";
            // 
            // deviceDetailsResponseBindingSource
            // 
            this.deviceDetailsResponseBindingSource.DataSource = typeof(MultiMiner.Xgminer.Api.Responses.DeviceDetailsResponse);
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.Xgminer.Device);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.kindDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.indexDataGridViewTextBoxColumn,
            this.iDDataGridViewTextBoxColumn,
            this.enabledDataGridViewCheckBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.temperatureDataGridViewTextBoxColumn,
            this.fanSpeedDataGridViewTextBoxColumn,
            this.fanPercentDataGridViewTextBoxColumn,
            this.gpuClockDataGridViewTextBoxColumn,
            this.memoryClockDataGridViewTextBoxColumn,
            this.gpuVoltageDataGridViewTextBoxColumn,
            this.gpuActivityDataGridViewTextBoxColumn,
            this.powerTuneDataGridViewTextBoxColumn,
            this.averageHashrateDataGridViewTextBoxColumn,
            this.currentHashrateDataGridViewTextBoxColumn,
            this.acceptedSharesDataGridViewTextBoxColumn,
            this.rejectedSharesDataGridViewTextBoxColumn,
            this.hardwareErrorsDataGridViewTextBoxColumn,
            this.utilityDataGridViewTextBoxColumn,
            this.intensityDataGridViewTextBoxColumn,
            this.poolIndexDataGridViewTextBoxColumn,
            this.rejectedSharesPercentDataGridViewTextBoxColumn,
            this.hardwareErrorsPercentDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.deviceInformationResponseBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(6, 142);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(503, 265);
            this.dataGridView1.TabIndex = 17;
            // 
            // deviceInformationResponseBindingSource
            // 
            this.deviceInformationResponseBindingSource.DataSource = typeof(MultiMiner.Xgminer.Api.Responses.DeviceInformationResponse);
            // 
            // cryptoCoinBindingSource
            // 
            this.cryptoCoinBindingSource.DataSource = typeof(MultiMiner.Engine.CryptoCoin);
            // 
            // kindDataGridViewTextBoxColumn
            // 
            this.kindDataGridViewTextBoxColumn.DataPropertyName = "Kind";
            this.kindDataGridViewTextBoxColumn.HeaderText = "Kind";
            this.kindDataGridViewTextBoxColumn.Name = "kindDataGridViewTextBoxColumn";
            this.kindDataGridViewTextBoxColumn.ReadOnly = true;
            this.kindDataGridViewTextBoxColumn.Visible = false;
            this.kindDataGridViewTextBoxColumn.Width = 5;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Visible = false;
            this.nameDataGridViewTextBoxColumn.Width = 5;
            // 
            // indexDataGridViewTextBoxColumn
            // 
            this.indexDataGridViewTextBoxColumn.DataPropertyName = "Index";
            this.indexDataGridViewTextBoxColumn.HeaderText = "Index";
            this.indexDataGridViewTextBoxColumn.Name = "indexDataGridViewTextBoxColumn";
            this.indexDataGridViewTextBoxColumn.ReadOnly = true;
            this.indexDataGridViewTextBoxColumn.Visible = false;
            this.indexDataGridViewTextBoxColumn.Width = 5;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Visible = false;
            this.iDDataGridViewTextBoxColumn.Width = 5;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            this.enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            this.enabledDataGridViewCheckBoxColumn.ReadOnly = true;
            this.enabledDataGridViewCheckBoxColumn.Visible = false;
            this.enabledDataGridViewCheckBoxColumn.Width = 5;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Visible = false;
            this.statusDataGridViewTextBoxColumn.Width = 5;
            // 
            // temperatureDataGridViewTextBoxColumn
            // 
            this.temperatureDataGridViewTextBoxColumn.DataPropertyName = "Temperature";
            this.temperatureDataGridViewTextBoxColumn.HeaderText = "Temperature";
            this.temperatureDataGridViewTextBoxColumn.Name = "temperatureDataGridViewTextBoxColumn";
            this.temperatureDataGridViewTextBoxColumn.ReadOnly = true;
            this.temperatureDataGridViewTextBoxColumn.Visible = false;
            this.temperatureDataGridViewTextBoxColumn.Width = 5;
            // 
            // fanSpeedDataGridViewTextBoxColumn
            // 
            this.fanSpeedDataGridViewTextBoxColumn.DataPropertyName = "FanSpeed";
            this.fanSpeedDataGridViewTextBoxColumn.HeaderText = "FanSpeed";
            this.fanSpeedDataGridViewTextBoxColumn.Name = "fanSpeedDataGridViewTextBoxColumn";
            this.fanSpeedDataGridViewTextBoxColumn.ReadOnly = true;
            this.fanSpeedDataGridViewTextBoxColumn.Visible = false;
            this.fanSpeedDataGridViewTextBoxColumn.Width = 5;
            // 
            // fanPercentDataGridViewTextBoxColumn
            // 
            this.fanPercentDataGridViewTextBoxColumn.DataPropertyName = "FanPercent";
            this.fanPercentDataGridViewTextBoxColumn.HeaderText = "FanPercent";
            this.fanPercentDataGridViewTextBoxColumn.Name = "fanPercentDataGridViewTextBoxColumn";
            this.fanPercentDataGridViewTextBoxColumn.ReadOnly = true;
            this.fanPercentDataGridViewTextBoxColumn.Visible = false;
            this.fanPercentDataGridViewTextBoxColumn.Width = 5;
            // 
            // gpuClockDataGridViewTextBoxColumn
            // 
            this.gpuClockDataGridViewTextBoxColumn.DataPropertyName = "GpuClock";
            this.gpuClockDataGridViewTextBoxColumn.HeaderText = "GpuClock";
            this.gpuClockDataGridViewTextBoxColumn.Name = "gpuClockDataGridViewTextBoxColumn";
            this.gpuClockDataGridViewTextBoxColumn.ReadOnly = true;
            this.gpuClockDataGridViewTextBoxColumn.Visible = false;
            this.gpuClockDataGridViewTextBoxColumn.Width = 5;
            // 
            // memoryClockDataGridViewTextBoxColumn
            // 
            this.memoryClockDataGridViewTextBoxColumn.DataPropertyName = "MemoryClock";
            this.memoryClockDataGridViewTextBoxColumn.HeaderText = "MemoryClock";
            this.memoryClockDataGridViewTextBoxColumn.Name = "memoryClockDataGridViewTextBoxColumn";
            this.memoryClockDataGridViewTextBoxColumn.ReadOnly = true;
            this.memoryClockDataGridViewTextBoxColumn.Visible = false;
            this.memoryClockDataGridViewTextBoxColumn.Width = 5;
            // 
            // gpuVoltageDataGridViewTextBoxColumn
            // 
            this.gpuVoltageDataGridViewTextBoxColumn.DataPropertyName = "GpuVoltage";
            this.gpuVoltageDataGridViewTextBoxColumn.HeaderText = "GpuVoltage";
            this.gpuVoltageDataGridViewTextBoxColumn.Name = "gpuVoltageDataGridViewTextBoxColumn";
            this.gpuVoltageDataGridViewTextBoxColumn.ReadOnly = true;
            this.gpuVoltageDataGridViewTextBoxColumn.Visible = false;
            this.gpuVoltageDataGridViewTextBoxColumn.Width = 5;
            // 
            // gpuActivityDataGridViewTextBoxColumn
            // 
            this.gpuActivityDataGridViewTextBoxColumn.DataPropertyName = "GpuActivity";
            this.gpuActivityDataGridViewTextBoxColumn.HeaderText = "GpuActivity";
            this.gpuActivityDataGridViewTextBoxColumn.Name = "gpuActivityDataGridViewTextBoxColumn";
            this.gpuActivityDataGridViewTextBoxColumn.ReadOnly = true;
            this.gpuActivityDataGridViewTextBoxColumn.Visible = false;
            this.gpuActivityDataGridViewTextBoxColumn.Width = 5;
            // 
            // powerTuneDataGridViewTextBoxColumn
            // 
            this.powerTuneDataGridViewTextBoxColumn.DataPropertyName = "PowerTune";
            this.powerTuneDataGridViewTextBoxColumn.HeaderText = "PowerTune";
            this.powerTuneDataGridViewTextBoxColumn.Name = "powerTuneDataGridViewTextBoxColumn";
            this.powerTuneDataGridViewTextBoxColumn.ReadOnly = true;
            this.powerTuneDataGridViewTextBoxColumn.Visible = false;
            this.powerTuneDataGridViewTextBoxColumn.Width = 5;
            // 
            // averageHashrateDataGridViewTextBoxColumn
            // 
            this.averageHashrateDataGridViewTextBoxColumn.DataPropertyName = "AverageHashrate";
            this.averageHashrateDataGridViewTextBoxColumn.HeaderText = "Average";
            this.averageHashrateDataGridViewTextBoxColumn.Name = "averageHashrateDataGridViewTextBoxColumn";
            this.averageHashrateDataGridViewTextBoxColumn.ReadOnly = true;
            this.averageHashrateDataGridViewTextBoxColumn.Width = 5;
            // 
            // currentHashrateDataGridViewTextBoxColumn
            // 
            this.currentHashrateDataGridViewTextBoxColumn.DataPropertyName = "CurrentHashrate";
            this.currentHashrateDataGridViewTextBoxColumn.HeaderText = "Current";
            this.currentHashrateDataGridViewTextBoxColumn.Name = "currentHashrateDataGridViewTextBoxColumn";
            this.currentHashrateDataGridViewTextBoxColumn.ReadOnly = true;
            this.currentHashrateDataGridViewTextBoxColumn.Width = 5;
            // 
            // acceptedSharesDataGridViewTextBoxColumn
            // 
            this.acceptedSharesDataGridViewTextBoxColumn.DataPropertyName = "AcceptedShares";
            this.acceptedSharesDataGridViewTextBoxColumn.HeaderText = "Acc.";
            this.acceptedSharesDataGridViewTextBoxColumn.Name = "acceptedSharesDataGridViewTextBoxColumn";
            this.acceptedSharesDataGridViewTextBoxColumn.ReadOnly = true;
            this.acceptedSharesDataGridViewTextBoxColumn.Width = 5;
            // 
            // rejectedSharesDataGridViewTextBoxColumn
            // 
            this.rejectedSharesDataGridViewTextBoxColumn.DataPropertyName = "RejectedShares";
            this.rejectedSharesDataGridViewTextBoxColumn.HeaderText = "RejectedShares";
            this.rejectedSharesDataGridViewTextBoxColumn.Name = "rejectedSharesDataGridViewTextBoxColumn";
            this.rejectedSharesDataGridViewTextBoxColumn.ReadOnly = true;
            this.rejectedSharesDataGridViewTextBoxColumn.Visible = false;
            this.rejectedSharesDataGridViewTextBoxColumn.Width = 5;
            // 
            // hardwareErrorsDataGridViewTextBoxColumn
            // 
            this.hardwareErrorsDataGridViewTextBoxColumn.DataPropertyName = "HardwareErrors";
            this.hardwareErrorsDataGridViewTextBoxColumn.HeaderText = "HardwareErrors";
            this.hardwareErrorsDataGridViewTextBoxColumn.Name = "hardwareErrorsDataGridViewTextBoxColumn";
            this.hardwareErrorsDataGridViewTextBoxColumn.ReadOnly = true;
            this.hardwareErrorsDataGridViewTextBoxColumn.Visible = false;
            this.hardwareErrorsDataGridViewTextBoxColumn.Width = 5;
            // 
            // utilityDataGridViewTextBoxColumn
            // 
            this.utilityDataGridViewTextBoxColumn.DataPropertyName = "Utility";
            this.utilityDataGridViewTextBoxColumn.HeaderText = "Utility";
            this.utilityDataGridViewTextBoxColumn.Name = "utilityDataGridViewTextBoxColumn";
            this.utilityDataGridViewTextBoxColumn.ReadOnly = true;
            this.utilityDataGridViewTextBoxColumn.Width = 5;
            // 
            // intensityDataGridViewTextBoxColumn
            // 
            this.intensityDataGridViewTextBoxColumn.DataPropertyName = "Intensity";
            this.intensityDataGridViewTextBoxColumn.HeaderText = "Intensity";
            this.intensityDataGridViewTextBoxColumn.Name = "intensityDataGridViewTextBoxColumn";
            this.intensityDataGridViewTextBoxColumn.ReadOnly = true;
            this.intensityDataGridViewTextBoxColumn.Visible = false;
            this.intensityDataGridViewTextBoxColumn.Width = 5;
            // 
            // poolIndexDataGridViewTextBoxColumn
            // 
            this.poolIndexDataGridViewTextBoxColumn.DataPropertyName = "PoolIndex";
            this.poolIndexDataGridViewTextBoxColumn.HeaderText = "PoolIndex";
            this.poolIndexDataGridViewTextBoxColumn.Name = "poolIndexDataGridViewTextBoxColumn";
            this.poolIndexDataGridViewTextBoxColumn.ReadOnly = true;
            this.poolIndexDataGridViewTextBoxColumn.Visible = false;
            this.poolIndexDataGridViewTextBoxColumn.Width = 5;
            // 
            // rejectedSharesPercentDataGridViewTextBoxColumn
            // 
            this.rejectedSharesPercentDataGridViewTextBoxColumn.DataPropertyName = "RejectedSharesPercent";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.HeaderText = "Rej. %";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.Name = "rejectedSharesPercentDataGridViewTextBoxColumn";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.ReadOnly = true;
            this.rejectedSharesPercentDataGridViewTextBoxColumn.Width = 5;
            // 
            // hardwareErrorsPercentDataGridViewTextBoxColumn
            // 
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.DataPropertyName = "HardwareErrorsPercent";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.HeaderText = "HW %";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.Name = "hardwareErrorsPercentDataGridViewTextBoxColumn";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.ReadOnly = true;
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.Width = 5;
            // 
            // DetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.hashrateLabel);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.closeDetailsButton);
            this.Name = "DetailsControl";
            this.Size = new System.Drawing.Size(515, 410);
            this.Load += new System.EventHandler(this.DetailsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinInformationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceDetailsResponseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInformationResponseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeDetailsButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingSource coinInformationBindingSource;
        private System.Windows.Forms.BindingSource cryptoCoinBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label hashrateLabel;
        private System.Windows.Forms.BindingSource deviceDetailsResponseBindingSource;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource deviceInformationResponseBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn kindDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperatureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fanSpeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fanPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gpuClockDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn memoryClockDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gpuVoltageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gpuActivityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn powerTuneDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn averageHashrateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currentHashrateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acceptedSharesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rejectedSharesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareErrorsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn utilityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intensityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn poolIndexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rejectedSharesPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareErrorsPercentDataGridViewTextBoxColumn;
    }
}
