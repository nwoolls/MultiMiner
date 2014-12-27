namespace MultiMiner.Win.Forms.Configuration
{
    partial class PerksForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerksForm));
            this.smileyPicture = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.perksConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.perksCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.percentEdit = new System.Windows.Forms.TextBox();
            this.percentLabel1 = new System.Windows.Forms.Label();
            this.percentLabel2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.advancedProxyCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.remotingPasswordEdit = new System.Windows.Forms.TextBox();
            this.remotingCheckBox = new System.Windows.Forms.CheckBox();
            this.incomeCheckBox = new System.Windows.Forms.CheckBox();
            this.exchangeApiCheckbox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.smileyPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.perksConfigurationBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // smileyPicture
            // 
            this.smileyPicture.Image = global::MultiMiner.Win.Properties.Resources.smiley_happy;
            this.smileyPicture.Location = new System.Drawing.Point(329, 70);
            this.smileyPicture.Name = "smileyPicture";
            this.smileyPicture.Size = new System.Drawing.Size(20, 20);
            this.smileyPicture.TabIndex = 10;
            this.smileyPicture.TabStop = false;
            this.smileyPicture.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MultiMiner.Win.Properties.Resources.info1;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 34);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(53, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(339, 52);
            this.label1.TabIndex = 8;
            this.label1.Text = "By enabling the perks in MultiMiner, 1% of your mining resources will go towards " +
    "the software author.";
            // 
            // perksConfigurationBindingSource
            // 
            this.perksConfigurationBindingSource.DataSource = typeof(MultiMiner.UX.Data.Configuration.Perks);
            // 
            // perksCheckBox
            // 
            this.perksCheckBox.AutoSize = true;
            this.perksCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "PerksEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.perksCheckBox.Location = new System.Drawing.Point(56, 69);
            this.perksCheckBox.Name = "perksCheckBox";
            this.perksCheckBox.Size = new System.Drawing.Size(192, 19);
            this.perksCheckBox.TabIndex = 0;
            this.perksCheckBox.Text = "Enable perks (at a 1% donation)";
            this.perksCheckBox.UseVisualStyleBackColor = true;
            this.perksCheckBox.CheckedChanged += new System.EventHandler(this.perksCheckBox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 349);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(407, 54);
            this.panel1.TabIndex = 3;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(211, 14);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(87, 27);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(305, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // percentEdit
            // 
            this.percentEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.perksConfigurationBindingSource, "DonationPercent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.percentEdit.Location = new System.Drawing.Point(271, 303);
            this.percentEdit.Name = "percentEdit";
            this.percentEdit.Size = new System.Drawing.Size(45, 23);
            this.percentEdit.TabIndex = 2;
            this.percentEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.percentEdit.Validating += new System.ComponentModel.CancelEventHandler(this.percentEdit_Validating);
            // 
            // percentLabel1
            // 
            this.percentLabel1.AutoSize = true;
            this.percentLabel1.Location = new System.Drawing.Point(53, 306);
            this.percentLabel1.Name = "percentLabel1";
            this.percentLabel1.Size = new System.Drawing.Size(207, 15);
            this.percentLabel1.TabIndex = 5;
            this.percentLabel1.Text = "Wish to donate more? Total donation:";
            // 
            // percentLabel2
            // 
            this.percentLabel2.AutoSize = true;
            this.percentLabel2.Location = new System.Drawing.Point(322, 306);
            this.percentLabel2.Name = "percentLabel2";
            this.percentLabel2.Size = new System.Drawing.Size(17, 15);
            this.percentLabel2.TabIndex = 13;
            this.percentLabel2.Text = "%";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.advancedProxyCheckBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.remotingPasswordEdit);
            this.groupBox1.Controls.Add(this.remotingCheckBox);
            this.groupBox1.Controls.Add(this.incomeCheckBox);
            this.groupBox1.Controls.Add(this.exchangeApiCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(56, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 181);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Perks";
            // 
            // advancedProxyCheckBox
            // 
            this.advancedProxyCheckBox.AutoSize = true;
            this.advancedProxyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "AdvancedProxying", true));
            this.advancedProxyCheckBox.Location = new System.Drawing.Point(16, 147);
            this.advancedProxyCheckBox.Name = "advancedProxyCheckBox";
            this.advancedProxyCheckBox.Size = new System.Drawing.Size(236, 19);
            this.advancedProxyCheckBox.TabIndex = 21;
            this.advancedProxyCheckBox.Text = "Enable advanced Stratum Proxy support";
            this.advancedProxyCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 15);
            this.label2.TabIndex = 20;
            this.label2.Text = "Password (optional):";
            // 
            // remotingPasswordEdit
            // 
            this.remotingPasswordEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.perksConfigurationBindingSource, "RemotingPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.remotingPasswordEdit.Location = new System.Drawing.Point(158, 112);
            this.remotingPasswordEdit.Name = "remotingPasswordEdit";
            this.remotingPasswordEdit.Size = new System.Drawing.Size(102, 23);
            this.remotingPasswordEdit.TabIndex = 19;
            this.remotingPasswordEdit.Text = "asdf";
            this.remotingPasswordEdit.UseSystemPasswordChar = true;
            // 
            // remotingCheckBox
            // 
            this.remotingCheckBox.AutoSize = true;
            this.remotingCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "EnableRemoting", true));
            this.remotingCheckBox.Location = new System.Drawing.Point(16, 83);
            this.remotingCheckBox.Name = "remotingCheckBox";
            this.remotingCheckBox.Size = new System.Drawing.Size(178, 19);
            this.remotingCheckBox.TabIndex = 18;
            this.remotingCheckBox.Text = "Enable MultiMiner Remoting";
            this.remotingCheckBox.UseVisualStyleBackColor = true;
            // 
            // incomeCheckBox
            // 
            this.incomeCheckBox.AutoSize = true;
            this.incomeCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "ShowIncomeRates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.incomeCheckBox.Location = new System.Drawing.Point(16, 54);
            this.incomeCheckBox.Name = "incomeCheckBox";
            this.incomeCheckBox.Size = new System.Drawing.Size(224, 19);
            this.incomeCheckBox.TabIndex = 17;
            this.incomeCheckBox.Text = "Show estimated income from devices";
            this.incomeCheckBox.UseVisualStyleBackColor = true;
            // 
            // exchangeApiCheckbox
            // 
            this.exchangeApiCheckbox.AutoSize = true;
            this.exchangeApiCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "ShowExchangeRates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.exchangeApiCheckbox.Location = new System.Drawing.Point(16, 26);
            this.exchangeApiCheckbox.Name = "exchangeApiCheckbox";
            this.exchangeApiCheckbox.Size = new System.Drawing.Size(244, 19);
            this.exchangeApiCheckbox.TabIndex = 16;
            this.exchangeApiCheckbox.Text = "Show exchange rates from Blockchain.info";
            this.exchangeApiCheckbox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(12, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PerksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(407, 403);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.percentEdit);
            this.Controls.Add(this.percentLabel2);
            this.Controls.Add(this.percentLabel1);
            this.Controls.Add(this.smileyPicture);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.perksCheckBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PerksForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Perks";
            this.Load += new System.EventHandler(this.PerksForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.smileyPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.perksConfigurationBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox perksCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.BindingSource perksConfigurationBindingSource;
        private System.Windows.Forms.PictureBox smileyPicture;
        private System.Windows.Forms.TextBox percentEdit;
        private System.Windows.Forms.Label percentLabel1;
        private System.Windows.Forms.Label percentLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox remotingPasswordEdit;
        private System.Windows.Forms.CheckBox remotingCheckBox;
        private System.Windows.Forms.CheckBox incomeCheckBox;
        private System.Windows.Forms.CheckBox exchangeApiCheckbox;
        private System.Windows.Forms.CheckBox advancedProxyCheckBox;
        private System.Windows.Forms.Button button1;
    }
}