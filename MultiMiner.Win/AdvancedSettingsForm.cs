using MultiMiner.Utility;
using MultiMiner.Win.Configuration;
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
            this.workingApplicationConfiguration = ObjectCopier.CloneObject<ApplicationConfiguration, ApplicationConfiguration>(applicationConfiguration);
        }

        private void AdvancedSettingsForm_Load(object sender, System.EventArgs e)
        {
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;
        }

        private void saveButton_Click(object sender, System.EventArgs e)
        {
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
