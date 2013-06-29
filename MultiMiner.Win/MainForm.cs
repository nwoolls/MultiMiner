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

        private void Form1_Load(object sender, EventArgs e)
        {
            MinerConfig minerConfig = new MinerConfig();
            minerConfig.ExecutablePath = @"C:\Users\Nathanial\Documents\visual studio 2012\Projects\MultiMiner\MultiMiner.Win\bin\Debug\Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);

            List<Device> devices = miner.GetDevices();
            deviceBindingSource.DataSource = devices;

        }
    }
}
