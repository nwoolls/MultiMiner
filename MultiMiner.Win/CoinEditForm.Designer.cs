namespace MultiMiner.Win
{
    partial class CoinEditForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.scryptRadioButton = new System.Windows.Forms.RadioButton();
            this.sha256RadioButton = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cryptoCoinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.symbolEdit = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 144);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(233, 39);
            this.panel1.TabIndex = 3;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(69, 8);
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
            this.cancelButton.Location = new System.Drawing.Point(150, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Coin name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Exchange symbol:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.scryptRadioButton);
            this.groupBox1.Controls.Add(this.sha256RadioButton);
            this.groupBox1.Location = new System.Drawing.Point(15, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 56);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Algorithm";
            // 
            // scryptRadioButton
            // 
            this.scryptRadioButton.AutoSize = true;
            this.scryptRadioButton.Location = new System.Drawing.Point(104, 23);
            this.scryptRadioButton.Name = "scryptRadioButton";
            this.scryptRadioButton.Size = new System.Drawing.Size(55, 17);
            this.scryptRadioButton.TabIndex = 1;
            this.scryptRadioButton.TabStop = true;
            this.scryptRadioButton.Text = "Scrypt";
            this.scryptRadioButton.UseVisualStyleBackColor = true;
            // 
            // sha256RadioButton
            // 
            this.sha256RadioButton.AutoSize = true;
            this.sha256RadioButton.Location = new System.Drawing.Point(14, 23);
            this.sha256RadioButton.Name = "sha256RadioButton";
            this.sha256RadioButton.Size = new System.Drawing.Size(68, 17);
            this.sha256RadioButton.TabIndex = 0;
            this.sha256RadioButton.TabStop = true;
            this.sha256RadioButton.Text = "SHA-256";
            this.sha256RadioButton.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(111, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(108, 20);
            this.textBox1.TabIndex = 0;
            // 
            // cryptoCoinBindingSource
            // 
            this.cryptoCoinBindingSource.DataSource = typeof(MultiMiner.Engine.CryptoCoin);
            // 
            // symbolEdit
            // 
            this.symbolEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Symbol", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.symbolEdit.Location = new System.Drawing.Point(111, 40);
            this.symbolEdit.Name = "symbolEdit";
            this.symbolEdit.Size = new System.Drawing.Size(108, 20);
            this.symbolEdit.TabIndex = 1;
            // 
            // CoinEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(233, 183);
            this.Controls.Add(this.symbolEdit);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoinEditForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Coin";
            this.Load += new System.EventHandler(this.CoinEditForm_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton scryptRadioButton;
        private System.Windows.Forms.RadioButton sha256RadioButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox symbolEdit;
        private System.Windows.Forms.BindingSource cryptoCoinBindingSource;
    }
}