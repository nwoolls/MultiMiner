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
            this.thresholdSymbolCombo = new System.Windows.Forms.ComboBox();
            this.thresholdSymbolLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exceptionEdit = new System.Windows.Forms.TextBox();
            this.mineSingleOverrideLabel = new System.Windows.Forms.Label();
            this.thresholdValueEdit = new System.Windows.Forms.TextBox();
            this.thresholdValueLabel = new System.Windows.Forms.Label();
            this.profitabilityKindLabel = new System.Windows.Forms.Label();
            this.profitabilityKindCombo = new System.Windows.Forms.ComboBox();
            this.intervalCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.baseCoinCombo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.miningBasisCombo = new System.Windows.Forms.ComboBox();
            this.applicationConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.suggestionsCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.strategyConfigurationBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 387);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 54);
            this.panel1.TabIndex = 7;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(218, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(313, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.strategyConfigurationBindingSource, "AutomaticallyMineCoins", true));
            this.checkBox1.Location = new System.Drawing.Point(14, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(220, 19);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Automatically mine coin(s) based on";
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
            this.singleCoinRadio.Location = new System.Drawing.Point(22, 99);
            this.singleCoinRadio.Name = "singleCoinRadio";
            this.singleCoinRadio.Size = new System.Drawing.Size(242, 19);
            this.singleCoinRadio.TabIndex = 2;
            this.singleCoinRadio.TabStop = true;
            this.singleCoinRadio.Text = "Mine only the single most profitable coin";
            this.singleCoinRadio.UseVisualStyleBackColor = true;
            // 
            // multiCoinRadio
            // 
            this.multiCoinRadio.AutoSize = true;
            this.multiCoinRadio.Location = new System.Drawing.Point(22, 130);
            this.multiCoinRadio.Name = "multiCoinRadio";
            this.multiCoinRadio.Size = new System.Drawing.Size(216, 19);
            this.multiCoinRadio.TabIndex = 3;
            this.multiCoinRadio.Text = "Mine all of the most profitable coins";
            this.multiCoinRadio.UseVisualStyleBackColor = true;
            this.multiCoinRadio.CheckedChanged += new System.EventHandler(this.multiCoinRadio_CheckedChanged);
            // 
            // thresholdSymbolCombo
            // 
            this.thresholdSymbolCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.thresholdSymbolCombo.FormattingEnabled = true;
            this.thresholdSymbolCombo.Location = new System.Drawing.Point(236, 29);
            this.thresholdSymbolCombo.Name = "thresholdSymbolCombo";
            this.thresholdSymbolCombo.Size = new System.Drawing.Size(129, 23);
            this.thresholdSymbolCombo.TabIndex = 0;
            // 
            // thresholdSymbolLabel
            // 
            this.thresholdSymbolLabel.AutoSize = true;
            this.thresholdSymbolLabel.Location = new System.Drawing.Point(19, 32);
            this.thresholdSymbolLabel.Name = "thresholdSymbolLabel";
            this.thresholdSymbolLabel.Size = new System.Drawing.Size(200, 15);
            this.thresholdSymbolLabel.TabIndex = 8;
            this.thresholdSymbolLabel.Text = "Don\'t mine coins less profitable than";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.exceptionEdit);
            this.groupBox1.Controls.Add(this.mineSingleOverrideLabel);
            this.groupBox1.Controls.Add(this.thresholdValueEdit);
            this.groupBox1.Controls.Add(this.thresholdValueLabel);
            this.groupBox1.Controls.Add(this.thresholdSymbolLabel);
            this.groupBox1.Controls.Add(this.thresholdSymbolCombo);
            this.groupBox1.Controls.Add(this.multiCoinRadio);
            this.groupBox1.Controls.Add(this.singleCoinRadio);
            this.groupBox1.Location = new System.Drawing.Point(14, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 198);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Strategies (optional)";
            // 
            // exceptionEdit
            // 
            this.exceptionEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionEdit.Enabled = false;
            this.exceptionEdit.Location = new System.Drawing.Point(236, 157);
            this.exceptionEdit.Name = "exceptionEdit";
            this.exceptionEdit.Size = new System.Drawing.Size(129, 23);
            this.exceptionEdit.TabIndex = 5;
            // 
            // mineSingleOverrideLabel
            // 
            this.mineSingleOverrideLabel.AutoSize = true;
            this.mineSingleOverrideLabel.Enabled = false;
            this.mineSingleOverrideLabel.Location = new System.Drawing.Point(41, 160);
            this.mineSingleOverrideLabel.Name = "mineSingleOverrideLabel";
            this.mineSingleOverrideLabel.Size = new System.Drawing.Size(167, 15);
            this.mineSingleOverrideLabel.TabIndex = 4;
            this.mineSingleOverrideLabel.Text = "Mine a single coin if it exceeds";
            // 
            // thresholdValueEdit
            // 
            this.thresholdValueEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.thresholdValueEdit.Location = new System.Drawing.Point(236, 63);
            this.thresholdValueEdit.Name = "thresholdValueEdit";
            this.thresholdValueEdit.Size = new System.Drawing.Size(129, 23);
            this.thresholdValueEdit.TabIndex = 1;
            // 
            // thresholdValueLabel
            // 
            this.thresholdValueLabel.AutoSize = true;
            this.thresholdValueLabel.Location = new System.Drawing.Point(19, 67);
            this.thresholdValueLabel.Name = "thresholdValueLabel";
            this.thresholdValueLabel.Size = new System.Drawing.Size(200, 15);
            this.thresholdValueLabel.TabIndex = 9;
            this.thresholdValueLabel.Text = "Don\'t mine coins less profitable than";
            // 
            // profitabilityKindLabel
            // 
            this.profitabilityKindLabel.AutoSize = true;
            this.profitabilityKindLabel.Location = new System.Drawing.Point(33, 46);
            this.profitabilityKindLabel.Name = "profitabilityKindLabel";
            this.profitabilityKindLabel.Size = new System.Drawing.Size(97, 15);
            this.profitabilityKindLabel.TabIndex = 3;
            this.profitabilityKindLabel.Text = "Profitability kind:";
            // 
            // profitabilityKindCombo
            // 
            this.profitabilityKindCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.profitabilityKindCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profitabilityKindCombo.FormattingEnabled = true;
            this.profitabilityKindCombo.Items.AddRange(new object[] {
            "Profitability Adjusted for Stales",
            "Average Profitability Past 7 Days",
            "Straight Profitability"});
            this.profitabilityKindCombo.Location = new System.Drawing.Point(145, 43);
            this.profitabilityKindCombo.Name = "profitabilityKindCombo";
            this.profitabilityKindCombo.Size = new System.Drawing.Size(234, 23);
            this.profitabilityKindCombo.TabIndex = 2;
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
            "2 hours",
            "3 hours",
            "6 hours",
            "12 hours"});
            this.intervalCombo.Location = new System.Drawing.Point(145, 74);
            this.intervalCombo.Name = "intervalCombo";
            this.intervalCombo.Size = new System.Drawing.Size(234, 23);
            this.intervalCombo.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Check every:";
            // 
            // baseCoinCombo
            // 
            this.baseCoinCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.baseCoinCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baseCoinCombo.FormattingEnabled = true;
            this.baseCoinCombo.Items.AddRange(new object[] {
            "Bitcoin",
            "Litecoin"});
            this.baseCoinCombo.Location = new System.Drawing.Point(145, 105);
            this.baseCoinCombo.Name = "baseCoinCombo";
            this.baseCoinCombo.Size = new System.Drawing.Size(234, 23);
            this.baseCoinCombo.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "Base coin:";
            // 
            // miningBasisCombo
            // 
            this.miningBasisCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.miningBasisCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.miningBasisCombo.FormattingEnabled = true;
            this.miningBasisCombo.Items.AddRange(new object[] {
            "Profitability",
            "Difficulty",
            "Price"});
            this.miningBasisCombo.Location = new System.Drawing.Point(245, 12);
            this.miningBasisCombo.Name = "miningBasisCombo";
            this.miningBasisCombo.Size = new System.Drawing.Size(133, 23);
            this.miningBasisCombo.TabIndex = 1;
            this.miningBasisCombo.SelectedIndexChanged += new System.EventHandler(this.miningBasisCombo_SelectedIndexChanged);
            // 
            // applicationConfigurationBindingSource
            // 
            this.applicationConfigurationBindingSource.DataSource = typeof(MultiMiner.Win.ApplicationConfiguration);
            // 
            // suggestionsCombo
            // 
            this.suggestionsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.suggestionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.suggestionsCombo.FormattingEnabled = true;
            this.suggestionsCombo.Items.AddRange(new object[] {
            "None",
            "SHA-256",
            "Scrypt",
            "Both"});
            this.suggestionsCombo.Location = new System.Drawing.Point(145, 136);
            this.suggestionsCombo.Name = "suggestionsCombo";
            this.suggestionsCombo.Size = new System.Drawing.Size(234, 23);
            this.suggestionsCombo.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Suggest coins:";
            // 
            // StrategiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(414, 441);
            this.Controls.Add(this.suggestionsCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.miningBasisCombo);
            this.Controls.Add(this.baseCoinCombo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.intervalCombo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.profitabilityKindCombo);
            this.Controls.Add(this.profitabilityKindLabel);
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
            ((System.ComponentModel.ISupportInitialize)(this.applicationConfigurationBindingSource)).EndInit();
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
        private System.Windows.Forms.ComboBox thresholdSymbolCombo;
        private System.Windows.Forms.Label thresholdSymbolLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox thresholdValueEdit;
        private System.Windows.Forms.Label thresholdValueLabel;
        private System.Windows.Forms.TextBox exceptionEdit;
        private System.Windows.Forms.Label mineSingleOverrideLabel;
        private System.Windows.Forms.Label profitabilityKindLabel;
        private System.Windows.Forms.ComboBox profitabilityKindCombo;
        private System.Windows.Forms.ComboBox intervalCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox baseCoinCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox miningBasisCombo;
        private System.Windows.Forms.BindingSource applicationConfigurationBindingSource;
        private System.Windows.Forms.ComboBox suggestionsCombo;
        private System.Windows.Forms.Label label1;
    }
}