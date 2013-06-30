using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private readonly EngineConfiguration engineConfiguration = new EngineConfiguration();

        public MainForm()
        {
            InitializeComponent();
            engineConfiguration.LoadCoinConfigurations();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinerConfiguration minerConfig = new MinerConfiguration();
            minerConfig.ExecutablePath = @"Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);

            List<Device> devices = miner.GetDevices();
            deviceBindingSource.DataSource = devices;

            RefreshCoinComboBox();

            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];
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

                    //deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.SaveDeviceConfigurations();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.LoadDeviceConfigurations();
        }
    }
}
