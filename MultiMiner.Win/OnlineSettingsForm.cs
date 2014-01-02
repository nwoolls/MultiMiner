using MultiMiner.Utility;
using MultiMiner.Win.Configuration;
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
    public partial class OnlineSettingsForm : MessageBoxFontForm
    {
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly ApplicationConfiguration workingApplicationConfiguration;

        public OnlineSettingsForm(ApplicationConfiguration applicationConfiguration)
        {
            InitializeComponent();

            this.applicationConfiguration = applicationConfiguration;
            workingApplicationConfiguration = ObjectCopier.CloneObject<ApplicationConfiguration, ApplicationConfiguration>(applicationConfiguration);
        }

        private void OnlineSettingsForm_Load(object sender, EventArgs e)
        {
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;

            remoteCommandsCheck.Enabled = workingApplicationConfiguration.MobileMinerMonitoring;
            pushNotificationsCheck.Enabled = workingApplicationConfiguration.MobileMinerMonitoring;
            httpsMobileMinerCheck.Enabled = workingApplicationConfiguration.MobileMinerMonitoring;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
