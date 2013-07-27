namespace MultiMiner.Win
{
    partial class StrategiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StrategiesForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.strategyConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.singleCoinRadio = new System.Windows.Forms.RadioButton();
            this.multiCoinRadio = new System.Windows.Forms.RadioButton();
            this.minCoinCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exceptPercentLabel = new System.Windows.Forms.Label();
            this.exceptionEdit = new System.Windows.Forms.TextBox();
            this.exceptionLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.minPercentageEdit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.proftabilityBasisCombo = new System.Windows.Forms.ComboBox();
            this.intervalCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.strategyConfigurationBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 264);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 47);
            this.panel1.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(163, 12);
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
            this.cancelButton.Location = new System.Drawing.Point(244, 12);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.strategyConfigurationBindingSource, "MineProfitableCoins", true));
            this.checkBox1.Location = new System.Drawing.Point(12, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(236, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Automatically mine the most profitable coin(s)";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // strategyConfigurationBindingSource
            // 
            this.strategyConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Configuration.StrategyConfiguration);
            // 
            // singleCoinRadio
            // 
            this.singleCoinRadio.AutoSize = true;
            this.singleCoinRadio.Checked = true;
            this.singleCoinRadio.Location = new System.Drawing.Point(19, 86);
            this.singleCoinRadio.Name = "singleCoinRadio";
            this.singleCoinRadio.Size = new System.Drawing.Size(212, 17);
            this.singleCoinRadio.TabIndex = 2;
            this.singleCoinRadio.TabStop = true;
            this.singleCoinRadio.Text = "Mine only the single most profitable coin";
            this.singleCoinRadio.UseVisualStyleBackColor = true;
            // 
            // multiCoinRadio
            // 
            this.multiCoinRadio.AutoSize = true;
            this.multiCoinRadio.Location = new System.Drawing.Point(19, 113);
            this.multiCoinRadio.Name = "multiCoinRadio";
            this.multiCoinRadio.Size = new System.Drawing.Size(190, 17);
            this.multiCoinRadio.TabIndex = 3;
            this.multiCoinRadio.Text = "Mine all of the most profitable coins";
            this.multiCoinRadio.UseVisualStyleBackColor = true;
            this.multiCoinRadio.CheckedChanged += new System.EventHandler(this.multiCoinRadio_CheckedChanged);
            // 
            // minCoinCombo
            // 
            this.minCoinCombo.FormattingEnabled = true;
            this.minCoinCombo.Location = new System.Drawing.Point(198, 25);
            this.minCoinCombo.Name = "minCoinCombo";
            this.minCoinCombo.Size = new System.Drawing.Size(90, 21);
            this.minCoinCombo.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Don\'t mine coins less profitable than";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.exceptPercentLabel);
            this.groupBox1.Controls.Add(this.exceptionEdit);
            this.groupBox1.Controls.Add(this.exceptionLabel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.minPercentageEdit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minCoinCombo);
            this.groupBox1.Controls.Add(this.multiCoinRadio);
            this.groupBox1.Controls.Add(this.singleCoinRadio);
            this.groupBox1.Location = new System.Drawing.Point(12, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 172);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Strategies";
            // 
            // exceptPercentLabel
            // 
            this.exceptPercentLabel.AutoSize = true;
            this.exceptPercentLabel.Enabled = false;
            this.exceptPercentLabel.Location = new System.Drawing.Point(273, 139);
            this.exceptPercentLabel.Name = "exceptPercentLabel";
            this.exceptPercentLabel.Size = new System.Drawing.Size(15, 13);
            this.exceptPercentLabel.TabIndex = 14;
            this.exceptPercentLabel.Text = "%";
            // 
            // exceptionEdit
            // 
            this.exceptionEdit.Enabled = false;
            this.exceptionEdit.Location = new System.Drawing.Point(198, 136);
            this.exceptionEdit.Name = "exceptionEdit";
            this.exceptionEdit.Size = new System.Drawing.Size(69, 20);
            this.exceptionEdit.TabIndex = 4;
            // 
            // exceptionLabel
            // 
            this.exceptionLabel.AutoSize = true;
            this.exceptionLabel.Enabled = false;
            this.exceptionLabel.Location = new System.Drawing.Point(35, 139);
            this.exceptionLabel.Name = "exceptionLabel";
            this.exceptionLabel.Size = new System.Drawing.Size(151, 13);
            this.exceptionLabel.TabIndex = 12;
            this.exceptionLabel.Text = "Mine a single coin if it exceeds";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(273, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "%";
            // 
            // minPercentageEdit
            // 
            this.minPercentageEdit.Location = new System.Drawing.Point(198, 55);
            this.minPercentageEdit.Name = "minPercentageEdit";
            this.minPercentageEdit.Size = new System.Drawing.Size(69, 20);
            this.minPercentageEdit.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Don\'t mine coins less profitable than";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Profitability basis:";
            // 
            // proftabilityBasisCombo
            // 
            this.proftabilityBasisCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.proftabilityBasisCombo.FormattingEnabled = true;
            this.proftabilityBasisCombo.Items.AddRange(new object[] {
            "Profitability Adjusted for Stales",
            "Average Profitability Past 7 Days",
            "Straight Profitability of BTC"});
            this.proftabilityBasisCombo.Location = new System.Drawing.Point(124, 62);
            this.proftabilityBasisCombo.Name = "proftabilityBasisCombo";
            this.proftabilityBasisCombo.Size = new System.Drawing.Size(176, 21);
            this.proftabilityBasisCombo.TabIndex = 2;
            // 
            // intervalCombo
            // 
            this.intervalCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.intervalCombo.FormattingEnabled = true;
            this.intervalCombo.Items.AddRange(new object[] {
            "5 minutes",
            "15 minutes",
            "30 minutes"});
            this.intervalCombo.Location = new System.Drawing.Point(124, 35);
            this.intervalCombo.Name = "intervalCombo";
            this.intervalCombo.Size = new System.Drawing.Size(176, 21);
            this.intervalCombo.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Check every:";
            // 
            // StrategiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 311);
            this.Controls.Add(this.intervalCombo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.proftabilityBasisCombo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StrategiesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Strategies";
            this.Load += new System.EventHandler(this.StrategiesForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.strategyConfigurationBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.BindingSource strategyConfigurationBindingSource;
        private System.Windows.Forms.RadioButton singleCoinRadio;
        private System.Windows.Forms.RadioButton multiCoinRadio;
        private System.Windows.Forms.ComboBox minCoinCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox minPercentageEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label exceptPercentLabel;
        private System.Windows.Forms.TextBox exceptionEdit;
        private System.Windows.Forms.Label exceptionLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox proftabilityBasisCombo;
        private System.Windows.Forms.ComboBox intervalCombo;
        private System.Windows.Forms.Label label5;
    }
}