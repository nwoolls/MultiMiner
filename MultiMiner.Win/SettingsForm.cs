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

            if (cgminerRadio.Checked)
                minerConfiguration.MinerBackend = MinerBackend.Cgminer;
            else
                minerConfiguration.MinerBackend = MinerBackend.Bfgminer;

            minerConfiguration.DisableGpu = disableGpuCheckbox.Checked;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettings();

            autoLaunchCheckBox.Visible = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
        }

        private void LoadSettings()
        {
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.SHA256))
                sha256ParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256];
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.Scrypt))
                scryptParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt];

            applicationConfigurationBindingSource.DataSource = this.applicationConfiguration;

            cgminerRadio.Checked = minerConfiguration.MinerBackend == MinerBackend.Cgminer;
            bfgminerRadio.Checked = minerConfiguration.MinerBackend == MinerBackend.Bfgminer;

            disableGpuCheckbox.Checked = minerConfiguration.DisableGpu;
        }

        private void remoteMonitoringCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMobileMinerEdits();
        }

        private void remoteCommandsCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMobileMinerEdits();
        }
        private void UpdateMobileMinerEdits()
        {
            emailAddressEdit.Enabled = remoteMonitoringCheck.Checked || remoteCommandsCheck.Checked;
            appKeyEdit.Enabled = emailAddressEdit.Enabled;
        }

        private void mobileMinerInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("MobileMiner is a mobile app, available soon on the Apple App Store, that allows you to remotely monitor and command your mining rigs. \n\nRemote commands are limited to \"start\", \"stop\", and \"restart\" for security reasons.", "MobileMiner", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
