using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class WizardForm : Form
    {
        public WizardForm()
        {
            InitializeComponent();
        }

        private void configureMobileMinerPage_Click(object sender, EventArgs e)
        {

        }

        private void mobileMinerInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://mobileminerapp.com/");
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            SetupWizardTabControl();
            minerComboBox.SelectedIndex = 0;
            coinComboBox.SelectedIndex = 0;
        }

        private void SetupWizardTabControl()
        {
            const int margin = 3;
            const int tabHeight = 21;

            wizardTabControl.SelectedTab = chooseMinerPage;
            wizardTabControl.Dock = DockStyle.None;
            wizardTabControl.Top = -(margin + tabHeight);
            wizardTabControl.Left = -(margin);
            wizardTabControl.Width = this.ClientSize.Width + (margin * 2);
            wizardTabControl.Height = this.ClientSize.Height - buttonPanel.Height + (margin * 2) + tabHeight;

            foreach (TabPage tabPage in wizardTabControl.TabPages)
            {
                tabPage.Padding = new Padding(6, tabPage.Padding.Left, 6, tabPage.Padding.Right);
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (wizardTabControl.SelectedIndex < wizardTabControl.TabPages.Count - 1)
                wizardTabControl.SelectedIndex += 1;
            else
                Close();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (wizardTabControl.SelectedIndex > 0)
                wizardTabControl.SelectedIndex -= 1;
        }

        private void wizardTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (wizardTabControl.SelectedIndex == wizardTabControl.TabPages.Count - 1)
                nextButton.Text = "Finish";
            else
                nextButton.Text = "Next >";

            backButton.Enabled = wizardTabControl.SelectedIndex > 0;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitcoinPoolsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://google.com/search?q=bitcoin+mining+pools");
        }

        private void litecoinPoolsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://google.com/search?q=litecoin+mining+pools");
        }
    }
}
