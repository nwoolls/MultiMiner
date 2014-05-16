namespace MultiMiner.Win.Forms.Configuration
{
    partial class GPUMinerSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GPUMinerSettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.xgminerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.disableGpuCheckbox = new System.Windows.Forms.CheckBox();
            this.algoCombo = new System.Windows.Forms.ComboBox();
            this.minerCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 162);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 54);
            this.panel1.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(149, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(243, 14);
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
            this.checkBox2.Location = new System.Drawing.Point(17, 106);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(145, 19);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Terminate GPU miners";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // xgminerConfigurationBindingSource
            // 
            this.xgminerConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Data.Configuration.Xgminer);
            // 
            // disableGpuCheckbox
            // 
            this.disableGpuCheckbox.AutoSize = true;
            this.disableGpuCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "DisableGpu", true));
            this.disableGpuCheckbox.Location = new System.Drawing.Point(17, 69);
            this.disableGpuCheckbox.Name = "disableGpuCheckbox";
            this.disableGpuCheckbox.Size = new System.Drawing.Size(131, 19);
            this.disableGpuCheckbox.TabIndex = 5;
            this.disableGpuCheckbox.Text = "Disable GPU mining";
            this.disableGpuCheckbox.UseVisualStyleBackColor = true;
            // 
            // algoCombo
            // 
            this.algoCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algoCombo.FormattingEnabled = true;
            this.algoCombo.Location = new System.Drawing.Point(17, 22);
            this.algoCombo.Name = "algoCombo";
            this.algoCombo.Size = new System.Drawing.Size(105, 23);
            this.algoCombo.TabIndex = 7;
            this.algoCombo.SelectedIndexChanged += new System.EventHandler(this.algoCombo_SelectedIndexChanged);
            // 
            // minerCombo
            // 
            this.minerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.minerCombo.FormattingEnabled = true;
            this.minerCombo.Location = new System.Drawing.Point(175, 22);
            this.minerCombo.Name = "minerCombo";
            this.minerCombo.Size = new System.Drawing.Size(153, 23);
            this.minerCombo.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Miner:";
            // 
            // GPUMinerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(345, 216);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.minerCombo);
            this.Controls.Add(this.algoCombo);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.disableGpuCheckbox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GPUMinerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure GPU Miner Settings";
            this.Load += new System.EventHandler(this.GPUMinerSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.BindingSource xgminerConfigurationBindingSource;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox disableGpuCheckbox;
        private System.Windows.Forms.ComboBox algoCombo;
        private System.Windows.Forms.ComboBox minerCombo;
        private System.Windows.Forms.Label label1;
    }
}