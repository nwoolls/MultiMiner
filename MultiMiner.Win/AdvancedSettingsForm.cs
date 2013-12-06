using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Win.Configuration;
using MultiMiner.Xgminer;
using System;

namespace MultiMiner.Win
{
    public partial class AdvancedSettingsForm : MessageBoxFontForm
    {
        private readonly XgminerConfiguration minerConfiguration;
        private readonly XgminerConfiguration workingMinerConfiguration;

        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly ApplicationConfiguration workingApplicationConfiguration;

        public AdvancedSettingsForm(XgminerConfiguration minerConfiguration, ApplicationConfiguration applicationConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
            this.workingMinerConfiguration = ObjectCopier.CloneObject<XgminerConfiguration, XgminerConfiguration>(minerConfiguration);

            this.applicationConfiguration = applicationConfiguration;
            this.workingApplicationConfiguration = ObjectCopier.CloneObject<ApplicationConfiguration, ApplicationConfiguration>(applicationConfiguration);
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            xgminerConfigurationBindingSource.DataSource = workingMinerConfiguration;
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;
            autoDesktopCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
            LoadSettings();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            ObjectCopier.CopyObject(workingMinerConfiguration, minerConfiguration);
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void LoadSettings()
        {
            intervalCombo.SelectedIndex = (int)workingApplicationConfiguration.ScheduledRestartMiningInterval;

            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.SHA256))
                sha256ParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256];
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.Scrypt))
                scryptParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt];

            autoDesktopCheckBox.Enabled = !disableGpuCheckbox.Checked;
        }

        private void SaveSettings()
        {
            workingApplicationConfiguration.ScheduledRestartMiningInterval = (ApplicationConfiguration.TimerInterval)intervalCombo.SelectedIndex;

            minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256] = sha256ParamsEdit.Text;
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt] = scryptParamsEdit.Text;
        }

        private void disableGpuCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            autoDesktopCheckBox.Enabled = !disableGpuCheckbox.Checked;
        }
    }
}
