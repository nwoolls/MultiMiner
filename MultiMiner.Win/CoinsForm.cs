using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class CoinsForm : Form
    {
        private List<CoinConfiguration> configurations = new List<CoinConfiguration>();
        private List<CryptoCoin> knownCoins;

        public CoinsForm(List<CoinConfiguration> configurations, List<CryptoCoin> knownCoins)
        {
            this.configurations = configurations;
            this.knownCoins = knownCoins;
            InitializeComponent();
        }

        private void CoinsForm_Load(object sender, EventArgs e)
        {
            PopulateConfigurations();
            UpdateButtonStates();
        }
        
        private void removeCoinButton_Click(object sender, EventArgs e)
        {
            DialogResult promptResult = MessageBox.Show("Remove the selected coin configuration?", "Confirm", MessageBoxButtons.YesNo);
            if (promptResult == System.Windows.Forms.DialogResult.Yes)
            {
                //required to clear bindings if this was the last coin in the list
                coinConfigurationBindingSource.DataSource = typeof(CoinConfiguration);
                miningPoolBindingSource.DataSource = typeof(MiningPool);

                CoinConfiguration configuration = configurations[coinListBox.SelectedIndex];
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

            foreach (CoinConfiguration configuration in configurations)
                coinListBox.Items.Add(configuration.Coin.Name);

            if (configurations.Count > 0)
                coinListBox.SelectedIndex = 0;
        }
        
        private void AddCoinConfiguration(CryptoCoin cryptoCoin)
        {
            CoinConfiguration configuration = configurations.SingleOrDefault(c => c.Coin.Symbol.Equals(cryptoCoin.Symbol));
            if (configuration != null)
            {
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }
            else
            {
                configuration = new CoinConfiguration();

                configuration.Coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(cryptoCoin.Symbol, StringComparison.OrdinalIgnoreCase));

                //user may have manually entered a coin
                if (configuration.Coin == null)
                {
                    configuration.Coin = new CryptoCoin();
                    configuration.Coin.Name = cryptoCoin.Name;
                    configuration.Coin.Symbol = cryptoCoin.Symbol;
                }

                configuration.Pools.Add(new MiningPool());

                configurations.Add(configuration);

                coinListBox.Items.Add(configuration.Coin.Name);
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }

            hostEdit.Focus();
        }

        private void coinListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (coinListBox.SelectedIndex >= 0)
            {
                CoinConfiguration configuration = configurations[coinListBox.SelectedIndex];

                coinConfigurationBindingSource.DataSource = configuration;
                miningPoolBindingSource.DataSource = configuration.Pools;
                poolListBox.DataSource = miningPoolBindingSource;
                poolListBox.DisplayMember = "Host";
            }

            UpdateButtonStates();
        }

        private void addPoolButton_Click(object sender, EventArgs e)
        {
            CoinConfiguration configuration = configurations[coinListBox.SelectedIndex];
            miningPoolBindingSource.Add(new MiningPool());
            poolListBox.SelectedIndex = configuration.Pools.Count - 1;
            hostEdit.Focus();
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
            removeCoinButton.Enabled = (coinListBox.SelectedIndex >= 0) && (coinListBox.SelectedIndex >= 0);
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

            CoinConfiguration currentConfiguration = (CoinConfiguration)coinConfigurationBindingSource.Current;
            currentConfiguration.ProfitabilityAdjustmentType = (CoinConfiguration.AdjustmentType)((ComboBox)sender).SelectedIndex;
        }

        private void coinConfigurationBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (coinConfigurationBindingSource.Current == null)
                return;

            CoinConfiguration currentConfiguration = (CoinConfiguration)coinConfigurationBindingSource.Current;
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

        private void textBox4_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = String.IsNullOrEmpty(userNameEdit.Text);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                userNameEdit.Focus();
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            if (configurations.Count > 0)
            {
                DialogResult warningResult = MessageBox.Show("Importing will overwrite your existing coin and pool configurations. Do you want to continue?",
                                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (warningResult == System.Windows.Forms.DialogResult.No)
                    return;
            }

            openFileDialog1.FileName = "CoinConfigurations.xml";
            openFileDialog1.Title = "Import CoinConfigurations.xml";
            openFileDialog1.Filter = "XML files|*.xml";
            openFileDialog1.DefaultExt = ".xml";

            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                string sourceFileName = openFileDialog1.FileName;
                string destinationFileName = EngineConfiguration.CoinConfigurationsFileName();
                if (File.Exists(destinationFileName))
                    File.Delete(destinationFileName);
                File.Copy(sourceFileName, destinationFileName);
                
                List<CoinConfiguration> coinConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<CoinConfiguration>>(destinationFileName);
                configurations.Clear();
                foreach (CoinConfiguration coinConfiguration in coinConfigurations)
                    configurations.Add(coinConfiguration);

                PopulateConfigurations();
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
                string sourceFileName = EngineConfiguration.CoinConfigurationsFileName();
                string destinationFileName = saveFileDialog1.FileName;
                if (File.Exists(destinationFileName))
                    File.Delete(destinationFileName);
                File.Copy(sourceFileName, destinationFileName);
            }
        }
    }
}
