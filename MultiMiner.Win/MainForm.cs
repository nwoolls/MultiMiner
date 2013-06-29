using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinerConfig minerConfig = new MinerConfig();
            minerConfig.ExecutablePath = @"Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);

            List<Device> devices = miner.GetDevices();
            deviceBindingSource.DataSource = devices;

        }

        private void deviceGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == CoinColumn.Index)
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
            CoinsForm coinsForm = new CoinsForm();
            coinsForm.ShowDialog();
        }

        private void deviceGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void deviceGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (deviceGridView.CurrentCell.RowIndex >= 0)
            {
                if (deviceGridView.CurrentCell.ColumnIndex == CoinColumn.Index)
                {
                    deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
    }
}
