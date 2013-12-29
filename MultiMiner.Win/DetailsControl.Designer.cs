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
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cryptoCoinBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            this.workersGridView = new System.Windows.Forms.DataGridView();
            this.deviceInformationResponseBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            this.label25 = new System.Windows.Forms.Label();
            this.averageHashrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentHashrateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptedSharesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rejectedSharesPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareErrorsPercentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.utilityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinInformationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceDetailsResponseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workersGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInformationResponseBindingSource)).BeginInit();
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
            this.nameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nameLabel.Location = new System.Drawing.Point(79, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(411, 22);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "label1";
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.Xgminer.Device);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Name", true));
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(79, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // cryptoCoinBindingSource
            // 
            this.cryptoCoinBindingSource.DataSource = typeof(MultiMiner.Engine.CryptoCoin);
            // 
            // coinInformationBindingSource
            // 
            this.coinInformationBindingSource.DataSource = typeof(MultiMiner.Coin.Api.CoinInformation);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Symbol", true));
            this.label3.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.label4.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.label5.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.label6.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.label8.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.label10.ForeColor = System.Drawing.SystemColors.GrayText;
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
            this.hashrateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hashrateLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.hashrateLabel.Location = new System.Drawing.Point(79, 126);
            this.hashrateLabel.Name = "hashrateLabel";
            this.hashrateLabel.Size = new System.Drawing.Size(48, 13);
            this.hashrateLabel.TabIndex = 16;
            this.hashrateLabel.Text = "label12";
            // 
            // deviceDetailsResponseBindingSource
            // 
            this.deviceDetailsResponseBindingSource.DataSource = typeof(MultiMiner.Xgminer.Api.Responses.DeviceDetailsResponse);
            // 
            // workersGridView
            // 
            this.workersGridView.AllowUserToAddRows = false;
            this.workersGridView.AllowUserToDeleteRows = false;
            this.workersGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workersGridView.AutoGenerateColumns = false;
            this.workersGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.workersGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.workersGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.workersGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.workersGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.averageHashrateDataGridViewTextBoxColumn,
            this.currentHashrateDataGridViewTextBoxColumn,
            this.acceptedSharesDataGridViewTextBoxColumn,
            this.rejectedSharesPercentDataGridViewTextBoxColumn,
            this.hardwareErrorsPercentDataGridViewTextBoxColumn,
            this.utilityDataGridViewTextBoxColumn});
            this.workersGridView.DataSource = this.deviceInformationResponseBindingSource;
            this.workersGridView.GridColor = System.Drawing.SystemColors.ControlLightLight;
            this.workersGridView.Location = new System.Drawing.Point(6, 246);
            this.workersGridView.Name = "workersGridView";
            this.workersGridView.ReadOnly = true;
            this.workersGridView.RowHeadersVisible = false;
            this.workersGridView.Size = new System.Drawing.Size(503, 161);
            this.workersGridView.TabIndex = 17;
            this.workersGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.workersGridView_CellFormatting);
            // 
            // deviceInformationResponseBindingSource
            // 
            this.deviceInformationResponseBindingSource.DataSource = typeof(MultiMiner.Xgminer.Api.Responses.DeviceInformationResponse);
            // 
            // workersTitleLabel
            // 
            this.workersTitleLabel.AutoSize = true;
            this.workersTitleLabel.Location = new System.Drawing.Point(6, 230);
            this.workersTitleLabel.Name = "workersTitleLabel";
            this.workersTitleLabel.Size = new System.Drawing.Size(50, 13);
            this.workersTitleLabel.TabIndex = 18;
            this.workersTitleLabel.Text = "Workers:";
            // 
            // tempLabel
            // 
            this.tempLabel.AutoSize = true;
            this.tempLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tempLabel.Location = new System.Drawing.Point(79, 139);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(48, 13);
            this.tempLabel.TabIndex = 20;
            this.tempLabel.Text = "label14";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 139);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "Temp:";
            // 
            // fanLabel
            // 
            this.fanLabel.AutoSize = true;
            this.fanLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.fanLabel.Location = new System.Drawing.Point(79, 152);
            this.fanLabel.Name = "fanLabel";
            this.fanLabel.Size = new System.Drawing.Size(41, 13);
            this.fanLabel.TabIndex = 22;
            this.fanLabel.Text = "label16";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 152);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(28, 13);
            this.label17.TabIndex = 21;
            this.label17.Text = "Fan:";
            // 
            // acceptedLabel
            // 
            this.acceptedLabel.AutoSize = true;
            this.acceptedLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.acceptedLabel.Location = new System.Drawing.Point(79, 165);
            this.acceptedLabel.Name = "acceptedLabel";
            this.acceptedLabel.Size = new System.Drawing.Size(41, 13);
            this.acceptedLabel.TabIndex = 24;
            this.acceptedLabel.Text = "label18";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 165);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 13);
            this.label19.TabIndex = 23;
            this.label19.Text = "Accepted:";
            // 
            // rejectedLabel
            // 
            this.rejectedLabel.AutoSize = true;
            this.rejectedLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.rejectedLabel.Location = new System.Drawing.Point(79, 178);
            this.rejectedLabel.Name = "rejectedLabel";
            this.rejectedLabel.Size = new System.Drawing.Size(41, 13);
            this.rejectedLabel.TabIndex = 26;
            this.rejectedLabel.Text = "label20";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 178);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(53, 13);
            this.label21.TabIndex = 25;
            this.label21.Text = "Rejected:";
            // 
            // errorsLabel
            // 
            this.errorsLabel.AutoSize = true;
            this.errorsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.errorsLabel.Location = new System.Drawing.Point(79, 191);
            this.errorsLabel.Name = "errorsLabel";
            this.errorsLabel.Size = new System.Drawing.Size(41, 13);
            this.errorsLabel.TabIndex = 28;
            this.errorsLabel.Text = "label22";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 191);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(37, 13);
            this.label23.TabIndex = 27;
            this.label23.Text = "Errors:";
            // 
            // utilityLabel
            // 
            this.utilityLabel.AutoSize = true;
            this.utilityLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.utilityLabel.Location = new System.Drawing.Point(79, 204);
            this.utilityLabel.Name = "utilityLabel";
            this.utilityLabel.Size = new System.Drawing.Size(41, 13);
            this.utilityLabel.TabIndex = 30;
            this.utilityLabel.Text = "label24";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 204);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(35, 13);
            this.label25.TabIndex = 29;
            this.label25.Text = "Utility:";
            // 
            // averageHashrateDataGridViewTextBoxColumn
            // 
            this.averageHashrateDataGridViewTextBoxColumn.DataPropertyName = "AverageHashrate";
            this.averageHashrateDataGridViewTextBoxColumn.HeaderText = "Average";
            this.averageHashrateDataGridViewTextBoxColumn.Name = "averageHashrateDataGridViewTextBoxColumn";
            this.averageHashrateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currentHashrateDataGridViewTextBoxColumn
            // 
            this.currentHashrateDataGridViewTextBoxColumn.DataPropertyName = "CurrentHashrate";
            this.currentHashrateDataGridViewTextBoxColumn.HeaderText = "Current";
            this.currentHashrateDataGridViewTextBoxColumn.Name = "currentHashrateDataGridViewTextBoxColumn";
            this.currentHashrateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // acceptedSharesDataGridViewTextBoxColumn
            // 
            this.acceptedSharesDataGridViewTextBoxColumn.DataPropertyName = "AcceptedShares";
            this.acceptedSharesDataGridViewTextBoxColumn.FillWeight = 50F;
            this.acceptedSharesDataGridViewTextBoxColumn.HeaderText = "Acc.";
            this.acceptedSharesDataGridViewTextBoxColumn.Name = "acceptedSharesDataGridViewTextBoxColumn";
            this.acceptedSharesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // rejectedSharesPercentDataGridViewTextBoxColumn
            // 
            this.rejectedSharesPercentDataGridViewTextBoxColumn.DataPropertyName = "RejectedSharesPercent";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.FillWeight = 50F;
            this.rejectedSharesPercentDataGridViewTextBoxColumn.HeaderText = "Rej. %";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.Name = "rejectedSharesPercentDataGridViewTextBoxColumn";
            this.rejectedSharesPercentDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // hardwareErrorsPercentDataGridViewTextBoxColumn
            // 
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.DataPropertyName = "HardwareErrorsPercent";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.FillWeight = 50F;
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.HeaderText = "HW %";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.Name = "hardwareErrorsPercentDataGridViewTextBoxColumn";
            this.hardwareErrorsPercentDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // utilityDataGridViewTextBoxColumn
            // 
            this.utilityDataGridViewTextBoxColumn.DataPropertyName = "Utility";
            this.utilityDataGridViewTextBoxColumn.FillWeight = 60F;
            this.utilityDataGridViewTextBoxColumn.HeaderText = "Utility";
            this.utilityDataGridViewTextBoxColumn.Name = "utilityDataGridViewTextBoxColumn";
            this.utilityDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // DetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.utilityLabel);
            this.Controls.Add(this.label25);
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
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "DetailsControl";
            this.Size = new System.Drawing.Size(515, 410);
            this.Load += new System.EventHandler(this.DetailsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinInformationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceDetailsResponseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workersGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceInformationResponseBindingSource)).EndInit();
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
        private System.Windows.Forms.DataGridView workersGridView;
        private System.Windows.Forms.BindingSource deviceInformationResponseBindingSource;
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
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.DataGridViewTextBoxColumn averageHashrateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currentHashrateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn acceptedSharesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rejectedSharesPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hardwareErrorsPercentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn utilityDataGridViewTextBoxColumn;
    }
}
