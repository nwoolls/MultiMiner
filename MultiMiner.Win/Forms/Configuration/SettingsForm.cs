using MultiMiner.MobileMiner.Helpers;
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

        private const int CoinChooseIndex = 0;
        private const int CoinWarzIndex = 1;
        private const int WhatMineIndex = 2;
        private const int WhatToMineIndex = 3;
        
        private void SaveSettings()
        {
            minerConfiguration.Priority = (ProcessPriorityClass)priorityCombo.SelectedItem;

            applicationConfiguration.UseCoinWarzApi = coinApiCombo.SelectedIndex == CoinWarzIndex;
            applicationConfiguration.UseWhatMineApi = coinApiCombo.SelectedIndex == WhatMineIndex;
            applicationConfiguration.UseWhatToMineApi = coinApiCombo.SelectedIndex == WhatToMineIndex;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // adjust height for hidden MobileMiner settings
            Height = Height - 65;

            PopulatePriorities(); //populate before loading settings
            LoadSettings();
            autoLaunchCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
            sysTrayCheckBox.Enabled = OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;

            coinApiCombo.SelectedIndex = applicationConfiguration.UseCoinWarzApi ? CoinWarzIndex : 
                (applicationConfiguration.UseWhatMineApi ? WhatMineIndex : 
                (applicationConfiguration.UseWhatToMineApi ? WhatToMineIndex :
                CoinChooseIndex));
            PopulateApiKey();
        }

        private void PopulateApiKey()
        {
            if (coinApiCombo.SelectedIndex == CoinWarzIndex)
                apiKeyEdit.Text = applicationConfiguration.CoinWarzApiKey;
            else if (coinApiCombo.SelectedIndex == WhatMineIndex)
                apiKeyEdit.Text = applicationConfiguration.WhatMineApiKey;
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
            if (coinApiCombo.SelectedIndex == CoinWarzIndex)
                Process.Start("http://www.coinwarz.com/v1/api/documentation");
            else if (coinApiCombo.SelectedIndex == WhatMineIndex)
                Process.Start("http://whatmine.com/api.php");
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

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://web.mobileminerapp.com/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Settings");
        }

        private void apiKeyEdit_Validated(object sender, EventArgs e)
        {
            if (coinApiCombo.SelectedIndex == CoinWarzIndex)
                applicationConfiguration.CoinWarzApiKey = apiKeyEdit.Text.Trim();
            else if (coinApiCombo.SelectedIndex == WhatMineIndex)
                applicationConfiguration.WhatMineApiKey = apiKeyEdit.Text.Trim();
        }

        private void coinApiCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (coinApiCombo.SelectedIndex == CoinWarzIndex)
                apiKeyLabel.Text = "CoinWarz key:";
            else if (coinApiCombo.SelectedIndex == WhatMineIndex)
                apiKeyLabel.Text = "WhatMine key:";
            PopulateApiKey();
        }
    }
}
