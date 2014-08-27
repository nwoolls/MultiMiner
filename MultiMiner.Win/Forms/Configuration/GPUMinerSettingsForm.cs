using MultiMiner.Utility.Forms;
using MultiMiner.Xgminer.Data;
using MultiMiner.Win.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using System.Windows.Forms;
using System.Diagnostics;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class GPUMinerSettingsForm : MessageBoxFontForm
    {
        private IList<CoinAlgorithm> algorithms;

        private readonly Engine.Data.Configuration.Xgminer minerConfiguration;
        private readonly Engine.Data.Configuration.Xgminer workingMinerConfiguration;

        public GPUMinerSettingsForm(Engine.Data.Configuration.Xgminer minerConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
            this.workingMinerConfiguration = ObjectCopier.CloneObject<Engine.Data.Configuration.Xgminer, Engine.Data.Configuration.Xgminer>(minerConfiguration);

            //manual clone needed
            this.workingMinerConfiguration.AlgorithmMiners = new SerializableDictionary<string, string>();
            foreach (string key in this.minerConfiguration.AlgorithmMiners.Keys)
                this.workingMinerConfiguration.AlgorithmMiners[key] = this.minerConfiguration.AlgorithmMiners[key];
        }

        private void GPUMinerSettingsForm_Load(object sender, EventArgs e)
        {
            PopulateAlgorithmList();
            xgminerConfigurationBindingSource.DataSource = workingMinerConfiguration;

            string algorithmName = AlgorithmNames.Scrypt.ToSpaceDelimitedWords();
            algoListView.Items.Find(algorithmName, false).First().Selected = true;
        }

        private void PopulateMinerCombo()
        {
            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);

            minerCombo.Items.Clear();
            IEnumerable<string> miners = MinerFactory.Instance.Miners
                .Select(m => m.Name)
                .OrderBy(m => m);

            foreach (string miner in miners)
                minerCombo.Items.Add(miner);

            string currentMiner = algorithm.DefaultMiner;

            if (minerConfiguration.AlgorithmMiners.ContainsKey(algorithmName))
                currentMiner = minerConfiguration.AlgorithmMiners[algorithmName];

            minerCombo.SelectedItem = currentMiner;
        }

        private void PopulateAlgorithmList()
        {
            //don't list SHA256 - we use BFGMiner for ASICs
            IEnumerable<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms.Where(a => a.Family != CoinAlgorithm.AlgorithmFamily.SHA2);

            algoListView.Items.Clear();

            foreach (CoinAlgorithm algorithm in algorithms)
            {
                string algorithmName = algorithm.Name.ToSpaceDelimitedWords();
                algoListView.Items.Add(algorithmName, algorithmName, 0);
            }
        }

        private void algoCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMinerCombo();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ObjectCopier.CopyObject(workingMinerConfiguration, minerConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void minerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateKernelArguments();
            SaveMinerChoice();
        }

        private void UpdateKernelArguments()
        {
            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            string minerName = minerCombo.Text;
            if (algorithm.MinerArguments.ContainsKey(minerName))
                kernelArgsEdit.Text = algorithm.MinerArguments[minerName];
            else
                kernelArgsEdit.Text = String.Empty;
        }

        private void SaveMinerChoice()
        {
            string algorithmName = SelectedAlgorithmName();
            workingMinerConfiguration.AlgorithmMiners[algorithmName] = minerCombo.Text;
        }

        private void kernelArgsEdit_Validated(object sender, EventArgs e)
        {
            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            string minerName = minerCombo.Text;
            algorithm.MinerArguments[minerName] = kernelArgsEdit.Text;
        }

        private string SelectedAlgorithmName()
        {
            string algorithmName = algoListView.SelectedItems.Count == 0 ? String.Empty : algoListView.SelectedItems[0].Text.Replace(" ", String.Empty);
            return algorithmName;
        }

        private void algoListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool algoSelected = algoListView.SelectedItems.Count > 0;
            minerCombo.Enabled = algoSelected;
            kernelArgsEdit.Enabled = algoSelected;
            addButton.Enabled = true;

            if (algoSelected)
            {
                PopulateMinerCombo();
                string algorithmName = SelectedAlgorithmName();
                CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
                removeButton.Enabled = !algorithm.BuiltIn;
            }
            else
            {
                removeButton.Enabled = false;
            }
        }

        private void algoListView_BeforeLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            e.CancelEdit = algorithm.BuiltIn;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Add a new item to the ListView, with an empty label
            // (you can set any default properties that you want to here)
            string algorithmName = "Unnamed";

            MinerFactory.Instance.RegisterAlgorithm(algorithmName, algorithmName, CoinAlgorithm.AlgorithmFamily.Unknown);

            ListViewItem item = algoListView.Items.Add(algorithmName);
            item.ImageIndex = 0;

            // Place the newly-added item into edit mode immediately
            item.Selected = true;
            item.BeginEdit();
        }

        private void algoListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
                //edit was canceled
                return;

            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            algorithm.Name = e.Label;
            algorithm.FullName = e.Label;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedAlgorithm();
        }

        private void RemoveSelectedAlgorithm()
        {
            string algorithmName = SelectedAlgorithmName();
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            DialogResult dialogResult = MessageBox.Show("Remove the selected algorithm?", algorithmName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                MinerFactory.Instance.Algorithms.Remove(algorithm);
                algoListView.Items.Remove(algoListView.SelectedItems[0]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/GPU-Algorithms");
        }
    }
}
