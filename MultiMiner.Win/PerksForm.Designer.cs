namespace MultiMiner.Win
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
            this.incomeCheckBox = new System.Windows.Forms.CheckBox();
            this.perksConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.coinbaseCheckBox = new System.Windows.Forms.CheckBox();
            this.perksCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.percentEdit = new System.Windows.Forms.TextBox();
            this.percentLabel1 = new System.Windows.Forms.Label();
            this.percentLabel2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.smileyPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.perksConfigurationBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // smileyPicture
            // 
            this.smileyPicture.Image = global::MultiMiner.Win.Properties.Resources.smiley_happy;
            this.smileyPicture.Location = new System.Drawing.Point(313, 70);
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
            this.label1.Size = new System.Drawing.Size(308, 52);
            this.label1.TabIndex = 8;
            this.label1.Text = "By enabling the perks in MultiMiner, 1% of your mining resources will go towards " +
    "the software author.";
            // 
            // incomeCheckBox
            // 
            this.incomeCheckBox.AutoSize = true;
            this.incomeCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "ShowIncomeRates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.incomeCheckBox.Location = new System.Drawing.Point(77, 125);
            this.incomeCheckBox.Name = "incomeCheckBox";
            this.incomeCheckBox.Size = new System.Drawing.Size(224, 19);
            this.incomeCheckBox.TabIndex = 7;
            this.incomeCheckBox.Text = "Show estimated income from devices";
            this.incomeCheckBox.UseVisualStyleBackColor = true;
            // 
            // perksConfigurationBindingSource
            // 
            this.perksConfigurationBindingSource.DataSource = typeof(MultiMiner.Win.Configuration.PerksConfiguration);
            // 
            // coinbaseCheckBox
            // 
            this.coinbaseCheckBox.AutoSize = true;
            this.coinbaseCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "ShowExchangeRates", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.coinbaseCheckBox.Location = new System.Drawing.Point(77, 97);
            this.coinbaseCheckBox.Name = "coinbaseCheckBox";
            this.coinbaseCheckBox.Size = new System.Drawing.Size(217, 19);
            this.coinbaseCheckBox.TabIndex = 6;
            this.coinbaseCheckBox.Text = "Show exchange rates from Coinbase";
            this.coinbaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // perksCheckBox
            // 
            this.perksCheckBox.AutoSize = true;
            this.perksCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.perksConfigurationBindingSource, "PerksEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.perksCheckBox.Location = new System.Drawing.Point(56, 69);
            this.perksCheckBox.Name = "perksCheckBox";
            this.perksCheckBox.Size = new System.Drawing.Size(192, 19);
            this.perksCheckBox.TabIndex = 5;
            this.perksCheckBox.Text = "Enable perks (at a 1% donation)";
            this.perksCheckBox.UseVisualStyleBackColor = true;
            this.perksCheckBox.CheckedChanged += new System.EventHandler(this.perksCheckBox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 199);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(376, 54);
            this.panel1.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(180, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(274, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // percentEdit
            // 
            this.percentEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.perksConfigurationBindingSource, "DonationPercent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.percentEdit.Location = new System.Drawing.Point(205, 155);
            this.percentEdit.Name = "percentEdit";
            this.percentEdit.Size = new System.Drawing.Size(45, 23);
            this.percentEdit.TabIndex = 11;
            this.percentEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.percentEdit.Validating += new System.ComponentModel.CancelEventHandler(this.percentEdit_Validating);
            // 
            // percentLabel1
            // 
            this.percentLabel1.AutoSize = true;
            this.percentLabel1.Location = new System.Drawing.Point(74, 158);
            this.percentLabel1.Name = "percentLabel1";
            this.percentLabel1.Size = new System.Drawing.Size(123, 15);
            this.percentLabel1.TabIndex = 12;
            this.percentLabel1.Text = "Wish to donate more?";
            // 
            // percentLabel2
            // 
            this.percentLabel2.AutoSize = true;
            this.percentLabel2.Location = new System.Drawing.Point(254, 158);
            this.percentLabel2.Name = "percentLabel2";
            this.percentLabel2.Size = new System.Drawing.Size(17, 15);
            this.percentLabel2.TabIndex = 13;
            this.percentLabel2.Text = "%";
            // 
            // PerksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(376, 253);
            this.Controls.Add(this.percentEdit);
            this.Controls.Add(this.percentLabel2);
            this.Controls.Add(this.percentLabel1);
            this.Controls.Add(this.smileyPicture);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.incomeCheckBox);
            this.Controls.Add(this.coinbaseCheckBox);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox perksCheckBox;
        private System.Windows.Forms.CheckBox coinbaseCheckBox;
        private System.Windows.Forms.CheckBox incomeCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.BindingSource perksConfigurationBindingSource;
        private System.Windows.Forms.PictureBox smileyPicture;
        private System.Windows.Forms.TextBox percentEdit;
        private System.Windows.Forms.Label percentLabel1;
        private System.Windows.Forms.Label percentLabel2;
    }
}