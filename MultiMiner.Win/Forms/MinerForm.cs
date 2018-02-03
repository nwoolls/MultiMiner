using Microsoft.Win32;
using MultiMiner.CoinApi.Data;
using MultiMiner.Discovery;
using MultiMiner.Discovery.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.MobileMiner.Data;
//using MultiMiner.MobileMiner.Embed;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using MultiMiner.Win.Controls;
using MultiMiner.Win.Controls.Notifications;
using MultiMiner.Win.Forms.Configuration;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms
{
    public partial class MinerForm : MessageBoxFontForm
    {
        #region Private fields  
        //fields
        private bool applicationSetup;
        private bool instancesAreaSetup;
        private bool detailsAreaSetup;
        private bool editingDeviceListView;
        private bool keyPressHandled;
        private Action notificationClickHandler;

        //controls
        private NotificationsControl notificationsControl;
        private InstancesControl instancesControl;
        private ApiConsoleControl apiConsoleControl;
        
        //view models
        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private bool refreshViewRequested;
        #endregion

        #region Constructor
        public MinerForm()
        {
            InitializeComponent();

            instancesContainer.Panel1Collapsed = true;
            detailsAreaContainer.Panel2Collapsed = true;

            app.PathConfiguration.LoadPathConfiguration();
            app.ApplicationConfiguration.LoadApplicationConfiguration(app.PathConfiguration.SharedConfigPath);

            if (app.ApplicationConfiguration.StartupMinimized)
                WindowState = FormWindowState.Minimized;
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
            instancesControl = new InstancesControl(app.InstanceManager)
            {
                Dock = DockStyle.Fill,
                Parent = instancesContainer.Panel1
            };
            instancesControl.SelectedInstanceChanged += instancesControl1_SelectedInstanceChanged;
        }

        private void SetupLookAndFeel()
        {
            Version win8Version = new Version(6, 2, 9200, 0);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8Version)
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
            if (accessibleMenu.Visible != app.ApplicationConfiguration.UseAccessibleMenu)
            {
                accessibleMenu.Visible = app.ApplicationConfiguration.UseAccessibleMenu;
                standardToolBar.Visible = !app.ApplicationConfiguration.UseAccessibleMenu;
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
            const int controlOffset = 2;
            const int controlHeight = 148;
            const int controlWidth = 358;

            notificationsControl = new NotificationsControl()
            {
                Visible = false,
                Height = controlHeight,
                Width = controlWidth,
                Parent = parent,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            notificationsControl.NotificationsChanged += notificationsControl1_NotificationsChanged;
            notificationsControl.NotificationAdded += notificationsControl1_NotificationAdded;

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                //adjust for different metrics/layout under OS X/Unix
                notificationsControl.Width += 50;

            //base this on control.Width, not ControlWidth
            notificationsControl.Left = parent.Width - notificationsControl.Width - controlOffset;
            //same here
            notificationsControl.Top = parent.Height - notificationsControl.Height - controlOffset;
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

            strategyCountdownLabel.Visible = app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
        }
        #endregion

        #region View / ViewModel behavior
        private void RemoveListViewItemsMissingFromViewModel(MinerFormViewModel viewModelToView)
        {
            for (int i = deviceListView.Items.Count - 1; i >= 0; i--)
            {
                DeviceViewModel listModel = (DeviceViewModel)deviceListView.Items[i].Tag;
                if (!viewModelToView.Devices.Contains(listModel) || !listModel.Visible)
                    deviceListView.Items.RemoveAt(i);

                //Network Device detection disabled
                if (!app.ApplicationConfiguration.NetworkDeviceDetection && (listModel.Kind == DeviceKind.NET))
                    deviceListView.Items.RemoveAt(i);
            }
        }

        private ListViewItem GetListViewItemForDeviceViewModel(DeviceViewModel deviceViewModel)
        {
            return deviceListView.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Tag == deviceViewModel);
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
        
        private void RefreshViewFromViewModel()
        {
            refreshViewRequested = true;
        }

        private void DoRefreshViewFromViewModel()
        {
            PositionAdvancedAreaCloseButton();
            RefreshDetailsToggleButton();
            PositionCoinChooseLabels();
            RefreshStrategiesCountdown();
            RefreshStrategiesLabel();
            RefreshListViewFromViewModel();
            UpdateMiningButtons();
            UpdateSaveButtons();
            RefreshStatusBarFromViewModel();
            SetNotificationAreaHint();
            UpdateInstancesStatsFromLocal();
            RefreshIncomeSummary();
            RefreshDetailsAreaIfVisible();
            RefreshCountdownLabel();
            RefreshCoinStatsLabel();
            SetupStatusBarLabelLayouts();
            RefreshCoinApiLabel();

            if (apiConsoleControl != null)
            {
                //call ToList() so we can get a copy - otherwise risk:
                //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
                List<MinerProcess> minerProcesses = app.MiningEngine.MinerProcesses.ToList();
                List<NetworkDevices.NetworkDevice> networkDevices = app.NetworkDevicesConfiguration.Devices.ToList();
                apiConsoleControl.PopulateMiners(minerProcesses, networkDevices, app.LocalViewModel);
            }

            instancesContainer.Panel1Collapsed = !app.PerksConfiguration.EnableRemoting || (app.InstanceManager.Instances.Count <= 1);
            instancesControl.Visible = !instancesContainer.Panel1Collapsed;
            if (instancesControl.Visible)
            {
                //can't set details container width until it is shown
                //test with ApplicationConfiguration.StartupMinimized
                if (!instancesAreaSetup && (instancesContainer.Width > 0) && (app.ApplicationConfiguration.InstancesAreaWidth > 0))
                {
                    instancesAreaSetup = true;
                    instancesContainer.SplitterDistance = app.ApplicationConfiguration.InstancesAreaWidth;
                }
            }
            
            dynamicIntensityButton.Checked = app.LocalViewModel.DynamicIntensity;
        }

        private void UpdateInstancesStatsFromLocal()
        {
            if (instancesControl.Visible)
            {
                Remoting.Data.Transfer.Machine machine = new Remoting.Data.Transfer.Machine();
                app.PopulateLocalMachineHashrates(machine, true);

                instancesControl.ApplyMachineInformation("localhost", machine);
            }
        }

        private void UpdateSaveButtons()
        {
            bool hasChanges = app.LocalViewModel.HasChanges;

            if (app.SelectedRemoteInstance != null)
                hasChanges = app.RemoteViewModel.HasChanges;

            saveButton.Visible = hasChanges;
            cancelButton.Visible = hasChanges;
            saveSeparator.Visible = hasChanges;

            saveButton.Enabled = hasChanges;
            cancelButton.Enabled = hasChanges;

            //accessible menu
            saveToolStripMenuItem.Enabled = hasChanges;
            cancelToolStripMenuItem.Enabled = hasChanges;
        }

        private void SetNotificationAreaHint()
        {
            //max length for notify icon text is 63
            string bubbleText = string.Format("MultiMiner - {0}", hashRateStatusLabel.Text);
            //must be less than (not equal to) 64 characters
            if (bubbleText.Length >= 64)
                bubbleText = bubbleText.Substring(0, 60) + "...";
            notifyIcon1.Text = bubbleText;
        }

        private void RefreshListViewFromViewModel()
        {
            if (app.Context == null)
                return;

            if (editingDeviceListView)
                return;

            List<int> selectedIndexes = deviceListView.SelectedIndices.Cast<int>().ToList();

            deviceListView.BeginUpdate();
            updatingListView = true;
            try
            {
                try
                {
                    utilityColumnHeader.Text = app.ApplicationConfiguration.ShowWorkUtility ? "Work Utility" : "Utility";
                }
                catch (InvalidOperationException)
                {
                    //user was resizing columns
                }

                //clear all coin stats first
                //there may be coins configured that are no longer returned in the stats
                ClearAllCoinStats();

                MinerFormViewModel viewModelToView = app.GetViewModelToView();

                RemoveListViewItemsMissingFromViewModel(viewModelToView);

                foreach (DeviceViewModel deviceViewModel in viewModelToView.Devices)
                {
                    //app is closing
                    if (app.Context == null)
                        return;

                    RefreshListViewForDeviceViewModel(deviceViewModel);
                }

                foreach (int selectedIndex in selectedIndexes)
                    //selectedIndex may be -1, check for that
                    if ((selectedIndex >= 0) && (selectedIndex < deviceListView.Items.Count))
                        deviceListView.Items[selectedIndex].Selected = true;

                //only This PC for now
                deviceListView.LabelEdit = app.SelectedRemoteInstance == null;

                ClearMinerStatsForDisabledCoins();
                RemoveInvalidCoinValuesFromListView();
            }
            finally
            {
                deviceListView.EndUpdate();
                updatingListView = false;
            }

            AutoSizeListViewColumns();
        }

        private void RefreshListViewForDeviceViewModel(DeviceViewModel deviceViewModel)
        {
            if (!deviceViewModel.Visible)
                return;

            //Network Devices should only show from the Local ViewModel
            if ((app.GetViewModelToView() == app.RemoteViewModel) &&
                (deviceViewModel.Kind == DeviceKind.NET))
                return;

            //Network Devices disabled
            if (!app.ApplicationConfiguration.NetworkDeviceDetection &&
                (deviceViewModel.Kind == DeviceKind.NET))
                return;

            ListViewItem listViewItem = FindOrAddListViewItemForViewModel(deviceViewModel);

            RefreshListViewItemForDeviceViewModel(listViewItem, deviceViewModel);
        }

        private void RefreshListViewItemForDeviceViewModel(ListViewItem listViewItem, DeviceViewModel deviceViewModel)
        {
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

            if (deviceViewModel.Kind == DeviceKind.NET)
            {
                if (app.NetworkDeviceWasStopped(deviceViewModel))
                {
                    listViewItem.ForeColor = SystemColors.GrayText;
                    listViewItem.UseItemStyleForSubItems = true;
                    listViewItem.Checked = false;
                }
                else
                {
                    listViewItem.ForeColor = SystemColors.WindowText;
                    listViewItem.UseItemStyleForSubItems = false;
                    listViewItem.Checked = true;
                }
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

                double difficulty = app.GetCachedNetworkDifficulty(deviceViewModel.Pool ?? String.Empty);
                if (difficulty == 0.0)
                    difficulty = deviceViewModel.Difficulty;

                listViewItem.SubItems["Difficulty"].Tag = difficulty;
                listViewItem.SubItems["Difficulty"].Text = difficulty.ToDifficultyString();

                if (deviceViewModel.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                {
                    const string unit = KnownCoins.BitcoinSymbol;
                    listViewItem.SubItems["Price"].Text = String.Format("{0} {1}", deviceViewModel.Price.ToFriendlyString(), unit);

                    //check .Mining to allow perks for Remoting when local PC is not mining
                    if ((app.MiningEngine.Donating || !app.MiningEngine.Mining) && app.PerksConfiguration.ShowExchangeRates
                    //ensure Exchange prices are available:
                    && (app.SellPrices != null))
                    {
                        ExchangeInformation exchangeInformation = app.SellPrices.Single(er => er.TargetCurrency.Equals(app.GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
                        double btcExchangeRate = exchangeInformation.ExchangeRate;

                        double coinExchangeRate = deviceViewModel.Price * btcExchangeRate;

                        listViewItem.SubItems["Exchange"].Tag = coinExchangeRate;
                        listViewItem.SubItems["Exchange"].Text = String.Format("{0}{1}", exchangeInformation.TargetSymbol, coinExchangeRate.ToFriendlyString(true));
                    }
                }

                switch (app.EngineConfiguration.StrategyConfiguration.ProfitabilityKind)
                {
                    case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                        listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.AdjustedProfitability, 2) + @"%";
                        break;
                    case Strategy.CoinProfitabilityKind.AverageProfitability:
                        listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.AverageProfitability, 2) + @"%";
                        break;
                    case Strategy.CoinProfitabilityKind.StraightProfitability:
                        listViewItem.SubItems["Profitability"].Text = Math.Round(deviceViewModel.Profitability, 2) + @"%";
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
                listViewItem.SubItems["Current"].Text = @"Idle";

            double hashrate = app.WorkUtilityToHashrate(deviceViewModel.WorkUtility);
            listViewItem.SubItems["Effective"].Text = hashrate == 0 ? String.Empty : hashrate.ToHashrateString();

            //check for >= 0.05 so we don't show 0% (due to the format string)
            listViewItem.SubItems["Rejected"].Text = deviceViewModel.RejectedSharesPercent >= 0.05 ? deviceViewModel.RejectedSharesPercent.ToString("0.#") + "%" : String.Empty;
            listViewItem.SubItems["Errors"].Text = deviceViewModel.HardwareErrorsPercent >= 0.05 ? deviceViewModel.HardwareErrorsPercent.ToString("0.#") + "%" : String.Empty;
            listViewItem.SubItems["Accepted"].Text = deviceViewModel.AcceptedShares > 0 ? deviceViewModel.AcceptedShares.ToString() : String.Empty;

            if (app.ApplicationConfiguration.ShowWorkUtility)
                listViewItem.SubItems[utilityColumnHeader.Text].Text = deviceViewModel.WorkUtility > 0.00 ? deviceViewModel.WorkUtility.ToString("0.###") : String.Empty;
            else
                listViewItem.SubItems[utilityColumnHeader.Text].Text = deviceViewModel.Utility > 0.00 ? deviceViewModel.Utility.ToString("0.###") : String.Empty;

            listViewItem.SubItems["Temp"].Text = deviceViewModel.Temperature > 0 ? deviceViewModel.Temperature + "°" : String.Empty;
            listViewItem.SubItems["Fan"].Text = deviceViewModel.FanPercent > 0 ? deviceViewModel.FanPercent + "%" : String.Empty;
            listViewItem.SubItems["Intensity"].Text = deviceViewModel.Intensity;

            RefreshPoolColumnForDeviceViewModel(listViewItem, deviceViewModel);

            PopulateIncomeForListViewItem(listViewItem, deviceViewModel);
        }

        private void RefreshPoolColumnForDeviceViewModel(ListViewItem listViewItem, DeviceViewModel deviceViewModel)
        {
            var pool = deviceViewModel.Pool.DomainFromHost();
            if (app.ApplicationConfiguration.ShowPoolPort)
            {
                var port = deviceViewModel.Url.PortFromHost();
                if (port.HasValue)
                {
                    pool = String.Format("{0}:{1}", pool, port.Value);
                }
            }
            listViewItem.SubItems["Pool"].Text = pool;
        }

        private void RefreshDetailsAreaIfVisible()
        {
            if (!detailsAreaContainer.Panel2Collapsed)
                RefreshDetailsArea();
        }

        private void RefreshDetailsArea()
        {
            MinerFormViewModel viewModelToView = app.GetViewModelToView();

            if (deviceListView.SelectedItems.Count == 0)
            {
                detailsControl1.ClearDetails(viewModelToView.Devices.Count);
                return;
            }

            DeviceViewModel selectedDevice = (DeviceViewModel)deviceListView.SelectedItems[0].Tag;

            detailsControl1.InspectDetails(selectedDevice, app.ApplicationConfiguration.ShowWorkUtility);
        }

        private void RefreshCoinPopupMenu()
        {
            coinPopupMenu.Items.Clear();

            MinerFormViewModel viewModelToView = app.GetViewModelToView();
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
            ToolStripMenuItem algoItem = parent.Cast<ToolStripMenuItem>().FirstOrDefault(item => item.Text.Equals(algorithm));

            if (algoItem == null)
            {
                algoItem = new ToolStripMenuItem()
                {
                    Text = algorithm
                };
                parent.Add(algoItem);
            }

            return algoItem;
        }

        private void RefreshCountdownLabel()
        {
            int countdownSeconds = app.StartupMiningCountdownSeconds;
            bool countingDown = countdownSeconds > 0;

            startupMiningPanel.Visible = countingDown;
            countdownLabel.Visible = countingDown; //or remains visible under Mono
            cancelStartupMiningButton.Visible = countingDown; //or remains visible under Mono

            if (countingDown)
            {
                startupMiningPanel.Left = (Width / 2) - (startupMiningPanel.Width / 2);
                startupMiningPanel.Top = (Height / 2) - (startupMiningPanel.Height / 2);

                countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", countdownSeconds);
                startupMiningPanel.Visible = true;
            }
        }

        private void UpdateUtilityColumnHeader()
        {
            ChangeSubItemText(utilityColumnHeader, app.ApplicationConfiguration.ShowWorkUtility ? "Work Utility" : "Utility");
        }

        private void ChangeSubItemText(ColumnHeader columnHeader, string caption)
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
                menuItem.Checked = !app.ApplicationConfiguration.HiddenColumns.Contains(columnText);

                menuItem.Click += ColumnHeaderMenuClick;
            }
        }

        private const int MaxColumnWidth = 195;
        //optimized for speed        
        private static void SetColumWidth(ColumnHeader column, int width)
        {
            if ((width < 0) || (column.Width != width))
                column.Width = width;
            if (column.Width > MaxColumnWidth)
                column.Width = MaxColumnWidth;
        }

        private void AutoSizeListViewColumns()
        {
            if (app.Context == null)
                return;

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

                        if (app.ApplicationConfiguration.HiddenColumns.Contains(column.Text))
                        {
                            SetColumWidth(column, 0);
                            continue;
                        }

                        bool hasValue = i == 0 || ListViewColumnHasValues(column.Text);

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
            return deviceListView.Items.Cast<ListViewItem>().Any(item => !String.IsNullOrEmpty(item.SubItems[headerText].Text));
        }

        private void RefreshCoinApiLabel()
        {
            coinApiLinkLabel.Text = app.GetEffectiveApiContext().GetApiName();

            PositionCoinChooseLabels();
        }

        private void UpdateMiningButtons()
        {
            if (app.SelectedRemoteInstance == null)
                UpdateMiningButtonsForLocal();
            else
                UpdateMiningButtonsForRemote();
        }

        private void UpdateMiningButtonsForRemote()
        {
            startButton.Enabled = !app.RemoteInstanceMining;

            stopButton.Enabled = app.RemoteInstanceMining;
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;

            //no remote detecting devices (yet)
            detectDevicesButton.Enabled = !app.RemoteInstanceMining;
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
            startButton.Enabled = app.MiningConfigurationValid() && !app.MiningEngine.Mining;// && (app.SelectedRemoteInstance == null);

            stopButton.Enabled = app.MiningEngine.Mining;// && (app.SelectedRemoteInstance == null);
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;
            //allow clicking Detect Devices with invalid configuration
            detectDevicesButton.Enabled = !app.MiningEngine.Mining && (app.SelectedRemoteInstance == null);
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

            MinerFormViewModel viewModelToView = app.GetViewModelToView();
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
            strategiesLabel.Text = app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins ? @" Strategies: enabled" : @" Strategies: disabled";
        }

        private void RefreshStrategiesCountdown()
        {
            //Time until strategy check: 60s
            if (app.MiningEngine.Mining && app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                strategyCountdownLabel.Text = string.Format("Time until strategy check: {0}m", app.CoinStatsCountdownMinutes);
            else
                strategyCountdownLabel.Text = "";
        }

        private void RemoveInvalidCoinValuesFromListView()
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (app.EngineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Enabled && c.PoolGroup.Name.Equals(item.SubItems["Coin"].Text)) == null)
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
            if (!((app.MiningEngine.Donating || !app.MiningEngine.Mining) && app.PerksConfiguration.ShowIncomeRates))
                return;

            if (app.CoinApiInformation == null)
                //no internet or error parsing API
                return;

            if (app.SellPrices == null)
                //no internet or error parsing API
                return;

            CoinInformation info = app.CoinApiInformation
                .ToList() //get a copy - populated async & collection may be modified
                .SingleOrDefault(c => c.Symbol.Equals(deviceViewModel.Coin.Id, StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                ExchangeInformation exchangeInformation = app.SellPrices.Single(er => er.TargetCurrency.Equals(app.GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
                
                if (app.PerksConfiguration.ShowExchangeRates && app.PerksConfiguration.ShowIncomeInUsd)
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
            detailsToggleButton.Text = briefMode ? @"▾ More details" : @"▴ Fewer details";
        }

        private void RefreshIncomeSummary()
        {
            incomeSummaryLabel.Text = app.GetIncomeSummaryText();

            incomeSummaryLabel.AutoSize = true;
            incomeSummaryLabel.Padding = new Padding(0, 11, 17, 0);
        }

        private void RefreshStatusBarFromViewModel()
        {
            deviceTotalLabel.Text = String.Format("{0} device(s)", app.GetVisibleDeviceCount());

            hashRateStatusLabel.Text = app.GetHashRateStatusText();

            hashRateStatusLabel.AutoSize = true;
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

        private void RefreshViewForConfigurationChanges()
        {
            System.Windows.Forms.Application.DoEvents();

            if (app.PerksConfiguration.PerksEnabled && app.PerksConfiguration.ShowExchangeRates)
                app.RefreshExchangeRates();
            
            SetupAccessibleMenu();

            app.SetupRestartTimer();
            app.SetupNetworkRestartTimer();
            app.SetupCoinApi();
            app.CheckForUpdates();
            app.SetupCoinStatsTimer();
            app.SuggestCoinsToMine();
            app.SetGpuEnvironmentVariables();

            SetListViewStyle((View)app.ApplicationConfiguration.ListViewStyle);

            //load brief mode first, then location
            SetBriefMode(app.ApplicationConfiguration.BriefUserInterface);

            //now location so we pick up the customizations
            if ((app.ApplicationConfiguration.AppPosition.Height > 0) &&
                (app.ApplicationConfiguration.AppPosition.Width > 9))
            {
                Location = new Point(app.ApplicationConfiguration.AppPosition.Left, app.ApplicationConfiguration.AppPosition.Top);
                Size = new Size(app.ApplicationConfiguration.AppPosition.Width, app.ApplicationConfiguration.AppPosition.Height);
            }

            if (app.ApplicationConfiguration.Maximized && (WindowState != FormWindowState.Minimized))
                WindowState = FormWindowState.Maximized;
            
            RefreshViewFromViewModel();
            
            System.Windows.Forms.Application.DoEvents();
        }
        #endregion

        #region Settings logic
        private void LoadSettings()
        {
            app.LoadSettings();

            RefreshViewForConfigurationChanges();
        }
                
        private void SaveSettings()
        {
            SavePosition();
            app.ApplicationConfiguration.DetailsAreaWidth = detailsAreaContainer.Width - detailsAreaContainer.SplitterDistance;
            app.ApplicationConfiguration.InstancesAreaWidth = instancesContainer.SplitterDistance;

            app.ApplicationConfiguration.SaveApplicationConfiguration();
        }

        private void SavePosition()
        {
			//don't save position if we haven't loaded settings
			//SetBriefMode() may trigger this and overwrite customizations
			if (applicationSetup &&
				//don't save position if Maximized
				(WindowState == FormWindowState.Normal))
				app.ApplicationConfiguration.AppPosition = new Rectangle(Location, Size);
        }
        #endregion

        #region UI event handlers
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
        
        private void MinerForm_Move(object sender, EventArgs e)
        {
            //position may be restored at any time in RefreshViewForConfigurationChanges()
            //ensure the position is updated in ApplicationConfiguration
            SavePosition();
        }

        private void deviceListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.A | Keys.Control))
            {
                //CTRL+A to select all devices
                SelectAllDevices();
                e.Handled = true;
            }
            else if (deviceListView.SelectedItems.Count == deviceListView.Items.Count)
            {
                //allow selecting groups
                //e.g. CTRL+A, N for Network
                char keyChar = (char)e.KeyValue;
                e.Handled = SelectDeviceGroup(keyChar.ToString());
            }
            keyPressHandled = e.Handled;
        }

        private void deviceListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = keyPressHandled;
            keyPressHandled = false;
        }

        private bool SelectDeviceGroup(string prefix)
        {
            bool result = false;
            foreach (ListViewItem listViewItem in deviceListView.Items)
            {
                listViewItem.Selected = listViewItem.Group.Header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
                if (listViewItem.Selected)
                    result = true;
            }
            return result;
        }

        private void SelectAllDevices()
        {
            foreach (ListViewItem item in deviceListView.Items)
                item.Selected = true;
        }

        private void CoinMenuItemClick(object sender, EventArgs e)
        {
            ToolStripItem menuItem = (ToolStripItem)sender;
            string coinSymbol = menuItem.Text;
            List<DeviceDescriptor> descriptors = deviceListView.SelectedItems.Cast<ListViewItem>()
                .Select(listViewItem => (DeviceViewModel) listViewItem.Tag).Cast<DeviceDescriptor>().ToList();

            app.SetDevicesToCoin(descriptors, coinSymbol);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            app.SaveChanges();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            app.CancelChanges();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            app.StopMining();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            HandleStartButtonClicked();
        }

        private void HandleStartButtonClicked()
        {
            if ((app.SelectedRemoteInstance == null) && app.ApplicationConfiguration.AutoSetDesktopMode)
                app.ToggleDynamicIntensityLocally(true);

            app.StartMining();
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            app.CancelMiningOnStartup();
        }

        private void detectDevicesButton_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }

        private void coinChooseLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(app.GetEffectiveApiContext().GetInfoUrl());
        }

        private void advancedToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            MinerFormViewModel viewModel = app.GetViewModelToView();

            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchToolStripMenuItem.Enabled = viewModel.ConfiguredCoins.Any();
            //
            dynamicIntensityToolStripMenuItem.Visible = !app.EngineConfiguration.XgminerConfiguration.DisableGpu;
            dynamicIntensityToolStripMenuItem.Checked = viewModel.DynamicIntensity;
            dynamicIntensityMenuSeperator.Visible = !app.EngineConfiguration.XgminerConfiguration.DisableGpu;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        private void scanHardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }
        
        private void dynamicIntensityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.ToggleDynamicIntensity(dynamicIntensityToolStripMenuItem.Checked);
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
            app.ConfigureSettings();
        }

        private void coinsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            app.ConfigurePools();
        }

        private void strategiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            app.ConfigureStrategies();
        }

        private void perksToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            app.ConfigurePerks();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.StartMining();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.StopMining();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.RestartMining();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.SaveChanges();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.CancelChanges();
        }

        private void quickSwitchToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshQuickSwitchMenu(quickSwitchToolStripMenuItem);
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogLaunchArgs args = (LogLaunchArgs)logLaunchArgsBindingSource.Current;

            LaunchLoggedMiner(args);
        }

        private static void LaunchLoggedMiner(LogLaunchArgs args)
        {
            string arguments = args.Arguments;
            arguments = arguments.Replace("-T -q", String.Empty).Trim();

            ProcessStartInfo startInfo = new ProcessStartInfo(args.ExecutablePath, arguments)
            {
                WorkingDirectory = Path.GetDirectoryName(args.ExecutablePath)
            };
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
                    processLogGridView.CurrentCell = clickedCell;  // Select the clicked cell, for instance

                    // Get mouse position relative to the grid
                    Point relativeMousePosition = processLogGridView.PointToClient(Cursor.Position);

                    // Show the context menu
                    processLogMenu.Show(processLogGridView, relativeMousePosition);
                }
            }
        }

        private void processLogGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Get mouse position relative to the grid
                Point relativeMousePosition = processLogGridView.PointToClient(Cursor.Position);

                // Show the context menu
                openLogMenu.Show(processLogGridView, relativeMousePosition);
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
            if (apiMonitorSideButton.Checked)
                fileName = "ApiLog.json";
            else if (processLogSideButton.Checked)
                fileName = "ProcessLog.json";
            else if (historySideButton.Checked)
                fileName = "MiningLog.json";

            OpenLogFile(fileName);
        }

        private void OpenLogFile(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                string logDirectory = app.GetLogDirectory();
                string logFilePath = Path.Combine(logDirectory, fileName);
                try
                {
                    // attempt to open with the app associated with .json
                    Process.Start(logFilePath);
                } catch (Win32Exception)
                {
                    // no app associated for .json - use Notepad (we're on Windows if we got this exception)
                    Process.Start("notepad", logFilePath);
                }
            }
        }

        private void deviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deviceListView.SelectedItems.Count > 0)
                RefreshDetailsArea();
        }

        private void listViewStyleButton_ButtonClick(object sender, EventArgs e)
        {
            RotateListViewStyle();
        }

        private void RotateListViewStyle()
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
            app.RestartMining();
        }

        private void perksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.ConfigurePerks();
        }

        private void perksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            app.ConfigurePerks();
        }

        private void deviceListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == incomeColumnHeader.Index)
            {
                using (new HourGlass())
                {
                    app.PerksConfiguration.ShowIncomeInUsd = !app.PerksConfiguration.ShowIncomeInUsd;
                    app.RefreshAllDeviceStats();
                    app.PerksConfiguration.SavePerksConfiguration();
                    AutoSizeListViewColumns();
                }
            }
            else if (e.Column == utilityColumnHeader.Index)
            {
                app.ApplicationConfiguration.ShowWorkUtility = !app.ApplicationConfiguration.ShowWorkUtility;
                app.ApplicationConfiguration.SaveApplicationConfiguration();

                UpdateUtilityColumnHeader();

                app.RefreshAllDeviceStats();
                AutoSizeListViewColumns();
                RefreshDetailsAreaIfVisible();
            }
            else if (e.Column == poolColumnHeader.Index)
            {
                app.ApplicationConfiguration.ShowPoolPort = !app.ApplicationConfiguration.ShowPoolPort;
                app.ApplicationConfiguration.SaveApplicationConfiguration();
                
                app.RefreshAllDeviceStats();
                AutoSizeListViewColumns();
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
            if (!headerRect.Contains(deviceListView.PointToClient(MousePosition)))
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
                    deviceListContextMenu.Show(Cursor.Position);
        }

        private void detectDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanHardware();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.ConfigureSettings();
        }

        private void coinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.ConfigurePools();
        }

        private void strategiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            app.ConfigureStrategies();
        }

        private void deviceListContextMenu_Opening(object sender, CancelEventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all

            quickSwitchPopupItem.Enabled = app.GetViewModelToView().ConfiguredCoins.Any();
        }

        private void quickSwitchPopupItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshQuickSwitchMenu(quickSwitchPopupItem);
        }

        private bool briefMode;
        private void detailsToggleButton_ButtonClick(object sender, EventArgs e)
        {
            SetBriefMode(!briefMode, true);
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
            if ((e.Button == MouseButtons.Right) && deviceListView.FocusedItem.Bounds.Contains(e.Location))
            {
                if (deviceListView.FocusedItem.Group.Name.Equals("networkListViewGroup"))
                {
                    UpdateNetworkDeviceMenu();
                    networkDeviceContextMenu.Show(Cursor.Position);
                }
                else
                {
                    ShowCoinPopupMenu();
                }
            }
        }

        private void ShowCoinPopupMenu()
        {
            ShowCoinPopupMenu(Cursor.Position);
        }

        private void ShowCoinPopupMenu(Control control, Point position)
        {
            SetupCoinPopupMenu();
            coinPopupMenu.Show(control, position);
        }

        private void ShowCoinPopupMenu(Point screenLocation)
        {
            SetupCoinPopupMenu();
            coinPopupMenu.Show(screenLocation);
        }

        private void SetupCoinPopupMenu()
        {
            string currentCoin = GetCurrentlySelectedCoinName();
            RefreshCoinPopupMenu();
            CheckCoinInPopupMenu(currentCoin);
        }

        private string GetCurrentlySelectedCoinName()
        {
            return deviceListView.FocusedItem.SubItems["Coin"].Text;
        }

        private void deviceListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (updatingListView)
                return;

            bool enabled = e.Item.Checked;
            List<DeviceDescriptor> descriptors = new List<DeviceDescriptor>();
            DeviceViewModel device = (DeviceViewModel)e.Item.Tag;
            DeviceDescriptor descriptor = new DeviceDescriptor();
            ObjectCopier.CopyObject(device, descriptor);
            descriptors.Add(descriptor);

            app.ToggleDevices(descriptors, enabled);
        }

        private void dynamicIntensityButton_Click(object sender, EventArgs e)
        {
            app.ToggleDynamicIntensity(dynamicIntensityButton.Checked);
        }

        private void settingsButton_ButtonClick(object sender, EventArgs e)
        {
            app.ConfigureSettings();
        }

        private void coinsButton_Click_1(object sender, EventArgs e)
        {
            app.ConfigurePools();
        }

        private void strategiesButton_Click_1(object sender, EventArgs e)
        {
            app.ConfigureStrategies();
        }

        private void advancedMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchItem.Enabled = app.EngineConfiguration.CoinConfigurations.Any(c => c.Enabled);

            dynamicIntensityButton.Visible = !app.EngineConfiguration.XgminerConfiguration.DisableGpu;
            dynamicIntensityButton.Checked = app.GetViewModelToView().DynamicIntensity;
            dynamicIntensitySeparator.Visible = !app.EngineConfiguration.XgminerConfiguration.DisableGpu;
        }

        private void notificationsControl1_NotificationsChanged(object sender)
        {
            notificationsControl.Visible = notificationsControl.NotificationCount() > 0;
            if (notificationsControl.Visible)
                notificationsControl.BringToFront();
        }

        private void historyGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //do unbound data in RowsAdded or it won't show until the DataGridView has been on-screen
            for (int i = 0; i < e.RowCount; i++)
            {
                int index = e.RowIndex + i;

                LogProcessCloseArgs ea = app.LogCloseEntries[index];

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
            app.LogNotificationToFile(text);
            app.QueueMobileMinerNotification(text, kind);
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
                if (app.ApplicationConfiguration.MinimizeToNotificationArea && (WindowState == FormWindowState.Minimized))
                {
                    notifyIcon1.Visible = true;
                    Hide();
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    notifyIcon1.Visible = false;
                }

                //handling for saving Maximized state
                if (WindowState == FormWindowState.Maximized)
                    app.ApplicationConfiguration.Maximized = true;

                if (WindowState == FormWindowState.Normal) //don't set to false for minimizing
                    app.ApplicationConfiguration.Maximized = false;

                SavePosition();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void showAppMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
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
            app.StopMining();
        }

        private void ColumnHeaderMenuClick(object sender, EventArgs e)
        {
            using (new HourGlass())
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                menuItem.Checked = !menuItem.Checked;
                if (menuItem.Checked)
                    app.ApplicationConfiguration.HiddenColumns.Remove(menuItem.Text);
                else
                    app.ApplicationConfiguration.HiddenColumns.Add(menuItem.Text);
                app.ApplicationConfiguration.SaveApplicationConfiguration();
                AutoSizeListViewColumns();
            }
        }

        private void HandleQuickSwitchClick(object sender, EventArgs e)
        {
            string coinSymbol = (string)((ToolStripMenuItem)sender).Tag;

            bool allRigs = ShouldQuickSwitchAllRigs(coinSymbol);

            bool disableStrategies = ShouldDisableStrategies();

            app.SetAllDevicesToCoin(coinSymbol, disableStrategies);

            if (allRigs)
                app.SetAllDevicesToCoinOnAllRigs(coinSymbol, disableStrategies);
        }

        private bool ShouldDisableStrategies()
        {
            bool disableStrategies = false;
            if (app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
            {
                DialogResult dialogResult = MessageBox.Show(
                    @"Would you like to disable Auto-Mining Strategies after switching coins?",
                    @"Quick Switch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                    disableStrategies = true;
            }
            return disableStrategies;
        }

        private bool ShouldQuickSwitchAllRigs(string coinSymbol)
        {
            bool allRigs = false;
            if (app.RemotingEnabled && (app.InstanceManager.Instances.Count > 1))
            {
                DialogResult dialogResult = MessageBox.Show(
                    String.Format("Would you like to Quick Switch to {0} on all of your online rigs?", coinSymbol),
                    @"MultiMiner Remoting", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
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
            if (updatingListView)
                return;

            //disallow toggling check-state for Network Devices
            DeviceViewModel selectedDevice = (DeviceViewModel)deviceListView.Items[e.Index].Tag;
            if (selectedDevice.Kind == DeviceKind.NET)
                e.NewValue = CheckState.Checked;
        }

        private void deviceListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            editingDeviceListView = true;
        }

        private void deviceListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            editingDeviceListView = false; //no returns before this

            if (e.Label == null)
                //edit was canceled
                return;

            ListViewItem editItem = deviceListView.Items[e.Item];
            DeviceViewModel deviceViewModel = (DeviceViewModel)editItem.Tag;

            app.RenameDevice(deviceViewModel, e.Label);
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
            app.ConfigureSettings();
        }

        private void poolsPlainButton_Click(object sender, EventArgs e)
        {
            app.ConfigurePools();
        }

        private void strategiesPlainButton_Click(object sender, EventArgs e)
        {
            app.ConfigureStrategies();
        }

        private void perksPlainButton_Click(object sender, EventArgs e)
        {
            app.ConfigurePerks();
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

        private void homeButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            ShowDeviceListView();
        }

        private void dashboardButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            //ShowWebBrowser(WebBrowserProvider.DashboardController);
        }

        private void metricsButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            //ShowWebBrowser(WebBrowserProvider.HistoryController);
        }

        private void apiConsoleSideButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            ShowApiConsole();
        }

        private void historySideButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            ShowHistory();
        }

        private void apiMonitorSideButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            ShowApiMonitor();
        }

        private void processLogSideButton_Click(object sender, EventArgs e)
        {
            ToggleSideBarButtons(sender);

            ShowProcessLog();
        }

        private void stickyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleNetworkDeviceSticky();
        }

        private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleNetworkDeviceHidden();
        }

        private void rebootDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RebootSelectedNetworkDevices();
        }

        private void executeCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteCommandOnSelectedNetworkDevices();
        }

        private void restartMiningToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RestartSelectedNetworkDevices();
        }

        private void startMiningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartSelectedNetworkDevices();
        }

        private void stopMiningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopSelectedNetworkDevices();
        }

        private void instancesControl1_SelectedInstanceChanged(object sender, Instance instance)
        {
            app.SelectRemoteInstance(instance);
        }

        private void refreshViewTimer_Tick(object sender, EventArgs e)
        {
            if (refreshViewRequested)
            {
                refreshViewRequested = false; //set before refreshing the View
                DoRefreshViewFromViewModel();
            }
        }
        #endregion
        
        #region Application startup / setup
        protected override void OnHandleCreated(EventArgs e)
        {
            app.Context = this;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            app.Context = null;
            base.OnHandleDestroyed(e);
        }

        private void SetupApplication()
        {
            SetupApplicationViewModel();

            HideAdvancedPanel();

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

            app.LoadPreviousHistory();

            CloseDetailsArea();

            FetchInitialCoinStats();

            //start updating the view
            refreshViewTimer.Interval = 500;
            refreshViewTimer.Enabled = true;

            app.UpdateBackendMinerAvailability(); //before CheckAndShowGettingStarted()
            CheckAndShowGettingStarted();

            LoadSettings();
            
            SetupDataBinding();

            app.SetupMiningEngineEvents();

            app.SetHasChangesLocally(false);

            //kill known owned processes to release inherited socket handles
            if (ApplicationViewModel.KillOwnedProcesses())
                //otherwise may still be prompted below by check for disowned miners
                Thread.Sleep(500);

            //check for disowned miners before refreshing devices
            if (app.ApplicationConfiguration.DetectDisownedMiners)
                app.CheckForDisownedMiners();

            app.SetupRemoting();
            
            app.CheckAndDownloadMiners();

            app.CheckForUpdates();

            app.SetHasChangesLocally(false);

            //ONLY if null, e.g. first launch or no XML
            //don't keep scanning on startup if there are no devices
            //maybe just using to monitor network devices
            if (app.Devices == null)
                ScanHardwareLocally();

            //after refreshing devices
            //app.SubmitMultiMinerStatistics();

            //scan for Network Devices after scanning for local hardware
            //makes more sense visually
            app.SetupNetworkDeviceDetection();

            //may need to do this if XML files became corrupt
            app.AddMissingDeviceConfigurations();
            
            if (deviceListView.Items.Count > 0)
            {
                deviceListView.Items[0].Selected = true;
                deviceListView.Items[0].Focused = true;
            }
            
            SetupAccessibleMenu();

            ShowStartupTips();

            //do this last as it can take a few seconds
            app.SetGpuEnvironmentVariables();

            //do this after all other data has loaded to prevent errors when the delay is set very low (1s)
            app.SetupMiningOnStartup();
            if (!ApplicationViewModel.MinerIsInstalled(MinerFactory.Instance.GetDefaultMiner()))
                app.CancelMiningOnStartup();
            if (!app.MiningConfigurationValid())
                app.CancelMiningOnStartup();

            app.SetupCoalescedTimers();

            //app.SubmitMobileMinerPools();

            RefreshViewFromViewModel();

            //so we know when culture changes
            SystemEvents.UserPreferenceChanged += SystemEventsUserPreferenceChanged;

            applicationSetup = true;
        }

        private void SetupDataBinding()
        {
            apiLogEntryBindingSource.DataSource = app.ApiLogEntries;
            app.ApiLogEntries.ListChanged += (object sender, ListChangedEventArgs e) =>
            {
                BeginInvoke((Action)(() =>
                {
                    apiLogEntryBindingSource.Position = e.NewIndex;
                }));
            };
            apiLogEntryBindingSource.MoveLast();

            logLaunchArgsBindingSource.DataSource = app.LogLaunchEntries;
            app.LogLaunchEntries.ListChanged += (object sender, ListChangedEventArgs e) =>
            {
                BeginInvoke((Action)(() =>
                {
                    logLaunchArgsBindingSource.Position = e.NewIndex;
                }));
            };
            logLaunchArgsBindingSource.MoveLast();

            logProcessCloseArgsBindingSource.DataSource = app.LogCloseEntries;
            app.LogCloseEntries.ListChanged += (object sender, ListChangedEventArgs e) =>
            {
                BeginInvoke((Action)(() =>
                {
                    logProcessCloseArgsBindingSource.Position = e.NewIndex;
                }));
            };
            logProcessCloseArgsBindingSource.MoveLast();
        }

        private void SetupApplicationViewModel()
        {
            app.NotificationReceived += HandleApplicationNotification;
            app.NotificationDismissed += HandleNotificationDismissed;
            app.CredentialsRequested += HandleCredentialsRequested;
            //app.MobileMinerAuthFailed += HandleMobileMinerAuthFailed;
            app.DataModified += HandleAppViewModelModified;
            app.ProgressStarted += HandleProgressStarted;
            app.ProgressCompleted += HandleProgressCompleted;
            app.ConfigurationModified += HandleConfigurationModified;
            app.OnConfigurePerks += HandleConfigurePerks;
            app.OnConfigurePools += HandleConfigurePools;
            app.OnConfigureSettings += HandleConfigureSettings;
            app.OnConfigureStrategies += HandleConfigureStrategies;
            app.RemoteInstanceRegistered += HandleRemoteInstanceRegistered;
            app.RemoteInstanceUnregistered += HandleRemoteInstanceUnregistered;
            app.RemoteInstanceModified += HandleRemoteInstanceModified;
            app.RemoteInstancesUnregistered += HandleRemoteInstancesUnregistered;

            app.PromptReceived += (object sender, PromptEventArgs e) =>
            {
                e.Result = (PromptResult)MessageBox.Show(e.Text, e.Caption, (MessageBoxButtons)e.Buttons, (MessageBoxIcon)e.Icon);
            };
        }
        
        private void HandleRemoteInstancesUnregistered(object sender, EventArgs e)
        {
            instancesControl.UnregisterInstances();
        }

        private void HandleRemoteInstanceModified(object sender, RemotingEventArgs ea)
        {
            instancesControl.ApplyMachineInformation(ea.IpAddress, ea.Machine);
        }

        private void HandleRemoteInstanceUnregistered(object sender, InstanceChangedArgs ea)
        {
            instancesControl.UnregisterInstance(ea.Instance);
        }

        private void HandleRemoteInstanceRegistered(object sender, InstanceChangedArgs ea)
        {
            instancesControl.RegisterInstance(ea.Instance);
        }

        private void HandleConfigureStrategies(object sender, ConfigurationEventArgs e)
        {
            StrategiesForm strategiesForm = new StrategiesForm(e.Engine.StrategyConfiguration, app.KnownCoins, e.Application);
            if (app.SelectedRemoteInstance != null)
                strategiesForm.Text = String.Format("{0}: {1}", strategiesForm.Text, app.SelectedRemoteInstance.MachineName);

            e.ConfigurationModified = strategiesForm.ShowDialog() == DialogResult.OK;
        }

        private void HandleConfigureSettings(object sender, ConfigurationEventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(e.Application, e.Engine.XgminerConfiguration, e.Paths, e.Perks);
            if (app.SelectedRemoteInstance != null)
                settingsForm.Text = String.Format("{0}: {1}", settingsForm.Text, app.SelectedRemoteInstance.MachineName);

            e.ConfigurationModified = settingsForm.ShowDialog() == DialogResult.OK;
        }

        private void HandleConfigurePools(object sender, ConfigurationEventArgs e)
        {
            PoolsForm coinsForm = new PoolsForm(e.Engine.CoinConfigurations, app.KnownCoins, e.Application, e.Perks);
            if (app.SelectedRemoteInstance != null)
                coinsForm.Text = String.Format("{0}: {1}", coinsForm.Text, app.SelectedRemoteInstance.MachineName);

            e.ConfigurationModified = coinsForm.ShowDialog() == DialogResult.OK;
        }

        private void HandleConfigurePerks(object sender, ConfigurationEventArgs e)
        {
            PerksForm perksForm = new PerksForm(e.Perks);
            if (app.SelectedRemoteInstance != null)
                perksForm.Text = String.Format("{0}: {1}", perksForm.Text, app.SelectedRemoteInstance.MachineName);

            e.ConfigurationModified = perksForm.ShowDialog() == DialogResult.OK;
        }

        private void HandleConfigurationModified(object sender, EventArgs e)
        {
            RefreshViewForConfigurationChanges();
        }

        private void HandleNotificationDismissed(object sender, NotificationEventArgs ea)
        {
            notificationsControl.RemoveNotification(ea.Id);
        }

        private ProgressForm progressForm;
        private void HandleProgressCompleted(object sender, EventArgs e)
        {
            progressForm.Close();
            progressForm.Dispose();
            progressForm = null;
        }

        private void HandleProgressStarted(object sender, ProgressEventArgs ea)
        {
            progressForm = new ProgressForm(ea.Text) { IsDownload = ea.IsDownload };
            progressForm.Show(); //not ShowDialog()

            //for Mono - show the UI
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(25);
            System.Windows.Forms.Application.DoEvents();
        }

        private void HandleAppViewModelModified(object sender, EventArgs e)
        {            
            RefreshViewFromViewModel();
        }

        //private void HandleMobileMinerAuthFailed(object sender, EventArgs e)
        //{
        //    MessageBox.Show(String.Format(@"Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog.{0}{0}MobileMiner remote commands will now be disabled.", Environment.NewLine), 
        //        @"Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    //check to make sure there are no modal windows already
        //    if (!ShowingModalDialog())
        //    {
        //        BeginInvoke((Action)(() =>
        //        {
        //            //code to update UI
        //            app.ConfigureSettingsLocally();
        //        }));
        //    }
        //}

        private static bool ShowingModalDialog()
        {
            return System.Windows.Forms.Application.OpenForms.Cast<Form>().Any(f => f.Modal);
        }

        private void HandleCredentialsRequested(object sender, CredentialsEventArgs ea)
        {
            CredentialsForm form = new CredentialsForm(ea.ProtectedResource, ea.Username, ea.Password);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ea.Username = form.Username;
                ea.Password = form.Password;
                ea.CredentialsProvided = true;
            }
        }

        private void HandleApplicationNotification(object sender, NotificationEventArgs ea)
        {
            BeginInvoke((Action)(() =>
            {
                //code to update UI
                PostNotification(ea.Id, ea.Text, ea.ClickHandler, ea.Kind, ea.InformationUrl);
            }));
        }

        private void SystemEventsUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            //culture settings changed
            if (e.Category == UserPreferenceCategory.Locale)
                //clear cached currency
                CultureInfo.CurrentCulture.ClearCachedData();
        }

        private void TearDownApplication()
        {
            app.Context = null;
            SaveSettings();
            app.StopMiningLocally();
            app.DisableRemoting();
        }

        private void HandleStartupMinimizedToNotificationArea()
        {
            if (app.ApplicationConfiguration.StartupMinimized && app.ApplicationConfiguration.MinimizeToNotificationArea)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
        }

        private void ShowStartupTips()
        {
            string tip;

            switch (app.ApplicationConfiguration.TipsShown)
            {
                case 0:
                    tip = "Tip: right-click device names to change coins";
                    PostNotification(tip, () =>
                    {
                        //only if we have non-network devices
                        if (app.LocalViewModel.Devices.Count(d => d.Kind != DeviceKind.NET) > 0)
                        {
                            string currentCoin = GetCurrentlySelectedCoinName();
                            CheckCoinInPopupMenu(currentCoin);

                            ListViewItem firstItem = deviceListView.Items[0];
                            Point popupPosition = firstItem.Position;
                            popupPosition.Offset(14, 6);
                            ShowCoinPopupMenu(deviceListView, popupPosition);
                        }
                    }, NotificationKind.Information);
                    app.ApplicationConfiguration.TipsShown++;
                    break;
                case 1:
                    tip = "Tip: right-click the main screen for common tasks";
                    PostNotification(tip, () =>
                    {
                        deviceListContextMenu.Show(deviceListView, 150, 100);
                    }, NotificationKind.Information);
                    app.ApplicationConfiguration.TipsShown++;
                    break;
                case 2:
                    tip = "Tip: restart mining after changing any settings";
                    PostNotification(tip, () =>
                    {
                    }, NotificationKind.Information);
                    app.ApplicationConfiguration.TipsShown++;
                    break;
                case 3:
                    tip = "Tip: enabling perks gives back to the author";
                    PostNotification(tip, () =>
                    {
                        app.ConfigurePerksLocally();
                    }, NotificationKind.Information);
                    app.ApplicationConfiguration.TipsShown++;
                    break;
            }

            app.ApplicationConfiguration.SaveApplicationConfiguration();
        }
        
        private void FetchInitialCoinStats()
        {
            app.EngineConfiguration.LoadStrategyConfiguration(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            app.EngineConfiguration.LoadCoinConfigurations(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            //already done in ctor app.ApplicationConfiguration.LoadApplicationConfiguration(); //needed before refreshing coins
            SetupNotificationsControl(); //needed before refreshing coins
            app.SetupCoinApi(); //so we target the correct API
            app.RefreshCoinStats();
        }

        private void CheckAndShowGettingStarted()
        {
            //only show if there's no settings yet
            if (File.Exists(app.ApplicationConfiguration.ApplicationConfigurationFileName()))
                return;

            WizardForm wizardForm = new WizardForm(app.KnownCoins);
            DialogResult dialogResult = wizardForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Engine.Data.Configuration.Engine newEngineConfiguration;
                UX.Data.Configuration.Application newApplicationConfiguration;
                Perks newPerksConfiguration;
                wizardForm.CreateConfigurations(out newEngineConfiguration, out newApplicationConfiguration, out newPerksConfiguration);

                ObjectCopier.CopyObject(newEngineConfiguration, app.EngineConfiguration);
                ObjectCopier.CopyObject(newApplicationConfiguration, app.ApplicationConfiguration);
                ObjectCopier.CopyObject(newPerksConfiguration, app.PerksConfiguration);

                app.EngineConfiguration.SaveCoinConfigurations();
                app.EngineConfiguration.SaveMinerConfiguration();
                app.ApplicationConfiguration.SaveApplicationConfiguration();
                app.PerksConfiguration.SavePerksConfiguration();

                SetBriefMode(app.ApplicationConfiguration.BriefUserInterface, true);
            }
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
                    "Would you like to download and install {0} now?", minerName), @"Miner Not Found",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (dialogResult == DialogResult.Yes)
                {
                    app.InstallBackendMinerLocally(miner);
                    ScanHardwareLocally();
                    showWarning = false;
                }
            }

            if (showWarning)
                MessageBox.Show(@"No copy of " + MinerNames.BFGMiner + @" was detected. Please go to https://github.com/nwoolls/multiminer for instructions on installing " + MinerNames.BFGMiner + @".",
                        @"Miner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region Application logic
        private void SetBriefMode(bool newBriefMode, bool fromUI = false)
        {
            briefMode = newBriefMode;
            RefreshDetailsToggleButton();

            //do this before adjusting the window size so we can base it on column widths
            AutoSizeListViewColumns();

            if (briefMode)
            {
                ShowDeviceListView();

                CloseDetailsArea();
                HideAdvancedPanel();

                if (fromUI)
                {
                    WindowState = FormWindowState.Normal;

                    int newWidth = deviceListView.Columns.Cast<ColumnHeader>().Sum(column => column.Width);
                    newWidth += 40; //needs to be pretty wide for e.g. Aero Basic
                    newWidth = Math.Max(newWidth, 300);
                    //don't (automatically) set the width to crop notifications
                    newWidth = Math.Max(newWidth, notificationsControl.Width + 24);

                    Size = new Size(newWidth, 400);
                }
            }
            else
            {
                if (fromUI && (WindowState != FormWindowState.Maximized))
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

            sideToolStrip.Visible = !briefMode;
            strategiesLabel.Visible = !briefMode;
            strategyCountdownLabel.Visible = !briefMode;
            deviceTotalLabel.Visible = !briefMode;

            advancedMenuItem.Visible = !briefMode;
            //don't hide settings - the wizard talks about the button
            //settingsButton.Visible = !briefMode;
            //settingsSeparator.Visible = !briefMode;

            footerPanel.Visible = !briefMode;

            app.ApplicationConfiguration.BriefUserInterface = briefMode;
            app.ApplicationConfiguration.SaveApplicationConfiguration();
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

                app.ApplicationConfiguration.ListViewStyle = (ListViewStyle)view;

                RefreshListViewFromViewModel();
            }
            finally
            {
                updatingListView = false;
            }
        }

        private void CloseDetailsArea()
        {
            detailsAreaContainer.Panel2.Hide();
            detailsAreaContainer.Panel2Collapsed = true;
        }

        private void ShowDetailsArea()
        {
            SetBriefMode(false);
            RefreshDetailsArea();

            if (!detailsAreaSetup && (app.ApplicationConfiguration.DetailsAreaWidth > 0))
            {
                detailsAreaContainer.SplitterDistance = detailsAreaContainer.Width - app.ApplicationConfiguration.DetailsAreaWidth;
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
        }
        
        private void ShowApiMonitor()
        {
            apiLogGridView.BorderStyle = BorderStyle.None;
            apiLogGridView.Dock = DockStyle.Fill;
            apiLogGridView.Parent = this;
            apiLogGridView.BringToFront();
            apiLogGridView.Show();
        }

        private void ShowProcessLog()
        {
            processLogGridView.BorderStyle = BorderStyle.None;
            processLogGridView.Dock = DockStyle.Fill;
            processLogGridView.Parent = this;
            processLogGridView.BringToFront();
            processLogGridView.Show();
        }

        private void ShowHistory()
        {
            historyGridView.BorderStyle = BorderStyle.None;
            historyGridView.Dock = DockStyle.Fill;
            historyGridView.Parent = this;
            historyGridView.BringToFront();
            historyGridView.Show();
        }

        private static void ShowAboutDialog()
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
        
        private void ShowApiConsole()
        {
            if (apiConsoleControl == null)
            {
                //call ToList() so we can get a copy - otherwise risk:
                //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
                List<MinerProcess> minerProcesses = app.MiningEngine.MinerProcesses.ToList();
                List<NetworkDevices.NetworkDevice> networkDevices = app.NetworkDevicesConfiguration.Devices.ToList();
                apiConsoleControl = new ApiConsoleControl(minerProcesses, networkDevices, app.LocalViewModel);
            }

            apiConsoleControl.Dock = DockStyle.Fill;
            apiConsoleControl.Parent = this;
            apiConsoleControl.BringToFront();
            apiConsoleControl.Show();
        }

        private void ShowDeviceListView()
        {
            advancedAreaContainer.Visible = true;
            advancedAreaContainer.BringToFront();
        }

        //private void ShowWebBrowser(string controller)
        //{
        //    WebBrowser embeddedBrowser = WebBrowserProvider.GetWebBrowser(
        //        controller,
        //        app.ApplicationConfiguration.MobileMinerEmailAddress,
        //        app.ApplicationConfiguration.MobileMinerApplicationKey);

        //    ShowEmbeddedBrowser(embeddedBrowser);
        //}

        private void ShowEmbeddedBrowser(WebBrowser embeddedBrowser)
        {
            embeddedBrowser.Dock = DockStyle.Fill;
            embeddedBrowser.IsWebBrowserContextMenuEnabled = false;
            embeddedBrowser.Parent = this;
            embeddedBrowser.Visible = true;
            embeddedBrowser.BringToFront();
            advancedAreaContainer.Visible = false;
        }

        private void ToggleSideBarButtons(object sender)
        {
            foreach (ToolStripButton item in sideToolStrip.Items)
                item.Checked = false;
            ((ToolStripButton)sender).Checked = true;
        }
        #endregion

        #region Mining logic
        private void PostNotification(string text, Action clickHandler, NotificationKind icon, string informationUrl = "")
        {
            PostNotification(text, text, clickHandler, icon, informationUrl);
        }

        private void PostNotification(string id, string text, Action clickHandler, NotificationKind kind, string informationUrl = "")
        {
            ToolTipIcon icon = ToolTipIcon.Info;
            switch (kind)
            {
                case NotificationKind.Default:
                    icon = ToolTipIcon.None;
                    break;
                case NotificationKind.Information:
                    icon = ToolTipIcon.Info;
                    break;
                case NotificationKind.Warning:
                    icon = ToolTipIcon.Warning;
                    break;
                case NotificationKind.Danger:
                    icon = ToolTipIcon.Error;
                    break;
            }

            notificationsControl.AddNotification(id, text, clickHandler, kind, informationUrl);

            if (notifyIcon1.Visible)
                ShowBalloonNotification(text, clickHandler, icon);
        }

        private void ShowBalloonNotification(string text, Action clickHandler, ToolTipIcon icon)
        {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.BalloonTipTitle = @"MultiMiner";
            notifyIcon1.BalloonTipIcon = icon;

            notificationClickHandler = clickHandler;

            notifyIcon1.ShowBalloonTip(1000); // ms
        }

        private bool updatingListView;
        private void ScanHardware()
        {
            if (app.SelectedRemoteInstance == null)
                ScanHardwareLocally();
            else
                app.ScanHardwareRemotely(app.SelectedRemoteInstance);
        }

        private void ScanHardwareLocally()
        {
            if (app.MiningEngine.Mining)
                return;

            using (new HourGlass())
            {
                updatingListView = true;
                try
                {
                    if (app.ScanHardwareLocally())
                    {
                        RefreshListViewFromViewModel();
                        RefreshDetailsAreaIfVisible();
                        RefreshStatusBarFromViewModel();

                        //it may not be possible to mine after discovering devices
                        UpdateMiningButtons();
                    }
                    else
                    {
                        ShowNotInstalledMinerWarning();
                    }
                }
                finally
                {
                    updatingListView = false;
                }
            }
        }
        #endregion

        #region Network Devices
        private void StartSelectedNetworkDevices()
        {
            foreach (ListViewItem item in deviceListView.SelectedItems)
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel)item.Tag;
                if (deviceViewModel.Kind == DeviceKind.NET)
                    app.StartNetworkDevice(deviceViewModel);
            }
        }

        private void StopSelectedNetworkDevices()
        {
            foreach (ListViewItem item in deviceListView.SelectedItems)
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel)item.Tag;
                if (deviceViewModel.Kind == DeviceKind.NET)
                    app.StopNetworkDevice(deviceViewModel);
            }
        }

        private void RestartSelectedNetworkDevices()
        {
            foreach (ListViewItem item in deviceListView.SelectedItems)
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel)item.Tag;
                if (deviceViewModel.Kind == DeviceKind.NET)
                    app.RestartNetworkDevice(deviceViewModel);
            }
        }

        private void RebootSelectedNetworkDevices()
        {
            //there may be more than once miner process per IP
            //we only want to send the reboot command once per-IP
            List<DeviceViewModel> distinctNetworkDevices = deviceListView.SelectedItems.Cast<ListViewItem>()
                .Select(lvi => (DeviceViewModel)lvi.Tag)
                .Where(d => d.Kind == DeviceKind.NET)
                .GroupBy(d => d.Path.Split(':').First())
                .Select(g => g.First())
                .ToList();

            distinctNetworkDevices.ForEach(d => app.RebootNetworkDevice(d));
        }

        private void ExecuteCommandOnSelectedNetworkDevices()
        {
            //use Focused device for RecentCommands
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            NetworkDevices.NetworkDevice networkDevice = app.GetNetworkDeviceByPath(deviceViewModel.Path);
            ShellCommandForm form = new ShellCommandForm(networkDevice.RecentCommands);
            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in deviceListView.SelectedItems)
                {
                    deviceViewModel = (DeviceViewModel)item.Tag;

                    if (deviceViewModel.Kind == DeviceKind.NET)
                    {
                        bool success = app.ExecuteNetworkDeviceCommand(deviceViewModel, form.ShellCommand);
                        if (success)
                        {
                            networkDevice = app.GetNetworkDeviceByPath(deviceViewModel.Path);
                            networkDevice.RecentCommands.Remove(form.ShellCommand);
                            networkDevice.RecentCommands.Insert(0, form.ShellCommand);
                            app.NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
                        }
                    }
                }
            }
        }

        private void NetworkDevicePoolClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string poolUrl = ((PoolInformation)menuItem.Tag).Url;

            foreach (ListViewItem item in deviceListView.SelectedItems)
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel)item.Tag;
                if (deviceViewModel.Kind == DeviceKind.NET)
                    app.SetNetworkDevicePool(deviceViewModel, poolUrl);
            }
        }

        private void UpdateNetworkDeviceMenu()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            NetworkDevices.NetworkDevice deviceConfiguration = app.NetworkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            PopulateNetworkDevicePoolMenu(deviceViewModel);
            stickyToolStripMenuItem.Checked = deviceConfiguration.Sticky;
        }

        private void ToggleNetworkDeviceSticky()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            app.ToggleNetworkDeviceSticky(deviceViewModel);
        }

        private void ToggleNetworkDeviceHidden()
        {
            DeviceViewModel deviceViewModel = (DeviceViewModel)deviceListView.FocusedItem.Tag;
            app.ToggleNetworkDeviceHidden(deviceViewModel);
            RefreshListViewFromViewModel();
        }
        
        private void PopulateNetworkDevicePoolMenu(DeviceViewModel viewModel)
        {
            networkDevicePoolMenu.DropDownItems.Clear();

            if (!app.NetworkDevicePools.ContainsKey(viewModel.Path))
                //Network Device is offline but pinned
                return;
            
            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = app.NetworkDevicePools[viewModel.Path];

            if (poolInformation == null)
                //RPC API call timed out
                return;

            foreach (PoolInformation pool in poolInformation)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(pool.Url.DomainFromHost()) { Tag = pool };
                menuItem.Click += NetworkDevicePoolClicked;
                networkDevicePoolMenu.DropDownItems.Add(menuItem);
            }
        }
        #endregion
    }
}
