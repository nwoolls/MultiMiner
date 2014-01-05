namespace MultiMiner.Win
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
            this.xgminerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.scryptConfigLink = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.sha256ParamsEdit = new System.Windows.Forms.TextBox();
            this.scryptParamsEdit = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.proxyPortEdit = new System.Windows.Forms.TextBox();
            this.proxyCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.intervalCombo = new System.Windows.Forms.ComboBox();
            this.autoDesktopCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.disableGpuCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // xgminerConfigurationBindingSource
            // 
            this.xgminerConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Configuration.XgminerConfiguration);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.scryptConfigLink);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.sha256ParamsEdit);
            this.groupBox3.Controls.Add(this.scryptParamsEdit);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(14, 76);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 158);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arguments";
            // 
            // textBox4
            // 
            this.textBox4.AccessibleName = "Scrypt";
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "ScanArguments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(121, 121);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(301, 23);
            this.textBox4.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Hardware scan:";
            // 
            // scryptConfigLink
            // 
            this.scryptConfigLink.AutoSize = true;
            this.scryptConfigLink.Location = new System.Drawing.Point(36, 93);
            this.scryptConfigLink.Name = "scryptConfigLink";
            this.scryptConfigLink.Size = new System.Drawing.Size(350, 15);
            this.scryptConfigLink.TabIndex = 15;
            this.scryptConfigLink.TabStop = true;
            this.scryptConfigLink.Text = "Example Scrypt configurations (required for decent performance)";
            this.scryptConfigLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.scryptConfigLink_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MultiMiner.Win.Properties.Resources.info;
            this.pictureBox1.Location = new System.Drawing.Point(14, 91);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(19, 18);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // sha256ParamsEdit
            // 
            this.sha256ParamsEdit.AccessibleName = "SHA-2";
            this.sha256ParamsEdit.Location = new System.Drawing.Point(121, 22);
            this.sha256ParamsEdit.Name = "sha256ParamsEdit";
            this.sha256ParamsEdit.Size = new System.Drawing.Size(301, 23);
            this.sha256ParamsEdit.TabIndex = 10;
            // 
            // scryptParamsEdit
            // 
            this.scryptParamsEdit.AccessibleName = "Scrypt";
            this.scryptParamsEdit.Location = new System.Drawing.Point(121, 55);
            this.scryptParamsEdit.Name = "scryptParamsEdit";
            this.scryptParamsEdit.Size = new System.Drawing.Size(301, 23);
            this.scryptParamsEdit.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Scrypt:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "SHA-2:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.proxyPortEdit);
            this.groupBox2.Controls.Add(this.proxyCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(14, 365);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(445, 127);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy Settings";
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
            // textBox3
            // 
            this.textBox3.AccessibleName = "Stratum port";
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "StratumProxyStratumPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(124, 90);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(126, 23);
            this.textBox3.TabIndex = 4;
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
            this.proxyPortEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.xgminerConfigurationBindingSource, "StratumProxyPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyPortEdit.Location = new System.Drawing.Point(124, 60);
            this.proxyPortEdit.Name = "proxyPortEdit";
            this.proxyPortEdit.Size = new System.Drawing.Size(126, 23);
            this.proxyPortEdit.TabIndex = 3;
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
            this.checkBox1.Location = new System.Drawing.Point(18, 46);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(135, 19);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Restart miners every:";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.Win.Configuration.ApplicationConfiguration);
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
            this.intervalCombo.Location = new System.Drawing.Point(171, 44);
            this.intervalCombo.Name = "intervalCombo";
            this.intervalCombo.Size = new System.Drawing.Size(93, 23);
            this.intervalCombo.TabIndex = 2;
            // 
            // autoDesktopCheckBox
            // 
            this.autoDesktopCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoDesktopCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "AutoSetDesktopMode", true));
            this.autoDesktopCheckBox.Location = new System.Drawing.Point(291, 6);
            this.autoDesktopCheckBox.Name = "autoDesktopCheckBox";
            this.autoDesktopCheckBox.Size = new System.Drawing.Size(162, 36);
            this.autoDesktopCheckBox.TabIndex = 3;
            this.autoDesktopCheckBox.Text = "Set Dynamic Intensity based on computer use";
            this.autoDesktopCheckBox.UseVisualStyleBackColor = true;
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
            this.groupBox1.Location = new System.Drawing.Point(14, 240);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(445, 119);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "API Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(119, 92);
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
            this.textBox2.Location = new System.Drawing.Point(122, 55);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(300, 23);
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
            this.textBox1.Location = new System.Drawing.Point(122, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(130, 23);
            this.textBox1.TabIndex = 0;
            // 
            // disableGpuCheckbox
            // 
            this.disableGpuCheckbox.AutoSize = true;
            this.disableGpuCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "DisableGpu", true));
            this.disableGpuCheckbox.Location = new System.Drawing.Point(18, 14);
            this.disableGpuCheckbox.Name = "disableGpuCheckbox";
            this.disableGpuCheckbox.Size = new System.Drawing.Size(131, 19);
            this.disableGpuCheckbox.TabIndex = 0;
            this.disableGpuCheckbox.Text = "Disable GPU mining";
            this.disableGpuCheckbox.UseVisualStyleBackColor = true;
            this.disableGpuCheckbox.CheckedChanged += new System.EventHandler(this.disableGpuCheckbox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 503);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 54);
            this.panel1.TabIndex = 8;
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
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "TerminateGpuMiners", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox2.Location = new System.Drawing.Point(291, 46);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(145, 19);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Terminate GPU miners";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // MinerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(471, 557);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.intervalCombo);
            this.Controls.Add(this.autoDesktopCheckBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.disableGpuCheckbox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MinerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Miner Settings";
            this.Load += new System.EventHandler(this.AdvancedSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox disableGpuCheckbox;
        private System.Windows.Forms.BindingSource xgminerConfigurationBindingSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox autoDesktopCheckBox;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox intervalCombo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox proxyPortEdit;
        private System.Windows.Forms.CheckBox proxyCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox sha256ParamsEdit;
        private System.Windows.Forms.TextBox scryptParamsEdit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel scryptConfigLink;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}