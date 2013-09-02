using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
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
            PopulateKnownCoins();
            PopulateConfigurations();
            UpdateButtonStates();
        }

        private void PopulateKnownCoins()
        {
            foreach (CryptoCoin coin in knownCoins)
            {
                ToolStripButton coinButton = new ToolStripButton(coin.Name);
                coinButton.Tag = coin.Symbol;
                addCoinDropDown.DropDownItems.Add(coinButton);
                coinButton.Click += HandleCoinButtonClick;
            }
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
            foreach (CoinConfiguration configuration in configurations)
            {
                coinListBox.Items.Add(configuration.Coin.Name);
            }

            if (configurations.Count > 0)
                coinListBox.SelectedIndex = 0;
        }

        private void HandleCoinButtonClick(object sender, EventArgs e)
        {
            string clickedSymbol = (string)((ToolStripButton)sender).Tag;
            AddCoinConfiguration(clickedSymbol);          
        }

        private void AddCoinConfiguration(string coinSymbol)
        {
            CoinConfiguration configuration = configurations.SingleOrDefault(c => c.Coin.Symbol.Equals(coinSymbol));
            if (configuration != null)
            {
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }
            else
            {
                configuration = new CoinConfiguration();

                configuration.Coin = knownCoins.Single(c => c.Symbol.Equals(coinSymbol));
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
            {
                string symbolToAdd = coinChooseForm.SelectedSymbol;
                AddCoinConfiguration(symbolToAdd);
            }
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
    }
}
