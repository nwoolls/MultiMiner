namespace MultiMiner.Win.Forms.Configuration
{
    partial class OnlineSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineSettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.httpsMobileMinerCheck = new System.Windows.Forms.CheckBox();
            this.networkOnlyCheck = new System.Windows.Forms.CheckBox();
            this.pushNotificationsCheck = new System.Windows.Forms.CheckBox();
            this.remoteCommandsCheck = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.suggestionsCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.intervalCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 230);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 54);
            this.panel1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(12, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 10;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(151, 13);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(101, 27);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(261, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(101, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox5.AutoSize = true;
            this.checkBox5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "ShowApiErrors", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox5.Location = new System.Drawing.Point(22, 188);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(182, 19);
            this.checkBox5.TabIndex = 2;
            this.checkBox5.Text = "Display API error notifications";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.UX.Data.Configuration.Application);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.httpsMobileMinerCheck);
            this.groupBox1.Controls.Add(this.networkOnlyCheck);
            this.groupBox1.Controls.Add(this.pushNotificationsCheck);
            this.groupBox1.Controls.Add(this.remoteCommandsCheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 82);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MobileMiner";
            this.groupBox1.Visible = false;
            // 
            // httpsMobileMinerCheck
            // 
            this.httpsMobileMinerCheck.AutoSize = true;
            this.httpsMobileMinerCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MobileMinerUsesHttps", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.httpsMobileMinerCheck.Location = new System.Drawing.Point(220, 49);
            this.httpsMobileMinerCheck.Name = "httpsMobileMinerCheck";
            this.httpsMobileMinerCheck.Size = new System.Drawing.Size(84, 19);
            this.httpsMobileMinerCheck.TabIndex = 3;
            this.httpsMobileMinerCheck.Text = "Use HTTPS";
            this.httpsMobileMinerCheck.UseVisualStyleBackColor = true;
            // 
            // networkOnlyCheck
            // 
            this.networkOnlyCheck.AutoSize = true;
            this.networkOnlyCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MobileMinerNetworkMonitorOnly", true));
            this.networkOnlyCheck.Location = new System.Drawing.Point(220, 22);
            this.networkOnlyCheck.Name = "networkOnlyCheck";
            this.networkOnlyCheck.Size = new System.Drawing.Size(97, 19);
            this.networkOnlyCheck.TabIndex = 2;
            this.networkOnlyCheck.Text = "Network only";
            this.networkOnlyCheck.UseVisualStyleBackColor = true;
            // 
            // pushNotificationsCheck
            // 
            this.pushNotificationsCheck.AutoSize = true;
            this.pushNotificationsCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MobileMinerPushNotifications", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pushNotificationsCheck.Location = new System.Drawing.Point(10, 49);
            this.pushNotificationsCheck.Name = "pushNotificationsCheck";
            this.pushNotificationsCheck.Size = new System.Drawing.Size(183, 19);
            this.pushNotificationsCheck.TabIndex = 1;
            this.pushNotificationsCheck.Text = "Push MultiMiner notifications";
            this.pushNotificationsCheck.UseVisualStyleBackColor = true;
            // 
            // remoteCommandsCheck
            // 
            this.remoteCommandsCheck.AutoSize = true;
            this.remoteCommandsCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MobileMinerRemoteCommands", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.remoteCommandsCheck.Location = new System.Drawing.Point(10, 22);
            this.remoteCommandsCheck.Name = "remoteCommandsCheck";
            this.remoteCommandsCheck.Size = new System.Drawing.Size(176, 19);
            this.remoteCommandsCheck.TabIndex = 0;
            this.remoteCommandsCheck.Text = "Enable remote management";
            this.remoteCommandsCheck.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.suggestionsCombo);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.intervalCombo);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 66);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Coin API";
            // 
            // suggestionsCombo
            // 
            this.suggestionsCombo.AccessibleName = "Suggest coins";
            this.suggestionsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.suggestionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.suggestionsCombo.FormattingEnabled = true;
            this.suggestionsCombo.Items.AddRange(new object[] {
            "None",
            "SHA-256",
            "Scrypt",
            "Both"});
            this.suggestionsCombo.Location = new System.Drawing.Point(248, 25);
            this.suggestionsCombo.Name = "suggestionsCombo";
            this.suggestionsCombo.Size = new System.Drawing.Size(91, 23);
            this.suggestionsCombo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 28;
            this.label1.Text = "Suggest:";
            // 
            // intervalCombo
            // 
            this.intervalCombo.AccessibleName = "Check every";
            this.intervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.intervalCombo.FormattingEnabled = true;
            this.intervalCombo.Items.AddRange(new object[] {
            "5 minutes",
            "15 minutes",
            "30 minutes",
            "1 hour",
            "2 hours",
            "3 hours",
            "6 hours",
            "12 hours"});
            this.intervalCombo.Location = new System.Drawing.Point(87, 25);
            this.intervalCombo.Name = "intervalCombo";
            this.intervalCombo.Size = new System.Drawing.Size(92, 23);
            this.intervalCombo.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 15);
            this.label5.TabIndex = 27;
            this.label5.Text = "Check every:";
            // 
            // OnlineSettingsForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(377, 284);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OnlineSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Service Settings";
            this.Load += new System.EventHandler(this.OnlineSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox networkOnlyCheck;
        private System.Windows.Forms.CheckBox pushNotificationsCheck;
        private System.Windows.Forms.CheckBox remoteCommandsCheck;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox suggestionsCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox intervalCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox httpsMobileMinerCheck;
    }
}