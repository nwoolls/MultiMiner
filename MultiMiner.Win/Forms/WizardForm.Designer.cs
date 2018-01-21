namespace MultiMiner.Win.Forms
{
    partial class WizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.nextButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.wizardTabControl = new System.Windows.Forms.TabControl();
            this.chooseMinerPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.downloadingMinerPage = new System.Windows.Forms.TabPage();
            this.downloadingMinerLabel = new System.Windows.Forms.Label();
            this.chooseCoinPage = new System.Windows.Forms.TabPage();
            this.coinComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.configurePoolPage = new System.Windows.Forms.TabPage();
            this.featuresButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.poolsLink = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.usernameEdit = new System.Windows.Forms.TextBox();
            this.hostEdit = new System.Windows.Forms.TextBox();
            this.portEdit = new System.Windows.Forms.TextBox();
            this.passwordEdit = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.configurePerksPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.remotingPasswordEdit = new System.Windows.Forms.TextBox();
            this.remotingCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.incomeCheckBox = new System.Windows.Forms.CheckBox();
            this.exchangeApiCheckbox = new System.Windows.Forms.CheckBox();
            this.perksCheckBox = new System.Windows.Forms.CheckBox();
            this.smileyPicture = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.whatNextPage = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.poolFeaturesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extraNonceSubscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableCoinbaseCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wizardTabControl.SuspendLayout();
            this.chooseMinerPage.SuspendLayout();
            this.downloadingMinerPage.SuspendLayout();
            this.chooseCoinPage.SuspendLayout();
            this.configurePoolPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.configurePerksPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smileyPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.whatNextPage.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.poolFeaturesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Location = new System.Drawing.Point(382, 7);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(87, 27);
            this.nextButton.TabIndex = 1;
            this.nextButton.Text = "Next >";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(43, 7);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(87, 27);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // wizardTabControl
            // 
            this.wizardTabControl.Controls.Add(this.chooseMinerPage);
            this.wizardTabControl.Controls.Add(this.downloadingMinerPage);
            this.wizardTabControl.Controls.Add(this.chooseCoinPage);
            this.wizardTabControl.Controls.Add(this.configurePoolPage);
            this.wizardTabControl.Controls.Add(this.configurePerksPage);
            this.wizardTabControl.Controls.Add(this.whatNextPage);
            this.wizardTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardTabControl.Location = new System.Drawing.Point(0, 0);
            this.wizardTabControl.Name = "wizardTabControl";
            this.wizardTabControl.SelectedIndex = 0;
            this.wizardTabControl.Size = new System.Drawing.Size(484, 259);
            this.wizardTabControl.TabIndex = 0;
            this.wizardTabControl.SelectedIndexChanged += new System.EventHandler(this.wizardTabControl_SelectedIndexChanged);
            // 
            // chooseMinerPage
            // 
            this.chooseMinerPage.Controls.Add(this.label1);
            this.chooseMinerPage.Location = new System.Drawing.Point(4, 24);
            this.chooseMinerPage.Name = "chooseMinerPage";
            this.chooseMinerPage.Padding = new System.Windows.Forms.Padding(3);
            this.chooseMinerPage.Size = new System.Drawing.Size(476, 231);
            this.chooseMinerPage.TabIndex = 0;
            this.chooseMinerPage.Text = "Choose Miner";
            this.chooseMinerPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.label1.Size = new System.Drawing.Size(470, 229);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // downloadingMinerPage
            // 
            this.downloadingMinerPage.Controls.Add(this.downloadingMinerLabel);
            this.downloadingMinerPage.Location = new System.Drawing.Point(4, 24);
            this.downloadingMinerPage.Name = "downloadingMinerPage";
            this.downloadingMinerPage.Padding = new System.Windows.Forms.Padding(3);
            this.downloadingMinerPage.Size = new System.Drawing.Size(476, 231);
            this.downloadingMinerPage.TabIndex = 1;
            this.downloadingMinerPage.Text = "Downloading Miner";
            this.downloadingMinerPage.UseVisualStyleBackColor = true;
            // 
            // downloadingMinerLabel
            // 
            this.downloadingMinerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.downloadingMinerLabel.Location = new System.Drawing.Point(3, 3);
            this.downloadingMinerLabel.Name = "downloadingMinerLabel";
            this.downloadingMinerLabel.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.downloadingMinerLabel.Size = new System.Drawing.Size(470, 142);
            this.downloadingMinerLabel.TabIndex = 1;
            this.downloadingMinerLabel.Text = "Please wait while cgminer is downloaded from adsf.com and installed into the fold" +
    "er c:\\whatever";
            // 
            // chooseCoinPage
            // 
            this.chooseCoinPage.Controls.Add(this.coinComboBox);
            this.chooseCoinPage.Controls.Add(this.label5);
            this.chooseCoinPage.Controls.Add(this.label4);
            this.chooseCoinPage.Location = new System.Drawing.Point(4, 24);
            this.chooseCoinPage.Name = "chooseCoinPage";
            this.chooseCoinPage.Padding = new System.Windows.Forms.Padding(3);
            this.chooseCoinPage.Size = new System.Drawing.Size(476, 231);
            this.chooseCoinPage.TabIndex = 2;
            this.chooseCoinPage.Text = "Choose Coin";
            this.chooseCoinPage.UseVisualStyleBackColor = true;
            // 
            // coinComboBox
            // 
            this.coinComboBox.AccessibleName = "Crypto currency";
            this.coinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.coinComboBox.FormattingEnabled = true;
            this.coinComboBox.Items.AddRange(new object[] {
            "Bitcoin",
            "Litecoin"});
            this.coinComboBox.Location = new System.Drawing.Point(220, 112);
            this.coinComboBox.Name = "coinComboBox";
            this.coinComboBox.Size = new System.Drawing.Size(140, 23);
            this.coinComboBox.TabIndex = 4;
            this.coinComboBox.SelectedIndexChanged += new System.EventHandler(this.coinComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "Crypto-currency:";
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.label4.Size = new System.Drawing.Size(470, 90);
            this.label4.TabIndex = 2;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // configurePoolPage
            // 
            this.configurePoolPage.Controls.Add(this.featuresButton);
            this.configurePoolPage.Controls.Add(this.pictureBox2);
            this.configurePoolPage.Controls.Add(this.poolsLink);
            this.configurePoolPage.Controls.Add(this.label6);
            this.configurePoolPage.Controls.Add(this.label7);
            this.configurePoolPage.Controls.Add(this.label8);
            this.configurePoolPage.Controls.Add(this.label9);
            this.configurePoolPage.Controls.Add(this.usernameEdit);
            this.configurePoolPage.Controls.Add(this.hostEdit);
            this.configurePoolPage.Controls.Add(this.portEdit);
            this.configurePoolPage.Controls.Add(this.passwordEdit);
            this.configurePoolPage.Controls.Add(this.label10);
            this.configurePoolPage.Location = new System.Drawing.Point(4, 24);
            this.configurePoolPage.Name = "configurePoolPage";
            this.configurePoolPage.Padding = new System.Windows.Forms.Padding(3);
            this.configurePoolPage.Size = new System.Drawing.Size(476, 231);
            this.configurePoolPage.TabIndex = 3;
            this.configurePoolPage.Text = "Configure Pool";
            this.configurePoolPage.UseVisualStyleBackColor = true;
            // 
            // featuresButton
            // 
            this.featuresButton.Location = new System.Drawing.Point(366, 114);
            this.featuresButton.Name = "featuresButton";
            this.featuresButton.Size = new System.Drawing.Size(81, 24);
            this.featuresButton.TabIndex = 4;
            this.featuresButton.Text = "Features";
            this.featuresButton.UseVisualStyleBackColor = true;
            this.featuresButton.Click += new System.EventHandler(this.featuresButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MultiMiner.Win.Properties.Resources.info;
            this.pictureBox2.Location = new System.Drawing.Point(36, 193);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(19, 18);
            this.pictureBox2.TabIndex = 35;
            this.pictureBox2.TabStop = false;
            // 
            // poolsLink
            // 
            this.poolsLink.AutoSize = true;
            this.poolsLink.Location = new System.Drawing.Point(63, 193);
            this.poolsLink.Name = "poolsLink";
            this.poolsLink.Size = new System.Drawing.Size(117, 15);
            this.poolsLink.TabIndex = 5;
            this.poolsLink.TabStop = true;
            this.poolsLink.Text = "Bitcoin mining pools";
            this.poolsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.bitcoinPoolsLink_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 15);
            this.label6.TabIndex = 31;
            this.label6.Text = "Host:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 15);
            this.label7.TabIndex = 30;
            this.label7.Text = "Worker name:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(325, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 15);
            this.label8.TabIndex = 29;
            this.label8.Text = "Port:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 148);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 15);
            this.label9.TabIndex = 28;
            this.label9.Text = "Password:";
            // 
            // usernameEdit
            // 
            this.usernameEdit.AccessibleName = "Worker name";
            this.usernameEdit.Location = new System.Drawing.Point(135, 114);
            this.usernameEdit.Name = "usernameEdit";
            this.usernameEdit.Size = new System.Drawing.Size(182, 23);
            this.usernameEdit.TabIndex = 2;
            this.usernameEdit.TextChanged += new System.EventHandler(this.hostEdit_TextChanged);
            // 
            // hostEdit
            // 
            this.hostEdit.AccessibleName = "Host";
            this.hostEdit.Location = new System.Drawing.Point(135, 84);
            this.hostEdit.Name = "hostEdit";
            this.hostEdit.Size = new System.Drawing.Size(182, 23);
            this.hostEdit.TabIndex = 0;
            this.hostEdit.TextChanged += new System.EventHandler(this.hostEdit_TextChanged);
            this.hostEdit.Validating += new System.ComponentModel.CancelEventHandler(this.hostEdit_Validating);
            this.hostEdit.Validated += new System.EventHandler(this.hostEdit_Validated);
            // 
            // portEdit
            // 
            this.portEdit.AccessibleName = "Port";
            this.portEdit.Location = new System.Drawing.Point(366, 84);
            this.portEdit.Name = "portEdit";
            this.portEdit.Size = new System.Drawing.Size(81, 23);
            this.portEdit.TabIndex = 1;
            this.portEdit.TextChanged += new System.EventHandler(this.hostEdit_TextChanged);
            // 
            // passwordEdit
            // 
            this.passwordEdit.AccessibleName = "Password";
            this.passwordEdit.Location = new System.Drawing.Point(135, 144);
            this.passwordEdit.Name = "passwordEdit";
            this.passwordEdit.Size = new System.Drawing.Size(182, 23);
            this.passwordEdit.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Location = new System.Drawing.Point(3, 3);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.label10.Size = new System.Drawing.Size(470, 78);
            this.label10.TabIndex = 32;
            this.label10.Text = "Now enter the connection and login information for your chosen mining pool. If yo" +
    "u have not yet chosen a mining pool for your crypto-currency, please consult Goo" +
    "gle to find a suitable provider.";
            // 
            // configurePerksPage
            // 
            this.configurePerksPage.Controls.Add(this.label3);
            this.configurePerksPage.Controls.Add(this.remotingPasswordEdit);
            this.configurePerksPage.Controls.Add(this.remotingCheckBox);
            this.configurePerksPage.Controls.Add(this.label2);
            this.configurePerksPage.Controls.Add(this.incomeCheckBox);
            this.configurePerksPage.Controls.Add(this.exchangeApiCheckbox);
            this.configurePerksPage.Controls.Add(this.perksCheckBox);
            this.configurePerksPage.Controls.Add(this.smileyPicture);
            this.configurePerksPage.Controls.Add(this.pictureBox3);
            this.configurePerksPage.Location = new System.Drawing.Point(4, 24);
            this.configurePerksPage.Name = "configurePerksPage";
            this.configurePerksPage.Padding = new System.Windows.Forms.Padding(3);
            this.configurePerksPage.Size = new System.Drawing.Size(476, 231);
            this.configurePerksPage.TabIndex = 6;
            this.configurePerksPage.Text = "Configure Perks";
            this.configurePerksPage.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 15);
            this.label3.TabIndex = 23;
            this.label3.Text = "Password (optional):";
            // 
            // remotingPasswordEdit
            // 
            this.remotingPasswordEdit.Location = new System.Drawing.Point(269, 188);
            this.remotingPasswordEdit.Name = "remotingPasswordEdit";
            this.remotingPasswordEdit.Size = new System.Drawing.Size(102, 23);
            this.remotingPasswordEdit.TabIndex = 22;
            this.remotingPasswordEdit.UseSystemPasswordChar = true;
            // 
            // remotingCheckBox
            // 
            this.remotingCheckBox.AutoSize = true;
            this.remotingCheckBox.Location = new System.Drawing.Point(117, 159);
            this.remotingCheckBox.Name = "remotingCheckBox";
            this.remotingCheckBox.Size = new System.Drawing.Size(178, 19);
            this.remotingCheckBox.TabIndex = 21;
            this.remotingCheckBox.Text = "Enable MultiMiner Remoting";
            this.remotingCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(51, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(414, 52);
            this.label2.TabIndex = 13;
            this.label2.Text = "By enabling the perks in MultiMiner, 1% of your mining resources will go towards " +
    "the software author.";
            // 
            // incomeCheckBox
            // 
            this.incomeCheckBox.AutoSize = true;
            this.incomeCheckBox.Checked = true;
            this.incomeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.incomeCheckBox.Location = new System.Drawing.Point(117, 130);
            this.incomeCheckBox.Name = "incomeCheckBox";
            this.incomeCheckBox.Size = new System.Drawing.Size(224, 19);
            this.incomeCheckBox.TabIndex = 12;
            this.incomeCheckBox.Text = "Show estimated income from devices";
            this.incomeCheckBox.UseVisualStyleBackColor = true;
            // 
            // exchangeApiCheckbox
            // 
            this.exchangeApiCheckbox.AutoSize = true;
            this.exchangeApiCheckbox.Checked = true;
            this.exchangeApiCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exchangeApiCheckbox.Location = new System.Drawing.Point(117, 102);
            this.exchangeApiCheckbox.Name = "exchangeApiCheckbox";
            this.exchangeApiCheckbox.Size = new System.Drawing.Size(250, 19);
            this.exchangeApiCheckbox.TabIndex = 11;
            this.exchangeApiCheckbox.Text = "Show exchange rates from Blockchain.info";
            this.exchangeApiCheckbox.UseVisualStyleBackColor = true;
            // 
            // perksCheckBox
            // 
            this.perksCheckBox.AutoSize = true;
            this.perksCheckBox.Checked = true;
            this.perksCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.perksCheckBox.Location = new System.Drawing.Point(96, 74);
            this.perksCheckBox.Name = "perksCheckBox";
            this.perksCheckBox.Size = new System.Drawing.Size(192, 19);
            this.perksCheckBox.TabIndex = 10;
            this.perksCheckBox.Text = "Enable perks (at a 1% donation)";
            this.perksCheckBox.UseVisualStyleBackColor = true;
            this.perksCheckBox.CheckedChanged += new System.EventHandler(this.perksCheckBox_CheckedChanged);
            // 
            // smileyPicture
            // 
            this.smileyPicture.Image = global::MultiMiner.Win.Properties.Resources.smiley_happy;
            this.smileyPicture.Location = new System.Drawing.Point(294, 75);
            this.smileyPicture.Name = "smileyPicture";
            this.smileyPicture.Size = new System.Drawing.Size(20, 20);
            this.smileyPicture.TabIndex = 15;
            this.smileyPicture.TabStop = false;
            this.smileyPicture.Visible = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MultiMiner.Win.Properties.Resources.info1;
            this.pictureBox3.Location = new System.Drawing.Point(10, 10);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(35, 34);
            this.pictureBox3.TabIndex = 14;
            this.pictureBox3.TabStop = false;
            // 
            // whatNextPage
            // 
            this.whatNextPage.Controls.Add(this.label13);
            this.whatNextPage.Location = new System.Drawing.Point(4, 24);
            this.whatNextPage.Name = "whatNextPage";
            this.whatNextPage.Padding = new System.Windows.Forms.Padding(3);
            this.whatNextPage.Size = new System.Drawing.Size(476, 231);
            this.whatNextPage.TabIndex = 5;
            this.whatNextPage.Text = "What Next";
            this.whatNextPage.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.Dock = System.Windows.Forms.DockStyle.Top;
            this.label13.Location = new System.Drawing.Point(3, 3);
            this.label13.Name = "label13";
            this.label13.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.label13.Size = new System.Drawing.Size(470, 261);
            this.label13.TabIndex = 33;
            this.label13.Text = resources.GetString("label13.Text");
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPanel.Controls.Add(this.button1);
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.nextButton);
            this.buttonPanel.Controls.Add(this.backButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 259);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(484, 42);
            this.buttonPanel.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.AccessibleName = "Help";
            this.button1.Image = global::MultiMiner.Win.Properties.Resources.help1;
            this.button1.Location = new System.Drawing.Point(10, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 11;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.Location = new System.Drawing.Point(288, 7);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(87, 27);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "< Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
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
            // WizardForm
            // 
            this.AcceptButton = this.nextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(484, 301);
            this.Controls.Add(this.wizardTabControl);
            this.Controls.Add(this.buttonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Getting Started";
            this.Load += new System.EventHandler(this.WizardForm_Load);
            this.wizardTabControl.ResumeLayout(false);
            this.chooseMinerPage.ResumeLayout(false);
            this.downloadingMinerPage.ResumeLayout(false);
            this.chooseCoinPage.ResumeLayout(false);
            this.chooseCoinPage.PerformLayout();
            this.configurePoolPage.ResumeLayout(false);
            this.configurePoolPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.configurePerksPage.ResumeLayout(false);
            this.configurePerksPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smileyPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.whatNextPage.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.poolFeaturesMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl wizardTabControl;
        private System.Windows.Forms.TabPage chooseMinerPage;
        private System.Windows.Forms.TabPage downloadingMinerPage;
        private System.Windows.Forms.TabPage chooseCoinPage;
        private System.Windows.Forms.TabPage configurePoolPage;
        private System.Windows.Forms.TabPage whatNextPage;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label downloadingMinerLabel;
        private System.Windows.Forms.ComboBox coinComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox usernameEdit;
        private System.Windows.Forms.TextBox hostEdit;
        private System.Windows.Forms.TextBox portEdit;
        private System.Windows.Forms.TextBox passwordEdit;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel poolsLink;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabPage configurePerksPage;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox incomeCheckBox;
        private System.Windows.Forms.CheckBox exchangeApiCheckbox;
        private System.Windows.Forms.CheckBox perksCheckBox;
        private System.Windows.Forms.PictureBox smileyPicture;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox remotingPasswordEdit;
        private System.Windows.Forms.CheckBox remotingCheckBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button featuresButton;
        private System.Windows.Forms.ContextMenuStrip poolFeaturesMenu;
        private System.Windows.Forms.ToolStripMenuItem disableCoinbaseCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extraNonceSubscriptToolStripMenuItem;
    }
}