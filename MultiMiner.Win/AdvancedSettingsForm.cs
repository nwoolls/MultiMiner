using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using System;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class AdvancedSettingsForm : Form
    {
        readonly XgminerConfiguration minerConfiguration;
        readonly XgminerConfiguration workingConfiguration;

        public AdvancedSettingsForm(XgminerConfiguration minerConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
            this.workingConfiguration = ObjectCopier.CloneObject<XgminerConfiguration, XgminerConfiguration>(minerConfiguration);
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            xgminerConfigurationBindingSource.DataSource = workingConfiguration;
            erupterCheckBox.Enabled = minerConfiguration.MinerBackend == Xgminer.MinerBackend.Bfgminer;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ObjectCopier.CopyObject(workingConfiguration, minerConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
