using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Utility.Forms;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.Win.Data.Configuration;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Data;
using System;
using System.Diagnostics;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class MinerSettingsForm : MessageBoxFontForm
    {
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration;
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer workingMinerConfiguration;

        private readonly Application applicationConfiguration;
        private readonly Application workingApplicationConfiguration;

        public MinerSettingsForm(MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration, Application applicationConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
            this.workingMinerConfiguration = ObjectCopier.CloneObject<MultiMiner.Engine.Data.Configuration.Xgminer, MultiMiner.Engine.Data.Configuration.Xgminer>(minerConfiguration);

            this.applicationConfiguration = applicationConfiguration;
            this.workingApplicationConfiguration = ObjectCopier.CloneObject<Application, Application>(applicationConfiguration);
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
            //if the user has disabled Auto-Set Dynamic Intensity, disable Dynamic Intensity as well
            if (!workingApplicationConfiguration.AutoSetDesktopMode &&
                (workingApplicationConfiguration.AutoSetDesktopMode != applicationConfiguration.AutoSetDesktopMode))
                workingMinerConfiguration.DesktopMode = false;

            workingApplicationConfiguration.ScheduledRestartMiningInterval = (Application.TimerInterval)intervalCombo.SelectedIndex;

            minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256] = sha256ParamsEdit.Text;
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt] = scryptParamsEdit.Text;
        }

        private void disableGpuCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            autoDesktopCheckBox.Enabled = !disableGpuCheckbox.Checked;
        }

        private void scryptConfigLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://litecoin.info/Mining_hardware_comparison#GPU");
        }
    }
}
