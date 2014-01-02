namespace MultiMiner.Win
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
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.httpsMobileMinerCheck = new System.Windows.Forms.CheckBox();
            this.pushNotificationsCheck = new System.Windows.Forms.CheckBox();
            this.remoteCommandsCheck = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 146);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 62);
            this.panel1.TabIndex = 2;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(143, 16);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(101, 31);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(253, 16);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(101, 31);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "ShowApiErrors", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox5.Location = new System.Drawing.Point(22, 109);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(182, 19);
            this.checkBox5.TabIndex = 1;
            this.checkBox5.Text = "Display API error notifications";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.Win.Configuration.ApplicationConfiguration);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.httpsMobileMinerCheck);
            this.groupBox1.Controls.Add(this.pushNotificationsCheck);
            this.groupBox1.Controls.Add(this.remoteCommandsCheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 82);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MobileMiner";
            // 
            // httpsMobileMinerCheck
            // 
            this.httpsMobileMinerCheck.AutoSize = true;
            this.httpsMobileMinerCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationConfigurationBindingSource, "MobileMinerUsesHttps", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.httpsMobileMinerCheck.Location = new System.Drawing.Point(222, 22);
            this.httpsMobileMinerCheck.Name = "httpsMobileMinerCheck";
            this.httpsMobileMinerCheck.Size = new System.Drawing.Size(84, 19);
            this.httpsMobileMinerCheck.TabIndex = 2;
            this.httpsMobileMinerCheck.Text = "Use HTTPS";
            this.httpsMobileMinerCheck.UseVisualStyleBackColor = true;
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
            this.remoteCommandsCheck.Size = new System.Drawing.Size(143, 19);
            this.remoteCommandsCheck.TabIndex = 0;
            this.remoteCommandsCheck.Text = "Enable remote control";
            this.remoteCommandsCheck.UseVisualStyleBackColor = true;
            // 
            // OnlineSettingsForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(372, 208);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox httpsMobileMinerCheck;
        private System.Windows.Forms.CheckBox pushNotificationsCheck;
        private System.Windows.Forms.CheckBox remoteCommandsCheck;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
    }
}