namespace MultiMiner.Win.Forms.Configuration
{
    partial class GPUMinerSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GPUMinerSettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.xgminerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.disableGpuCheckbox = new System.Windows.Forms.CheckBox();
            this.minerCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.kernelArgsEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.algoListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.autoDesktopCheckBox = new System.Windows.Forms.CheckBox();
            this.applicationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 295);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(337, 54);
            this.panel1.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(12, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(141, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(235, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "TerminateGpuMiners", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox2.Location = new System.Drawing.Point(12, 265);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(144, 19);
            this.checkBox2.TabIndex = 7;
            this.checkBox2.Text = "Terminate GPU miners";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // xgminerConfigurationBindingSource
            // 
            this.xgminerConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Data.Configuration.Xgminer);
            // 
            // disableGpuCheckbox
            // 
            this.disableGpuCheckbox.AutoSize = true;
            this.disableGpuCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.xgminerConfigurationBindingSource, "DisableGpu", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.disableGpuCheckbox.Location = new System.Drawing.Point(12, 238);
            this.disableGpuCheckbox.Name = "disableGpuCheckbox";
            this.disableGpuCheckbox.Size = new System.Drawing.Size(131, 19);
            this.disableGpuCheckbox.TabIndex = 6;
            this.disableGpuCheckbox.Text = "Disable GPU mining";
            this.disableGpuCheckbox.UseVisualStyleBackColor = true;
            this.disableGpuCheckbox.CheckedChanged += new System.EventHandler(this.disableGpuCheckbox_CheckedChanged);
            // 
            // minerCombo
            // 
            this.minerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.minerCombo.FormattingEnabled = true;
            this.minerCombo.Location = new System.Drawing.Point(126, 136);
            this.minerCombo.Name = "minerCombo";
            this.minerCombo.Size = new System.Drawing.Size(197, 23);
            this.minerCombo.TabIndex = 3;
            this.minerCombo.SelectedIndexChanged += new System.EventHandler(this.minerCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Miner:";
            // 
            // kernelArgsEdit
            // 
            this.kernelArgsEdit.Location = new System.Drawing.Point(126, 169);
            this.kernelArgsEdit.Name = "kernelArgsEdit";
            this.kernelArgsEdit.Size = new System.Drawing.Size(197, 23);
            this.kernelArgsEdit.TabIndex = 4;
            this.kernelArgsEdit.Validated += new System.EventHandler(this.kernelArgsEdit_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Kernel arguments:";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(248, 28);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(248, 58);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // algoListView
            // 
            this.algoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.algoListView.FullRowSelect = true;
            this.algoListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.algoListView.HideSelection = false;
            this.algoListView.LabelEdit = true;
            this.algoListView.Location = new System.Drawing.Point(12, 28);
            this.algoListView.MultiSelect = false;
            this.algoListView.Name = "algoListView";
            this.algoListView.Size = new System.Drawing.Size(230, 97);
            this.algoListView.SmallImageList = this.imageList1;
            this.algoListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.algoListView.TabIndex = 0;
            this.algoListView.UseCompatibleStateImageBehavior = false;
            this.algoListView.View = System.Windows.Forms.View.List;
            this.algoListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.algoListView_AfterLabelEdit);
            this.algoListView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.algoListView_BeforeLabelEdit);
            this.algoListView.SelectedIndexChanged += new System.EventHandler(this.algoListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 130;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "document_calculus.png");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "Algorithms:";
            // 
            // autoDesktopCheckBox
            // 
            this.autoDesktopCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoDesktopCheckBox.AutoSize = true;
            this.autoDesktopCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationBindingSource, "AutoSetDesktopMode", true));
            this.autoDesktopCheckBox.Location = new System.Drawing.Point(12, 209);
            this.autoDesktopCheckBox.Name = "autoDesktopCheckBox";
            this.autoDesktopCheckBox.Size = new System.Drawing.Size(267, 19);
            this.autoDesktopCheckBox.TabIndex = 5;
            this.autoDesktopCheckBox.Text = "Set Dynamic Intensity based on computer use";
            this.autoDesktopCheckBox.UseVisualStyleBackColor = true;
            // 
            // applicationBindingSource
            // 
            this.applicationBindingSource.DataSource = typeof(MultiMiner.UX.Data.Configuration.Application);
            // 
            // GPUMinerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(337, 349);
            this.Controls.Add(this.autoDesktopCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.algoListView);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.kernelArgsEdit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.minerCombo);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.disableGpuCheckbox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GPUMinerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure GPU Miner Settings";
            this.Load += new System.EventHandler(this.GPUMinerSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xgminerConfigurationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.BindingSource xgminerConfigurationBindingSource;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox disableGpuCheckbox;
        private System.Windows.Forms.ComboBox minerCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox kernelArgsEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ListView algoListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoDesktopCheckBox;
        private System.Windows.Forms.BindingSource applicationBindingSource;
    }
}