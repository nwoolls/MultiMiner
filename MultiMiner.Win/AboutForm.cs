using MultiMiner.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            PopulateAppVersions();
        }

        private void PopulateAppVersions()
        {
            string multiMinerVersion = Engine.Installer.GetInstalledMinerVersion();
            multiMinerLabel.Text = "MultiMiner " + multiMinerVersion;

            PopulateXgminerVersion(Xgminer.MinerBackend.Cgminer, cgminerLabel);
            PopulateXgminerVersion(Xgminer.MinerBackend.Bfgminer, bfgminerLabel);
        }

        private static void PopulateXgminerVersion(MultiMiner.Xgminer.MinerBackend minerBackend, Label targetLabel)
        {
            string xgminerName = MinerPath.GetMinerName(minerBackend);
            string xgminerPath = MinerPath.GetPathToInstalledMiner(minerBackend);
            string xgminerVersion = Xgminer.Installer.GetInstalledMinerVersion(minerBackend, xgminerPath);

            if (string.IsNullOrEmpty(xgminerVersion))
                targetLabel.Text = String.Format("{0} not installed", xgminerName);
            else
                targetLabel.Text = String.Format("{0} {1} installed", xgminerName, xgminerVersion);
        }

        private void multiMinerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://multiminerapp.com/");
        }
    }
}
