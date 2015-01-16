using MultiMiner.Engine.Data;
using MultiMiner.Utility.Net;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    public partial class DetailsControl : MessageBoxFontUserControl
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

        private void SetupFonts()
        {
            //do this in code so font name isn't stored in the .cs
            hashrateLabel.Font = new Font(hashrateLabel.Font, FontStyle.Bold);
            tempLabel.Font = new Font(tempLabel.Font, FontStyle.Bold);
            poolLabel.Font = new Font(poolLabel.Font, FontStyle.Bold);
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Bold);
            nameLabel.Font = new Font(nameLabel.Font.Name, 12.0f);
            deviceCountLabel.Font = new Font(nameLabel.Font.Name, 12.0f);

        }
        private void PositionControls()
        {
            closeDetailsButton.Size = new Size(22, 22);
            const int offset = 2;
            closeDetailsButton.Location = new Point(Width - closeDetailsButton.Width - offset, 0 + offset);
            closeDetailsButton.BringToFront();

            workersGridView.Width = Width - (workersGridView.Left * 2);
            workersGridView.Height = Height - workersGridView.Top - 6;
        }

        public void ClearDetails(int deviceCount)
        {
            SetupFonts();

            deviceCountLabel.Text = deviceCount + " item";
            if (deviceCount > 1)
                deviceCountLabel.Text = deviceCountLabel.Text + "s";
            noDetailsPanel.Dock = DockStyle.Fill;
            noDetailsPanel.BringToFront();
            noDetailsPanel.Visible = true;
            closeDetailsButton.BringToFront();
            proxyInfoPanel.Visible = false;
        }

        public void InspectDetails(DeviceViewModel deviceViewModel, bool showWorkUtility)
        {            
            SetupFonts();

            noDetailsPanel.Visible = false;
                        
            hashrateLabel.Text = deviceViewModel.AverageHashrate.ToHashrateString();
            currentRateLabel.Text = deviceViewModel.CurrentHashrate.ToHashrateString();

            workersGridView.Visible = (deviceViewModel.Workers.Count > 0);
            workersTitleLabel.Visible = workersGridView.Visible;
            symbolLabel.Visible = (deviceViewModel.Coin != null) && (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin);

            SetupProxyDetails(deviceViewModel);

            SetupNetworkDeviceDetails(deviceViewModel);
                        
            //device may not be configured
            if (deviceViewModel.Coin != null)
                cryptoCoinBindingSource.DataSource = deviceViewModel.Coin;
            else
                cryptoCoinBindingSource.DataSource = new PoolGroup();
            cryptoCoinBindingSource.ResetBindings(false);

            deviceBindingSource.DataSource = deviceViewModel;
            deviceBindingSource.ResetBindings(false);

            workerBindingSource.DataSource = deviceViewModel.Workers;
            workerBindingSource.ResetBindings(false);

            SetupDevicePicture(deviceViewModel);

            nameLabel.Width = Width - nameLabel.Left - closeDetailsButton.Width;

            acceptedLabel.Text = deviceViewModel.AcceptedShares.ToString("#,#.###############");
            rejectedLabel.Text = deviceViewModel.RejectedShares.ToString("#,#.###############");
            errorsLabel.Text = deviceViewModel.HardwareErrors.ToString("#,#.###############");

            Binding bestShareBinding = bestShareLabel.DataBindings[0];
            if (deviceViewModel.BestShare > 32)
                bestShareBinding.FormatString = "#,#";
            else if (deviceViewModel.BestShare > 1)
                bestShareBinding.FormatString = "#,#.##";
            else
                bestShareBinding.FormatString = "#,#.######";

            if (showWorkUtility)
            {
                utilityLabel.Text = deviceViewModel.WorkUtility.ToString("#,#.###");
                utilityDataGridViewTextBoxColumn.DataPropertyName = "WorkUtility";
            }
            else
            {
                utilityLabel.Text = deviceViewModel.Utility.ToString("#,#.###");
                utilityDataGridViewTextBoxColumn.DataPropertyName = "Utility";
            }
            utilityPrefixLabel.Text = showWorkUtility ? "Work utility:" : "Utility:";

            if (deviceViewModel.Temperature > 0)
                tempLabel.Text = deviceViewModel.Temperature + "°";
            else
                tempLabel.Text = String.Empty;

            if (deviceViewModel.FanPercent > 0)
                fanLabel.Text = deviceViewModel.FanPercent + "%";
            else
                fanLabel.Text = String.Empty;

            UpdateColumnVisibility();
        }

        private void SetupDevicePicture(DeviceViewModel deviceViewModel)
        {
            switch (deviceViewModel.Kind)
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
                case DeviceKind.NET:
                    pictureBox1.Image = imageList1.Images[4];
                    break;
            }
        }

        private void SetupProxyDetails(DeviceViewModel deviceViewModel)
        {
            bool visible = false;

            if (deviceViewModel.Kind == DeviceKind.PXY)
            {
                string[] ports = deviceViewModel.Path.Split(':');
                if (ports.Length == 2)
                {
                    string localIPAddress = LocalNetwork.GetLocalIPAddress();
                    proxyGetworkLabel.Text = String.Format("http://{0}:{1}", localIPAddress, ports[0]);
                    proxyStratumLabel.Text = String.Format("stratum+tcp://{0}:{1}", localIPAddress, ports[1]);

                    visible = true;
                }
            }

            proxyInfoPanel.Visible = visible;
            if (visible)
                proxyInfoPanel.BringToFront();
        }

        private void SetupNetworkDeviceDetails(DeviceViewModel deviceViewModel)
        {
            serialTitleLabel.Visible = deviceViewModel.Kind != DeviceKind.NET;
            serialValueLabel.Visible = deviceViewModel.Kind != DeviceKind.NET;
            processorsTitleLabel.Visible = deviceViewModel.Kind != DeviceKind.NET;
            processorsValueLabel.Visible = deviceViewModel.Kind != DeviceKind.NET;

            if (deviceViewModel.Kind == DeviceKind.NET)
            {
                pathValueLabel.Width = Width - pathValueLabel.Left - 6;

                nameLabel.Cursor = Cursors.Hand;
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Underline);
                nameLabel.ForeColor = Color.Blue;

                statusLabel.Text = String.Format("{0}{1}{2}", deviceViewModel.ChainStatus[0], Environment.NewLine, deviceViewModel.ChainStatus[1]);
            }
            else
            {
                pathValueLabel.Width = serialTitleLabel.Left - pathValueLabel.Left - 6;                

                nameLabel.Cursor = Cursors.Default;
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Regular);
                nameLabel.ForeColor = Color.FromArgb(30, 57, 91);

                statusLabel.Text = String.Empty;
            }
        }

        private void UpdateColumnVisibility()
        {
            foreach (DataGridViewColumn column in workersGridView.Columns)
            {
                bool visible = workersGridView.Rows.Cast<DataGridViewRow>().Any(row => !String.IsNullOrEmpty((string) row.Cells[column.Index].FormattedValue));
                column.Visible = visible;
            }
        }

        private void workersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
                return;

            if (e.ColumnIndex == currentHashrateDataGridViewTextBoxColumn.Index)
            {
                e.Value = ((double)e.Value).ToHashrateString();
            }
            else if (e.ColumnIndex == hardwareErrorsPercentDataGridViewTextBoxColumn.Index)
            {
                //check for >= 0.05 so we don't show 0% (due to the format string)
                e.Value = (double)e.Value >= 0.05 ? ((double)e.Value).ToString("0.#") + "%" : String.Empty;
            }
            else if (e.ColumnIndex == temperatureDataGridViewTextBoxColumn.Index)
            {
                e.Value = (double)e.Value > 0 ? ((double)e.Value).ToString() + "°" : String.Empty;
            }
        }

        private void nameLabel_Click(object sender, EventArgs e)
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceBindingSource.DataSource;

            if (deviceViewModel.Kind == DeviceKind.NET)
            {
                Process.Start("http://" + deviceViewModel.Name);
            }
        }

        private void workersGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //don't allow data validation / formatting errors trigger the DataGridView error dialog
            e.ThrowException = false;
            e.Cancel = true;
        }

    }
}
