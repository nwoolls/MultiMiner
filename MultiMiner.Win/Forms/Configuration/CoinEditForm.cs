using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Extensions;
using MultiMiner.Xgminer.Data;
using System;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class CoinEditForm : MessageBoxFontForm
    {
        private readonly PoolGroup cryptoCoin;
        public CoinEditForm(PoolGroup cryptoCoin)
        {
            InitializeComponent();
            this.cryptoCoin = cryptoCoin;
        }

        private void CoinEditForm_Load(object sender, EventArgs e)
        {
            PopulateAlgorithmCombo();
            LoadSettings();
        }

        private void PopulateAlgorithmCombo()
        {
            algorithmCombo.Items.Clear();
            System.Collections.Generic.List<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
                algorithmCombo.Items.Add(algorithm.Name.ToSpaceDelimitedWords());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            SaveSettings();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private bool ValidateInput()
        {
            //require a symbol be specified, symbol is used throughout the app
            if (String.IsNullOrEmpty(cryptoCoin.Id))
            {
                symbolEdit.Focus();
                return false;
            }
            return true;
        }

        private void LoadSettings()
        {
            algorithmCombo.Text = cryptoCoin.Algorithm.ToSpaceDelimitedWords();

            cryptoCoinBindingSource.DataSource = cryptoCoin;

        }

        private void SaveSettings()
        {
            cryptoCoin.Algorithm = algorithmCombo.Text.Replace(" ", String.Empty);
        }
    }
}
