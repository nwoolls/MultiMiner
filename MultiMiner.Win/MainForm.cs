using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private List<Device> devices;
        private readonly EngineConfiguration engineConfiguration = new EngineConfiguration();
        private readonly KnownCoins knownCoins = new KnownCoins();
        private readonly MiningEngine miningEngine = new MiningEngine();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            devices = GetDevices();
            deviceBindingSource.DataSource = devices;

            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];

            engineConfiguration.LoadCoinConfigurations();
            RefreshCoinComboBox();

            engineConfiguration.LoadDeviceConfigurations();
            LoadGridValuesFromConfiguration();

            engineConfiguration.LoadMinerConfiguration();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private static List<Device> GetDevices()
        {
            MultiMiner.Xgminer.MinerConfiguration minerConfig = new MultiMiner.Xgminer.MinerConfiguration();
            minerConfig.ExecutablePath = @"Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);
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
            coinColumn.Items.Add("Configure Coins");
            coinColumn.Items.Add(string.Empty);
            foreach (CoinConfiguration configuration in engineConfiguration.CoinConfigurations)
            {
                coinColumn.Items.Add(configuration.Coin.Name);
            }
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

                    deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveGridValuesToConfiguration();
            engineConfiguration.SaveDeviceConfigurations();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;
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
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
        }

        private void SaveGridValuesToConfiguration()
        {
            deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            
            engineConfiguration.DeviceConfigurations.Clear();
            int index = 0;
            DeviceKind? lastDeviceKind = null;
            for (int i = 0; i < devices.Count; i++)
            {
                if ((lastDeviceKind != null) && (lastDeviceKind != devices[i].Kind))
                    index = 0;
                
                CryptoCoin coin = knownCoins.Coins.SingleOrDefault(c => c.Name.Equals(deviceGridView.Rows[i].Cells[coinColumn.Index].Value));
                if (coin != null)
                {
                    DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                    deviceConfiguration.DeviceKind = devices[i].Kind;
                    deviceConfiguration.DeviceIndex = index;
                    deviceConfiguration.CoinSymbol = coin.Symbol;

                    engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
                }

                lastDeviceKind = devices[i].Kind;
                index++;
            }
        }

        private void LoadGridValuesFromConfiguration()
        {
            int index = 0;
            DeviceKind? lastDeviceKind = null;
            for (int i = 0; i < devices.Count; i++)
            {
                Device device = devices[i];

                if ((lastDeviceKind != null) && (lastDeviceKind != devices[i].Kind))
                    index = 0;

                DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(
                    c => (c.DeviceKind == device.Kind)
                    && (c.DeviceIndex == index));

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

                lastDeviceKind = devices[i].Kind;
                index++;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            miningEngine.StopMining();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            miningEngine.StartMining(engineConfiguration);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(engineConfiguration.MinerConfiguration);
            DialogResult dialogResult = settingsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                engineConfiguration.SaveMinerConfiguration();
            else
                engineConfiguration.LoadMinerConfiguration();
        }
    }
}
