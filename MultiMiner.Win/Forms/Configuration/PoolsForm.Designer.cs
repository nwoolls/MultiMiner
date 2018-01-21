namespace MultiMiner.Win.Forms.Configuration
{
    partial class PoolsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PoolsForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addCoinButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.removeCoinButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyCoinButton = new System.Windows.Forms.ToolStripButton();
            this.editCoinButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importButton = new System.Windows.Forms.ToolStripSplitButton();
            this.exportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.coinListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.saveToRemotingCheckBox = new System.Windows.Forms.CheckBox();
            this.applicationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.featuresButton = new System.Windows.Forms.Button();
            this.poolFeaturesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extraNonceSubscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableCoinbaseCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userNameCombo = new System.Windows.Forms.ComboBox();
            this.miningPoolBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.poolDownButton = new System.Windows.Forms.Button();
            this.poolUpButton = new System.Windows.Forms.Button();
            this.addPoolButton = new System.Windows.Forms.Button();
            this.removePoolButton = new System.Windows.Forms.Button();
            this.poolListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hostEdit = new System.Windows.Forms.TextBox();
            this.portEdit = new System.Windows.Forms.TextBox();
            this.passwordEdit = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.coinConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.adjustProfitCombo = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationBindingSource)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.poolFeaturesMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miningPoolBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCoinButton,
            this.toolStripButton2,
            this.removeCoinButton,
            this.toolStripSeparator2,
            this.copyCoinButton,
            this.editCoinButton,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.importButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(651, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.TabStop = true;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addCoinButton
            // 
            this.addCoinButton.Image = global::MultiMiner.Win.Properties.Resources.add_button;
            this.addCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addCoinButton.Name = "addCoinButton";
            this.addCoinButton.Size = new System.Drawing.Size(77, 22);
            this.addCoinButton.Text = "Add Coin";
            this.addCoinButton.ToolTipText = "Add a coin configuration";
            this.addCoinButton.Click += new System.EventHandler(this.addCoinButton_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::MultiMiner.Win.Properties.Resources.add_button;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(104, 22);
            this.toolStripButton2.Text = "Add Multipool";
            this.toolStripButton2.ToolTipText = "Add a Multipool configuration";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // removeCoinButton
            // 
            this.removeCoinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeCoinButton.Image = global::MultiMiner.Win.Properties.Resources.delete_button_error;
            this.removeCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeCoinButton.Name = "removeCoinButton";
            this.removeCoinButton.Size = new System.Drawing.Size(23, 22);
            this.removeCoinButton.Text = "Remove";
            this.removeCoinButton.ToolTipText = "Remove a configuration";
            this.removeCoinButton.Click += new System.EventHandler(this.removeCoinButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // copyCoinButton
            // 
            this.copyCoinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyCoinButton.Image = global::MultiMiner.Win.Properties.Resources.clipboard_copy;
            this.copyCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyCoinButton.Name = "copyCoinButton";
            this.copyCoinButton.Size = new System.Drawing.Size(23, 22);
            this.copyCoinButton.Text = "Copy Configuration";
            this.copyCoinButton.Click += new System.EventHandler(this.copyCoinButton_Click);
            // 
            // editCoinButton
            // 
            this.editCoinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editCoinButton.Image = global::MultiMiner.Win.Properties.Resources.document_text_edit;
            this.editCoinButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editCoinButton.Name = "editCoinButton";
            this.editCoinButton.Size = new System.Drawing.Size(23, 22);
            this.editCoinButton.Text = "Edit Configuration";
            this.editCoinButton.Click += new System.EventHandler(this.editCoinButton_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::MultiMiner.Win.Properties.Resources.sort;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(48, 22);
            this.toolStripButton1.Text = "Sort";
            this.toolStripButton1.ToolTipText = "Alphabetize configurations";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // importButton
            // 
            this.importButton.BackColor = System.Drawing.SystemColors.Control;
            this.importButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportButton});
            this.importButton.Image = global::MultiMiner.Win.Properties.Resources.list_import;
            this.importButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 22);
            this.importButton.Text = "Import";
            this.importButton.ToolTipText = "Import coin and pool configurations";
            this.importButton.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // exportButton
            // 
            this.exportButton.Image = global::MultiMiner.Win.Properties.Resources.list_export;
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(107, 22);
            this.exportButton.Text = "Export";
            this.exportButton.ToolTipText = "Export coin and pool configurations";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // coinListBox
            // 
            this.coinListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.coinListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.coinListBox.FormattingEnabled = true;
            this.coinListBox.ItemHeight = 15;
            this.coinListBox.Location = new System.Drawing.Point(0, 25);
            this.coinListBox.Name = "coinListBox";
            this.coinListBox.Size = new System.Drawing.Size(186, 364);
            this.coinListBox.TabIndex = 1;
            this.coinListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.coinListBox_DrawItem);
            this.coinListBox.SelectedIndexChanged += new System.EventHandler(this.coinListBox_SelectedIndexChanged);
            this.coinListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.coinListBox_DragDrop);
            this.coinListBox.DragOver += new System.Windows.Forms.DragEventHandler(this.coinListBox_DragOver);
            this.coinListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.coinListBox_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.saveToRemotingCheckBox);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 389);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(651, 54);
            this.panel1.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(12, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 4;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveToRemotingCheckBox
            // 
            this.saveToRemotingCheckBox.AutoSize = true;
            this.saveToRemotingCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.applicationBindingSource, "SaveCoinsToAllMachines", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.saveToRemotingCheckBox.Location = new System.Drawing.Point(49, 19);
            this.saveToRemotingCheckBox.Name = "saveToRemotingCheckBox";
            this.saveToRemotingCheckBox.Size = new System.Drawing.Size(198, 19);
            this.saveToRemotingCheckBox.TabIndex = 2;
            this.saveToRemotingCheckBox.Text = "Apply to all rigs on your network";
            this.saveToRemotingCheckBox.UseVisualStyleBackColor = true;
            // 
            // applicationBindingSource
            // 
            this.applicationBindingSource.DataSource = typeof(MultiMiner.UX.Data.Configuration.Application);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(455, 14);
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
            this.cancelButton.Location = new System.Drawing.Point(549, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip2.Size = new System.Drawing.Size(108, 26);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.featuresButton);
            this.groupBox1.Controls.Add(this.userNameCombo);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.poolDownButton);
            this.groupBox1.Controls.Add(this.poolUpButton);
            this.groupBox1.Controls.Add(this.addPoolButton);
            this.groupBox1.Controls.Add(this.removePoolButton);
            this.groupBox1.Controls.Add(this.poolListBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.hostEdit);
            this.groupBox1.Controls.Add(this.portEdit);
            this.groupBox1.Controls.Add(this.passwordEdit);
            this.groupBox1.Location = new System.Drawing.Point(196, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 221);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pools";
            // 
            // featuresButton
            // 
            this.featuresButton.ContextMenuStrip = this.poolFeaturesMenu;
            this.featuresButton.Location = new System.Drawing.Point(346, 127);
            this.featuresButton.Name = "featuresButton";
            this.featuresButton.Size = new System.Drawing.Size(81, 24);
            this.featuresButton.TabIndex = 9;
            this.featuresButton.Text = "Features";
            this.featuresButton.UseVisualStyleBackColor = true;
            this.featuresButton.Click += new System.EventHandler(this.featuresButton_Click);
            // 
            // poolFeaturesMenu
            // 
            this.poolFeaturesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extraNonceSubscriptToolStripMenuItem,
            this.disableCoinbaseCheckToolStripMenuItem});
            this.poolFeaturesMenu.Name = "poolFeaturesMenu";
            this.poolFeaturesMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.poolFeaturesMenu.Size = new System.Drawing.Size(192, 48);
            this.poolFeaturesMenu.Opening += new System.ComponentModel.CancelEventHandler(this.poolFeaturesMenu_Opening);
            // 
            // extraNonceSubscriptToolStripMenuItem
            // 
            this.extraNonceSubscriptToolStripMenuItem.CheckOnClick = true;
            this.extraNonceSubscriptToolStripMenuItem.Name = "extraNonceSubscriptToolStripMenuItem";
            this.extraNonceSubscriptToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.extraNonceSubscriptToolStripMenuItem.Text = "Extra-nonce Subscribe";
            this.extraNonceSubscriptToolStripMenuItem.Click += new System.EventHandler(this.extraNonceSubscriptToolStripMenuItem_Click);
            // 
            // disableCoinbaseCheckToolStripMenuItem
            // 
            this.disableCoinbaseCheckToolStripMenuItem.CheckOnClick = true;
            this.disableCoinbaseCheckToolStripMenuItem.Name = "disableCoinbaseCheckToolStripMenuItem";
            this.disableCoinbaseCheckToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.disableCoinbaseCheckToolStripMenuItem.Text = "Skip Coinbase Check";
            this.disableCoinbaseCheckToolStripMenuItem.Click += new System.EventHandler(this.disableCoinbaseCheckToolStripMenuItem_Click);
            // 
            // userNameCombo
            // 
            this.userNameCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.userNameCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.userNameCombo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Username", true));
            this.userNameCombo.FormattingEnabled = true;
            this.userNameCombo.Location = new System.Drawing.Point(129, 128);
            this.userNameCombo.Name = "userNameCombo";
            this.userNameCombo.Size = new System.Drawing.Size(174, 23);
            this.userNameCombo.Sorted = true;
            this.userNameCombo.TabIndex = 7;
            this.userNameCombo.SelectedIndexChanged += new System.EventHandler(this.userNameCombo_SelectedIndexChanged);
            // 
            // miningPoolBindingSource
            // 
            this.miningPoolBindingSource.DataSource = typeof(MultiMiner.Xgminer.Data.MiningPool);
            this.miningPoolBindingSource.CurrentChanged += new System.EventHandler(this.miningPoolBindingSource_CurrentChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label10.Location = new System.Drawing.Point(352, 190);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 15);
            this.label10.TabIndex = 26;
            this.label10.Text = "optional";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 190);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 15);
            this.label9.TabIndex = 25;
            this.label9.Text = "Pool arguments:";
            // 
            // textBox4
            // 
            this.textBox4.AccessibleName = "Miner arguments";
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "MinerFlags", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(129, 187);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(217, 23);
            this.textBox4.TabIndex = 10;
            // 
            // poolDownButton
            // 
            this.poolDownButton.AccessibleName = "Down";
            this.poolDownButton.BackColor = System.Drawing.SystemColors.Control;
            this.poolDownButton.Image = global::MultiMiner.Win.Properties.Resources.arrow_down;
            this.poolDownButton.Location = new System.Drawing.Point(399, 58);
            this.poolDownButton.Name = "poolDownButton";
            this.poolDownButton.Size = new System.Drawing.Size(29, 29);
            this.poolDownButton.TabIndex = 4;
            this.poolDownButton.UseVisualStyleBackColor = false;
            this.poolDownButton.Click += new System.EventHandler(this.poolDownButton_Click);
            // 
            // poolUpButton
            // 
            this.poolUpButton.AccessibleName = "Up";
            this.poolUpButton.BackColor = System.Drawing.SystemColors.Control;
            this.poolUpButton.Image = global::MultiMiner.Win.Properties.Resources.arrow_up;
            this.poolUpButton.Location = new System.Drawing.Point(399, 22);
            this.poolUpButton.Name = "poolUpButton";
            this.poolUpButton.Size = new System.Drawing.Size(29, 29);
            this.poolUpButton.TabIndex = 3;
            this.poolUpButton.UseVisualStyleBackColor = false;
            this.poolUpButton.Click += new System.EventHandler(this.poolUpButton_Click);
            // 
            // addPoolButton
            // 
            this.addPoolButton.BackColor = System.Drawing.SystemColors.Control;
            this.addPoolButton.Image = global::MultiMiner.Win.Properties.Resources.add;
            this.addPoolButton.Location = new System.Drawing.Point(309, 22);
            this.addPoolButton.Name = "addPoolButton";
            this.addPoolButton.Size = new System.Drawing.Size(83, 29);
            this.addPoolButton.TabIndex = 1;
            this.addPoolButton.Text = "Add";
            this.addPoolButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.addPoolButton.UseVisualStyleBackColor = false;
            this.addPoolButton.Click += new System.EventHandler(this.addPoolButton_Click);
            // 
            // removePoolButton
            // 
            this.removePoolButton.BackColor = System.Drawing.SystemColors.Control;
            this.removePoolButton.Image = global::MultiMiner.Win.Properties.Resources.remove;
            this.removePoolButton.Location = new System.Drawing.Point(309, 58);
            this.removePoolButton.Name = "removePoolButton";
            this.removePoolButton.Size = new System.Drawing.Size(83, 29);
            this.removePoolButton.TabIndex = 2;
            this.removePoolButton.Text = "Remove";
            this.removePoolButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.removePoolButton.UseVisualStyleBackColor = false;
            this.removePoolButton.Click += new System.EventHandler(this.removePoolButton_Click);
            // 
            // poolListBox
            // 
            this.poolListBox.AccessibleName = "Pools";
            this.poolListBox.DataSource = this.miningPoolBindingSource;
            this.poolListBox.DisplayMember = "Host";
            this.poolListBox.FormattingEnabled = true;
            this.poolListBox.ItemHeight = 15;
            this.poolListBox.Location = new System.Drawing.Point(7, 22);
            this.poolListBox.Name = "poolListBox";
            this.poolListBox.Size = new System.Drawing.Size(296, 64);
            this.poolListBox.TabIndex = 0;
            this.poolListBox.SelectedIndexChanged += new System.EventHandler(this.poolListBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "Host:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "Worker name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 21;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 20;
            this.label1.Text = "Password:";
            // 
            // hostEdit
            // 
            this.hostEdit.AccessibleName = "Host";
            this.hostEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Host", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hostEdit.Location = new System.Drawing.Point(129, 98);
            this.hostEdit.Name = "hostEdit";
            this.hostEdit.Size = new System.Drawing.Size(174, 23);
            this.hostEdit.TabIndex = 5;
            this.hostEdit.Validating += new System.ComponentModel.CancelEventHandler(this.hostEdit_Validating);
            this.hostEdit.Validated += new System.EventHandler(this.hostEdit_Validated);
            // 
            // portEdit
            // 
            this.portEdit.AccessibleName = "Port";
            this.portEdit.Location = new System.Drawing.Point(346, 98);
            this.portEdit.Name = "portEdit";
            this.portEdit.Size = new System.Drawing.Size(81, 23);
            this.portEdit.TabIndex = 6;
            this.portEdit.Validating += new System.ComponentModel.CancelEventHandler(this.portEdit_Validating);
            this.portEdit.Validated += new System.EventHandler(this.portEdit_Validated);
            // 
            // passwordEdit
            // 
            this.passwordEdit.AccessibleName = "Password";
            this.passwordEdit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.miningPoolBindingSource, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.passwordEdit.Location = new System.Drawing.Point(129, 157);
            this.passwordEdit.Name = "passwordEdit";
            this.passwordEdit.Size = new System.Drawing.Size(174, 23);
            this.passwordEdit.TabIndex = 8;
            // 
            // textBox3
            // 
            this.textBox3.AccessibleName = "Miner arguments";
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.coinConfigurationBindingSource, "MinerFlags", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(325, 295);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(217, 23);
            this.textBox3.TabIndex = 4;
            // 
            // coinConfigurationBindingSource
            // 
            this.coinConfigurationBindingSource.DataSource = typeof(MultiMiner.Engine.Data.Configuration.Coin);
            this.coinConfigurationBindingSource.CurrentChanged += new System.EventHandler(this.coinConfigurationBindingSource_CurrentChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(204, 298);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "Coin arguments:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(204, 328);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 15);
            this.label6.TabIndex = 20;
            this.label6.Text = "Adjust profitability:";
            // 
            // textBox5
            // 
            this.textBox5.AccessibleName = "Adjust profitability";
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.coinConfigurationBindingSource, "ProfitabilityAdjustment", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox5.Location = new System.Drawing.Point(325, 325);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(74, 23);
            this.textBox5.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(404, 329);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "%";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(428, 329);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "Adjust by:";
            // 
            // adjustProfitCombo
            // 
            this.adjustProfitCombo.AccessibleName = "Adjust by";
            this.adjustProfitCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.adjustProfitCombo.FormattingEnabled = true;
            this.adjustProfitCombo.Items.AddRange(new object[] {
            "Addition",
            "Multiplication"});
            this.adjustProfitCombo.Location = new System.Drawing.Point(497, 325);
            this.adjustProfitCombo.Name = "adjustProfitCombo";
            this.adjustProfitCombo.Size = new System.Drawing.Size(126, 23);
            this.adjustProfitCombo.TabIndex = 6;
            this.adjustProfitCombo.SelectedIndexChanged += new System.EventHandler(this.adjustProfitCombo_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.coinConfigurationBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(196, 37);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(171, 19);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Coin configuration enabled";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.coinConfigurationBindingSource, "NotifyOnShareAccepted2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox8.Location = new System.Drawing.Point(432, 362);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(158, 19);
            this.checkBox8.TabIndex = 8;
            this.checkBox8.Text = "Notify on share accepted";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.coinConfigurationBindingSource, "NotifyOnBlockFound2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox7.Location = new System.Drawing.Point(207, 362);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(143, 19);
            this.checkBox7.TabIndex = 7;
            this.checkBox7.Text = "Notify on block found";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label11.Location = new System.Drawing.Point(548, 298);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 15);
            this.label11.TabIndex = 27;
            this.label11.Text = "optional";
            // 
            // PoolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(651, 443);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBox8);
            this.Controls.Add(this.checkBox7);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.adjustProfitCombo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.coinListBox);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PoolsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Pools";
            this.Load += new System.EventHandler(this.CoinsForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.applicationBindingSource)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.poolFeaturesMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.miningPoolBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
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
        private System.Windows.Forms.TextBox hostEdit;
        private System.Windows.Forms.TextBox portEdit;
        private System.Windows.Forms.TextBox passwordEdit;
        private System.Windows.Forms.BindingSource miningPoolBindingSource;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.BindingSource coinConfigurationBindingSource;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox adjustProfitCombo;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button poolDownButton;
        private System.Windows.Forms.Button poolUpButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripButton addCoinButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSplitButton importButton;
        private System.Windows.Forms.ToolStripMenuItem exportButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox saveToRemotingCheckBox;
        private System.Windows.Forms.BindingSource applicationBindingSource;
        private System.Windows.Forms.ToolStripButton copyCoinButton;
        private System.Windows.Forms.ComboBox userNameCombo;
        private System.Windows.Forms.ToolStripButton editCoinButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button featuresButton;
        private System.Windows.Forms.ContextMenuStrip poolFeaturesMenu;
        private System.Windows.Forms.ToolStripMenuItem extraNonceSubscriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableCoinbaseCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}