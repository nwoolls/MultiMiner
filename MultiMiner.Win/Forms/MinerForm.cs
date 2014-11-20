using MultiMiner.Engine;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using MultiMiner.Win.Controls.Notifications;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Win.Extensions;
using MultiMiner.Win.Data.Configuration;
using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Remoting;
using MultiMiner.Services;
using MultiMiner.Discovery;
using MultiMiner.Discovery.Data;
using MultiMiner.Win.ViewModels;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using MultiMiner.Xgminer.Discovery;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.Win.Data;
using MultiMiner.Utility.Forms;
using MultiMiner.Win.Controls;
using MultiMiner.Win.Forms.Configuration;
using MultiMiner.Xgminer.Data;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Installers;
using MultiMiner.ExchangeApi.Data;
using System.Globalization;
using Microsoft.Win32;
using MultiMiner.MultipoolApi.Data;

namespace MultiMiner.Win.Forms
{
    public partial class MinerForm : MessageBoxFontForm
    {
        #region Private fields
        //API contexts
        private IApiContext coinChooseApiContext;
        private IApiContext coinWarzApiContext;
        private IApiContext whatMineApiContext;
        private IApiContext successfulApiContext;
        
        //coalesced timers
        private Timers timers = new Timers();

        //Coin API information
        private List<CoinInformation> coinApiInformation;
        private IEnumerable<ExchangeInformation> sellPrices;

        //RPC API information
        private Dictionary<string, double> minerNetworkDifficulty = new Dictionary<string, double>();
        private readonly Dictionary<MinerProcess, List<DeviceDetails>> processDeviceDetails = new Dictionary<MinerProcess, List<DeviceDetails>>();
        private readonly List<DeviceInformation> allDeviceInformation = new List<DeviceInformation>();

        //configuration
        private Engine.Data.Configuration.Engine engineConfiguration = new Engine.Data.Configuration.Engine();
        private Data.Configuration.Application applicationConfiguration = new Data.Configuration.Application();
        private readonly Paths pathConfiguration = new Paths();
        private Perks perksConfiguration = new Perks();
        private NetworkDevices networkDevicesConfiguration = new NetworkDevices();
        private Metadata metadataConfiguration = new Metadata();

        //hardware information
        private List<Xgminer.Data.Device> devices;
        private readonly Dictionary<DeviceViewModel, DeviceDetails> deviceDetailsMapping = new Dictionary<DeviceViewModel, DeviceDetails>();
        private readonly Dictionary<DeviceViewModel, string> lastDevicePoolMapping = new Dictionary<DeviceViewModel, string>();
        private readonly Dictionary<string, List<PoolInformation>> networkDevicePools = new Dictionary<string, List<PoolInformation>>();
        private readonly Dictionary<string, List<MinerStatistics>> networkDeviceStatistics = new Dictionary<string, List<MinerStatistics>>();
        private readonly Dictionary<string, VersionInformation> networkDeviceVersions = new Dictionary<string, VersionInformation>();

        //currently mining information
        private List<Engine.Data.Configuration.Device> miningDeviceConfigurations;
        private List<Engine.Data.Configuration.Coin> miningCoinConfigurations;

        //data sources
        private readonly List<ApiLogEntry> apiLogEntries = new List<ApiLogEntry>();
        private readonly List<LogLaunchArgs> logLaunchEntries = new List<LogLaunchArgs>();
        private readonly List<LogProcessCloseArgs> logCloseEntries = new List<LogProcessCloseArgs>();

        //fields
        private int startupMiningCountdownSeconds = 0;
        private int coinStatsCountdownMinutes = 0;
        private bool settingsLoaded = false;
        private readonly double difficultyMuliplier = Math.Pow(2, 32);
        private bool applicationSetup = false;
        private bool editingDeviceListView = false;
        private Action notificationClickHandler;

        //logic
        private List<PoolGroup> knownCoins = new List<PoolGroup>();
        private readonly MiningEngine miningEngine = new MiningEngine();
        private readonly List<int> processedCommandIds = new List<int>();
        private readonly List<MobileMiner.Data.Notification> queuedNotifications = new List<MobileMiner.Data.Notification>();

        //controls
        private NotificationsControl notificationsControl;
        private InstancesControl instancesControl;
        private ApiConsoleForm apiConsoleForm;

        //remoting
        private RemotingServer remotingServer;
        private Discovery.Listener discoveryListener;
        private Remoting.Broadcast.Listener broadcastListener;
        private int fingerprint;
        private Random random = new Random();
        private Instance selectedRemoteInstance = null;
        public bool remotingEnabled { get; set; }
        public bool remoteInstanceMining { get; set; }
        Remoting.Data.Transfer.Configuration.Perks remotePerksConfig;
        Remoting.Data.Transfer.Configuration.Path remotePathConfig;
        Remoting.Data.Transfer.Configuration.Engine remoteEngineConfig;
        Remoting.Data.Transfer.Configuration.Application remoteApplicationConfig;

        //view models
        private MinerFormViewModel localViewModel = new MinerFormViewModel();
        private MinerFormViewModel remoteViewModel = new MinerFormViewModel();
        #endregion

        #region Constructor
        public MinerForm()
        {
            InitializeComponent();

            pathConfiguration.LoadPathConfiguration();

            instancesContainer.Panel1Collapsed = true;
            detailsAreaContainer.Panel2Collapsed = true;

            applicationConfiguration.LoadApplicationConfiguration(pathConfiguration.SharedConfigPath);

            if (applicationConfiguration.StartupMinimized)
                this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region View life-cycle
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupApplication();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TearDownApplication();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            deviceListView.Focus();
        }
        #endregion

        #region View setup
        private void PositionAdvancedAreaCloseButton()
        {
            closeApiButton.Parent = advancedAreaContainer.Panel2;
            closeApiButton.BringToFront();
            panel2.Visible = false;
        }

        private void SetupInstancesControl()
        {
            instancesControl = new InstancesControl();
            instancesControl.Dock = DockStyle.Fill;
            instancesControl.SelectedInstanceChanged += instancesControl1_SelectedInstanceChanged;
            instancesControl.Parent = instancesContainer.Panel1;
        }

        private void SetupLookAndFeel()
        {
            Version win8version = new Version(6, 2, 9200, 0);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8version)
            {
                // its win8 or higher.
                accessibleMenu.BackColor = SystemColors.ControlLightLight;
                standardToolBar.BackColor = SystemColors.ControlLightLight;
            }
            else
            {
                accessibleMenu.BackColor = SystemColors.Control;
                standardToolBar.BackColor = SystemColors.Control;
            }

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
            {
                accessibleMenu.BackColor = SystemColors.Control;
                standardToolBar.BackColor = SystemColors.Control;
                statusStrip1.BackColor = SystemColors.Control;
            }
        }

        private void SetupAccessibleMenu()
        {
            if (accessibleMenu.Visible != applicationConfiguration.UseAccessibleMenu)
            {
                accessibleMenu.Visible = applicationConfiguration.UseAccessibleMenu;
                standardToolBar.Visible = !applicationConfiguration.UseAccessibleMenu;
            }
        }

        private void SetupInitialButtonVisibility()
        {
            saveButton.Visible = false;
            cancelButton.Visible = false;
            saveSeparator.Visible = false;
            stopButton.Visible = false;
        }

        private void SetupNotificationsControl()
        {
            Control parent = detailsAreaContainer.Panel1;

            //carefully measured to fit notifications when they scroll
            const int ControlOffset = 2;
            const int ControlHeight = 148;
            const int ControlWidth = 358;

            this.notificationsControl = new NotificationsControl()
            {
                Visible = false,
                Height = ControlHeight,
                Width = ControlWidth,
                Parent = parent,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            notificationsControl.NotificationsChanged += notificationsControl1_NotificationsChanged;
            notificationsControl.NotificationAdded += notificationsControl1_NotificationAdded;

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                //adjust for different metrics/layout under OS X/Unix
                notificationsControl.Width += 50;

            //base this on control.Width, not ControlWidth
            notificationsControl.Left = parent.Width - notificationsControl.Width - ControlOffset;
            //same here
            notificationsControl.Top = parent.Height - notificationsControl.Height - ControlOffset;
        }

        private void SetupGridColumns()
        {
            //format prices in the History grid
            startPriceColumn.DefaultCellStyle.Format = ".########";
            endPriceColumn.DefaultCellStyle.Format = ".########";
        }

        private void PositionCoinChooseLabels()
        {
            //so things align correctly under Mono
            coinApiLinkLabel.Left = coinChoosePrefixLabel.Left + coinChoosePrefixLabel.Width;
            coinChooseSuffixLabel.Left = coinApiLinkLabel.Left + coinApiLinkLabel.Width;
        }

        private void SetupStatusBarLabelLayouts()
        {
            hashRateStatusLabel.AutoSize = true;
            hashRateStatusLabel.Spring = true;
            
            deviceTotalLabel.AutoSize = true;
            deviceTotalLabel.Padding = new Padding(12, 0, 0, 0);

            strategyCountdownLabel.Visible = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
        }
        #endregion

        #region Model / ViewModel behavior
        private void ApplyModelsToViewModel()
        {
            ApplyDevicesToViewModel();
            localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                engineConfiguration.CoinConfigurations);
            ApplyCoinInformationToViewModel();
            localViewModel.ApplyCoinConfigurationModels(engineConfiguration.CoinConfigurations);
        }

        private void ApplyDevicesToViewModel()
        {
            //ApplyDeviceModels() ensures we have a 1-to-1 with listview items
            localViewModel.ApplyDeviceModels(devices, networkDevicesConfiguration.Devices, metadataConfiguration.Devices);
        }

        private void ApplyCoinInformationToViewModel()
        {
            if (coinApiInformation != null)
                localViewModel.ApplyCoinInformationModels(coinApiInformation
                    .ToList()); //get a copy - populated async & collection may be modified)
        }
        #endregion

        #region View / ViewModel behavior
        private MinerFormViewModel GetViewModelToView()
        {
            MinerFormViewModel viewModelToView = localViewModel;
            if (this.selectedRemoteInstance != null)
                viewModelToView = remoteViewModel;
            return viewModelToView;
        }

        private void RemoveListViewItemsMissingFromViewModel(MinerFormViewModel viewModelToView)
        {
            for (int i = deviceListView.Items.Count - 1; i >= 0; i--)
            {
                DeviceViewModel listModel = (DeviceViewModel)deviceListView.Items[i].Tag;
                if (!viewModelToView.Devices.Contains(listModel) || !listModel.Visible)
                    deviceListView.Items.RemoveAt(i);

                //Network Device detection disabled
                if (!applicationConfiguration.NetworkDeviceDetection && (listModel.Kind == DeviceKind.NET))
                    deviceListView.Items.RemoveAt(i);
            }
        }

        private ListViewItem GetListViewItemForDeviceViewModel(DeviceViewModel deviceViewModel)
        {
            foreach (ListViewItem item in deviceListView.Items)
            {
                if (item.Tag == deviceViewModel)
                    return item;
            }
            return null;
        }

        private ListViewItem FindOrAddListViewItemForViewModel(DeviceViewModel deviceViewModel)
        {
            ListViewItem listViewItem = GetListViewItemForDeviceViewModel(deviceViewModel);
            if (listViewItem != null)
                return listViewItem;

            listViewItem = new ListViewItem();

            switch (deviceViewModel.Kind)
            {
                case DeviceKind.CPU:
                    listViewItem.Group = deviceListView.Groups["cpuListViewGroup"];
                    listViewItem.ImageIndex = 3;
                    break;
                case DeviceKind.GPU:
                    listViewItem.Group = deviceListView.Groups["gpuListViewGroup"];
                    listViewItem.ImageIndex = 0;
                    break;
                case DeviceKind.USB:
                    listViewItem.Group = deviceListView.Groups["usbListViewGroup"];
                    listViewItem.ImageIndex = 1;
                    break;
                case DeviceKind.PXY:
                    listViewItem.Group = deviceListView.Groups["proxyListViewGroup"];
                    listViewItem.ImageIndex = 2;
                    break;
                case DeviceKind.NET:
                    listViewItem.Group = deviceListView.Groups["networkListViewGroup"];
                    listViewItem.ImageIndex = 4;
                    break;
            }

            listViewItem.Text = deviceViewModel.Name;

            //start at i = 1, skip the first column
            for (int i = 1; i < deviceListView.Columns.Count; i++)
            {
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(listViewItem, String.Empty)
                {
                    Name = deviceListView.Columns[i].Text,
                    ForeColor = SystemColors.WindowFrame
                });
            }

            listViewItem.SubItems["Coin"].ForeColor = SystemColors.WindowText;
            listViewItem.SubItems["Errors"].ForeColor = SystemColors.WindowText;
            listViewItem.SubItems["Rejected"].ForeColor = SystemColors.WindowText;

            listViewItem.UseItemStyleForSubItems = false;

            listViewItem.Checked = deviceViewModel.Enabled;

            listViewItem.SubItems["Driver"].Text = deviceViewModel.Driver;

            //Tag must be set before adding as it is used for sorting
            listViewItem.Tag = deviceViewModel;

            deviceListView.Items.Add(listViewItem);

