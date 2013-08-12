using MultiMiner.Coinchoose.Api;
using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace MultiMiner.Win
{
    public partial class WizardForm : Form
    {
        private List<CryptoCoin> coins;

        public WizardForm(List<CryptoCoin> knownCoins)
        {
            InitializeComponent();
            this.coins = knownCoins;
        }

        private void configureMobileMinerPage_Click(object sender, EventArgs e)
        {

        }

        private void mobileMinerInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://mobileminerapp.com/");
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            SetupWizardTabControl();
            PopulateCoins();
            minerComboBox.SelectedIndex = 0;
            coinComboBox.SelectedIndex = 0;
        }
        
        private void PopulateCoins()
        {
            coinComboBox.Items.Clear();

            coinComboBox.Items.Add("Bitcoin");
            coinComboBox.Items.Add("Litecoin");
            coinComboBox.Items.Add("-");

            foreach (CryptoCoin coin in coins)
            {
                if (coinComboBox.Items.IndexOf(coin.Name) == -1)
                    coinComboBox.Items.Add(coin.Name);
            }
        }

        private void SetupWizardTabControl()
        {
            const int margin = 3;
            const int tabHeight = 21;

            wizardTabControl.SelectedTab = chooseMinerPage;
            wizardTabControl.Dock = DockStyle.None;
            wizardTabControl.Top = -(margin + tabHeight);
            wizardTabControl.Left = -(margin);
            wizardTabControl.Width = this.ClientSize.Width + (margin * 2);
            wizardTabControl.Height = this.ClientSize.Height - buttonPanel.Height + (margin * 2) + tabHeight;

            foreach (TabPage tabPage in wizardTabControl.TabPages)
            {
                tabPage.Padding = new Padding(6, tabPage.Padding.Left, 6, tabPage.Padding.Right);
            }
        }

        private static bool MinerIsInstalled(MinerBackend minerBackend)
        {
            string path = MinerPath.GetPathToInstalledMiner(minerBackend);
            return File.Exists(path);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            if (wizardTabControl.SelectedTab == chooseMinerPage)
            {
                MinerBackend minerBackend = GetSelectedMinerBackend();
                if (MinerIsInstalled(minerBackend))
                {
                    wizardTabControl.SelectedIndex += 2;
                }
                else
                {
                    wizardTabControl.SelectedIndex += 1;
                }
            }
            else
            {
                if (wizardTabControl.SelectedIndex < wizardTabControl.TabPages.Count - 1)
                    wizardTabControl.SelectedIndex += 1;
                else
                    DialogResult = System.Windows.Forms.DialogResult.OK;
            }

            
            if (wizardTabControl.SelectedTab == downloadingMinerPage)
            {
                if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                    showLinuxInstallationInstructions();
                else
                    DownloadChosenMiner();
            }
        }

        private void showLinuxInstallationInstructions()
        {
            downloadingMinerLabel.Text =
@"Unfortunately, prebuilt binaries of bfgminer are not available for Linux at this time. Additionally, only 64-bit binaries are avilable for cgminer.

To install cgminer and/or bfgminer on Linux, please consult the websites for each miner. There are repositories for many popular Linux distributions.";
        }

        private bool ValidateInput()
        {
            if (wizardTabControl.SelectedTab == configureMobileMinerPage)
            {
                if (emailAddressEdit.Enabled && !String.IsNullOrEmpty(emailAddressEdit.Text) &&
                    !InputValidation.IsValidEmailAddress(emailAddressEdit.Text))
                {
                    emailAddressEdit.Focus();
                    MessageBox.Show("Please specify a valid email address. This must be the same address used to register MobileMiner.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (appKeyEdit.Enabled && !String.IsNullOrEmpty(appKeyEdit.Text) &&
                    !InputValidation.IsValidApplicationKey(appKeyEdit.Text))
                {
                    appKeyEdit.Focus();
                    MessageBox.Show("Please specify a valid application key. If you are unsure, copy and paste the application key from the email you received.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (wizardTabControl.SelectedTab == chooseCoinPage)
            {
                //skip the downloading page
                wizardTabControl.SelectedIndex -= 2;
            }
            else
            {
                if (wizardTabControl.SelectedIndex > 0)
                    wizardTabControl.SelectedIndex -= 1;
            }
        }

        private void wizardTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void DownloadChosenMiner()
        {
            MinerBackend minerBackend = GetSelectedMinerBackend();

            string minerName = MinerPath.GetMinerName(minerBackend);
            string minerPath = Path.Combine("Miners", minerName);
            string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);

            downloadingMinerLabel.Text = String.Format("Please wait while {0} is downloaded from {2} and installed into the folder {1}", minerName, destinationFolder, Installer.GetMinerDownloadRoot(minerBackend));
            Application.DoEvents();

            Cursor = Cursors.WaitCursor;
            Installer.InstallMiner(minerBackend, destinationFolder);
            Cursor = Cursors.Default;

            wizardTabControl.SelectedTab = chooseCoinPage;
        }

        private MinerBackend GetSelectedMinerBackend()
        {
            MinerBackend minerBackend = MinerBackend.Cgminer;
            if (minerComboBox.SelectedIndex == 1)
                minerBackend = MinerBackend.Bfgminer;
            return minerBackend;
        }

        private void UpdateButtons()
        {
            bool nextButtonEnabled = true;
            bool closeButtonEnabled = true;
            bool backButtonEnabled = true;

            if (wizardTabControl.SelectedIndex == wizardTabControl.TabPages.Count - 1)
                nextButton.Text = "Finish";
            else
                nextButton.Text = "Next >";

            backButtonEnabled = wizardTabControl.SelectedIndex > 0;

            if (wizardTabControl.SelectedTab == chooseCoinPage)
            {
                nextButtonEnabled = coinComboBox.Text != "-";
            }
            else if (wizardTabControl.SelectedTab == configurePoolPage)
            {
                int dummy;
                nextButtonEnabled = !String.IsNullOrEmpty(hostEdit.Text) &&
                    !String.IsNullOrEmpty(portEdit.Text) &&
                    !String.IsNullOrEmpty(usernameEdit.Text) &&
                    Int32.TryParse(portEdit.Text, out dummy);
            }
            else if (wizardTabControl.SelectedTab == downloadingMinerPage)
            {
                //no downloading miners under Linux
                if (OSVersionPlatform.GetConcretePlatform() != PlatformID.Unix)
                {
                    nextButtonEnabled = false;
                    backButtonEnabled = false;
                    closeButtonEnabled = false;
                }
            }

            nextButton.Enabled = nextButtonEnabled;
            closeButton.Enabled = closeButtonEnabled;
            backButton.Enabled = backButtonEnabled;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitcoinPoolsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(String.Format("https://google.com/search?q={0}+mining+pools", coinComboBox.Text));
        }

        private void coinComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
            poolsLink.Text = coinComboBox.Text + " mining pools";
        }

        private void hostEdit_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void remoteMonitoringCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMobileMinerEdits();
        }

        private void remoteCommandsCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMobileMinerEdits();
        }

        private void UpdateMobileMinerEdits()
        {
            emailAddressEdit.Enabled = remoteMonitoringCheck.Checked || remoteCommandsCheck.Checked;
            appKeyEdit.Enabled = emailAddressEdit.Enabled;
        }

        public void CreateConfigurations(out EngineConfiguration engineConfiguration, 
            out ApplicationConfiguration applicationConfiguraion)
        {
            engineConfiguration = CreateEngineConfiguration();
            applicationConfiguraion = CreateApplicationConfiguration();
        }

        private EngineConfiguration CreateEngineConfiguration()
        {
            EngineConfiguration engineConfiguration;
            engineConfiguration = new EngineConfiguration();

            engineConfiguration.XgminerConfiguration.MinerBackend = MinerBackend.Cgminer;
            if (minerComboBox.SelectedIndex == 1)
                engineConfiguration.XgminerConfiguration.MinerBackend = MinerBackend.Bfgminer;

            CoinConfiguration coinConfiguration = new CoinConfiguration();

            CryptoCoin coin = coins.Single(c => c.Name.Equals(coinComboBox.Text));

            coinConfiguration.Coin = coin;
            coinConfiguration.Enabled = true;

            MiningPool miningPool = new MiningPool();

            miningPool.Host = hostEdit.Text;
            miningPool.Port = Int32.Parse(portEdit.Text);
            miningPool.Username = usernameEdit.Text;
            miningPool.Password = passwordEdit.Text;

            coinConfiguration.Pools.Add(miningPool);

            engineConfiguration.CoinConfigurations.Add(coinConfiguration);
            return engineConfiguration;
        }

        private ApplicationConfiguration CreateApplicationConfiguration()
        {
            ApplicationConfiguration applicationConfiguraion;
            applicationConfiguraion = new ApplicationConfiguration();
            applicationConfiguraion.MobileMinerMonitoring = remoteMonitoringCheck.Checked;
            applicationConfiguraion.MobileMinerRemoteCommands = remoteCommandsCheck.Checked;
            applicationConfiguraion.MobileMinerEmailAddress = emailAddressEdit.Text;
            applicationConfiguraion.MobileMinerApplicationKey = appKeyEdit.Text;
            return applicationConfiguraion;
        }
    }
}
