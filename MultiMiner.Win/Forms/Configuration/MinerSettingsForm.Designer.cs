namespace MultiMiner.Win.Forms.Configuration
{
    partial class MinerSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinerSettingsForm));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.algoArgEdit = new System.Windows.Forms.TextBox();
            this.algoArgCombo = new System.Windows.Forms.ComboBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.xgminerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.advancedProxiesButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.stratumProxyPortEdit = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.proxyPortEdit = new System.Windows.Forms.TextBox();
            this.proxyCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.intervalCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.gpuSettingsLink = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.networkIntervalCombo = new System.Windows.Forms.ComboBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.algoArgEdit);
            this.groupBox3.Controls.Add(this.algoArgCombo);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(14, 97);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 93);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arguments";
            // 
            // algoArgEdit
            // 
            this.algoArgEdit.AccessibleName = "Permitted IPs";
            this.algoArgEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algoArgEdit.Location = new System.Drawing.Point(119, 22);
            this.algoArgEdit.Name = "algoArgEdit";
            this.algoArgEdit.Size = new System.Drawing.Size(311, 23);
            this.algoArgEdit.TabIndex = 1;
            this.algoArgEdit.Validated += new System.EventHandler(this.algoArgEdit_Validated);
            // 
            // algoArgCombo
            // 
            this.algoArgCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algoArgCombo.FormattingEnabled = true;
            this.algoArgCombo.Items.AddRange(new object[] {
            "Scrypt",
            "Scrypt-Jane",
            "Scrypt-N",
            "SHA-256",
            "X11"});
            this.algoArgCombo.Location = new System.Drawing.Point(14, 22);
            this.algoArgCombo.Name = "algoArgCombo";
            this.algoArgCombo.Size = new System.Drawing.Size(99, 23);
            this.algoArgCombo.Sorted = true;
            this.algoArgCombo.TabIndex = 0;
            this.algoArgCombo.SelectedIndexChanged += new System.EventHandler(this.argAlgoCombo_SelectedIndexChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "DisableUsbProbe", true));
            this.checkBox3.Location = new System.Drawing.Point(275, 57);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(133, 19);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.Text = "Disable USB probing";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // xgminerConfigurationBindingSource
            // 
            this.xgminerConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Data.Configuration.Xgminer);
            // 
            // textBox4
            // 
            this.textBox4.AccessibleName = "Scrypt";
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "ScanArguments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(119, 55);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(143, 23);
            this.textBox4.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "Hardware scan:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.advancedProxiesButton);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.stratumProxyPortEdit);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.proxyPortEdit);
            this.groupBox2.Controls.Add(this.proxyCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(14, 296);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(445, 127);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy Settings";
            // 
            // advancedProxiesButton
            // 
            this.advancedProxiesButton.Location = new System.Drawing.Point(349, 24);
            this.advancedProxiesButton.Name = "advancedProxiesButton";
            this.advancedProxiesButton.Size = new System.Drawing.Size(81, 28);
            this.advancedProxiesButton.TabIndex = 16;
            this.advancedProxiesButton.Text = "Advanced";
            this.advancedProxiesButton.UseVisualStyleBackColor = true;
            this.advancedProxiesButton.Click += new System.EventHandler(this.advancedProxiesButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 15);
            this.label5.TabIndex = 15;
            this.label5.Text = "Stratum port:";
            // 
            // stratumProxyPortEdit
            // 
            this.stratumProxyPortEdit.AccessibleName = "Stratum port";
            this.stratumProxyPortEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stratumProxyPortEdit.Location = new System.Drawing.Point(119, 89);
            this.stratumProxyPortEdit.Name = "stratumProxyPortEdit";
            this.stratumProxyPortEdit.Size = new System.Drawing.Size(143, 23);
            this.stratumProxyPortEdit.TabIndex = 4;
            this.stratumProxyPortEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.proxyPortEdit_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Getwork port:";
            // 
            // proxyPortEdit
            // 
            this.proxyPortEdit.AccessibleName = "Getwork port";
            this.proxyPortEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyPortEdit.Location = new System.Drawing.Point(119, 60);
            this.proxyPortEdit.Name = "proxyPortEdit";
            this.proxyPortEdit.Size = new System.Drawing.Size(143, 23);
            this.proxyPortEdit.TabIndex = 3;
            this.proxyPortEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.proxyPortEdit_KeyPress);
            // 
            // proxyCheckBox
            // 
            this.proxyCheckBox.AutoSize = true;
            this.proxyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "StratumProxy", true));
            this.proxyCheckBox.Location = new System.Drawing.Point(16, 30);
            this.proxyCheckBox.Name = "proxyCheckBox";
            this.proxyCheckBox.Size = new System.Drawing.Size(322, 19);
            this.proxyCheckBox.TabIndex = 2;
            this.proxyCheckBox.Text = "Enable stratum proxy (for stand-alone mining hardware)";
            this.proxyCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "ScheduledRestartMining", true));
            this.checkBox1.Location = new System.Drawing.Point(18, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(137, 19);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Restart mining every:";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // intervalCombo
            // 
            this.intervalCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.intervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.intervalCombo.FormattingEnabled = true;
            this.intervalCombo.Items.AddRange(new object[] {
            "5 minutes",
            "15 minutes",
            "30 minutes",
            "1 hour",
            "3 hours",
            "6 hours",
            "12 hours"});
            this.intervalCombo.Location = new System.Drawing.Point(210, 13);
            this.intervalCombo.Name = "intervalCombo";
            this.intervalCombo.Size = new System.Drawing.Size(117, 23);
            this.intervalCombo.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(14, 196);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(445, 94);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "API Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(272, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 15);
            this.label3.TabIndex = 28;
            this.label3.Text = "127.0.0.1 is assumed/implied";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 15);
            this.label2.TabIndex = 27;
            this.label2.Text = "Permitted IPs:";
            // 
            // textBox2
            // 
            this.textBox2.AccessibleName = "Permitted IPs";
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "AllowedApiIps", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(119, 55);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(143, 23);
            this.textBox2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 25;
            this.label1.Text = "Starting port:";
            // 
            // textBox1
            // 
            this.textBox1.AccessibleName = "Starting port";
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "StartingApiPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(119, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(143, 23);
            this.textBox1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 437);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 54);
            this.panel1.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(14, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(275, 14);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(87, 27);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(369, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // gpuSettingsLink
            // 
            this.gpuSettingsLink.AutoSize = true;
            this.gpuSettingsLink.Location = new System.Drawing.Point(40, 73);
            this.gpuSettingsLink.Name = "gpuSettingsLink";
            this.gpuSettingsLink.Size = new System.Drawing.Size(164, 15);
            this.gpuSettingsLink.TabIndex = 4;
            this.gpuSettingsLink.TabStop = true;
            this.gpuSettingsLink.Text = "Advanced GPU miner settings";
            this.gpuSettingsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.gpuSettingsLink_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MultiMiner.Win.Properties.Resources.hardware_option;
            this.pictureBox1.Location = new System.Drawing.Point(18, 71);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(19, 18);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "ScheduledRestartNetworkDevices", true));
            this.checkBox2.Location = new System.Drawing.Point(18, 44);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(187, 19);
            this.checkBox2.TabIndex = 17;
            this.checkBox2.Text = "Restart Network Devices every:";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // networkIntervalCombo
            // 
            this.networkIntervalCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.networkIntervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.networkIntervalCombo.FormattingEnabled = true;
            this.networkIntervalCombo.Items.AddRange(new object[] {
            "5 minutes",
            "15 minutes",
            "30 minutes",
            "1 hour",
            "3 hours",
            "6 hours",
            "12 hours"});
            this.networkIntervalCombo.Location = new System.Drawing.Point(210, 42);
            this.networkIntervalCombo.Name = "networkIntervalCombo";
            this.networkIntervalCombo.Size = new System.Drawing.Size(117, 23);
            this.networkIntervalCombo.TabIndex = 2;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "ScheduledRebootNetworkDevices", true));
            this.checkBox4.Location = new System.Drawing.Point(339, 44);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(105, 19);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "Reboot instead";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.UX.Data.Configuration.Application);
            // 
            // MinerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(471, 491);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.networkIntervalCombo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gpuSettingsLink);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.intervalCombo);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MinerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Miner Settings";
            this.Load += new System.EventHandler(this.AdvancedSettingsForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.BindingSource xgminerConfigurationBindingSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox intervalCombo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox proxyPortEdit;
        private System.Windows.Forms.CheckBox proxyCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox stratumProxyPortEdit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.ComboBox algoArgCombo;
        private System.Windows.Forms.TextBox algoArgEdit;
        private System.Windows.Forms.Button advancedProxiesButton;
        private System.Windows.Forms.LinkLabel gpuSettingsLink;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ComboBox networkIntervalCombo;
        private System.Windows.Forms.CheckBox checkBox4;
    }
}