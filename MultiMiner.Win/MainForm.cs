using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Net;
using MultiMiner.Coinchoose.Api;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private List<CoinInformation> coinInformation;
        private List<Device> devices;
        private readonly EngineConfiguration engineConfiguration = new EngineConfiguration();
        private List<CryptoCoin> knownCoins = new List<CryptoCoin>();
        private readonly MiningEngine miningEngine = new MiningEngine();
        private readonly ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
        private int startupMiningCountdownSeconds = 0;
        private int coinStatsCountdownMinutes = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            coinStatsCountdownMinutes = 15;
            coinStatsTimer.Interval = coinStatsCountdownMinutes * 60 * 1000; //15 minutes

            RefreshCoinStats();

            LoadSettings();

            RefreshBackendLabel();

            RefreshCoinComboBox();

            PositionCoinchooseLabels();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;

            RefreshDevices();

            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];
        }

        private void PositionCoinchooseLabels()
        {
            //so things align correctly under Mono
            coinChooseLinkLabel.Left = coinChoosePrefixLabel.Left + coinChoosePrefixLabel.Width;
            coinChooseSuffixLabel.Left = coinChooseLinkLabel.Left + coinChooseLinkLabel.Width;
        }

        private void RefreshDevices()
        {
            devices = GetDevices();
            deviceBindingSource.DataSource = devices;
            LoadGridValuesFromConfiguration();
            CheckAndHideNameColumn();
        }

        private void CheckAndHideNameColumn()
        {
            nameColumn.Visible = devices.Count(d => !string.IsNullOrEmpty(d.Name)) > 0;
        }

        private void LoadSettings()
        {
            engineConfiguration.LoadCoinConfigurations();
            engineConfiguration.LoadDeviceConfigurations();
            engineConfiguration.LoadMinerConfiguration();
            engineConfiguration.LoadStrategyConfiguration();

            coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.MineProfitableCoins;
            RefreshStrategiesLabel();
            RefreshStrategiesCountdown();

            applicationConfiguration.LoadApplicationConfiguration();

            if (applicationConfiguration.StartMiningOnStartup)
            {
                startupMiningTimer.Interval = 1000 * applicationConfiguration.StartupMiningDelay;
                startupMiningCountdownSeconds = applicationConfiguration.StartupMiningDelay;
                startupMiningTimer.Enabled = true;
                startupMiningCountdownTimer.Enabled = true;
                RefreshCountdownLabel();
            }

            startupMiningPanel.Visible = applicationConfiguration.StartMiningOnStartup;

            crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
        }

        private void RefreshCountdownLabel()
        {
            countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", startupMiningCountdownSeconds);    
        }

        private List<Device> GetDevices()
        {
            string minerName = "cgminer";
            if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                minerName = "bfgminer";

            MinerConfiguration minerConfiguration = new MinerConfiguration();
            minerConfiguration.MinerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    minerConfiguration.ExecutablePath = minerName;
                    break;
                default:
                    minerConfiguration.ExecutablePath = string.Format(@"Miners\{0}\{0}.exe", minerName);
                    break;
            }

            Miner miner = new Miner(minerConfiguration);
            return miner.DeviceList();
        }

        private void ConfigureCoins()
        {
            CoinsForm coinsForm = new CoinsForm(engineConfiguration.CoinConfigurations, knownCoins);
            DialogResult dialogResult = coinsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                engineConfiguration.SaveCoinConfigurations();
            else
                engineConfiguration.LoadCoinConfigurations();
            RefreshCoinComboBox();
        }

        private void RefreshCoinComboBox()
        {
            coinColumn.Items.Clear();

            foreach (CoinConfiguration configuration in engineConfiguration.CoinConfigurations)
                coinColumn.Items.Add(configuration.Coin.Name);

            coinColumn.Items.Add(string.Empty);
        }
     
        private void deviceGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (deviceGridView.CurrentCell.RowIndex >= 0)
            {
                if (deviceGridView.CurrentCell.ColumnIndex == coinColumn.Index)
                {
                    deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            SaveGridValuesToConfiguration();
            engineConfiguration.SaveDeviceConfigurations();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;

            if (miningEngine.Mining)
            {
                miningEngine.RestartMining();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.LoadDeviceConfigurations();
            LoadGridValuesFromConfiguration();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void deviceGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == coinColumn.Index)
            {
                saveButton.Enabled = true;
                cancelButton.Enabled = true;

                if (this.coinInformation != null)
                    LoadGridValuesFromCoinStats();
            }
        }

        private void SaveGridValuesToConfiguration()
        {
            deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            
            engineConfiguration.DeviceConfigurations.Clear();

            for (int i = 0; i < devices.Count; i++)
            {                
                CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Name.Equals(deviceGridView.Rows[i].Cells[coinColumn.Index].Value));
                if (coin != null)
                {
                    DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                    deviceConfiguration.DeviceIndex = i;
                    deviceConfiguration.CoinSymbol = coin.Symbol;

                    engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
                }
            }
        }

        private void LoadGridValuesFromConfiguration()
        {
            bool saveEnabled = saveButton.Enabled;
            try
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    Device device = devices[i];

                    DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(
                        c => (c.DeviceIndex == i));

                    if (deviceConfiguration != null)
                    {
                        CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(deviceConfiguration.CoinSymbol));
                        if (coin != null)
                            deviceGridView.Rows[i].Cells[coinColumn.Index].Value = coin.Name;
                    }
                    else
                    {
                        deviceGridView.Rows[i].Cells[coinColumn.Index].Value = string.Empty;
                    }
                }                
            }
            finally
            {
                //restore button states after
                saveButton.Enabled = saveEnabled;
                cancelButton.Enabled = saveEnabled;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void StopMining()
        {
            miningEngine.StopMining();
            deviceStatsTimer.Enabled = false;
            coinStatsCountdownTimer.Enabled = false;
            RefreshStrategiesCountdown();
            scryptRateLabel.Text = string.Empty;
            sha256RateLabel.Text = string.Empty;

            stopButton.Enabled = false;
            startButton.Enabled = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
            StartMining();
        }

        private void StartMining()
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;

            miningEngine.StartMining(engineConfiguration, devices, coinInformation);
            deviceStatsTimer.Enabled = true;
            coinStatsCountdownTimer.Enabled = true;
            RefreshStrategiesCountdown();

            //to get changes from strategy config
            LoadGridValuesFromConfiguration();
        }

        private void RefreshBackendLabel()
        {
            if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                backendLabel.Text = "Backend: bfgminer";
            else if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Cgminer)
                backendLabel.Text = "Backend: cgminer";
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            ShowApplicationSettings();
        }

        private void ShowApplicationSettings()
        {
            SettingsForm settingsForm = new SettingsForm(applicationConfiguration, engineConfiguration.XgminerConfiguration);
            DialogResult dialogResult = settingsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                engineConfiguration.SaveMinerConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                RefreshDevices();
                RefreshBackendLabel();
                crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
            }
            else
            {
                engineConfiguration.LoadMinerConfiguration();
                applicationConfiguration.LoadApplicationConfiguration();
            }
        }

        private bool IdentifierIsGpu(string identifier)
        {
            return (identifier.Equals("GPU") || identifier.Equals("OCL"));
        }

        private void statsTimer_Tick(object sender, EventArgs e)
        {
            ClearStatsForDisabledCoins();
            PopulateStatsFromMinerProcesses();
        }

        private void ClearStatsForDisabledCoins()
        {
            if (saveButton.Enabled) //otherwise cleared coin isn't saved yet
                return;

            foreach (DataGridViewRow row in deviceGridView.Rows)
            {
                if (row.Cells[coinColumn.Index].Value == null)
                {
                    row.Cells[temperatureColumn.Index].Value = null;
                    row.Cells[hashRateColumn.Index].Value = null;
                    row.Cells[acceptedColumn.Index].Value = null;
                    row.Cells[rejectedColumn.Index].Value = null;
                    row.Cells[errorsColumn.Index].Value = null;
                }
            }
        }

        private void PopulateStatsFromMinerProcesses()
        {
            double totalScryptRate = 0;
            double totalSha256Rate = 0;

            bool hasTempValue = false;

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                MultiMiner.Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;
                if (apiContext != null)
                {
                    IEnumerable<MultiMiner.Xgminer.Api.DeviceInformation> enabledDevices = null;
                    try
                    {
                        enabledDevices = apiContext.GetDeviceInformation().Where(d => d.Enabled);
                    }
                    catch (IOException ex)
                    {
                        //don't fail and crash out due to any issues communicating via the API
                        continue;
                    }

                    foreach (MultiMiner.Xgminer.Api.DeviceInformation deviceInformation in enabledDevices)
                    {
                        int index = 0;
                        int rowIndex = -1;

                        for (int i = 0; i < devices.Count; i++)
                        {
                            if ((deviceInformation.Kind.Equals("GPU") && IdentifierIsGpu(devices[i].Identifier))
                                || (!deviceInformation.Kind.Equals("GPU") && !IdentifierIsGpu(devices[i].Identifier)))
                            {
                                if (index == deviceInformation.Index)
                                {
                                    rowIndex = i;
                                    break;
                                }
                                index++;
                            }
                        }

                        if (rowIndex >= 0)
                        {
                            if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.Scrypt)
                                totalScryptRate += deviceInformation.AverageHashrate;
                            else if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.SHA256)
                                totalSha256Rate += deviceInformation.AverageHashrate;

                            deviceGridView.Rows[rowIndex].Cells[temperatureColumn.Index].Value = deviceInformation.Temperature;
                            deviceGridView.Rows[rowIndex].Cells[hashRateColumn.Index].Value = deviceInformation.AverageHashrate;
                            deviceGridView.Rows[rowIndex].Cells[acceptedColumn.Index].Value = deviceInformation.AcceptedShares;
                            deviceGridView.Rows[rowIndex].Cells[rejectedColumn.Index].Value = deviceInformation.RejectedShares;
                            deviceGridView.Rows[rowIndex].Cells[errorsColumn.Index].Value = deviceInformation.HardwareErrors;
                            deviceGridView.Rows[rowIndex].Cells[intensityColumn.Index].Value = deviceInformation.Intensity;

                            if (deviceInformation.Temperature > 0)
                                hasTempValue = true;
                        }
                    }
                }
            }

            scryptRateLabel.Text = "Scrypt: " + totalScryptRate + " kh/s";
            sha256RateLabel.Text = "SHA256: " + totalSha256Rate / 1000 + " mh/s";

            //hide the temperature column if there are no tempts returned (USBs, OS X, etc)
            temperatureColumn.Visible = hasTempValue;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopMining();
        }

        private void coinStatsTimer_Tick(object sender, EventArgs e)
        {
            RefreshCoinStats();

            miningEngine.ApplyMiningStrategy(devices, coinInformation);

            //to get changes from strategy config
            LoadGridValuesFromConfiguration();

            coinStatsCountdownMinutes = coinStatsTimer.Interval / 1000 / 60;
        }

        private void RefreshCoinStats()
        {
            try
            {
                coinInformation = MultiMiner.Coinchoose.Api.ApiContext.GetCoinInformation();
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if (ex is WebException)
                {
                    LoadKnownCoinsFromFile();
                    return;
                }
                throw;
            }

            LoadGridValuesFromCoinStats();
            LoadKnownCoinsFromCoinStats();
            RefreshCoinStatsLabel();
        }

        private void RefreshCoinStatsLabel()
        {
            coinStatsLabel.Text = string.Format("Coin stats last fetched at {0}.", DateTime.Now.ToShortTimeString());
        }

        private void LoadKnownCoinsFromCoinStats()
        {
            foreach (CoinInformation item in coinInformation)
            {
                //find existing known coin or create a knew one
                CryptoCoin knownCoin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(item.Symbol));
                if (knownCoin == null)
                {
                    knownCoin = new CryptoCoin();
                    this.knownCoins.Add(knownCoin);
                }

                knownCoin.Symbol = item.Symbol;
                knownCoin.Name = item.Name;
                if (item.Algorithm.Contains("scrypt"))
                    knownCoin.Algorithm = CoinAlgorithm.Scrypt;
                else
                    knownCoin.Algorithm = CoinAlgorithm.SHA256;

            }
            SaveKnownCoinsToFile();
        }

        private static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private static string KnownCoinsFileName()
        {
            return Path.Combine(AppDataPath(), "KnownCoinsCache.xml");
        }

        private void LoadKnownCoinsFromFile()
        {
            knownCoins = ConfigurationReaderWriter.ReadConfiguration<List<CryptoCoin>>(KnownCoinsFileName());
        }

        private void SaveKnownCoinsToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(knownCoins, KnownCoinsFileName());
        }

        private void LoadGridValuesFromCoinStats()
        {
            foreach (CoinInformation coin in coinInformation)
            {
                foreach (DataGridViewRow row in deviceGridView.Rows)
                {
                    if (coin.Name.Equals((string)row.Cells[coinColumn.Index].Value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        row.Cells[difficultyColumn.Index].Value = coin.Difficulty.ToString(".################");
                        row.Cells[priceColumn.Index].Value = coin.Price.ToString(".################");
                        row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.AdjustedProfitability, 2); 
                    }
                }
            }
        }

        private void startupMiningTimer_Tick(object sender, EventArgs e)
        {
            startupMiningPanel.Visible = false;
            startupMiningTimer.Enabled = false;
            startupMiningCountdownTimer.Enabled = false;

            Application.DoEvents();

            StartMining();
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            startupMiningCountdownSeconds--;
            RefreshCountdownLabel();
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            startupMiningTimer.Enabled = false;
            startupMiningCountdownTimer.Enabled = false;
            startupMiningPanel.Visible = false;
            countdownLabel.Visible = false; //or remains visible under Mono
            cancelStartupMiningButton.Visible = false; //or remains visible under Mono
        }

        private void crashRecoveryTimer_Tick(object sender, EventArgs e)
        {
            miningEngine.RelaunchCrashedMiners();
        }

        private void detectDevicesButton_Click(object sender, EventArgs e)
        {
            RefreshDevices();
        }

        private void strategiesButton_Click(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void RefreshStrategiesLabel()
        {
            if (engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                strategiesLabel.Text = "Strategies: enabled";
            }
            else
            {
                strategiesLabel.Text = "Strategies: disabled";
            }
        }

        private void RefreshStrategiesCountdown()
        {
            //Time until strategy check: 60s
            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                strategyCountdownLabel.Text = string.Format("Time until strategy check: {0}m", coinStatsCountdownMinutes);
            }
            else
            {
                strategyCountdownLabel.Text = "";
            }
        }

        private void ConfigureStrategies()
        {
            StrategiesForm strategiesForm = new StrategiesForm(engineConfiguration.StrategyConfiguration, knownCoins);
            DialogResult dialogResult = strategiesForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                engineConfiguration.SaveStrategyConfiguration();
                coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.MineProfitableCoins;
                RefreshStrategiesLabel();
            }
            else
            {
                engineConfiguration.LoadStrategyConfiguration();
            }
        }

        private void coinStatsCountdownTimer_Tick(object sender, EventArgs e)
        {
            coinStatsCountdownMinutes--;
            RefreshStrategiesCountdown();
        }

        private void coinChooseLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://coinchoose.com/");
        }

        private void coinsButton_Click(object sender, EventArgs e)
        {
            ConfigureCoins();
        }
    }
}
