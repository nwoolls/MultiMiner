using MultiMiner.UX.Data.Configuration;
using System;
using System.Diagnostics;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class PerksForm : MessageBoxFontForm
    {
        public PerksForm(Perks perksConfiguration)
        {
            InitializeComponent();
            this.perksConfigurationBindingSource.DataSource = perksConfiguration;
        }

        private void perksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
            UpdateThanks();
        }

        private void UpdateThanks()
        {
            string textBase = "Enable perks (at a 1% donation)";
            if (perksCheckBox.Checked)
                textBase = textBase + " - thank you!";

            perksCheckBox.Text = textBase;
            smileyPicture.Visible = perksCheckBox.Checked;
            smileyPicture.Left = perksCheckBox.Left + perksCheckBox.Width + 2;
        }

        private void UpdateControlStates()
        {
            this.exchangeApiCheckbox.Enabled = perksCheckBox.Checked;
            this.incomeCheckBox.Enabled = perksCheckBox.Checked;
            this.percentEdit.Enabled = perksCheckBox.Checked;
            this.percentLabel1.Enabled = perksCheckBox.Checked;
            this.percentLabel2.Enabled = perksCheckBox.Checked;
            this.remotingCheckBox.Enabled = perksCheckBox.Checked;
            this.remotingPasswordEdit.Enabled = perksCheckBox.Checked;
            this.advancedProxyCheckBox.Enabled = perksCheckBox.Checked;
        }

        private void PerksForm_Load(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void percentEdit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int percent = 1;
            bool success = int.TryParse(percentEdit.Text, out percent);
            if (success)
            {
                if (percent < 1)
                    success = false;
                if (percent > 100)
                    success = false;
            }
            e.Cancel = !success;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Perks");
        }
    }
}
