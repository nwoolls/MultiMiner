using MultiMiner.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class CoinChooseForm : MessageBoxFontForm
    {
        public CryptoCoin SelectedCoin { get; set; }

        private List<CryptoCoin> sortedCoins;

        public CoinChooseForm(List<CryptoCoin> knownCoins)
        {
            InitializeComponent();
            sortedCoins = knownCoins.OrderBy(c => c.Name).ToList();
        }

        private void CoinChooseForm_Load(object sender, EventArgs e)
        {
            PopulateCoinCombo();
            if (coinCombo.Items.Count > 0)
                coinCombo.SelectedIndex = 0;
            coinCombo.Focus();
        }

        private void PopulateCoinCombo()
        {
            foreach (CryptoCoin sortedCoin in sortedCoins)
                coinCombo.Items.Add(sortedCoin.Name);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string coinName = coinCombo.Text;
            CryptoCoin knownCoin = sortedCoins.SingleOrDefault(c => c.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));

            if (knownCoin == null)
            {
                CryptoCoin newCoin = new CryptoCoin();
                newCoin.Name = coinName;
                newCoin.Algorithm = Xgminer.CoinAlgorithm.SHA256;

                CoinEditForm coinEditForm = new CoinEditForm(newCoin);
                if (coinEditForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    SelectedCoin = newCoin;
                else
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                SelectedCoin = knownCoin;
            }
        }
    }
}
