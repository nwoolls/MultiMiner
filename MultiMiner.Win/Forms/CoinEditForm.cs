using MultiMiner.Engine.Data;
using MultiMiner.Utility.Forms;
using MultiMiner.Xgminer.Data;
using MultiMiner.UX.Extensions;
using System;

namespace MultiMiner.Win.Forms
{
    public partial class CoinEditForm : MessageBoxFontForm
    {
        private readonly CryptoCoin cryptoCoin;
        public CoinEditForm(CryptoCoin cryptoCoin)
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
            foreach (CoinAlgorithm algorithm in (CoinAlgorithm[])Enum.GetValues(typeof(CoinAlgorithm)))
                algorithmCombo.Items.Add(algorithm.ToString().ToSpaceDelimitedWords());
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
            if (String.IsNullOrEmpty(cryptoCoin.Symbol))
            {
                symbolEdit.Focus();
                return false;
            }
            return true;
        }

        private void LoadSettings()
        {
            algorithmCombo.SelectedIndex = (int)cryptoCoin.Algorithm;

            cryptoCoinBindingSource.DataSource = cryptoCoin;

        }

        private void SaveSettings()
        {
            cryptoCoin.Algorithm = (CoinAlgorithm)algorithmCombo.SelectedIndex;
        }
    }
}
