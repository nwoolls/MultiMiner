using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            if (!ValidateInput())
                return;

            SaveSettings();

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private bool ValidateInput()
        {
            if (emailAddressEdit.Enabled && !String.IsNullOrEmpty(emailAddressEdit.Text) &&
                !InputValidation.IsValidEmailAddress(emailAddressEdit.Text))
            {
                emailAddressEdit.Focus();
                MessageBox.Show("Please specify a valid email address. This must be the same address used to register MobileMiner.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (appKeyEdit.Enabled && !String.IsNullOrEmpty(appKeyEdit.Text) &&
                !InputValidation.IsValidApplicationKey(appKeyEdit.Text))
            {
                appKeyEdit.Focus();
                MessageBox.Show("Please specify a valid application key. If you are unsure, copy and paste the application key from the email you received.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        
        private void SaveSettings()
        {
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256] = sha256ParamsEdit.Text;
            minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt] = scryptParamsEdit.Text;

            if (minerCombo.SelectedIndex == 0)
                minerConfiguration.MinerBackend = MinerBackend.Cgminer;
            else
                minerConfiguration.MinerBackend = MinerBackend.Bfgminer;

            minerConfiguration.DisableGpu = disableGpuCheckbox.Checked;

            minerConfiguration.Priority = (ProcessPriorityClass)priorityCombo.SelectedItem;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            PopulatePriorities(); //populate before loading settings
            LoadSettings();
            autoLaunchCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
            sysTrayCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
        }

        private void PopulatePriorities()
        {
            Array possibleProprties = Enum.GetValues(typeof(ProcessPriorityClass));
            priorityCombo.DataSource = possibleProprties;
            priorityCombo.SelectedItem = ProcessPriorityClass.Normal;
        }

        private void LoadSettings()
        {
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.SHA256))
                sha256ParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.SHA256];
            if (minerConfiguration.AlgorithmFlags.ContainsKey(CoinAlgorithm.Scrypt))
                scryptParamsEdit.Text = minerConfiguration.AlgorithmFlags[CoinAlgorithm.Scrypt];

            applicationConfigurationBindingSource.DataSource = this.applicationConfiguration;

            if (minerConfiguration.MinerBackend == MinerBackend.Cgminer)
                minerCombo.SelectedIndex = 0;
            else
                minerCombo.SelectedIndex = 1;

            disableGpuCheckbox.Checked = minerConfiguration.DisableGpu;

            priorityCombo.SelectedItem = minerConfiguration.Priority;
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
            Process.Start("http://mobileminerapp.com/");
        }
    }
}
