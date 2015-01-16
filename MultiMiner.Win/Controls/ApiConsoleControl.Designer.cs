namespace MultiMiner.Win.Forms
{
    partial class ApiConsoleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiConsoleControl));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.helpTextBox = new Controls.ReadOnlyTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.outputTextBox = new Controls.ReadOnlyTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.minerComboBox = new System.Windows.Forms.ComboBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(612, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 401);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // helpTextBox
            // 
            this.helpTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.helpTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.helpTextBox.HideSelection = false;
            this.helpTextBox.Location = new System.Drawing.Point(615, 0);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.helpTextBox.Size = new System.Drawing.Size(343, 401);
            this.helpTextBox.TabIndex = 0;
            this.helpTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.helpTextBox_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.outputTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.minerComboBox);
            this.panel1.Controls.Add(this.inputTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(612, 401);
            this.panel1.TabIndex = 9;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 10F);
            this.outputTextBox.HideSelection = false;
            this.outputTextBox.Location = new System.Drawing.Point(12, 45);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(587, 312);
            this.outputTextBox.TabIndex = 2;
            this.outputTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.outputTextBox_MouseUp);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 368);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Input:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Miner:";
            // 
            // minerComboBox
            // 
            this.minerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.minerComboBox.FormattingEnabled = true;
            this.minerComboBox.Location = new System.Drawing.Point(61, 14);
            this.minerComboBox.Name = "minerComboBox";
            this.minerComboBox.Size = new System.Drawing.Size(233, 23);
            this.minerComboBox.TabIndex = 1;
            this.minerComboBox.SelectedIndexChanged += new System.EventHandler(this.minerComboBox_SelectedIndexChanged);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.Location = new System.Drawing.Point(61, 365);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(538, 23);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(572, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 10;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ApiConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(958, 401);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.helpTextBox);
            this.Name = "ApiConsoleControl";
            this.Text = "API Console";
            this.Load += new System.EventHandler(this.ApiConsoleForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox minerComboBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private Controls.ReadOnlyTextBox helpTextBox;
        private Controls.ReadOnlyTextBox outputTextBox;
        private System.Windows.Forms.Button button1;
    }
}