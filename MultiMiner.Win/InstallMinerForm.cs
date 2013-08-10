using System;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class InstallMinerForm : Form
    {
        public enum MinerInstallOption
        {
            Cgminer = 0,
            Bfgminer = 1,
            Both = 2
        }

        public InstallMinerForm()
        {
            InitializeComponent();
        }

        public MinerInstallOption SelectedOption { get; set; }

        private void yesButton_Click(object sender, EventArgs e)
        {
            if (cgminerButton.Checked)
                SelectedOption = MinerInstallOption.Cgminer;
            else if (bfgminerButton.Checked)
                SelectedOption = MinerInstallOption.Bfgminer;
            else if (bothButton.Checked)
                SelectedOption = MinerInstallOption.Both;
        }
    }
}
