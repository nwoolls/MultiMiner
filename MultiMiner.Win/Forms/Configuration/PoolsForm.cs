using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class PoolsForm : MessageBoxFontForm
    {
        private readonly List<Engine.Data.Configuration.Coin> configurations = new List<Engine.Data.Configuration.Coin>();
        private readonly List<PoolGroup> knownCoins;

        public PoolsForm(List<Engine.Data.Configuration.Coin> coinConfigurations, List<PoolGroup> knownCoins,
            UX.Data.Configuration.Application applicationConfiguration, UX.Data.Configuration.Perks perksConfiguration)
        {
            this.configurations = coinConfigurations;
            this.knownCoins = knownCoins;

            InitializeComponent();

            saveToRemotingCheckBox.Visible = false;
            if (perksConfiguration.PerksEnabled && perksConfiguration.EnableRemoting)
            {
                saveToRemotingCheckBox.Visible = true;
                this.applicationBindingSource.DataSource = applicationConfiguration;
            }
        }

        private void CoinsForm_Load(object sender, EventArgs e)
        {
            //not supported on mono
            if (OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix)
                coinListBox.AllowDrop = true;
            PopulateConfigurations();
            UpdateButtonStates();
        }
        
        private void removeCoinButton_Click(object sender, EventArgs e)
        {
            DialogResult promptResult = MessageBox.Show("Remove the selected configuration?", "Confirm", MessageBoxButtons.YesNo);
            if (promptResult == System.Windows.Forms.DialogResult.Yes)
            {
                //required to clear bindings if this was the last coin in the list
                coinConfigurationBindingSource.DataSource = typeof(Engine.Data.Configuration.Coin);
                miningPoolBindingSource.DataSource = typeof(MiningPool);

                Engine.Data.Configuration.Coin configuration = configurations[coinListBox.SelectedIndex];
                configurations.Remove(configuration);
                coinListBox.Items.RemoveAt(coinListBox.SelectedIndex);

                //select a coin - otherwise nothing will be selected
                if (configurations.Count > 0)
                    coinListBox.SelectedIndex = 0;
            }
        }

        private void PopulateConfigurations()
        {
            coinListBox.Items.Clear();

            foreach (Engine.Data.Configuration.Coin configuration in configurations)
                coinListBox.Items.Add(configuration.PoolGroup.Name);

            if (configurations.Count > 0)
                coinListBox.SelectedIndex = 0;
        }

        private Engine.Data.Configuration.Coin AddCoinConfiguration(PoolGroup poolGroup)
        {
            //don't allow two configurations for the same coin symbol
            Engine.Data.Configuration.Coin configuration = configurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(poolGroup.Id, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                //don't allow two configurations for the same coin name
                configuration = configurations.SingleOrDefault(c => c.PoolGroup.Name.Equals(poolGroup.Name, StringComparison.OrdinalIgnoreCase));

            if (configuration != null)
            {
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }
            else
            {
                configuration = new Engine.Data.Configuration.Coin();

                configuration.PoolGroup = knownCoins.SingleOrDefault(c => c.Id.Equals(poolGroup.Id, StringComparison.OrdinalIgnoreCase));

                //user may have manually entered a coin or may be using a Multipool
                if (configuration.PoolGroup == null)
                {
                    configuration.PoolGroup = new PoolGroup();
                    ObjectCopier.CopyObject(poolGroup, configuration.PoolGroup);
                }

                //at this point, configuration.CryptoCoin.Algorithm MAY be the CoinAlgorithm.FullName
                //that is how data from Coin API is stored
                //but coin configurations are based on CoinAlgorithm.Name
                CoinAlgorithm algorithm = MinerFactory.Instance.Algorithms.SingleOrDefault(a => 
                    a.FullName.Equals(configuration.PoolGroup.Algorithm, StringComparison.OrdinalIgnoreCase));
                if (algorithm != null)
                    configuration.PoolGroup.Algorithm = algorithm.Name;

                MiningPool miningPool = new MiningPool()
                {
                    Host = UX.Data.Configuration.PoolDefaults.HostPrefix,
                    Port = UX.Data.Configuration.PoolDefaults.Port
                };
                configuration.Pools.Add(miningPool);

                configurations.Add(configuration);

                coinListBox.Items.Add(configuration.PoolGroup.Name);
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }

            hostEdit.Focus();
            hostEdit.SelectionStart = hostEdit.SelectionLength;

            return configuration;
        }

        private void coinListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (coinListBox.SelectedIndex >= 0)
                BindToCurrentConfiguration();

            UpdateButtonStates();
        }

        private void BindToCurrentConfiguration()
        {
            Engine.Data.Configuration.Coin configuration = configurations[coinListBox.SelectedIndex];

            coinConfigurationBindingSource.DataSource = configuration;
            miningPoolBindingSource.DataSource = configuration.Pools;
            poolListBox.DataSource = miningPoolBindingSource;
            poolListBox.DisplayMember = "Host";
        }

        private void addPoolButton_Click(object sender, EventArgs e)
        {
            Engine.Data.Configuration.Coin configuration = configurations[coinListBox.SelectedIndex];

            MiningPool miningPool = new MiningPool()
            {
                Host = UX.Data.Configuration.PoolDefaults.HostPrefix,
                Port = UX.Data.Configuration.PoolDefaults.Port
            };
            miningPoolBindingSource.Add(miningPool);

            poolListBox.SelectedIndex = configuration.Pools.Count - 1;
            hostEdit.Focus();
            hostEdit.SelectionStart = hostEdit.SelectionLength;
        }

        private void removePoolButton_Click(object sender, EventArgs e)
        {
            DialogResult promptResult = MessageBox.Show("Remove the selected pool configuration?", "Confirm", MessageBoxButtons.YesNo);
            if (promptResult == System.Windows.Forms.DialogResult.Yes)
            {
                miningPoolBindingSource.RemoveAt(poolListBox.SelectedIndex);
                hostEdit.Focus();
            }
        }

        private void UpdateButtonStates()
        {
            addPoolButton.Enabled = coinListBox.SelectedIndex >= 0;
            removePoolButton.Enabled = (coinListBox.SelectedIndex >= 0) && (poolListBox.SelectedIndex >= 0);
            removeCoinButton.Enabled = (coinListBox.SelectedIndex >= 0);
            copyCoinButton.Enabled = (coinListBox.SelectedIndex >= 0);
            editCoinButton.Enabled = (coinListBox.SelectedIndex >= 0);
            poolUpButton.Enabled = (poolListBox.SelectedIndex >= 1);
            poolDownButton.Enabled = (poolListBox.SelectedIndex < poolListBox.Items.Count - 1);
        }
        
        private void poolListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void adjustProfitCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (coinConfigurationBindingSource.Current == null)
                return;

            Engine.Data.Configuration.Coin currentConfiguration = (Engine.Data.Configuration.Coin)coinConfigurationBindingSource.Current;
            currentConfiguration.ProfitabilityAdjustmentType = (Engine.Data.Configuration.Coin.AdjustmentType)((ComboBox)sender).SelectedIndex;
        }

        private void coinConfigurationBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (coinConfigurationBindingSource.Current == null)
                return;
            //prevents a crash under Mono on macOS - "System.IndexOutOfRangeException: list position"
            poolListBox.SelectedItem = null;

            Engine.Data.Configuration.Coin currentConfiguration = (Engine.Data.Configuration.Coin)coinConfigurationBindingSource.Current;
            adjustProfitCombo.SelectedIndex = (int)currentConfiguration.ProfitabilityAdjustmentType;
        }

        private void poolUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedPool(-1);
        }

        private void poolDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedPool(1);
        }

        private void MoveSelectedPool(int offset)
        {
            if (miningPoolBindingSource.Current == null)
                return;

            Object currentObject = miningPoolBindingSource.Current;
            int currentIndex = miningPoolBindingSource.IndexOf(currentObject);
            int newIndex = currentIndex + offset;
            miningPoolBindingSource.RemoveAt(currentIndex);
            miningPoolBindingSource.Insert(newIndex, currentObject);
            miningPoolBindingSource.Position = newIndex;
            poolListBox.Focus();
        }

        private void addCoinButton_Click(object sender, EventArgs e)
        {
            CoinChooseForm coinChooseForm = new CoinChooseForm(knownCoins);
            DialogResult dialogResult = coinChooseForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                AddCoinConfiguration(coinChooseForm.SelectedCoin);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                userNameCombo.Focus();
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "CoinConfigurations.xml";
            openFileDialog1.Title = "Import CoinConfigurations.xml";
            openFileDialog1.Filter = "XML files|*.xml";
            openFileDialog1.DefaultExt = ".xml";

            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                string sourceFileName = openFileDialog1.FileName;

                MergeConfigurationsFromFile(sourceFileName);

                PopulateConfigurations();
            }
        }

        private void MergeConfigurationsFromFile(string configurationsFileName)
        {
            List<Engine.Data.Configuration.Coin> sourceConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<Engine.Data.Configuration.Coin>>(configurationsFileName);
            List<Engine.Data.Configuration.Coin> destinationConfigurations = configurations;

            foreach (Engine.Data.Configuration.Coin sourceConfiguration in sourceConfigurations)
            {
                int existingIndex = destinationConfigurations.FindIndex(c => c.PoolGroup.Id.Equals(sourceConfiguration.PoolGroup.Id));
                if (existingIndex == -1)
                    destinationConfigurations.Add(sourceConfiguration);
                else
                    destinationConfigurations[existingIndex] = sourceConfiguration;
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "CoinConfigurations.xml";
            saveFileDialog1.Title = "Export CoinConfigurations.xml";
            saveFileDialog1.Filter = "XML files|*.xml";
            saveFileDialog1.DefaultExt = ".xml";

            DialogResult dialogResult = saveFileDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //string sourceFileName = coinConfigurationsFileName;
                string destinationFileName = saveFileDialog1.FileName;
                if (File.Exists(destinationFileName))
                    File.Delete(destinationFileName);
                ConfigurationReaderWriter.WriteConfiguration(configurations, destinationFileName);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SortConfigurations();
        }

        private void SortConfigurations()
        {
            configurations.Sort((config1, config2) => config1.PoolGroup.Name.CompareTo(config2.PoolGroup.Name));
            PopulateConfigurations();
        }

        private void coinListBox_MouseMove(object sender, MouseEventArgs e)
        {
            //not supported on mono
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (coinListBox.SelectedItem == null) return;
                coinListBox.DoDragDrop(coinListBox.SelectedItem, DragDropEffects.Move);
            }
        }

        private void coinListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void coinListBox_DragDrop(object sender, DragEventArgs e)
        {
            Point point = coinListBox.PointToClient(new Point(e.X, e.Y));
            int index = coinListBox.IndexFromPoint(point);
            if (index < 0) index = coinListBox.Items.Count - 1;

            string coinName = (string)e.Data.GetData(typeof(string));

            MoveCoinToIndex(coinName, index);
        }

        private void MoveCoinToIndex(string coinName, int index)
        {
            Engine.Data.Configuration.Coin configuration = configurations.Single(
                config => config.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));

            configurations.Remove(configuration);
            configurations.Insert(index, configuration);

            coinListBox.Items.Remove(coinName);
            coinListBox.Items.Insert(index, coinName);

            coinListBox.SelectedIndex = index;
        }

        private void copyCoinButton_Click(object sender, EventArgs e)
        {
            CoinChooseForm coinChooseForm = new CoinChooseForm(knownCoins);
            DialogResult dialogResult = coinChooseForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                PoolGroup destinationCoin = coinChooseForm.SelectedCoin;

                Engine.Data.Configuration.Coin sourceConfiguration = configurations[coinListBox.SelectedIndex];

                MultiMiner.Engine.Data.Configuration.Coin destinationConfiguration = AddCoinConfiguration(destinationCoin);

                ObjectCopier.CopyObject(sourceConfiguration, destinationConfiguration, "CryptoCoin");

                BindToCurrentConfiguration();
                coinConfigurationBindingSource.ResetBindings(false);
                miningPoolBindingSource.ResetBindings(false);
            }
        }

        private void coinListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            //draw disabled Coin configurations with SystemColors.GrayText
            //e.Index will be -1 if the user deletes all items in the list
            if (e.Index == -1)
                return;

            Engine.Data.Configuration.Coin configuration = configurations[e.Index];

            Color textColor = SystemColors.WindowText;
            if (!configuration.Enabled)
                textColor = SystemColors.GrayText;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                textColor = SystemColors.HighlightText;

            Rectangle textBounds = e.Bounds;
            textBounds.Inflate(0, 2);

            e.DrawBackground();
            using (Brush myBrush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                    e.Font, myBrush, textBounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }

        private void miningPoolBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            PopulateWorkerNames();
            PopulatePort();
        }

        private void PopulateWorkerNames()
        {
            userNameCombo.Items.Clear();

            foreach (Engine.Data.Configuration.Coin configuration in configurations)
            {
                IEnumerable<string> coinWorkerNames = configuration.Pools
                    .Select(p => p.Username)
                    .Where(p => !String.IsNullOrEmpty(p))
                    .Distinct();

                userNameCombo.Items.AddRange(coinWorkerNames.Where(wn => !userNameCombo.Items.Contains(wn)).ToArray());
            }
        }

        private void userNameCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            MiningPool currentPool = (MiningPool)miningPoolBindingSource.Current;
            if (currentPool == null)
            {
                return;
            }

            //if the password is blank
            if (String.IsNullOrEmpty(currentPool.Password))
            {
                //and a workername is selected
                if (userNameCombo.SelectedItem != null)
                {
                    string workerName = (string)userNameCombo.SelectedItem;
                    MultiMiner.Engine.Data.Configuration.Coin configuration = configurations
                                        .FirstOrDefault(c => c.Pools.Any(p => p.Username.Equals(workerName)));

                    //default to the password used for the same worker on another config
                    if (configuration != null)
                        currentPool.Password = configuration.Pools.First(p => p.Username.Equals(workerName)).Password;
                }
            }
        }

        //parse the port out for folks that paste in host:port
        private void hostEdit_Validated(object sender, EventArgs e)
        {
            ParseHostForPort();
        }

        private void ParseHostForPort()
        {
            MiningPool currentPool = (MiningPool)miningPoolBindingSource.Current;
            if (currentPool == null)
            {
                return;
            }

            int newPort;
            string newHost;

            if (currentPool.Host.ParseHostAndPort(out newHost, out newPort))
            {
                currentPool.Host = newHost;
                currentPool.Port = newPort;
                PopulatePort(); // we don't data-bind port, crashes on mono for invalid values

                //required since we are validating this edit
                hostEdit.Text = newHost;
            }
        }

        private void EditCurrentConfiguration()
        {
            if (coinListBox.SelectedIndex == -1)
                return;

            Coin currentConfiguration = configurations[coinListBox.SelectedIndex];

            if (currentConfiguration.PoolGroup.Kind == PoolGroup.PoolGroupKind.SingleCoin)
            {
                EditCurrentCoin(currentConfiguration);
            }
            else
            {
                EditCurrentMultipool(currentConfiguration);
            }
        }

        private void EditCurrentMultipool(Coin currentConfiguration)
        {
            using (MultipoolChooseForm multipoolChooseForm = new MultipoolChooseForm(currentConfiguration.PoolGroup))
            {
                DialogResult dialogResult = multipoolChooseForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    PoolGroup workingMultipool = multipoolChooseForm.SelectedMultipool;

                    Coin existingConfiguration =
                        configurations.SingleOrDefault(c => (c != currentConfiguration)
                            && c.PoolGroup.Id.Equals(workingMultipool.Id, StringComparison.OrdinalIgnoreCase));

                    if (existingConfiguration == null)
                    {
                        ObjectCopier.CopyObject(workingMultipool, currentConfiguration.PoolGroup);
                        coinListBox.Items[coinListBox.SelectedIndex] = workingMultipool.Name;
                    }
                    else
                    {
                        //don't create a dupe
                        MessageBox.Show(String.Format("A configuration for {0} already exists.", workingMultipool.Name),
                            "Duplicate Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void EditCurrentCoin(Coin currentConfiguration)
        {
            PoolGroup workingCoin = new PoolGroup();
            ObjectCopier.CopyObject(currentConfiguration.PoolGroup, workingCoin);

            using (CoinEditForm coinEditForm = new CoinEditForm(workingCoin))
            {
                DialogResult dialogResult = coinEditForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    Coin existingConfiguration =
                        configurations.SingleOrDefault(c => (c != currentConfiguration)
                            && c.PoolGroup.Id.Equals(workingCoin.Id, StringComparison.OrdinalIgnoreCase));

                    if (existingConfiguration == null)
                    {
                        ObjectCopier.CopyObject(workingCoin, currentConfiguration.PoolGroup);
                        coinListBox.Items[coinListBox.SelectedIndex] = workingCoin.Name;
                    }
                    else
                    {
                        //don't create a dupe
                        MessageBox.Show(String.Format("A configuration for {0} already exists.", workingCoin.Id),
                            "Duplicate Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void editCoinButton_Click(object sender, EventArgs e)
        {
            EditCurrentConfiguration();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Pools");
        }

        private void poolFeaturesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            extraNonceSubscriptToolStripMenuItem.Checked = hostEdit.Text.Contains(PoolFeatures.XNSubFragment);
            disableCoinbaseCheckToolStripMenuItem.Checked = hostEdit.Text.Contains(PoolFeatures.SkipCBCheckFragment);
        }

        private void extraNonceSubscriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdatePoolFeature(PoolFeatures.XNSubFragment, extraNonceSubscriptToolStripMenuItem.Checked);
        }

        private void featuresButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = featuresButton.PointToScreen(new Point(featuresButton.Left, featuresButton.Bottom));
            if (screenPoint.Y + poolFeaturesMenu.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
                poolFeaturesMenu.Show(featuresButton, new Point(0, -poolFeaturesMenu.Size.Height));
            else
                poolFeaturesMenu.Show(featuresButton, new Point(0, featuresButton.Height));    
        }

        private void disableCoinbaseCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdatePoolFeature(PoolFeatures.SkipCBCheckFragment, disableCoinbaseCheckToolStripMenuItem.Checked);
        }

        private void UpdatePoolFeature(string anchor, bool enabled)
        {
            string uriSegment = "/" + anchor;
            if (enabled)
                hostEdit.Text = hostEdit.Text + uriSegment;
            else
                hostEdit.Text = hostEdit.Text.Replace(uriSegment, String.Empty);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MultipoolChooseForm multipoolChooseForm = new MultipoolChooseForm();
            DialogResult dialogResult = multipoolChooseForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
                AddCoinConfiguration(multipoolChooseForm.SelectedMultipool);
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

        // begin port handling - don't databind as it crashes on macOS when an invalid value is entered (e.g. alpha or no value - delete port)
        private void portEdit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string portText = ((TextBox)sender).Text;
            e.Cancel = !ValidatePortText(portText);
        }

        private bool ValidatePortText(string portText)
        {
            int port;
            bool isValid = Int32.TryParse(portText, out port);
            if (isValid)
            {
                return true;
            }
            else
            {
                MessageBox.Show(String.Format("The specified value '{0}' is not a valid port.", portText), "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void portEdit_Validated(object sender, EventArgs e)
        {
            MiningPool currentPool = (MiningPool)miningPoolBindingSource.Current;
            if (currentPool == null)
            {
                return;
            }

            string portText = ((TextBox)sender).Text;
            currentPool.Port = Int32.Parse(portText); // already know it's valid from the Validating event
        }

        private void PopulatePort()
        {
            MiningPool currentPool = (MiningPool)miningPoolBindingSource.Current;
            if (currentPool == null)
            {
                return;
            }

            portEdit.Text = currentPool.Port.ToString();
        }

        // end port handling
    }
}
