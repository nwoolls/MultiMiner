using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private JArray coinInformation;
        private List<Device> devices;
        private readonly EngineConfiguration engineConfiguration = new EngineConfiguration();
        private readonly KnownCoins knownCoins = new KnownCoins();
        private readonly MiningEngine miningEngine = new MiningEngine();
        private readonly ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
        private int countdownSeconds = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshCoinStats();

            devices = GetDevices();
            deviceBindingSource.DataSource = devices;

            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];

            LoadSettings();

            RefreshCoinComboBox();
            LoadGridValuesFromConfiguration();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void LoadSettings()
        {
            engineConfiguration.LoadCoinConfigurations();
            engineConfiguration.LoadDeviceConfigurations();
            engineConfiguration.LoadMinerConfiguration();
            applicationConfiguration.LoadApplicationConfiguration();

            if (applicationConfiguration.StartMiningOnStartup)
            {
                startupMiningTimer.Interval = 1000 * applicationConfiguration.StartupMiningDelay;
                countdownSeconds = applicationConfiguration.StartupMiningDelay;
                startupMiningTimer.Enabled = true;
                countdownTimer.Enabled = true;
                RefreshCountdownLabel();
            }

            startupMiningPanel.Visible = applicationConfiguration.StartMiningOnStartup;

            crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
        }

        private void RefreshCountdownLabel()
        {
            countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", countdownSeconds);    
        }

        private static List<Device> GetDevices()
        {
            MinerConfiguration minerConfiguration = new MinerConfiguration();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    minerConfiguration.ExecutablePath = @"cgminer";
                    break;
                default:
                    minerConfiguration.ExecutablePath = @"Miners\cgminer\cgminer.exe";
                    break;
            }

            Miner miner = new Miner(minerConfiguration);
            return miner.GetDevices();
        }

        private void ConfigureCoins()
        {
            CoinsForm coinsForm = new CoinsForm(engineConfiguration.CoinConfigurations);
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
            coinColumn.Items.Add("Configure Coins");
        }

        private bool configuringCoins = false;        
        private void deviceGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (deviceGridView.CurrentCell.RowIndex >= 0)
            {
                if (deviceGridView.CurrentCell.ColumnIndex == coinColumn.Index)
                {
                    string value = (string)deviceGridView.CurrentCell.EditedFormattedValue;
                    if (value.Equals("Configure Coins"))
                        CheckAndConfigureCoins();

                    deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void CheckAndConfigureCoins()
        {
            if (!configuringCoins)
            {
                configuringCoins = true;
                try
                {
                    deviceGridView.CancelEdit();
                    ConfigureCoins();
                }
                finally
                {
                    configuringCoins = false;
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
                //have to do this in CellValueChanged under Mono - CurrentCellDirtyStateChanged not called
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    string value = (string)deviceGridView.CurrentCell.EditedFormattedValue;
                    if (value.Equals("Configure Coins"))
                        CheckAndConfigureCoins();
                }

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
                CryptoCoin coin = knownCoins.Coins.SingleOrDefault(c => c.Name.Equals(deviceGridView.Rows[i].Cells[coinColumn.Index].Value));
                if (coin != null)
                {
                    DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                    deviceConfiguration.DeviceKind = devices[i].Kind;
                    deviceConfiguration.DeviceIndex = i;
                    deviceConfiguration.CoinSymbol = coin.Symbol;

                    engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
                }
            }
        }

        private void LoadGridValuesFromConfiguration()
        {
            for (int i = 0; i < devices.Count; i++)
            {
                Device device = devices[i];
                
                DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(
                    c => (c.DeviceKind == device.Kind)
                    && (c.DeviceIndex == i));

                if (deviceConfiguration != null)
                {
                    CryptoCoin coin = knownCoins.Coins.SingleOrDefault(c => c.Symbol.Equals(deviceConfiguration.CoinSymbol));
                    if (coin != null)
                        deviceGridView.Rows[i].Cells[coinColumn.Index].Value = coin.Name;
                }
                else
                {
                    deviceGridView.Rows[i].Cells[coinColumn.Index].Value = string.Empty;
                }
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

            miningEngine.StartMining(engineConfiguration);
            deviceStatsTimer.Enabled = true;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(applicationConfiguration, engineConfiguration.MinerConfiguration);
            DialogResult dialogResult = settingsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                engineConfiguration.SaveMinerConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
            }
            else
            {
                engineConfiguration.LoadMinerConfiguration();
                applicationConfiguration.LoadApplicationConfiguration();
            }
        }

        private void statsTimer_Tick(object sender, EventArgs e)
        {
            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                MultiMiner.Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;
                if (apiContext != null)
                {
                    IEnumerable<MultiMiner.Xgminer.Api.DeviceInformation> enabledDevices = null;
                    try
                    {
                        enabledDevices = apiContext.GetDeviceInformation().Where(d => d.Enabled); ;                       
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
                            if ((deviceInformation.Kind.Equals("GPU") && (devices[i].Kind == DeviceKind.GPU)) || (!deviceInformation.Kind.Equals("GPU") && (devices[i].Kind == DeviceKind.USB)))
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
                            deviceGridView.Rows[rowIndex].Cells[temperatureColumn.Index].Value = deviceInformation.Temperature;
                            deviceGridView.Rows[rowIndex].Cells[hashRateColumn.Index].Value = deviceInformation.AverageHashrate;
                            deviceGridView.Rows[rowIndex].Cells[acceptedColumn.Index].Value = deviceInformation.AcceptedShares;
                            deviceGridView.Rows[rowIndex].Cells[rejectedColumn.Index].Value = deviceInformation.RejectedShares;
                            deviceGridView.Rows[rowIndex].Cells[errorsColumn.Index].Value = deviceInformation.HardwareErrors;
                        }
                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopMining();
        }

        private void coinStatsTimer_Tick(object sender, EventArgs e)
        {
            RefreshCoinStats();
        }

        private void RefreshCoinStats()
        {
            coinInformation = MultiMiner.Coinchoose.Api.ApiContext.GetCoinInformation();

            LoadGridValuesFromCoinStats();
        }

        private void LoadGridValuesFromCoinStats()
        {
            foreach (JToken jToken in coinInformation)
            {
                string symbol = jToken.Value<string>("symbol");
                string name = jToken.Value<string>("name");
                double difficulty = jToken.Value<double>("difficulty");
                double price = jToken.Value<double>("price");

                foreach (DataGridViewRow row in deviceGridView.Rows)
                {
                    if (name.Equals((string)row.Cells[coinColumn.Index].Value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        row.Cells[difficultyColumn.Index].Value = difficulty.ToString(".################");
                        row.Cells[priceColumn.Index].Value = price.ToString(".################");
                    }
                }
            }
        }

        private void startupMiningTimer_Tick(object sender, EventArgs e)
        {
            startupMiningPanel.Visible = false;
            startupMiningTimer.Enabled = false;
            countdownTimer.Enabled = false;

            Application.DoEvents();

            StartMining();
        }

        private void cancelAutoMineButton_Click(object sender, EventArgs e)
        {
            startupMiningTimer.Enabled = false;
            startupMiningPanel.Visible = false;
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            countdownSeconds--;
            RefreshCountdownLabel();
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            startupMiningTimer.Enabled = false;
            countdownTimer.Enabled = false;
            startupMiningPanel.Visible = false;
            countdownLabel.Visible = false; //or remains visible under Mono
            cancelStartupMiningButton.Visible = false; //or remains visible under Mono
        }

        private void crashRecoveryTimer_Tick(object sender, EventArgs e)
        {
            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
                if (minerProcess.Process.HasExited)
                    minerProcess.Process = new Miner(minerProcess.MinerConfiguration).Launch();
        }
    }
}
