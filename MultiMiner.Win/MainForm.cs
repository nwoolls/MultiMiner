using MultiMiner.Engine;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private List<CoinConfiguration> configurations = new List<CoinConfiguration>();

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
                    configurations = (List<CoinConfiguration>)serializer.Deserialize(reader);
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
                serializer.Serialize(writer, configurations);
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

        }

        private void deviceGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == coinColumn.Index)
                {
                    string value = (string)deviceGridView.CurrentCell.EditedFormattedValue;
                    if (value.Equals("Add Coin"))
                    {
                        ConfigureCoins();
                    }
                }
            }
        }

        private void ConfigureCoins()
        {
            CoinsForm coinsForm = new CoinsForm(configurations);
            DialogResult dialogResult = coinsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                SaveCoinConfigurations();
        }

        private void deviceGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
