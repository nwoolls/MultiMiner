using MultiMiner.Utility.Forms;
using MultiMiner.Xgminer.Data;
using MultiMiner.Win.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;

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
            PopulateAlgorithmCombo();
            xgminerConfigurationBindingSource.DataSource = workingMinerConfiguration;
            LoadSettings();

            algoCombo.Text = AlgorithmNames.Scrypt.ToString().ToSpaceDelimitedWords();
        }

        private void LoadSettings()
        {
            algoCombo.SelectedIndex = 0;
        }

        private void SaveSettings()
        {
        }

        private void PopulateMinerCombo()
        {
            string algorithmName = algoCombo.Text.Replace(" ", String.Empty);
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

        private void PopulateAlgorithmCombo()
        {
            algoCombo.Items.Clear();

            //don't list SHA256 - we use BFGMiner for ASICs
            IEnumerable<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms.Where(a => a.Family != CoinAlgorithm.AlgorithmFamily.SHA2);

            foreach (CoinAlgorithm algorithm in algorithms)
            {
                algoCombo.Items.Add(algorithm.Name.ToSpaceDelimitedWords());
            }
        }

        private void algoCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMinerCombo();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
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
            string algorithmName = algoCombo.Text.Replace(" ", String.Empty);
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            string minerName = minerCombo.Text;
            if (algorithm.MinerArguments.ContainsKey(minerName))
                kernelArgsEdit.Text = algorithm.MinerArguments[minerName];
            else
                kernelArgsEdit.Text = String.Empty;
        }

        private void SaveMinerChoice()
        {
            string algorithm = algoCombo.Text.Replace(" ", String.Empty);
            workingMinerConfiguration.AlgorithmMiners[algorithm] = minerCombo.Text;
        }

        private void kernelArgsEdit_Validated(object sender, EventArgs e)
        {
            string algorithmName = algoCombo.Text.Replace(" ", String.Empty);
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            string minerName = minerCombo.Text;
            algorithm.MinerArguments[minerName] = kernelArgsEdit.Text;
        }
    }
}
