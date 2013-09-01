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
        private readonly ApplicationConfiguration applicationConfiguration;

        public StrategiesForm(StrategyConfiguration strategyConfiguration, List<CryptoCoin> knownCoins, 
            ApplicationConfiguration applicationConfiguration)
        {
            InitializeComponent();
            this.strategyConfiguration = strategyConfiguration;
            this.applicationConfiguration = applicationConfiguration;
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

            if (multiCoinRadio.Checked)
                exceptionEdit.Text = strategyConfiguration.MineMostProfitableOverridePercentage.ToString();

            proftabilityBasisCombo.SelectedIndex = (int)strategyConfiguration.ProfitabilityBasis;
            baseCoinCombo.SelectedIndex = (int)strategyConfiguration.BaseCoin;

            intervalCombo.SelectedIndex = (int)applicationConfiguration.StrategyCheckInterval;
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

            if (multiCoinRadio.Checked)
            {
                if (string.IsNullOrEmpty(exceptionEdit.Text))
                    this.strategyConfiguration.MineMostProfitableOverridePercentage = null;
                else
                {
                    double percentage;
                    if (double.TryParse(exceptionEdit.Text, out percentage))
                        strategyConfiguration.MineMostProfitableOverridePercentage = percentage;
                }
            }

            strategyConfiguration.ProfitabilityBasis = (StrategyConfiguration.CoinProfitabilityBasis)proftabilityBasisCombo.SelectedIndex;
            strategyConfiguration.BaseCoin = (Coinchoose.Api.BaseCoin)baseCoinCombo.SelectedIndex;

            applicationConfiguration.StrategyCheckInterval = (ApplicationConfiguration.CoinStrategyCheckInterval)intervalCombo.SelectedIndex;
        }

        private void multiCoinRadio_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExceptionControls();
        }

        private void RefreshExceptionControls()
        {
            exceptionEdit.Enabled = multiCoinRadio.Checked;
            exceptionLabel.Enabled = multiCoinRadio.Checked;
            exceptPercentLabel.Enabled = multiCoinRadio.Checked;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
    }

}
