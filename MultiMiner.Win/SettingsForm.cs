using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class SettingsForm : MessageBoxFontForm
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

            minerConfiguration.Priority = (ProcessPriorityClass)priorityCombo.SelectedItem;

            applicationConfiguration.UseCoinWarzApi = coinApiCombo.SelectedIndex == 1;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            PopulatePriorities(); //populate before loading settings
            LoadSettings();
            autoLaunchCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
            sysTrayCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;

            coinApiCombo.SelectedIndex = applicationConfiguration.UseCoinWarzApi ? 1 : 0;
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

        private void minerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdvancedSettingsForm advancedSettingsForm = new AdvancedSettingsForm(minerConfiguration, applicationConfiguration);
            advancedSettingsForm.ShowDialog();
        }

        private void apiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.coinwarz.com/v1/api/documentation");
        }

        private void coinApiCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            apiKeyEdit.Enabled = coinApiCombo.SelectedIndex == 1;
            apiKeyLabel.Enabled = apiKeyEdit.Enabled;
        }
    }
}
