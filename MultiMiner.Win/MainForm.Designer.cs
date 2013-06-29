namespace MultiMiner.Win
{
    partial class MainForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.kindDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.platformDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.kindDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.vendorDataGridViewTextBoxColumn,
            this.platformDataGridViewTextBoxColumn,
            this.versionDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.deviceBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(12, 51);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(718, 292);
            this.dataGridView1.TabIndex = 0;
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MultiMiner.Xgminer.Device);
            // 
            // kindDataGridViewTextBoxColumn
            // 
            this.kindDataGridViewTextBoxColumn.DataPropertyName = "Kind";
            this.kindDataGridViewTextBoxColumn.HeaderText = "Kind";
            this.kindDataGridViewTextBoxColumn.Name = "kindDataGridViewTextBoxColumn";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // vendorDataGridViewTextBoxColumn
            // 
            this.vendorDataGridViewTextBoxColumn.DataPropertyName = "Vendor";
            this.vendorDataGridViewTextBoxColumn.HeaderText = "Vendor";
            this.vendorDataGridViewTextBoxColumn.Name = "vendorDataGridViewTextBoxColumn";
            // 
            // platformDataGridViewTextBoxColumn
            // 
            this.platformDataGridViewTextBoxColumn.DataPropertyName = "Platform";
            this.platformDataGridViewTextBoxColumn.HeaderText = "Platform";
            this.platformDataGridViewTextBoxColumn.Name = "platformDataGridViewTextBoxColumn";
            // 
            // versionDataGridViewTextBoxColumn
            // 
            this.versionDataGridViewTextBoxColumn.DataPropertyName = "Version";
            this.versionDataGridViewTextBoxColumn.HeaderText = "Version";
            this.versionDataGridViewTextBoxColumn.Name = "versionDataGridViewTextBoxColumn";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 434);
            this.Controls.Add(this.dataGridView1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn kindDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn platformDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn versionDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource deviceBindingSource;
    }
}

