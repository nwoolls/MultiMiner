using MultiMiner.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class CoinsForm : Form
    {
        private List<CoinConfiguration> configurations = new List<CoinConfiguration>();
        private KnownCoins knownCoins = new KnownCoins();

        public CoinsForm()
        {
            InitializeComponent();
        }

        private void CoinsForm_Load(object sender, EventArgs e)
        {
            PopulateKnownCoins();
            PopulateConfigurations();
        }

        private void PopulateKnownCoins()
        {
            foreach (CryptoCoin coin in knownCoins.Coins)
            {
                ToolStripButton coinButton = new ToolStripButton(coin.Name);
                coinButton.Tag = coin.Symbol;
                addCoinButton.DropDownItems.Add(coinButton);
                coinButton.Click += HandleCoinButtonClick;
            }
        }

        private void removeCoinButton_Click(object sender, EventArgs e)
        {

        }

        private void PopulateConfigurations()
        {
            foreach (CoinConfiguration configuration in configurations)
            {
                coinListBox.Items.Add(configuration.CryptoCoin.Name);
            }
        }

        private void HandleCoinButtonClick(object sender, EventArgs e)
        {
            string clickedSymbol = (string)((ToolStripButton)sender).Tag;
            CoinConfiguration configuration = configurations.SingleOrDefault(c => c.CryptoCoin.Symbol.Equals(clickedSymbol));
            if (configuration != null)
            {
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }
            else
            {
                configuration = new CoinConfiguration();

                configuration.CryptoCoin = knownCoins.Coins.Single(c => c.Symbol.Equals(clickedSymbol));
                configuration.Pools.Add(new MiningPool());

                configurations.Add(configuration);

                coinListBox.Items.Add(configuration.CryptoCoin.Name);
                coinListBox.SelectedIndex = configurations.IndexOf(configuration);
            }

            hostEdit.Focus();            
        }

        private void coinListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CoinConfiguration configuration = configurations[coinListBox.SelectedIndex];
            miningPoolBindingSource.DataSource = configuration.Pools;
            poolListBox.DataSource = miningPoolBindingSource;
            poolListBox.DisplayMember = "Host";
        }
    }
}
