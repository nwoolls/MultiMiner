namespace MultiMiner.Win
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.disableGpuCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bfgminerRadio = new System.Windows.Forms.RadioButton();
            this.cgminerRadio = new System.Windows.Forms.RadioButton();
            this.sha256ParamsEdit = new System.Windows.Forms.TextBox();
            this.scryptParamsEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.autoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 417);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 47);
            this.panel1.TabIndex = 3;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(276, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(357, 12);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.disableGpuCheckbox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.bfgminerRadio);
            this.groupBox1.Controls.Add(this.cgminerRadio);
            this.groupBox1.Controls.Add(this.sha256ParamsEdit);
            this.groupBox1.Controls.Add(this.scryptParamsEdit);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Miner";
            // 
            // checkBox4
            // 
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "DetectDisownedMiners", true));
            this.checkBox4.Location = new System.Drawing.Point(239, 107);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(174, 31);
            this.checkBox4.TabIndex = 6;
            this.checkBox4.Text = "Detect disowned miners (orphaned mining processes)";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "RestartCrashedMiners", true));
            this.checkBox2.Location = new System.Drawing.Point(12, 130);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(221, 17);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "Restart sick/dead/frozen/crashed miners";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // disableGpuCheckbox
            // 
            this.disableGpuCheckbox.AutoSize = true;
            this.disableGpuCheckbox.Location = new System.Drawing.Point(12, 107);
            this.disableGpuCheckbox.Name = "disableGpuCheckbox";
            this.disableGpuCheckbox.Size = new System.Drawing.Size(120, 17);
            this.disableGpuCheckbox.TabIndex = 4;
            this.disableGpuCheckbox.Text = "Disable GPU mining";
            this.disableGpuCheckbox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Mining backend:";
            // 
            // bfgminerRadio
            // 
            this.bfgminerRadio.AutoSize = true;
            this.bfgminerRadio.Location = new System.Drawing.Point(239, 21);
            this.bfgminerRadio.Name = "bfgminerRadio";
            this.bfgminerRadio.Size = new System.Drawing.Size(65, 17);
            this.bfgminerRadio.TabIndex = 1;
            this.bfgminerRadio.Text = "bfgminer";
            this.bfgminerRadio.UseVisualStyleBackColor = true;
            // 
            // cgminerRadio
            // 
            this.cgminerRadio.AutoSize = true;
            this.cgminerRadio.Checked = true;
            this.cgminerRadio.Location = new System.Drawing.Point(117, 21);
            this.cgminerRadio.Name = "cgminerRadio";
            this.cgminerRadio.Size = new System.Drawing.Size(62, 17);
            this.cgminerRadio.TabIndex = 0;
            this.cgminerRadio.TabStop = true;
            this.cgminerRadio.Text = "cgminer";
            this.cgminerRadio.UseVisualStyleBackColor = true;
            // 
            // sha256ParamsEdit
            // 
            this.sha256ParamsEdit.Location = new System.Drawing.Point(117, 48);
            this.sha256ParamsEdit.Name = "sha256ParamsEdit";
            this.sha256ParamsEdit.Size = new System.Drawing.Size(286, 20);
            this.sha256ParamsEdit.TabIndex = 2;
            // 
            // scryptParamsEdit
            // 
            this.scryptParamsEdit.Location = new System.Drawing.Point(117, 77);
            this.scryptParamsEdit.Name = "scryptParamsEdit";
            this.scryptParamsEdit.Size = new System.Drawing.Size(286, 20);
            this.scryptParamsEdit.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Scrypt parameters:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "SHA256 parameters:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.autoLaunchCheckBox);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 106);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Application";
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MinimizeToNotificationArea", true));
            this.checkBox1.Location = new System.Drawing.Point(239, 25);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(174, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Minimize to the notification area (system tray)";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(176, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "seconds";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "StartMiningOnStartup", true));
            this.checkBox3.Location = new System.Drawing.Point(12, 48);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(182, 17);
            this.checkBox3.TabIndex = 1;
            this.checkBox3.Text = "Begin mining when the app starts";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Delay mining by";
            // 
            // autoLaunchCheckBox
            // 
            this.autoLaunchCheckBox.AutoSize = true;
            this.autoLaunchCheckBox.Checked = true;
            this.autoLaunchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoLaunchCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "LaunchOnWindowsLogin", true));
            this.autoLaunchCheckBox.Location = new System.Drawing.Point(12, 25);
            this.autoLaunchCheckBox.Name = "autoLaunchCheckBox";
            this.autoLaunchCheckBox.Size = new System.Drawing.Size(198, 17);
            this.autoLaunchCheckBox.TabIndex = 0;
            this.autoLaunchCheckBox.Text = "Launch when you log in to Windows";
            this.autoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.applicationConfigurationBindingSource, "StartupMiningDelay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(118, 71);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "45";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 304);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(420, 112);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MobileMiner";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Application key:";
            
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "URL:";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.applicationConfigurationBindingSource, "MobileMinerUrl", true));
            this.textBox2.Location = new System.Drawing.Point(116, 25);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(286, 20);
            this.textBox2.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.applicationConfigurationBindingSource, "MobileMinerEmailAddress", true));
            this.textBox3.Location = new System.Drawing.Point(116, 51);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(286, 20);
            this.textBox3.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Email address:";
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.applicationConfigurationBindingSource, "MobileMinerApplicationKey", true));
            this.textBox4.Location = new System.Drawing.Point(116, 77);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(286, 20);
            this.textBox4.TabIndex = 2;
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.Win.ApplicationConfiguration);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 464);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox sha256ParamsEdit;
        private System.Windows.Forms.TextBox scryptParamsEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
        private System.Windows.Forms.RadioButton bfgminerRadio;
        private System.Windows.Forms.RadioButton cgminerRadio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox disableGpuCheckbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox autoLaunchCheckBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label7;
    }
}