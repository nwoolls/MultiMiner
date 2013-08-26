namespace MultiMiner.Win
{
    partial class AdvancedSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.erupterCheckBox = new System.Windows.Forms.CheckBox();
            this.disableGpuCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 197);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 47);
            this.panel1.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(235, 12);
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
            this.cancelButton.Location = new System.Drawing.Point(316, 12);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // erupterCheckBox
            // 
            this.erupterCheckBox.AutoSize = true;
            this.erupterCheckBox.Checked = true;
            this.erupterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.erupterCheckBox.Location = new System.Drawing.Point(12, 35);
            this.erupterCheckBox.Name = "erupterCheckBox";
            this.erupterCheckBox.Size = new System.Drawing.Size(199, 17);
            this.erupterCheckBox.TabIndex = 21;
            this.erupterCheckBox.Text = "Erupter-specific driver (bfgminer only)";
            this.erupterCheckBox.UseVisualStyleBackColor = true;
            // 
            // disableGpuCheckbox
            // 
            this.disableGpuCheckbox.AutoSize = true;
            this.disableGpuCheckbox.Location = new System.Drawing.Point(12, 12);
            this.disableGpuCheckbox.Name = "disableGpuCheckbox";
            this.disableGpuCheckbox.Size = new System.Drawing.Size(120, 17);
            this.disableGpuCheckbox.TabIndex = 19;
            this.disableGpuCheckbox.Text = "Disable GPU mining";
            this.disableGpuCheckbox.UseVisualStyleBackColor = true;
            // 
            // AdvancedSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 244);
            this.Controls.Add(this.erupterCheckBox);
            this.Controls.Add(this.disableGpuCheckbox);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Miner Settings";
            this.Load += new System.EventHandler(this.AdvancedSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox erupterCheckBox;
        private System.Windows.Forms.CheckBox disableGpuCheckbox;
    }
}