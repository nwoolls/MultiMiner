using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using MultiMiner.Win.Configuration;

namespace MultiMiner.Win
{
    public partial class StrategiesForm : MessageBoxFontForm
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
            applicationConfigurationBindingSource.DataSource = this.applicationConfiguration;
            PopulateKnownCoins();
            LoadSettings();
        }

        private void PopulateKnownCoins()
        {
            foreach (CryptoCoin coin in knownCoins)
            {
                thresholdSymbolCombo.Items.Add(coin.Name);
            }
            thresholdSymbolCombo.Items.Add("");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(this.strategyConfiguration.MinimumThresholdSymbol));
            if (coin != null)
                thresholdSymbolCombo.Text = coin.Name;
            
            singleCoinRadio.Checked = strategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMost;
            multiCoinRadio.Checked = strategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.AllMost;

            thresholdValueEdit.Text = strategyConfiguration.MinimumThresholdValue.ToString();

            if (multiCoinRadio.Checked)
                exceptionEdit.Text = strategyConfiguration.MineSingleMostOverrideValue.ToString();

            profitabilityKindCombo.SelectedIndex = (int)strategyConfiguration.ProfitabilityKind;
                        
            miningBasisCombo.SelectedIndex = (int)strategyConfiguration.MiningBasis;
        }

        private void SaveSettings()
        {
            if (string.IsNullOrEmpty(thresholdSymbolCombo.Text))
                this.strategyConfiguration.MinimumThresholdSymbol = string.Empty;
            else
            {
                CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Name.Equals(thresholdSymbolCombo.Text));
                if (coin != null)
                    this.strategyConfiguration.MinimumThresholdSymbol = coin.Symbol;
            }

            if (singleCoinRadio.Checked)
                strategyConfiguration.SwitchStrategy = StrategyConfiguration.CoinSwitchStrategy.SingleMost;
            else
                strategyConfiguration.SwitchStrategy = StrategyConfiguration.CoinSwitchStrategy.AllMost;

            if (string.IsNullOrEmpty(thresholdValueEdit.Text))
                this.strategyConfiguration.MinimumThresholdValue = null;
            else
            {
                double percentage;
                if (double.TryParse(thresholdValueEdit.Text, out percentage))
                    strategyConfiguration.MinimumThresholdValue = percentage;
            }

            if (multiCoinRadio.Checked)
            {
                if (string.IsNullOrEmpty(exceptionEdit.Text))
                    this.strategyConfiguration.MineSingleMostOverrideValue = null;
                else
                {
                    double percentage;
                    if (double.TryParse(exceptionEdit.Text, out percentage))
                        strategyConfiguration.MineSingleMostOverrideValue = percentage;
                }
            }

            strategyConfiguration.ProfitabilityKind = (StrategyConfiguration.CoinProfitabilityKind)profitabilityKindCombo.SelectedIndex;
            strategyConfiguration.MiningBasis = (StrategyConfiguration.CoinMiningBasis)miningBasisCombo.SelectedIndex;
        }

        private void multiCoinRadio_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExceptionControls();
        }

        private void RefreshExceptionControls()
        {
            exceptionEdit.Enabled = multiCoinRadio.Checked;
            mineSingleOverrideLabel.Enabled = multiCoinRadio.Checked;
        }

        private void miningBasisCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsForMiningBasis();
        }

        private void UpdateControlsForMiningBasis()
        {
            StrategyConfiguration.CoinMiningBasis miningBasis = (StrategyConfiguration.CoinMiningBasis)miningBasisCombo.SelectedIndex;
            switch (miningBasis)
            {
                case StrategyConfiguration.CoinMiningBasis.Profitability:
                    profitabilityKindCombo.Enabled = true;
                    thresholdSymbolLabel.Text = "Don't mine coins less profitable than";
                    singleCoinRadio.Text = "Mine only the single most profitable coin";
                    multiCoinRadio.Text = "Mine all of the most profitable coins";
                    mineSingleOverrideLabel.Text = "Mine a single coin if it exceeds";
                    break;
                case StrategyConfiguration.CoinMiningBasis.Difficulty:
                    profitabilityKindCombo.Enabled = false;
                    thresholdSymbolLabel.Text = "Don't mine coins more difficult than";
                    singleCoinRadio.Text = "Mine only the single least difficult coin";
                    multiCoinRadio.Text = "Mine all of the least difficult coins";
                    mineSingleOverrideLabel.Text = "Mine a single coin if it falls below";
                    break;
                case StrategyConfiguration.CoinMiningBasis.Price:
                    profitabilityKindCombo.Enabled = false;
                    thresholdSymbolLabel.Text = "Don't mine coins less valuable than";
                    singleCoinRadio.Text = "Mine only the single most valuable coin";
                    multiCoinRadio.Text = "Mine all of the most valuable coins";
                    mineSingleOverrideLabel.Text = "Mine a single coin if it exceeds";
                    break;
            }

            profitabilityKindLabel.Enabled = profitabilityKindCombo.Enabled;
            thresholdValueLabel.Text = thresholdSymbolLabel.Text;
        }
    }

}
