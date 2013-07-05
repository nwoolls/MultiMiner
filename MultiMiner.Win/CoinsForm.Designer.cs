namespace MultiMiner.Win
{
    partial class CoinsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoinsForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addCoinButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.removeCoinButton = new System.Windows.Forms.ToolStripButton();
            this.coinListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addPoolButton = new System.Windows.Forms.Button();
            this.removePoolButton = new System.Windows.Forms.Button();
            this.poolListBox = new System.Windows.Forms.ListBox();
            this.miningPoolBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.hostEdit = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.coinConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miningPoolBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCoinButton,
            this.removeCoinButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(558, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addCoinButton
            // 
            this.addCoinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addCoinButton.Image = ((System.Drawing.Image)(resources.GetObject("addCoinButton.Image")));
            this.addCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addCoinButton.Name = "addCoinButton";
            this.addCoinButton.Size = new System.Drawing.Size(70, 22);
            this.addCoinButton.Text = "Add Coin";
            // 
            // removeCoinButton
            // 
            this.removeCoinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeCoinButton.Image = ((System.Drawing.Image)(resources.GetObject("removeCoinButton.Image")));
            this.removeCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeCoinButton.Name = "removeCoinButton";
            this.removeCoinButton.Size = new System.Drawing.Size(82, 22);
            this.removeCoinButton.Text = "Remove Coin";
            this.removeCoinButton.Click += new System.EventHandler(this.removeCoinButton_Click);
            // 
            // coinListBox
            // 
            this.coinListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.coinListBox.FormattingEnabled = true;
            this.coinListBox.Location = new System.Drawing.Point(0, 25);
            this.coinListBox.Name = "coinListBox";
            this.coinListBox.Size = new System.Drawing.Size(160, 239);
            this.coinListBox.TabIndex = 3;
            this.coinListBox.SelectedIndexChanged += new System.EventHandler(this.coinListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 264);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(558, 47);
            this.panel1.TabIndex = 4;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(390, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(471, 12);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addPoolButton);
            this.groupBox1.Controls.Add(this.removePoolButton);
            this.groupBox1.Controls.Add(this.poolListBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.hostEdit);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(166, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 182);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pools";
            // 
            // addPoolButton
            // 
            this.addPoolButton.Location = new System.Drawing.Point(265, 19);
            this.addPoolButton.Name = "addPoolButton";
            this.addPoolButton.Size = new System.Drawing.Size(100, 23);
            this.addPoolButton.TabIndex = 1;
            this.addPoolButton.Text = "Add Pool";
            this.addPoolButton.UseVisualStyleBackColor = true;
            this.addPoolButton.Click += new System.EventHandler(this.addPoolButton_Click);
            // 
            // removePoolButton
            // 
            this.removePoolButton.Location = new System.Drawing.Point(265, 52);
            this.removePoolButton.Name = "removePoolButton";
            this.removePoolButton.Size = new System.Drawing.Size(100, 23);
            this.removePoolButton.TabIndex = 2;
            this.removePoolButton.Text = "Remove Pool";
            this.removePoolButton.UseVisualStyleBackColor = true;
            this.removePoolButton.Click += new System.EventHandler(this.removePoolButton_Click);
            // 
            // poolListBox
            // 
            this.poolListBox.DataSource = this.miningPoolBindingSource;
            this.poolListBox.DisplayMember = "Host";
            this.poolListBox.FormattingEnabled = true;
            this.poolListBox.Location = new System.Drawing.Point(6, 19);
            this.poolListBox.Name = "poolListBox";
            this.poolListBox.Size = new System.Drawing.Size(253, 56);
            this.poolListBox.TabIndex = 0;
            this.poolListBox.SelectedIndexChanged += new System.EventHandler(this.poolListBox_SelectedIndexChanged);
            // 
            // miningPoolBindingSource
            // 
            this.miningPoolBindingSource.DataSource = typeof(MultiMiner.Xgminer.MiningPool);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Host:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(262, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Password:";
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(71, 121);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(188, 20);
            this.textBox4.TabIndex = 5;
            // 
            // hostEdit
            // 
            this.hostEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Host", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hostEdit.Location = new System.Drawing.Point(71, 95);
            this.hostEdit.Name = "hostEdit";
            this.hostEdit.Size = new System.Drawing.Size(188, 20);
            this.hostEdit.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Port", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(297, 95);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(68, 20);
            this.textBox2.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(71, 147);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(188, 20);
            this.textBox1.TabIndex = 6;
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.coinConfigurationBindingSource, "MinerFlags", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(270, 223);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(261, 20);
            this.textBox3.TabIndex = 17;
            // 
            // coinConfigurationBindingSource
            // 
            this.coinConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Configuration.CoinConfiguration);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Miner parameters:";
            // 
            // CoinsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 311);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.coinListBox);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoinsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Coins";
            this.Load += new System.EventHandler(this.CoinsForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miningPoolBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton addCoinButton;
        private System.Windows.Forms.ListBox coinListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStripButton removeCoinButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addPoolButton;
        private System.Windows.Forms.Button removePoolButton;
        private System.Windows.Forms.ListBox poolListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox hostEdit;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.BindingSource miningPoolBindingSource;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.BindingSource coinConfigurationBindingSource;
    }
}