using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class SettingsForm : Form
    {
        private readonly XgminerConfiguration minerConfiguration;
        private readonly ApplicationConfiguration applicationConfiguration;

        public SettingsForm(ApplicationConfiguration applicationConfiguration, XgminerConfiguration minerConfiguration)
        {
            InitializeComponent();

            this.minerConfiguration = minerConfiguration;
            this.applicationConfiguration = applicationConfiguration;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256] = sha256ParamsEdit.Text;
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt] = scryptParamsEdit.Text;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.SHA256))
                sha256ParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256];
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.Scrypt))
                scryptParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt];

            applicationConfigurationBindingSource.DataSource = this.applicationConfiguration;
        }
    }
}
