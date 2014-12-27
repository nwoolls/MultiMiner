namespace MultiMiner.Win.Controls
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.closeDetailsButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.nameLabel = new System.Windows.Forms.Label();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cryptoCoinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.symbolLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.processorsTitleLabel = new System.Windows.Forms.Label();
            this.processorsValueLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pathValueLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.serialValueLabel = new System.Windows.Forms.Label();
            this.serialTitleLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.hashrateLabel = new System.Windows.Forms.Label();
            this.workersGridView = new System.Windows.Forms.DataGridView();
            this.workerNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentHashrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperatureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareErrorsPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.utilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.workersTitleLabel = new System.Windows.Forms.Label();
            this.tempLabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.fanLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.acceptedLabel = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.rejectedLabel = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.errorsLabel = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.utilityLabel = new System.Windows.Forms.Label();
            this.utilityPrefixLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lastShareLabel = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.poolLabel = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.bestShareLabel = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.noDetailsPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.deviceCountLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.currentRateLabel = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.proxyInfoPanel = new System.Windows.Forms.Panel();
            this.proxyStratumLabel = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.proxyGetworkLabel = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workersGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workerBindingSource)).BeginInit();
            this.noDetailsPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.proxyInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeDetailsButton
            // 
            this.closeDetailsButton.AccessibleName = "Close";
            this.closeDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeDetailsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeDetailsButton.Location = new System.Drawing.Point(572, 3);
            this.closeDetailsButton.Name = "closeDetailsButton";
            this.closeDetailsButton.Size = new System.Drawing.Size(26, 25);
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
            this.imageList1.Images.SetKeyName(4, "hardware-internet.png");
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameLabel.AutoEllipsis = true;
            this.nameLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Name", true));
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.nameLabel.Location = new System.Drawing.Point(10, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(561, 25);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "label1";
            this.nameLabel.Click += new System.EventHandler(this.nameLabel_Click);
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.UX.ViewModels.DeviceViewModel);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Name", true));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.label2.Location = new System.Drawing.Point(92, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // cryptoCoinBindingSource
            // 
            this.cryptoCoinBindingSource.DataSource = typeof(MultiMiner.Engine.Data.PoolGroup);
            // 
            // symbolLabel
            // 
            this.symbolLabel.AutoSize = true;
            this.symbolLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Id", true));
            this.symbolLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.symbolLabel.Location = new System.Drawing.Point(92, 45);
            this.symbolLabel.Name = "symbolLabel";
            this.symbolLabel.Size = new System.Drawing.Size(38, 15);
            this.symbolLabel.TabIndex = 5;
            this.symbolLabel.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Algorithm", true));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.label4.Location = new System.Drawing.Point(92, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "label4";
            // 
            // processorsTitleLabel
            // 
            this.processorsTitleLabel.AutoSize = true;
            this.processorsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.processorsTitleLabel.Location = new System.Drawing.Point(183, 110);
            this.processorsTitleLabel.Name = "processorsTitleLabel";
            this.processorsTitleLabel.Size = new System.Drawing.Size(66, 15);
            this.processorsTitleLabel.TabIndex = 7;
            this.processorsTitleLabel.Text = "Processors:";
            // 
            // processorsValueLabel
            // 
            this.processorsValueLabel.AutoSize = true;
            this.processorsValueLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "ProcessorCount", true));
            this.processorsValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.processorsValueLabel.Location = new System.Drawing.Point(255, 110);
            this.processorsValueLabel.Name = "processorsValueLabel";
            this.processorsValueLabel.Size = new System.Drawing.Size(38, 15);
            this.processorsValueLabel.TabIndex = 8;
            this.processorsValueLabel.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoEllipsis = true;
            this.label6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Driver", true));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.label6.Location = new System.Drawing.Point(92, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label7.Location = new System.Drawing.Point(7, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "Driver:";
            // 
            // pathValueLabel
            // 
            this.pathValueLabel.AutoEllipsis = true;
            this.pathValueLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Path", true));
            this.pathValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.pathValueLabel.Location = new System.Drawing.Point(92, 125);
            this.pathValueLabel.Name = "pathValueLabel";
            this.pathValueLabel.Size = new System.Drawing.Size(85, 15);
            this.pathValueLabel.TabIndex = 12;
            this.pathValueLabel.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label9.Location = new System.Drawing.Point(7, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 15);
            this.label9.TabIndex = 11;
            this.label9.Text = "Path:";
            // 
            // serialValueLabel
            // 
            this.serialValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serialValueLabel.AutoEllipsis = true;
            this.serialValueLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Serial", true));
            this.serialValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.serialValueLabel.Location = new System.Drawing.Point(255, 125);
            this.serialValueLabel.Name = "serialValueLabel";
            this.serialValueLabel.Size = new System.Drawing.Size(339, 15);
            this.serialValueLabel.TabIndex = 14;
            this.serialValueLabel.Text = "label10";
            // 
            // serialTitleLabel
            // 
            this.serialTitleLabel.AutoSize = true;
            this.serialTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.serialTitleLabel.Location = new System.Drawing.Point(183, 125);
            this.serialTitleLabel.Name = "serialTitleLabel";
            this.serialTitleLabel.Size = new System.Drawing.Size(38, 15);
            this.serialTitleLabel.TabIndex = 13;
            this.serialTitleLabel.Text = "Serial:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label12.Location = new System.Drawing.Point(7, 140);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 15);
            this.label12.TabIndex = 15;
            this.label12.Text = "Average:";
            // 
            // hashrateLabel
            // 
            this.hashrateLabel.AutoSize = true;
            this.hashrateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.hashrateLabel.Location = new System.Drawing.Point(92, 140);
            this.hashrateLabel.Name = "hashrateLabel";
            this.hashrateLabel.Size = new System.Drawing.Size(44, 15);
            this.hashrateLabel.TabIndex = 16;
            this.hashrateLabel.Text = "label12";
            // 
            // workersGridView
            // 
            this.workersGridView.AllowUserToAddRows = false;
            this.workersGridView.AllowUserToDeleteRows = false;
            this.workersGridView.AllowUserToResizeRows = false;
            this.workersGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workersGridView.AutoGenerateColumns = false;
            this.workersGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.workersGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.workersGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.workersGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.workersGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.workerNameDataGridViewTextBoxColumn,
            this.currentHashrateDataGridViewTextBoxColumn,
            this.temperatureDataGridViewTextBoxColumn,
            this.hardwareErrorsPercentDataGridViewTextBoxColumn,
            this.utilityDataGridViewTextBoxColumn});
            this.workersGridView.DataSource = this.workerBindingSource;
            this.workersGridView.GridColor = System.Drawing.SystemColors.ControlLightLight;
            this.workersGridView.Location = new System.Drawing.Point(7, 287);
            this.workersGridView.Name = "workersGridView";
            this.workersGridView.ReadOnly = true;
            this.workersGridView.RowHeadersVisible = false;
            this.workersGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.workersGridView.Size = new System.Drawing.Size(587, 226);
            this.workersGridView.TabIndex = 17;
            this.workersGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.workersGridView_CellFormatting);
            this.workersGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.workersGridView_DataError);
            // 
            // workerNameDataGridViewTextBoxColumn
            // 
            this.workerNameDataGridViewTextBoxColumn.DataPropertyName = "WorkerName";
            this.workerNameDataGridViewTextBoxColumn.FillWeight = 75F;
            this.workerNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.workerNameDataGridViewTextBoxColumn.Name = "workerNameDataGridViewTextBoxColumn";
            this.workerNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currentHashrateDataGridViewTextBoxColumn
            // 
            this.currentHashrateDataGridViewTextBoxColumn.DataPropertyName = "CurrentHashrate";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.currentHashrateDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.currentHashrateDataGridViewTextBoxColumn.HeaderText = "Current";
            this.currentHashrateDataGridViewTextBoxColumn.Name = "currentHashrateDataGridViewTextBoxColumn";
            this.currentHashrateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // temperatureDataGridViewTextBoxColumn
            // 
            this.temperatureDataGridViewTextBoxColumn.DataPropertyName = "Temperature";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.temperatureDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.temperatureDataGridViewTextBoxColumn.FillWeight = 60F;
            this.temperatureDataGridViewTextBoxColumn.HeaderText = "Temp";
            this.temperatureDataGridViewTextBoxColumn.Name = "temperatureDataGridViewTextBoxColumn";
            this.temperatureDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // hardwareErrorsPercentDataGridViewTextBoxColumn
            // 
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.DataPropertyName = "HardwareErrorsPercent";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.FillWeight = 75F;
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.HeaderText = "Errors";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.Name = "hardwareErrorsPercentDataGridViewTextBoxColumn";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // utilityDataGridViewTextBoxColumn
            // 
            this.utilityDataGridViewTextBoxColumn.DataPropertyName = "Utility";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "#,#.###";
            this.utilityDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.utilityDataGridViewTextBoxColumn.FillWeight = 75F;
            this.utilityDataGridViewTextBoxColumn.HeaderText = "Utility";
            this.utilityDataGridViewTextBoxColumn.Name = "utilityDataGridViewTextBoxColumn";
            this.utilityDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // workerBindingSource
            // 
            this.workerBindingSource.DataSource = typeof(MultiMiner.UX.ViewModels.DeviceViewModel);
            // 
            // workersTitleLabel
            // 
            this.workersTitleLabel.AutoSize = true;
            this.workersTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.workersTitleLabel.Location = new System.Drawing.Point(7, 269);
            this.workersTitleLabel.Name = "workersTitleLabel";
            this.workersTitleLabel.Size = new System.Drawing.Size(53, 15);
            this.workersTitleLabel.TabIndex = 18;
            this.workersTitleLabel.Text = "Workers:";
            // 
            // tempLabel
            // 
            this.tempLabel.AutoEllipsis = true;
            this.tempLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.tempLabel.Location = new System.Drawing.Point(92, 156);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(85, 13);
            this.tempLabel.TabIndex = 20;
            this.tempLabel.Text = "label14";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label15.Location = new System.Drawing.Point(7, 155);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 15);
            this.label15.TabIndex = 19;
            this.label15.Text = "Temp:";
            // 
            // fanLabel
            // 
            this.fanLabel.AutoSize = true;
            this.fanLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.fanLabel.Location = new System.Drawing.Point(255, 155);
            this.fanLabel.Name = "fanLabel";
            this.fanLabel.Size = new System.Drawing.Size(44, 15);
            this.fanLabel.TabIndex = 22;
            this.fanLabel.Text = "label16";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label17.Location = new System.Drawing.Point(183, 155);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 15);
            this.label17.TabIndex = 21;
            this.label17.Text = "Fan:";
            // 
            // acceptedLabel
            // 
            this.acceptedLabel.AutoEllipsis = true;
            this.acceptedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.acceptedLabel.Location = new System.Drawing.Point(92, 170);
            this.acceptedLabel.Name = "acceptedLabel";
            this.acceptedLabel.Size = new System.Drawing.Size(85, 15);
            this.acceptedLabel.TabIndex = 24;
            this.acceptedLabel.Text = "label18";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label19.Location = new System.Drawing.Point(7, 170);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(60, 15);
            this.label19.TabIndex = 23;
            this.label19.Text = "Accepted:";
            // 
            // rejectedLabel
            // 
            this.rejectedLabel.AutoSize = true;
            this.rejectedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.rejectedLabel.Location = new System.Drawing.Point(255, 170);
            this.rejectedLabel.Name = "rejectedLabel";
            this.rejectedLabel.Size = new System.Drawing.Size(44, 15);
            this.rejectedLabel.TabIndex = 26;
            this.rejectedLabel.Text = "label20";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label21.Location = new System.Drawing.Point(183, 170);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(55, 15);
            this.label21.TabIndex = 25;
            this.label21.Text = "Rejected:";
            // 
            // errorsLabel
            // 
            this.errorsLabel.AutoSize = true;
            this.errorsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.errorsLabel.Location = new System.Drawing.Point(255, 185);
            this.errorsLabel.Name = "errorsLabel";
            this.errorsLabel.Size = new System.Drawing.Size(44, 15);
            this.errorsLabel.TabIndex = 28;
            this.errorsLabel.Text = "label22";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label23.Location = new System.Drawing.Point(183, 185);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(40, 15);
            this.label23.TabIndex = 27;
            this.label23.Text = "Errors:";
            // 
            // utilityLabel
            // 
            this.utilityLabel.AutoEllipsis = true;
            this.utilityLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.utilityLabel.Location = new System.Drawing.Point(92, 185);
            this.utilityLabel.Name = "utilityLabel";
            this.utilityLabel.Size = new System.Drawing.Size(85, 15);
            this.utilityLabel.TabIndex = 30;
            this.utilityLabel.Text = "label24";
            // 
            // utilityPrefixLabel
            // 
            this.utilityPrefixLabel.AutoSize = true;
            this.utilityPrefixLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.utilityPrefixLabel.Location = new System.Drawing.Point(7, 185);
            this.utilityPrefixLabel.Name = "utilityPrefixLabel";
            this.utilityPrefixLabel.Size = new System.Drawing.Size(41, 15);
            this.utilityPrefixLabel.TabIndex = 29;
            this.utilityPrefixLabel.Text = "Utility:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "LastShareDifficulty", true));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.label13.Location = new System.Drawing.Point(255, 215);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 15);
            this.label13.TabIndex = 36;
            this.label13.Text = "label18";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label14.Location = new System.Drawing.Point(183, 215);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 15);
            this.label14.TabIndex = 35;
            this.label14.Text = "Difficulty:";
            // 
            // lastShareLabel
            // 
            this.lastShareLabel.AutoEllipsis = true;
            this.lastShareLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "LastShareTime", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "t"));
            this.lastShareLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.lastShareLabel.Location = new System.Drawing.Point(92, 215);
            this.lastShareLabel.Name = "lastShareLabel";
            this.lastShareLabel.Size = new System.Drawing.Size(85, 15);
            this.lastShareLabel.TabIndex = 34;
            this.lastShareLabel.Text = "label16";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label18.Location = new System.Drawing.Point(7, 215);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(62, 15);
            this.label18.TabIndex = 33;
            this.label18.Text = "Last share:";
            // 
            // poolLabel
            // 
            this.poolLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.poolLabel.AutoEllipsis = true;
            this.poolLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Url", true));
            this.poolLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.poolLabel.Location = new System.Drawing.Point(92, 200);
            this.poolLabel.Name = "poolLabel";
            this.poolLabel.Size = new System.Drawing.Size(502, 15);
            this.poolLabel.TabIndex = 32;
            this.poolLabel.Text = "label14";
            this.poolLabel.UseMnemonic = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label22.Location = new System.Drawing.Point(7, 200);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(34, 15);
            this.label22.TabIndex = 31;
            this.label22.Text = "Pool:";
            // 
            // bestShareLabel
            // 
            this.bestShareLabel.AutoSize = true;
            this.bestShareLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "BestShare", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "#,#.###############"));
            this.bestShareLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.bestShareLabel.Location = new System.Drawing.Point(92, 230);
            this.bestShareLabel.Name = "bestShareLabel";
            this.bestShareLabel.Size = new System.Drawing.Size(44, 15);
            this.bestShareLabel.TabIndex = 38;
            this.bestShareLabel.Text = "label18";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label26.Location = new System.Drawing.Point(7, 230);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(63, 15);
            this.label26.TabIndex = 37;
            this.label26.Text = "Best share:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "PoolStalePercent", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "P1"));
            this.label27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.label27.Location = new System.Drawing.Point(92, 245);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(44, 15);
            this.label27.TabIndex = 40;
            this.label27.Text = "label18";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label28.Location = new System.Drawing.Point(7, 245);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(35, 15);
            this.label28.TabIndex = 39;
            this.label28.Text = "Stale:";
            // 
            // noDetailsPanel
            // 
            this.noDetailsPanel.Controls.Add(this.panel2);
            this.noDetailsPanel.Controls.Add(this.deviceCountLabel);
            this.noDetailsPanel.Location = new System.Drawing.Point(112, 316);
            this.noDetailsPanel.Name = "noDetailsPanel";
            this.noDetailsPanel.Size = new System.Drawing.Size(338, 254);
            this.noDetailsPanel.TabIndex = 41;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Location = new System.Drawing.Point(11, 29);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(1);
            this.panel2.Size = new System.Drawing.Size(75, 75);
            this.panel2.TabIndex = 44;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = global::MultiMiner.Win.Properties.Resources.computer_list;
            this.pictureBox2.Location = new System.Drawing.Point(1, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(73, 73);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // deviceCountLabel
            // 
            this.deviceCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deviceCountLabel.AutoEllipsis = true;
            this.deviceCountLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.deviceBindingSource, "Name", true));
            this.deviceCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.deviceCountLabel.Location = new System.Drawing.Point(10, 3);
            this.deviceCountLabel.Name = "deviceCountLabel";
            this.deviceCountLabel.Size = new System.Drawing.Size(316, 25);
            this.deviceCountLabel.TabIndex = 43;
            this.deviceCountLabel.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(11, 29);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(75, 75);
            this.panel1.TabIndex = 42;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(73, 73);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // currentRateLabel
            // 
            this.currentRateLabel.AutoSize = true;
            this.currentRateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.currentRateLabel.Location = new System.Drawing.Point(255, 140);
            this.currentRateLabel.Name = "currentRateLabel";
            this.currentRateLabel.Size = new System.Drawing.Size(44, 15);
            this.currentRateLabel.TabIndex = 44;
            this.currentRateLabel.Text = "label12";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label29.Location = new System.Drawing.Point(183, 140);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(50, 15);
            this.label29.TabIndex = 43;
            this.label29.Text = "Current:";
            // 
            // proxyInfoPanel
            // 
            this.proxyInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyInfoPanel.Controls.Add(this.proxyStratumLabel);
            this.proxyInfoPanel.Controls.Add(this.label25);
            this.proxyInfoPanel.Controls.Add(this.proxyGetworkLabel);
            this.proxyInfoPanel.Controls.Add(this.label20);
            this.proxyInfoPanel.Location = new System.Drawing.Point(4, 109);
            this.proxyInfoPanel.Name = "proxyInfoPanel";
            this.proxyInfoPanel.Size = new System.Drawing.Size(590, 31);
            this.proxyInfoPanel.TabIndex = 45;
            this.proxyInfoPanel.Visible = false;
            // 
            // proxyStratumLabel
            // 
            this.proxyStratumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyStratumLabel.AutoEllipsis = true;
            this.proxyStratumLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.proxyStratumLabel.Location = new System.Drawing.Point(88, 16);
            this.proxyStratumLabel.Name = "proxyStratumLabel";
            this.proxyStratumLabel.Size = new System.Drawing.Size(498, 15);
            this.proxyStratumLabel.TabIndex = 36;
            this.proxyStratumLabel.Text = "label14";
            this.proxyStratumLabel.UseMnemonic = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label25.Location = new System.Drawing.Point(3, 16);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 15);
            this.label25.TabIndex = 35;
            this.label25.Text = "Stratum:";
            // 
            // proxyGetworkLabel
            // 
            this.proxyGetworkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyGetworkLabel.AutoEllipsis = true;
            this.proxyGetworkLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.proxyGetworkLabel.Location = new System.Drawing.Point(88, 1);
            this.proxyGetworkLabel.Name = "proxyGetworkLabel";
            this.proxyGetworkLabel.Size = new System.Drawing.Size(498, 15);
            this.proxyGetworkLabel.TabIndex = 34;
            this.proxyGetworkLabel.Text = "label14";
            this.proxyGetworkLabel.UseMnemonic = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(103)))), ((int)(((byte)(121)))));
            this.label20.Location = new System.Drawing.Point(3, 1);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(54, 15);
            this.label20.TabIndex = 33;
            this.label20.Text = "Getwork:";
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.statusLabel.Location = new System.Drawing.Point(92, 75);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(502, 31);
            this.statusLabel.TabIndex = 46;
            this.statusLabel.Text = "label1";
            // 
            // DetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.currentRateLabel);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.noDetailsPanel);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.bestShareLabel);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lastShareLabel);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.poolLabel);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.utilityLabel);
            this.Controls.Add(this.utilityPrefixLabel);
            this.Controls.Add(this.errorsLabel);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.rejectedLabel);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.acceptedLabel);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.fanLabel);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.tempLabel);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.workersTitleLabel);
            this.Controls.Add(this.workersGridView);
            this.Controls.Add(this.hashrateLabel);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.serialValueLabel);
            this.Controls.Add(this.serialTitleLabel);
            this.Controls.Add(this.pathValueLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.processorsValueLabel);
            this.Controls.Add(this.processorsTitleLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.symbolLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.closeDetailsButton);
            this.Controls.Add(this.proxyInfoPanel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "DetailsControl";
            this.Size = new System.Drawing.Size(601, 517);
            this.Load += new System.EventHandler(this.DetailsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workersGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workerBindingSource)).EndInit();
            this.noDetailsPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.proxyInfoPanel.ResumeLayout(false);
            this.proxyInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeDetailsButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label symbolLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label processorsTitleLabel;
        private System.Windows.Forms.Label processorsValueLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label pathValueLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label serialValueLabel;
        private System.Windows.Forms.Label serialTitleLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label hashrateLabel;
        private System.Windows.Forms.DataGridView workersGridView;
        private System.Windows.Forms.Label workersTitleLabel;
        private System.Windows.Forms.Label tempLabel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label fanLabel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label acceptedLabel;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label rejectedLabel;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label errorsLabel;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label utilityLabel;
        private System.Windows.Forms.Label utilityPrefixLabel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lastShareLabel;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label poolLabel;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label bestShareLabel;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Panel noDetailsPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label deviceCountLabel;
        private System.Windows.Forms.Label currentRateLabel;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.BindingSource cryptoCoinBindingSource;
        private System.Windows.Forms.BindingSource workerBindingSource;
        private System.Windows.Forms.Panel proxyInfoPanel;
        private System.Windows.Forms.Label proxyStratumLabel;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label proxyGetworkLabel;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn workerNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currentHashrateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperatureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareErrorsPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn utilityDataGridViewTextBoxColumn;
    }
}
