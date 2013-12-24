namespace MultiMiner.Win
{
    partial class DetailsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.closeDetailsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // closeDetailsButton
            // 
            this.closeDetailsButton.AccessibleName = "Close";
            this.closeDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeDetailsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeDetailsButton.Location = new System.Drawing.Point(301, 3);
            this.closeDetailsButton.Name = "closeDetailsButton";
            this.closeDetailsButton.Size = new System.Drawing.Size(22, 22);
            this.closeDetailsButton.TabIndex = 1;
            this.closeDetailsButton.Text = "✖";
            this.closeDetailsButton.UseVisualStyleBackColor = true;
            this.closeDetailsButton.Click += new System.EventHandler(this.closeApiButton_Click);
            // 
            // DetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.closeDetailsButton);
            this.Name = "DetailsControl";
            this.Size = new System.Drawing.Size(326, 396);
            this.Load += new System.EventHandler(this.DetailsControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeDetailsButton;
    }
}
