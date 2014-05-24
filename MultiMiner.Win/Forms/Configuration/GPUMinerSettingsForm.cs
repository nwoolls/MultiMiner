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
            this.workingMinerConfiguration.AlgorithmMiners = new SerializableDictionary<CoinAlgorithm, string>();
            foreach (CoinAlgorithm key in this.minerConfiguration.AlgorithmMiners.Keys)
                this.workingMinerConfiguration.AlgorithmMiners[key] = this.minerConfiguration.AlgorithmMiners[key];
        }

        private void GPUMinerSettingsForm_Load(object sender, EventArgs e)
        {
            PopulateAlgorithmCombo();
            xgminerConfigurationBindingSource.DataSource = workingMinerConfiguration;
            LoadSettings();
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
            CoinAlgorithm algorithm = algorithms[algoCombo.SelectedIndex];

            minerCombo.Items.Clear();
            IEnumerable<string> miners = MinerFactory.Instance.Miners
                .Select(m => m.Name)
                .OrderBy(m => m);


            foreach (string miner in miners)
                minerCombo.Items.Add(miner);

            string currentMiner = MinerFactory.Instance.DefaultMiners[algorithm].Name;

            if (minerConfiguration.AlgorithmMiners.ContainsKey(algorithm))
                currentMiner = minerConfiguration.AlgorithmMiners[algorithm];

            minerCombo.SelectedItem = currentMiner;
        }

        private void PopulateAlgorithmCombo()
        {
            algoCombo.Items.Clear();
            algorithms = ((CoinAlgorithm[])Enum.GetValues(typeof(CoinAlgorithm))).ToList().Where(algo => algo != CoinAlgorithm.SHA256).ToList();
            foreach (CoinAlgorithm algorithm in algorithms)
            {
                if (AlgorithmIsSupported(algorithm))
                    algoCombo.Items.Add(algorithm.ToString().ToSpaceDelimitedWords());
            }
        }

        private static bool AlgorithmIsSupported(CoinAlgorithm algorithm)
        {
            return MinerFactory.Instance.DefaultMiners.ContainsKey(algorithm);
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
            SaveMinerChoice();
        }

        private void SaveMinerChoice()
        {
            CoinAlgorithm algorithm = algorithms[algoCombo.SelectedIndex];
            workingMinerConfiguration.AlgorithmMiners[algorithm] = minerCombo.Text;

        }
    }
}
