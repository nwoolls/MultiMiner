using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Installers;
using MultiMiner.Utility.OS;
using MultiMiner.UX.Data;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Application = MultiMiner.UX.Data.Configuration.Application;

namespace MultiMiner.Win.Forms
{
    public partial class WizardForm : MessageBoxFontForm
    {
        private List<PoolGroup> coins;

        public WizardForm(List<PoolGroup> knownCoins)
        {
            InitializeComponent();
            this.coins = knownCoins;
        }

        private void mobileMinerInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            SetupWizardTabControl();
            PopulateCoins();
            coinComboBox.SelectedIndex = 0;
            UpdateCheckStates();
            SetupPoolDefaults();
            UpdateThanks();
        }

        private void SetupPoolDefaults()
        {
            hostEdit.Text = PoolDefaults.HostPrefix;
            portEdit.Text = PoolDefaults.Port.ToString();
        }

        private void PopulateCoins()
        {
            coinComboBox.Items.Clear();

            coinComboBox.Items.Add(KnownCoins.BitcoinName);
            coinComboBox.Items.Add(KnownCoins.LitecoinName);
            coinComboBox.Items.Add("-");

            coins.Where(c => c.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                .OrderBy(c => c.Name)
                .ToList()
                .ForEach((c) =>
                {
                    if (coinComboBox.Items.IndexOf(c.Name) == -1)
                        coinComboBox.Items.Add(c.Name);
                });
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
                tabPage.BackColor = SystemColors.ControlLightLight;
            }

            buttonPanel.BringToFront();
        }

