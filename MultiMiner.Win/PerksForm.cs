using MultiMiner.Win.Configuration;
using System;

namespace MultiMiner.Win
{
    public partial class PerksForm : MessageBoxFontForm
    {
        public PerksForm(PerksConfiguration perksConfiguration)
        {
            InitializeComponent();
            this.perksConfigurationBindingSource.DataSource = perksConfiguration;
        }

        private void perksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckStates();
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

        private void UpdateCheckStates()
        {
            this.coinbaseCheckBox.Enabled = perksCheckBox.Checked;
            this.incomeCheckBox.Enabled = perksCheckBox.Checked;
        }

        private void PerksForm_Load(object sender, EventArgs e)
        {
            UpdateCheckStates();
        }
    }
}
