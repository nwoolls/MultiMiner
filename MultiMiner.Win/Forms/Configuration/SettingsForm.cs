using MultiMiner.MobileMiner.Helpers;
using MultiMiner.Utility.Forms;
using MultiMiner.Utility.OS;
using MultiMiner.UX.Data.Configuration;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class SettingsForm : MessageBoxFontForm
    {
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration;
        private readonly UX.Data.Configuration.Application applicationConfiguration;
        private readonly Paths pathConfiguration;
        private readonly Perks perksConfiguration;

        public SettingsForm(UX.Data.Configuration.Application applicationConfiguration, MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration,
            Paths pathConfiguration, Perks perksConfiguration)
        {
            InitializeComponent();

            this.minerConfiguration = minerConfiguration;
            this.applicationConfiguration = applicationConfiguration;
            this.pathConfiguration = pathConfiguration;
            this.perksConfiguration = perksConfiguration;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            CleanUpInput();

            if (!ValidateInput())
                return;

            SaveSettings();

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void CleanUpInput()
        {
            //clean up input
            emailAddressEdit.Text = emailAddressEdit.Text.Trim();
            appKeyEdit.Text = appKeyEdit.Text.Trim();
            apiKeyEdit.Text = apiKeyEdit.Text.Trim();
        }

        private bool ValidateInput()
        {
            //validate input
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

            UpdateMobileMinerControls();
        }

        private void PopulatePriorities()
        {
            Array possibleProprties = Enum.GetValues(typeof(ProcessPriorityClass));
            priorityCombo.DataSource = possibleProprties;
            priorityCombo.SelectedItem = ProcessPriorityClass.Normal;
        }

        private void LoadSettings()
        {
            applicationConfigurationBindingSource.DataSource = this.applicationConfiguration;

            priorityCombo.SelectedItem = minerConfiguration.Priority;
        }

        private void remoteMonitoringCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMobileMinerControls();
        }

        private void UpdateMobileMinerControls()
        {
            emailAddressEdit.Enabled = remoteMonitoringCheck.Checked;
            emailAddressEdit.Enabled = remoteMonitoringCheck.Checked;
            appKeyEdit.Enabled = remoteMonitoringCheck.Checked;            
        }

        private void mobileMinerInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://mobileminerapp.com/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (MinerSettingsForm advancedSettingsForm = new MinerSettingsForm(minerConfiguration, applicationConfiguration, perksConfiguration))
            {
                advancedSettingsForm.ShowDialog();
            }
        }

        private void apiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.coinwarz.com/v1/api/documentation");
        }

        private void advancedSettingsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (AdvancedSettingsForm advancedSettingsForm = new AdvancedSettingsForm(applicationConfiguration, pathConfiguration))
            {
                advancedSettingsForm.ShowDialog();
            }
        }

        private void serviceSettingsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (OnlineSettingsForm advancedSettingsForm = new OnlineSettingsForm(applicationConfiguration))
            {
                advancedSettingsForm.ShowDialog();
            }
        }
    }
}
