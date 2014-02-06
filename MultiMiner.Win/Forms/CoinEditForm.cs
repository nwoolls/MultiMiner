using MultiMiner.Engine;
using MultiMiner.Utility.Forms;
using MultiMiner.Xgminer;
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
            LoadSettings();
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
            sha256RadioButton.Checked = cryptoCoin.Algorithm == CoinAlgorithm.SHA256;
            scryptRadioButton.Checked = cryptoCoin.Algorithm == CoinAlgorithm.Scrypt;
            cryptoCoinBindingSource.DataSource = cryptoCoin;

        }

        private void SaveSettings()
        {
            cryptoCoin.Algorithm = sha256RadioButton.Checked ? CoinAlgorithm.SHA256 : CoinAlgorithm.Scrypt;
        }
    }
}
