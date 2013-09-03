using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using System;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class AdvancedSettingsForm : Form
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
            erupterCheckBox.Enabled = minerConfiguration.MinerBackend == Xgminer.MinerBackend.Bfgminer;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ObjectCopier.CopyObject(workingMinerConfiguration, minerConfiguration);
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
