using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Installers;
using MultiMiner.UX.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms
{
    public partial class AboutForm : MessageBoxFontForm
    {
        private int backendMinerIndex {get; set;}

        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            PopulateCopyright();
            PopulateAppVersions();
        }

        private void PopulateCopyright()
        {
            DateTime compileDate = Assembly.GetExecutingAssembly().GetCompileDate();
            const string source = "(C) 2013";
            licenseTextBox.Text = licenseTextBox.Text.Replace(source, String.Format("{0} - {1}", source, compileDate.Year));
        }

        private void PopulateAppVersions()
        {
            multiMinerLabel.Text = "MultiMiner " + MultiMinerInstaller.GetInstalledMinerVersion();
            revisionLabel.Text = String.Format("(rev {0})", MultiMinerInstaller.GetInstalledMinerRevision());
            revisionLabel.Left = multiMinerLabel.Left + multiMinerLabel.Width;

            PopulateBackendMinerVersion();
        }

        private void multiMinerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://multiminerapp.com/");
        }

        private void PopulateBackendMinerVersion()
        {
            MinerDescriptor miner = MinerFactory.Instance.Miners[backendMinerIndex];
            string xgminerName = miner.Name;
            string xgminerPath = MinerPath.GetPathToInstalledMiner(miner);
            string xgminerVersion = String.Empty;

            if (File.Exists(xgminerPath))
                xgminerVersion = MinerInstaller.GetInstalledMinerVersion(xgminerPath, miner.LegacyApi);

            if (string.IsNullOrEmpty(xgminerVersion))
                bfgminerLabel.Text = String.Format("{0} not installed", xgminerName);
            else
                bfgminerLabel.Text = String.Format("{0} {1} installed", xgminerName, xgminerVersion);
        }

        private void backendMinerLabel_Click(object sender, EventArgs e)
        {
            backendMinerIndex++;
            if (backendMinerIndex == MinerFactory.Instance.Miners.Count)
                backendMinerIndex = 0;

            PopulateBackendMinerVersion();
        }
    }
}
