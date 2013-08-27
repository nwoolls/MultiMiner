namespace MultiMiner.Win
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.multiMinerLabel = new System.Windows.Forms.Label();
            this.bfgminerLabel = new System.Windows.Forms.Label();
            this.cgminerLabel = new System.Windows.Forms.Label();
            this.multiMinerLink = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // multiMinerLabel
            // 
            this.multiMinerLabel.AutoSize = true;
            this.multiMinerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.multiMinerLabel.Location = new System.Drawing.Point(97, 12);
            this.multiMinerLabel.Name = "multiMinerLabel";
            this.multiMinerLabel.Size = new System.Drawing.Size(114, 16);
            this.multiMinerLabel.TabIndex = 1;
            this.multiMinerLabel.Text = "MultiMiner 1.1.1";
            // 
            // bfgminerLabel
            // 
            this.bfgminerLabel.AutoSize = true;
            this.bfgminerLabel.Location = new System.Drawing.Point(97, 63);
            this.bfgminerLabel.Name = "bfgminerLabel";
            this.bfgminerLabel.Size = new System.Drawing.Size(111, 13);
            this.bfgminerLabel.TabIndex = 3;
            this.bfgminerLabel.Text = "xgminer 1.1.1 installed";
            // 
            // cgminerLabel
            // 
            this.cgminerLabel.AutoSize = true;
            this.cgminerLabel.Location = new System.Drawing.Point(97, 50);
            this.cgminerLabel.Name = "cgminerLabel";
            this.cgminerLabel.Size = new System.Drawing.Size(111, 13);
            this.cgminerLabel.TabIndex = 4;
            this.cgminerLabel.Text = "xgminer 1.1.1 installed";
            // 
            // multiMinerLink
            // 
            this.multiMinerLink.AutoSize = true;
            this.multiMinerLink.Location = new System.Drawing.Point(97, 28);
            this.multiMinerLink.Name = "multiMinerLink";
            this.multiMinerLink.Size = new System.Drawing.Size(94, 13);
            this.multiMinerLink.TabIndex = 5;
            this.multiMinerLink.TabStop = true;
            this.multiMinerLink.Text = "multiminerapp.com";
            this.multiMinerLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.multiMinerLink_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 338);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(469, 47);
            this.panel1.TabIndex = 6;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(382, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 82);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(443, 254);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MultiMiner.Win.Properties.Resources.window_sel;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 385);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.multiMinerLink);
            this.Controls.Add(this.cgminerLabel);
            this.Controls.Add(this.bfgminerLabel);
            this.Controls.Add(this.multiMinerLabel);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About MultiMiner";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label multiMinerLabel;
        private System.Windows.Forms.Label bfgminerLabel;
        private System.Windows.Forms.Label cgminerLabel;
        private System.Windows.Forms.LinkLabel multiMinerLink;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox textBox1;
    }
}