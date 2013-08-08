namespace MultiMiner.Win
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.wizardTabControl = new System.Windows.Forms.TabControl();
            this.chooseMinerPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.minerComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.downloadingMinerPage = new System.Windows.Forms.TabPage();
            this.downloadingMinerLabel = new System.Windows.Forms.Label();
            this.chooseCoinPage = new System.Windows.Forms.TabPage();
            this.coinComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.configurePoolPage = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.hostEdit = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.configureMobileMinerPage = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.mobileMinerInfoLink = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.remoteCommandsCheck = new System.Windows.Forms.CheckBox();
            this.remoteMonitoringCheck = new System.Windows.Forms.CheckBox();
            this.appKeyEdit = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.emailAddressEdit = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.whatNextPage = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.bitcoinPoolsLink = new System.Windows.Forms.LinkLabel();
            this.litecoinPoolsLink = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.wizardTabControl.SuspendLayout();
            this.chooseMinerPage.SuspendLayout();
            this.downloadingMinerPage.SuspendLayout();
            this.chooseCoinPage.SuspendLayout();
            this.configurePoolPage.SuspendLayout();
            this.configureMobileMinerPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.whatNextPage.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardTabControl
            // 
            this.wizardTabControl.Controls.Add(this.chooseMinerPage);
            this.wizardTabControl.Controls.Add(this.downloadingMinerPage);
            this.wizardTabControl.Controls.Add(this.chooseCoinPage);
            this.wizardTabControl.Controls.Add(this.configurePoolPage);
            this.wizardTabControl.Controls.Add(this.configureMobileMinerPage);
            this.wizardTabControl.Controls.Add(this.whatNextPage);
            this.wizardTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardTabControl.Location = new System.Drawing.Point(0, 0);
            this.wizardTabControl.Name = "wizardTabControl";
            this.wizardTabControl.SelectedIndex = 0;
            this.wizardTabControl.Size = new System.Drawing.Size(436, 203);
            this.wizardTabControl.TabIndex = 0;
            this.wizardTabControl.SelectedIndexChanged += new System.EventHandler(this.wizardTabControl_SelectedIndexChanged);
            // 
            // chooseMinerPage
            // 
            this.chooseMinerPage.Controls.Add(this.label3);
            this.chooseMinerPage.Controls.Add(this.minerComboBox);
            this.chooseMinerPage.Controls.Add(this.label2);
            this.chooseMinerPage.Controls.Add(this.label1);
            this.chooseMinerPage.Location = new System.Drawing.Point(4, 22);
            this.chooseMinerPage.Name = "chooseMinerPage";
            this.chooseMinerPage.Padding = new System.Windows.Forms.Padding(3);
            this.chooseMinerPage.Size = new System.Drawing.Size(428, 177);
            this.chooseMinerPage.TabIndex = 0;
            this.chooseMinerPage.Text = "Choose Miner";
            this.chooseMinerPage.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(424, 109);
            this.label3.TabIndex = 3;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // minerComboBox
            // 
            this.minerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.minerComboBox.FormattingEnabled = true;
            this.minerComboBox.Items.AddRange(new object[] {
            "cgminer",
            "bfgminer"});
            this.minerComboBox.Location = new System.Drawing.Point(200, 60);
            this.minerComboBox.Name = "minerComboBox";
            this.minerComboBox.Size = new System.Drawing.Size(121, 21);
            this.minerComboBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mining engine:";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label1.Size = new System.Drawing.Size(422, 62);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome and thank you for choosing MultiMiner. To get started, please select the " +
    "mining engine you would like to use with MultiMiner.";
            // 
            // downloadingMinerPage
            // 
            this.downloadingMinerPage.Controls.Add(this.downloadingMinerLabel);
            this.downloadingMinerPage.Location = new System.Drawing.Point(4, 22);
            this.downloadingMinerPage.Name = "downloadingMinerPage";
            this.downloadingMinerPage.Padding = new System.Windows.Forms.Padding(3);
            this.downloadingMinerPage.Size = new System.Drawing.Size(428, 177);
            this.downloadingMinerPage.TabIndex = 1;
            this.downloadingMinerPage.Text = "Downloading Miner";
            this.downloadingMinerPage.UseVisualStyleBackColor = true;
            // 
            // downloadingMinerLabel
            // 
            this.downloadingMinerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.downloadingMinerLabel.Location = new System.Drawing.Point(3, 3);
            this.downloadingMinerLabel.Name = "downloadingMinerLabel";
            this.downloadingMinerLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.downloadingMinerLabel.Size = new System.Drawing.Size(422, 123);
            this.downloadingMinerLabel.TabIndex = 1;
            this.downloadingMinerLabel.Text = "Please wait while cgminer is downloaded from adsf.com and installed into the fold" +
    "er c:\\whatever";
            // 
            // chooseCoinPage
            // 
            this.chooseCoinPage.Controls.Add(this.coinComboBox);
            this.chooseCoinPage.Controls.Add(this.label5);
            this.chooseCoinPage.Controls.Add(this.label4);
            this.chooseCoinPage.Location = new System.Drawing.Point(4, 22);
            this.chooseCoinPage.Name = "chooseCoinPage";
            this.chooseCoinPage.Padding = new System.Windows.Forms.Padding(3);
            this.chooseCoinPage.Size = new System.Drawing.Size(428, 177);
            this.chooseCoinPage.TabIndex = 2;
            this.chooseCoinPage.Text = "Choose Coin";
            this.chooseCoinPage.UseVisualStyleBackColor = true;
            // 
            // coinComboBox
            // 
            this.coinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.coinComboBox.FormattingEnabled = true;
            this.coinComboBox.Items.AddRange(new object[] {
            "Bitcoin",
            "Litecoin"});
            this.coinComboBox.Location = new System.Drawing.Point(200, 97);
            this.coinComboBox.Name = "coinComboBox";
            this.coinComboBox.Size = new System.Drawing.Size(121, 21);
            this.coinComboBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(110, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Crypto-currency:";
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label4.Size = new System.Drawing.Size(422, 78);
            this.label4.TabIndex = 2;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // configurePoolPage
            // 
            this.configurePoolPage.Controls.Add(this.pictureBox3);
            this.configurePoolPage.Controls.Add(this.pictureBox2);
            this.configurePoolPage.Controls.Add(this.litecoinPoolsLink);
            this.configurePoolPage.Controls.Add(this.bitcoinPoolsLink);
            this.configurePoolPage.Controls.Add(this.label6);
            this.configurePoolPage.Controls.Add(this.label7);
            this.configurePoolPage.Controls.Add(this.label8);
            this.configurePoolPage.Controls.Add(this.label9);
            this.configurePoolPage.Controls.Add(this.textBox4);
            this.configurePoolPage.Controls.Add(this.hostEdit);
            this.configurePoolPage.Controls.Add(this.textBox2);
            this.configurePoolPage.Controls.Add(this.textBox1);
            this.configurePoolPage.Controls.Add(this.label10);
            this.configurePoolPage.Location = new System.Drawing.Point(4, 22);
            this.configurePoolPage.Name = "configurePoolPage";
            this.configurePoolPage.Padding = new System.Windows.Forms.Padding(3);
            this.configurePoolPage.Size = new System.Drawing.Size(428, 177);
            this.configurePoolPage.TabIndex = 3;
            this.configurePoolPage.Text = "Configure Pool";
            this.configurePoolPage.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Top;
            this.label10.Location = new System.Drawing.Point(3, 3);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label10.Size = new System.Drawing.Size(422, 68);
            this.label10.TabIndex = 32;
            this.label10.Text = "Now enter the connection and login information for your chosen mining pool. If yo" +
    "u have not yet chosen a mining pool for your crypto-currency, please consult Goo" +
    "gle to find a suitable provider.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Host:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Username:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(284, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Port:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(33, 132);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Password:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(121, 103);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(157, 20);
            this.textBox4.TabIndex = 26;
            // 
            // hostEdit
            // 
            this.hostEdit.Location = new System.Drawing.Point(121, 77);
            this.hostEdit.Name = "hostEdit";
            this.hostEdit.Size = new System.Drawing.Size(157, 20);
            this.hostEdit.TabIndex = 24;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(319, 77);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(70, 20);
            this.textBox2.TabIndex = 25;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(121, 129);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(157, 20);
            this.textBox1.TabIndex = 27;
            // 
            // configureMobileMinerPage
            // 
            this.configureMobileMinerPage.Controls.Add(this.mobileMinerInfoLink);
            this.configureMobileMinerPage.Controls.Add(this.pictureBox1);
            this.configureMobileMinerPage.Controls.Add(this.remoteCommandsCheck);
            this.configureMobileMinerPage.Controls.Add(this.remoteMonitoringCheck);
            this.configureMobileMinerPage.Controls.Add(this.appKeyEdit);
            this.configureMobileMinerPage.Controls.Add(this.label11);
            this.configureMobileMinerPage.Controls.Add(this.emailAddressEdit);
            this.configureMobileMinerPage.Controls.Add(this.label12);
            this.configureMobileMinerPage.Controls.Add(this.label14);
            this.configureMobileMinerPage.Location = new System.Drawing.Point(4, 22);
            this.configureMobileMinerPage.Name = "configureMobileMinerPage";
            this.configureMobileMinerPage.Padding = new System.Windows.Forms.Padding(3);
            this.configureMobileMinerPage.Size = new System.Drawing.Size(428, 177);
            this.configureMobileMinerPage.TabIndex = 4;
            this.configureMobileMinerPage.Text = "Configure MobileMiner";
            this.configureMobileMinerPage.UseVisualStyleBackColor = true;
            this.configureMobileMinerPage.Click += new System.EventHandler(this.configureMobileMinerPage_Click);
            // 
            // label14
            // 
            this.label14.Dock = System.Windows.Forms.DockStyle.Top;
            this.label14.Location = new System.Drawing.Point(3, 3);
            this.label14.Name = "label14";
            this.label14.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label14.Size = new System.Drawing.Size(422, 68);
            this.label14.TabIndex = 33;
            this.label14.Text = "MobileMiner allows you to remotely monitor and control mining from your smartphon" +
    "e. Enter your MobileMiner information below or click Next to skip this step.";
            // 
            // mobileMinerInfoLink
            // 
            this.mobileMinerInfoLink.AutoSize = true;
            this.mobileMinerInfoLink.Location = new System.Drawing.Point(59, 155);
            this.mobileMinerInfoLink.Name = "mobileMinerInfoLink";
            this.mobileMinerInfoLink.Size = new System.Drawing.Size(150, 13);
            this.mobileMinerInfoLink.TabIndex = 14;
            this.mobileMinerInfoLink.TabStop = true;
            this.mobileMinerInfoLink.Text = "Learn more about MobileMiner";
            this.mobileMinerInfoLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mobileMinerInfoLink_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MultiMiner.Win.Properties.Resources.info;
            this.pictureBox1.Location = new System.Drawing.Point(37, 153);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // remoteCommandsCheck
            // 
            this.remoteCommandsCheck.AutoSize = true;
            this.remoteCommandsCheck.Location = new System.Drawing.Point(241, 67);
            this.remoteCommandsCheck.Name = "remoteCommandsCheck";
            this.remoteCommandsCheck.Size = new System.Drawing.Size(148, 17);
            this.remoteCommandsCheck.TabIndex = 9;
            this.remoteCommandsCheck.Text = "Enable remote commands";
            this.remoteCommandsCheck.UseVisualStyleBackColor = true;
            // 
            // remoteMonitoringCheck
            // 
            this.remoteMonitoringCheck.AutoSize = true;
            this.remoteMonitoringCheck.Location = new System.Drawing.Point(37, 67);
            this.remoteMonitoringCheck.Name = "remoteMonitoringCheck";
            this.remoteMonitoringCheck.Size = new System.Drawing.Size(145, 17);
            this.remoteMonitoringCheck.TabIndex = 7;
            this.remoteMonitoringCheck.Text = "Enable remote monitoring";
            this.remoteMonitoringCheck.UseVisualStyleBackColor = true;
            // 
            // appKeyEdit
            // 
            this.appKeyEdit.Enabled = false;
            this.appKeyEdit.Location = new System.Drawing.Point(121, 121);
            this.appKeyEdit.Name = "appKeyEdit";
            this.appKeyEdit.Size = new System.Drawing.Size(268, 20);
            this.appKeyEdit.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(33, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Email address:";
            // 
            // emailAddressEdit
            // 
            this.emailAddressEdit.Enabled = false;
            this.emailAddressEdit.Location = new System.Drawing.Point(121, 95);
            this.emailAddressEdit.Name = "emailAddressEdit";
            this.emailAddressEdit.Size = new System.Drawing.Size(268, 20);
            this.emailAddressEdit.TabIndex = 10;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(33, 124);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Application key:";
            // 
            // whatNextPage
            // 
            this.whatNextPage.Controls.Add(this.label13);
            this.whatNextPage.Location = new System.Drawing.Point(4, 22);
            this.whatNextPage.Name = "whatNextPage";
            this.whatNextPage.Padding = new System.Windows.Forms.Padding(3);
            this.whatNextPage.Size = new System.Drawing.Size(428, 177);
            this.whatNextPage.TabIndex = 5;
            this.whatNextPage.Text = "What Next";
            this.whatNextPage.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.Dock = System.Windows.Forms.DockStyle.Top;
            this.label13.Location = new System.Drawing.Point(3, 3);
            this.label13.Name = "label13";
            this.label13.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label13.Size = new System.Drawing.Size(422, 226);
            this.label13.TabIndex = 33;
            this.label13.Text = resources.GetString("label13.Text");
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.nextButton);
            this.buttonPanel.Controls.Add(this.backButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 203);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(436, 36);
            this.buttonPanel.TabIndex = 1;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(12, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Location = new System.Drawing.Point(349, 6);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 1;
            this.nextButton.Text = "Next >";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.Location = new System.Drawing.Point(268, 6);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "< Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // bitcoinPoolsLink
            // 
            this.bitcoinPoolsLink.AutoSize = true;
            this.bitcoinPoolsLink.Location = new System.Drawing.Point(59, 171);
            this.bitcoinPoolsLink.Name = "bitcoinPoolsLink";
            this.bitcoinPoolsLink.Size = new System.Drawing.Size(100, 13);
            this.bitcoinPoolsLink.TabIndex = 33;
            this.bitcoinPoolsLink.TabStop = true;
            this.bitcoinPoolsLink.Text = "Bitcoin mining pools";
            this.bitcoinPoolsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.bitcoinPoolsLink_LinkClicked);
            // 
            // litecoinPoolsLink
            // 
            this.litecoinPoolsLink.AutoSize = true;
            this.litecoinPoolsLink.Location = new System.Drawing.Point(284, 171);
            this.litecoinPoolsLink.Name = "litecoinPoolsLink";
            this.litecoinPoolsLink.Size = new System.Drawing.Size(105, 13);
            this.litecoinPoolsLink.TabIndex = 34;
            this.litecoinPoolsLink.TabStop = true;
            this.litecoinPoolsLink.Text = "Litecoin mining pools";
            this.litecoinPoolsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.litecoinPoolsLink_LinkClicked);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MultiMiner.Win.Properties.Resources.info;
            this.pictureBox2.Location = new System.Drawing.Point(36, 171);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 35;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MultiMiner.Win.Properties.Resources.info;
            this.pictureBox3.Location = new System.Drawing.Point(262, 171);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // WizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 239);
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
            this.chooseMinerPage.PerformLayout();
            this.downloadingMinerPage.ResumeLayout(false);
            this.chooseCoinPage.ResumeLayout(false);
            this.chooseCoinPage.PerformLayout();
            this.configurePoolPage.ResumeLayout(false);
            this.configurePoolPage.PerformLayout();
            this.configureMobileMinerPage.ResumeLayout(false);
            this.configureMobileMinerPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.whatNextPage.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl wizardTabControl;
        private System.Windows.Forms.TabPage chooseMinerPage;
        private System.Windows.Forms.TabPage downloadingMinerPage;
        private System.Windows.Forms.TabPage chooseCoinPage;
        private System.Windows.Forms.TabPage configurePoolPage;
        private System.Windows.Forms.TabPage configureMobileMinerPage;
        private System.Windows.Forms.TabPage whatNextPage;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox minerComboBox;
        private System.Windows.Forms.Label label2;
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
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox hostEdit;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.LinkLabel mobileMinerInfoLink;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox remoteCommandsCheck;
        private System.Windows.Forms.CheckBox remoteMonitoringCheck;
        private System.Windows.Forms.TextBox appKeyEdit;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox emailAddressEdit;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel litecoinPoolsLink;
        private System.Windows.Forms.LinkLabel bitcoinPoolsLink;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}