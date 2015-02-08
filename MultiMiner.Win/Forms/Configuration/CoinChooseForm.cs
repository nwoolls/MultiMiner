using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Extensions;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class CoinChooseForm : MessageBoxFontForm
    {
        public PoolGroup SelectedCoin { get; set; }

        private List<PoolGroup> filteredCoins;
        private readonly List<PoolGroup> sortedCoins;

        public CoinChooseForm(List<PoolGroup> knownCoins)
        {
            InitializeComponent();
            sortedCoins = knownCoins.OrderBy(c => c.Name).ToList();
        }

        private void CoinChooseForm_Load(object sender, EventArgs e)
        {
            PopulateAlgorithmCombo();
            algoCombo.Text = ApplicationViewModel.PrimaryAlgorithm;

            coinCombo.Focus();
        }

        private void PopulateAlgorithmCombo()
        {
            algoCombo.Items.Clear();
            List<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
                algoCombo.Items.Add(algorithm.Name.ToSpaceDelimitedWords());           
        }

        private CoinAlgorithm GetSelectedAlgorithm()
        {
            string algorithmName = algoCombo.Text.Replace(" ", String.Empty);
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            return algorithm;
        }

        private void PopulateCoinCombo()
        {
            CoinAlgorithm algorithm = GetSelectedAlgorithm();
            filteredCoins = sortedCoins.Where(sc => (sc.Kind == PoolGroup.PoolGroupKind.SingleCoin) 
                && sc.Algorithm.Equals(algorithm.FullName, StringComparison.OrdinalIgnoreCase)).ToList();

            coinCombo.Text = String.Empty;
            coinCombo.Items.Clear();
            foreach (PoolGroup sortedCoin in filteredCoins)
                coinCombo.Items.Add(String.Format("{0} ({1})", sortedCoin.Name, sortedCoin.Id));

            if (coinCombo.Items.Count > 0)
                coinCombo.SelectedIndex = 0;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            PoolGroup knownCoin = null;
            if (coinCombo.SelectedIndex >= 0)
                knownCoin = filteredCoins[coinCombo.SelectedIndex];
            
            if (knownCoin == null)
            {
                PoolGroup newCoin = new PoolGroup();
                newCoin.Name = coinCombo.Text;
                newCoin.Algorithm = GetSelectedAlgorithm().Name;

                using (CoinEditForm coinEditForm = new CoinEditForm(newCoin))
                {
                    if (coinEditForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        SelectedCoin = newCoin;
                    else
                        DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
            }
            else
            {
                SelectedCoin = knownCoin;
            }
        }

        private void algoCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateCoinCombo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Crypto-Coins");
        }
    }
}
