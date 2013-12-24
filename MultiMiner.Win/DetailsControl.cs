using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiMiner.Xgminer;
using MultiMiner.Coin.Api;
using MultiMiner.Engine.Configuration;
using MultiMiner.Engine;
using MultiMiner.Xgminer.Api.Responses;
using MultiMiner.Win.Extensions;

namespace MultiMiner.Win
{
    public partial class DetailsControl : UserControl
    {
        //events
        //delegate declarations
        public delegate void CloseClickedHandler(object sender);

        //event declarations        
        public event CloseClickedHandler CloseClicked;

        public DetailsControl()
        {
            InitializeComponent();
        }

        private void closeApiButton_Click(object sender, EventArgs e)
        {
            if (CloseClicked != null)
                CloseClicked(this);
        }

        private void DetailsControl_Load(object sender, EventArgs e)
        {
            PositionControls();
        }

        private void PositionControls()
        {
            closeDetailsButton.Size = new Size(22, 22);
            const int offset = 2;
            closeDetailsButton.Location = new Point(this.Width - closeDetailsButton.Width - offset, 0 + offset);
            closeDetailsButton.BringToFront();

            dataGridView1.Width = this.Width - (dataGridView1.Left * 2);
            dataGridView1.Height = this.Height - dataGridView1.Top - 6;
        }

        public void InspectDetails(Device device, CoinConfiguration coinConfiguration, CoinInformation coinInformation,
            DeviceDetailsResponse deviceDetails, List<DeviceInformationResponse> deviceInformation)
        {
            double hashrate = 0;
            foreach (DeviceInformationResponse individualDevice in deviceInformation)
            {
                hashrate += individualDevice.AverageHashrate;
            }
            hashrateLabel.Text = hashrate.ToHashrateString();

            dataGridView1.Visible = (device.Kind == DeviceKind.PXY) &&
                (deviceInformation.Count > 0);

            //Internet or Coin API could be down
            if (coinInformation != null)
            {
            }

            //device may not be configured
            if (coinConfiguration != null)
                cryptoCoinBindingSource.DataSource = coinConfiguration.Coin;
            else
                cryptoCoinBindingSource.DataSource = new CryptoCoin();

            deviceInformationResponseBindingSource.DataSource = deviceInformation;

            //may not be hashing yet
            if (deviceDetails != null)
                deviceDetailsResponseBindingSource.DataSource = deviceDetails;
            else
                deviceDetailsResponseBindingSource.DataSource = new DeviceDetailsResponse();

            deviceBindingSource.DataSource = device;

            switch (device.Kind)
            {
                case DeviceKind.CPU:
                    pictureBox1.Image = imageList1.Images[3];
                    break;
                case DeviceKind.GPU:
                    pictureBox1.Image = imageList1.Images[0];
                    break;
                case DeviceKind.USB:
                    pictureBox1.Image = imageList1.Images[1];
                    break;
                case DeviceKind.PXY:
                    pictureBox1.Image = imageList1.Images[2];
                    break;
            }

            nameLabel.Width = this.Width - nameLabel.Left - closeDetailsButton.Width;
        }
    }
}
