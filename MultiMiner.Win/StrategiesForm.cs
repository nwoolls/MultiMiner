using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using System;
using System.Windows.Forms;
using System.Linq;

namespace MultiMiner.Win
{
    public partial class StrategiesForm : Form
    {
        private readonly KnownCoins knownCoins = new KnownCoins();
        private readonly StrategyConfiguration strategyConfiguration;

        public StrategiesForm(StrategyConfiguration strategyConfiguration)
        {
            InitializeComponent();
            this.strategyConfiguration = strategyConfiguration;
        }

        private void StrategiesForm_Load(object sender, EventArgs e)
        {
            strategyConfigurationBindingSource.DataSource = this.strategyConfiguration;
            PopulateKnownCoins();
            LoadSettings();
        }

        private void PopulateKnownCoins()
        {
            foreach (CryptoCoin coin in knownCoins.Coins)
            {
                minCoinCombo.Items.Add(coin.Name);
                permCoinCombo.Items.Add(coin.Name);
            }
            minCoinCombo.Items.Add("");
            permCoinCombo.Items.Add("");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            CryptoCoin coin = knownCoins.Coins.SingleOrDefault(c => c.Symbol.Equals(this.strategyConfiguration.MinimumProfitabilitySymbol));
            if (coin != null)
                minCoinCombo.Text = coin.Name;

            coin = knownCoins.Coins.SingleOrDefault(c => c.Symbol.Equals(this.strategyConfiguration.PermanentCoinSymbol));
            if (coin != null)
                permCoinCombo.Text = coin.Name;
        }

        private void SaveSettings()
        {
            CryptoCoin coin = knownCoins.Coins.SingleOrDefault(c => c.Name.Equals(minCoinCombo.Text));
            if (coin != null)
                this.strategyConfiguration.MinimumProfitabilitySymbol = coin.Symbol;

            coin = knownCoins.Coins.SingleOrDefault(c => c.Name.Equals(permCoinCombo.Text));
            if (coin != null)
                this.strategyConfiguration.PermanentCoinSymbol = coin.Symbol;
        }
    }

}
