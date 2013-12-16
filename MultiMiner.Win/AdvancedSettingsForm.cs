using MultiMiner.Utility;
using MultiMiner.Win.Configuration;
using System;
using System.IO;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class AdvancedSettingsForm : Form
    {
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly ApplicationConfiguration workingApplicationConfiguration;

        public AdvancedSettingsForm(ApplicationConfiguration applicationConfiguration)
        {
            InitializeComponent();

            this.applicationConfiguration = applicationConfiguration;
            workingApplicationConfiguration = ObjectCopier.CloneObject<ApplicationConfiguration, ApplicationConfiguration>(applicationConfiguration);
            if (String.IsNullOrEmpty(workingApplicationConfiguration.LogFilePath))
                workingApplicationConfiguration.LogFilePath = ApplicationPaths.AppDataPath();
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (workingApplicationConfiguration.LogFilePath.Equals(ApplicationPaths.AppDataPath()))
                workingApplicationConfiguration.LogFilePath = String.Empty;

            if (!applicationConfiguration.LogFilePath.Equals(workingApplicationConfiguration.LogFilePath, StringComparison.OrdinalIgnoreCase))
                CopyLogFiles();

            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void CopyLogFiles()
        {
            string sourceDirectory = applicationConfiguration.LogFilePath;
            if (String.IsNullOrEmpty(sourceDirectory))
                sourceDirectory = ApplicationPaths.AppDataPath();

            CopyFilesToFolder(sourceDirectory, workingApplicationConfiguration.LogFilePath, "*.json");
        }

        private void CopyFilesToFolder(string sourceDirectory, string destinationDirectory, string searchPattern)
        {
            if (Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
                foreach (string sourceFilePath in Directory.GetFiles(sourceDirectory, searchPattern))
                {
                    string fileName = Path.GetFileName(sourceFilePath);
                    string destinationFilePath = Path.Combine(workingApplicationConfiguration.LogFilePath, fileName);

                    File.Copy(sourceFilePath, destinationFilePath, true);
                }
            }
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
    }
}
