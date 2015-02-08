using MultiMiner.Engine;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.ViewModels;
using MultiMiner.Win.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms
{
    public partial class ApiConsoleControl : MessageBoxFontUserControl
    {
        readonly List<Xgminer.Api.ApiContext> apiContexts = new List<Xgminer.Api.ApiContext>();
        List<string> requests = new List<string>();
        int requestIndex = -1;

        public ApiConsoleControl(List<MinerProcess> localMiners, List<NetworkDevices.NetworkDevice> networkMiners, MinerFormViewModel viewModel)
        {
            InitializeComponent();
            inputTextBox.Enabled = false;
            PopulateMiners(localMiners, networkMiners, viewModel);
        }

        private void ApiConsoleForm_Load(object sender, EventArgs e)
        {
            LoadRpcApiHelp();
            ActiveControl = inputTextBox;
        }

        private bool populatingMiners;
        public void PopulateMiners(List<MinerProcess> localMiners, List<NetworkDevices.NetworkDevice> networkMiners, MinerFormViewModel viewModel)
        {
            populatingMiners = true;
            string previousSelection = (string)minerComboBox.SelectedItem;

            minerComboBox.Items.Clear();
            apiContexts.Clear();

            foreach (MinerProcess localMiner in localMiners)
            {
                string devicePath = String.Format("{0}:{1}", localMiner.ApiContext.IpAddress, localMiner.ApiContext.Port);
                minerComboBox.Items.Add(String.Format("{0} ({1})", localMiner.Miner.Name, devicePath));
                apiContexts.Add(new Xgminer.Api.ApiContext(localMiner.ApiContext.Port, localMiner.ApiContext.IpAddress));
            }

            if (minerComboBox.Items.Count > 0)
            {
                minerComboBox.Items.Add("-");
                apiContexts.Add(null);
            }

            foreach (NetworkDevices.NetworkDevice networkMiner in networkMiners)
            {
                string devicePath = String.Format("{0}:{1}", networkMiner.IPAddress, networkMiner.Port);
                DeviceViewModel deviceModel = viewModel.Devices.SingleOrDefault(d => d.Path.Equals(devicePath));
                //the Network Device may be offline
                //null check as view models may not yet be populated
                if ((deviceModel != null) && deviceModel.Visible)
                {
                    string minerName = viewModel.GetFriendlyDeviceName(devicePath, devicePath);
                    minerComboBox.Items.Add(String.Format("{0} ({1})", minerName, devicePath));
                    apiContexts.Add(new Xgminer.Api.ApiContext(networkMiner.Port, networkMiner.IPAddress));
                }
            }

            if ((minerComboBox.SelectedItem == null) && (minerComboBox.Items.Count > 0))
                minerComboBox.SelectedIndex = 0;

            if (!String.IsNullOrEmpty(previousSelection))
            {
                int previousSelectionIndex = minerComboBox.Items.IndexOf(previousSelection);
                if (previousSelectionIndex >= 0)
                    minerComboBox.SelectedIndex = previousSelectionIndex;
            }

            populatingMiners = false;
        }

        private void LoadRpcApiHelp()
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                const string RpcApiReadmeUrl = "https://raw.githubusercontent.com/luke-jr/bfgminer/bfgminer/README.RPC";
                string apiHelp = webClient.DownloadString(RpcApiReadmeUrl);
                apiHelp = apiHelp.Replace("\n", Environment.NewLine);
                helpTextBox.Text = apiHelp;
            }
            helpTextBox.Font = new Font(helpTextBox.Font.FontFamily, helpTextBox.Font.Size + 2);
            helpTextBox.Width = Width / 3;

            helpTextBox.SelectionStart = 0;
            helpTextBox.SelectionLength = 0;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string request = inputTextBox.Text;

            if (e.KeyCode == Keys.Return)
                e.Handled = HandleReplInput(request);
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
                e.Handled = HandleReplNavigation(e.KeyCode == Keys.Up);
        }

        private bool HandleReplNavigation(bool navigateUp)
        {
            if (navigateUp)
            {
                if (requestIndex >= 0)
                {
                    inputTextBox.Text = requests[requestIndex];
                    if (requestIndex > 0)
                        requestIndex--;
                }
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
            else
            {
                if (requestIndex < requests.Count - 1)
                {
                    requestIndex++;
                    inputTextBox.Text = requests[requestIndex];
                }
                else
                {
                    requestIndex = requests.Count - 1;
                    inputTextBox.Text = String.Empty;
                }
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }

            return true;
        }

        private bool HandleReplInput(string request)
        {
            if (request.Equals("clear", StringComparison.OrdinalIgnoreCase))
                outputTextBox.Clear();
            else
            {
                Xgminer.Api.ApiContext apiContext = apiContexts[minerComboBox.SelectedIndex];
                if (apiContext != null)
                {
                    string response = apiContext.GetResponse(request, Xgminer.Api.ApiContext.LongCommandTimeoutMs);
                    string requestResponse = String.Format("{0} => {1}{2}", request, Environment.NewLine, response);
                    outputTextBox.AppendText(Environment.NewLine + Environment.NewLine + requestResponse);
                }
                else
                    minerComboBox.Focus();
            }
            inputTextBox.Clear();
            if ((requests.Count == 0) || !requests.Last().Equals(request))
                requests.Add(request);
            requestIndex = requests.Count - 1;

            return true;
        }
        private void minerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (minerComboBox.SelectedItem == null)
                inputTextBox.Enabled = false;
            else
            {
                inputTextBox.Enabled = true;
                FocusInput();
            }
        }

        private void FocusInput()
        {
            if (!populatingMiners)
                inputTextBox.Focus();
        }

        private void helpTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            FocusInput();
        }

        private void outputTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            FocusInput();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/API-Console");
        }
    }
}