        private static bool MinerIsInstalled()
        {
            string path = MinerPath.GetPathToInstalledMiner(MinerFactory.Instance.GetDefaultMiner());
            return File.Exists(path);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            if (wizardTabControl.SelectedTab == chooseMinerPage)
            {
                if (MinerIsInstalled())
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
@"Unfortunately, prebuilt binaries of " + MinerNames.BFGMiner + @" are not available for Linux at this time.

To install " + MinerNames.BFGMiner + @" on Linux please consult the website for " + MinerNames.BFGMiner + @". There are repositories for many popular Linux distributions.";
        }

        private bool ValidateInput()
        {
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

            if (wizardTabControl.SelectedTab == configurePoolPage)
                hostEdit.SelectionStart = hostEdit.SelectionLength;
        }

        private void DownloadChosenMiner()
        {
            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
            string minerName = miner.Name;
            string minerPath = Path.Combine("Miners", minerName);
            string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
            
            downloadingMinerLabel.Text = String.Format("Please wait while {0} is downloaded from {2} and installed into the folder {1}", minerName, destinationFolder, new Uri(miner.Url).Authority);
            System.Windows.Forms.Application.DoEvents();

            Cursor = Cursors.WaitCursor;
            MinerInstaller.InstallMiner(UserAgent.AgentString, miner, destinationFolder);
            Cursor = Cursors.Default;

            wizardTabControl.SelectedTab = chooseCoinPage;
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
        }

        private void remoteCommandsCheck_CheckedChanged(object sender, EventArgs e)
        {
        }

        public void CreateConfigurations(out Engine.Data.Configuration.Engine engineConfiguration,
            out UX.Data.Configuration.Application applicationConfiguraion,
            out Perks perksConfiguration)
        {
            engineConfiguration = CreateEngineConfiguration();
            applicationConfiguraion = CreateApplicationConfiguration();
            perksConfiguration = CreatePerksConfiguration();
        }

        private Perks CreatePerksConfiguration()
        {
            Perks result = new Perks();

            result.PerksEnabled = perksCheckBox.Checked;
            result.ShowExchangeRates = exchangeApiCheckbox.Checked;
            result.ShowIncomeRates = incomeCheckBox.Checked;
            result.EnableRemoting = remotingCheckBox.Checked;
            result.RemotingPassword = remotingPasswordEdit.Text;

            return result;
        }

        private Engine.Data.Configuration.Engine CreateEngineConfiguration()
        {
            Engine.Data.Configuration.Engine engineConfiguration = new Engine.Data.Configuration.Engine();

            Engine.Data.Configuration.Coin coinConfiguration = new Engine.Data.Configuration.Coin();

            PoolGroup coin = coins.SingleOrDefault(c => c.Name.Equals(coinComboBox.Text));

            //no Internet connection (or coin API offline) - only BTC and LTC were available
            if (coin == null)
            {
                coin = new PoolGroup();
                coin.Name = coinComboBox.Text;

                if (coin.Name.Equals(KnownCoins.LitecoinName, StringComparison.OrdinalIgnoreCase))
                {
                    coin.Algorithm = AlgorithmNames.Scrypt;
                    coin.Id = KnownCoins.LitecoinSymbol;
                }
                else
                {
                    coin.Algorithm = AlgorithmNames.SHA256;
                    coin.Id = KnownCoins.BitcoinSymbol;
                }
            }
            else
            {
                coin = coins.Single(c => c.Name.Equals(coinComboBox.Text));
            }

            coinConfiguration.PoolGroup = coin;
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

        private UX.Data.Configuration.Application CreateApplicationConfiguration()
        {
            Application applicationConfiguraion = new Application
            {
                BriefUserInterface = false
            };

            //make things obvious for new user, don't hide them

            return applicationConfiguraion;
        }

        private void perksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckStates();
            UpdateThanks();
        }

        private void UpdateThanks()
        {
            string textBase = "Enable perks (at a 1% donation)";
            if (perksCheckBox.Checked)
                textBase = textBase + " - thank you!";

            perksCheckBox.Text = textBase;
            smileyPicture.Visible = perksCheckBox.Checked;
            smileyPicture.Left = perksCheckBox.Left + perksCheckBox.Width + 2;
        }

        private void UpdateCheckStates()
        {
            this.exchangeApiCheckbox.Enabled = perksCheckBox.Checked;
            this.incomeCheckBox.Enabled = perksCheckBox.Checked;
            this.remotingCheckBox.Enabled = perksCheckBox.Checked;
            this.remotingPasswordEdit.Enabled = perksCheckBox.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Getting-Started");
        }

        private void featuresButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = featuresButton.PointToScreen(new Point(featuresButton.Left, featuresButton.Bottom));
            if (screenPoint.Y + poolFeaturesMenu.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
                poolFeaturesMenu.Show(featuresButton, new Point(0, -poolFeaturesMenu.Size.Height));
            else
                poolFeaturesMenu.Show(featuresButton, new Point(0, featuresButton.Height));    
        }

        private void poolFeaturesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            extraNonceSubscriptToolStripMenuItem.Checked = hostEdit.Text.Contains(PoolFeatures.XNSubFragment);
            disableCoinbaseCheckToolStripMenuItem.Checked = hostEdit.Text.Contains(PoolFeatures.SkipCBCheckFragment);
        }

        private void UpdatePoolFeature(string fragment, bool enabled)
        {
            hostEdit.Text = PoolFeatures.UpdatePoolFeature(hostEdit.Text, fragment, enabled);
        }

        private void extraNonceSubscriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdatePoolFeature(PoolFeatures.XNSubFragment, extraNonceSubscriptToolStripMenuItem.Checked);
        }

        private void disableCoinbaseCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdatePoolFeature(PoolFeatures.SkipCBCheckFragment, disableCoinbaseCheckToolStripMenuItem.Checked);
        }

        private void hostEdit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string hostText = ((TextBox)sender).Text;
            e.Cancel = !ValidateHostText(hostText);
        }

        private bool ValidateHostText(string hostText)
        {
            string prefix = "";
            if (!hostText.Contains("://"))
            {
                prefix = "dummy://";
            }
            try
            {
                new Uri(prefix + hostText);
            }
            catch (UriFormatException)
            {
                MessageBox.Show(String.Format("The specified value '{0}' is not a valid URI.", hostText), "Invalid URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void hostEdit_Validated(object sender, EventArgs e)
        {
            ParseHostForPort();
        }

        private void ParseHostForPort()
        {
            int newPort;
            string newHost;

            if (hostEdit.Text.ParseHostAndPort(out newHost, out newPort))
            {
                hostEdit.Text = newHost;
                portEdit.Text = newPort.ToString();
            }
        }
    }
}
