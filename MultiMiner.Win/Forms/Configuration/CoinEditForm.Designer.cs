namespace MultiMiner.Win.Forms.Configuration
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.symbolEdit = new System.Windows.Forms.TextBox();
            this.algorithmCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cryptoCoinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cryptoCoinBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 115);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(272, 45);
            this.panel1.TabIndex = 3;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(80, 9);
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
            this.cancelButton.Location = new System.Drawing.Point(175, 9);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Coin name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Exchange symbol:";
            // 
            // textBox1
            // 
            this.textBox1.AccessibleName = "Coin name";
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(129, 16);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(125, 23);
            this.textBox1.TabIndex = 0;
            // 
            // symbolEdit
            // 
            this.symbolEdit.AccessibleName = "Exchange symbol";
            this.symbolEdit.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.symbolEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.cryptoCoinBindingSource, "Id", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.symbolEdit.Location = new System.Drawing.Point(129, 46);
            this.symbolEdit.MaxLength = 5;
            this.symbolEdit.Name = "symbolEdit";
            this.symbolEdit.Size = new System.Drawing.Size(125, 23);
            this.symbolEdit.TabIndex = 1;
            // 
            // algorithmCombo
            // 
            this.algorithmCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmCombo.FormattingEnabled = true;
            this.algorithmCombo.Location = new System.Drawing.Point(129, 75);
            this.algorithmCombo.Name = "algorithmCombo";
            this.algorithmCombo.Size = new System.Drawing.Size(125, 23);
            this.algorithmCombo.Sorted = true;
            this.algorithmCombo.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Algorithm:";
            // 
            // cryptoCoinBindingSource
            // 
            this.cryptoCoinBindingSource.DataSource = typeof(MultiMiner.Engine.Data.PoolGroup);
            // 
            // CoinEditForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(272, 160);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.algorithmCombo);
            this.Controls.Add(this.symbolEdit);
            this.Controls.Add(this.textBox1);
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox symbolEdit;
        private System.Windows.Forms.BindingSource cryptoCoinBindingSource;
        private System.Windows.Forms.ComboBox algorithmCombo;
        private System.Windows.Forms.Label label3;
    }
}