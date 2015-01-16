using MultiMiner.Engine;
using MultiMiner.Utility.IO;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data.Configuration;
using System;
using System.Diagnostics;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class AdvancedSettingsForm : MessageBoxFontForm
    {
        private readonly Application applicationConfiguration;
        private readonly Application workingApplicationConfiguration;

        private readonly Paths pathConfiguration;
        private readonly Paths workingPathConfiguration;

        public AdvancedSettingsForm(Application applicationConfiguration, Paths pathConfiguration)
        {
            InitializeComponent();

            this.applicationConfiguration = applicationConfiguration;
            workingApplicationConfiguration = ObjectCopier.CloneObject<Application, Application>(applicationConfiguration);

            this.pathConfiguration = pathConfiguration;
            workingPathConfiguration = ObjectCopier.CloneObject<Paths, Paths>(pathConfiguration);

            if (String.IsNullOrEmpty(workingApplicationConfiguration.LogFilePath))
                workingApplicationConfiguration.LogFilePath = ApplicationPaths.AppDataPath();

            if (String.IsNullOrEmpty(workingPathConfiguration.SharedConfigPath))
                workingPathConfiguration.SharedConfigPath = ApplicationPaths.AppDataPath();
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;
            pathConfigurationBindingSource.DataSource = workingPathConfiguration;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            CopyLogFilesIfChanged();

            //don't persist default values
            ClearDefaultValues();

            ObjectCopier.CopyObject(workingPathConfiguration, pathConfiguration);
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void ClearDefaultValues()
        {
            if (workingApplicationConfiguration.LogFilePath.Equals(ApplicationPaths.AppDataPath()))
                workingApplicationConfiguration.LogFilePath = String.Empty;

            if (workingPathConfiguration.SharedConfigPath.Equals(ApplicationPaths.AppDataPath()))
                workingPathConfiguration.SharedConfigPath = String.Empty;
        }

        private void CopyLogFilesIfChanged()
        {
            string originalPath = applicationConfiguration.LogFilePath;
            if (string.IsNullOrEmpty(originalPath))
                originalPath = ApplicationPaths.AppDataPath();
            if (!originalPath.Equals(workingApplicationConfiguration.LogFilePath, StringComparison.OrdinalIgnoreCase))
                FileCopier.CopyFilesToFolder(originalPath, workingApplicationConfiguration.LogFilePath, "*.json");
        }

        private void logPathButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = logPathEdit.Text;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                workingApplicationConfiguration.LogFilePath = folderBrowserDialog1.SelectedPath;
                logPathEdit.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void configPathButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = configPathEdit.Text;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathConfiguration.SharedConfigPath = folderBrowserDialog1.SelectedPath;
                configPathEdit.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Settings#advanced-application-settings");
        }
    }
}
