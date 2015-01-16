using MultiMiner.Engine.Data.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.Engine.Data;
using System.Diagnostics;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class StrategiesForm : MessageBoxFontForm
    {
        private readonly List<PoolGroup> knownCoins;
        private readonly Strategy strategyConfiguration;
        private readonly Application applicationConfiguration;

        public StrategiesForm(Strategy strategyConfiguration, List<PoolGroup> knownCoins, 
            Application applicationConfiguration)
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
            foreach (PoolGroup coin in knownCoins)
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
            PoolGroup coin = knownCoins.SingleOrDefault(c => c.Id.Equals(this.strategyConfiguration.MinimumThresholdSymbol));
            if (coin != null)
                thresholdSymbolCombo.Text = coin.Name;
            
            singleCoinRadio.Checked = strategyConfiguration.SwitchStrategy == Strategy.CoinSwitchStrategy.SingleMost;
            multiCoinRadio.Checked = strategyConfiguration.SwitchStrategy == Strategy.CoinSwitchStrategy.AllMost;

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
                PoolGroup coin = knownCoins.SingleOrDefault(c => c.Name.Equals(thresholdSymbolCombo.Text));
                if (coin != null)
                    this.strategyConfiguration.MinimumThresholdSymbol = coin.Id;
            }

            if (singleCoinRadio.Checked)
                strategyConfiguration.SwitchStrategy = Strategy.CoinSwitchStrategy.SingleMost;
            else
                strategyConfiguration.SwitchStrategy = Strategy.CoinSwitchStrategy.AllMost;

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

            strategyConfiguration.ProfitabilityKind = (Strategy.CoinProfitabilityKind)profitabilityKindCombo.SelectedIndex;
            strategyConfiguration.MiningBasis = (Strategy.CoinMiningBasis)miningBasisCombo.SelectedIndex;
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
            Strategy.CoinMiningBasis miningBasis = (Strategy.CoinMiningBasis)miningBasisCombo.SelectedIndex;
            switch (miningBasis)
            {
                case Strategy.CoinMiningBasis.Profitability:
                    profitabilityKindCombo.Enabled = true;
                    thresholdSymbolLabel.Text = "Don't mine coins less profitable than";
                    singleCoinRadio.Text = "Mine only the single most profitable coin";
                    multiCoinRadio.Text = "Mine all of the most profitable coins";
                    mineSingleOverrideLabel.Text = "Mine a single coin if it exceeds";
                    break;
                case Strategy.CoinMiningBasis.Difficulty:
                    profitabilityKindCombo.Enabled = false;
                    thresholdSymbolLabel.Text = "Don't mine coins more difficult than";
                    singleCoinRadio.Text = "Mine only the single least difficult coin";
                    multiCoinRadio.Text = "Mine all of the least difficult coins";
                    mineSingleOverrideLabel.Text = "Mine a single coin if it falls below";
                    break;
                case Strategy.CoinMiningBasis.Price:
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

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Auto-Mining-Strategies");
        }
    }

}