            return listViewItem;
        }

        private double WorkUtilityToHashrate(double workUtility)
        {
            //this will be wrong for Scrypt until 3.10.1
            double multiplier = 71582788 / 1000;
            double hashrate = workUtility * multiplier;
            return hashrate;
        }

        private string GetCurrentCultureCurrency()
        {
            string currencySymbol = RegionInfo.CurrentRegion.ISOCurrencySymbol;
            if ((sellPrices != null) && (sellPrices.SingleOrDefault(sp => sp.TargetCurrency.Equals(currencySymbol)) == null))
                currencySymbol = "USD";

            return currencySymbol;
        }

        private void RefreshListViewFromViewModel()
        {
            if (editingDeviceListView)
                return;

            List<int> selectedIndexes = new List<int>();
            foreach (int selectedIndex in deviceListView.SelectedIndices)
                selectedIndexes.Add(selectedIndex);

            deviceListView.BeginUpdate();
            this.updatingListView = true;
            try
            {
                try
                {
                    utilityColumnHeader.Text = applicationConfiguration.ShowWorkUtility ? "Work Utility" : "Utility";
                }
                catch (InvalidOperationException)
                {
                    //user was resizing columns
                }

                //clear all coin stats first
                //there may be coins configured that are no longer returned in the stats
                ClearAllCoinStats();

                MinerFormViewModel viewModelToView = GetViewModelToView();

                RemoveListViewItemsMissingFromViewModel(viewModelToView);

                for (int i = 0; i < viewModelToView.Devices.Count; i++)
                {
                    DeviceViewModel deviceViewModel = viewModelToView.Devices[i];
                    if (!deviceViewModel.Visible)
                        continue;

                    //Network Devices should only show from the Local ViewModel
                    if ((viewModelToView == remoteViewModel) &&
                        (deviceViewModel.Kind == DeviceKind.NET))
                        continue;

                    //Network Devices disabled
                    if (!applicationConfiguration.NetworkDeviceDetection &&
                        (deviceViewModel.Kind == DeviceKind.NET))
                        continue;

                    //app is closing
                    if (tearingDown)
                        break;

                    ListViewItem listViewItem = FindOrAddListViewItemForViewModel(deviceViewModel);
                    

                    if (!String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                        listViewItem.Text = deviceViewModel.FriendlyName;

                    /* configuration info
                     * */
                    listViewItem.Checked = deviceViewModel.Enabled;

                    if (listViewItem.Checked)
                    {
                        listViewItem.ForeColor = SystemColors.WindowText;
                        listViewItem.UseItemStyleForSubItems = false;
                    }
                    else
                    {
                        listViewItem.ForeColor = SystemColors.GrayText;
                        listViewItem.UseItemStyleForSubItems = true;
                    }

                    /* Coin info
                     * */
                    //check for Coin != null, device may not have a coin configured
                    //Network Devices assume BTC (for now)
                    if (deviceViewModel.Coin == null)
                    {
                        listViewItem.SubItems["Coin"].Text = String.Empty;
                        listViewItem.SubItems["Difficulty"].Text = String.Empty;
                        listViewItem.SubItems["Price"].Text = String.Empty;
                        listViewItem.SubItems["Exchange"].Text = String.Empty;
                        listViewItem.SubItems["Profitability"].Text = String.Empty;
                    }
                    else
                    {
                        listViewItem.SubItems["Coin"].Text = deviceViewModel.Coin.Name;

                        double difficulty = GetCachedNetworkDifficulty(deviceViewModel.Pool ?? String.Empty);
                        if (difficulty == 0.0)
                            difficulty = deviceViewModel.Difficulty;

                        listViewItem.SubItems["Difficulty"].Tag = difficulty;
                        listViewItem.SubItems["Difficulty"].Text = difficulty.ToDifficultyString();

                        if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                        {
                            string unit = KnownCoins.BitcoinSymbol;
                            listViewItem.SubItems["Price"].Text = String.Format("{0} {1}", deviceViewModel.Price.ToFriendlyString(), unit);

                            //check .Mining to allow perks for Remoting when local PC is not mining
                            if ((miningEngine.Donating || !miningEngine.Mining) && perksConfiguration.ShowExchangeRates
                            //ensure Exchange prices are available:
                            && (sellPrices != null))
                            {
                                ExchangeInformation exchangeInformation = sellPrices.Single(er => er.TargetCurrency.Equals(GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
                                double btcExchangeRate = exchangeInformation.ExchangeRate;
                                double coinExchangeRate = 0.00;

                                coinExchangeRate = deviceViewModel.Price * btcExchangeRate;

                                listViewItem.SubItems["Exchange"].Tag = coinExchangeRate;
                                listViewItem.SubItems["Exchange"].Text = String.Format("{0}{1}", exchangeInformation.TargetSymbol, coinExchangeRate.ToFriendlyString(true));
                            }
                        }

                        switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                        {
                            case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                                listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.AdjustedProfitability, 2) + "%";
                                break;
                            case Strategy.CoinProfitabilityKind.AverageProfitability:
                                listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.AverageProfitability, 2) + "%";
                                break;
                            case Strategy.CoinProfitabilityKind.StraightProfitability:
                                listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.Profitability, 2) + "%";
                                break;
                        }
                    }

                    /* device info
                     * */
                    listViewItem.SubItems["Average"].Text = deviceViewModel.AverageHashrate == 0 ? String.Empty : deviceViewModel.AverageHashrate.ToHashrateString();
                    listViewItem.SubItems["Current"].Text = deviceViewModel.CurrentHashrate == 0 ? String.Empty : deviceViewModel.CurrentHashrate.ToHashrateString();

                    //ensure column is visible in Tiles view
                    if (String.IsNullOrEmpty(listViewItem.SubItems["Current"].Text) &&
                        (deviceListView.View == View.Tile))
                        listViewItem.SubItems["Current"].Text = "Idle";

                    double hashrate = WorkUtilityToHashrate(deviceViewModel.WorkUtility);
                    listViewItem.SubItems["Effective"].Text = hashrate == 0 ? String.Empty : hashrate.ToHashrateString();

                    //check for >= 0.05 so we don't show 0% (due to the format string)
                    listViewItem.SubItems["Rejected"].Text = deviceViewModel.RejectedSharesPercent >= 0.05 ? deviceViewModel.RejectedSharesPercent.ToString("0.#") + "%" : String.Empty;
                    listViewItem.SubItems["Errors"].Text = deviceViewModel.HardwareErrorsPercent >= 0.05 ? deviceViewModel.HardwareErrorsPercent.ToString("0.#") + "%" : String.Empty;
                    listViewItem.SubItems["Accepted"].Text = deviceViewModel.AcceptedShares > 0 ? deviceViewModel.AcceptedShares.ToString() : String.Empty;

                    if (applicationConfiguration.ShowWorkUtility)
                        listViewItem.SubItems[utilityColumnHeader.Text].Text = deviceViewModel.WorkUtility > 0.00 ? deviceViewModel.WorkUtility.ToString("0.###") : String.Empty;
                    else
                        listViewItem.SubItems[utilityColumnHeader.Text].Text = deviceViewModel.Utility > 0.00 ? deviceViewModel.Utility.ToString("0.###") : String.Empty;

                    listViewItem.SubItems["Temp"].Text = deviceViewModel.Temperature > 0 ? deviceViewModel.Temperature + "°" : String.Empty;
                    listViewItem.SubItems["Fan"].Text = deviceViewModel.FanPercent > 0 ? deviceViewModel.FanPercent + "%" : String.Empty;
                    listViewItem.SubItems["Intensity"].Text = deviceViewModel.Intensity;

                    listViewItem.SubItems["Pool"].Text = deviceViewModel.Pool.DomainFromHost();

                    PopulateIncomeForListViewItem(listViewItem, deviceViewModel);
                }

                foreach (int selectedIndex in selectedIndexes)
                    //selectedIndex may be -1, check for that
                    if ((selectedIndex >= 0) && (selectedIndex < deviceListView.Items.Count))
                        deviceListView.Items[selectedIndex].Selected = true;
            }
            finally
            {
                deviceListView.EndUpdate();
                this.updatingListView = false;
            }
        }
        #endregion

        #region View / Model behavior
        private void SaveChanges()
        {
            if (this.selectedRemoteInstance == null)
            {
                SaveChangesLocally();
            }
            else
            {
                SaveChangesRemotely(this.selectedRemoteInstance);
            }
        }

        private void SaveChangesLocally()
        {
            SaveViewModelValuesToConfiguration();
            engineConfiguration.SaveDeviceConfigurations();

            localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                engineConfiguration.CoinConfigurations);

            SetHasChangesLocally(false);

            System.Windows.Forms.Application.DoEvents();

            UpdateMiningButtons();
            ClearMinerStatsForDisabledCoins();

            //update coin stats now that we saved coin changes
            RefreshListViewFromViewModel();

            //take into account above changes
            AutoSizeListViewColumns();
        }

        private void CancelChanges()
        {
            if (this.selectedRemoteInstance == null)
            {
                CancelChangesLocally();
            }
            else
            {
                CancelChangesRemotely(this.selectedRemoteInstance);
            }
        }
        private void CancelChangesLocally()
        {
            engineConfiguration.LoadDeviceConfigurations();

            localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                engineConfiguration.CoinConfigurations);

            RefreshListViewFromViewModel();

            SetHasChangesLocally(false);
            AutoSizeListViewColumns();
        }
        #endregion

        #region View population
        private void SetHasChanges(bool hasChanges)
        {
            if (this.selectedRemoteInstance == null)
            {
                SetHasChangesLocally(hasChanges);
            }
            else
            {
                SetHasChangesRemotely(hasChanges);
            }
        }

        private void SetHasChangesLocally(bool hasChanges)
        {
            this.localViewModel.HasChanges = hasChanges;

            if (this.selectedRemoteInstance == null)
            {
                saveButton.Visible = hasChanges;
                cancelButton.Visible = hasChanges;
                saveSeparator.Visible = hasChanges;

                saveButton.Enabled = hasChanges;
                cancelButton.Enabled = hasChanges;

                //accessible menu
                saveToolStripMenuItem.Enabled = hasChanges;
                cancelToolStripMenuItem.Enabled = hasChanges;
            }
        }

        private void SetHasChangesRemotely(bool hasChanges)
        {
            this.remoteViewModel.HasChanges = hasChanges;

            if (this.selectedRemoteInstance != null)
            {
                saveButton.Visible = hasChanges;
                cancelButton.Visible = hasChanges;
                saveSeparator.Visible = hasChanges;

                saveButton.Enabled = hasChanges;
                cancelButton.Enabled = hasChanges;

                //accessible menu
                saveToolStripMenuItem.Enabled = hasChanges;
                cancelToolStripMenuItem.Enabled = hasChanges;
            }
        }

        private void RefreshDetailsAreaIfVisible()
        {
            if (!detailsAreaContainer.Panel2Collapsed)
                RefreshDetailsArea();
        }

        private void RefreshDetailsArea()
        {
            MinerFormViewModel viewModelToView = GetViewModelToView();

            if (deviceListView.SelectedItems.Count == 0)
            {
                detailsControl1.ClearDetails(viewModelToView.Devices.Count);
                return;
            }

            DeviceViewModel selectedDevice = (DeviceViewModel)deviceListView.SelectedItems[0].Tag;

            detailsControl1.InspectDetails(selectedDevice, applicationConfiguration.ShowWorkUtility);
        }

        private void RefreshCoinPopupMenu()
        {
            coinPopupMenu.Items.Clear();

            MinerFormViewModel viewModelToView = GetViewModelToView();
            foreach (PoolGroup configuredCoin in viewModelToView.ConfiguredCoins)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem()
                {
                    Text = configuredCoin.Name,
                    Tag = configuredCoin.Id
                };
                menuItem.Click += CoinMenuItemClick;

                if (viewModelToView.ConfiguredCoins.Count > 10)
                {
                    //if there are more than 10 Coin Configurations, break up the menu by algo
                    ToolStripMenuItem algoItem = FindOrCreateAlgoMenuItem(coinPopupMenu.Items, configuredCoin.Algorithm);
                    algoItem.DropDownItems.Add(menuItem);
                }
                else
                {
                    coinPopupMenu.Items.Add(menuItem);
                }
            }
        }

        private static ToolStripMenuItem FindOrCreateAlgoMenuItem(ToolStripItemCollection parent, string algorithm)
        {
            ToolStripMenuItem algoItem = null;

            foreach (ToolStripMenuItem item in parent)
            {
                if (item.Text.Equals(algorithm.ToString()))
                {
                    algoItem = item;
                    break;
                }
            }

            if (algoItem == null)
            {
                algoItem = new ToolStripMenuItem()
                {
                    Text = algorithm.ToString()
                };
                parent.Add(algoItem);
            }

            return algoItem;
        }

        private void RefreshCountdownLabel()
        {
            startupMiningPanel.Left = (this.Width / 2) - (startupMiningPanel.Width / 2);
            startupMiningPanel.Top = (this.Height / 2) - (startupMiningPanel.Height / 2);

            countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", startupMiningCountdownSeconds);
            startupMiningPanel.Visible = true;
        }

        private void UpdateUtilityColumnHeader()
        {
            ChangeSubItemText(deviceListView, utilityColumnHeader, applicationConfiguration.ShowWorkUtility ? "Work Utility" : "Utility");
        }

        private void ChangeSubItemText(ListView listView, ColumnHeader columnHeader, string caption)
        {
            string oldValue = columnHeader.Text;
            columnHeader.Text = caption;
            foreach (ListViewItem item in deviceListView.Items)
                item.SubItems[oldValue].Name = columnHeader.Text;
        }

        private void SetupColumnHeaderMenu()
        {
            columnHeaderMenu.Items.Clear();

            foreach (ColumnHeader column in deviceListView.Columns)
            {
                if ((column == nameColumnHeader)
                    || (column == coinColumnHeader))
                    continue;

                string columnText = column.Text;

                ToolStripMenuItem menuItem = (ToolStripMenuItem)columnHeaderMenu.Items.Add(columnText);
                menuItem.Checked = !applicationConfiguration.HiddenColumns.Contains(columnText);

                menuItem.Click += ColumnHeaderMenuClick;
            }
        }

        private const int maxColumnWidth = 195;
        //optimized for speed        
        private static void SetColumWidth(ColumnHeader column, int width)
        {
            if ((width < 0) || (column.Width != width))
                column.Width = width;
            if (column.Width > maxColumnWidth)
                column.Width = maxColumnWidth;
        }

        private void AutoSizeListViewColumns()
        {
            if (editingDeviceListView)
                return;

            if (deviceListView.View != View.Details)
                return;

            deviceListView.BeginUpdate();
            try
            {
                if (briefMode)
                {
                    SetColumWidth(nameColumnHeader, -2);
                    SetColumWidth(driverColumnHeader, 0);
                    SetColumWidth(coinColumnHeader, -2);
                    SetColumWidth(difficultyColumnHeader, 0);
                    SetColumWidth(priceColumnHeader, 0);
                    SetColumWidth(profitabilityColumnHeader, -2);
                    SetColumWidth(poolColumnHeader, 0);

                    if (ListViewColumnHasValues("Temp"))
                        SetColumWidth(tempColumnHeader, -2);
                    else if (tempColumnHeader.Width != 0)
                        SetColumWidth(tempColumnHeader, 0);

                    SetColumWidth(hashrateColumnHeader, -2);
                    SetColumWidth(currentRateColumnHeader, 0);
                    SetColumWidth(acceptedColumnHeader, 0);
                    SetColumWidth(rejectedColumnHeader, 0);
                    SetColumWidth(errorsColumnHeader, 0);
                    SetColumWidth(utilityColumnHeader, 0);
                    SetColumWidth(intensityColumnHeader, 0);
                    SetColumWidth(fanColumnHeader, 0);
                    SetColumWidth(incomeColumnHeader, 0);
                    SetColumWidth(exchangeColumnHeader, 0);
                }
                else
                {
                    for (int i = 0; i < deviceListView.Columns.Count; i++)
                    {
                        ColumnHeader column = deviceListView.Columns[i];

                        if (applicationConfiguration.HiddenColumns.Contains(column.Text))
                        {
                            SetColumWidth(column, 0);
                            continue;
                        }

                        bool hasValue = false;
                        if (i == 0)
                            hasValue = true;
                        else
                            hasValue = ListViewColumnHasValues(column.Text);

                        if (hasValue)
                            SetColumWidth(column, -2);
                        else
                            SetColumWidth(column, 0);
                    }
                }
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private bool ListViewColumnHasValues(string headerText)
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (!String.IsNullOrEmpty(item.SubItems[headerText].Text))
                    return true;
            return false;
        }

        private void AutoSizeListViewColumnsEvery(int count)
        {
            autoSizeColumnsFlag++;
            if (autoSizeColumnsFlag == count)
            {
                autoSizeColumnsFlag = 0;
                AutoSizeListViewColumns();
            }
        }
        private ushort autoSizeColumnsFlag = 0;

        private IApiContext GetEffectiveApiContext()
        {
            if (this.successfulApiContext != null)
                return this.successfulApiContext;
            else if (this.applicationConfiguration.UseCoinWarzApi)
                return this.coinWarzApiContext;
            else if (this.applicationConfiguration.UseWhatMineApi)
                return this.whatMineApiContext;
            else
                return this.coinChooseApiContext;
        }

        private void RefreshCoinApiLabel()
        {
            coinApiLinkLabel.Text = this.GetEffectiveApiContext().GetApiName();

            PositionCoinChooseLabels();
        }

        private void UpdateMiningButtons()
        {
            if (this.selectedRemoteInstance == null)
            {
                UpdateMiningButtonsForLocal();
            }
            else
            {
                UpdateMiningButtonsForRemote();
            }
        }

        private void UpdateMiningButtonsForRemote()
        {
            startButton.Enabled = !remoteInstanceMining;

            stopButton.Enabled = remoteInstanceMining;
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;

            //no remote detecting devices (yet)
            detectDevicesButton.Enabled = !remoteInstanceMining;
            detectDevicesToolStripMenuItem.Enabled = detectDevicesButton.Enabled;

            startButton.Visible = startButton.Enabled;
            stopButton.Visible = stopButton.Enabled;

            if (!startButton.Visible && !stopButton.Visible)
                startButton.Visible = true; //show something, even if disabled

            //sys tray menu
            startMenuItem.Visible = startMenuItem.Enabled;
            stopMenuItem.Visible = stopMenuItem.Enabled;

            //accessible menu
            startToolStripMenuItem.Enabled = startMenuItem.Enabled;
            stopToolStripMenuItem.Enabled = stopMenuItem.Enabled;
            restartToolStripMenuItem.Enabled = stopMenuItem.Enabled;
            scanHardwareToolStripMenuItem.Enabled = detectDevicesButton.Enabled;

            //process log menu
            launchToolStripMenuItem.Enabled = startMenuItem.Enabled;
        }

        private void UpdateMiningButtonsForLocal()
        {
            startButton.Enabled = MiningConfigurationValid() && !miningEngine.Mining;// && (this.selectedRemoteInstance == null);

            stopButton.Enabled = miningEngine.Mining;// && (this.selectedRemoteInstance == null);
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;
            //allow clicking Detect Devices with invalid configuration
            detectDevicesButton.Enabled = !miningEngine.Mining && (this.selectedRemoteInstance == null);
            detectDevicesToolStripMenuItem.Enabled = detectDevicesButton.Enabled;

            startButton.Visible = startButton.Enabled;
            stopButton.Visible = stopButton.Enabled;

            if (!startButton.Visible && !stopButton.Visible)
                startButton.Visible = true; //show something, even if disabled

            //sys tray menu
            startMenuItem.Visible = startMenuItem.Enabled;
            stopMenuItem.Visible = stopMenuItem.Enabled;

            //accessible menu
            startToolStripMenuItem.Enabled = startMenuItem.Enabled;
            stopToolStripMenuItem.Enabled = stopMenuItem.Enabled;
            restartToolStripMenuItem.Enabled = stopMenuItem.Enabled;
            scanHardwareToolStripMenuItem.Enabled = detectDevicesButton.Enabled;

            //process log menu
            launchToolStripMenuItem.Enabled = startMenuItem.Enabled;
        }

        private void RefreshQuickSwitchMenu(ToolStripDropDownItem parent)
        {
            quickCoinMenu.Items.Clear();

            MinerFormViewModel viewModelToView = GetViewModelToView();
            List<PoolGroup> coinConfigurations = viewModelToView.ConfiguredCoins;
            
            foreach (PoolGroup coinConfiguration in coinConfigurations)
            {
                ToolStripMenuItem coinSwitchItem = new ToolStripMenuItem()
                {
                    Text = coinConfiguration.Name,
                    Tag = coinConfiguration.Id
                };
                coinSwitchItem.Click += HandleQuickSwitchClick;

                if (coinConfigurations.Count > 10)
                {
                    //if there are more than 10 Coin Configurations, break up the menu by algo
                    ToolStripMenuItem algoItem = FindOrCreateAlgoMenuItem(quickCoinMenu.Items, coinConfiguration.Algorithm);
                    algoItem.DropDownItems.Add(coinSwitchItem);

                }
                else
                {
                    quickCoinMenu.Items.Add(coinSwitchItem);
                }
            }

            //Mono under Linux absolutely doesn't like having one context menu assigned to multiple
            //toolstrip items' DropDown property at once, so we have to target a single one here
            parent.DropDown = quickCoinMenu;
        }

        private static string GetPoolNameByIndex(Engine.Data.Configuration.Coin coinConfiguration, int poolIndex)
        {
            string result = String.Empty;

            if (poolIndex >= 0)
            {
                if (coinConfiguration != null)
                {
                    //the poolIndex may be greater than the Pools count if donating
                    if (poolIndex < coinConfiguration.Pools.Count)
                    {
                        result = coinConfiguration.Pools[poolIndex].Host;
                    }
                }
            }

            return result;
        }

        private void ClearAllCoinStats()
        {
            deviceListView.BeginUpdate();
            try
            {
                foreach (ListViewItem item in deviceListView.Items)
                    ClearCoinStatsForGridListViewItem(item);
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private void RefreshStrategiesLabel()
        {
            if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                strategiesLabel.Text = " Strategies: enabled";
            else
                strategiesLabel.Text = " Strategies: disabled";
        }

        private void RefreshStrategiesCountdown()
        {
            //Time until strategy check: 60s
            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                strategyCountdownLabel.Text = string.Format("Time until strategy check: {0}m", coinStatsCountdownMinutes);
            else
                strategyCountdownLabel.Text = "";
        }

        private void RemoveInvalidCoinValuesFromListView()
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Enabled && c.PoolGroup.Name.Equals(item.SubItems["Coin"].Text)) == null)
                    item.SubItems["Coin"].Text = String.Empty;

            ClearCoinStatsForDisabledCoins();
        }

        private void ClearMinerStatsForDisabledCoins()
        {
            if (saveButton.Enabled) //otherwise cleared coin isn't saved yet
                return;

            deviceListView.BeginUpdate();
            try
            {
                foreach (ListViewItem item in deviceListView.Items)
                    if (!item.Checked)
                        ClearDeviceInfoForListViewItem(item);
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private void ClearDeviceInfoForListViewItem(ListViewItem item)
        {
            item.SubItems["Temp"].Text = String.Empty;
            item.SubItems["Average"].Text = String.Empty;
            item.SubItems["Current"].Text = String.Empty;
            item.SubItems["Effective"].Text = String.Empty;
            item.SubItems["Accepted"].Text = String.Empty;
            item.SubItems["Rejected"].Text = String.Empty;
            item.SubItems["Errors"].Text = String.Empty;
            item.SubItems[utilityColumnHeader.Text].Text = String.Empty;
            item.SubItems["Intensity"].Text = String.Empty;
            item.SubItems["Pool"].Text = String.Empty;
            item.SubItems["Fan"].Text = String.Empty;
            item.SubItems["Daily"].Text = String.Empty;
        }

        private void PopulateIncomeForListViewItem(ListViewItem item, DeviceViewModel deviceViewModel)
        {
            item.SubItems["Daily"].Text = String.Empty;

            //check for Coin != null, device may not have a coin configured
            if (deviceViewModel.Coin == null)
                return;

            //check .Mining to allow perks for Remoting when local PC is not mining
            if (!((miningEngine.Donating || !miningEngine.Mining) && perksConfiguration.ShowIncomeRates))
                return;

            if (coinApiInformation == null)
                //no internet or error parsing API
                return;

            if (sellPrices == null)
                //no internet or error parsing API
                return;

            CoinInformation info = coinApiInformation
                .ToList() //get a copy - populated async & collection may be modified
                .SingleOrDefault(c => c.Symbol.Equals(deviceViewModel.Coin.Id, StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                ExchangeInformation exchangeInformation = sellPrices.Single(er => er.TargetCurrency.Equals(GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));

                if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                {
                    double difficulty = (double)item.SubItems["Difficulty"].Tag;
                    double hashrate = deviceViewModel.CurrentHashrate * 1000;
                    double fullDifficulty = difficulty * difficultyMuliplier;
                    double secondsToCalcShare = fullDifficulty / hashrate;
                    const double secondsPerDay = 86400;
                    double sharesPerDay = secondsPerDay / secondsToCalcShare;
                    double btcPerDay = sharesPerDay * info.Reward;

                    deviceViewModel.Daily = btcPerDay;
                }
                else
                {
                    //info.Price is in BTC/Ghs/Day
                    deviceViewModel.Daily = info.Price * deviceViewModel.CurrentHashrate / 1000 / 1000;
                }

                if (perksConfiguration.ShowExchangeRates && perksConfiguration.ShowIncomeInUsd)
                {
                    double exchangeRate = exchangeInformation.ExchangeRate;
                    if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                        //item.SubItems["Exchange"].Tag may be null
                        exchangeRate = item.SubItems["Exchange"].Tag == null ? 0 : (double)item.SubItems["Exchange"].Tag;

                    double fiatPerDay = deviceViewModel.Daily * exchangeRate;
                    if (fiatPerDay > 0.00)
                        item.SubItems["Daily"].Text = String.Format("{0}{1}", exchangeInformation.TargetSymbol, fiatPerDay.ToFriendlyString(true));
                }
                else
                {
                    if (deviceViewModel.Daily > 0.00)
                    {
                        string coinSymbol = KnownCoins.BitcoinSymbol;
                        if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                            coinSymbol = info.Symbol;
                        item.SubItems["Daily"].Text = String.Format("{0} {1}", deviceViewModel.Daily.ToFriendlyString(), coinSymbol);
                    }
                }
            }
        }

        private void RefreshDetailsToggleButton()
        {
            if (briefMode)
                detailsToggleButton.Text = "▾ More details";
            else
                detailsToggleButton.Text = "▴ Fewer details";
        }

        private void RefreshIncomeSummary()
        {
            if (sellPrices == null)
            {
                //no internet or error parsing API
                incomeSummaryLabel.Text = "";
                return;
            }

            if (coinApiInformation == null)
            {
                //no internet or error parsing API
                incomeSummaryLabel.Text = "";
                return;
            }

            //check .Mining to allow perks for Remoting when local PC is not mining
            if ((!miningEngine.Donating && miningEngine.Mining)
                || !perksConfiguration.ShowIncomeRates)
            {
                incomeSummaryLabel.Text = "";
                return;
            }

            string summary = String.Empty;

            Dictionary<string, double> incomeForCoins = GetIncomeForCoins();

            if (incomeForCoins.Count == 0)
                incomeSummaryLabel.Text = "";
            else
            {
                const string addition = " + ";
                double usdTotal = 0.00;
                string usdSymbol = "$";
                foreach (string coinSymbol in incomeForCoins.Keys)
                {
                    double coinIncome = incomeForCoins[coinSymbol];
                    CoinInformation coinInfo = coinApiInformation
                        .ToList() //get a copy - populated async & collection may be modified
                        .SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
                    if (coinInfo != null)
                    {
                        ExchangeInformation exchangeInformation = sellPrices.Single(er => er.TargetCurrency.Equals(GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
                        usdSymbol = exchangeInformation.TargetSymbol;
                        double btcExchangeRate = exchangeInformation.ExchangeRate;
                        double coinUsd = btcExchangeRate * coinInfo.Price;

                        double coinDailyUsd = coinIncome * coinUsd;
                        usdTotal += coinDailyUsd;

                        if (coinIncome > 0)
                            summary = String.Format("{0}{1} {2}{3}", summary, coinIncome.ToFriendlyString(), coinInfo.Symbol, addition);
                    }
                }

                if (!String.IsNullOrEmpty(summary))
                {
                    summary = summary.Remove(summary.Length - addition.Length, addition.Length); //remove trailing " + "

                    if (perksConfiguration.ShowExchangeRates)
                        summary = String.Format("{0} = {1}{2} / day", summary, usdSymbol, usdTotal.ToFriendlyString(true));

                    incomeSummaryLabel.Text = summary;

                    incomeSummaryLabel.AutoSize = true;
                    incomeSummaryLabel.Padding = new Padding(0, 11, 17, 0);
                }
            }
        }

        private Dictionary<string, double> GetIncomeForCoins()
        {
            Dictionary<string, double> coinsIncome = new Dictionary<string, double>();

            MinerFormViewModel viewModelToView = GetViewModelToView();

            foreach (DeviceViewModel deviceViewModel in viewModelToView.Devices)
            {
                //check for Coin != null, device may not have a coin configured
                if (deviceViewModel.Coin != null)
                {
                    string coinSymbol = KnownCoins.BitcoinSymbol;
                    if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                        coinSymbol = deviceViewModel.Coin.Id;

                    double coinIncome = deviceViewModel.Daily;

                    if (coinsIncome.ContainsKey(coinSymbol))
                        coinsIncome[coinSymbol] = coinsIncome[coinSymbol] + coinIncome;
                    else
                        coinsIncome[coinSymbol] = coinIncome;
                }
            }

            return coinsIncome;
        }

        private static void ClearCoinStatsForGridListViewItem(ListViewItem item)
        {
            item.SubItems["Difficulty"].Tag = 0.0;
            item.SubItems["Difficulty"].Text = String.Empty;

            item.SubItems["Price"].Text = String.Empty;
            item.SubItems["Profitability"].Text = String.Empty;

            item.SubItems["Exchange"].Tag = 0.0;
            item.SubItems["Exchange"].Text = String.Empty;
        }

        private void ClearCoinStatsForDisabledCoins()
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (string.IsNullOrEmpty(item.SubItems["Coin"].Text))
                    ClearCoinStatsForGridListViewItem(item);
        }

        private void CheckCoinInPopupMenu(string currentCoin)
        {
            foreach (ToolStripItem item in coinPopupMenu.Items)
                ((ToolStripMenuItem)item).Checked = false;

            if (!String.IsNullOrEmpty(currentCoin))
            {
                foreach (ToolStripItem item in coinPopupMenu.Items)
                    if (item.Text.Equals(currentCoin))
                    {
                        ((ToolStripMenuItem)item).Checked = true;
                        break;
                    }
            }
        }

        private void RefreshCoinStatsLabel()
        {
            coinChooseSuffixLabel.Text = string.Format("at {0}", DateTime.Now.ToShortTimeString());
        }
        #endregion

        #region Settings dialogs
        private void ConfigureSettings()
        {
            if (this.selectedRemoteInstance == null)
            {
                ConfigureSettingsLocally();
            }
            else
            {
                ConfigureSettingsRemotely();
            }
        }

        private void ConfigureSettingsRemotely()
        {
            Data.Configuration.Application workingApplicationConfiguration = new Data.Configuration.Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();
            Paths workingPathConfiguration = new Paths();
            Perks workingPerksConfiguration = new Perks();

            GetRemoteApplicationConfiguration(this.selectedRemoteInstance);

            ObjectCopier.CopyObject(this.remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(this.remoteEngineConfig.ToModelObject(), workingEngineConfiguration);
            ObjectCopier.CopyObject(this.remotePathConfig, workingPathConfiguration);
            ObjectCopier.CopyObject(this.remotePerksConfig, workingPerksConfiguration);

            SettingsForm settingsForm = new SettingsForm(
                workingApplicationConfiguration, 
                workingEngineConfiguration.XgminerConfiguration, 
                workingPathConfiguration,
                workingPerksConfiguration);

            settingsForm.Text = String.Format("{0}: {1}", settingsForm.Text, this.selectedRemoteInstance.MachineName);
            DialogResult dialogResult = settingsForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), this.remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), this.remoteEngineConfig);
                ObjectCopier.CopyObject(workingPathConfiguration, this.remotePathConfig);
                ObjectCopier.CopyObject(workingPerksConfiguration, this.remotePerksConfig);

                SetConfigurationRemotely(this.selectedRemoteInstance, this.remoteApplicationConfig, this.remoteEngineConfig, this.remotePathConfig, null);
            }
        }

        private void ConfigureSettingsLocally()
        {
            bool oldNetworkDeviceDetection = applicationConfiguration.NetworkDeviceDetection;
            bool oldCoinWarzValue = applicationConfiguration.UseCoinWarzApi;
            bool oldWhatMineValue = applicationConfiguration.UseWhatMineApi;
            string oldCoinWarzKey = applicationConfiguration.CoinWarzApiKey;
            string oldWhatMineKey = applicationConfiguration.WhatMineApiKey;

            string oldConfigPath = pathConfiguration.SharedConfigPath;

            SettingsForm settingsForm = new SettingsForm(
                applicationConfiguration, 
                engineConfiguration.XgminerConfiguration, 
                pathConfiguration,
                perksConfiguration);

            DialogResult dialogResult = settingsForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.Application.DoEvents();

                pathConfiguration.SavePathConfiguration();

                //save settings as the "shared" config path may have changed
                //these are settings not considered machine/device-specific
                //e.g. no device settings, no miner settings
                string newConfigPath = pathConfiguration.SharedConfigPath;
                MigrateSettingsToNewFolder(oldConfigPath, newConfigPath);

                applicationConfiguration.SaveApplicationConfiguration(newConfigPath);
                perksConfiguration.SavePerksConfiguration(newConfigPath);
                engineConfiguration.SaveCoinConfigurations(newConfigPath);
                engineConfiguration.SaveStrategyConfiguration(newConfigPath);
                engineConfiguration.SaveMinerConfiguration();
                MiningEngine.SaveAlgorithmConfigurations();
                SaveKnownCoinsToFile();

                //don't refresh coin stats excessively
                if ((oldCoinWarzValue != applicationConfiguration.UseCoinWarzApi) ||
                    !oldCoinWarzKey.Equals(applicationConfiguration.CoinWarzApiKey) || 
                    (oldWhatMineValue != applicationConfiguration.UseWhatMineApi) ||
                    !oldWhatMineKey.Equals(applicationConfiguration.WhatMineApiKey))
                {
                    SetupCoinApi(); //pickup API key changes
                    RefreshCoinStatsAsync();
                }

                //if we are not detecting Network Devices, start the async checks
                if (applicationConfiguration.NetworkDeviceDetection &&
                    (!oldNetworkDeviceDetection))
                {
                    CheckNetworkDevicesAsync();
                    FindNetworkDevicesAsync();
                }

                SubmitMobileMinerPools();

                RefreshViewForConfigurationChanges();
                RefreshListViewFromViewModel();
            }
            else
            {
                engineConfiguration.LoadMinerConfiguration();
                pathConfiguration.LoadPathConfiguration();
                applicationConfiguration.LoadApplicationConfiguration(pathConfiguration.SharedConfigPath);
            }
        }

        private void ConfigureUnconfiguredDevices()
        {
            foreach (Engine.Data.Configuration.Device deviceConfiguration in engineConfiguration.DeviceConfigurations)
            {
                bool configured = !String.IsNullOrEmpty(deviceConfiguration.CoinSymbol);
                bool misConfigured = configured &&
                    !engineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));

                if (!configured || misConfigured)
                {
                    Engine.Data.Configuration.Coin coinConfiguration = null;
                    if (deviceConfiguration.Kind == DeviceKind.GPU)
                        coinConfiguration = engineConfiguration.CoinConfigurations.FirstOrDefault(cc => cc.PoolGroup.Algorithm.Equals(AlgorithmNames.Scrypt, StringComparison.OrdinalIgnoreCase));
                    if (coinConfiguration == null)
                        coinConfiguration = engineConfiguration.CoinConfigurations.FirstOrDefault(cc => cc.PoolGroup.Algorithm.Equals(AlgorithmNames.SHA256, StringComparison.OrdinalIgnoreCase));

                    if (coinConfiguration != null)
                        deviceConfiguration.CoinSymbol = coinConfiguration.PoolGroup.Id;
                }
            }
        }

        private void FixMisconfiguredDevices()
        {
            foreach (Engine.Data.Configuration.Device deviceConfiguration in engineConfiguration.DeviceConfigurations)
            {
                bool misconfigured = !String.IsNullOrEmpty(deviceConfiguration.CoinSymbol) &&
                    !engineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));

                if (misconfigured)
                    deviceConfiguration.CoinSymbol = String.Empty;
            }
        }

        private void RefreshViewForConfigurationChanges()
        {
            System.Windows.Forms.Application.DoEvents();

            if (perksConfiguration.PerksEnabled && perksConfiguration.ShowExchangeRates)
                RefreshExchangeRates();

            RefreshListViewFromViewModel();
            RefreshIncomeSummary();
            AutoSizeListViewColumns();
            SetupRemoting();
            RefreshStrategiesLabel();
            UpdateMiningButtons();
            RemoveInvalidCoinValuesFromListView();
            RefreshCoinPopupMenu();
            SetupStatusBarLabelLayouts();

            SetupCoinApi();
            RefreshCoinApiLabel();
            SetupRestartTimer();
            CheckForUpdates();
            SetupCoinStatsTimer();
            SuggestCoinsToMine();

            SetupAccessibleMenu();
            SetGpuEnvironmentVariables();

            localViewModel.DynamicIntensity = engineConfiguration.XgminerConfiguration.DesktopMode;
            dynamicIntensityButton.Checked = localViewModel.DynamicIntensity;

            System.Windows.Forms.Application.DoEvents();
        }

        private void ConfigureCoins()
        {
            if (this.selectedRemoteInstance == null)
            {
                ConfigureCoinsLocally();
            }
            else
            {
                ConfigureCoinsRemotely();
            }
        }

        private void ConfigureCoinsRemotely()
        {
            Data.Configuration.Application workingApplicationConfiguration = new Data.Configuration.Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();

            GetRemoteApplicationConfiguration(this.selectedRemoteInstance);

            ObjectCopier.CopyObject(this.remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(this.remoteEngineConfig.ToModelObject(), workingEngineConfiguration);

            PoolsForm coinsForm = new PoolsForm(workingEngineConfiguration.CoinConfigurations, knownCoins, workingApplicationConfiguration, perksConfiguration);
            coinsForm.Text = String.Format("{0}: {1}", coinsForm.Text, this.selectedRemoteInstance.MachineName);
            DialogResult dialogResult = coinsForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), this.remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), this.remoteEngineConfig);
                SetConfigurationRemotely(this.selectedRemoteInstance, this.remoteApplicationConfig, this.remoteEngineConfig, null, null);

                if (applicationConfiguration.SaveCoinsToAllMachines && perksConfiguration.PerksEnabled && perksConfiguration.EnableRemoting)
                    SetCoinConfigurationOnAllRigs(this.remoteEngineConfig.CoinConfigurations);
            }
        }

        private void ConfigureCoinsLocally()
        {
            PoolsForm coinsForm = new PoolsForm(engineConfiguration.CoinConfigurations, knownCoins, applicationConfiguration, perksConfiguration);
            DialogResult dialogResult = coinsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                FixMisconfiguredDevices();
                ConfigureUnconfiguredDevices();

                engineConfiguration.SaveCoinConfigurations();
                engineConfiguration.SaveDeviceConfigurations();
                applicationConfiguration.SaveApplicationConfiguration();

                //may be able to auto-assign more devices now that coins are setup
                AddMissingDeviceConfigurations();
                
                ApplyModelsToViewModel();
                RefreshViewForConfigurationChanges();

                SubmitMobileMinerPools();

                if (applicationConfiguration.SaveCoinsToAllMachines && perksConfiguration.PerksEnabled && perksConfiguration.EnableRemoting)
                    SetCoinConfigurationOnAllRigs(this.engineConfiguration.CoinConfigurations.ToArray());
            }
            else
            {
                engineConfiguration.LoadCoinConfigurations(pathConfiguration.SharedConfigPath);
                applicationConfiguration.LoadApplicationConfiguration(pathConfiguration.SharedConfigPath);
            }
        }

        private void ConfigureStrategiesLocally()
        {
            StrategiesForm strategiesForm = new StrategiesForm(engineConfiguration.StrategyConfiguration, knownCoins,
                applicationConfiguration);
            DialogResult dialogResult = strategiesForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                engineConfiguration.SaveStrategyConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();

                RefreshViewForConfigurationChanges();
            }
            else
            {
                engineConfiguration.LoadStrategyConfiguration(pathConfiguration.SharedConfigPath);
                applicationConfiguration.LoadApplicationConfiguration(pathConfiguration.SharedConfigPath);
            }
        }

        private void ConfigureStrategiesRemotely()
        {
            Data.Configuration.Application workingApplicationConfiguration = new Data.Configuration.Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();

            GetRemoteApplicationConfiguration(this.selectedRemoteInstance);

            ObjectCopier.CopyObject(this.remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(this.remoteEngineConfig.ToModelObject(), workingEngineConfiguration);

            StrategiesForm strategiesForm = new StrategiesForm(workingEngineConfiguration.StrategyConfiguration, knownCoins,
                workingApplicationConfiguration);
            strategiesForm.Text = String.Format("{0}: {1}", strategiesForm.Text, this.selectedRemoteInstance.MachineName);
            DialogResult dialogResult = strategiesForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), this.remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), this.remoteEngineConfig);

                SetConfigurationRemotely(this.selectedRemoteInstance, this.remoteApplicationConfig, this.remoteEngineConfig, null, null);
            }
        }

        private void ConfigureStrategies()
        {
            if (this.selectedRemoteInstance == null)
            {
                ConfigureStrategiesLocally();
            }
            else
            {
                ConfigureStrategiesRemotely();
            }
        }

        private void ConfigurePerks()
        {
            if (this.selectedRemoteInstance == null)
            {
                ConfigurePerksLocally();
            }
            else
            {
                ConfigurePerksRemotely();
            }
        }

        private void ConfigurePerksRemotely()
        {
            Perks workingPerksConfiguration = new Perks();

            GetRemoteApplicationConfiguration(this.selectedRemoteInstance);

            ObjectCopier.CopyObject(this.remotePerksConfig, workingPerksConfiguration);

            PerksForm perksForm = new PerksForm(workingPerksConfiguration);
            perksForm.Text = String.Format("{0}: {1}", perksForm.Text, this.selectedRemoteInstance.MachineName);
            DialogResult dialogResult = perksForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ObjectCopier.CopyObject(workingPerksConfiguration, this.remotePerksConfig);
                SetConfigurationRemotely(this.selectedRemoteInstance, null, null, null, this.remotePerksConfig);
            }
        }

        private void ConfigurePerksLocally()
        {
            PerksForm perksForm = new PerksForm(perksConfiguration);
            DialogResult dialogResult = perksForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                bool miningWithMultipleProxies = miningEngine.Mining && (devices.Count(d => d.Kind == DeviceKind.PXY) > 1);
                if (!perksConfiguration.PerksEnabled && miningWithMultipleProxies)
                {
                    throw new Exception(MiningEngine.AdvancedProxiesRequirePerksMessage);
                }

                perksConfiguration.SavePerksConfiguration();

                RefreshViewForConfigurationChanges();
            }
            else
                perksConfiguration.LoadPerksConfiguration(pathConfiguration.SharedConfigPath);
        }
        #endregion

        #region Settings logic
        private void LoadSettings()
        {
            engineConfiguration.LoadAllConfigurations(pathConfiguration.SharedConfigPath);

            SetupCoinApi();

            RefreshStrategiesLabel();
            RefreshStrategiesCountdown();

            //already done in ctor //applicationConfiguration.LoadApplicationConfiguration();

            perksConfiguration.LoadPerksConfiguration(pathConfiguration.SharedConfigPath);
            RefreshExchangeRates();

            SetListViewStyle(applicationConfiguration.ListViewStyle);

            //load brief mode first, then location
            SetBriefMode(applicationConfiguration.BriefUserInterface);

            //now location so we pick up the customizations
            if ((applicationConfiguration.AppPosition.Height > 0) &&
                (applicationConfiguration.AppPosition.Width > 9))
            {
                this.Location = new Point(applicationConfiguration.AppPosition.Left, applicationConfiguration.AppPosition.Top);
                this.Size = new Size(applicationConfiguration.AppPosition.Width, applicationConfiguration.AppPosition.Height);
            }

            if (applicationConfiguration.Maximized)
                this.WindowState = FormWindowState.Maximized;

            if (applicationConfiguration.LogAreaVisible)
            {
                ShowApiMonitor();
                if ((applicationConfiguration.LogAreaTabIndex >= 0) &&
                    (applicationConfiguration.LogAreaTabIndex < advancedTabControl.TabCount))
                    advancedTabControl.SelectedIndex = applicationConfiguration.LogAreaTabIndex;
                if ((applicationConfiguration.LogAreaDistance > 0) &&
                    //can't set splitter distance with the app minimized :( InvalidOperationException
                    !applicationConfiguration.StartupMinimized)
                    advancedAreaContainer.SplitterDistance = applicationConfiguration.LogAreaDistance;
            }
            else
                HideAdvancedPanel();

            //can't set details container width until it is shown
            if (applicationConfiguration.InstancesAreaWidth > 0)
                instancesContainer.SplitterDistance = applicationConfiguration.InstancesAreaWidth;
            
            SetupCoinStatsTimer();

            ClearPoolsFlaggedDown();

            ApplyModelsToViewModel();
            localViewModel.DynamicIntensity = engineConfiguration.XgminerConfiguration.DesktopMode;
            dynamicIntensityButton.Checked = localViewModel.DynamicIntensity;

            metadataConfiguration.LoadDeviceMetadataConfiguration();

            //allow resize/maximize/etc to render
            System.Windows.Forms.Application.DoEvents();

            this.settingsLoaded = true;
        }

        private void SetupNetworkDeviceDetection()
        {
            //network devices
            this.networkDevicesConfiguration.LoadNetworkDevicesConfiguration();
            
            if (applicationConfiguration.NetworkDeviceDetection)
            {
                CheckNetworkDevicesAsync();
                FindNetworkDevicesAsync();
            }
        }

        private void FindNetworkDevices()
        {
            List<string> localIpRanges = Utility.Net.LocalNetwork.GetLocalIPAddressRanges();
            if (localIpRanges.Count == 0)
                return; //no network connection

            const int startingPort = 4028;
            const int endingPort = 4030;

            foreach (string localIpRange in localIpRanges)
            {
                List<IPEndPoint> miners = MinerFinder.Find(localIpRange, startingPort, endingPort);

                //remove own miners
                miners.RemoveAll(m => Utility.Net.LocalNetwork.GetLocalIPAddresses().Contains(m.Address.ToString()));

                List<NetworkDevices.NetworkDevice> newDevices = miners.ToNetworkDevices();

                //merge in miners, don't remove miners here
                //let CheckNetworkDevices() remove miners since it does not depend on port scanning
                //some users are manually entering devices in the XML
                List<NetworkDevices.NetworkDevice> existingDevices = networkDevicesConfiguration.Devices;
                newDevices = newDevices
                    .Where(d1 => !existingDevices.Any(d2 => d2.IPAddress.Equals(d1.IPAddress) && (d2.Port == d1.Port)))
                    .ToList();
                networkDevicesConfiguration.Devices.AddRange(newDevices);
            }
            
            networkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        private void CheckNetworkDevices()
        {
            List<IPEndPoint> endpoints = networkDevicesConfiguration.Devices.ToIPEndPoints();

            //remove own miners
            endpoints.RemoveAll(m => Utility.Net.LocalNetwork.GetLocalIPAddresses().Contains(m.Address.ToString()));

            endpoints = MinerFinder.Check(endpoints);

            List<NetworkDevices.NetworkDevice> existingDevices = networkDevicesConfiguration.Devices;
            List<NetworkDevices.NetworkDevice> prunedDevices = endpoints.ToNetworkDevices();
            //add in Sticky devices not already in the pruned devices
            //Sticky devices allow users to mark Network Devices that should never be removed
            prunedDevices.AddRange(
                existingDevices
                    .Where(d1 => d1.Sticky && !prunedDevices.Any(d2 => d2.IPAddress.Equals(d1.IPAddress) && (d2.Port == d1.Port)))
            );

            //filter the devices by prunedDevices - do not assign directly as we need to
            //preserve properties on the existing configurations - e.g. Sticky
            networkDevicesConfiguration.Devices =
                networkDevicesConfiguration.Devices
                .Where(ed => prunedDevices.Any(pd => pd.IPAddress.Equals(ed.IPAddress) && (pd.Port == ed.Port)))
                .ToList();

            networkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        private void CheckNetworkDevicesAsync()
        {
            Action asyncAction = CheckNetworkDevices;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (tearingDown) return;

                    BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        HandleNetworkDeviceDiscovery();
                    }));

                }, null);
        }

        private void HandleNetworkDeviceDiscovery()
        {
            ApplyModelsToViewModel();

            deviceListView.BeginUpdate();
            try
            {
                //after the above call, no devices in the ViewModel have stats
                //refresh them
                if (localViewModel.Devices.Count(d => d.Kind != DeviceKind.NET) > 0)
                    RefreshDeviceStats();

                if (localViewModel.Devices.Count(d => d.Kind == DeviceKind.NET) > 0)
                    RefreshNetworkDeviceStatsAsync();
            }
            finally
            {
                deviceListView.EndUpdate();
            }

            AutoSizeListViewColumns();
        }

        private void FindNetworkDevicesAsync()
        {
            Action asyncAction = FindNetworkDevices;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (tearingDown) return;

                    BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        HandleNetworkDeviceDiscovery();
                    }));

                }, null);
        }

        private void SaveSettings()
        {
            applicationConfiguration.LogAreaTabIndex = advancedTabControl.SelectedIndex;
            SavePosition();

            applicationConfiguration.DetailsAreaWidth = detailsAreaContainer.Width - detailsAreaContainer.SplitterDistance;
            applicationConfiguration.InstancesAreaWidth = instancesContainer.SplitterDistance;

            this.applicationConfiguration.SaveApplicationConfiguration();
        }

        private void SavePosition()
        {
            if (this.WindowState == FormWindowState.Normal)
                this.applicationConfiguration.AppPosition = new Rectangle(this.Location, this.Size);
        }

        //try to match up devices without configurations with configurations without devices
        //could happen if, for instance, a COM port changes for a device
        private void FixOrphanedDeviceConfigurations()
        {
            foreach (Xgminer.Data.Device device in devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));

                //if there is no configuration specifically for the device
                if (existingConfiguration == null)
                {
                    //find a configuration that uses the same driver and that, itself, has no specifically matching device
                    Engine.Data.Configuration.Device orphanedConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                        c => c.Driver.Equals(device.Driver, StringComparison.OrdinalIgnoreCase) &&
                                !devices.Exists(d => d.Equals(c)));

                    if (orphanedConfiguration != null)
                        orphanedConfiguration.Assign(device);
                }
            }
        }

        //each device needs to have a DeviceConfiguration
        //this will remove any access ones after populating devices
        //for instance if the user starts up the app with missing devices
        private void RemoveExcessDeviceConfigurations()
        {
            engineConfiguration.DeviceConfigurations.RemoveAll(c => !devices.Exists(d => d.Equals(c)));
        }

        //each device needs to have a DeviceConfiguration
        //this will add any missing ones after populating devices
        //for instance if the user starts up the app with a new device
        private void AddMissingDeviceConfigurations()
        {
            bool hasBtcConfigured = engineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.PoolGroup.Id.Equals(KnownCoins.BitcoinSymbol, StringComparison.OrdinalIgnoreCase));
            bool hasLtcConfigured = engineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.PoolGroup.Id.Equals(KnownCoins.LitecoinSymbol, StringComparison.OrdinalIgnoreCase));

            foreach (Xgminer.Data.Device device in devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));
                if (existingConfiguration == null)
                {
                    Engine.Data.Configuration.Device newConfiguration = new Engine.Data.Configuration.Device();

                    newConfiguration.Assign(device);

                    if (device.SupportsAlgorithm(AlgorithmNames.Scrypt) && hasLtcConfigured)
                        newConfiguration.CoinSymbol = KnownCoins.LitecoinSymbol;
                    else if (device.SupportsAlgorithm(AlgorithmNames.SHA256) && hasBtcConfigured)
                        newConfiguration.CoinSymbol = KnownCoins.BitcoinSymbol;

                    newConfiguration.Enabled = true;
                    engineConfiguration.DeviceConfigurations.Add(newConfiguration);
                }
            }
        }

        private bool DeviceConfigurationValid(Engine.Data.Configuration.Device deviceConfiguration)
        {
            bool result = deviceConfiguration.Enabled && !string.IsNullOrEmpty(deviceConfiguration.CoinSymbol);
            if (result)
            {
                Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                result = coinConfiguration == null ? false : coinConfiguration.Pools.Where(p => !String.IsNullOrEmpty(p.Host) && !String.IsNullOrEmpty(p.Username)).Count() > 0;
            }
            return result;
        }

        private bool MiningConfigurationValid()
        {
            bool miningConfigurationValid = engineConfiguration.DeviceConfigurations.Count(
                c => DeviceConfigurationValid(c)) > 0;
            if (!miningConfigurationValid)
            {
                miningConfigurationValid = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                    (engineConfiguration.CoinConfigurations.Count(c => c.Enabled) > 0) &&
                    (engineConfiguration.DeviceConfigurations.Count(c => c.Enabled) > 0);
            }
            return miningConfigurationValid;
        }

        private Engine.Data.Configuration.Coin CoinConfigurationForDevice(Xgminer.Data.Device device)
        {
            //get the actual device configuration, text in the ListViewItem may be unsaved
            Engine.Data.Configuration.Device deviceConfiguration = null;
            if (miningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningDeviceConfigurations
                (miningDeviceConfigurations != null))
                deviceConfiguration = miningDeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));
            else
                deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));

            if (deviceConfiguration == null)
                return null;

            string itemCoinSymbol = deviceConfiguration.CoinSymbol;

            List<Engine.Data.Configuration.Coin> configurations;
            if (miningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningCoinConfigurations
                (miningCoinConfigurations != null))
                configurations = this.miningCoinConfigurations;
            else
                configurations = engineConfiguration.CoinConfigurations;

            Engine.Data.Configuration.Coin coinConfiguration = configurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(itemCoinSymbol, StringComparison.OrdinalIgnoreCase));
            return coinConfiguration;
        }

        private void SaveCoinStatsToKnownCoins()
        {
            //coinApiInformation may be Null if Coin APIs are offline
            if (coinApiInformation == null)
                coinApiInformation = new List<CoinInformation>();

            foreach (CoinInformation item in coinApiInformation)
            {
                //find existing known coin or create a knew one
                PoolGroup knownCoin = knownCoins.SingleOrDefault(c => c.Id.Equals(item.Symbol));
                if (knownCoin == null)
                {
                    knownCoin = new PoolGroup();
                    this.knownCoins.Add(knownCoin);
                }

                knownCoin.Id = item.Symbol;
                knownCoin.Name = item.Name;
                knownCoin.Algorithm = item.Algorithm;
            }
            SaveKnownCoinsToFile();
        }

        private static string KnownDevicesFileName()
        {
            string filePath = ApplicationPaths.AppDataPath();
            return Path.Combine(filePath, "KnownDevicesCache.xml");
        }

        private void LoadKnownDevicesFromFile()
        {
            string knownDevicesFileName = KnownDevicesFileName();
            if (File.Exists(knownDevicesFileName))
            {
                devices = ConfigurationReaderWriter.ReadConfiguration<List<Xgminer.Data.Device>>(knownDevicesFileName);
                ApplyModelsToViewModel();
                RefreshListViewFromViewModel();
            }
        }

        private void SaveKnownDevicesToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(devices, KnownDevicesFileName());
        }

        private string KnownCoinsFileName()
        {
            string filePath;
            if (String.IsNullOrEmpty(pathConfiguration.SharedConfigPath))
                filePath = ApplicationPaths.AppDataPath();
            else
                filePath = pathConfiguration.SharedConfigPath;
            return Path.Combine(filePath, "KnownCoinsCache.xml");
        }

        private void LoadKnownCoinsFromFile()
        {
            string knownCoinsFileName = KnownCoinsFileName();
            if (File.Exists(knownCoinsFileName))
            {
                knownCoins = ConfigurationReaderWriter.ReadConfiguration<List<PoolGroup>>(knownCoinsFileName);
                RemoveBunkCoins(knownCoins);
            }
        }

        private static void RemoveBunkCoins(List<PoolGroup> knownCoins)
        {
            //CoinChoose.com served up ButterFlyCoin as BOC, and then later as BFC
            PoolGroup badCoin = knownCoins.SingleOrDefault(c => c.Id.Equals("BOC", StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
                knownCoins.Remove(badCoin);
        }

        private void SaveKnownCoinsToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(knownCoins, KnownCoinsFileName());
        }

        private void SaveViewModelValuesToConfiguration()
        {
            engineConfiguration.DeviceConfigurations.Clear();

            foreach (Xgminer.Data.Device device in devices)
            {
                //don't assume 1-to-1 of Devices and ViewModel.Devices
                //Devices doesn't include Network Devices
                DeviceViewModel viewModel = localViewModel.Devices.Single(vm => vm.Equals(device));

                //pull this from coin configurations, not known coins, may not be in CoinChoose
                PoolGroup coin = viewModel.Coin;
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device();
                deviceConfiguration.Assign(viewModel);
                deviceConfiguration.Enabled = viewModel.Enabled;
                deviceConfiguration.CoinSymbol = coin == null ? string.Empty : coin.Id;
                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }
        }
        #endregion

        #region UI event handlers
        private void CoinMenuItemClick(object sender, EventArgs e)
        {
            ToolStripItem menuItem = (ToolStripItem)sender;
            string coinSymbol = menuItem.Text;
            List<DeviceDescriptor> devices = new List<DeviceDescriptor>();
            MinerFormViewModel viewModelToView = GetViewModelToView();

            foreach (ListViewItem selectedItem in deviceListView.SelectedItems)
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel)selectedItem.Tag;
                devices.Add(deviceViewModel);
            }

            SetDevicesToCoin(devices, coinSymbol);
        }

        private void SetDevicesToCoinLocally(IEnumerable<DeviceDescriptor> devices, string coinName)
        {
            foreach (DeviceDescriptor device in devices)
            {
                DeviceViewModel deviceViewModel = localViewModel.Devices.SingleOrDefault(dvm => dvm.Equals(device));
                if ((deviceViewModel != null) && (deviceViewModel.Kind != DeviceKind.NET))
                    deviceViewModel.Coin = engineConfiguration.CoinConfigurations.Single(cc => cc.PoolGroup.Name.Equals(coinName)).PoolGroup;
            }

            RefreshListViewFromViewModel();

            AutoSizeListViewColumns();

            SetHasChangesLocally(true);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelChanges();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            HandleStartButtonClicked();
        }

        private void HandleStartButtonClicked()
        {
            if ((this.selectedRemoteInstance == null) && applicationConfiguration.AutoSetDesktopMode)
                ToggleDynamicIntensityLocally(true);

            StartMining();
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            CancelMiningOnStartup();
        }

        private void detectDevicesButton_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }

        private void coinChooseLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(GetEffectiveApiContext().GetInfoUrl());
        }

        private void apiMonitorButton_Click(object sender, EventArgs e)
        {
            ShowApiMonitor();
        }

        private void advancedToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            MinerFormViewModel viewModel = GetViewModelToView();

            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchToolStripMenuItem.Enabled = viewModel.ConfiguredCoins.Count() > 0;
            //
            dynamicIntensityToolStripMenuItem.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;
            dynamicIntensityToolStripMenuItem.Checked = viewModel.DynamicIntensity;
            dynamicIntensityMenuSeperator.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        private void scanHardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }

        private void historyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void processLogToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowProcessLog();
        }

        private void aPIMonitorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowApiMonitor();
        }

        private void dynamicIntensityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleDynamicIntensity(dynamicIntensityToolStripMenuItem.Checked);
        }

        private void largeIconsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.LargeIcon);
        }

        private void smallIconsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.SmallIcon);
        }

        private void listToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.List);
        }

        private void detailsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.Details);
        }

        private void tilesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.Tile);
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConfigureSettings();
        }

        private void coinsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConfigureCoins();
        }

        private void strategiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void perksToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartMining();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartMining();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelChanges();
        }

        private void quickSwitchToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshQuickSwitchMenu(quickSwitchToolStripMenuItem);
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogLaunchArgs args = (LogLaunchArgs)logLaunchArgsBindingSource.Current;

            string arguments = args.Arguments;
            arguments = arguments.Replace("-T -q", String.Empty).Trim();

            ProcessStartInfo startInfo = new ProcessStartInfo(args.ExecutablePath, arguments);
            startInfo.WorkingDirectory = Path.GetDirectoryName(args.ExecutablePath);
            Process.Start(startInfo);
        }

        private void processLogGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Ignore if a column or row header is clicked
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    // Here you can do whatever you want with the cell
                    this.processLogGridView.CurrentCell = clickedCell;  // Select the clicked cell, for instance

                    // Get mouse position relative to the grid
                    var relativeMousePosition = processLogGridView.PointToClient(Cursor.Position);

                    // Show the context menu
                    this.processLogMenu.Show(processLogGridView, relativeMousePosition);
                }
            }
        }

        private void processLogGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Get mouse position relative to the grid
                var relativeMousePosition = processLogGridView.PointToClient(Cursor.Position);

                // Show the context menu
                this.openLogMenu.Show(processLogGridView, relativeMousePosition);
            }
        }

        private void detailsControl1_CloseClicked(object sender)
        {
            CloseDetailsArea();
        }

        private void deviceListView_DoubleClick(object sender, EventArgs e)
        {
            ShowDetailsArea();
        }

        private void openLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = String.Empty;
            if (advancedTabControl.SelectedTab == apiMonitorPage)
                fileName = "ApiLog.json";
            else if (advancedTabControl.SelectedTab == processLogPage)
                fileName = "ProcessLog.json";
            else if (advancedTabControl.SelectedTab == historyPage)
                fileName = "MiningLog.json";

            if (!String.IsNullOrEmpty(fileName))
            {
                string logDirectory = GetLogDirectory();
                string logFilePath = Path.Combine(logDirectory, fileName);
                Process.Start(logFilePath);
            }
        }

        private void deviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deviceListView.SelectedItems.Count > 0)
                RefreshDetailsArea();
        }

        private void listViewStyleButton_ButtonClick(object sender, EventArgs e)
        {
            switch (deviceListView.View)
            {
                case View.LargeIcon:
                    SetListViewStyle(View.SmallIcon);
                    break;
                case View.Details:
                    SetListViewStyle(View.Tile);
                    break;
                case View.SmallIcon:
                    SetListViewStyle(View.List);
                    break;
                case View.List:
                    SetListViewStyle(View.Details);
                    break;
                case View.Tile:
                    SetListViewStyle(View.LargeIcon);
                    break;
            }
        }

        private void historyGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 0) || (e.ColumnIndex == 1))
            {
                e.Value = ((DateTime)e.Value).ToReallyShortDateTimeString();
                e.FormattingApplied = true;
            }
        }

        private void processLogGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (e.Value != null)
                {
                    e.Value = ((DateTime)e.Value).ToReallyShortDateTimeString();
                    e.FormattingApplied = true;
                }
            }
            else if (e.ColumnIndex == 2)
            {
                if (e.Value != null)
                {
                    e.Value = MakeRelative((String)e.Value, AppDomain.CurrentDomain.BaseDirectory);
                    e.FormattingApplied = true;
                }
            }
        }

        private static string MakeRelative(string absolutePath, string referencePath)
        {
            var fileUri = new Uri(absolutePath);
            var referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar);
        }

        private void apiLogGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Value = ((DateTime)e.Value).ToReallyShortDateTimeString();
                e.FormattingApplied = true;
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            AutoSizeListViewColumns();
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            RestartMining();
        }

        private void perksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void perksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void deviceListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == incomeColumnHeader.Index)
            {
                using (new HourGlass())
                {
                    perksConfiguration.ShowIncomeInUsd = !perksConfiguration.ShowIncomeInUsd;
                    RefreshDeviceStats();
                    perksConfiguration.SavePerksConfiguration();
                    AutoSizeListViewColumns();
                }
            }
            else if (e.Column == utilityColumnHeader.Index)
            {
                applicationConfiguration.ShowWorkUtility = !applicationConfiguration.ShowWorkUtility;
                applicationConfiguration.SaveApplicationConfiguration();

                UpdateUtilityColumnHeader();

                RefreshDeviceStats();
                AutoSizeListViewColumns();
                RefreshDetailsAreaIfVisible();
            }
        }

        private void columnHeaderMenu_Opening(object sender, CancelEventArgs e)
        {
            if (deviceListView.View != View.Details)
            {
                e.Cancel = true;
                return;
            }

            Rectangle headerRect = new Rectangle(0, 0, deviceListView.Width, 20);
            if (!headerRect.Contains(deviceListView.PointToClient(Control.MousePosition)))
                e.Cancel = true;

            if (!e.Cancel)
            {
                if (columnHeaderMenu.Items.Count <= 1) //1 dummy item so it opens
                    SetupColumnHeaderMenu();
            }
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.LargeIcon);
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.SmallIcon);
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.List);
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.Details);
        }

        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetListViewStyle(View.Tile);
        }

        private void deviceListView_MouseUp(object sender, MouseEventArgs e)
        {
            //display the devices context menu when no item is selected
            if (e.Button == MouseButtons.Right)
                if ((deviceListView.FocusedItem == null) || !deviceListView.FocusedItem.Bounds.Contains(e.Location))
                {
                    deviceListContextMenu.Show(Cursor.Position);
                }
        }

        private void detectDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void processLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowProcessLog();
        }

        private void aPIMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowApiMonitor();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureSettings();
        }

        private void coinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureCoins();
        }

        private void strategiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void deviceListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all

            quickSwitchPopupItem.Enabled = GetViewModelToView().ConfiguredCoins.Count() > 0;
        }

        private void quickSwitchPopupItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshQuickSwitchMenu(quickSwitchPopupItem);
        }

        private bool briefMode = false;
        private void detailsToggleButton_ButtonClick(object sender, EventArgs e)
        {
            SetBriefMode(!briefMode);
            RefreshDetailsToggleButton();
        }

        private void deviceListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            //don't allow 0-width (hidden) columns to be resized
            if (deviceListView.Columns[e.ColumnIndex].Width == 0)
            {
                e.Cancel = true;
                e.NewWidth = 0;
            }
        }

        private void deviceListView_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (deviceListView.FocusedItem.Bounds.Contains(e.Location) == true))
            {
                if (deviceListView.FocusedItem.Group.Name.Equals("networkListViewGroup"))
                {
                    UpdateNetworkDeviceMenu();

                    networkDeviceContextMenu.Show(Cursor.Position);
                }
                else
                {
                    string currentCoin = GetCurrentlySelectedCoinName();

                    CheckCoinInPopupMenu(currentCoin);

                    coinPopupMenu.Show(Cursor.Position);
                }
            }
        }

        private string GetCurrentlySelectedCoinName()
        {
            return deviceListView.FocusedItem.SubItems["Coin"].Text;
        }

        private void deviceListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (this.updatingListView)
                return;

            bool enabled = e.Item.Checked;
            List<DeviceDescriptor> descriptors = new List<DeviceDescriptor>();
            MinerFormViewModel viewModel = GetViewModelToView();
            DeviceViewModel device = viewModel.Devices[e.Item.Index];
            DeviceDescriptor descriptor = new DeviceDescriptor();
            ObjectCopier.CopyObject(device, descriptor);
            descriptors.Add(descriptor);

            ToggleDevices(descriptors, enabled);
        }

        private void ToggleDevicesLocally(IEnumerable<DeviceDescriptor> descriptors, bool enabled)
        {
            foreach (DeviceDescriptor descriptor in descriptors)
            {
                DeviceViewModel viewModel = localViewModel.Devices.SingleOrDefault(dvm => dvm.Equals(descriptor));
                if (viewModel != null)
                    viewModel.Enabled = enabled;
            }

            SetHasChangesLocally(true);

            RefreshListViewFromViewModel();
        }

        private void ToggleDevices(IEnumerable<DeviceDescriptor> devices, bool enabled)
        {
            if (this.selectedRemoteInstance == null)
            {
                ToggleDevicesLocally(devices, enabled);
            }
            else
            {
                ToggleDevicesRemotely(this.selectedRemoteInstance, devices, enabled);
            }
        }

        private void dynamicIntensityButton_Click(object sender, EventArgs e)
        {
            ToggleDynamicIntensity(dynamicIntensityButton.Checked);
        }
        
        private void advancedAreaContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (settingsLoaded)
                applicationConfiguration.LogAreaDistance = e.SplitY;
        }

        private void settingsButton_ButtonClick(object sender, EventArgs e)
        {
            ConfigureSettings();
        }

        private void coinsButton_Click_1(object sender, EventArgs e)
        {
            ConfigureCoins();
        }

        private void strategiesButton_Click_1(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void advancedMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchItem.Enabled = engineConfiguration.CoinConfigurations.Where(c => c.Enabled).Count() > 0;
            //
            dynamicIntensityButton.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;

            dynamicIntensityButton.Checked = GetViewModelToView().DynamicIntensity;

            dynamicIntensitySeparator.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;
        }

        private void notificationsControl1_NotificationsChanged(object sender)
        {
            notificationsControl.Visible = notificationsControl.NotificationCount() > 0;
            if (notificationsControl.Visible)
                notificationsControl.BringToFront();
        }

        private void processLogButton_Click(object sender, EventArgs e)
        {
            ShowProcessLog();
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void historyGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //do unbound data in RowsAdded or it won't show until the DataGridView has been on-screen
            for (int i = 0; i < e.RowCount; i++)
            {
                int index = e.RowIndex + i;

                LogProcessCloseArgs ea = this.logCloseEntries[index];

                string devicesString = "??";
                //convert device descriptors to homan readable string
                //check for NULL because the history JSON is reloaded on startup and older
                //versions didn't have this property serialized
                if (ea.DeviceDescriptors != null)
                    devicesString = GetFormattedDevicesString(ea.DeviceDescriptors);

                historyGridView.Rows[index].Cells[devicesColumn.Index].Value = devicesString;

                TimeSpan timeSpan = ea.EndDate - ea.StartDate;
                historyGridView.Rows[index].Cells[durationColumn.Index].Value = String.Format("{0:0.##} minutes", timeSpan.TotalMinutes);
            }
        }

        private static string GetFormattedDevicesString(List<DeviceDescriptor> deviceDescriptors)
        {
            return String.Join(" ", deviceDescriptors.Select(d => d.ToString()).ToArray());
        }

        private void quickSwitchItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshQuickSwitchMenu(quickSwitchItem);
        }

        private void notificationsControl1_NotificationAdded(string text, MobileMiner.Data.NotificationKind kind)
        {
            LogNotificationToFile(text);
            QueueMobileMinerNotification(text, kind);
        }

        private void closeApiButton_Click(object sender, EventArgs e)
        {
            HideAdvancedPanel();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (applicationSetup)
            {
                //handling for minimizing to notifcation area
                if (applicationConfiguration.MinimizeToNotificationArea && (this.WindowState == FormWindowState.Minimized))
                {
                    notifyIcon1.Visible = true;
                    this.Hide();
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    notifyIcon1.Visible = false;
                }

                //handling for saving Maximized state
                if (this.WindowState == FormWindowState.Maximized)
                    applicationConfiguration.Maximized = true;

                if (this.WindowState == FormWindowState.Normal) //don't set to false for minimizing
                    applicationConfiguration.Maximized = false;

                SavePosition();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void showAppMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void quitAppMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void startMenuItem_Click(object sender, EventArgs e)
        {
            HandleStartButtonClicked();
        }

        private void stopMenuItem_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void ColumnHeaderMenuClick(object sender, EventArgs e)
        {
            using (new HourGlass())
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                menuItem.Checked = !menuItem.Checked;
                if (menuItem.Checked)
                    applicationConfiguration.HiddenColumns.Remove(menuItem.Text);
                else
                    applicationConfiguration.HiddenColumns.Add(menuItem.Text);
                applicationConfiguration.SaveApplicationConfiguration();
                AutoSizeListViewColumns();
            }
        }

        private void HandleQuickSwitchClick(object sender, EventArgs e)
        {
            string coinSymbol = (string)((ToolStripMenuItem)sender).Tag;

            bool allRigs = ShouldQuickSwitchAllRigs(coinSymbol);

            bool disableStrategies = ShouldDisableStrategies();

            SetAllDevicesToCoin(coinSymbol, disableStrategies);

            if (allRigs)
                SetAllDevicesToCoinOnAllRigs(coinSymbol, disableStrategies);
        }

        private bool ShouldDisableStrategies()
        {
            bool disableStrategies = false;
            if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Would you like to disable Auto-Mining Strategies after switching coins?",
                    "Quick Switch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    disableStrategies = true;
            }
            return disableStrategies;
        }

        private bool ShouldQuickSwitchAllRigs(string coinSymbol)
        {
            bool allRigs = false;
            if (remotingEnabled && (instancesControl.Instances.Count > 1))
            {
                DialogResult dialogResult = MessageBox.Show(
                    String.Format("Would you like to Quick Switch to {0} on all of your online rigs?", coinSymbol),
                    "MultiMiner Remoting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    allRigs = true;
            }
            return allRigs;
        }

        private void startStartupMiningButton_Click(object sender, EventArgs e)
        {
            HandleStartButtonClicked();
        }

        private void deviceListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.updatingListView)
                return;

            //disallow toggling check-state for Network Devices
            MinerFormViewModel viewModelToView = GetViewModelToView();
            if (viewModelToView.Devices[e.Index].Kind == DeviceKind.NET)
                e.NewValue = CheckState.Checked;
        }

        private void deviceListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            editingDeviceListView = true;
        }

        private void deviceListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
                //edit was canceled
                return;

            editingDeviceListView = false;

            ListViewItem editItem = deviceListView.Items[e.Item];
            DeviceViewModel deviceViewModel = (DeviceViewModel)editItem.Tag;

            RenameDevice(deviceViewModel, e.Label);
        }

        private void restartMiningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartNetworkDevice();
        }

        private void adminPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceViewModel networkDevice = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            Process.Start("http://" + networkDevice.Name);
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            notificationClickHandler();
        }

        private void advancedTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //best / only way to ensure that items like History show initially scrolled to
            //the bottom so new entries don't require user intervention to see them
            EnsureRecentLogDataVisibility();
        }

        private void EnsureRecentLogDataVisibility()
        {
            if (advancedTabControl.SelectedTab == historyPage)
                logProcessCloseArgsBindingSource.MoveLast();
            else if (advancedTabControl.SelectedTab == processLogPage)
                logLaunchArgsBindingSource.MoveLast();
            else if (advancedTabControl.SelectedTab == apiMonitorPage)
                apiLogEntryBindingSource.MoveLast();
        }

        private void settingsPlainButton_Click(object sender, EventArgs e)
        {
            ConfigureSettings();
        }

        private void poolsPlainButton_Click(object sender, EventArgs e)
        {
            ConfigureCoins();
        }

        private void strategiesPlainButton_Click(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void perksPlainButton_Click(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void helpToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki");
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }
        #endregion

        #region Timer setup
        private void SetupMiningOnStartup()
        {
            if (applicationConfiguration.StartMiningOnStartup)
            {
                //minimum 1s delay for mining on startup - 0 not allowed
                startupMiningCountdownSeconds = Math.Max(1, applicationConfiguration.StartupMiningDelay);
                RefreshCountdownLabel();
            }
        }

        private void SetupRestartTimer()
        {
            //if enabled, we want to restart it so this can be used when we start mining
            restartTimer.Enabled = false;
            restartTimer.Interval = applicationConfiguration.ScheduledRestartMiningInterval.ToMinutes() * 60 * 1000; //dynamic
            restartTimer.Enabled = applicationConfiguration.ScheduledRestartMining;
        }

        private void SetupCoinStatsTimer()
        {
            int coinStatsMinutes = 15;
            Data.Configuration.Application.TimerInterval timerInterval = applicationConfiguration.StrategyCheckInterval;

            coinStatsMinutes = timerInterval.ToMinutes();

            coinStatsTimer.Enabled = false;

            coinStatsCountdownMinutes = coinStatsMinutes;
            coinStatsTimer.Interval = coinStatsMinutes * 60 * 1000; //dynamic

            coinStatsTimer.Enabled = true;
        }

#if DEBUG
        private void debugOneSecondTimer_Tick(object sender, EventArgs e)
        {
            //updates in order to try to reproduce threading issues
            RefreshDetailsAreaIfVisible();
            RefreshListViewFromViewModel();
        }
#endif
        
        private void SetupCoalescedTimers()
        {
#if DEBUG
            timers.CreateTimer(Timers.OneSecondInterval, debugOneSecondTimer_Tick);
#endif
            timers.CreateTimer(Timers.OneHourInterval, oneHourTimer_Tick);
            timers.CreateTimer(Timers.OneMinuteInterval, oneMinuteTimer_Tick);
            timers.CreateTimer(Timers.ThirtySecondInterval, thirtySecondTimer_Tick);
            timers.CreateTimer(Timers.FiveMinuteInterval, fiveMinuteTimer_Tick);
            timers.CreateTimer(Timers.TenSecondInterval, tenSecondTimer_Tick);
            timers.CreateTimer(Timers.FifteenSecondInterval, fifteenSecondTimer_Tick);
            timers.CreateTimer(Timers.FifteenMinuteInterval, fifteenMinuteTimer_Tick);
            timers.CreateTimer(Timers.ThirtyMinuteInterval, thirtyMinuteTimer_Tick);
            timers.CreateTimer(Timers.OneSecondInterval, oneSecondTimer_Tick);
            timers.CreateTimer(Timers.TwelveHourInterval, twelveHourTimer_Tick);
        }

        #endregion

        #region Timer event handlers

        private void coinStatsTimer_Tick(object sender, EventArgs e)
        {
            RefreshCoinStatsAsync();
            
            coinStatsCountdownMinutes = coinStatsTimer.Interval / 1000 / 60;
        }

        private static bool ShowingModalDialog()
        {
            foreach (Form f in System.Windows.Forms.Application.OpenForms)
                if (f.Modal)
                    return true;

            return false;
        }

        private void restartTimer_Tick(object sender, EventArgs e)
        {
            RestartMiningLocallyIfMining();
        }

        private void UpdateLocalViewFromRemoteInstance()
        {
            if (!remotingEnabled)
                return;

            if (this.selectedRemoteInstance == null)
                return;

            GetRemoteApplicationModels(this.selectedRemoteInstance);
            SetHasChanges(this.remoteViewModel.HasChanges);

            updatingListView = true;
            try
            {
                RefreshListViewFromViewModel();
                RefreshIncomeSummary();
            }
            finally
            {
                updatingListView = false;
            }

            AutoSizeListViewColumns();
            UpdateMiningButtons();
            RefreshStatusBarFromViewModel();
        }

        private void oneHourTimer_Tick(object sender, EventArgs e)
        {            
            ClearPoolsFlaggedDown();
        }

        private void twelveHourTimer_Tick(object sender, EventArgs e)
        {
            UpdateBackendMinerAvailability();
            CheckForUpdates();
        }

        private void fiveMinuteTimer_Tick(object sender, EventArgs e)
        {
            //submit queued notifications to MobileMiner
            SubmitMobileMinerNotifications();

            //submit pools to MobileMiner before clearing cache
            SubmitMobileMinerPools();

            //clear cached information for Network Devices
            //(so we pick up changes)
            networkDevicePools.Clear();
            networkDeviceStatistics.Clear();
            networkDeviceVersions.Clear();
        }

        private void oneMinuteTimer_Tick(object sender, EventArgs e)
        {
            //if we do this with the Settings dialog open the user may have partially entered credentials
            if (!ShowingModalDialog())
                SubmitMobileMinerStatistics();
            
            //only broadcast if there are other instances (not just us)
            if (remotingEnabled && perksConfiguration.EnableRemoting && (instancesControl.Instances.Count > 1))
            {
                //broadcast 0 (e.g. even if not mining)
                BroadcastHashrate();
            }

            //coin stats countdown
            coinStatsCountdownMinutes--;
            RefreshStrategiesCountdown();

            PopulateSummaryInfoFromProcesses();

#if DEBUG
            SubmitMobileMinerNotifications();
#endif

            //restart suspect network devices
            if (applicationConfiguration.RestartCrashedMiners)
                RestartSuspectNetworkDevices();
        }

        private void thirtySecondTimer_Tick(object sender, EventArgs e)
        {
            //if we do this with the Settings dialog open the user may have partially entered credentials
            if (!ShowingModalDialog())
                CheckForMobileMinerCommands();

            UpdateLocalViewFromRemoteInstance();

            if (applicationConfiguration.RestartCrashedMiners && miningEngine.RelaunchCrashedMiners())
            {
                //clear any details stored correlated to processes - they could all be invalid after this
                processDeviceDetails.Clear();
                SaveOwnedProcesses();

                if (apiConsoleForm != null)
                    apiConsoleForm.PopulateMiners(miningEngine.MinerProcesses, networkDevicesConfiguration.Devices, localViewModel);
            }

            if (miningEngine.Mining)
            {
                RefreshPoolInfo();
                RefreshDetailsAreaIfVisible();
            }
        }

        private void fifteenMinuteTimer_Tick(object sender, EventArgs e)
        {
            ClearCachedNetworkDifficulties();

            if (applicationConfiguration.NetworkDeviceDetection)
            {
                CheckNetworkDevicesAsync();
                FindNetworkDevicesAsync();
            }
        }

        private void fifteenSecondTimer_Tick(object sender, EventArgs e)
        {
            if (applicationConfiguration.NetworkDeviceDetection)
                RefreshNetworkDeviceStatsAsync();
        }

        private void tenSecondTimer_Tick(object sender, EventArgs e)
        {
            if (miningEngine.Mining)
            {
                long timerInterval = ((System.Windows.Forms.Timer)sender).Interval;
                CheckIdleTimeForDynamicIntensity(timerInterval);
                ClearMinerStatsForDisabledCoins();
                RefreshDeviceStats();
            }
        }

        private void thirtyMinuteTimer_Tick(object sender, EventArgs e)
        {
            RefreshExchangeRates();
        }

        private void oneSecondTimer_Tick(object sender, EventArgs e)
        {
            CheckMiningOnStartupStatus();
        }
        #endregion

        #region MultiMiner remoting
        private void SetupRemoting()
        {
            using (new HourGlass())
            {
                if (perksConfiguration.EnableRemoting && perksConfiguration.PerksEnabled && !remotingEnabled)
                    EnableRemoting();
                else if ((!perksConfiguration.EnableRemoting || !perksConfiguration.PerksEnabled) && remotingEnabled)
                    DisableRemoting();
            }
        }

        private double GetLocalInstanceHashrate(string algorithm, bool includeNetworkDevices)
        {
            return GetTotalHashrate(localViewModel, algorithm, includeNetworkDevices);
        }

        private double GetVisibleInstanceHashrate(string algorithm, bool includeNetworkDevices)
        {
            return GetTotalHashrate(GetViewModelToView(), algorithm, includeNetworkDevices);
        }

        private static double GetTotalHashrate(MinerFormViewModel viewModel, string algorithm, bool includeNetworkDevices)
        {
            double result = 0.00;

            //only include Visible devices
            foreach (DeviceViewModel device in viewModel.Devices.Where(d => d.Visible))
            {
                if ((device.Coin != null) && 
                    
                    (device.Coin.Algorithm.Equals(algorithm, StringComparison.OrdinalIgnoreCase)) &&

                    //optionally filter out Network Devices
                    (includeNetworkDevices || (device.Kind != DeviceKind.NET)))
                    result += device.CurrentHashrate;
            }

            return result;
        }

        private void BroadcastHashrate()
        {
            //broadcast 0 (e.g. even if not mining)
            Remoting.Data.Transfer.Machine machine = new Remoting.Data.Transfer.Machine();
            PopulateLocalMachineHashrates(machine, false);

            try
            {
                Remoting.Broadcast.Broadcaster.Broadcast(machine);
            }
            catch (SocketException ex)
            {
                //e.g. no network connection on Linux
                ShowMultiMinerRemotingError(ex);
            }
        }

        private void PopulateLocalMachineHashrates(Remoting.Data.Transfer.Machine machine, bool includeNetworkDevices)
        {
            machine.TotalScryptHashrate = GetLocalInstanceHashrate(AlgorithmNames.Scrypt, includeNetworkDevices);
            machine.TotalSha256Hashrate = GetLocalInstanceHashrate(AlgorithmNames.SHA256, includeNetworkDevices);

            IList<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
            {
                double hashrate = GetLocalInstanceHashrate(algorithm.Name, includeNetworkDevices);
                if (hashrate > 0.00)
                    machine.TotalHashrates[algorithm.Name] = hashrate;
            }
        }

        private void DisableRemoting()
        {
            StopDiscovery();

            if (remotingServer != null)
                remotingServer.Shutdown();

            if (broadcastListener != null)
                broadcastListener.Stop();

            instancesControl.Visible = false;
            instancesContainer.Panel1Collapsed = true;

            instancesControl.UnregisterInstances();

            remotingEnabled = false;
        }

        private void PerformRequestedCommand(string clientAddress, string signature, Action action)
        {
            Instance remoteInstance = instancesControl.Instances.SingleOrDefault(i => i.IpAddress.Equals(clientAddress));
            if (remoteInstance == null)
                return;

            string expectedSignature = GetReceivingSignature(remoteInstance);
            if (!expectedSignature.Equals(signature))
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    string message = "MultiMiner Remoting signature verification failed";
                    PostNotification(message,
                        message, () =>
                        {
                        }, ToolTipIcon.Error);
                }));

                return;
            }

            action();
        }

        private void StartMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    StartMiningLocally();
                }));
            });
        }

        private void StopMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    StopMiningLocally();
                }));
            });
        }

        private void RestartMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    RestartMiningLocally();
                }));
            });
        }

        private void ScanHardwareRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ScanHardwareLocally();
                }));
            });
        }

        private void SetAllDevicesToCoinRequested(object sender, string coinSymbol, bool disableStrategies, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    SetAllDevicesToCoinLocally(coinSymbol, disableStrategies);
                }));
            });
        }

        private void SetDeviceToCoinRequested(object sender, IEnumerable<DeviceDescriptor> devices, string coinSymbol, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    SetDevicesToCoinLocally(devices, coinSymbol);
                }));
            });
        }

        private void SaveChangesRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    SaveChangesLocally();
                }));
            });
        }

        private void CancelChangesRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    CancelChangesLocally();
                }));
            });
        }

        private void ToggleDevicesRequested(object sender, IEnumerable<DeviceDescriptor> devices, bool enabled, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ToggleDevicesLocally(devices, enabled);
                }));
            });
        }

        private void ToggleDynamicIntensityRequested(object sender, bool enabled, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ToggleDynamicIntensityLocally(enabled);
                }));
            });
        }

        private void GetModelRequested(object sender, ModelEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ea.Devices = GetDeviceTransferObjects();
                ea.ConfiguredCoins = localViewModel.ConfiguredCoins.ToList();
                ea.Mining = miningEngine.Mining;
                ea.DynamicIntensity = localViewModel.DynamicIntensity;
                ea.HasChanges = localViewModel.HasChanges;
            });
        }

        private List<Remoting.Data.Transfer.Device> GetDeviceTransferObjects()
        {
            List<Remoting.Data.Transfer.Device> newList = new List<Remoting.Data.Transfer.Device>();
            foreach (DeviceViewModel viewModel in localViewModel.Devices)
            {
                MultiMiner.Remoting.Data.Transfer.Device dto = new Remoting.Data.Transfer.Device();
                ObjectCopier.CopyObject(viewModel, dto, "Workers");
                foreach (DeviceViewModel source in viewModel.Workers)
                {
                    Remoting.Data.Transfer.Device destination = new Remoting.Data.Transfer.Device();
                    ObjectCopier.CopyObject(source, destination, "Workers");
                    dto.Workers.Add(destination);
                }
                newList.Add(dto);
            }
            return newList;
        }

        private void GetConfigurationRequested(object sender, ConfigurationEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ObjectCopier.CopyObject(applicationConfiguration.ToTransferObject(), ea.Application);
                ObjectCopier.CopyObject(engineConfiguration.ToTransferObject(), ea.Engine);
                ObjectCopier.CopyObject(pathConfiguration, ea.Path);
                ObjectCopier.CopyObject(perksConfiguration, ea.Perks);
            });
        }

        private void SetCoinConfigurationsRequested(object sender, Engine.Data.Configuration.Coin[] coinConfigurations, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ObjectCopier.CopyObject(coinConfigurations, this.engineConfiguration.CoinConfigurations);
                this.engineConfiguration.SaveCoinConfigurations();

                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    localViewModel.ApplyCoinConfigurationModels(engineConfiguration.CoinConfigurations);
                    localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                        engineConfiguration.CoinConfigurations);
                    RefreshViewForConfigurationChanges();
                }));
            });
        }

        private void SetConfigurationRequested(object sender, ConfigurationEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                string oldConfigPath = this.pathConfiguration.SharedConfigPath;

                if (ea.Application != null)
                {
                    ObjectCopier.CopyObject(ea.Application.ToModelObject(), this.applicationConfiguration);
                    this.applicationConfiguration.SaveApplicationConfiguration();
                }

                if (ea.Engine != null)
                {
                    ObjectCopier.CopyObject(ea.Engine.ToModelObject(), this.engineConfiguration);
                    this.engineConfiguration.SaveCoinConfigurations();
                    this.engineConfiguration.SaveMinerConfiguration();
                    this.engineConfiguration.SaveStrategyConfiguration();
                    this.engineConfiguration.SaveDeviceConfigurations();
                }

                if (ea.Path != null)
                {
                    ObjectCopier.CopyObject(ea.Path, this.pathConfiguration);
                    this.pathConfiguration.SavePathConfiguration();
                }

                if (ea.Perks != null)
                {
                    ObjectCopier.CopyObject(ea.Perks, this.perksConfiguration);
                    this.perksConfiguration.SavePerksConfiguration();
                }

                //save settings as the "shared" config path may have changed
                //these are settings not considered machine/device-specific
                //e.g. no device settings, no miner settings
                string newConfigPath = pathConfiguration.SharedConfigPath;
                MigrateSettingsToNewFolder(oldConfigPath, newConfigPath);

                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    localViewModel.ApplyCoinConfigurationModels(engineConfiguration.CoinConfigurations);
                    localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                        engineConfiguration.CoinConfigurations);
                    RefreshViewForConfigurationChanges();
                }));
            });
        }

        private void MigrateSettingsToNewFolder(string oldConfigPath, string newConfigPath)
        {
            //if the shared config path changed, and there are already settings
            //in that path, load those settings (so they aren't overwritten)
            //idea being the user has shared settings already there they want to use
            if (!Path.Equals(oldConfigPath, newConfigPath))
            {
                if (File.Exists(Path.Combine(newConfigPath, Path.GetFileName(applicationConfiguration.ApplicationConfigurationFileName()))))
                    applicationConfiguration.LoadApplicationConfiguration(newConfigPath);
                if (File.Exists(Path.Combine(newConfigPath, Path.GetFileName(perksConfiguration.PerksConfigurationFileName()))))
                    perksConfiguration.LoadPerksConfiguration(newConfigPath);
                if (File.Exists(Path.Combine(newConfigPath, Path.GetFileName(engineConfiguration.CoinConfigurationsFileName()))))
                    engineConfiguration.LoadCoinConfigurations(newConfigPath);
                if (File.Exists(Path.Combine(newConfigPath, Path.GetFileName(engineConfiguration.StrategyConfigurationsFileName()))))
                    engineConfiguration.LoadStrategyConfiguration(newConfigPath);
            }
        }

        private void UpgradeMultiMinerRequested(object sender, RemoteCommandEventArgs ea)
        {
            string installedVersion, availableVersion;
            bool updatesAvailable = MultiMinerHasUpdates(out availableVersion, out installedVersion);
            if (!updatesAvailable)
                return;

            BeginInvoke((Action)(() =>
            {
                //code to update UI
                bool wasMining = miningEngine.Mining;

                if (wasMining)
                    StopMiningLocally();

                //this will restart the app
                InstallMultiMinerLocally();
            }));
        }

        private void UpgradeBackendMinerRequested(object sender, RemoteCommandEventArgs ea)
        {
            string installedVersion, availableVersion;
            bool updatesAvailable = BackendMinerHasUpdates(out availableVersion, out installedVersion);
            if (!updatesAvailable)
                return;

            BeginInvoke((Action)(() =>
            {
                //code to update UI
                bool wasMining = miningEngine.Mining;

                if (wasMining)
                    StopMiningLocally();

                InstallBackendMinerLocally(MinerFactory.Instance.GetDefaultMiner());

                //only start mining if we stopped mining
                if (wasMining)
                    StartMiningLocally();
            }));
        }

        private void SetupRemotingEventHandlers()
        {
            ApplicationProxy.Instance.StartMiningRequested -= StartMiningRequested;
            ApplicationProxy.Instance.StartMiningRequested += StartMiningRequested;

            ApplicationProxy.Instance.StopMiningRequested -= StopMiningRequested;
            ApplicationProxy.Instance.StopMiningRequested += StopMiningRequested;

            ApplicationProxy.Instance.RestartMiningRequested -= RestartMiningRequested;
            ApplicationProxy.Instance.RestartMiningRequested += RestartMiningRequested;

            ApplicationProxy.Instance.ScanHardwareRequested -= ScanHardwareRequested;
            ApplicationProxy.Instance.ScanHardwareRequested += ScanHardwareRequested;

            ApplicationProxy.Instance.SetAllDevicesToCoinRequested -= SetAllDevicesToCoinRequested;
            ApplicationProxy.Instance.SetAllDevicesToCoinRequested += SetAllDevicesToCoinRequested;

            ApplicationProxy.Instance.SetDeviceToCoinRequested -= SetDeviceToCoinRequested;
            ApplicationProxy.Instance.SetDeviceToCoinRequested += SetDeviceToCoinRequested;

            ApplicationProxy.Instance.SaveChangesRequested -= SaveChangesRequested;
            ApplicationProxy.Instance.SaveChangesRequested += SaveChangesRequested;

            ApplicationProxy.Instance.CancelChangesRequested -= CancelChangesRequested;
            ApplicationProxy.Instance.CancelChangesRequested += CancelChangesRequested;

            ApplicationProxy.Instance.ToggleDevicesRequested -= ToggleDevicesRequested;
            ApplicationProxy.Instance.ToggleDevicesRequested += ToggleDevicesRequested;

            ApplicationProxy.Instance.ToggleDynamicIntensityRequested -= ToggleDynamicIntensityRequested;
            ApplicationProxy.Instance.ToggleDynamicIntensityRequested += ToggleDynamicIntensityRequested;

            ApplicationProxy.Instance.GetModelRequested -= GetModelRequested;
            ApplicationProxy.Instance.GetModelRequested += GetModelRequested;

            ApplicationProxy.Instance.GetConfigurationRequested -= GetConfigurationRequested;
            ApplicationProxy.Instance.GetConfigurationRequested += GetConfigurationRequested;

            ApplicationProxy.Instance.SetConfigurationRequested -= SetConfigurationRequested;
            ApplicationProxy.Instance.SetConfigurationRequested += SetConfigurationRequested;

            ApplicationProxy.Instance.UpgradeMultiMinerRequested -= UpgradeMultiMinerRequested;
            ApplicationProxy.Instance.UpgradeMultiMinerRequested += UpgradeMultiMinerRequested;

            ApplicationProxy.Instance.UpgradeBackendMinerRequested -= UpgradeBackendMinerRequested;
            ApplicationProxy.Instance.UpgradeBackendMinerRequested += UpgradeBackendMinerRequested;

            ApplicationProxy.Instance.SetCoinConfigurationsRequested -= SetCoinConfigurationsRequested;
            ApplicationProxy.Instance.SetCoinConfigurationsRequested += SetCoinConfigurationsRequested;
        }

        private void EnableRemoting()
        {
            SetupRemotingEventHandlers();

            fingerprint = random.Next();

            if (workGroupName == null)
                this.workGroupName = Utility.Net.LocalNetwork.GetWorkGroupName();

            //start Broadcast Listener before Discovery so we can
            //get initial info (hashrates) sent by other instances
            broadcastListener = new Remoting.Broadcast.Listener();
            broadcastListener.PacketReceived += HandlePacketReceived;
            broadcastListener.Listen();

            SetupDiscovery();

            remotingServer = new RemotingServer();
            remotingServer.Startup();

            UpdateInstancesVisibility();

            remotingEnabled = true;
        }

        private void HandlePacketReceived(object sender, Remoting.Broadcast.PacketReceivedArgs ea)
        {
            Type type = typeof(Remoting.Data.Transfer.Machine);
            if (ea.Packet.Descriptor.Equals(type.FullName))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Remoting.Data.Transfer.Machine dto = serializer.Deserialize<Remoting.Data.Transfer.Machine>(ea.Packet.Payload);

                if ((instancesControl.ThisPCInstance != null) &&
                    (instancesControl.ThisPCInstance.IpAddress.Equals(ea.IpAddress)))
                    //don't process packets broadcast by This PC
                    //for instance we don't broadcast out hashrate for Network Devices and
                    //so don't want to process the packet
                    return;

                //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                if (tearingDown) return;

                BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    instancesControl.ApplyMachineInformation(ea.IpAddress, dto);
                }));
            }
        }

        private void SetupDiscovery()
        {
            StopDiscovery();
            StartDiscovery();
        }

        private void StartDiscovery()
        {
            discoveryListener = new Listener();
            discoveryListener.InstanceOnline += HandleInstanceOnline;
            discoveryListener.InstanceOffline += HandleInstanceOffline;
            discoveryListener.Listen(fingerprint);

            try
            {
                Broadcaster.Broadcast(Verbs.Online, fingerprint);
            }
            catch (SocketException ex)
            {
                //e.g. no network connection on Linux
                ShowMultiMinerRemotingError(ex);
            }
        }

        private void StopDiscovery()
        {
            if (discoveryListener != null)
                discoveryListener.Stop();

            //broadcast after so we aren't needless processing our own message
            try
            {
                Broadcaster.Broadcast(Verbs.Offline, fingerprint);
            }
            catch (SocketException ex)
            {
                //e.g. no network connection on Linux
                ShowMultiMinerRemotingError(ex);
            }
        }

        private void HandleInstanceOnline(object sender, InstanceChangedArgs ea)
        {
            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
            if (tearingDown) return;

            BeginInvoke((Action)(() =>
            {
                //code to update UI
                instancesControl.RegisterInstance(ea.Instance);
                UpdateInstancesVisibility();
            }));

            //send our hashrate back to the machine that is now Online
            if (!ea.Instance.MachineName.Equals(Environment.MachineName))
                SendHashrate(ea.Instance.IpAddress);
        }

        private void SendHashrate(string ipAddress)
        {
            Remoting.Data.Transfer.Machine machine = new Remoting.Data.Transfer.Machine();
            PopulateLocalMachineHashrates(machine, false);
            Remoting.Broadcast.Sender.Send(IPAddress.Parse(ipAddress), machine);
        }

        private void HandleInstanceOffline(object sender, InstanceChangedArgs ea)
        {
            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
            if (tearingDown) return;

            BeginInvoke((Action)(() =>
            {
                //code to update UI
                instancesControl.UnregisterInstance(ea.Instance);
                UpdateInstancesVisibility();
            }));
        }

        private void UpdateInstancesVisibility()
        {
            instancesContainer.Panel1Collapsed = !perksConfiguration.EnableRemoting || (instancesControl.Instances.Count <= 1);
            instancesControl.Visible = !instancesContainer.Panel1Collapsed;
        }

        private void instancesControl1_SelectedInstanceChanged(object sender, Instance instance)
        {
            bool isThisPc = instance.MachineName.Equals(Environment.MachineName);

            if (!isThisPc)
                GetRemoteApplicationModels(instance);

            //don't set flags until remote VM is fetched
            if (isThisPc)
                this.selectedRemoteInstance = null;
            else
                this.selectedRemoteInstance = instance;

            //only This PC for now
            deviceListView.LabelEdit = isThisPc;

            updatingListView = true;
            try
            {
                RefreshListViewFromViewModel();
                AutoSizeListViewColumns();

                deviceListView.CheckBoxes = (deviceListView.View != View.Tile);
            }
            finally
            {
                updatingListView = false;
            }

            RefreshIncomeSummary();
            UpdateMiningButtons();
            RefreshCoinPopupMenu();
            SetHasChanges(GetViewModelToView().HasChanges);
            RefreshStatusBarFromViewModel();
        }

        private static IRemotingService GetServiceChannelForInstance(Instance instance)
        {
            EndpointAddress address = new EndpointAddress(new Uri(String.Format("net.tcp://{0}:{1}/RemotingService", instance.IpAddress, Config.RemotingPort)));
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            ChannelFactory<IRemotingService> factory = new ChannelFactory<IRemotingService>(binding, address);
            IRemotingService serviceChannel = factory.CreateChannel();
            return serviceChannel;
        }

        private void PerformRemoteCommand(Instance instance, Action<IRemotingService> action)
        {
            using (new HourGlass())
            {
                try
                {
                    IRemotingService serviceChannel = GetServiceChannelForInstance(instance);
                    action(serviceChannel);
                }
                catch (SystemException ex)
                {
                    if ((ex is CommunicationException) || (ex is TimeoutException))
                        ShowMultiMinerRemotingError(ex);
                    else
                        throw;
                }
            }
        }

        private void GetRemoteApplicationModels(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                Remoting.Data.Transfer.Device[] devices;
                PoolGroup[] configurations;
                bool mining, hasChanges, dynamicIntensity;

                //set some safe defaults in case the call fails
                this.remoteViewModel = new MinerFormViewModel();

                service.GetApplicationModels(GetSendingSignature(instance), out devices, out configurations, out mining, out hasChanges, out dynamicIntensity);

                this.remoteInstanceMining = mining;
                this.remoteViewModel.HasChanges = hasChanges;
                this.remoteViewModel.DynamicIntensity = dynamicIntensity;
                SaveDeviceTransferObjects(devices);
                SaveCoinTransferObjects(configurations);
            });
        }

        private void GetRemoteApplicationConfiguration(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.GetApplicationConfiguration(
                    GetSendingSignature(instance),
                    out remoteApplicationConfig,
                    out remoteEngineConfig,
                    out remotePathConfig,
                    out remotePerksConfig);
            });
        }

        private void SaveCoinTransferObjects(IEnumerable<PoolGroup> configurations)
        {
            this.remoteViewModel.ConfiguredCoins = configurations.ToList();
        }

        private void ShowMultiMinerRemotingError(SystemException ex)
        {
            BeginInvoke((Action)(() =>
            {
                //code to update UI
                string message = "MultiMiner Remoting communication failed";
                PostNotification(message,
                    message, () =>
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }, ToolTipIcon.Error);
            }));
        }

        private const uint remotingPepper = 4108157753;
        private string workGroupName = null;

        private static string GetStringHash(string text)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(text);

            // this is where you get the actual binary hash
            SHA256Managed hasher = new SHA256Managed();
            byte[] inputHashedBytes = hasher.ComputeHash(inputBytes);

            // but you want it in a string format, similar to a variety of UNIX tools
            string result = BitConverter.ToString(inputHashedBytes)
                // this will remove all the dashes in between each two characters
               .Replace("-", string.Empty)
                // and make it lowercase
               .ToLower();

            return result;
        }

        private string GetSendingSignature(Instance destination)
        {
            string signature = String.Format("{0}{1}{2}{3}{4}{5}{6}",
                Environment.MachineName,
                this.fingerprint,
                destination.MachineName,
                destination.Fingerprint,
                remotingPepper,
                workGroupName,
                perksConfiguration.RemotingPassword);
            return GetStringHash(signature);
        }

        private string GetReceivingSignature(Instance source)
        {
            string signature = String.Format("{0}{1}{2}{3}{4}{5}{6}",
                source.MachineName,
                source.Fingerprint,
                Environment.MachineName,
                this.fingerprint,
                remotingPepper,
                workGroupName,
                perksConfiguration.RemotingPassword);
            return GetStringHash(signature);
        }

        private void StartMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.StartMining(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void SaveChangesRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.SaveChanges(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void CancelChangesRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.CancelChanges(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void UpdateViewFromRemoteInTheFuture(int seconds)
        {
            timer = new System.Threading.Timer(
                                (object state) =>
                                {
                                    BeginInvoke((Action)(() =>
                                    {
                                        //code to update UI
                                        UpdateLocalViewFromRemoteInstance();
                                        timer.Dispose();
                                    }));
                                }
                                , null // no state required
                                , TimeSpan.FromSeconds(seconds) // Do it in x seconds
                                , TimeSpan.FromMilliseconds(-1)); // don't repeat
        }

        private System.Threading.Timer timer = null;
        private void StopMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.StopMining(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void RestartMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.RestartMining(GetSendingSignature(instance));
            });
        }

        private void ScanHardwareRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.ScanHardware(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void SetAllDevicesToCoinRemotely(Instance instance, string coinSymbol, bool disableStrategies)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.SetAllDevicesToCoin(GetSendingSignature(instance), coinSymbol, disableStrategies);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void SetDevicesToCoinRemotely(Instance instance, IEnumerable<DeviceDescriptor> devices, string coinName)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                List<DeviceDescriptor> descriptors = CloneDescriptors(devices);
                service.SetDevicesToCoin(GetSendingSignature(instance), descriptors.ToArray(), coinName);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private static List<DeviceDescriptor> CloneDescriptors(IEnumerable<DeviceDescriptor> devices)
        {
            List<DeviceDescriptor> descriptors = new List<DeviceDescriptor>();
            foreach (DeviceDescriptor device in devices)
            {
                DeviceDescriptor descriptor = new DeviceDescriptor();
                ObjectCopier.CopyObject(device, descriptor);
                descriptors.Add(descriptor);
            }
            return descriptors;
        }

        private void ToggleDevicesRemotely(Instance instance, IEnumerable<DeviceDescriptor> devices, bool enabled)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                List<DeviceDescriptor> descriptors = CloneDescriptors(devices);
                service.ToggleDevices(GetSendingSignature(instance), descriptors.ToArray(), enabled);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void ToggleDynamicIntensityRemotely(Instance instance, bool enabled)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.ToggleDynamicIntensity(GetSendingSignature(instance), enabled);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void SetCoinConfigurationOnAllRigs(Engine.Data.Configuration.Coin[] coinConfigurations)
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = instancesControl.Instances.Where(i => i != instancesControl.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                PerformRemoteCommand(instance, (service) =>
                {
                    service.SetCoinConfigurations(GetSendingSignature(instance), coinConfigurations);
                });
            }
        }

        private void SetConfigurationRemotely(
            Instance instance,
            Remoting.Data.Transfer.Configuration.Application application,
            Remoting.Data.Transfer.Configuration.Engine engine,
            Remoting.Data.Transfer.Configuration.Path path,
            Remoting.Data.Transfer.Configuration.Perks perks)
        {
            PerformRemoteCommand(instance, (service) =>
            {
                service.SetApplicationConfiguration(
                    GetSendingSignature(instance),
                    application,
                    engine,
                    path,
                    perks);
            });
        }

        private void SetDevicesToCoin(List<DeviceDescriptor> devices, string coinName)
        {
            if (this.selectedRemoteInstance == null)
            {
                SetDevicesToCoinLocally(devices, coinName);
            }
            else
            {
                SetDevicesToCoinRemotely(this.selectedRemoteInstance, devices, coinName);
            }
        }

        private void SaveDeviceTransferObjects(IEnumerable<Remoting.Data.Transfer.Device> devices)
        {
            remoteViewModel.Devices.Clear();
            foreach (Remoting.Data.Transfer.Device dto in devices)
            {
                DeviceViewModel viewModel = new DeviceViewModel();
                ObjectCopier.CopyObject(dto, viewModel, "Workers");
                foreach (Remoting.Data.Transfer.Device source in dto.Workers)
                {
                    DeviceViewModel destination = new DeviceViewModel();
                    ObjectCopier.CopyObject(source, destination, "Workers");
                    viewModel.Workers.Add(destination);
                }
                remoteViewModel.Devices.Add(viewModel);
            }
        }

        private void InstallBackendMinerRemotely()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = instancesControl.Instances.Where(i => i != instancesControl.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                PerformRemoteCommand(instance, (service) =>
                {
                    service.UpgradeBackendMiner(GetSendingSignature(instance));
                });
            }
        }

        private void InstallMultiMinerRemotely()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = instancesControl.Instances.Where(i => i != instancesControl.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                PerformRemoteCommand(instance, (service) =>
                {
                    service.UpgradeMultiMiner(GetSendingSignature(instance));
                });
            }
        }
        #endregion

        #region Coin API
        private void SetupCoinApi()
        {
            this.coinWarzApiContext = new CoinWarz.ApiContext(applicationConfiguration.CoinWarzApiKey);
            this.coinChooseApiContext = new CoinChoose.ApiContext();
            this.whatMineApiContext = new WhatMine.ApiContext(applicationConfiguration.WhatMineApiKey);
        }

        private void ShowMultipoolApiErrorNotification(MultipoolApi.IApiContext apiContext, Exception ex)
        {
            string siteUrl = apiContext.GetInfoUrl();
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message,
                summary, () =>
                {
                    MessageBox.Show(String.Format("{0}: {1}", summary, details), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                },
                ToolTipIcon.Warning, apiUrl);
        }

        private void ShowCoinApiErrorNotification(IApiContext apiContext, Exception ex)
        {
            string siteUrl = apiContext.GetInfoUrl();
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message,
                summary, () =>
                {
                    MessageBox.Show(String.Format("{0}: {1}", summary, details), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                },
                ToolTipIcon.Warning, apiUrl);
        }

        private bool ApplyCoinInformationToViewModel(IApiContext apiContext)
        {
            try
            {
                //remove dupes by Symbol in case the Coin API returns them - seen from user
                coinApiInformation = apiContext.GetCoinInformation(UserAgent.AgentString).GroupBy(c => c.Symbol).Select(g => g.First()).ToList();

                successfulApiContext = apiContext;

                ApplyCoinInformationToViewModel();
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException) || (ex is CoinApiException) ||
                    (ex is JsonReaderException))
                {
                    if (applicationConfiguration.ShowApiErrors)
                    {
                        BeginInvoke((Action)(() =>
                        {
                            //code to update UI
                            ShowCoinApiErrorNotification(apiContext, ex);
                        }));
                    }
                    return false;
                }
                throw;
            }

            return true;
        }

        private void RefreshCoinStatsAsync()
        {
            Action asyncAction = RefreshAllCoinStats;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);
                    BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        UpdateApplicationFromCoinStats();
                    }));

                }, null);
        }

        private void RefreshCoinStats()
        {
            RefreshAllCoinStats();
            UpdateApplicationFromCoinStats();
        }

        private void UpdateApplicationFromCoinStats()
        {
            SaveCoinStatsToKnownCoins();
            RefreshListViewFromViewModel();
            RefreshCoinStatsLabel();
            AutoSizeListViewColumns();
            SuggestCoinsToMine();
            RefreshDetailsAreaIfVisible();
            CheckAndApplyMiningStrategy();
        }

        private void RefreshAllCoinStats()
        {
            RefreshSingleCoinStats();
            RefreshMultiCoinStats();
        }

        private IEnumerable<MultipoolInformation> GetNiceHashInformation()
        {
            IEnumerable<MultipoolInformation> multipoolInformation = null;
            NiceHash.ApiContext apiContext = new NiceHash.ApiContext();
            try
            {
                multipoolInformation = apiContext.GetMultipoolInformation();
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException) || (ex is CoinApiException) ||
                    (ex is JsonReaderException))
                {
                    if (applicationConfiguration.ShowApiErrors)
                        ShowMultipoolApiErrorNotification(apiContext, ex);
                    return null;
                }
                throw;
            }
            return multipoolInformation;
        }

        private void RefreshMultiCoinStats()
        {
            RefreshNiceHashStats();
        }

        private void RefreshNiceHashStats()
        {
            const string Prefix = "NiceHash";

            //the NiceHash API is slow
            //only fetch from them if:
            //1. We have no NiceHash coins in KnownCoins
            //2. Or we have a Multipool setup for NiceHash
            bool initialLoad = !knownCoins.Any(kc => kc.Id.Contains(Prefix));
            bool miningNiceHash = engineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Contains(Prefix));
            if (!initialLoad && !miningNiceHash)
            {
                return;
            }

            IEnumerable<MultipoolInformation> multipoolInformation = GetNiceHashInformation();

            //we're offline or the API is offline
            if (multipoolInformation == null)
                return;
            
            //coinApiInformation may be Null if Single-Coin APIs are offline
            if (coinApiInformation == null)
                coinApiInformation = new List<CoinInformation>();

            coinApiInformation.AddRange(multipoolInformation
                .Select(mpi => new CoinInformation
                {
                    Symbol = Prefix + ":" + mpi.Algorithm,
                    Name = Prefix + " - " + mpi.Algorithm,
                    Profitability = mpi.Profitability,
                    AverageProfitability = mpi.Profitability,
                    AdjustedProfitability = mpi.Profitability,
                    Price = mpi.Price,
                    Algorithm = KnownAlgorithms.Algorithms.Single(ka => ka.Name.Equals(mpi.Algorithm)).FullName
                }));
        }

        private void RefreshSingleCoinStats()
        {
            //always load known coins from file
            //CoinChoose may not show coins it once did if there are no orders
            LoadKnownCoinsFromFile();

            IApiContext preferredApiContext, backupApiContext;
            if (applicationConfiguration.UseCoinWarzApi)
            {
                preferredApiContext = this.coinWarzApiContext;
                backupApiContext = this.coinChooseApiContext;
            }
            else if (applicationConfiguration.UseWhatMineApi)
            {
                preferredApiContext = this.whatMineApiContext;
                backupApiContext = this.coinChooseApiContext;
            }
            else
            {
                preferredApiContext = this.coinChooseApiContext;
                backupApiContext = this.coinWarzApiContext;
            }

            bool success = ApplyCoinInformationToViewModel(preferredApiContext);
            if (!success &&
                //don't try to use CoinWarz as a backup unless the user has entered an API key for CoinWarz
                ((backupApiContext == this.coinChooseApiContext) || !String.IsNullOrEmpty(this.applicationConfiguration.CoinWarzApiKey)))
                success = ApplyCoinInformationToViewModel(backupApiContext);
            
            FixCoinSymbolDiscrepencies();
        }

        private void FixCoinSymbolDiscrepencies()
        {
            FixKnownCoinSymbolDiscrepencies();
            SaveKnownCoinsToFile();

            FixCoinApiSymbolDiscrepencies();
        }

        private void FixCoinApiSymbolDiscrepencies()
        {
            //we're offline or the API is offline
            if (coinApiInformation == null)
                return;

            CoinInformation badCoin = coinApiInformation
                .ToList() //get a copy - populated async & collection may be modified
                .SingleOrDefault(c => !String.IsNullOrEmpty(c.Symbol) && c.Symbol.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
            {
                CoinInformation goodCoin = coinApiInformation
                    .ToList() //get a copy - populated async & collection may be modified
                    .SingleOrDefault(c => !String.IsNullOrEmpty(c.Symbol) && c.Symbol.Equals(KnownCoins.DogecoinSymbol, StringComparison.OrdinalIgnoreCase));
                if (goodCoin == null)
                    badCoin.Symbol = KnownCoins.DogecoinSymbol;
                else
                    coinApiInformation.Remove(badCoin);
            }
        }

        private void FixKnownCoinSymbolDiscrepencies()
        {
            PoolGroup badCoin = knownCoins.SingleOrDefault(c => c.Id.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
            {
                PoolGroup goodCoin = knownCoins.SingleOrDefault(c => c.Id.Equals(KnownCoins.DogecoinSymbol, StringComparison.OrdinalIgnoreCase));
                if (goodCoin == null)
                    badCoin.Id = KnownCoins.DogecoinSymbol;
                else
                    knownCoins.Remove(badCoin);
            }
        }
        #endregion

        #region Exchange API
        private void RefreshExchangeRates()
        {
            if (perksConfiguration.PerksEnabled && perksConfiguration.ShowExchangeRates)
            {
                try
                {
                    sellPrices = new Blockchain.ApiContext().GetExchangeInformation();
                }
                catch (Exception ex)
                {
                    //don't crash if website cannot be resolved or JSON cannot be parsed
                    if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException))
                    {
                        if (applicationConfiguration.ShowApiErrors)
                            ShowExchangeApiErrorNotification(ex);
                        return;
                    }
                    throw;
                }
            }
        }

        private void ShowExchangeApiErrorNotification(Exception ex)
        {
            ExchangeApi.IApiContext apiContext = new Blockchain.ApiContext();

            string siteUrl = apiContext.GetInfoUrl();
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            PostNotification(ex.Message,
                String.Format("Error parsing the {0} JSON API", apiName), () =>
                {
                    Process.Start(apiUrl);
                },
                ToolTipIcon.Warning, siteUrl);
        }
        #endregion

        #region MobileMiner API
        private string GetMobileMinerUrl()
        {
            string prefix = "https://";
            if (!applicationConfiguration.MobileMinerUsesHttps)
                prefix = "http://";

            //custom domain makes it easier to migrate hosts if needed
            string result = prefix + "api.mobileminerapp.com";

            if (!OSVersionPlatform.IsWindowsVistaOrHigher())
                //SNI SSL not supported on XP
                result = prefix + "mobileminer.azurewebsites.net/api";

            return result;
        }

        private const string mobileMinerApiKey = "P3mVX95iP7xfoI";

        //don't show a dialog for a 403 after successful submissions.
        //it's not ideal but there have been two reports now of this
        //being triggered by someone who has valid credentials, and
        //i've seen it myself as well
        private bool mobileMinerSuccess = false;
        private void SubmitMobileMinerStatistics()
        {
            //are remote monitoring enabled?
            if (!applicationConfiguration.MobileMinerMonitoring)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList = new List<MobileMiner.Data.MiningStatistics>();
            
            Action<List<MultiMiner.MobileMiner.Data.MiningStatistics>> asyncAction = AddAllMinerStatistics;
            asyncAction.BeginInvoke(statisticsList,
                ar =>
                {
                    asyncAction.EndInvoke(ar);
                    BeginInvoke((Action)(() =>
                    {
                        //code to update UI

                        if (statisticsList.Count > 0)
                        {
                            if (submitMiningStatisticsDelegate == null)
                                submitMiningStatisticsDelegate = SubmitMiningStatistics;

                            submitMiningStatisticsDelegate.BeginInvoke(statisticsList, submitMiningStatisticsDelegate.EndInvoke, null);
                        }

                    }));

                }, null);
        }

        private void AddAllMinerStatistics(List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList)
        {
            if (!applicationConfiguration.MobileMinerNetworkMonitorOnly)
                AddLocalMinerStatistics(statisticsList);

            AddNetworkMinerStatistics(statisticsList);
        }

        private void AddNetworkMinerStatistics(List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList)
        {
            //is Network Device detection enabled?
            if (!applicationConfiguration.NetworkDeviceDetection)
                return;

            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<NetworkDevices.NetworkDevice> networkDevices = networkDevicesConfiguration.Devices.ToList();

            foreach (Data.Configuration.NetworkDevices.NetworkDevice networkDevice in networkDevices)
            {
                List<DeviceInformation> deviceInformationList = GetDeviceInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

                if (deviceInformationList == null) //handled failure getting API info
                    continue;

                List<PoolInformation> poolInformationList = GetCachedPoolInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

                Xgminer.Api.Data.VersionInformation versionInformation = GetVersionInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

                //we cannot continue without versionInformation as the MinerName is required by MobileMiner or it returns HTTP 400
                if (versionInformation == null) //handled failure getting API info
                    continue;

                foreach (DeviceInformation deviceInformation in deviceInformationList)
                {
                    string devicePath = String.Format("{0}:{1}", networkDevice.IPAddress, networkDevice.Port);

                    //don't submit stats until we have a valid ViewModel for the Network Device
                    DeviceViewModel deviceViewModel = localViewModel.Devices.SingleOrDefault(d => d.Path.Equals(devicePath));
                    if (deviceViewModel == null)
                        continue;

                    MobileMiner.Data.MiningStatistics miningStatistics = new MobileMiner.Data.MiningStatistics()
                    {
                        // submit the Friendly device / machine name
                        MachineName = localViewModel.GetFriendlyDeviceName(devicePath, devicePath),

                        // versionInformation may be null if the read timed out
                        MinerName = versionInformation == null ? String.Empty : versionInformation.Name,

                        CoinName = KnownCoins.BitcoinName,
                        CoinSymbol = KnownCoins.BitcoinSymbol,
                        Algorithm = AlgorithmFullNames.SHA256,
                        Appliance = true
                    };

                    miningStatistics.PopulateFrom(deviceInformation);

                    //ensure poolIndex is valid for poolInformationList
                    //user(s) reported index errors so we can't out on the RPC API here
                    //https://github.com/nwoolls/MultiMiner/issues/64
                    if ((deviceInformation.PoolIndex >= 0) &&
                        // poolInformationList may be null if an RPC API call timed out
                        (poolInformationList != null) &&
                        (deviceInformation.PoolIndex < poolInformationList.Count))
                    {
                        string poolUrl = poolInformationList[deviceInformation.PoolIndex].Url;
                        miningStatistics.PoolName = poolUrl.DomainFromHost();

                        Coin coinConfiguration = CoinConfigurationForPoolUrl(poolUrl);
                        if (coinConfiguration != null)
                        {
                            miningStatistics.CoinName = coinConfiguration.PoolGroup.Name;
                            miningStatistics.CoinSymbol = coinConfiguration.PoolGroup.Id;
                            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(coinConfiguration.PoolGroup.Algorithm);

                            //MobileMiner is only SHA & Scrypt for now
                            if ((algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA2) ||
                                (algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA3))
                                miningStatistics.Algorithm = AlgorithmFullNames.SHA256;
                            else
                                miningStatistics.Algorithm = AlgorithmFullNames.Scrypt;
                        }
                    }

                    statisticsList.Add(miningStatistics);
                }
            }
        }

        private string GetFriendlyDeviceName(MultiMiner.Xgminer.Data.Device device)
        {
            string result = device.Name;
                      
            DeviceViewModel deviceViewModel = localViewModel.Devices.SingleOrDefault(d => d.Equals(device));
            if ((deviceViewModel != null) && !String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                result = deviceViewModel.FriendlyName;
 
            return result;
        }

        private VersionInformation GetVersionInfoFromAddress(string ipAddress, int port)
        {
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            VersionInformation versionInformation = null;
            try
            {
                try
                {
                    versionInformation = apiContext.GetVersionInformation();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    versionInformation = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                versionInformation = null;
            }

            return versionInformation;
        }

        private void AddLocalMinerStatistics(List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList)
        {            
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<MinerProcess> minerProcesses = miningEngine.MinerProcesses.ToList();

            foreach (MinerProcess minerProcess in minerProcesses)
            {
                List<DeviceInformation> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                    continue;

                //starting with bfgminer 3.7 we need the DEVDETAILS response to tie things from DEVS up with -d? details
                List<DeviceDetails> processDevices = GetProcessDeviceDetails(minerProcess, deviceInformationList);

                if (processDevices == null) //handled failure getting API info
                    continue;

                foreach (DeviceInformation deviceInformation in deviceInformationList)
                {
                    MobileMiner.Data.MiningStatistics miningStatistics = new MobileMiner.Data.MiningStatistics();

                    miningStatistics.MachineName = Environment.MachineName;

                    PopulateMobileMinerStatistics(miningStatistics, deviceInformation, GetCoinNameForApiContext(minerProcess.ApiContext));

                    DeviceDetails deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
                        && (d.ID == deviceInformation.ID));
                    int deviceIndex = GetDeviceIndexForDeviceDetails(deviceDetails, minerProcess);
                    Xgminer.Data.Device device = devices[deviceIndex];
                    Engine.Data.Configuration.Coin coinConfiguration = CoinConfigurationForDevice(device);
                    
                    miningStatistics.FullName = GetFriendlyDeviceName(device);

                    miningStatistics.PoolName = GetPoolNameByIndex(coinConfiguration, deviceInformation.PoolIndex).DomainFromHost();

                    statisticsList.Add(miningStatistics);
                }
            }
        }

        private string GetCoinNameForApiContext(Xgminer.Api.ApiContext apiContext)
        {
            string coinName = string.Empty;

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                Xgminer.Api.ApiContext loopContext = minerProcess.ApiContext;
                if (loopContext == apiContext)
                {
                    coinName = minerProcess.MinerConfiguration.CoinName;
                    break;
                }
            }

            return coinName;
        }

        private void PopulateMobileMinerStatistics(MultiMiner.MobileMiner.Data.MiningStatistics miningStatistics, DeviceInformation deviceInformation,
            string coinName)
        {
            miningStatistics.MinerName = "MultiMiner";
            miningStatistics.CoinName = coinName;
            Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Name.Equals(coinName));
            PoolGroup coin = coinConfiguration.PoolGroup;

            //don't send non-coin Ids to MobileMiner
            if (coin.Kind != PoolGroup.PoolGroupKind.MultiCoin)
                miningStatistics.CoinSymbol = coin.Id;

            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(coin.Algorithm);

            //MobileMiner currently only supports SHA and Scrypt
            //attempt to treat them as "Families" for now
            if ((algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA2) ||
                (algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA3))
                //SHA family algorithms grouped together
                miningStatistics.Algorithm = AlgorithmFullNames.SHA256;
            else
                //assume Scrypt for rest until MobileMiner supports more
                miningStatistics.Algorithm = AlgorithmFullNames.Scrypt;

            miningStatistics.PopulateFrom(deviceInformation);
        }

        private Action<List<MultiMiner.MobileMiner.Data.MiningStatistics>> submitMiningStatisticsDelegate;

        private void SubmitMiningStatistics(List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList)
        {
            try
            {
                MobileMiner.ApiContext.SubmitMiningStatistics(GetMobileMinerUrl(), mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    statisticsList);
                mobileMinerSuccess = true;
            }
            catch (WebException ex)
            {
                //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        if (!mobileMinerSuccess)
                        {
                            applicationConfiguration.MobileMinerMonitoring = false;
                            applicationConfiguration.SaveApplicationConfiguration();
                            MessageBox.Show("Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog." +
                                Environment.NewLine + Environment.NewLine +
                                "MobileMiner remote monitoring will now be disabled.", "Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            //check to make sure there are no modal windows already
                            if (!ShowingModalDialog())
                            {
                                BeginInvoke((Action)(() =>
                                {
                                    //code to update UI
                                    ConfigureSettingsLocally();
                                }));
                            }
                        }
                    }
                    else if (applicationConfiguration.ShowApiErrors)
                    {
                        if (InvokeRequired)
                            BeginInvoke((Action)(() =>
                            {
                                //code to update UI
                                ShowMobileMinerApiErrorNotification(ex);
                            }));
                        else
                            ShowMobileMinerApiErrorNotification(ex);
                    }
                }
            }
        }

        private void QueueMobileMinerNotification(string text, MobileMiner.Data.NotificationKind kind)
        {
            MobileMiner.Data.Notification notification = new MobileMiner.Data.Notification() 
            { 
                NotificationText = text, 
                MachineName = Environment.MachineName, 
                NotificationKind = kind 
            };
            queuedNotifications.Add(notification);
        }

        private void SubmitMobileMinerNotifications()
        {
            //are remote notifications enabled?
            if (!applicationConfiguration.MobileMinerPushNotifications)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            //do we have notifications to push?
            if (queuedNotifications.Count == 0)
                return;

            if (submitNotificationsDelegate == null)
                submitNotificationsDelegate = SubmitNotifications;

            submitNotificationsDelegate.BeginInvoke(submitNotificationsDelegate.EndInvoke, null);
        }

        private Action submitNotificationsDelegate;

        private void SubmitNotifications()
        {
            try
            {
                MobileMiner.ApiContext.SubmitNotifications(GetMobileMinerUrl(), mobileMinerApiKey,
                        applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                        queuedNotifications);
                queuedNotifications.Clear();
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentException))
                {
                    //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                    //could also be a json parsing error
                    return;
                }
                throw;
            }
        }
        
        private void SubmitMobileMinerPools()
        {
            //are remote commands enabled?
            if (!applicationConfiguration.MobileMinerRemoteCommands)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            Dictionary<string, List<string>> machinePools = new Dictionary<string, List<string>>();
            List<string> pools = new List<string>();
            foreach (Coin coin in engineConfiguration.CoinConfigurations.Where(cc => cc.Enabled))
                pools.Add(coin.PoolGroup.Name);
            machinePools[Environment.MachineName] = pools;

            foreach (KeyValuePair<string, List<PoolInformation>> networkDevicePool in networkDevicePools)
            {
                //ipAddress:port
                string machinePath = networkDevicePool.Key;
                string machineName = localViewModel.GetFriendlyDeviceName(machinePath, machinePath);
                // poolInformationList may be null if an RPC API call timed out
                if (networkDevicePool.Value != null)
                {
                    machinePools[machineName] = networkDevicePool.Value
                        .Select(pi => pi.Url.DomainFromHost()).ToList();
                }
            }

            if (submitPoolsDelegate == null)
                submitPoolsDelegate = SubmitPools;

            submitPoolsDelegate.BeginInvoke(machinePools, submitPoolsDelegate.EndInvoke, null);
        }

        private Action<Dictionary<string, List<string>>> submitPoolsDelegate;

        private void SubmitPools(Dictionary<string, List<string>> machinePools)
        {
            try
            {
                MobileMiner.ApiContext.SubmitMachinePools(GetMobileMinerUrl(), mobileMinerApiKey,
                        applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                        machinePools);
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentException))
                {
                    //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                    //could also be a json parsing error
                    return;
                }
                throw;
            }
        }

        private void CheckForMobileMinerCommands()
        {
            //are remote commands enabled?
            if (!applicationConfiguration.MobileMinerRemoteCommands)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            if (checkForRemoteCommandsDelegate == null)
                checkForRemoteCommandsDelegate = GetRemoteCommands;

            checkForRemoteCommandsDelegate.BeginInvoke(checkForRemoteCommandsDelegate.EndInvoke, null);
        }

        private Action checkForRemoteCommandsDelegate;

        private void GetRemoteCommands()
        {
            List<MobileMiner.Data.RemoteCommand> commands = new List<MobileMiner.Data.RemoteCommand>();

            List<string> machineNames = new List<string>();

            if (!applicationConfiguration.MobileMinerNetworkMonitorOnly)
                machineNames.Add(Environment.MachineName);

            IEnumerable<DeviceViewModel> networkDevices = localViewModel.Devices.Where(d => d.Kind == DeviceKind.NET);
            foreach (DeviceViewModel deviceViewModel in networkDevices)
            {
                string machineName = localViewModel.GetFriendlyDeviceName(deviceViewModel.Path, deviceViewModel.Path);
                machineNames.Add(machineName);
            }

            if (machineNames.Count == 0)
                return;

            try
            {
                commands = MobileMiner.ApiContext.GetCommands(GetMobileMinerUrl(), mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    machineNames);
                mobileMinerSuccess = true;
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentException))
                {
                    //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                    //could also be a json parsing error
                    if (ex is WebException)
                    {
                        WebException webException = (WebException)ex;

                        HttpWebResponse response = webException.Response as HttpWebResponse;
                        if (response != null)
                        {
                            if (response.StatusCode == HttpStatusCode.Forbidden)
                            {
                                if (!mobileMinerSuccess)
                                {
                                    this.applicationConfiguration.MobileMinerRemoteCommands = false;
                                    this.applicationConfiguration.SaveApplicationConfiguration();
                                    MessageBox.Show("Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog." +
                                        Environment.NewLine + Environment.NewLine +
                                        "MobileMiner remote commands will now be disabled.", "Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    //check to make sure there are no modal windows already
                                    if (!ShowingModalDialog())
                                    {
                                        BeginInvoke((Action)(() =>
                                        {
                                            //code to update UI
                                            ConfigureSettingsLocally();
                                        }));
                                    }
                                }
                            }
                            else if (applicationConfiguration.ShowApiErrors)
                            {
                                BeginInvoke((Action)(() =>
                                {
                                    //code to update UI
                                    ShowMobileMinerApiErrorNotification(webException);
                                }));
                            }
                        }
                    }

                    return;
                }
                throw;
            }

            if (InvokeRequired)
                BeginInvoke((Action<List<MobileMiner.Data.RemoteCommand>>)((c) => ProcessRemoteCommands(c)), commands);
            else
                ProcessRemoteCommands(commands);
        }

        private void ProcessRemoteCommands(List<MobileMiner.Data.RemoteCommand> commands)
        {
            if (commands.Count > 0)
            {
                MobileMiner.Data.RemoteCommand command = commands.First();

                //check this before actually executing the command
                //point being, say for some reason it takes 2 minutes to restart mining
                //if we check for commands again in that time, we don't want to process it again
                if (processedCommandIds.Contains(command.Id))
                    return;

                processedCommandIds.Add(command.Id);

                if (deleteRemoteCommandDelegate == null)
                    deleteRemoteCommandDelegate = DeleteRemoteCommand;

                if (command.Machine.Name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                {
                    if (command.CommandText.Equals("START", StringComparison.OrdinalIgnoreCase))
                    {
                        SaveChangesLocally(); //necessary to ensure device configurations exist for devices
                        StartMiningLocally();
                    }
                    else if (command.CommandText.Equals("STOP", StringComparison.OrdinalIgnoreCase))
                        StopMiningLocally();
                    else if (command.CommandText.Equals("RESTART", StringComparison.OrdinalIgnoreCase))
                    {
                        StopMiningLocally();
                        SaveChangesLocally(); //necessary to ensure device configurations exist for devices
                        StartMiningLocally();
                    }
                    else if (command.CommandText.StartsWith("SWITCH", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] parts = command.CommandText.Split('|');
                        string coinName = parts[1];
                        Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Name.Equals(coinName));
                        if (coinConfiguration != null)
                            SetAllDevicesToCoinLocally(coinConfiguration.PoolGroup.Id, true);
                    }

                    deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
                }
                else
                {
                    string remoteMachineName = command.Machine.Name;
                    DeviceViewModel networkDevice = GetNetworkDeviceByFriendlyName(remoteMachineName);
                    if (networkDevice != null)
                    {
                        Uri uri = new Uri("http://" + networkDevice.Path);
                        Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

                        //setup logging
                        apiContext.LogEvent -= LogApiEvent;
                        apiContext.LogEvent += LogApiEvent;

                        if (command.CommandText.StartsWith("SWITCH", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] parts = command.CommandText.Split('|');
                            string poolName = parts[1];

                            //we may not have the pool info cached yet / anymore
                            if (networkDevicePools.ContainsKey(networkDevice.Path))
                            {
                                List<PoolInformation> pools = networkDevicePools[networkDevice.Path];
                                int poolIndex = pools.FindIndex(pi => pi.Url.DomainFromHost().Equals(poolName, StringComparison.OrdinalIgnoreCase));

                                apiContext.SwitchPool(poolIndex);
                            }
                        }
                        else if (command.CommandText.Equals("RESTART", StringComparison.OrdinalIgnoreCase))
                        {
                            apiContext.RestartMining();
                        }

                        deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
                    }
                }
            }
        }

        private DeviceViewModel GetNetworkDeviceByFriendlyName(string friendlyDeviceName)
        {
            DeviceViewModel result = null;

            IEnumerable<DeviceViewModel> networkDevices = localViewModel.Devices.Where(d => d.Kind == DeviceKind.NET);
            foreach (DeviceViewModel item in networkDevices)
            {
                if (localViewModel.GetFriendlyDeviceName(item.Path, item.Path).Equals(friendlyDeviceName, StringComparison.OrdinalIgnoreCase))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        private Action<MobileMiner.Data.RemoteCommand> deleteRemoteCommandDelegate;

        private void DeleteRemoteCommand(MobileMiner.Data.RemoteCommand command)
        {
            try
            {
                MobileMiner.ApiContext.DeleteCommand(GetMobileMinerUrl(), mobileMinerApiKey,
                                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                                    command.Machine.Name, command.Id);
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentException))
                {
                    //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                    //could also be a json parsing error
                    return;
                }
                throw;
            }
        }

        private void ShowMobileMinerApiErrorNotification(WebException ex)
        {
            PostNotification(ex.Message,
                String.Format("Error accessing the MobileMiner API ({0})", (int)((HttpWebResponse)ex.Response).StatusCode), () =>
                {
                    Process.Start("http://mobileminerapp.com");
                },
                ToolTipIcon.Warning, "");
        }
        #endregion

        #region Stats API
        private void SubmitMultiMinerStatistics()
        {
            string installedVersion = Engine.Installers.MultiMinerInstaller.GetInstalledMinerVersion();
            if (installedVersion.Equals(applicationConfiguration.SubmittedStatsVersion))
                return;

            Stats.Data.Machine machineStat = new Stats.Data.Machine()
            {
                Name = Environment.MachineName,
                MinerVersion = installedVersion
            };

            if (submitMinerStatisticsDelegate == null)
                submitMinerStatisticsDelegate = SubmitMinerStatistics;

            submitMinerStatisticsDelegate.BeginInvoke(machineStat, submitMinerStatisticsDelegate.EndInvoke, null);
        }
        private Action<Stats.Data.Machine> submitMinerStatisticsDelegate;

        private void SubmitMinerStatistics(Stats.Data.Machine machineStat)
        {
            try
            {
                //plain text so users can see what we are posting - transparency
                Stats.ApiContext.SubmitMinerStatistics("http://multiminerstats.azurewebsites.net/api/", machineStat);
                applicationConfiguration.SubmittedStatsVersion = machineStat.MinerVersion;
            }
            catch (WebException)
            {
                //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
            }
        }
        #endregion

        #region RPC API

        private static double AdjustWorkUtilityForPoolMultipliers(double workUtility, string algorithmName)
        {
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            if (algorithm.Family == CoinAlgorithm.AlgorithmFamily.Scrypt)
            {
                const int DumbScryptMultiplier = 65536;
                return workUtility / DumbScryptMultiplier;
            }

            if (algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA3)
            {
                const int DumbSHA3Multiplier = 256;
                return workUtility / DumbSHA3Multiplier;
            }

            return workUtility;
        }

        private void RefreshDeviceStats()
        {
            allDeviceInformation.Clear();

            //first clear stats for each row
            //this is because the PXY row stats get summed 
            localViewModel.ClearDeviceInformationFromViewModel();

            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<MinerProcess> minerProcesses = miningEngine.MinerProcesses.ToList();
            foreach (MinerProcess minerProcess in minerProcesses)
            {
                ClearSuspectProcessFlags(minerProcess);

                List<DeviceInformation> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);
                if (deviceInformationList == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                allDeviceInformation.AddRange(deviceInformationList);

                //starting with bfgminer 3.7 we need the DEVDETAILS response to tie things from DEVS up with -d? details
                List<DeviceDetails> processDevices = GetProcessDeviceDetails(minerProcess, deviceInformationList);
                if (processDevices == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                //clear accepted shares as we'll be summing that as well
                minerProcess.AcceptedShares = 0;

                string coinSymbol = null;

                foreach (DeviceInformation deviceInformation in deviceInformationList)
                {
                    //don't consider a standalone miner suspect - restarting the proxy doesn't help and often hurts
                    if (!deviceInformation.Name.Equals("PXY", StringComparison.OrdinalIgnoreCase))
                        FlagSuspiciousMiner(minerProcess, deviceInformation);

                    DeviceDetails deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
                        && (d.ID == deviceInformation.ID));
                    int deviceIndex = GetDeviceIndexForDeviceDetails(deviceDetails, minerProcess);

                    if (deviceIndex >= 0)
                    {
                        minerProcess.AcceptedShares += deviceInformation.AcceptedShares;

                        Xgminer.Data.Device device = devices[deviceIndex];
                        Engine.Data.Configuration.Coin coinConfiguration = CoinConfigurationForDevice(device);

                        if (minerProcess.Miner.LegacyApi && (coinConfiguration != null))
                            deviceInformation.WorkUtility = AdjustWorkUtilityForPoolMultipliers(deviceInformation.WorkUtility, coinConfiguration.PoolGroup.Algorithm);

                        DeviceViewModel deviceViewModel = localViewModel.ApplyDeviceInformationResponseModel(device, deviceInformation);
                        deviceDetailsMapping[deviceViewModel] = deviceDetails;

                        if (coinConfiguration != null)
                        {
                            coinSymbol = coinConfiguration.PoolGroup.Id;

                            string poolName = GetPoolNameByIndex(coinConfiguration, deviceViewModel.PoolIndex);
                            //may be blank if donating
                            if (!String.IsNullOrEmpty(poolName))
                            {
                                deviceViewModel.Pool = poolName;
                                lastDevicePoolMapping[deviceViewModel] = poolName;
                            }
                            else
                            {
                                if (lastDevicePoolMapping.ContainsKey(deviceViewModel))
                                    deviceViewModel.Pool = lastDevicePoolMapping[deviceViewModel];
                            }

                            if (!String.IsNullOrEmpty(deviceViewModel.Pool))
                                CheckAndSetNetworkDifficulty(minerProcess.ApiContext.IpAddress, minerProcess.ApiContext.Port, deviceViewModel.Pool);
                        }
                    }
                }

                FlagSuspiciousProxy(minerProcess, deviceInformationList);

                localViewModel.ApplyDeviceDetailsResponseModels(minerProcess.MinerConfiguration.DeviceDescriptors, processDevices);
            }

            RefreshViewFromDeviceStats();
        }

        //cache based on pool URI rather than coin symbol
        //the coin symbol may be guessed wrong for Network Devices
        //this can (and has) resulting in wildly inaccurate income estimates
        private double GetCachedNetworkDifficulty(string poolUri)
        {
            double result = 0.0;
            if (minerNetworkDifficulty.ContainsKey(poolUri))
                result = minerNetworkDifficulty[poolUri];
            return result;
        }

        private void ClearCachedNetworkDifficulties()
        {
            minerNetworkDifficulty.Clear();
        }

        private void SetCachedNetworkDifficulty(string poolUri, double difficulty)
        {
            minerNetworkDifficulty[poolUri] = difficulty;
        }

        private void CheckAndSetNetworkDifficulty(string ipAddress, int port, string poolUri)
        {
            double difficulty = GetCachedNetworkDifficulty(poolUri);
            if (difficulty == 0.0)
            {
                MultiMiner.Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(port, ipAddress);
                difficulty = GetNetworkDifficultyFromMiner(apiContext);
                SetCachedNetworkDifficulty(poolUri, difficulty);
            }
        }

        private double GetNetworkDifficultyFromMiner(Xgminer.Api.ApiContext apiContext)
        {            
            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            NetworkCoinInformation coinInformation = null;

            try
            {
                try
                {
                    coinInformation = apiContext.GetCoinInformation();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    coinInformation = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                coinInformation = null;
            }

            return coinInformation == null ? 0.0 : coinInformation.NetworkDifficulty;
        }

        private void RefreshViewFromDeviceStats()
        {
            RefreshListViewFromViewModel();
            RefreshStatusBarFromViewModel();

            //max length for notify icon text is 63
            string bubbleText = string.Format("MultiMiner - {0}", hashRateStatusLabel.Text);
            //must be less than (not equal to) 64 characters
            if (bubbleText.Length >= 64)
                bubbleText = bubbleText.Substring(0, 60) + "...";
            notifyIcon1.Text = bubbleText;

            //auto sizing the columns is moderately CPU intensive, so only do it every /count/ times
            AutoSizeListViewColumnsEvery(2);

            UpdateInstancesStatsFromLocal();

            RefreshIncomeSummary();
            RefreshDetailsAreaIfVisible();
        }

        private void RemoveSelfReferencingNetworkDevices()
        {
            List<string> localIpAddresses = Utility.Net.LocalNetwork.GetLocalIPAddresses();
            IEnumerable<DeviceViewModel> networkDevices = localViewModel.Devices.Where(d => d.Kind == DeviceKind.NET).ToList();
            foreach (DeviceViewModel networkDevice in networkDevices)
            {
                if (localIpAddresses.Contains(networkDevice.Pool.DomainFromHost()))
                    //actually remove rather than setting Visible = false
                    //Visible = false items still get fetched with RefreshNetworkDeviceStats()
                    localViewModel.Devices.Remove(networkDevice);
            }
        }

        private List<PoolInformation> GetCachedPoolInfoFromAddress(string ipAddress, int port)
        {
            List<PoolInformation> poolInformationList = null;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (this.networkDevicePools.ContainsKey(key))
                poolInformationList = this.networkDevicePools[key];
            else
            {
                poolInformationList = GetPoolInfoFromAddress(ipAddress, port);
                networkDevicePools[key] = poolInformationList;
            }
            return poolInformationList;
        }

        private VersionInformation GetCachedMinerVersionFromAddress(string ipAddress, int port)
        {
            VersionInformation versionInfo = null;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (this.networkDeviceVersions.ContainsKey(key))
                versionInfo = this.networkDeviceVersions[key];
            else
            {
                versionInfo = GetVersionInfoFromAddress(ipAddress, port);
                networkDeviceVersions[key] = versionInfo;
            }
            return versionInfo;
        }

        private List<MinerStatistics> GetCachedMinerStatisticsFromViewModel(DeviceViewModel deviceViewModel)
        {
            string[] portions = deviceViewModel.Path.Split(':');
            string ipAddress = portions[0];
            int port = int.Parse(portions[1]);
            return GetCachedMinerStatisticsFromAddress(ipAddress, port);
        }

        private List<MinerStatistics> GetCachedMinerStatisticsFromAddress(string ipAddress, int port)
        {
            List<MinerStatistics> minerStatisticsList = null;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (this.networkDeviceStatistics.ContainsKey(key))
                minerStatisticsList = this.networkDeviceStatistics[key];
            else
            {
                minerStatisticsList = GetMinerStatisticsFromAddress(ipAddress, port);
                networkDeviceStatistics[key] = minerStatisticsList;
            }
            return minerStatisticsList;
        }

        private void RefreshNetworkDeviceStatsAsync()
        {
            Action asyncAction = RefreshNetworkDeviceStats;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);
                    BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        ApplyCoinInformationToViewModel();

                        RemoveSelfReferencingNetworkDevices();

                        RefreshViewFromDeviceStats();
                    }));

                }, null);
        }

        private void RefreshNetworkDeviceStats()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            IEnumerable<DeviceViewModel> networkDevices = localViewModel.Devices.Where(d => d.Kind == DeviceKind.NET).ToList();

            foreach (DeviceViewModel deviceViewModel in networkDevices)
            {
                //is the app closing?
                //otherwise a check for formHandleValid is needed before the call to this.BeginInvoke below
                //this makes more sense though and has other benefits
                if (tearingDown)
                    return;

                string[] portions = deviceViewModel.Path.Split(':');
                string ipAddress = portions[0];
                int port = int.Parse(portions[1]);

                List<DeviceInformation> deviceInformationList = GetDeviceInfoFromAddress(ipAddress, port);

                //first clear stats for each row
                //this is because the NET row stats get summed 
                this.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    MinerFormViewModel.ClearDeviceInformation(deviceViewModel);

                    //deviceInformationList or poolInformationList may be down if the API was unreachable
                    //at the time
                    if (deviceInformationList != null)
                    {
                        List<MinerStatistics> minerStatistics = GetCachedMinerStatisticsFromAddress(ipAddress, port);
                        //null if API call fails
                        //check for minerStatistics.Count > 0 needed (error reported with callstack)
                        if ((minerStatistics != null) && (minerStatistics.Count > 0))
                        {
                            MinerStatistics firstStatistics = minerStatistics.First();
                            deviceViewModel.Frequency = firstStatistics.Frequency;
                            deviceViewModel.ChainStatus = firstStatistics.ChainStatus;
                        }

                        int poolIndex = -1;
                        foreach (DeviceInformation deviceInformation in deviceInformationList)
                        {
                            localViewModel.ApplyDeviceInformationResponseModel(deviceViewModel, deviceInformation);
                            poolIndex = deviceInformation.PoolIndex >= 0 ? deviceInformation.PoolIndex : poolIndex;
                        }

                        List<PoolInformation> poolInformationList = GetCachedPoolInfoFromAddress(ipAddress, port);
                        if ((poolInformationList != null) &&
                            //ensure poolIndex is valid for poolInformationList
                            //user(s) reported index errors so we can't count on the RPC API here
                            //https://github.com/nwoolls/MultiMiner/issues/64
                            ((poolIndex >= 0) && (poolIndex < poolInformationList.Count)))
                        {
                            PoolInformation poolInformation = poolInformationList[poolIndex];

                            deviceViewModel.Pool = poolInformation.Url;

                            deviceViewModel.LastShareDifficulty = poolInformation.LastShareDifficulty;
                            deviceViewModel.LastShareTime = poolInformation.LastShareTime;
                            deviceViewModel.Url = poolInformation.Url;
                            deviceViewModel.BestShare = poolInformation.BestShare;
                            deviceViewModel.PoolStalePercent = poolInformation.PoolStalePercent;

                            deviceViewModel.Visible = true;

                            Coin coinConfiguration = CoinConfigurationForPoolUrl(poolInformation.Url);
                            if (coinConfiguration != null)
                                deviceViewModel.Coin = coinConfiguration.PoolGroup;
                        }
                        
                        VersionInformation versionInfo = GetCachedMinerVersionFromAddress(ipAddress, port);
                        //null if API call fails
                        if (versionInfo != null)
                        {
                            //fix work utility for Network Devices using CGMiner-forks that mine alt coins
                            if (!versionInfo.Name.Equals(MinerNames.BFGMiner, StringComparison.OrdinalIgnoreCase))
                                deviceViewModel.WorkUtility = AdjustWorkUtilityForPoolMultipliers(deviceViewModel.WorkUtility, deviceViewModel.Coin.Algorithm);
                        }
                    }

                    if (!String.IsNullOrEmpty(deviceViewModel.Pool))
                        CheckAndSetNetworkDifficulty(ipAddress, port, deviceViewModel.Pool);
                }));
            }         
        }

        private Coin CoinConfigurationForPoolUrl(string poolUrl)
        {
            Coin coinConfiguration =
                engineConfiguration.CoinConfigurations
                    .FirstOrDefault(cc =>
                        cc.Pools
                            .Any(p => String.Format("{0}:{1}", p.Host.ShortHostFromHost(), p.Port).Equals(poolUrl.ShortHostFromHost(), StringComparison.OrdinalIgnoreCase))
                    );

            return coinConfiguration;
        }

        private void UpdateInstancesStatsFromLocal()
        {
            if (instancesControl.Visible)
            {
                Remoting.Data.Transfer.Machine machine = new Remoting.Data.Transfer.Machine();
                PopulateLocalMachineHashrates(machine, true);

                instancesControl.ApplyMachineInformation("localhost", machine);
                UpdateInstancesVisibility();
            }
        }

        private int GetDeviceIndexForDeviceDetails(DeviceDetails deviceDetails, MinerProcess minerProcess)
        {
            int result = devices
                .FindIndex((device) => {
                    return device.Driver.Equals(deviceDetails.Driver, StringComparison.OrdinalIgnoreCase)
                    &&
                    (
                        //serial == serial && path == path (serial may not be unique)
                        (!String.IsNullOrEmpty(device.Serial) && device.Serial.Equals(deviceDetails.Serial, StringComparison.OrdinalIgnoreCase)
                            && !String.IsNullOrEmpty(device.Path) && device.Path.Equals(deviceDetails.DevicePath, StringComparison.OrdinalIgnoreCase))

                        //serial == serial && path == String.Empty - WinUSB/LibUSB has no path, but has a serial #
                        || (!String.IsNullOrEmpty(device.Serial) && device.Serial.Equals(deviceDetails.Serial, StringComparison.OrdinalIgnoreCase)
                            && String.IsNullOrEmpty(device.Path) && String.IsNullOrEmpty(deviceDetails.DevicePath))

                        //path == path
                        || (!String.IsNullOrEmpty(device.Path) && device.Path.Equals(deviceDetails.DevicePath, StringComparison.OrdinalIgnoreCase))

                        //proxy == proxy && ID = RelativeIndex
                        || (device.Driver.Equals("proxy", StringComparison.OrdinalIgnoreCase) && (minerProcess.MinerConfiguration.DeviceDescriptors.Contains(device)))

                        //opencl = opencl && ID = RelativeIndex
                        || (device.Driver.Equals("opencl", StringComparison.OrdinalIgnoreCase) && (device.RelativeIndex == deviceDetails.ID))

                        //cpu = cpu && ID = RelativeIndex
                        || (device.Driver.Equals("cpu", StringComparison.OrdinalIgnoreCase) && (device.RelativeIndex == deviceDetails.ID))
                    );
            
                });

            return result;
        }

        private void FlagSuspiciousMiner(MinerProcess minerProcess, DeviceInformation deviceInformation)
        {
            if (deviceInformation.Status.ToLower().Contains("sick"))
                minerProcess.HasSickDevice = true;
            if (deviceInformation.Status.ToLower().Contains("dead"))
                minerProcess.HasDeadDevice = true;
            if (deviceInformation.CurrentHashrate == 0)
                minerProcess.HasZeroHashrateDevice = true;

            //only check GPUs for subpar hashrate
            //ASICs spike too much for this to be reliable there
            //don't check average hashrate if using dynamic intensity
            if (deviceInformation.Kind.Equals("GPU", StringComparison.OrdinalIgnoreCase) &&
                !engineConfiguration.XgminerConfiguration.DesktopMode)
            {
                //avoid div by 0
                if (deviceInformation.AverageHashrate > 0)
                {
                    double performanceRatio = deviceInformation.CurrentHashrate / deviceInformation.AverageHashrate;
                    if (performanceRatio <= 0.50)
                        minerProcess.HasPoorPerformingDevice = true;
                }
            }

            if (miningCoinConfigurations == null)
                //started mining but haven't yet assigned mining members
                //cannot check the following yet
                return;
                        
            //Work Utility not returned by legacy API miners
            if (!minerProcess.Miner.LegacyApi)
            {
                double effectiveHashrate = WorkUtilityToHashrate(deviceInformation.WorkUtility);
                //avoid div by 0
                if (deviceInformation.AverageHashrate > 0)
                {
                    double performanceRatio = effectiveHashrate / deviceInformation.AverageHashrate;
                    if (performanceRatio <= 0.25)
                        minerProcess.StoppedAcceptingShares = true;
                }
            }
        }

        private static void FlagSuspiciousProxy(MinerProcess minerProcess, List<DeviceInformation> deviceInformationList)
        {
            double currentProxyHashrate = deviceInformationList
                                .Where(device => device.Name.Equals("PXY", StringComparison.OrdinalIgnoreCase))
                                .Sum(device => device.CurrentHashrate);

            double averageProxyHashrate = deviceInformationList
                .Where(device => device.Name.Equals("PXY", StringComparison.OrdinalIgnoreCase))
                .Sum(device => device.AverageHashrate);

            //proxy is 0 hashrate and used to have a positive hashrate
            if ((averageProxyHashrate > 0) && (currentProxyHashrate == 0))
            {
                minerProcess.HasZeroHashrateDevice = true;
            }
        }

        private static void ClearSuspectProcessFlags(MinerProcess minerProcess)
        {
            minerProcess.HasDeadDevice = false;
            minerProcess.HasSickDevice = false;
            minerProcess.HasZeroHashrateDevice = false;
            minerProcess.MinerIsFrozen = false;
            minerProcess.HasPoorPerformingDevice = false;
            minerProcess.StoppedAcceptingShares = false;
        }

        private void RefreshPoolInfo()
        {
            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                List<PoolInformation> poolInformation = GetPoolInfoFromProcess(minerProcess);

                if (poolInformation == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                localViewModel.ApplyPoolInformationResponseModels(minerProcess.CoinSymbol, poolInformation);
            }
            RefreshDetailsAreaIfVisible();
        }

        private List<PoolInformation> GetPoolInfoFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<PoolInformation> poolInformation = null;
            try
            {
                try
                {
                    poolInformation = apiContext.GetPoolInformation();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    poolInformation = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                poolInformation = null;
            }

            return poolInformation;
        }

        private List<PoolInformation> GetPoolInfoFromAddress(string ipAddress, int port)
        {
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<PoolInformation> poolInformation = null;
            try
            {
                try
                {
                    poolInformation = apiContext.GetPoolInformation();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    poolInformation = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                poolInformation = null;
            }

            return poolInformation;
        }

        private List<MinerStatistics> GetMinerStatisticsFromAddress(string ipAddress, int port)
        {
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<MinerStatistics> minerStatistics = null;
            try
            {
                try
                {
                    minerStatistics = apiContext.GetMinerStatistics();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    minerStatistics = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                minerStatistics = null;
            }

            return minerStatistics;
        }

        private void PopulateSummaryInfoFromProcesses()
        {
            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                SummaryInformation summaryInformation = GetSummaryInfoFromProcess(minerProcess);

                if (summaryInformation == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                CheckAndNotifyFoundBlocks(minerProcess, summaryInformation.FoundBlocks);

                CheckAndNotifyAcceptedShares(minerProcess, summaryInformation.AcceptedShares);
            }
        }

        private List<DeviceDetails> GetProcessDeviceDetails(MinerProcess minerProcess, List<DeviceInformation> deviceInformationList)
        {
            List<DeviceDetails> processDevices = null;
            if (processDeviceDetails.ContainsKey(minerProcess))
            {
                processDevices = processDeviceDetails[minerProcess];

                foreach (DeviceInformation deviceInformation in deviceInformationList)
                {
                    DeviceDetails deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
                        && (d.ID == deviceInformation.ID));
                    if (deviceDetails == null)
                    {
                        //devs API returned a device not in the previous DEVDETAILS response
                        //need to clear our previous response and get a new one
                        processDevices = null;
                        break;
                    }
                }
            }

            if (processDevices == null)
            {
                processDevices = GetDeviceDetailsFromProcess(minerProcess);

                //null returned if there is an RCP API error
                if (processDevices != null)
                    processDeviceDetails[minerProcess] = processDevices;
            }
            return processDevices;
        }

        private List<DeviceInformation> GetDeviceInfoFromAddress(string ipAddress, int port)
        {
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceInformation> deviceInformationList = null;
            try
            {
                try
                {
                    //assume Network Devices, for now, run cgminer or older bfgminer with default --log of 5s
                    const int NetworkDeviceLogInterval = 5;
                    //some Network Devices don't have the horsepower to return API results immediately
                    const int CommandTimeoutMs = 3000;
                    deviceInformationList = apiContext.GetDeviceInformation(NetworkDeviceLogInterval, CommandTimeoutMs).Where(d => d.Enabled).ToList();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    deviceInformationList = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                deviceInformationList = null;
            }

            return deviceInformationList;
        }

        private List<DeviceInformation> GetDeviceInfoFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceInformation> deviceInformationList = null;
            try
            {
                try
                {
                    deviceInformationList = apiContext.GetDeviceInformation(minerProcess.MinerConfiguration.LogInterval).Where(d => d.Enabled).ToList();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    deviceInformationList = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                deviceInformationList = null;
            }

            return deviceInformationList;
        }

        private List<DeviceDetails> GetDeviceDetailsFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceDetails> deviceDetailsList = null;
            try
            {
                try
                {
                    deviceDetailsList = apiContext.GetDeviceDetails().ToList();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    deviceDetailsList = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                deviceDetailsList = null;
            }

            return deviceDetailsList;
        }

        private SummaryInformation GetSummaryInfoFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            SummaryInformation summaryInformation = null;
            try
            {
                try
                {
                    summaryInformation = apiContext.GetSummaryInformation();
                }
                catch (IOException)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    summaryInformation = null;
                }
            }
            catch (SocketException)
            {
                //won't be able to connect for the first 5s or so
                summaryInformation = null;
            }

            return summaryInformation;
        }
        #endregion

        #region Logging logic
        private void LogProcessLaunch(object sender, LogLaunchArgs ea)
        {
            logLaunchArgsBindingSource.Position = logLaunchArgsBindingSource.Add(ea);

            while (logLaunchArgsBindingSource.Count > 1000)
                logLaunchArgsBindingSource.RemoveAt(0);

            LogProcessLaunchToFile(ea);
        }

        private void LogProcessLaunchToFile(LogLaunchArgs ea)
        {
            const string logFileName = "ProcessLog.json";
            LogObjectToFile(ea, logFileName);
        }

        private void LogObjectToFile(object objectToLog, string logFileName)
        {
            string logDirectory = GetLogDirectory();
            string logFilePath = Path.Combine(logDirectory, logFileName);
            ObjectLogger logger = new ObjectLogger(applicationConfiguration.RollOverLogFiles, applicationConfiguration.OldLogFileSets);
            logger.LogObjectToFile(objectToLog, logFilePath);
        }

        private string GetLogDirectory()
        {
            string logDirectory = ApplicationPaths.AppDataPath();
            if (!String.IsNullOrEmpty(applicationConfiguration.LogFilePath))
            {
                Directory.CreateDirectory(applicationConfiguration.LogFilePath);
                if (Directory.Exists(applicationConfiguration.LogFilePath))
                    logDirectory = applicationConfiguration.LogFilePath;
            }
            return logDirectory;
        }

        private void LogProcessClose(object sender, LogProcessCloseArgs ea)
        {
            logProcessCloseArgsBindingSource.Position = logProcessCloseArgsBindingSource.Add(ea);

            while (logProcessCloseArgsBindingSource.Count > MaxHistoryOnScreen)
                logProcessCloseArgsBindingSource.RemoveAt(0);

            LogProcessCloseToFile(ea);
        }
        
        private void LogProcessCloseToFile(LogProcessCloseArgs ea)
        {
            const string logFileName = "MiningLog.json";
            //log an anonymous type so MinerConfiguration is ommitted
            LogObjectToFile(
                new
                {
                    StartDate = ea.StartDate,
                    EndDate = ea.EndDate,
                    CoinName = ea.CoinName,
                    CoinSymbol = ea.CoinSymbol,
                    StartPrice = ea.StartPrice,
                    EndPrice = ea.EndPrice,
                    AcceptedShares = ea.AcceptedShares,
                    DeviceDescriptors = ea.DeviceDescriptors
                }, logFileName);
        }

        private void LogNotificationToFile(string text)
        {
            const string logFileName = "NotificationLog.json";
            LogObjectToFile(new
            {
                DateTime = DateTime.Now,
                Notification = text
            }, logFileName);
        }

        private void LogApiEventToFile(ApiLogEntry logEntry)
        {
            const string logFileName = "ApiLog.json";
            LogObjectToFile(logEntry, logFileName);
        }
        #endregion

        #region Mining event handlers
        private void ProcessLaunchFailed(object sender, LaunchFailedArgs ea)
        {
            this.BeginInvoke((Action)(() =>
            {
                if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                {
                    string notificationReason = String.Empty;

                    int enabledConfigurationCount = engineConfiguration.CoinConfigurations.Count(c => c.Enabled);

                    //only disable the configuration if there are others enabled - otherwise left idling
                    if (enabledConfigurationCount > 1)
                    {

                        //if auto mining is enabled, flag pools down in the coin configuration and display a notification
                        Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(config => config.PoolGroup.Name.Equals(ea.CoinName, StringComparison.OrdinalIgnoreCase));
                        coinConfiguration.PoolsDown = true;
                        engineConfiguration.SaveCoinConfigurations();

                        //if no enabled configurations, stop mining
                        int enabledConfigurations = engineConfiguration.CoinConfigurations.Count(config => config.Enabled && !config.PoolsDown);
                        if (enabledConfigurations == 0)
                            StopMiningLocally();
                        else
                            //if there are enabled configurations, apply mining strategy
                            CheckAndApplyMiningStrategy();

                        notificationReason = String.Format("Configuration for {0} disabled - all pools down", ea.CoinName);
                    }
                    else
                    {
                        //otherwise just notify - relaunching option will take care of the rest
                        notificationReason = String.Format("All pools for {0} configuration are down", ea.CoinName);
                    }

                    PostNotification(notificationReason, notificationReason, () =>
                    {
                        ConfigureCoinsLocally();
                    }, ToolTipIcon.Error, "");
                }
                else
                {
                    if (!applicationConfiguration.RestartCrashedMiners)
                    {
                        //if we are not restarting miners, display a dialog
                        MessageBox.Show(ea.Reason, "Launching Miner Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //just notify - relaunching option will take care of the rest
                        string notificationReason = String.Format("All pools for {0} configuration are down", ea.CoinName);
                        PostNotification(notificationReason, notificationReason, () =>
                        {
                            ConfigureCoinsLocally();
                        }, ToolTipIcon.Error, "");
                    }
                }
            }));
        }

        private void ProcessAuthenticationFailed(object sender, AuthenticationFailedArgs ea)
        {
            this.BeginInvoke((Action)(() =>
            {
                //code to update UI
                PostNotification(ea.Reason, ea.Reason, () =>
                {
                    ConfigureCoinsLocally();
                }, ToolTipIcon.Error, "");
            }));
        }

        private void LogApiEvent(object sender, Xgminer.Api.LogEventArgs eventArgs)
        {
            ApiLogEntry logEntry = new ApiLogEntry();

            logEntry.DateTime = eventArgs.DateTime;
            logEntry.Request = eventArgs.Request;
            logEntry.Response = eventArgs.Response;
            Xgminer.Api.ApiContext apiContext = (Xgminer.Api.ApiContext)sender;
            logEntry.CoinName = GetCoinNameForApiContext(apiContext);
            logEntry.Machine = apiContext.IpAddress + ":" + apiContext.Port;

            //make sure BeginInvoke is allowed
            if (formHandleValid)
            {
                this.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    apiLogEntryBindingSource.Position = apiLogEntryBindingSource.Add(logEntry);
                    while (apiLogEntryBindingSource.Count > 1000)
                        apiLogEntryBindingSource.RemoveAt(0);
                }));
            }

            LogApiEventToFile(logEntry);
        }
        #endregion

        #region Application startup / setup
        private bool formHandleValid = false;
        protected override void OnHandleCreated(EventArgs e)
        {
            formHandleValid = true;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            formHandleValid = false;
            base.OnHandleDestroyed(e);
        }

        private void SetupApplication()
        {
            HandleStartupMinimizedToNotificationArea();

            accessibleMenu.Visible = false;

            SetupLookAndFeel();

            //make it easier for users to understand there are selected items
            //trying to make the context menu discoverable
            deviceListView.HideSelection = false;

            incomeSummaryLabel.Text = String.Empty;

            SetupInitialButtonVisibility();

            SetupInstancesControl();

            SetupGridColumns();

            logProcessCloseArgsBindingSource.DataSource = logCloseEntries;
            LoadPreviousHistory();
            
            CloseDetailsArea();

            FetchInitialCoinStats();

            UpdateBackendMinerAvailability(); //before CheckAndShowGettingStarted()
            CheckAndShowGettingStarted();

            LoadSettings();

            RefreshDetailsToggleButton();

            RefreshCoinApiLabel();

            RefreshCoinPopupMenu();

            PositionCoinChooseLabels();

            apiLogEntryBindingSource.DataSource = apiLogEntries;

            SetupMiningEngineEvents();
            logLaunchArgsBindingSource.DataSource = logLaunchEntries;

            SetHasChangesLocally(false);

            //kill known owned processes to release inherited socket handles
            if (KillOwnedProcesses())
                //otherwise may still be prompted below by check for disowned miners
                Thread.Sleep(500);

            //check for disowned miners before refreshing devices
            if (applicationConfiguration.DetectDisownedMiners)
                CheckForDisownedMiners();

            SetupRemoting();

            SetupStatusBarLabelLayouts();

            CheckAndDownloadMiners();

            CheckForUpdates();

            SetHasChangesLocally(false);

            LoadKnownDevicesFromFile();
            //ONLY if null, e.g. first launch or no XML
            //don't keep scanning on startup if there are no devices
            //maybe just using to monitor network devices
            if (devices == null)
                ScanHardwareLocally();

            //after refreshing devices
            SubmitMultiMinerStatistics();

            //scan for Network Devices after scanning for local hardware
            //makes more sense visually
            SetupNetworkDeviceDetection();
            
            //may need to do this if XML files became corrupt
            AddMissingDeviceConfigurations();

            UpdateMiningButtons();

            AutoSizeListViewColumns();
            
            if (deviceListView.Items.Count > 0)
            {
                deviceListView.Items[0].Selected = true;
                deviceListView.Items[0].Focused = true;
            }

            PositionAdvancedAreaCloseButton();

            SetupAccessibleMenu();

            ShowStartupTips();

            //do this last as it can take a few seconds
            SetGpuEnvironmentVariables();

            //do this after all other data has loaded to prevent errors when the delay is set very low (1s)
            SetupMiningOnStartup();
            if (!MinerIsInstalled(MinerFactory.Instance.GetDefaultMiner()))
                CancelMiningOnStartup();
            if (!MiningConfigurationValid())
                CancelMiningOnStartup();

            SetupCoalescedTimers();
            
            SubmitMobileMinerPools();

            //so we know when culture changes
            SystemEvents.UserPreferenceChanged += SystemEventsUserPreferenceChanged;

            applicationSetup = true;
        }

        private void SystemEventsUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            //culture settings changed
            if (e.Category == UserPreferenceCategory.Locale)
                //clear cached currency
                CultureInfo.CurrentCulture.ClearCachedData();
        }

        private bool tearingDown = false;
        private void TearDownApplication()
        {
            tearingDown = true;
            SaveSettings();
            StopMiningLocally();
            DisableRemoting();
        }

        private void HandleStartupMinimizedToNotificationArea()
        {
            if (applicationConfiguration.StartupMinimized && applicationConfiguration.MinimizeToNotificationArea)
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void ShowStartupTips()
        {
            string tip = null;

            switch (applicationConfiguration.TipsShown)
            {
                case 0:
                    tip = "Tip: right-click device names to change coins";
                    PostNotification(tip, tip, () =>
                    {
                        //only if we have non-network devices
                        if (localViewModel.Devices.Count(d => d.Kind != DeviceKind.NET) > 0)
                        {
                            string currentCoin = GetCurrentlySelectedCoinName();
                            CheckCoinInPopupMenu(currentCoin);

                            ListViewItem firstItem = deviceListView.Items[0];
                            Point popupPosition = firstItem.Position;
                            popupPosition.Offset(14, 6);
                            coinPopupMenu.Show(deviceListView, popupPosition);
                        }
                    }, ToolTipIcon.Info, "");
                    applicationConfiguration.TipsShown++;
                    break;
                case 1:
                    tip = "Tip: right-click the main screen for common tasks";
                    PostNotification(tip, tip, () =>
                    {
                        deviceListContextMenu.Show(deviceListView, 150, 100);
                    }, ToolTipIcon.Info, "");
                    applicationConfiguration.TipsShown++;
                    break;
                case 2:
                    tip = "Tip: restart mining after changing any settings";
                    PostNotification(tip, tip, () =>
                    {
                    }, ToolTipIcon.Info, "");
                    applicationConfiguration.TipsShown++;
                    break;
                case 3:
                    tip = "Tip: enabling perks gives back to the author";
                    PostNotification(tip, tip, () =>
                    {
                        ConfigurePerksLocally();
                    }, ToolTipIcon.Info, "");
                    applicationConfiguration.TipsShown++;
                    break;
            }

            applicationConfiguration.SaveApplicationConfiguration();
        }

        //required for GPU mining
        private void SetGpuEnvironmentVariables()
        {
            if (applicationConfiguration.SetGpuEnvironmentVariables)
            {
                using (new HourGlass())
                {
                    const string GpuMaxAllocPercent = "GPU_MAX_ALLOC_PERCENT";
                    const string GpuUseSyncObjects = "GPU_USE_SYNC_OBJECTS";

                    SetEnvironmentVariableIfNotSet(GpuMaxAllocPercent, "100");
                    SetEnvironmentVariableIfNotSet(GpuUseSyncObjects, "1");
                }
            }
        }

        private static void SetEnvironmentVariableIfNotSet(string name, string value)
        {
            string currentValue = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (String.IsNullOrEmpty(currentValue))
                Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
        }

        private void FetchInitialCoinStats()
        {
            engineConfiguration.LoadStrategyConfiguration(pathConfiguration.SharedConfigPath); //needed before refreshing coins
            engineConfiguration.LoadCoinConfigurations(pathConfiguration.SharedConfigPath); //needed before refreshing coins
            //already done in ctor applicationConfiguration.LoadApplicationConfiguration(); //needed before refreshing coins
            SetupNotificationsControl(); //needed before refreshing coins
            SetupCoinApi(); //so we target the correct API
            RefreshCoinStats();
        }

        private void SetupMiningEngineEvents()
        {
            miningEngine.LogProcessClose += LogProcessClose;
            miningEngine.LogProcessLaunch += LogProcessLaunch;
            miningEngine.ProcessLaunchFailed += ProcessLaunchFailed;
            miningEngine.ProcessAuthenticationFailed += ProcessAuthenticationFailed;
        }

        private const int MaxHistoryOnScreen = 1000;
        private void LoadPreviousHistory()
        {
            const string logFileName = "MiningLog.json";
            string logDirectory = ApplicationPaths.AppDataPath();
            if (Directory.Exists(applicationConfiguration.LogFilePath))
                logDirectory = applicationConfiguration.LogFilePath;
            string logFilePath = Path.Combine(logDirectory, logFileName);
            if (File.Exists(logFilePath))
            {
                try
                {
                    List<LogProcessCloseArgs> loadLogFile = ObjectLogger.LoadLogFile<LogProcessCloseArgs>(logFilePath).ToList();
                    loadLogFile.RemoveRange(0, Math.Max(0, loadLogFile.Count - MaxHistoryOnScreen));

                    //add via the BindingSource, not logCloseEntries
                    //populating logCloseEntries and then binding causes errors on Linux
                    logProcessCloseArgsBindingSource.SuspendBinding();
                    foreach (LogProcessCloseArgs logProcessCloseArgs in loadLogFile)
                        logProcessCloseArgsBindingSource.Add(logProcessCloseArgs);
                    logProcessCloseArgsBindingSource.ResumeBinding();
                }
                catch (SystemException)
                {
                    //old MiningLog.json file - wrong format serialized
                    //MiningLog.json rolls over so we will eventually be able to
                    //load the previous log file
                    //seen as both ArgumentException and InvalidCastException - catching SystemException
                    return;
                }
            }
        }

        private void CheckAndShowGettingStarted()
        {
            //only show if there's no settings yet
            if (File.Exists(applicationConfiguration.ApplicationConfigurationFileName()))
                return;

            WizardForm wizardForm = new WizardForm(this.knownCoins);
            DialogResult dialogResult = wizardForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Engine.Data.Configuration.Engine newEngineConfiguration;
                Data.Configuration.Application newApplicationConfiguration;
                Perks newPerksConfiguration;
                wizardForm.CreateConfigurations(out newEngineConfiguration, out newApplicationConfiguration, out newPerksConfiguration);

                this.engineConfiguration = newEngineConfiguration;
                this.applicationConfiguration = newApplicationConfiguration;
                this.perksConfiguration = newPerksConfiguration;

                this.engineConfiguration.SaveCoinConfigurations();
                this.engineConfiguration.SaveMinerConfiguration();
                this.applicationConfiguration.SaveApplicationConfiguration();
                this.perksConfiguration.SavePerksConfiguration();

                SetBriefMode(applicationConfiguration.BriefUserInterface);
            }
        }

        private void CheckAndDownloadMiners()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                return; //can't auto download binaries on Linux

            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
            if (!MinerIsInstalled(miner) && 
                //may not have a Url for the miner if call to server failed
                !String.IsNullOrEmpty(miner.Url))
                InstallBackendMinerLocally(miner);
        }

        private void InstallBackendMinerLocally(MinerDescriptor miner)
        {
            string minerName = miner.Name;

            ProgressForm progressForm = new ProgressForm(String.Format("Downloading and installing {0} from {1}",
                minerName, new Uri(miner.Url).Authority));
            progressForm.Show();

            //for Mono - show the UI
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(25);
            System.Windows.Forms.Application.DoEvents();
            try
            {
                string minerPath = Path.Combine("Miners", minerName);
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
                MinerInstaller.InstallMiner(UserAgent.AgentString, miner, destinationFolder);
                //may have been installed via Remoting - dismiss notification
                notificationsControl.RemoveNotification(bfgminerNotificationId.ToString());
            }
            finally
            {
                progressForm.Close();
            }
        }

        private static bool MinerIsInstalled(MinerDescriptor miner)
        {
            string path = MinerPath.GetPathToInstalledMiner(miner);
            return File.Exists(path);
        }

        private void CheckForDisownedMiners()
        {
            //to-do: detect disowned miners for all types of running miners

            if (DisownedMinersDetected())
            {
                DialogResult messageBoxResult = MessageBox.Show("MultiMiner has detected running miners that it does not own. Would you like to kill them?",
                    "Disowned Miners Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (messageBoxResult == System.Windows.Forms.DialogResult.Yes)
                {
                    KillDisownedMiners();
                }
            }
        }

        private bool DisownedMinersDetected()
        {
            foreach (MinerDescriptor miner in MinerFactory.Instance.Miners)
                if (DisownedMinersDetected(miner.FileName))
                    return true;

            return false;
        }

        private bool DisownedMinersDetected(string minerName)
        {
            List<Process> disownedMiners = Process.GetProcessesByName(minerName).ToList();

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
                disownedMiners.Remove(minerProcess.Process);

            return disownedMiners.Count > 0;
        }

        private void KillDisownedMiners()
        {
            foreach (MinerDescriptor miner in MinerFactory.Instance.Miners)
                KillDisownedMiners(miner.FileName);
        }

        private void KillDisownedMiners(string fileName)
        {
            List<Process> disownedMiners = Process.GetProcessesByName(fileName).ToList();

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
                disownedMiners.Remove(minerProcess.Process);

            foreach (Process disownedMiner in disownedMiners)
                MinerProcess.KillProcess(disownedMiner);
        }

        private void ShowNotInstalledMinerWarning()
        {
            bool showWarning = true;

            if (OSVersionPlatform.GetConcretePlatform() != PlatformID.Unix)
            {
                MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
                string minerName = miner.Name;

                DialogResult dialogResult = MessageBox.Show(String.Format(
                    "No copy of {0} was detected. " +
                    "Would you like to download and install {0} now?", minerName), "Miner Not Found",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    InstallBackendMinerLocally(miner);
                    ScanHardwareLocally();
                    showWarning = false;
                }
            }

            if (showWarning)
                MessageBox.Show("No copy of " + MinerNames.BFGMiner + " was detected. Please go to https://github.com/nwoolls/multiminer for instructions on installing " + MinerNames.BFGMiner + ".",
                        "Miner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ConfigureDevicesForNewUser()
        {
            Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single();

            for (int i = 0; i < devices.Count; i++)
            {
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device()
                {
                    CoinSymbol = coinConfiguration.PoolGroup.Id,
                    Enabled = true
                };

                deviceConfiguration.Assign(devices[i]);
                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            engineConfiguration.SaveDeviceConfigurations();
            UpdateMiningButtons();
        }

        private static bool ConfigFileHandled()
        {
            foreach (MinerDescriptor miner in MinerFactory.Instance.Miners)
            {
                if (!ConfigFileHandledForMiner(miner))
                    return false;                
            }

            return true;
        }

        private static bool ConfigFileHandledForMiner(MinerDescriptor miner)
        {
            const string bakExtension = ".mmbak";
            string minerName = miner.Name;
            string minerExecutablePath = MinerPath.GetPathToInstalledMiner(miner);
            string confFileFilePath = String.Empty;

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
            {
                string minerFolderName = "." + minerName;
                string minerFileName = minerName + ".conf";
                confFileFilePath = Path.Combine(Path.Combine(OSVersionPlatform.GetHomeDirectoryPath(), minerFolderName), minerFileName);
            }
            else
            {
                confFileFilePath = Path.ChangeExtension(minerExecutablePath, ".conf");
            }

            if (File.Exists(confFileFilePath))
            {
                string confFileName = Path.GetFileName(confFileFilePath);
                string confBakFileName = confFileName + bakExtension;

                DialogResult dialogResult = MessageBox.Show(String.Format("A {0} file has been detected in your miner directory. This file interferes with the arguments supplied by MultiMiner. Can MultiMiner rename this file to {1}?",
                    confFileName, confBakFileName), "External Configuration Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == System.Windows.Forms.DialogResult.No)
                    return false;

                string confBakFileFilePath = confFileFilePath + bakExtension;
                File.Delete(confBakFileFilePath);
                File.Move(confFileFilePath, confBakFileFilePath);
            }

            return true;
        }
        #endregion

        #region Primary application logic
        private void SetBriefMode(bool newBriefMode)
        {
            briefMode = newBriefMode;
            RefreshDetailsToggleButton();

            //do this before adjusting the window size so we can base it on column widths
            AutoSizeListViewColumns();

            if (briefMode)
            {
                CloseDetailsArea();
                HideAdvancedPanel();
                WindowState = FormWindowState.Normal;

                int newWidth = 0;

                foreach (ColumnHeader column in deviceListView.Columns)
                    newWidth += column.Width;
                newWidth += 40; //needs to be pretty wide for e.g. Aero Basic

                newWidth = Math.Max(newWidth, 300);

                //don't (automatically) set the width to crop notifications
                newWidth = Math.Max(newWidth, notificationsControl.Width + 24);

                Size = new Size(newWidth, 400);

            }
            else
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    //use Math.Max so it won't size smaller to show more
                    Size = new Size(Math.Max(Size.Width, 720), Math.Max(Size.Height, 500));
                }
            }

            settingsPlainButton.Visible = !briefMode;
            strategiesPlainButton.Visible = !briefMode;
            poolsPlainButton.Visible = !briefMode;
            perksPlainButton.Visible = !briefMode;
            settingsButton.Visible = briefMode;

            strategiesLabel.Visible = !briefMode;
            strategyCountdownLabel.Visible = !briefMode;
            deviceTotalLabel.Visible = !briefMode;

            advancedMenuItem.Visible = !briefMode;
            //don't hide settings - the wizard talks about the button
            //settingsButton.Visible = !briefMode;
            //settingsSeparator.Visible = !briefMode;

            footerPanel.Visible = !briefMode;

            applicationConfiguration.BriefUserInterface = briefMode;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void SetListViewStyle(View view)
        {
            updatingListView = true;
            try
            {
                deviceListView.CheckBoxes = false;
                deviceListView.View = view;
                deviceListView.CheckBoxes = view != View.Tile;

                switch (view)
                {
                    case View.LargeIcon:
                        listViewStyleButton.Image = Properties.Resources.view_medium_icons;
                        break;
                    case View.Details:
                        listViewStyleButton.Image = Properties.Resources.view_details;
                        break;
                    case View.SmallIcon:
                        listViewStyleButton.Image = Properties.Resources.view_small_icons;
                        break;
                    case View.List:
                        listViewStyleButton.Image = Properties.Resources.view_list;
                        break;
                    case View.Tile:
                        listViewStyleButton.Image = Properties.Resources.view_large_icons;
                        break;
                }

                applicationConfiguration.ListViewStyle = view;

                RefreshListViewFromViewModel();

                if (view == View.Details)
                    AutoSizeListViewColumns();
            }
            finally
            {
                updatingListView = false;
            }
        }

        private const int bfgminerNotificationId = 100;
        private const int multiMinerNotificationId = 102;

        private void CheckForMultiMinerUpdates()
        {
            bool updatesAvailable = false;
            string availableVersion, installedVersion;

            using (new HourGlass())
                updatesAvailable = MultiMinerHasUpdates(out availableVersion, out installedVersion);

            if (updatesAvailable)
                DisplayMultiMinerUpdateNotification(availableVersion, installedVersion);
        }

        private static bool MultiMinerHasUpdates(out string availableVersion, out string installedVersion)
        {
            availableVersion = String.Empty;
            installedVersion = String.Empty;

            try
            {
                availableVersion = Engine.Installers.MultiMinerInstaller.GetAvailableMinerVersion();
            }
            catch (WebException)
            {
                //downloads website is down
                return false;
            }

            if (String.IsNullOrEmpty(availableVersion))
                return false;

            installedVersion = Engine.Installers.MultiMinerInstaller.GetInstalledMinerVersion();

            if (ThisVersionGreater(availableVersion, installedVersion))
                return true;

            return false;
        }

        private void DisplayMultiMinerUpdateNotification(string availableMinerVersion, string installedMinerVersion)
        {
            if (notificationsControl == null)
                //app is closing
                return;

            PostNotification(multiMinerNotificationId.ToString(),
                String.Format("MultiMiner version {0} is available ({1} installed)",
                    availableMinerVersion, installedMinerVersion), 
                () =>
                    {
                        bool allRigs = ShouldUpdateAllRigs();
                                        
                        bool wasMining = miningEngine.Mining;

                        if (wasMining)
                            StopMiningLocally();

                        //remote first as we'll be restarting
                        if (allRigs)
                            InstallMultiMinerRemotely();

                        //this will restart the app
                        InstallMultiMinerLocally();
                    }, ToolTipIcon.Info, "http://releases.multiminerapp.com");
        }

        private bool ShouldUpdateAllRigs()
        {
            bool allRigs = false;
            if (remotingEnabled && (instancesControl.Instances.Count > 1))
            {
                DialogResult dialogResult = MessageBox.Show("Would you like to apply this update to all of your online rigs?",
                    "MultiMiner Remoting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    allRigs = true;
            }
            return allRigs;
        }
        
        private static bool ThisVersionGreater(string thisVersion, string thatVersion)
        {
            Version thisVersionObj = new Version(thisVersion);
            Version thatVersionObj = new Version(thatVersion);

            return thisVersionObj > thatVersionObj;
        }

        private void CheckForBackendMinerUpdates()
        {
            bool updatesAvailable = false;
            string availableVersion, installedVersion;

            using (new HourGlass())
                updatesAvailable = BackendMinerHasUpdates(out availableVersion, out installedVersion);

            if (updatesAvailable)
                DisplayBackendMinerUpdateNotification(availableVersion, installedVersion);
        }

        private void UpdateBackendMinerAvailability()
        {
            using (new HourGlass())
            {
                List<AvailableMiner> availableMiners = null;
                try
                {
                    availableMiners = AvailableMiners.GetAvailableMiners(UserAgent.AgentString);                    
                }
                catch (WebException ex)
                {
                    //user has reported the following as a transient error and I have seen it as well
                    //for myself it may have potentially been a Fiddler proxy issue
                    //System.Net.WebException: The remote name could not be resolved: 'www.multiminerapp.com'
                    ShowMinerCheckErrorNotification(ex);
                }

                if (availableMiners != null)
                {
                    foreach (MinerDescriptor minerDescriptor in MinerFactory.Instance.Miners)
                    {
                        AvailableMiner availableMiner = availableMiners.SingleOrDefault(am => am.Name.Equals(minerDescriptor.Name, StringComparison.OrdinalIgnoreCase));
                        //no Scrypt-Jane miner for OS X (yet)
                        if (availableMiner != null)
                        {
                            minerDescriptor.Version = availableMiner.Version;
                            minerDescriptor.Url = availableMiner.Url;
                        }
                    }
                }
            }
        }

        private void ShowMinerCheckErrorNotification(WebException ex)
        {
            string notificationText = "Error checking for backend miner availability";
            //ensure Response is HttpWebResponse to avoid NullReferenceException
            if ((ex.Response != null) && (ex.Response is HttpWebResponse))
                notificationText = String.Format("{1} ({0})", (int)((HttpWebResponse)ex.Response).StatusCode, notificationText);
        
            PostNotification(ex.Message,
                notificationText, () =>
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                },
                ToolTipIcon.Warning, "");
        }

        private static bool BackendMinerHasUpdates(out string availableVersion, out string installedVersion)
        {
            availableVersion = String.Empty;
            installedVersion = String.Empty;

            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();

            if (!MinerIsInstalled(miner))
                return false;

            availableVersion = MinerFactory.Instance.GetDefaultMiner().Version;

            if (String.IsNullOrEmpty(availableVersion))
                return false;

            installedVersion = MinerInstaller.GetInstalledMinerVersion(MinerPath.GetPathToInstalledMiner(miner), miner.LegacyApi);

            if (ThisVersionGreater(availableVersion, installedVersion))
                return true;

            return false;
        }

        private void DisplayBackendMinerUpdateNotification(string availableMinerVersion, string installedMinerVersion)
        {
            if (notificationsControl == null)
                //app is closing
                return;

            int notificationId = bfgminerNotificationId;

            string informationUrl = "https://github.com/luke-jr/bfgminer/blob/bfgminer/NEWS";

            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
            string minerName = miner.Name;

            PostNotification(notificationId.ToString(),
                String.Format("{0} version {1} is available ({2} installed)",
                    minerName, availableMinerVersion, installedMinerVersion),
                () =>
                {
                    bool allRigs = ShouldUpdateAllRigs();

                    bool wasMining = miningEngine.Mining;

                    if (wasMining)
                        StopMiningLocally();

                    if (allRigs)
                        InstallBackendMinerRemotely();

                    InstallBackendMinerLocally(miner);

                    //only start mining if we stopped mining
                    if (wasMining)
                        StartMiningLocally();
                }, ToolTipIcon.Info, informationUrl);
        }

        private static void InstallMultiMinerLocally()
        {
            ProgressForm progressForm = new ProgressForm("Downloading and installing MultiMiner from " + Engine.Installers.MultiMinerInstaller.GetMinerDownloadRoot());
            progressForm.Show();

            //for Mono - show the UI
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(25);
            System.Windows.Forms.Application.DoEvents();
            try
            {
                MultiMiner.Engine.Installers.MultiMinerInstaller.InstallMiner(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
            }
            finally
            {
                progressForm.Close();
            }
        }

        private void SetAllDevicesToCoin(string coinSymbol, bool disableStrategies)
        {
            if (this.selectedRemoteInstance == null)
            {
                SetAllDevicesToCoinLocally(coinSymbol, disableStrategies);
            }
            else
            {
                SetAllDevicesToCoinRemotely(this.selectedRemoteInstance, coinSymbol, disableStrategies);
            }
        }

        private void SetAllDevicesToCoinOnAllRigs(string coinSymbol, bool disableStrategies)
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = instancesControl.Instances.Where(i => i != instancesControl.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
                SetAllDevicesToCoinRemotely(instance, coinSymbol, disableStrategies);
        }

        private void SetAllDevicesToCoinLocally(string coinSymbol, bool disableStrategies)
        {
            bool wasMining = miningEngine.Mining;
            StopMiningLocally();

            Engine.Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(coinSymbol));

            engineConfiguration.DeviceConfigurations.Clear();

            foreach (Xgminer.Data.Device device in devices)
            {
                //don't assume 1-to-1 of Devices and ViewModel.Devices
                //Devices doesn't include Network Devices
                DeviceViewModel viewModel = localViewModel.Devices.Single(vm => vm.Equals(device));

                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device();
                deviceConfiguration.Assign(viewModel);
                if (viewModel.Kind == DeviceKind.NET)
                {
                    //assume BTC for Network Devices (for now)
                    deviceConfiguration.CoinSymbol = KnownCoins.BitcoinSymbol;
                    deviceConfiguration.Enabled = true;
                }
                else if (viewModel.Kind == DeviceKind.PXY)
                {
                    if (viewModel.SupportsAlgorithm(coinConfiguration.PoolGroup.Algorithm) &&
                        //don't change the Algo a Proxy is mining - don't know what is pointed at it
                        (viewModel.Coin.Algorithm == coinConfiguration.PoolGroup.Algorithm))
                        deviceConfiguration.CoinSymbol = coinConfiguration.PoolGroup.Id;
                    else
                        deviceConfiguration.CoinSymbol = viewModel.Coin == null ? String.Empty : viewModel.Coin.Name;

                    deviceConfiguration.Enabled = viewModel.Enabled;
                }
                else
                {
                    if (viewModel.SupportsAlgorithm(coinConfiguration.PoolGroup.Algorithm))
                        deviceConfiguration.CoinSymbol = coinConfiguration.PoolGroup.Id;
                    else
                        deviceConfiguration.CoinSymbol = viewModel.Coin == null ? String.Empty : viewModel.Coin.Name;

                    deviceConfiguration.Enabled = viewModel.Enabled;
                }

                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            localViewModel.ApplyDeviceConfigurationModels(engineConfiguration.DeviceConfigurations,
                engineConfiguration.CoinConfigurations);

            engineConfiguration.SaveDeviceConfigurations();

            RefreshListViewFromViewModel();

            AutoSizeListViewColumns();

            if (wasMining)
            {
                bool wasAutoMining = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
                if (wasAutoMining)
                    EnableMiningStrategies(false);

                StartMiningLocally();

                //only re-enable if they were enabled before
                if (!disableStrategies && wasAutoMining)
                    EnableMiningStrategies(true);
            }
            else
            {
                if (disableStrategies)
                    EnableMiningStrategies(false);

                //so the Start button becomes enabled if we now have a valid config
                UpdateMiningButtons();
            }

            RefreshStrategiesCountdown();
        }

        private void EnableMiningStrategies(bool enabled = true)
        {
            engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = enabled;
            engineConfiguration.SaveStrategyConfiguration();
        }

        private void CheckForUpdates()
        {
            PlatformID concretePlatform = OSVersionPlatform.GetConcretePlatform();

            CheckForMultiMinerUpdates();

            //we cannot auto install miners on Unix (yet)
            if (applicationConfiguration.CheckForMinerUpdates && (concretePlatform != PlatformID.Unix))
                TryToCheckForMinerUpdates();
        }

        private void TryToCheckForMinerUpdates()
        {
            try
            {
                CheckForBackendMinerUpdates();
            }
            catch (ArgumentException)
            {
                string error = String.Format("Error checking for {0} updates", 
                    MinerFactory.Instance.GetDefaultMiner().Name);
                PostNotification(error, error, () =>
                {
                }, ToolTipIcon.Warning, "");
            }
        }

        private void CloseDetailsArea()
        {
            detailsAreaContainer.Panel2.Hide();
            detailsAreaContainer.Panel2Collapsed = true;
        }

        private bool detailsAreaSetup = false;
        private void ShowDetailsArea()
        {
            SetBriefMode(false);
            RefreshDetailsArea();


            if (!detailsAreaSetup && (applicationConfiguration.DetailsAreaWidth > 0))
            {
                detailsAreaContainer.SplitterDistance = detailsAreaContainer.Width - applicationConfiguration.DetailsAreaWidth;

                detailsAreaSetup = true;
            }


            detailsAreaContainer.Panel2Collapsed = false;
            detailsAreaContainer.Panel2.Show();
        }

        private void HideAdvancedPanel()
        {
            advancedAreaContainer.Panel2.Hide();
            advancedAreaContainer.Panel2Collapsed = true;
            //hide all controls or they will show/flicker under OS X/mono
            closeApiButton.Visible = false;
            apiLogGridView.Visible = false;

            applicationConfiguration.LogAreaVisible = false;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void ShowAdvancedPanel()
        {
            if (briefMode)
                SetBriefMode(false);

            closeApiButton.Top = 0;
            closeApiButton.Left = closeApiButton.Parent.Width - closeApiButton.Width - 1;

            closeApiButton.Visible = true;
            apiLogGridView.Visible = true;
            advancedAreaContainer.Panel2Collapsed = false;
            advancedAreaContainer.Panel2.Show();

            EnsureRecentLogDataVisibility();
        }

        private void ShowApiMonitor()
        {
            advancedTabControl.SelectedTab = apiMonitorPage;
            ShowAdvancedPanel();

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void ShowProcessLog()
        {
            advancedTabControl.SelectedTab = processLogPage;
            ShowAdvancedPanel();

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void ShowHistory()
        {
            advancedTabControl.SelectedTab = historyPage;
            ShowAdvancedPanel();

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private static void ShowAboutDialog()
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void RenameDevice(DeviceViewModel deviceViewModel, string name)
        {
            //rename the device in the user metadata
            Metadata.DeviceMetadata deviceData = metadataConfiguration.Devices.SingleOrDefault(d => d.Equals(deviceViewModel));
            if (deviceData == null)
            {
                deviceData = new Metadata.DeviceMetadata();
                ObjectCopier.CopyObject(deviceViewModel, deviceData);
                metadataConfiguration.Devices.Add(deviceData);
            }
            deviceData.FriendlyName = name;
            metadataConfiguration.SaveDeviceMetadataConfiguration();

            //rename the device ViewModel itself
            deviceViewModel.FriendlyName = name;
        }               

        private static string GetOwnedProcessFilePath()
        {
            return Path.Combine(Path.GetTempPath(), "MultiMiner.Processes.xml");
        }

        //https://github.com/nwoolls/MultiMiner/issues/152
        //http://social.msdn.microsoft.com/Forums/vstudio/en-US/94ba760c-7080-4614-8a56-15582c48f900/child-process-keeps-parents-socket-open-diagnosticsprocess-and-nettcplistener?forum=netfxbcl
        //keep track of processes we've launched so we can kill them later
        private void SaveOwnedProcesses()
        {
            OwnedProcesses.SaveOwnedProcesses(miningEngine.MinerProcesses.Select(mp => mp.Process), GetOwnedProcessFilePath());
        }

        private static bool KillOwnedProcesses()
        {
            bool processesKilled = false;
            string filePath = GetOwnedProcessFilePath();
            IEnumerable<Process> ownedProcesses = OwnedProcesses.GetOwnedProcesses(filePath);
            foreach (Process ownedProcess in ownedProcesses)
            {
                MinerProcess.KillProcess(ownedProcess);
                processesKilled = true;
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            return processesKilled;
        }
        #endregion

        #region Primary mining logic
        private void StartMining()
        {
            if (this.selectedRemoteInstance == null)
            {
                StartMiningLocally();
            }
            else
            {
                StartMiningRemotely(this.selectedRemoteInstance);
            }
        }

        private void StopMining()
        {
            if (this.selectedRemoteInstance == null)
            {
                StopMiningLocally();
            }
            else
            {
                StopMiningRemotely(this.selectedRemoteInstance);
            }
        }

        private void StopMiningLocally()
        {
            using (new HourGlass())
            {
                miningEngine.StopMining();
            }

            localViewModel.ClearDeviceInformationFromViewModel();

            ClearCachedNetworkDifficulties();
            processDeviceDetails.Clear();
            lastDevicePoolMapping.Clear();
            RefreshStrategiesCountdown();
            hashRateStatusLabel.Text = string.Empty;
            notifyIcon1.Text = "MultiMiner - Stopped";
            UpdateMiningButtons();
            RefreshIncomeSummary();
            AutoSizeListViewColumns();
            RefreshDetailsAreaIfVisible();
            ClearPoolsFlaggedDown();
            SaveOwnedProcesses();

            if (apiConsoleForm != null)
                apiConsoleForm.PopulateMiners(miningEngine.MinerProcesses, networkDevicesConfiguration.Devices, localViewModel);
        }

        private void RestartMining()
        {
            if (this.selectedRemoteInstance == null)
            {
                RestartMiningLocally();
            }
            else
            {
                RestartMiningRemotely(this.selectedRemoteInstance);
            }
        }

        private void RestartMiningLocally()
        {
            StopMiningLocally();

            //refresh stats from Coin API so the Restart button can be used as a way to
            //force MultiMiner to apply updated mining strategies
            RefreshCoinStats();

            StartMiningLocally();
        }

        private void RestartMiningLocallyIfMining()
        {
            if (miningEngine.Mining)
            {
                StopMiningLocally();
                StartMiningLocally();
            }
        }

        private void CheckAndApplyMiningStrategy()
        {
            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                //ensure the user isn't editing settings
                !ShowingModalDialog())
            {
                bool changed = miningEngine.ApplyMiningStrategy(coinApiInformation
                    .ToList()); //get a copy - populated async & collection may be modified)

                //save any changes made by the engine
                engineConfiguration.SaveDeviceConfigurations();

                //create a deep clone of the mining & device configurations
                //this is so we can accurately display e.g. the currently mining pools
                //even if the user changes pool info without restarting mining
                this.miningCoinConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Coin>, List<Engine.Data.Configuration.Coin>>(engineConfiguration.CoinConfigurations);
                this.miningDeviceConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Device>, List<Engine.Data.Configuration.Device>>(engineConfiguration.DeviceConfigurations);

                //update the ViewModel
                ApplyModelsToViewModel();

                //to get changes from strategy config
                //to refresh coin stats due to changed coin selections
                RefreshListViewFromViewModel();

                if (changed)
                {
                    ShowCoinChangeNotification();
                    SaveOwnedProcesses();

                    if (apiConsoleForm != null)
                        apiConsoleForm.PopulateMiners(miningEngine.MinerProcesses, networkDevicesConfiguration.Devices, localViewModel);
                }
            }
        }

        public void PostNotification(string id, string text, Action clickHandler, ToolTipIcon icon, string informationUrl = "")
        {
            MobileMiner.Data.NotificationKind kind = MobileMiner.Data.NotificationKind.Information;
            switch (icon)
            {
                case ToolTipIcon.None:
                    kind = MobileMiner.Data.NotificationKind.Default;
                    break;
                case ToolTipIcon.Info:
                    kind = MobileMiner.Data.NotificationKind.Information;
                    break;
                case ToolTipIcon.Warning:
                    kind = MobileMiner.Data.NotificationKind.Warning;
                    break;
                case ToolTipIcon.Error:
                    kind = MobileMiner.Data.NotificationKind.Danger;
                    break;
            }

            notificationsControl.AddNotification(id, text, clickHandler, kind, informationUrl);

            if (notifyIcon1.Visible)
                ShowBalloonNotification(text, clickHandler, icon);
        }

        private void ShowBalloonNotification(string text, Action clickHandler, ToolTipIcon icon)
        {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.BalloonTipTitle = "MultiMiner";
            notifyIcon1.BalloonTipIcon = icon;

            notificationClickHandler = clickHandler;

            notifyIcon1.ShowBalloonTip(1000); // ms
        }

        private void ShowCoinChangeNotification()
        {
            IEnumerable<string> coinList = miningEngine.MinerProcesses
                .Select(mp => mp.CoinSymbol)
                // there may be multiple processes for one coin symbol
                .Distinct()
                // sort the symbols
                .OrderBy(cs => cs);

            string id = Guid.NewGuid().ToString();
            string text = String.Format("Mining switched to {0} based on {1}", 
                String.Join(", ", coinList.ToArray()), 
                engineConfiguration.StrategyConfiguration.MiningBasis);
            string url = successfulApiContext.GetInfoUrl();

            PostNotification(id,
                String.Format(text), () =>
                {
                    ConfigureStrategies();
                },
                ToolTipIcon.Info, url);
        }

        private void CheckAndNotifyFoundBlocks(MinerProcess minerProcess, long foundBlocks)
        {
            //started mining but haven't yet assigned mining members
            if (miningCoinConfigurations == null)
                return;

            string coinName = minerProcess.MinerConfiguration.CoinName;
            //reference miningCoinConfigurations so that we get access to the mining coins
            Engine.Data.Configuration.Coin configuration = miningCoinConfigurations.SingleOrDefault(c => c.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnBlockFound2 && (foundBlocks > minerProcess.FoundBlocks))
            {
                minerProcess.FoundBlocks = foundBlocks;

                string notificationReason = String.Format("Block(s) found for {0} (block {1})",
                    coinName, minerProcess.FoundBlocks);

                PostNotification(notificationReason, notificationReason, () =>
                {
                }, ToolTipIcon.Info, "");
            }
        }

        private void CheckAndNotifyAcceptedShares(MinerProcess minerProcess, long acceptedShares)
        {
            //started mining but haven't yet assigned mining members
            if (miningCoinConfigurations == null)
                return;

            string coinName = minerProcess.MinerConfiguration.CoinName;
            //reference miningCoinConfigurations so that we get access to the mining coins
            Engine.Data.Configuration.Coin configuration = miningCoinConfigurations.SingleOrDefault(c => c.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnShareAccepted2 && (acceptedShares > minerProcess.AcceptedShares))
            {
                minerProcess.AcceptedShares = acceptedShares;

                string notificationReason = String.Format("Share(s) accepted for {0} (share {1})",
                    coinName, minerProcess.AcceptedShares);

                PostNotification(notificationReason, notificationReason, () =>
                {
                }, ToolTipIcon.Info, "");
            }
        }

        private bool updatingListView = false;
        private void ScanHardware()
        {
            if (this.selectedRemoteInstance == null)
            {
                ScanHardwareLocally();
            }
            else
            {
                ScanHardwareRemotely(this.selectedRemoteInstance);
            }
        }

        private void ScanHardwareLocally()
        {
            if (miningEngine.Mining)
                return;

            ProgressForm progressForm = new ProgressForm("Scanning hardware for devices capable of mining. Please be patient.");
            updatingListView = true;
            try
            {
                progressForm.IsDownload = false;
                //not ShowDialog()
                progressForm.Show();
                try
                {
                    using (new HourGlass())
                    {
                        DevicesService devicesService = new DevicesService(engineConfiguration.XgminerConfiguration);
                        MinerDescriptor defaultMiner = MinerFactory.Instance.GetDefaultMiner();
                        devices = devicesService.GetDevices(defaultMiner, MinerPath.GetPathToInstalledMiner(defaultMiner));

                        //safe to do here as we are Scanning Hardware - we are not mining
                        //no data to lose in the ViewModel
                        //clearing means our sort order within the ListView is preserved
                        //and things like selecting the first item work better
                        //http://social.msdn.microsoft.com/Forums/windows/en-US/8a81c5a6-251c-4bf9-91c5-a937b5cfe9f3/possible-bug-in-listview-control-topitem-property-doesnt-work-with-groups
                        localViewModel.Devices.Clear();

                        ApplyModelsToViewModel();
                        //populate ListView directly after - maintain 1-to-1 for ViewModel to ListView items
                        RefreshListViewFromViewModel();
                    }
                }
                catch (Win32Exception)
                {
                    //miner not installed/not launched
                    devices = new List<Xgminer.Data.Device>(); //dummy empty device list

                    ShowNotInstalledMinerWarning();
                }

                if ((devices.Count > 0) && (engineConfiguration.DeviceConfigurations.Count == 0) &&
                    (engineConfiguration.CoinConfigurations.Count == 1))
                {
                    //setup devices for a brand new user
                    ConfigureDevicesForNewUser();
                }

                //first try to match up devices without configurations with configurations without devices
                //could happen if, for instance, a COM port changes for a device
                FixOrphanedDeviceConfigurations();

                //there needs to be a device config for each device
                AddMissingDeviceConfigurations();
                //but no configurations for devices that have gone missing
                RemoveExcessDeviceConfigurations();
                //remove any duplicate configurations
                engineConfiguration.RemoveDuplicateDeviceConfigurations();

                RefreshListViewFromViewModel();
                RefreshDetailsAreaIfVisible();

                //clean up mappings from previous device list
                deviceDetailsMapping.Clear();

                //auto-size columns
                AutoSizeListViewColumns();
                RefreshStatusBarFromViewModel();

                //it may not be possible to mine after discovering devices
                UpdateMiningButtons();

                //cache devices
                SaveKnownDevicesToFile();
            }
            finally
            {
                updatingListView = false;
                progressForm.Close();
                progressForm.Dispose();
            }
        }

        private void RefreshStatusBarFromViewModel()
        {
            MinerFormViewModel viewModel = GetViewModelToView();
            //don't include Network Devices in the count for Remote ViewModels
            deviceTotalLabel.Text = String.Format("{0} device(s)", viewModel.Devices.Count(d => (viewModel == localViewModel) || (d.Kind != DeviceKind.NET)));
            
            hashRateStatusLabel.Text = GetHashRateStatusText(viewModel);

            hashRateStatusLabel.AutoSize = true;
        }

        private string GetHashRateStatusText(MinerFormViewModel viewModel)
        {
            string hashRateText = String.Empty;
            IList<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
            {
                double hashRate = GetVisibleInstanceHashrate(algorithm.Name, viewModel == localViewModel);
                if (hashRate > 0.00)
                {
                    if (!String.IsNullOrEmpty(hashRateText))
                        hashRateText = hashRateText + "   ";
                    hashRateText = String.Format("{0}{1}: {2}", hashRateText, algorithm.Name, hashRate.ToHashrateString());
                }
            }
            return hashRateText;
        }
        private void StartMiningLocally()
        {
            //do not set Dynamic Intensity here - may have already been set by idleTimer_Tick
            //don't want to override

            CancelMiningOnStartup(); //in case clicked during countdown
            
            SaveChangesLocally();

            if (!MiningConfigurationValid())
                return;

            if (miningEngine.Mining)
                return;

            //download miners BEFORE checking for config files
            DownloadRequiredMiners();

            if (!ConfigFileHandled())
                return;

            startButton.Enabled = false; //immediately disable, update after
            startMenuItem.Enabled = false;

            try
            {
                using (new HourGlass())
                {
                    int donationPercent = 0;
                    if (perksConfiguration.PerksEnabled)
                        donationPercent = perksConfiguration.DonationPercent;
                    miningEngine.StartMining(
                        engineConfiguration,
                        devices, 
                        coinApiInformation
                            .ToList(), //get a copy - populated async & collection may be modified
                        donationPercent);
                }
            }
            catch (MinerLaunchException ex)
            {
                MessageBox.Show(ex.Message, "Error Launching Miner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //do this AFTER we start mining to pick up any Auto-Mining changes

            //create a deep clone of the mining & device configurations
            //this is so we can accurately display e.g. the currently mining pools
            //even if the user changes pool info without restartinging mining
            this.miningCoinConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Coin>, List<Engine.Data.Configuration.Coin>>(engineConfiguration.CoinConfigurations);
            this.miningDeviceConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Device>, List<Engine.Data.Configuration.Device>>(engineConfiguration.DeviceConfigurations);

            engineConfiguration.SaveDeviceConfigurations(); //save any changes made by the engine

            //update ViewModel with potential changes 
            ApplyModelsToViewModel();

            autoSizeColumnsFlag = 0;

            RefreshStrategiesCountdown();

            //so restart timer based on when mining started, not a constantly ticking timer
            //see https://bitcointalk.org/index.php?topic=248173.msg4593795#msg4593795
            SetupRestartTimer();

            //to get changes from strategy config
            //to get updated coin stats for coin changes
            RefreshListViewFromViewModel();

            AutoSizeListViewColumns();
            UpdateMiningButtons();

            SaveOwnedProcesses();

            if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                // if no Internet / network connection, we did not Auto-Mine
                (this.coinApiInformation != null))
                ShowCoinChangeNotification();

            if (apiConsoleForm != null)
                apiConsoleForm.PopulateMiners(miningEngine.MinerProcesses, networkDevicesConfiguration.Devices, localViewModel);
        }

        //download miners required for configured coins / algorithms
        private void DownloadRequiredMiners()
        {
            IEnumerable<string> configuredAlgorithms = engineConfiguration.CoinConfigurations
                .Where(config => config.Enabled)
                .Select(config => config.PoolGroup.Algorithm)
                .Distinct();

            SerializableDictionary<string, string> algorithmMiners = engineConfiguration.XgminerConfiguration.AlgorithmMiners;

            foreach (string algorithmName in configuredAlgorithms)
            {
                //safe to assume we are downloading GPU miners here
                MinerDescriptor miner = MinerFactory.Instance.GetMiner(DeviceKind.GPU, algorithmName, algorithmMiners);

                //is miner configured for algorithm
                if (miner != null)
                    CheckAndDownloadMiner(miner);
            }
        }

        private void CheckAndDownloadMiner(MinerDescriptor miner)
        {
            string installedFilePath = MinerPath.GetPathToInstalledMiner(miner);
            if (!File.Exists(installedFilePath))
                InstallBackendMinerLocally(miner);
        }

        private void ToggleDynamicIntensityLocally(bool enabled)
        {
            engineConfiguration.XgminerConfiguration.DesktopMode = enabled;
            dynamicIntensityButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;
            engineConfiguration.SaveMinerConfiguration();

            GetViewModelToView().DynamicIntensity = enabled;
        }

        private void ToggleDynamicIntensity(bool enabled)
        {
            if (this.selectedRemoteInstance == null)
            {
                ToggleDynamicIntensityLocally(enabled);
            }
            else
            {
                ToggleDynamicIntensityRemotely(this.selectedRemoteInstance, enabled);
            }
        }

        private void ClearPoolsFlaggedDown()
        {
            foreach (Engine.Data.Configuration.Coin coinConfiguration in engineConfiguration.CoinConfigurations)
                coinConfiguration.PoolsDown = false;
            engineConfiguration.SaveCoinConfigurations();
        }

        private void SuggestCoinsToMine()
        {
            if (!applicationConfiguration.SuggestCoinsToMine)
                return;
            if (applicationConfiguration.SuggestionsAlgorithm == CoinSuggestionsAlgorithm.None)
                return;
            if (coinApiInformation == null) //no network connection
                return;

            IEnumerable<CoinInformation> coinsToMine = CoinSuggestions.GetCoinsToMine(
                coinApiInformation
                    .ToList(), //get a copy - populated async & collection may be modified) 
                applicationConfiguration.SuggestionsAlgorithm, 
                engineConfiguration.StrategyConfiguration, 
                engineConfiguration.CoinConfigurations);

            foreach (CoinInformation coin in coinsToMine)
                NotifyCoinToMine(this.successfulApiContext, coin);
        }

        private void NotifyCoinToMine(IApiContext apiContext, CoinInformation coin)
        {
            string value = coin.AverageProfitability.ToString(".#") + "%";
            string noun = "average profitability";

            switch (engineConfiguration.StrategyConfiguration.MiningBasis)
            {
                case Strategy.CoinMiningBasis.Difficulty:
                    value = coin.Difficulty.ToString(".####");
                    noun = "difficulty";
                    break;
                case Strategy.CoinMiningBasis.Price:
                    value = coin.Price.ToString(".########");
                    noun = "price";
                    break;
            }

            string infoUrl = apiContext.GetInfoUrl();

            PostNotification(coin.Symbol,
                String.Format("Consider mining {0} ({1} {2})",
                    coin.Symbol, value, noun), 
                () =>
                    {
                        Process.Start(String.Format("https://www.google.com/search?q={0}+{1}+mining+pools",
                            coin.Symbol, coin.Name));
                    }, ToolTipIcon.Info, infoUrl);
        }

        private void CancelMiningOnStartup()
        {
            startupMiningCountdownSeconds = 0;
            startupMiningPanel.Visible = false;
            countdownLabel.Visible = false; //or remains visible under Mono
            cancelStartupMiningButton.Visible = false; //or remains visible under Mono
        }

        private void CheckIdleTimeForDynamicIntensity(long timerInterval)
        {
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                return; //idle detection code uses User32.dll

            if (applicationConfiguration.AutoSetDesktopMode)
            {
                TimeSpan idleTimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount - IdleTimeFinder.GetLastInputTime());

                const int idleMinutesForDesktopMode = 2;

                //if idle for more than 1 minute, disable Desktop Mode
                if (idleTimeSpan.TotalMinutes > idleMinutesForDesktopMode)
                {
                    if (engineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        ToggleDynamicIntensityLocally(false);
                        RestartMiningLocallyIfMining();
                    }
                }
                //else if idle for less than the idleTimer interval, enable Desktop Mode
                else if (idleTimeSpan.TotalMilliseconds <= timerInterval)
                {
                    if (!engineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        ToggleDynamicIntensityLocally(true);
                        RestartMiningLocallyIfMining();
                    }
                }
            }
        }

        private void CheckMiningOnStartupStatus()
        {
            if (startupMiningCountdownSeconds > 0)
            {
                startupMiningCountdownSeconds--;
                RefreshCountdownLabel();
                if (startupMiningCountdownSeconds == 0)
                {
                    startupMiningPanel.Visible = false;
                    System.Windows.Forms.Application.DoEvents();
                    StartMiningLocally();
                }
            }
        }
        #endregion

        #region Network Devices
        
        private void RestartSuspectNetworkDevices()
        {
            RestartNetworkDevicesForChainStatus();
            RestartNetworkDevicesForSubparHashrate();
        }

        private bool DeviceIsWarmedUp(DeviceViewModel deviceViewModel)
        {
            bool warm = false;

            List<MinerStatistics> statistics = GetCachedMinerStatisticsFromViewModel(deviceViewModel);
            //null in case of API failure
            if ((statistics != null) && (statistics.Count > 0))
                warm = statistics.First().Elapsed > MiningEngine.SecondsToWarmUpMiner;

            return warm;
        }

        private void RestartNetworkDevicesForSubparHashrate()
        {
            IEnumerable<DeviceViewModel> suspectNetworkDevices =
                localViewModel
                .Devices.Where(
                    d => (d.Kind == DeviceKind.NET)
                        && (d.AverageHashrate > 0)
                        && ((d.CurrentHashrate / d.AverageHashrate) < 0.5)
                        && DeviceIsWarmedUp(d)
                ).ToList();

            foreach (DeviceViewModel networkDevice in suspectNetworkDevices)
                RestartSuspectNetworkDevice(networkDevice, "subpar hashrate");
        }

        private void RestartNetworkDevicesForChainStatus()
        {
            IEnumerable<DeviceViewModel> suspectNetworkDevices =
                localViewModel
                .Devices.Where(
                    d => (d.Kind == DeviceKind.NET)
                        && (d.ChainStatus.Any(cs => !String.IsNullOrEmpty(cs) && cs.Count(c => c == 'x') > 2))
                        && DeviceIsWarmedUp(d)
                ).ToList();

            foreach (DeviceViewModel networkDevice in suspectNetworkDevices)
            {
                //we don't want to keep trying to restart it over and over - clear suspect status
                ClearChainStatus(networkDevice);

                RestartSuspectNetworkDevice(networkDevice, "chain status");
            }
        }

        private void RestartSuspectNetworkDevice(DeviceViewModel networkDevice, string reason)
        {
            string message = String.Format("Restarting {0} ({1})", networkDevice.FriendlyName, reason);
            try
            {
                if (!RestartNetworkDevice(networkDevice))
                {
                    if (!applicationConfiguration.ShowApiErrors)
                    {
                        //early exit - we aren't notifying for API errors
                        return;
                    }

                    message = String.Format("Access denied restarting {0} ({1})", networkDevice.FriendlyName, reason);
                }
            }
            catch (SocketException)
            {
                message = String.Format("Timeout restarting {0} ({1})", networkDevice.FriendlyName, reason);
            }

            //code to update UI
            PostNotification(message,
                message, () =>
                {
                }, ToolTipIcon.Error);
        }

        private static void ClearChainStatus(DeviceViewModel networkDevice)
        {
            for (int i = 0; i < networkDevice.ChainStatus.Length; i++)
                networkDevice.ChainStatus[i] = String.Empty;
        }

        private void RestartNetworkDevice()
        {
            DeviceViewModel networkDevice = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            RestartNetworkDevice(networkDevice);
        }

        private bool RestartNetworkDevice(DeviceViewModel networkDevice)
        {
            Uri uri = new Uri("http://" + networkDevice.Path);
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            string response = apiContext.RestartMining();
            bool result = !response.ToLower().Contains("STATUS=E".ToLower());

            if (result)
            {
                //clear cached stats so we do not restart newly restarted instances
                networkDeviceStatistics.Remove(networkDevice.Path);
            }

            return result;
        }

        private void SetNetworkDevicePool(int poolIndex)
        {
            DeviceViewModel networkDevice = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            Uri uri = new Uri("http://" + networkDevice.Path);
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            apiContext.SwitchPool(poolIndex);
        }

        private void NetworkDevicePoolClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            SetNetworkDevicePool((int)menuItem.Tag);
        }

        private void UpdateNetworkDeviceMenu()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            NetworkDevices.NetworkDevice deviceConfiguration = networkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            PopulateNetworkDevicePoolMenu(deviceViewModel);
            stickyToolStripMenuItem.Checked = deviceConfiguration.Sticky;
        }

        private void ToggleNetworkDeviceSticky()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            NetworkDevices.NetworkDevice deviceConfiguration = networkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Sticky = !deviceConfiguration.Sticky;
            networkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        private void ToggleNetworkDeviceHidden()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            NetworkDevices.NetworkDevice deviceConfiguration = networkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Hidden = !deviceConfiguration.Hidden;
            networkDevicesConfiguration.SaveNetworkDevicesConfiguration();

            ApplyDevicesToViewModel();
            RefreshListViewFromViewModel();
        }

        private void PopulateNetworkDevicePoolMenu(DeviceViewModel viewModel)
        {
            networkDevicePoolMenu.DropDownItems.Clear();

            if (!networkDevicePools.ContainsKey(viewModel.Path))
                //Network Device is offline but pinned
                return;
            
            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = networkDevicePools[viewModel.Path];

            if (poolInformation == null)
                //RPC API call timed out
                return;

            for (int i = 0; i < poolInformation.Count; i++)
            {
                PoolInformation pool = poolInformation[i];
                ToolStripMenuItem menuItem = new ToolStripMenuItem(pool.Url.DomainFromHost());
                menuItem.Tag = i;
                menuItem.Click += NetworkDevicePoolClicked;
                networkDevicePoolMenu.DropDownItems.Add(menuItem);
            }
        }
        #endregion

        private void aPIConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apiConsoleForm = new ApiConsoleForm(miningEngine.MinerProcesses, networkDevicesConfiguration.Devices, localViewModel);
            apiConsoleForm.Show();

        }

        private void stickyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleNetworkDeviceSticky();
        }

        private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleNetworkDeviceHidden();
        }
    }
}
