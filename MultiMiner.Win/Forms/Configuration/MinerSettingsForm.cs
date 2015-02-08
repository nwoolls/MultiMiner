using MultiMiner.Engine;
using MultiMiner.Engine.Extensions;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Diagnostics;
using System.Linq;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class MinerSettingsForm : MessageBoxFontForm
    {
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration;
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer workingMinerConfiguration;

        private readonly Application applicationConfiguration;
        private readonly Application workingApplicationConfiguration;

        private readonly Perks perksConfiguration;

        public MinerSettingsForm(MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration, Application applicationConfiguration,
            Perks perksConfiguration)
        {
            InitializeComponent();
            this.minerConfiguration = minerConfiguration;
            this.workingMinerConfiguration = ObjectCopier.CloneObject<MultiMiner.Engine.Data.Configuration.Xgminer, MultiMiner.Engine.Data.Configuration.Xgminer>(minerConfiguration);

            this.applicationConfiguration = applicationConfiguration;
            this.workingApplicationConfiguration = ObjectCopier.CloneObject<Application, Application>(applicationConfiguration);

            this.perksConfiguration = perksConfiguration;
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            xgminerConfigurationBindingSource.DataSource = workingMinerConfiguration;
            applicationConfigurationBindingSource.DataSource = workingApplicationConfiguration;
            PopulateIntervalCombo(intervalCombo);
            PopulateIntervalCombo(networkIntervalCombo);
            PopulateAlgorithmCombo();
            LoadSettings();

            algoArgCombo.Text = ApplicationViewModel.PrimaryAlgorithm;
        }

        private void PopulateAlgorithmCombo()
        {
            algoArgCombo.Items.Clear();
            System.Collections.Generic.List<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
                algoArgCombo.Items.Add(algorithm.Name.ToSpaceDelimitedWords());
        }
        
        private static void PopulateIntervalCombo(System.Windows.Forms.ComboBox combo)
        {
            combo.Items.Clear();
            foreach (Application.TimerInterval interval in (Application.TimerInterval[])Enum.GetValues(typeof(Application.TimerInterval)))
                combo.Items.Add(interval.ToString().ToSpaceDelimitedWords());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            ObjectCopier.CopyObject(workingMinerConfiguration, minerConfiguration);
            ObjectCopier.CopyObject(workingApplicationConfiguration, applicationConfiguration);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void LoadSettings()
        {
            intervalCombo.SelectedIndex = (int)workingApplicationConfiguration.ScheduledRestartMiningInterval;
            networkIntervalCombo.SelectedIndex = (int)workingApplicationConfiguration.ScheduledRestartNetworkDevicesInterval;

            algoArgCombo.SelectedIndex = 0;

            LoadProxySettings();
        }

        private void LoadProxySettings()
        {
            MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor proxy = minerConfiguration.StratumProxies.First();

            proxyPortEdit.Text = proxy.GetworkPort.ToString();
            stratumProxyPortEdit.Text = proxy.StratumPort.ToString();
        }

        private void SaveSettings()
        {
            //if the user has disabled Auto-Set Dynamic Intensity, disable Dynamic Intensity as well
            if (!workingApplicationConfiguration.AutoSetDesktopMode &&
                (workingApplicationConfiguration.AutoSetDesktopMode != applicationConfiguration.AutoSetDesktopMode))
                workingMinerConfiguration.DesktopMode = false;

            workingApplicationConfiguration.ScheduledRestartMiningInterval = (Application.TimerInterval)intervalCombo.SelectedIndex;
            workingApplicationConfiguration.ScheduledRestartNetworkDevicesInterval = (Application.TimerInterval)networkIntervalCombo.SelectedIndex;

            SaveProxySettings();
        }

        private void SaveProxySettings()
        {
            MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor proxy = minerConfiguration.StratumProxies.First();

            proxy.GetworkPort = int.Parse(proxyPortEdit.Text);
            proxy.StratumPort = int.Parse(stratumProxyPortEdit.Text);
        }

        private void argAlgoCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string algorithm = algoArgCombo.Text.Replace(" ", String.Empty);
            if (workingMinerConfiguration.AlgorithmFlags.ContainsKey(algorithm))
                algoArgEdit.Text = workingMinerConfiguration.AlgorithmFlags[algorithm];
            else
                algoArgEdit.Text = String.Empty;
        }

        private void algoArgEdit_Validated(object sender, EventArgs e)
        {
            string algorithm = algoArgCombo.Text.Replace(" ", String.Empty);
            workingMinerConfiguration.AlgorithmFlags[algorithm] = algoArgEdit.Text;
        }

        private void advancedProxiesButton_Click(object sender, EventArgs e)
        {
            if (!perksConfiguration.PerksEnabled)
            {
                if (!perksConfiguration.PerksEnabled)
                {
                    System.Windows.Forms.MessageBox.Show(MiningEngine.AdvancedProxiesRequirePerksMessage,
                        "Perks Required", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

                using (PerksForm perksForm = new PerksForm(perksConfiguration))
                {
                    perksForm.ShowDialog();
                }
            }
            else
            {
                using (ProxySettingsForm proxySettingsForm = new ProxySettingsForm(minerConfiguration))
                {
                    System.Windows.Forms.DialogResult dialogResult = proxySettingsForm.ShowDialog();
                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        perksConfiguration.AdvancedProxying = true;
                        perksConfiguration.SavePerksConfiguration();

                        LoadProxySettings();
                    }
                }
            }
        }

        private void proxyPortEdit_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void gpuSettingsLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            using (GPUMinerSettingsForm gpuSettingsForm = new GPUMinerSettingsForm(workingMinerConfiguration, workingApplicationConfiguration))
            {
                gpuSettingsForm.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Settings#advanced-miner-settings");
        }
    }
}
