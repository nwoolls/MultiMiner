using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace MultiMiner.Win
{
    public partial class StrategiesForm : Form
    {
        private readonly List<CryptoCoin> knownCoins;
        private readonly StrategyConfiguration strategyConfiguration;

        public StrategiesForm(StrategyConfiguration strategyConfiguration, List<CryptoCoin> knownCoins)
        {
            InitializeComponent();
            this.strategyConfiguration = strategyConfiguration;
            this.knownCoins = knownCoins;
        }

        private void StrategiesForm_Load(object sender, EventArgs e)
        {
            strategyConfigurationBindingSource.DataSource = this.strategyConfiguration;
            PopulateKnownCoins();
            LoadSettings();
        }

        private void PopulateKnownCoins()
        {
            foreach (CryptoCoin coin in knownCoins)
            {
                minCoinCombo.Items.Add(coin.Name);
            }
            minCoinCombo.Items.Add("");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(this.strategyConfiguration.MinimumProfitabilitySymbol));
            if (coin != null)
                minCoinCombo.Text = coin.Name;
            
            singleCoinRadio.Checked = strategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMostProfitable;
            multiCoinRadio.Checked = strategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.AllMostProfitable;

            minPercentageEdit.Text = strategyConfiguration.MinimumProfitabilityPercentage.ToString();
        }

        private void SaveSettings()
        {
            if (string.IsNullOrEmpty(minCoinCombo.Text))
                this.strategyConfiguration.MinimumProfitabilitySymbol = string.Empty;
            else
            {
                CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Name.Equals(minCoinCombo.Text));
                if (coin != null)
                    this.strategyConfiguration.MinimumProfitabilitySymbol = coin.Symbol;
            }

            if (singleCoinRadio.Checked)
                strategyConfiguration.SwitchStrategy = StrategyConfiguration.CoinSwitchStrategy.SingleMostProfitable;
            else
                strategyConfiguration.SwitchStrategy = StrategyConfiguration.CoinSwitchStrategy.AllMostProfitable;

            if (string.IsNullOrEmpty(minPercentageEdit.Text))
                this.strategyConfiguration.MinimumProfitabilityPercentage = null;
            else
            {
                double percentage;
                if (double.TryParse(minPercentageEdit.Text, out percentage))
                    strategyConfiguration.MinimumProfitabilityPercentage = percentage;
            }
        }
    }

}
