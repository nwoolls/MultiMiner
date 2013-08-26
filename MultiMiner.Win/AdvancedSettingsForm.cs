using MultiMiner.Engine.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class AdvancedSettingsForm : Form
    {
        XgminerConfiguration minerConfiguration;

        public AdvancedSettingsForm(XgminerConfiguration minerConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            disableGpuCheckbox.Checked = minerConfiguration.DisableGpu;
            erupterCheckBox.Checked = minerConfiguration.ErupterDriver;
            erupterCheckBox.Enabled = minerConfiguration.MinerBackend == Xgminer.MinerBackend.Bfgminer;
        }

        private void SaveSettings()
        {
            minerConfiguration.DisableGpu = disableGpuCheckbox.Checked;
            minerConfiguration.ErupterDriver = erupterCheckBox.Checked;
        }
    }
}
