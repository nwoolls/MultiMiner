using MultiMiner.Utility.Forms;
using MultiMiner.Xgminer.Data;
using MultiMiner.Win.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Engine;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class GPUMinerSettingsForm : MessageBoxFontForm
    {
        private IList<CoinAlgorithm> algorithms;
        public GPUMinerSettingsForm(Engine.Data.Configuration.Xgminer minerConfiguration)
        {
            InitializeComponent();
            xgminerConfigurationBindingSource.DataSource = minerConfiguration;
        }

        private void GPUMinerSettingsForm_Load(object sender, EventArgs e)
        {
            PopulateAlgorithmCombo();
            LoadSettings();
        }

        private void LoadSettings()
        {
            algoCombo.SelectedIndex = 0;
        }

        private void PopulateMinerCombo()
        {
            CoinAlgorithm algorithm = algorithms[algoCombo.SelectedIndex];

            minerCombo.Items.Clear();
            IEnumerable<string> miners = MinerFactory.Instance.Miners
                .Where(miner => miner.Algorithm == algorithm)
                .Select(m => m.Name);
            foreach (string miner in miners)
            {
                minerCombo.Items.Add(miner);
            }
            minerCombo.SelectedIndex = 0;
        }

        private void PopulateAlgorithmCombo()
        {
            algoCombo.Items.Clear();
            algorithms = ((CoinAlgorithm[])Enum.GetValues(typeof(CoinAlgorithm))).ToList().Where(algo => algo != CoinAlgorithm.SHA256).ToList();
            foreach (CoinAlgorithm algorithm in algorithms)
                    algoCombo.Items.Add(algorithm.ToString().ToSpaceDelimitedWords());
        }

        private void algoCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMinerCombo();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }
    }
}
