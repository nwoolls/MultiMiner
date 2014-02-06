using MultiMiner.Engine;
using MultiMiner.Utility.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

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
                coinCombo.Items.Add(String.Format("{0} ({1})", sortedCoin.Name, sortedCoin.Symbol));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            CryptoCoin knownCoin = null;
            if (coinCombo.SelectedIndex >= 0)
                knownCoin = sortedCoins[coinCombo.SelectedIndex];
            
            if (knownCoin == null)
            {
                CryptoCoin newCoin = new CryptoCoin();
                newCoin.Name = coinCombo.Text;
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
