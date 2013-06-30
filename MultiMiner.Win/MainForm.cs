using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private EngineConfiguration engineConfiguration = new EngineConfiguration();

        public MainForm()
        {
            InitializeComponent();
            LoadCoinConfigurations();
        }

        private void LoadCoinConfigurations()
        {
            string fileName = CoinConfigurationsFileName();
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CoinConfiguration>));
                using (TextReader reader = new StreamReader(fileName))
                {
                    engineConfiguration.CoinConfigurations = (List<CoinConfiguration>)serializer.Deserialize(reader);
                } 
            }
        }

        private void SaveCoinConfigurations()
        {
            string fileName = CoinConfigurationsFileName();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            XmlSerializer serializer = new XmlSerializer(typeof(List<CoinConfiguration>));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, engineConfiguration.CoinConfigurations);
            }           
            
        }

        private string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private string CoinConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "CoinConfigurations.xml");
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
                SaveCoinConfigurations();
            else
                LoadCoinConfigurations();
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
    }
}
