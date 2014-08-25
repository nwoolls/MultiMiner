namespace MultiMiner.Win.Forms.Configuration
{
    partial class ProxySettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxySettingsForm));
            this.proxyListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.getworkPortEdit = new System.Windows.Forms.TextBox();
            this.proxyDescriptorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addProxyButton = new System.Windows.Forms.Button();
            this.removeProxyButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.proxyDescriptorBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // proxyListBox
            // 
            this.proxyListBox.FormattingEnabled = true;
            this.proxyListBox.ItemHeight = 15;
            this.proxyListBox.Location = new System.Drawing.Point(16, 16);
            this.proxyListBox.Name = "proxyListBox";
            this.proxyListBox.Size = new System.Drawing.Size(228, 64);
            this.proxyListBox.TabIndex = 0;
            this.proxyListBox.SelectedIndexChanged += new System.EventHandler(this.proxyListBox_SelectedIndexChanged);
            this.proxyListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.proxyListBox_Format);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Getwork port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Stratum port:";
            // 
            // getworkPortEdit
            // 
            this.getworkPortEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.proxyDescriptorBindingSource, "GetWorkPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.getworkPortEdit.Location = new System.Drawing.Point(144, 90);
            this.getworkPortEdit.Name = "getworkPortEdit";
            this.getworkPortEdit.Size = new System.Drawing.Size(100, 23);
            this.getworkPortEdit.TabIndex = 3;
            // 
            // proxyDescriptorBindingSource
            // 
            this.proxyDescriptorBindingSource.DataSource = typeof(MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor);
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.proxyDescriptorBindingSource, "StratumPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(144, 119);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 23);
            this.textBox2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 159);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 54);
            this.panel1.TabIndex = 5;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(156, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(250, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // addProxyButton
            // 
            this.addProxyButton.BackColor = System.Drawing.SystemColors.Control;
            this.addProxyButton.Image = global::MultiMiner.Win.Properties.Resources.add;
            this.addProxyButton.Location = new System.Drawing.Point(254, 16);
            this.addProxyButton.Name = "addProxyButton";
            this.addProxyButton.Size = new System.Drawing.Size(83, 28);
            this.addProxyButton.TabIndex = 1;
            this.addProxyButton.Text = "Add";
            this.addProxyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.addProxyButton.UseVisualStyleBackColor = false;
            this.addProxyButton.Click += new System.EventHandler(this.addProxyButton_Click);
            // 
            // removeProxyButton
            // 
            this.removeProxyButton.BackColor = System.Drawing.SystemColors.Control;
            this.removeProxyButton.Image = global::MultiMiner.Win.Properties.Resources.remove;
            this.removeProxyButton.Location = new System.Drawing.Point(254, 52);
            this.removeProxyButton.Name = "removeProxyButton";
            this.removeProxyButton.Size = new System.Drawing.Size(83, 28);
            this.removeProxyButton.TabIndex = 2;
            this.removeProxyButton.Text = "Remove";
            this.removeProxyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.removeProxyButton.UseVisualStyleBackColor = false;
            this.removeProxyButton.Click += new System.EventHandler(this.removeProxyButton_Click);
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(16, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 9;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProxySettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(352, 213);
            this.Controls.Add(this.addProxyButton);
            this.Controls.Add(this.removeProxyButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.getworkPortEdit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.proxyListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProxySettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Proxy Settings";
            this.Load += new System.EventHandler(this.ProxySettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.proxyDescriptorBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox proxyListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox getworkPortEdit;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addProxyButton;
        private System.Windows.Forms.Button removeProxyButton;
        private System.Windows.Forms.BindingSource proxyDescriptorBindingSource;
        private System.Windows.Forms.Button button1;
    }
}