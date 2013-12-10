using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
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
using MultiMiner.Utility;
using MultiMiner.Win.Notifications;
using MultiMiner.Xgminer.Api.Responses;
using MultiMiner.Win.Extensions;
using MultiMiner.Win.Configuration;
using MultiMiner.Coin.Api;

namespace MultiMiner.Win
{
    public partial class MainForm : MessageBoxFontForm
    {
        private List<Coin.Api.CoinInformation> coinInformation;
        private List<Device> devices;
        private EngineConfiguration engineConfiguration = new EngineConfiguration();
        private List<CryptoCoin> knownCoins = new List<CryptoCoin>();
        private readonly MiningEngine miningEngine = new MiningEngine();
        private ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
        private int startupMiningCountdownSeconds = 0;
        private int coinStatsCountdownMinutes = 0;
        private readonly List<ApiLogEntry> apiLogEntries = new List<ApiLogEntry>();
        private bool formLoaded = false;
        private readonly List<LogLaunchArgs> logLaunchEntries = new List<LogLaunchArgs>();
        private readonly List<int> processedCommandIds = new List<int>();
        private readonly List<LogProcessCloseArgs> logCloseEntries = new List<LogProcessCloseArgs>();
        private NotificationsControl notificationsControl;
        private bool settingsLoaded = false;
        private Dictionary<MinerProcess, List<DeviceDetailsResponse>> processDeviceDetails = new Dictionary<MinerProcess, List<DeviceDetailsResponse>>();
        private MultiMiner.Coin.Api.IApiContext coinApiContext = new CoinChoose.Api.ApiContext();
        private List<CoinConfiguration> miningCoinConfigurations;
        private PerksConfiguration perksConfiguration = new PerksConfiguration();
        private MultiMiner.Coinbase.Api.SellPrices sellPrices;
        private readonly double difficultyMuliplier = Math.Pow(2, 32);

        public MainForm()
        {
            InitializeComponent();

            applicationConfiguration.LoadApplicationConfiguration();
            if (applicationConfiguration.StartupMinimized)
                this.WindowState = FormWindowState.Minimized;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (applicationConfiguration.StartupMinimized && applicationConfiguration.MinimizeToNotificationArea)
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }

            incomeSummaryLabel.Text = String.Empty;

            SetupInitialButtonVisibility();

            SetupGridColumns();

            LoadPreviousHistory();
            logLaunchArgsBindingSource.DataSource = logCloseEntries;

            const int mobileMinerInterval = 32; //seconds
            mobileMinerTimer.Interval = mobileMinerInterval * 1000;

            FetchInitialCoinStats();

            CheckAndShowGettingStarted();
            
            LoadSettings();

            RefreshDetailsToggleButton();

            RefreshCoinApiLabel();

            RefreshCoinPopupMenu();

            PositionCoinChooseLabels();

            apiLogEntryBindingSource.DataSource = apiLogEntries;

            SetupMiningEngineEvents();
            logLaunchArgsBindingSource.DataSource = logLaunchEntries;
            logProcessCloseArgsBindingSource.DataSource = logCloseEntries;

            UpdateChangesButtons(false);
            
            if (!HasMinersInstalled())
                CancelMiningOnStartup();

            if (!MiningConfigurationValid())
                CancelMiningOnStartup();

            //check for disowned miners before refreshing devices
            if (applicationConfiguration.DetectDisownedMiners)
                CheckForDisownedMiners();

            SetupStatusBarLabelLayouts();

            CheckAndDownloadMiners();
            
            SetupAutoUpdates();

            UpdateChangesButtons(false);

            RefreshDevices();
            
            UpdateMiningButtons();

            AutoSizeListViewColumns();

            formLoaded = true;

            logProcessCloseArgsBindingSource.MoveLast();
        }

        private void FetchInitialCoinStats()
        {
            engineConfiguration.LoadStrategyConfiguration(); //needed before refreshing coins
            engineConfiguration.LoadCoinConfigurations(); //needed before refreshing coins
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

        private void SetupInitialButtonVisibility()
        {
            saveButton.Visible = false;
            cancelButton.Visible = false;
            saveSeparator.Visible = false;
            stopButton.Visible = false;
        }

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
            string logFilePath = Path.Combine(AppDataPath(), logFileName);
            ObjectLogger logger = new ObjectLogger(applicationConfiguration.RollOverLogFiles, applicationConfiguration.OldLogFileSets);
            logger.LogObjectToFile(objectToLog, logFilePath);
        }

        private void LogProcessClose(object sender, LogProcessCloseArgs ea)
        {
            CheckAndAddStratumDeviceIndex(ea);

            logProcessCloseArgsBindingSource.Position = logProcessCloseArgsBindingSource.Add(ea);

            while (logProcessCloseArgsBindingSource.Count > MaxHistoryOnScreen)
                logProcessCloseArgsBindingSource.RemoveAt(0);

            LogProcessCloseToFile(ea);
        }

        private void CheckAndAddStratumDeviceIndex(LogProcessCloseArgs ea)
        {
            //check and include the index of the virtual stratum proxy "device"
            if (ea.MinerConfiguration.StratumProxy)
            {
                Device proxyDevice = devices.SingleOrDefault(d => d.Kind == DeviceKind.PXY);
                if (proxyDevice != null)
                    ea.DeviceDescriptors.Add(proxyDevice);
            }
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

        private const int MaxHistoryOnScreen = 1000;
        private void LoadPreviousHistory()
        {
            //logCloseEntries
            //LogObjectToFile(ea, logFileName);
            const string logFileName = "MiningLog.json";
            string logFilePath = Path.Combine(AppDataPath(), logFileName);
            if (File.Exists(logFilePath))
            {
                try
                {
                    List<LogProcessCloseArgs> loadLogFile = ObjectLogger.LoadLogFile<LogProcessCloseArgs>(logFilePath).ToList();
                    loadLogFile.RemoveRange(0, Math.Max(0, loadLogFile.Count - MaxHistoryOnScreen));
                    logCloseEntries.AddRange(loadLogFile);
                }
                catch (ArgumentException ex)
                {
                    //old MiningLog.json file - wrong format serialized
                    //MiningLog.json rolls over so we will eventually be able to
                    //load the previous log file
                    return;
                }
            }
        }

        private void UpdateChangesButtons(bool hasChanges)
        {
            saveButton.Visible = hasChanges;
            cancelButton.Visible = hasChanges;
            saveSeparator.Visible = hasChanges;

            saveButton.Enabled = hasChanges;
            cancelButton.Enabled = hasChanges;
        }

        private void SetupAutoUpdates()
        {
            updateCheckTimer.Interval = 3600000; //1h
            updateCheckTimer.Enabled = true;
            CheckForUpdates();
        }

        private void SetupNotificationsControl()
        {
            Control parent = advancedAreaContainer.Panel1;

            const int ControlOffset = 2;
            const int ControlHeight = 143;
            const int ControlWidth = 320;

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

        private void notificationsControl1_NotificationAdded(string text)
        {
            LogNotificationToFile(text);
            SubmitMobileMinerNotification(text);
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

                        //if auto mining is enabled, disable the coin configuration and display a notification
                        CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(config => config.Coin.Name.Equals(ea.CoinName, StringComparison.OrdinalIgnoreCase));
                        coinConfiguration.Enabled = false;
                        engineConfiguration.SaveCoinConfigurations();

                        //if no enabled configurations, stop mining
                        int enabledConfigurations = engineConfiguration.CoinConfigurations.Count(config => config.Enabled);
                        if (enabledConfigurations == 0)
                            StopMining();
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

                    notificationsControl.AddNotification(notificationReason, notificationReason, () =>
                    {
                        ConfigureCoins();
                    }, "");
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
                        notificationsControl.AddNotification(notificationReason, notificationReason, () =>
                        {
                            ConfigureCoins();
                        }, "");
                    }
                }
            }));
        }

        private void ProcessAuthenticationFailed(object sender, AuthenticationFailedArgs ea)
        {
            this.BeginInvoke((Action)(() =>
            {
                //code to update UI
                notificationsControl.AddNotification(ea.Reason, ea.Reason, () =>
                        {
                            ConfigureCoins();
                        }, "");
            }));
        }

        private void CheckAndShowGettingStarted()
        {
            //only show if there's no settings yet
            if (File.Exists(ApplicationConfiguration.ApplicationConfigurationFileName()))
                return;

            WizardForm wizardForm = new WizardForm(this.knownCoins);
            DialogResult dialogResult = wizardForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                EngineConfiguration newEngineConfiguration;
                ApplicationConfiguration newApplicationConfiguration;
                PerksConfiguration newPerksConfiguration;
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

        private static void CheckAndDownloadMiners()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                return; //can't auto download binaries on Linux

            bool hasMiners = HasMinersInstalled();

            if (!hasMiners)
                InstallMiner();
        }
        
        private static void InstallMiner()
        {
            string minerName = MinerPath.GetMinerName();

            ProgressForm progressForm = new ProgressForm(String.Format("Downloading and installing {0} from {1}", minerName, Xgminer.Installer.GetMinerDownloadRoot()));
            progressForm.Show();

            //for Mono - show the UI
            Application.DoEvents();
            Thread.Sleep(25); 
            Application.DoEvents();
            try
            {
                string minerPath = Path.Combine("Miners", minerName);
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
                Xgminer.Installer.InstallMiner(destinationFolder);
            }
            finally
            {
                progressForm.Close();
            }
        }

        private static bool HasMinersInstalled()
        {
            return MinerIsInstalled();
        }
        
        private static bool MinerIsInstalled()
        {
            string path = MinerPath.GetPathToInstalledMiner();
            return File.Exists(path);
        }

        private void CheckForDisownedMiners()
        {
            string minerName = MinerPath.GetMinerName();

            List<Process> disownedMiners = Process.GetProcessesByName(minerName).ToList();

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
                disownedMiners.Remove(minerProcess.Process);

            if (disownedMiners.Count > 0)
            {
                DialogResult messageBoxResult = MessageBox.Show("MultiMiner has detected running miners that it does not own. Would you like to kill them?", 
                    "Disowned Miners Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (messageBoxResult == System.Windows.Forms.DialogResult.Yes)
                    foreach (Process disownedMiner in disownedMiners)
                        MinerProcess.KillProcess(disownedMiner);
            }
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

        private bool updatingListView = false;
        private void RefreshDevices()
        {
            updatingListView = true;
            try
            {
                try
                {
                    using (new HourGlass())
                    {
                        devices = GetDevices();
                    }
                }
                catch (Win32Exception ex)
                {
                    //miner not installed/not launched
                    devices = new List<Device>(); //dummy empty device list

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

                PopulateListViewFromDevices();
                LoadListViewValuesFromConfiguration();
                LoadListViewValuesFromCoinStats();

                //auto-size columns
                AutoSizeListViewColumns();

                deviceTotalLabel.Text = String.Format("{0} device(s)", devices.Count);
            }
            finally
            {
                updatingListView = false;
            }
        }

        //try to match up devices without configurations with configurations without devices
        //could happen if, for instance, a COM port changes for a device
        private void FixOrphanedDeviceConfigurations()
        {
            foreach (Device device in devices)
            {
                DeviceConfiguration existingConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));

                //if there is no configuration specifically for the device
                if (existingConfiguration == null)
                {
                    //find a configuration that uses the same driver and that, itself, has no specifically matching device
                    DeviceConfiguration orphanedConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                        c => c.Driver.Equals(device.Driver, StringComparison.OrdinalIgnoreCase) &&
                                !devices.Exists(d => d.Equals(c)));

                    if (orphanedConfiguration != null)
                        orphanedConfiguration.Assign(device);
                }
            }
        }

        //optimized for speed
        private static void SetColumWidth(ColumnHeader column, int width)
        {
            if ((width < 0) || (column.Width != width))
                column.Width = width;
        }

        private void AutoSizeListViewColumns()
        {
            if (deviceListView.View != View.Details)
                return;
                                    
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
        
        private bool ListViewColumnHasValues(string headerText)
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (!String.IsNullOrEmpty(item.SubItems[headerText].Text))
                    return true;
            return false;
        }

        private void PopulateListViewFromDevices()
        {
            deviceListView.BeginUpdate();
            try
            {
                deviceListView.Items.Clear();

                foreach (Device device in devices)
                {
                    ListViewItem listViewItem = new ListViewItem();

                    switch (device.Kind)
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
                    }
                    
                    listViewItem.Text = device.Name;

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


                    listViewItem.SubItems["Driver"].Text = device.Driver;

                    deviceListView.Items.Add(listViewItem);
                }
            }
            finally
            {
                deviceListView.EndUpdate();
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
            foreach (Device device in devices)
            {
                DeviceConfiguration existingConfiguration = engineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));
                if (existingConfiguration == null)
                {
                    DeviceConfiguration newConfiguration = new DeviceConfiguration();

                    newConfiguration.Assign(device);

                    //if the user has BTC configured, default to that
                    string btcSymbol = "BTC";
                    bool hasBtcConfigured = engineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.Coin.Symbol.Equals(btcSymbol, StringComparison.OrdinalIgnoreCase));
                    if (hasBtcConfigured)
                        newConfiguration.CoinSymbol = btcSymbol;

                    newConfiguration.Enabled = true;
                    engineConfiguration.DeviceConfigurations.Add(newConfiguration);
                }
            }
        }
        
        private void ShowNotInstalledMinerWarning()
        {
            bool showWarning = true;

            if (OSVersionPlatform.GetConcretePlatform() != PlatformID.Unix)
            {
                string minerName = MinerPath.GetMinerName();

                DialogResult dialogResult = MessageBox.Show(String.Format(
                    "No copy of bfgminer was detected. " +
                    "Would you like to download and install {0} now?", minerName), "Miner Not Found",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    InstallMiner();
                    RefreshDevices();
                    showWarning = false;
                }
            }

            if (showWarning)
                MessageBox.Show("No copy of bfgminer was detected. Please go to https://github.com/nwoolls/multiminer for instructions on installing bfgminer.",
                        "Miner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ConfigureDevicesForNewUser()
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single();

            for (int i = 0; i < devices.Count; i++)
            {
                DeviceConfiguration deviceConfiguration = new DeviceConfiguration() 
                { 
                    CoinSymbol = coinConfiguration.Coin.Symbol, 
                    Enabled = true 
                };

                deviceConfiguration.Assign(devices[i]);
                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            engineConfiguration.SaveDeviceConfigurations();
            UpdateMiningButtons();
        }
        
        private void LoadSettings()
        {
            engineConfiguration.LoadAllConfigurations();

            SetupCoinApi();

            RefreshStrategiesLabel();
            RefreshStrategiesCountdown();

            dynamicIntensityButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;

            //already done in ctor //applicationConfiguration.LoadApplicationConfiguration();

            perksConfiguration.LoadPerksConfiguration();
            exchangeRateTimer.Interval = 1000 * 60 * 30; //30 minutes
            exchangeRateTimer.Enabled = perksConfiguration.PerksEnabled && perksConfiguration.ShowExchangeRates;
            RefreshExchangeRates();

            SetListViewStyle(applicationConfiguration.ListViewStyle);

            //load brief mode first, then location
            SetBriefMode(applicationConfiguration.BriefUserInterface);

            //now location so we pick up the customizations
            if ((applicationConfiguration.AppPosition != null) &&
                (applicationConfiguration.AppPosition.Height > 0) &&
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
                if (applicationConfiguration.LogAreaDistance > 0)
                    advancedAreaContainer.SplitterDistance = applicationConfiguration.LogAreaDistance;
            }
            else
                HideAdvancedPanel();

            if (applicationConfiguration.StartMiningOnStartup)
            {
                //minimum 1s delay for mining on startup - 0 not allowed
                startupMiningCountdownSeconds = Math.Max(1, applicationConfiguration.StartupMiningDelay);
                startupMiningCountdownTimer.Enabled = true;
                RefreshCountdownLabel();
            }

            startupMiningPanel.Visible = applicationConfiguration.StartMiningOnStartup;

            crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;

            SetupCoinStatsTimer();

            idleTimer.Interval = 15 * 1000; //check every 15s
            idleTimer.Enabled = true;

            //allow resize/maximize/etc to render
            Application.DoEvents();

            this.settingsLoaded = true;
        }

        private void RefreshExchangeRates()
        {
            if (perksConfiguration.PerksEnabled && perksConfiguration.ShowExchangeRates)
            {
                try
                {
                    sellPrices = Coinbase.Api.ApiContext.GetSellPrices();
                }
                catch (Exception ex)
                {
                    //don't crash if website cannot be resolved or JSON cannot be parsed
                    if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException))
                    {
                        ShowCoinbaseApiErrorNotification(ex);
                        return;
                    }
                    throw;
                }
            }
        }

        private void ShowCoinbaseApiErrorNotification(Exception ex)
        {
            string siteUrl = Coinbase.Api.ApiContext.GetInfoUrl();
            string apiUrl = Coinbase.Api.ApiContext.GetApiUrl();
            string apiName = Coinbase.Api.ApiContext.GetApiName();

            notificationsControl.AddNotification(ex.Message,
                String.Format("Error parsing the {0} JSON API", apiName), () =>
                {
                    Process.Start(apiUrl);
                },
                siteUrl);
        }

        private void RefreshCountdownLabel()
        {
            countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", startupMiningCountdownSeconds);    
        }

        private List<Device> GetDevices()
        {
            MinerConfiguration minerConfiguration = new MinerConfiguration() 
            { 
                ExecutablePath = MinerPath.GetPathToInstalledMiner(), 
                DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu 
            };

            Miner miner = new Miner(minerConfiguration);

            List<Device> detectedDevices = miner.ListDevices();

            if (engineConfiguration.XgminerConfiguration.StratumProxy)
            {
                detectedDevices.Add(new Device() 
                { 
                    Kind = DeviceKind.PXY, 
                    Driver = "proxy", 
                    Name = "Stratum Proxy" 
                });
            }

            SortDevices(detectedDevices);

            return detectedDevices;
        }

        private static void SortDevices(List<Device> detectedDevices)
        {
            detectedDevices.Sort((d1, d2) =>
                {
                    int result = 0;

                    result = d1.Kind.CompareTo(d2.Kind);

                    if (result == 0)
                        result = d1.Driver.CompareTo(d2.Driver);

                    if (result == 0)
                        result = d1.Name.CompareTo(d2.Name);

                    if (result == 0)
                        result = d1.Path.CompareTo(d2.Path);

                    if (result == 0)
                        result = d1.RelativeIndex.CompareTo(d2.RelativeIndex);

                    return result;
                });
        }

        private void ConfigureCoins()
        {
            CoinsForm coinsForm = new CoinsForm(engineConfiguration.CoinConfigurations, knownCoins);
            DialogResult dialogResult = coinsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Application.DoEvents();

                engineConfiguration.SaveCoinConfigurations();

                RemoveInvalidCoinValuesFromListView();

                RefreshCoinPopupMenu();

                //SaveChanges() will restart mining if needed
                SaveChanges();
            }
            else
                engineConfiguration.LoadCoinConfigurations();
        }

        private void RemoveInvalidCoinValuesFromListView()
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Enabled && c.Coin.Name.Equals(item.SubItems["Coin"].Text)) == null)
                    item.SubItems["Coin"].Text = String.Empty;

            ClearCoinStatsForDisabledCoins();
        }

        private void ClearCoinStatsForDisabledCoins()
        {
            foreach (ListViewItem item in deviceListView.Items)
                if (string.IsNullOrEmpty(item.SubItems["Coin"].Text))
                    ClearCoinStatsForGridListViewItem(item);
        }

        private void RefreshCoinPopupMenu()
        {
            coinPopupMenu.Items.Clear();
            foreach (CoinConfiguration configuration in engineConfiguration.CoinConfigurations.Where(c => c.Enabled))
            {
                ToolStripItem menuItem = coinPopupMenu.Items.Add(configuration.Coin.Name);
                menuItem.Click += CoinMenuItemClick;
            }
        }

        private void CoinMenuItemClick(object sender, EventArgs e)
        {
            ToolStripItem menuItem = (ToolStripItem)sender;

            foreach (ListViewItem selectedItem in deviceListView.SelectedItems)
                selectedItem.SubItems["Coin"].Text = menuItem.Text;

            LoadListViewValuesFromCoinStats();

            AutoSizeListViewColumns();

            UpdateChangesButtons(true);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //SaveChanges() will restart mining if needed
            SaveChanges();
        }

        private void SaveChanges()
        {
            SaveListViewValuesToConfiguration();
            engineConfiguration.SaveDeviceConfigurations();
            LoadListViewValuesFromConfiguration();

            UpdateChangesButtons(false);

            Application.DoEvents();

            UpdateMiningButtons();
            ClearMinerStatsForDisabledCoins();
            
            //update coin stats now that we saved coin changes
            LoadListViewValuesFromCoinStats();

            //take into account above changes
            AutoSizeListViewColumns();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.LoadDeviceConfigurations();
            LoadListViewValuesFromConfiguration();
            LoadListViewValuesFromCoinStats();

            UpdateChangesButtons(false);
            AutoSizeListViewColumns();
        }

        private void SaveListViewValuesToConfiguration()
        {
            engineConfiguration.DeviceConfigurations.Clear();

            for (int i = 0; i < devices.Count; i++)
            {
                ListViewItem listViewItem = deviceListView.Items[i];

                //pull this from coin configurations, not known coins, may not be in CoinChoose
                string coinValue = listViewItem.SubItems["Coin"].Text;
                CryptoCoin coin = null;
                if (!String.IsNullOrEmpty(coinValue))
                    coin = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinValue, StringComparison.OrdinalIgnoreCase)).Coin;

                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                deviceConfiguration.Assign(devices[i]);

                deviceConfiguration.Enabled = listViewItem.Checked;
                deviceConfiguration.CoinSymbol = coin == null ? string.Empty : coin.Symbol;

                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);

            }
        }

        private void LoadListViewValuesFromConfiguration()
        {
            bool saveEnabled = saveButton.Enabled;

            deviceListView.BeginUpdate();

            for (int i = 0; i < devices.Count; i++)
            {
                DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(c => (c.Equals(devices[i])));
                ListViewItem listViewItem = deviceListView.Items[i];

                if (deviceConfiguration != null)
                {
                    //ensure the coin configuration still exists
                    CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Coin.Symbol.Equals(deviceConfiguration.CoinSymbol));
                    if (coinConfiguration != null)
                        listViewItem.SubItems["Coin"].Text = coinConfiguration.Coin.Name;
                    else
                        listViewItem.SubItems["Coin"].Text = string.Empty;

                    listViewItem.Checked = deviceConfiguration.Enabled;

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
                }
                else
                {
                    listViewItem.SubItems["Coin"].Text = string.Empty;
                    listViewItem.Checked = true;
                }
            }

            //restore button states after
            UpdateChangesButtons(saveEnabled);

            deviceListView.EndUpdate();
        }
        
        private void UpdateMiningButtons()
        {
            startButton.Enabled = MiningConfigurationValid() && !miningEngine.Mining;

            stopButton.Enabled = miningEngine.Mining;
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;
            //allow clicking Detect Devices with invalid configuration
            detectDevicesButton.Enabled = !miningEngine.Mining;
            detectDevicesToolStripMenuItem.Enabled = !miningEngine.Mining;

            startButton.Visible = startButton.Enabled;
            stopButton.Visible = stopButton.Enabled;

            if (!startButton.Visible && !stopButton.Visible)
                startButton.Visible = true; //show something, even if disabled

            startMenuItem.Visible = startMenuItem.Enabled;
            stopMenuItem.Visible = stopMenuItem.Enabled;
        }

        private bool MiningConfigurationValid()
        {
            bool miningConfigurationValid = engineConfiguration.DeviceConfigurations.Count(
                c => c.Enabled && !string.IsNullOrEmpty(c.CoinSymbol)) > 0;
            if (!miningConfigurationValid)
            {
                miningConfigurationValid = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                    (engineConfiguration.CoinConfigurations.Count(c => c.Enabled) > 0) &&
                    (engineConfiguration.DeviceConfigurations.Count(c => c.Enabled) > 0);
            }
            return miningConfigurationValid;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void StopMining()
        {
            using (new HourGlass())
            {
                miningEngine.StopMining();
            }

            processDeviceDetails.Clear();
            deviceStatsTimer.Enabled = false;
            minerSummaryTimer.Enabled = false;
            coinStatsCountdownTimer.Enabled = false;
            RefreshStrategiesCountdown();
            scryptRateLabel.Text = string.Empty;
            sha256RateLabel.Text = string.Empty;
            notifyIcon1.Text = "MultiMiner - Stopped";
            UpdateMiningButtons();
            ClearAllMinerStats();
            RefreshIncomeSummary();
            AutoSizeListViewColumns();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (applicationConfiguration.AutoSetDesktopMode)
                EnableDesktopMode(true);

            SaveChanges();
            StartMining(true);
        }

        private void StartMining(bool donate = false)
        {
            if (!MiningConfigurationValid())
                return;

            if (miningEngine.Mining)
                return;

            if (!ConfigFileHandled())
                return;

            startButton.Enabled = false; //immediately disable, update after
            startMenuItem.Enabled = false;
            
            //create a deep clone of the mining configurations
            //this is so we can accurately display e.g. the currently mining pools
            //even if the user changes pool info without restartinging mining
            this.miningCoinConfigurations = ObjectCopier.DeepCloneObject<List<CoinConfiguration>, List<CoinConfiguration>>(engineConfiguration.CoinConfigurations);

            try
            {
                using (new HourGlass())
                {
                    miningEngine.StartMining(engineConfiguration, devices, coinInformation, perksConfiguration.PerksEnabled);
                }
            }
            catch (MinerLaunchException ex)
            {
                MessageBox.Show(ex.Message, "Error Launching Miner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!donate)
                engineConfiguration.SaveDeviceConfigurations(); //save any changes made by the engine

            deviceStatsTimer.Enabled = true;
            minerSummaryTimer.Enabled = true;
            coinStatsCountdownTimer.Enabled = true;
            RefreshStrategiesCountdown();

            //to get changes from strategy config
            LoadListViewValuesFromConfiguration();
            //to get updated coin stats for coin changes
            LoadListViewValuesFromCoinStats();

            AutoSizeListViewColumns();

            UpdateMiningButtons();
        }

        private static bool ConfigFileHandled()
        {
            const string bakExtension = ".mmbak";

            string minerName = MinerPath.GetMinerName();
            string minerExecutablePath = MinerPath.GetPathToInstalledMiner();
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
        
        private void ConfigureSettings()
        {
            SettingsForm settingsForm = new SettingsForm(applicationConfiguration, engineConfiguration.XgminerConfiguration);
            DialogResult dialogResult = settingsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Application.DoEvents();

                engineConfiguration.SaveMinerConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                SetupCoinApi();
                RefreshCoinApiLabel();
                crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
                SetupRestartTimer();
                CheckForUpdates();
                RefreshCoinStats();
                
                Application.DoEvents();
            }
            else
            {
                engineConfiguration.LoadMinerConfiguration();
                applicationConfiguration.LoadApplicationConfiguration();
            }
        }

        private void SetupCoinApi()
        {
            if (applicationConfiguration.UseCoinWarzApi)
                this.coinApiContext = new CoinWarz.Api.ApiContext(applicationConfiguration.CoinWarzApiKey);
            else
                this.coinApiContext = new CoinChoose.Api.ApiContext();
        }

        private void RefreshCoinApiLabel()
        {
            coinApiLinkLabel.Text = this.coinApiContext.GetApiName();

            PositionCoinChooseLabels();
        }

        private void SetupRestartTimer()
        {
            restartTimer.Interval = applicationConfiguration.ScheduledRestartMiningInterval.ToMinutes() * 60 * 1000;
            restartTimer.Enabled = applicationConfiguration.ScheduledRestartMining;
        }

        private void statsTimer_Tick(object sender, EventArgs e)
        {
            ClearMinerStatsForDisabledCoins();
            RefreshDeviceStats();
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

        private static void ClearDeviceInfoForListViewItem(ListViewItem item)
        {
            item.SubItems["Temp"].Text = String.Empty;

            item.SubItems["Hashrate"].Text = String.Empty;
            item.SubItems["Hashrate"].Tag = 0.00;

            item.SubItems["Accepted"].Text = String.Empty;
            item.SubItems["Accepted"].Tag = 0;

            item.SubItems["Rejected"].Text = String.Empty;
            item.SubItems["Rejected"].Tag = 0.00;

            item.SubItems["Errors"].Text = String.Empty;
            item.SubItems["Errors"].Tag = 0.00;

            item.SubItems["Utility"].Text = String.Empty;
            item.SubItems["Utility"].Tag = 0.00;

            item.SubItems["Intensity"].Text = String.Empty;
            item.SubItems["Pool"].Text = String.Empty;

            item.SubItems["Daily"].Text = String.Empty;
            item.SubItems["Daily"].Tag = 0.00;
        }

        private void PopulateDeviceStatsForListViewItem(DeviceInformationResponse deviceInformation, ListViewItem item)
        {
            deviceListView.BeginUpdate();
            try
            {
                //stratum devices get lumped together, so we sum those
                if (deviceInformation.Name.Equals("PXY", StringComparison.OrdinalIgnoreCase))
                {
                    item.SubItems["Hashrate"].Tag = (double)(item.SubItems["Hashrate"].Tag ?? 0.00) + deviceInformation.AverageHashrate;
                    item.SubItems["Rejected"].Tag = (double)(item.SubItems["Rejected"].Tag ?? 0.00) + deviceInformation.RejectedSharesPercent;
                    item.SubItems["Errors"].Tag = (double)(item.SubItems["Errors"].Tag ?? 0.00) + deviceInformation.HardwareErrorsPercent;
                    item.SubItems["Accepted"].Tag = (int)(item.SubItems["Accepted"].Tag ?? 0) + deviceInformation.AcceptedShares;
                    item.SubItems["Utility"].Tag = (double)(item.SubItems["Utility"].Tag ?? 0.00) + deviceInformation.Utility;
                }
                else
                {
                    item.SubItems["Hashrate"].Tag = deviceInformation.AverageHashrate;
                    item.SubItems["Rejected"].Tag = deviceInformation.RejectedSharesPercent;
                    item.SubItems["Errors"].Tag = deviceInformation.HardwareErrorsPercent;
                    item.SubItems["Accepted"].Tag = deviceInformation.AcceptedShares;
                    item.SubItems["Utility"].Tag = deviceInformation.Utility;
                }

                item.SubItems["Hashrate"].Text = ((double)item.SubItems["Hashrate"].Tag).ToHashrateString();

                //check for >= 0.05 so we don't show 0% (due to the format string)
                item.SubItems["Rejected"].Text = (double)item.SubItems["Rejected"].Tag >= 0.05 ? ((double)item.SubItems["Rejected"].Tag).ToString("0.#") + "%" : String.Empty;
                item.SubItems["Errors"].Text = (double)item.SubItems["Errors"].Tag >= 0.05 ? ((double)item.SubItems["Errors"].Tag).ToString("0.#") + "%" : String.Empty;

                item.SubItems["Accepted"].Text = (int)item.SubItems["Accepted"].Tag > 0 ? ((int)item.SubItems["Accepted"].Tag).ToString() : String.Empty;

                item.SubItems["Utility"].Text = (double)item.SubItems["Utility"].Tag >= 0.00 ? ((double)item.SubItems["Utility"].Tag).ToString("0.###") : String.Empty;

                item.SubItems["Temp"].Text = deviceInformation.Temperature > 0 ? deviceInformation.Temperature + "°" : String.Empty;
                item.SubItems["Fan"].Text = deviceInformation.FanPercent > 0 ? deviceInformation.FanPercent + "%" : String.Empty;
                item.SubItems["Intensity"].Text = deviceInformation.Intensity;

                PopulatePoolForListViewItem(deviceInformation.PoolIndex, item);

                PopulateIncomeForListViewItem(item);
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private void PopulateIncomeForListViewItem(ListViewItem item)
        {
            item.SubItems["Daily"].Text = String.Empty;

            if (!(miningEngine.Donating && perksConfiguration.ShowIncomeRates))
                return;

            //base this off the active configuration, not the text in the ListView (may be unsaved)
            CoinConfiguration coinConfiguration = CoinConfigurationForListViewItem(item);
            //guard against null-ref if the device has no configuration
            if (coinConfiguration == null)
                return;

            CoinInformation info = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinConfiguration.Coin.Symbol, StringComparison.OrdinalIgnoreCase));
            if (info != null)
            {
                double difficulty = (double)item.SubItems["Difficulty"].Tag;
                double hashrate = (double)item.SubItems["Hashrate"].Tag * 1000;
                double fullDifficulty = difficulty * difficultyMuliplier;
                double secondsToCalcShare = fullDifficulty / hashrate;
                const double secondsPerDay = 86400;
                double sharesPerDay = secondsPerDay / secondsToCalcShare;
                double rewardPerDay = sharesPerDay * info.Reward;

                item.SubItems["Daily"].Tag = rewardPerDay;

                if (perksConfiguration.ShowExchangeRates && perksConfiguration.ShowIncomeInUsd)
                {
                    item.SubItems["Daily"].Text = String.Format("${0:0.00}", rewardPerDay * (double)item.SubItems["Exchange"].Tag);
                }
                else
                {
                    item.SubItems["Daily"].Text = String.Format("{0:0.#####} {1}", rewardPerDay, info.Symbol);
                }
            }
        }

        private void PopulatePoolForListViewItem(int poolIndex, ListViewItem item)
        {
            if (poolIndex >= 0)
            {
                CoinConfiguration coinConfiguration = CoinConfigurationForListViewItem(item);
                if (coinConfiguration == null)
                    item.SubItems["Pool"].Text = String.Empty;
                else
                {
                    //the poolIndex may be greater than the Pools count if the user edits
                    //their pools while mining
                    if (poolIndex < coinConfiguration.Pools.Count)
                    {
                        string poolHost = coinConfiguration.Pools[poolIndex].Host;
                        string poolDomain = poolHost.DomainFromHost();

                        item.SubItems["Pool"].Text = poolDomain;
                    }
                    else
                    {
                        if (miningEngine.Donating)
                            item.SubItems["Pool"].Text = "donation"; //donation pool won't be in list
                        else
                            item.SubItems["Pool"].Text = String.Empty;
                    }
                }
            }
            else
                item.SubItems["Pool"].Text = String.Empty;
        }

        private CoinConfiguration CoinConfigurationForListViewItem(ListViewItem item)
        {
            int itemIndex = deviceListView.Items.IndexOf(item);
            Device device = devices[itemIndex];
            //get the actual device configuration, text in the ListViewItem may be unsaved
            DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));
            if (deviceConfiguration == null)
                return null;

            string itemCoinSymbol = deviceConfiguration.CoinSymbol;

            List<CoinConfiguration> configurations;
            if (miningEngine.Mining)
                configurations = this.miningCoinConfigurations;
            else
                configurations = engineConfiguration.CoinConfigurations;

            CoinConfiguration coinConfiguration = configurations.SingleOrDefault(c => c.Coin.Symbol.Equals(itemCoinSymbol, StringComparison.OrdinalIgnoreCase));
            return coinConfiguration;
        }

        private void ClearAllMinerStats()
        {
            deviceListView.BeginUpdate();
            try
            {
                foreach (ListViewItem item in deviceListView.Items)
                    ClearDeviceInfoForListViewItem(item);
            }
            finally
            {
                deviceListView.EndUpdate();
            }
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

        private static void ClearCoinStatsForGridListViewItem(ListViewItem item)
        {
            item.SubItems["Difficulty"].Tag = 0.0;
            item.SubItems["Difficulty"].Text = String.Empty;

            item.SubItems["Price"].Text = String.Empty;
            item.SubItems["Profitability"].Text = String.Empty;

            item.SubItems["Exchange"].Tag = 0.0;
            item.SubItems["Exchange"].Text = String.Empty;
        }

        private void LogApiEvent(object sender, Xgminer.Api.LogEventArgs eventArgs)
        {
            ApiLogEntry logEntry = new ApiLogEntry();

            logEntry.DateTime = eventArgs.DateTime;
            logEntry.Request = eventArgs.Request;
            logEntry.Response = eventArgs.Response;
            logEntry.CoinName = GetCoinNameForApiContext((Xgminer.Api.ApiContext)sender);

            apiLogEntryBindingSource.Position = apiLogEntryBindingSource.Add(logEntry);

            while (apiLogEntryBindingSource.Count > 1000)
                apiLogEntryBindingSource.RemoveAt(0);

            LogApiEventToFile(logEntry);
        }

        private void LogApiEventToFile(ApiLogEntry logEntry)
        {
            const string logFileName = "ApiLog.json";
            LogObjectToFile(logEntry, logFileName);
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

        private void PopulateSummaryInfoFromProcesses()
        {
            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                SummaryInformationResponse summaryInformation = GetSummaryInfoFromProcess(minerProcess);

                if (summaryInformation == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                CheckAndNotifyFoundBlocks(minerProcess, summaryInformation.FoundBlocks);

                CheckAndNotifyAcceptedShares(minerProcess, summaryInformation.AcceptedShares);
            }
        }

        private void CheckAndNotifyFoundBlocks(MinerProcess minerProcess, long foundBlocks)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            CoinConfiguration configuration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnBlockFound && (foundBlocks > minerProcess.FoundBlocks))
            {
                minerProcess.FoundBlocks = foundBlocks;
                
                string notificationReason = String.Format("Block(s) found for {0} (block {1})",
                    coinName, minerProcess.FoundBlocks);

                notificationsControl.AddNotification(notificationReason, notificationReason, () =>
                {
                }, "");
            }
        }

        private void CheckAndNotifyAcceptedShares(MinerProcess minerProcess, long acceptedShares)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            CoinConfiguration configuration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnShareAccepted && (acceptedShares > minerProcess.AcceptedShares))
            {
                minerProcess.AcceptedShares = acceptedShares;

                string notificationReason = String.Format("Share(s) accepted for {0} (share {1})",
                    coinName, minerProcess.AcceptedShares);

                notificationsControl.AddNotification(notificationReason, notificationReason, () =>
                {
                }, "");
            }
        }

        private void RefreshDeviceStats()
        {
            double totalScryptRate = 0;
            double totalSha256Rate = 0;

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                ClearSuspectProcessFlags(minerProcess);

                List<DeviceInformationResponse> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                //starting with bfgminer 3.7 we need the DEVDETAILS response to tie things from DEVS up with -d? details
                List<DeviceDetailsResponse> processDevices = GetProcessDevices(minerProcess, deviceInformationList);

                //first clear stats for each row
                //this is because the PXY row stats get summed  
                deviceListView.BeginUpdate();
                try
                {
                    foreach (DeviceInformationResponse deviceInformation in deviceInformationList)
                    {
                        DeviceDetailsResponse deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
                            && (d.ID == deviceInformation.ID));
                        int itemIndex = GetItemIndexForDeviceDetails(deviceDetails);
                        if (itemIndex >= 0)
                            //could legitimately be -1 if the API is returning a device we don't know about
                            ClearDeviceInfoForListViewItem(deviceListView.Items[itemIndex]);
                    }

                    //clear accepted shares as we'll be summing that as well
                    minerProcess.AcceptedShares = 0;

                    foreach (DeviceInformationResponse deviceInformation in deviceInformationList)
                    {
                        if (deviceInformation.Status.ToLower().Contains("sick"))
                            minerProcess.HasSickDevice = true;
                        if (deviceInformation.Status.ToLower().Contains("dead"))
                            minerProcess.HasDeadDevice = true;
                        if (deviceInformation.CurrentHashrate == 0)
                            minerProcess.HasZeroHashrateDevice = true;

                        //don't check average hashrate if using dynamic intensity and its a GPU
                        if (!engineConfiguration.XgminerConfiguration.DesktopMode ||
                            (!deviceInformation.Kind.Equals("GPU", StringComparison.OrdinalIgnoreCase)))
                        {
                            //avoid div by 0
                            if (deviceInformation.AverageHashrate > 0)
                            {
                                double performanceRatio = deviceInformation.CurrentHashrate / deviceInformation.AverageHashrate;
                                if (performanceRatio <= 0.50)
                                    minerProcess.HasPoorPerformingDevice = true;
                            }
                        }

                        DeviceDetailsResponse deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
                            && (d.ID == deviceInformation.ID));
                        int itemIndex = GetItemIndexForDeviceDetails(deviceDetails);

                        if (itemIndex >= 0)
                        {
                            if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.Scrypt)
                                totalScryptRate += deviceInformation.AverageHashrate;
                            else if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.SHA256)
                                totalSha256Rate += deviceInformation.AverageHashrate;

                            PopulateDeviceStatsForListViewItem(deviceInformation, deviceListView.Items[itemIndex]);

                            minerProcess.AcceptedShares += deviceInformation.AcceptedShares;
                        }
                    }
                }
                finally
                {
                    deviceListView.EndUpdate();
                }
            }

            //Mh not mh, mh is milli
            scryptRateLabel.Text = totalScryptRate == 0 ? String.Empty : String.Format("Scrypt: {0}", totalScryptRate.ToHashrateString());
            //spacing used to pad out the status bar item
            sha256RateLabel.Text = totalSha256Rate == 0 ? String.Empty : String.Format("SHA-2: {0}", totalSha256Rate.ToHashrateString()); 

            scryptRateLabel.AutoSize = true;
            sha256RateLabel.AutoSize = true;

            notifyIcon1.Text = string.Format("MultiMiner - {0} {1}", scryptRateLabel.Text, sha256RateLabel.Text);
            
            int count = 3;
            //auto sizing the columns is moderately CPU intensive, so only do it every /count/ times
            AutoSizeListViewColumnsEvery(count);

            RefreshIncomeSummary();
        }

        private void RefreshIncomeSummary()
        {
            if (sellPrices == null)
            {
                //no internet or error parsing API
                incomeSummaryLabel.Text = "";
                return;
            }

            if (coinInformation == null)
            {
                //no internet or error parsing API
                incomeSummaryLabel.Text = "";
                return;
            }

            if (!miningEngine.Donating || !perksConfiguration.ShowIncomeRates)
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
                foreach (string coinName in incomeForCoins.Keys)
                {
                    double coinIncome = incomeForCoins[coinName];
                    CoinInformation coinInfo = coinInformation.SingleOrDefault(c => c.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
                    if (coinInfo != null)
                    {
                        double coinUsd = sellPrices.Subtotal.Amount * coinInfo.Price;
                        double coinDailyUsd = coinIncome * coinUsd;
                        usdTotal += coinDailyUsd;

                        summary = String.Format("{0}{1:0.####} {2}{3}", summary, coinIncome, coinInfo.Symbol, addition);
                    }
                }

                if (!String.IsNullOrEmpty(summary))
                {
                    summary = summary.Remove(summary.Length - addition.Length, addition.Length); //remove trailing " + "

                    if (perksConfiguration.ShowExchangeRates)
                        summary = String.Format("{0} = ${1} / day", summary, String.Format("{0:0.00}", usdTotal));

                    incomeSummaryLabel.Text = summary;

                    incomeSummaryLabel.AutoSize = true;
                    incomeSummaryLabel.Padding = new Padding(0, 11, 17, 0);
                }
            }
        }

        private Dictionary<string, double> GetIncomeForCoins()
        {
            Dictionary<string, double> coinsIncome = new Dictionary<string, double>();

            for (int i = 0; i < deviceListView.Items.Count; i++)
            {
                ListViewItem listItem = deviceListView.Items[i];
                if (listItem.SubItems["Daily"].Tag != null)
                {
                    //report on the actual, mining coin, not just what is in the ListView
                    //e.g. we may be donating
                    CoinConfiguration coinConfiguration = CoinConfigurationForListViewItem(listItem);

                    if (coinConfiguration == null)
                        //no configuration for list item, continue to next
                        continue;

                    string coinName = coinConfiguration.Coin.Name;
                    double coinIncome = (double)listItem.SubItems["Daily"].Tag;

                    if (coinsIncome.ContainsKey(coinName))
                        coinsIncome[coinName] = coinsIncome[coinName] + coinIncome;
                    else
                        coinsIncome[coinName] = coinIncome;
                }
            }
            return coinsIncome;
        }

        private static void ClearSuspectProcessFlags(MinerProcess minerProcess)
        {
            minerProcess.HasDeadDevice = false;
            minerProcess.HasSickDevice = false;
            minerProcess.HasZeroHashrateDevice = false;
            minerProcess.MinerIsFrozen = false;
            minerProcess.HasPoorPerformingDevice = false;
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

        private List<DeviceDetailsResponse> GetProcessDevices(MinerProcess minerProcess, List<DeviceInformationResponse> deviceInformationList)
        {
            List<DeviceDetailsResponse> processDevices = null;
            if (processDeviceDetails.ContainsKey(minerProcess))
            {
                processDevices = processDeviceDetails[minerProcess];

                foreach (DeviceInformationResponse deviceInformation in deviceInformationList)
                {
                    DeviceDetailsResponse deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
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
                processDeviceDetails[minerProcess] = processDevices;
            }
            return processDevices;
        }

        private List<DeviceInformationResponse> GetDeviceInfoFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceInformationResponse> deviceInformationList = null;
            try
            {
                try
                {
                    deviceInformationList = apiContext.GetDeviceInformation().Where(d => d.Enabled).ToList();
                }
                catch (IOException ex)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    deviceInformationList = null;
                }
            }
            catch (SocketException ex)
            {
                //won't be able to connect for the first 5s or so
                deviceInformationList = null;
            }

            return deviceInformationList;
        }

        private List<DeviceDetailsResponse> GetDeviceDetailsFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceDetailsResponse> deviceDetailsList = null;
            try
            {
                try
                {
                    deviceDetailsList = apiContext.GetDeviceDetails().ToList();
                }
                catch (IOException ex)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    deviceDetailsList = null;
                }
            }
            catch (SocketException ex)
            {
                //won't be able to connect for the first 5s or so
                deviceDetailsList = null;
            }

            return deviceDetailsList;
        }

        private SummaryInformationResponse GetSummaryInfoFromProcess(MinerProcess minerProcess)
        {
            Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            SummaryInformationResponse summaryInformation = null;
            try
            {
                try
                {
                    summaryInformation = apiContext.GetSummaryInformation();
                }
                catch (IOException ex)
                {
                    //don't fail and crash out due to any issues communicating via the API
                    summaryInformation = null;
                }
            }
            catch (SocketException ex)
            {
                //won't be able to connect for the first 5s or so
                summaryInformation = null;
            }

            return summaryInformation;
        }
        
        private int GetItemIndexForDeviceDetails(DeviceDetailsResponse deviceDetails)
        {            
            for (int i = 0; i < devices.Count; i++)
            {
                Device device = devices[i];
                
                if (device.Driver.Equals(deviceDetails.Driver, StringComparison.OrdinalIgnoreCase)
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
                    
                    //proxy == proxy
                    || (device.Driver.Equals("proxy", StringComparison.OrdinalIgnoreCase))
                    
                    //opencl = opencl && ID = RelativeIndex
                    || (device.Driver.Equals("opencl", StringComparison.OrdinalIgnoreCase) && (device.RelativeIndex == deviceDetails.ID))

                    //cpu = cpu && ID = RelativeIndex
                    || (device.Driver.Equals("cpu", StringComparison.OrdinalIgnoreCase) && (device.RelativeIndex == deviceDetails.ID))

                    ))
                {
                    return i;
                }
            }

            return -1;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            StopMining();
        }

        private void SaveSettings()
        {
            applicationConfiguration.LogAreaTabIndex = advancedTabControl.SelectedIndex;
            SavePosition();
            this.applicationConfiguration.SaveApplicationConfiguration();
        }

        private void SavePosition()
        {
            if (this.WindowState == FormWindowState.Normal)
                this.applicationConfiguration.AppPosition = new Rectangle(this.Location, this.Size);
        }

        private void coinStatsTimer_Tick(object sender, EventArgs e)
        {
            RefreshCoinStats();

            CheckAndApplyMiningStrategy();

            coinStatsCountdownMinutes = coinStatsTimer.Interval / 1000 / 60;
        }

        private void CheckAndApplyMiningStrategy()
        {
            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                //ensure the user isn't editing settings
                !ShowingModalDialog())
            {
                miningEngine.ApplyMiningStrategy(coinInformation);
                //save any changes made by the engine
                engineConfiguration.SaveDeviceConfigurations();
                //to get changes from strategy config
                LoadListViewValuesFromConfiguration();
                //to refresh coin stats due to changed coin selections
                LoadListViewValuesFromCoinStats();
            }
        }

        private void RefreshCoinStats()
        {
            //always load known coins from file
            //CoinChoose may not shown coins it once did if there are no orders
            LoadKnownCoinsFromFile();

            try
            {
                coinInformation = coinApiContext.GetCoinInformation(
                    UserAgent.AgentString,
                    engineConfiguration.StrategyConfiguration.BaseCoin).ToList();
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException) || (ex is Coin.Api.CoinApiException))
                {
                    ShowCoinApiErrorNotification(ex);
                    return;
                }
                throw;
            }
            
            LoadListViewValuesFromCoinStats();
            LoadKnownCoinsFromCoinStats();
            RefreshCoinStatsLabel();
            AutoSizeListViewColumns();
            SuggestCoinsToMine();
        }

        private void ShowCoinApiErrorNotification(Exception ex)
        {
            string siteUrl = this.coinApiContext.GetInfoUrl(engineConfiguration.StrategyConfiguration.BaseCoin);
            string apiUrl = this.coinApiContext.GetApiUrl(engineConfiguration.StrategyConfiguration.BaseCoin);
            string apiName = this.coinApiContext.GetApiName();

            notificationsControl.AddNotification(ex.Message,
                String.Format("Error parsing the {0} JSON API", apiName), () =>
                {
                    Process.Start(apiUrl);
                }, 
                siteUrl);
        }

        private void ShowMobileMinerApiErrorNotification(WebException ex)
        {
            notificationsControl.AddNotification(ex.Message,
                String.Format("Error accessing the MobileMiner API ({0})",  (int)((HttpWebResponse)ex.Response).StatusCode), () =>
                {
                    Process.Start("http://mobileminerapp.com");
                },
                "");
        }

        private void SuggestCoinsToMine()
        {
            if (!applicationConfiguration.SuggestCoinsToMine)
                return;
            if (applicationConfiguration.SuggestionsAlgorithm == ApplicationConfiguration.CoinSuggestionsAlgorithm.None)
                return;
            if (coinInformation == null) //no network connection
                return;

            IEnumerable<Coin.Api.CoinInformation> coinsToMine = GetCoinsToMine();

            foreach (Coin.Api.CoinInformation coin in coinsToMine)
                NotifyCoinToMine(coin);
        }

        private IEnumerable<Coin.Api.CoinInformation> GetCoinsToMine()
        {
            IEnumerable<Coin.Api.CoinInformation> filteredCoins = this.coinInformation;
            if (applicationConfiguration.SuggestionsAlgorithm == ApplicationConfiguration.CoinSuggestionsAlgorithm.SHA256)
                filteredCoins = filteredCoins.Where(c => c.Algorithm.Equals("SHA-256", StringComparison.OrdinalIgnoreCase));
            else if (applicationConfiguration.SuggestionsAlgorithm == ApplicationConfiguration.CoinSuggestionsAlgorithm.Scrypt)
                filteredCoins = filteredCoins.Where(c => c.Algorithm.Equals("Scrypt", StringComparison.OrdinalIgnoreCase));

            IEnumerable<Coin.Api.CoinInformation> orderedCoins = filteredCoins.OrderByDescending(c => c.AverageProfitability);
            switch (engineConfiguration.StrategyConfiguration.MiningBasis)
            {
                case StrategyConfiguration.CoinMiningBasis.Difficulty:
                    orderedCoins = coinInformation.OrderBy(c => c.Difficulty);
                    break;
                case StrategyConfiguration.CoinMiningBasis.Price:
                    orderedCoins = coinInformation.OrderByDescending(c => c.Price);
                    break;
            }

            //added checks for coin.Symbol and coin.Exchange
            //current CoinChoose.com feed for LTC profitability has a NULL exchange for Litecoin
            IEnumerable<Coin.Api.CoinInformation> unconfiguredCoins = orderedCoins.Where(coin => !String.IsNullOrEmpty(coin.Symbol) && !engineConfiguration.CoinConfigurations.Any(config => config.Coin.Symbol.Equals(coin.Symbol, StringComparison.OrdinalIgnoreCase)));
            IEnumerable<Coin.Api.CoinInformation> coinsToMine = unconfiguredCoins.Take(3);
            return coinsToMine;
        }

        private void NotifyCoinToMine(MultiMiner.Coin.Api.CoinInformation coin)
        {
            string value = coin.AverageProfitability.ToString(".#") + "%";
            string noun = "average profitability";

            switch (engineConfiguration.StrategyConfiguration.MiningBasis)
            {
                case StrategyConfiguration.CoinMiningBasis.Difficulty:
                    value = coin.Difficulty.ToString(".####");
                    noun = "difficulty";
                    break;
                case StrategyConfiguration.CoinMiningBasis.Price:
                    value = coin.Price.ToString(".########");
                    noun = "price";
                    break;
            }

            string infoUrl = coinApiContext.GetInfoUrl(engineConfiguration.StrategyConfiguration.BaseCoin);

            notificationsControl.AddNotification(coin.Symbol,
                String.Format("Consider mining {0} ({1} {2})",
                    coin.Symbol, value, noun), () =>
                    {
                        Process.Start(String.Format("https://www.google.com/search?q={0}+{1}+mining+pools",
                            coin.Symbol, coin.Name));
                    }, infoUrl);
        }

        private void RefreshCoinStatsLabel()
        {
            coinChooseSuffixLabel.Text = string.Format("at {0}", DateTime.Now.ToShortTimeString());
        }

        private void LoadKnownCoinsFromCoinStats()
        {
            foreach (Coin.Api.CoinInformation item in coinInformation)
            {
                //find existing known coin or create a knew one
                CryptoCoin knownCoin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(item.Symbol));
                if (knownCoin == null)
                {
                    knownCoin = new CryptoCoin();
                    this.knownCoins.Add(knownCoin);
                }

                knownCoin.Symbol = item.Symbol;
                knownCoin.Name = item.Name;

                //needs to be a case insensitive check to work with both CoinChoose and CoinWarz
                if (item.Algorithm.ToLower().Contains("scrypt"))
                    knownCoin.Algorithm = CoinAlgorithm.Scrypt;
                else
                    knownCoin.Algorithm = CoinAlgorithm.SHA256;

            }
            SaveKnownCoinsToFile();
        }

        private static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private static string KnownCoinsFileName()
        {
            return Path.Combine(AppDataPath(), "KnownCoinsCache.xml");
        }

        private void LoadKnownCoinsFromFile()
        {
            string knownCoinsFileName = KnownCoinsFileName();
            if (File.Exists(knownCoinsFileName))
            {
                knownCoins = ConfigurationReaderWriter.ReadConfiguration<List<CryptoCoin>>(knownCoinsFileName);
                RemoveBunkCoins(knownCoins);
            }
        }

        private static void RemoveBunkCoins(List<CryptoCoin> knownCoins)
        {
            //CoinChoose.com served up ButterFlyCoin as BOC, and then later as BFC
            CryptoCoin badCoin = knownCoins.SingleOrDefault(c => c.Symbol.Equals("BOC", StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
                knownCoins.Remove(badCoin);
        }

        private void SaveKnownCoinsToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(knownCoins, KnownCoinsFileName());
        }

        private void LoadListViewValuesFromCoinStats()
        {
            deviceListView.BeginUpdate();
            try
            {
                //clear all coin stats first
                //there may be coins configured that are no longer returned in the stats
                ClearAllCoinStats();

                if (coinInformation != null) //null if no network connection
                    foreach (Coin.Api.CoinInformation coin in coinInformation)
                        foreach (ListViewItem item in deviceListView.Items)
                        {
                            CoinConfiguration coinConfiguration = CoinConfigurationForListViewItem(item);
                            if ((coinConfiguration != null) && coin.Symbol.Equals(coinConfiguration.Coin.Symbol, StringComparison.OrdinalIgnoreCase))
                            {
                                PopulateCoinStatsForListViewItem(coin, item);
                            }
                        }
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private void PopulateCoinStatsForListViewItem(Coin.Api.CoinInformation coin, ListViewItem item)
        {
            deviceListView.BeginUpdate();
            try
            {
                item.SubItems["Difficulty"].Tag = coin.Difficulty;
                item.SubItems["Difficulty"].Text = coin.Difficulty.ToDifficultyString();

                string unit = "BTC";
                if (!applicationConfiguration.UseCoinWarzApi && (engineConfiguration.StrategyConfiguration.BaseCoin == Coin.Api.BaseCoin.Litecoin))
                    unit = "LTC";

                item.SubItems["Price"].Text = String.Format("{0:.#####} {1}", coin.Price, unit);

                if (miningEngine.Donating && perksConfiguration.ShowExchangeRates
                    //ensure Coinbase is available:
                    && (sellPrices != null))
                {
                    double btcExchangeRate = sellPrices.Subtotal.Amount;
                    double coinExchangeRate = coin.Price * btcExchangeRate;

                    item.SubItems["Exchange"].Tag = coinExchangeRate;
                    item.SubItems["Exchange"].Text = String.Format("${0:0.00}", coinExchangeRate);
                }

                switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                {
                    case StrategyConfiguration.CoinProfitabilityKind.AdjustedProfitability:
                        item.SubItems["Profitability"].Text = Math.Round(coin.AdjustedProfitability, 2) + "%";
                        break;
                    case StrategyConfiguration.CoinProfitabilityKind.AverageProfitability:
                        item.SubItems["Profitability"].Text = Math.Round(coin.AverageProfitability, 2) + "%";
                        break;
                    case StrategyConfiguration.CoinProfitabilityKind.StraightProfitability:
                        item.SubItems["Profitability"].Text = Math.Round(coin.Profitability, 2) + "%";
                        break;
                } 
            }
            finally
            {
                deviceListView.EndUpdate();
            }
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            startupMiningCountdownSeconds--;
            RefreshCountdownLabel();
            if (startupMiningCountdownSeconds == 0)
            {
                startupMiningPanel.Visible = false;
                startupMiningCountdownTimer.Enabled = false;
                Application.DoEvents();

                StartMining();
            }
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            CancelMiningOnStartup();
        }

        private void CancelMiningOnStartup()
        {
            startupMiningCountdownTimer.Enabled = false;
            startupMiningPanel.Visible = false;
            countdownLabel.Visible = false; //or remains visible under Mono
            cancelStartupMiningButton.Visible = false; //or remains visible under Mono
        }

        private void crashRecoveryTimer_Tick(object sender, EventArgs e)
        {
            miningEngine.RelaunchCrashedMiners();
        }

        private void detectDevicesButton_Click(object sender, EventArgs e)
        {
            RefreshDevices();
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

        private void ConfigureStrategies()
        {
            StrategiesForm strategiesForm = new StrategiesForm(engineConfiguration.StrategyConfiguration, knownCoins, 
                applicationConfiguration);
            DialogResult dialogResult = strategiesForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Application.DoEvents();

                engineConfiguration.SaveStrategyConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                SetupCoinStatsTimer();
                
                //so updated profitability is shown
                RefreshCoinStats();

                RefreshStrategiesLabel();
                LoadListViewValuesFromCoinStats();
                UpdateMiningButtons();

                Application.DoEvents();
            }
            else
            {
                engineConfiguration.LoadStrategyConfiguration();
                applicationConfiguration.LoadApplicationConfiguration();
            }
        }

        private void SetupCoinStatsTimer()
        {
            int coinStatsMinutes = 15;
            ApplicationConfiguration.TimerInterval timerInterval = applicationConfiguration.StrategyCheckInterval;

            coinStatsMinutes = timerInterval.ToMinutes();

            coinStatsTimer.Enabled = false;
            coinStatsCountdownTimer.Enabled = false;

            coinStatsCountdownMinutes = coinStatsMinutes;
            coinStatsTimer.Interval = coinStatsMinutes * 60 * 1000;

            coinStatsTimer.Enabled = true;
            coinStatsCountdownTimer.Enabled = true;
        }

        private void coinStatsCountdownTimer_Tick(object sender, EventArgs e)
        {
            coinStatsCountdownMinutes--;
            RefreshStrategiesCountdown();
        }

        private void coinChooseLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.coinApiContext.GetInfoUrl(engineConfiguration.StrategyConfiguration.BaseCoin));
        }

        private void closeApiButton_Click(object sender, EventArgs e)
        {
            HideAdvancedPanel();
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

            closeApiButton.Visible = true;
            apiLogGridView.Visible = true;
            advancedAreaContainer.Panel2Collapsed = false;
            advancedAreaContainer.Panel2.Show();
        }

        private void ShowApiMonitor()
        {
            ShowAdvancedPanel();
            advancedTabControl.SelectedTab = apiMonitorPage;

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void apiMonitorButton_Click(object sender, EventArgs e)
        {
            ShowApiMonitor();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (formLoaded)
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
            Application.Exit();
        }

        private void startMenuItem_Click(object sender, EventArgs e)
        {
            StartMining();
        }

        private void stopMenuItem_Click(object sender, EventArgs e)
        {
            StopMining();
        }

        private void mobileMinerTimer_Tick(object sender, EventArgs e)
        {
            //if we do this with the Settings dialog open the user may have partially entered credentials
            if (!ShowingModalDialog())
            {
                //check for commands first so we can report mining activity after
                CheckForMobileMinerCommands();
                SubmitMobileMinerStats();
            }
        }
        
        private string GetMobileMinerUrl()
        {
            string prefix = "https://";
            if (!applicationConfiguration.MobileMinerUsesHttps)
                prefix = "http://";

            //deprecate api.mobileminerapp.com starting 11/28
            //more expensive for no functional benefit
            //string result = prefix + "api.mobileminerapp.com";

            //if (!OSVersionPlatform.IsWindowsVistaOrHigher())
            //    //SNI SSL not supported on XP
            
            string result = prefix + "mobileminer.azurewebsites.net/api";

            return result;
        }

        private const string mobileMinerApiKey = "P3mVX95iP7xfoI";
        
        //don't show a dialog for a 403 after successful submissions.
        //it's not ideal but there have been two reports now of this
        //being triggered by someone who has valid credentials, and
        //i've seen it myself as well
        private bool mobileMinerSuccess = false;
        private void SubmitMobileMinerStats()
        {
            //are remote monitoring enabled?
            if (!applicationConfiguration.MobileMinerMonitoring)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            List<MultiMiner.MobileMiner.Api.MiningStatistics> statisticsList = new List<MobileMiner.Api.MiningStatistics>();

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                List<DeviceInformationResponse> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                    continue;

                foreach (DeviceInformationResponse deviceInformation in deviceInformationList)
                {
                    MultiMiner.MobileMiner.Api.MiningStatistics miningStatistics = new MobileMiner.Api.MiningStatistics();

                    //set CoinName first as it is used in PopulateMiningStatistics
                    PopulateMiningStatistics(miningStatistics, deviceInformation, GetCoinNameForApiContext(minerProcess.ApiContext));

                    statisticsList.Add(miningStatistics);
                }
            }

            if (statisticsList.Count > 0)
            {
                if (submitMiningStatisticsDelegate == null)
                    submitMiningStatisticsDelegate = SubmitMiningStatistics;

                submitMiningStatisticsDelegate.BeginInvoke(statisticsList, null, null);
            }
        }

        private void PopulateMiningStatistics(MultiMiner.MobileMiner.Api.MiningStatistics miningStatistics, DeviceInformationResponse deviceInformation,
            string coinName)
        {
            miningStatistics.MinerName = "MultiMiner";
            miningStatistics.CoinName = coinName;
            CryptoCoin coin = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinName)).Coin;
            miningStatistics.CoinSymbol = coin.Symbol;

            if (coin.Algorithm == CoinAlgorithm.Scrypt)
                miningStatistics.Algorithm = "scrypt";
            else if (coin.Algorithm == CoinAlgorithm.SHA256)
                miningStatistics.Algorithm = "SHA-256";

            miningStatistics.PopulateFrom(deviceInformation);
        }

        private Action<List<MultiMiner.MobileMiner.Api.MiningStatistics>> submitMiningStatisticsDelegate;

        private void SubmitMiningStatistics(List<MultiMiner.MobileMiner.Api.MiningStatistics> statisticsList)
        {
            try
            {
                MobileMiner.Api.ApiContext.SubmitMiningStatistics(GetMobileMinerUrl(), mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    Environment.MachineName, statisticsList);
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
                                if (InvokeRequired)
                                    BeginInvoke((Action)(() =>
                                    {
                                        //code to update UI
                                        ConfigureSettings();
                                    }));
                                else
                                    ConfigureSettings();
                            }
                        }
                    }
                    else
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

        private void SubmitMobileMinerNotification(string text)
        {
            //are remote notifications enabled?
            if (!applicationConfiguration.MobileMinerPushNotifications)
                return;

            //is MobileMiner configured?
            if (string.IsNullOrEmpty(applicationConfiguration.MobileMinerApplicationKey) ||
                string.IsNullOrEmpty(applicationConfiguration.MobileMinerEmailAddress))
                return;

            if (submitNotificationDelegate == null)
                submitNotificationDelegate = SubmitNotification;

            submitNotificationDelegate.BeginInvoke(text, null, null);
        }

        private Action<string> submitNotificationDelegate;

        private void SubmitNotification(string text)
        {
            try
            {
                string notificationText = String.Format("{0}: {1}", Environment.MachineName, text);
                MobileMiner.Api.ApiContext.SubmitNotifications(GetMobileMinerUrl(), mobileMinerApiKey,
                        applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                        new List<string> { notificationText });
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

        private static bool ShowingModalDialog()
        {
            foreach (Form f in Application.OpenForms)
                if (f.Modal)
                    return true;

            return false;
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

            checkForRemoteCommandsDelegate.BeginInvoke(null, null);
        }

        private Action checkForRemoteCommandsDelegate;

        private void GetRemoteCommands()
        {
            List<MobileMiner.Api.RemoteCommand> commands = new List<MobileMiner.Api.RemoteCommand>();

            try
            {
                commands = MobileMiner.Api.ApiContext.GetCommands(GetMobileMinerUrl(), mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    Environment.MachineName);
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
                                        if (InvokeRequired)
                                            BeginInvoke((Action)(() =>
                                            {
                                                //code to update UI
                                                ConfigureSettings();
                                            }));
                                        else
                                            ConfigureSettings();
                                    }
                                }
                            }
                            else
                            {
                                if (InvokeRequired)
                                    BeginInvoke((Action)(() =>
                                    {
                                        //code to update UI
                                        ShowMobileMinerApiErrorNotification(webException);
                                    }));
                                else
                                    ShowMobileMinerApiErrorNotification(webException);
                            }
                        }
                    }

                    return;
                }
                throw;
            }

            if (InvokeRequired)
                BeginInvoke((Action<List<MobileMiner.Api.RemoteCommand>>)((c) => ProcessRemoteCommands(c)), commands);
            else
                ProcessRemoteCommands(commands);
        }

        private void ProcessRemoteCommands(List<MobileMiner.Api.RemoteCommand> commands)
        {
            if (commands.Count > 0)
            {
                MobileMiner.Api.RemoteCommand command = commands.First();

                //check this before actually executing the command
                //point being, say for some reason it takes 2 minutes to restart mining
                //if we check for commands again in that time, we don't want to process it again
                if (processedCommandIds.Contains(command.Id))
                    return;

                processedCommandIds.Add(command.Id);

                if (command.CommandText.Equals("START", StringComparison.OrdinalIgnoreCase))
                {
                    SaveChanges(); //necessary to ensure device configurations exist for devices
                    StartMining();
                }
                else if (command.CommandText.Equals("STOP", StringComparison.OrdinalIgnoreCase))
                    StopMining();
                else if (command.CommandText.Equals("RESTART", StringComparison.OrdinalIgnoreCase))
                {
                    StopMining();
                    SaveChanges(); //necessary to ensure device configurations exist for devices
                    StartMining();
                }

                if (deleteRemoteCommandDelegate == null)
                    deleteRemoteCommandDelegate = DeleteRemoteCommand;

                deleteRemoteCommandDelegate.BeginInvoke(command, null, null);
            }
        }

        private Action<MobileMiner.Api.RemoteCommand> deleteRemoteCommandDelegate;

        private void DeleteRemoteCommand(MobileMiner.Api.RemoteCommand command)
        {
            MobileMiner.Api.ApiContext.DeleteCommand(GetMobileMinerUrl(), mobileMinerApiKey,
                                applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                                Environment.MachineName, command.Id);
        }

        private void processLogButton_Click(object sender, EventArgs e)
        {
            ShowProcessLog();
        }

        private void ShowProcessLog()
        {
            ShowAdvancedPanel();
            advancedTabControl.SelectedTab = processLogPage;

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void ShowHistory()
        {
            ShowAdvancedPanel();
            advancedTabControl.SelectedTab = historyPage;

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
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
            PopulateQuickSwitchMenu();
        }

        private void PopulateQuickSwitchMenu()
        {
            quickCoinMenu.Items.Clear();

            quickSwitchItem.DropDown = quickCoinMenu;
            quickSwitchPopupItem.DropDown = quickCoinMenu;
            
            foreach (CoinConfiguration coinConfiguration in engineConfiguration.CoinConfigurations.Where(c => c.Enabled))
            {
                ToolStripMenuItem coinSwitchItem = new ToolStripMenuItem() 
                { 
                    Text = coinConfiguration.Coin.Name, 
                    Tag = coinConfiguration.Coin.Symbol 
                };
                coinSwitchItem.Click += HandleQuickSwitchClick;

                quickCoinMenu.Items.Add(coinSwitchItem);
            }
        }

        private void HandleQuickSwitchClick(object sender, EventArgs e)
        {
            bool wasMining = miningEngine.Mining;
            StopMining();
            
            string coinSymbol = (string)((ToolStripMenuItem)sender).Tag;

            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Coin.Symbol.Equals(coinSymbol));

            SetAllDevicesToCoin(coinConfiguration);

            engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = false; 

            engineConfiguration.SaveDeviceConfigurations();
            engineConfiguration.SaveStrategyConfiguration();

            LoadListViewValuesFromConfiguration();
            LoadListViewValuesFromCoinStats(); 

            AutoSizeListViewColumns();

            if (wasMining)
                StartMining();
            else
                //so the Start button becomes enabled if we now have a valid config
                UpdateMiningButtons();
        }

        private void SetAllDevicesToCoin(CoinConfiguration coinConfiguration)
        {
            engineConfiguration.DeviceConfigurations.Clear();

            for (int i = 0; i < devices.Count; i++)
            {
                ListViewItem listViewItem = deviceListView.Items[i];

                Device device = devices[i];
                CryptoCoin currentCoin = knownCoins.SingleOrDefault(c => c.Name.Equals(listViewItem.SubItems["Coin"].Text));

                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                deviceConfiguration.Assign(device);

                if (coinConfiguration.Coin.Algorithm == CoinAlgorithm.Scrypt)
                {
                    if (device.Kind == DeviceKind.GPU)
                        deviceConfiguration.CoinSymbol = coinConfiguration.Coin.Symbol;
                    else
                        deviceConfiguration.CoinSymbol = currentCoin == null ? String.Empty : currentCoin.Symbol;
                }
                else
                {
                    deviceConfiguration.CoinSymbol = coinConfiguration.Coin.Symbol;
                }

                deviceConfiguration.Enabled = listViewItem.Checked;

                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }           
        }

        private void advancedMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchItem.Enabled = engineConfiguration.CoinConfigurations.Where(c => c.Enabled).Count() > 0;

            //
            dynamicIntensityButton.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;
            dynamicIntensitySeparator.Visible = !engineConfiguration.XgminerConfiguration.DisableGpu;
        }

        private void notificationsControl1_NotificationsChanged(object sender)
        {
            notificationsControl.Visible = notificationsControl.NotificationCount() > 0;
            if (notificationsControl.Visible)
                notificationsControl.BringToFront();
        }

        private void updateCheckTimer_Tick(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
        
        private void CheckForUpdates()
        {
            PlatformID concretePlatform = OSVersionPlatform.GetConcretePlatform();

            //we cannot auto update the .app file (yet)
            CheckForMultiMinerUpdates();

            //we cannot auto install miners on Unix (yet)
            if (applicationConfiguration.CheckForMinerUpdates && (concretePlatform != PlatformID.Unix))
                TryToCheckForMinerUpdates();
        }

        private void TryToCheckForMinerUpdates()
        {
            try
            {
                CheckForMinerUpdates();
            }
            catch (ArgumentException ex)
            {
                string error = String.Format("Error checking for {0} updates", "bfgminer");
                notificationsControl.AddNotification(error, error, () =>
                {
                }, "");
            }
        }

        private const int bfgminerNotificationId = 100;
        private const int multiMinerNotificationId = 102;

        private void CheckForMultiMinerUpdates()
        {
            string availableMinerVersion = String.Empty;
            try
            {
                availableMinerVersion = Engine.Installer.GetAvailableMinerVersion();
            }
            catch (WebException ex)
            {
                //downloads website is down
                return;
            }

            string installedMinerVersion = Engine.Installer.GetInstalledMinerVersion();
            
            if (!AutomaticUpgradeAllowed(installedMinerVersion, availableMinerVersion))
                return;

            if (ThisVersionGreater(availableMinerVersion, installedMinerVersion))
            {
                notificationsControl.AddNotification(multiMinerNotificationId.ToString(),
                    String.Format("MultiMiner version {0} is available ({1} installed)",
                        availableMinerVersion, installedMinerVersion), () =>
                        {
                            bool wasMining = miningEngine.Mining;
                            StopMining();
                            InstallMultiMiner();
                            if (wasMining)
                                StartMining();
                        }, "http://releases.multiminerapp.com");
            }
        }

        private bool AutomaticUpgradeAllowed(string installedMinerVersion, string availableMinerVersion)
        {
            //don't automatically prompt to upgrade from 1.0 to 2.0
            Version sourceVersion = new Version(installedMinerVersion);
            Version targetVersion = new Version(availableMinerVersion);
            return sourceVersion.Major == targetVersion.Major;
        }

        private static bool ThisVersionGreater(string thisVersion, string thatVersion)
        {
            Version thisVersionObj = new Version(thisVersion);
            Version thatVersionObj = new Version(thatVersion);

            return thisVersionObj > thatVersionObj;
        }

        private void CheckForMinerUpdates()
        {
            if (!MinerIsInstalled())
                return;

            string availableMinerVersion = GetAvailableBackendVersion();
            if (String.IsNullOrEmpty(availableMinerVersion))
                return;

            string installedMinerVersion = Xgminer.Installer.GetInstalledMinerVersion(MinerPath.GetPathToInstalledMiner());
            if (ThisVersionGreater(availableMinerVersion, installedMinerVersion))
                DisplayMinerUpdateNotification(availableMinerVersion, installedMinerVersion);
        }

        private void DisplayMinerUpdateNotification(string availableMinerVersion, string installedMinerVersion)
        {
            int notificationId = bfgminerNotificationId;

            string informationUrl = "https://github.com/luke-jr/bfgminer/blob/bfgminer/NEWS";

            string minerName = MinerPath.GetMinerName();
            notificationsControl.AddNotification(notificationId.ToString(),
                String.Format("{0} version {1} is available ({2} installed)",
                    minerName, availableMinerVersion, installedMinerVersion), () =>
                {
                    bool wasMining = miningEngine.Mining;

                    //only stop mining if this is the engine being used
                    if (wasMining)
                        StopMining();

                    InstallMiner();

                    //only start mining if we stopped mining
                    if (wasMining)
                        StartMining();
                }, informationUrl);
        }

        private static string GetAvailableBackendVersion()
        {
            string result = String.Empty;
            try
            {
                result = Xgminer.Installer.GetAvailableMinerVersion();
            }
            catch (WebException ex)
            {
                //downloads website is down
            }
            return result;
        }

        private static void InstallMultiMiner()
        {
            ProgressForm progressForm = new ProgressForm("Downloading and installing MultiMiner from " + Engine.Installer.GetMinerDownloadRoot());
            progressForm.Show();

            //for Mono - show the UI
            Application.DoEvents();
            Thread.Sleep(25);
            Application.DoEvents();
            try
            {
                MultiMiner.Engine.Installer.InstallMiner(Path.GetDirectoryName(Application.ExecutablePath));
            }
            finally
            {
                progressForm.Close();
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void idleTimer_Tick(object sender, EventArgs e)
        {
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                return; //idle detection code uses User32.dll

            if (applicationConfiguration.AutoSetDesktopMode && miningEngine.Mining)
            {
                TimeSpan idleTimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount - IdleTimeFinder.GetLastInputTime());

                const int idleMinutesForDesktopMode = 2;

                //if idle for more than 1 minute, disable Desktop Mode
                if (idleTimeSpan.TotalMinutes > idleMinutesForDesktopMode)
                {
                    if (engineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        EnableDesktopMode(false);
                        RestartMiningIfMining();
                    }
                }
                //else if idle for less than the idleTimer interval, enable Desktop Mode
                else if (idleTimeSpan.TotalMilliseconds <= idleTimer.Interval)
                {
                    if (!engineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        EnableDesktopMode(true);
                        RestartMiningIfMining();
                    }
                }
            }
        }

        private void EnableDesktopMode(bool enabled)
        {
            engineConfiguration.XgminerConfiguration.DesktopMode = enabled;
            dynamicIntensityButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;
            engineConfiguration.SaveMinerConfiguration();
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

        private void restartTimer_Tick(object sender, EventArgs e)
        {
            RestartMiningIfMining();
        }

        private void RestartMiningIfMining()
        {
            if (miningEngine.Mining)
            {
                StopMining();
                StartMining();
            }
        }

        private void minerSummaryTimer_Tick(object sender, EventArgs e)
        {
            PopulateSummaryInfoFromProcesses();
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
            if (e.Button == MouseButtons.Right)
                if (deviceListView.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    string currentCoin = GetCurrentlySelectedCoinName();

                    CheckCoinInPopupMenu(currentCoin);

                    coinPopupMenu.Show(Cursor.Position);
                }
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

        private string GetCurrentlySelectedCoinName()
        {
            string currentCoin = null;
            foreach (ListViewItem selectedItem in deviceListView.SelectedItems)
            {
                string itemValue = selectedItem.SubItems["Coin"].Text;
                if (!String.IsNullOrEmpty(itemValue))
                {
                    currentCoin = itemValue;
                    break;
                }
            }
            return currentCoin;
        }

        private void deviceListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!updatingListView)
                UpdateChangesButtons(true);
        }

        private void dynamicIntensityButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.XgminerConfiguration.DesktopMode = dynamicIntensityButton.Checked;
            engineConfiguration.SaveMinerConfiguration();
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
            RefreshDevices();
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
            quickSwitchPopupItem.Enabled = engineConfiguration.CoinConfigurations.Where(c => c.Enabled).Count() > 0;
        }

        private void quickSwitchPopupItem_DropDownOpening(object sender, EventArgs e)
        {
            PopulateQuickSwitchMenu();
        }

        private bool briefMode = false;
        private void detailsToggleButton_ButtonClick(object sender, EventArgs e)
        {
            SetBriefMode(!briefMode);
            RefreshDetailsToggleButton();
        }

        private void RefreshDetailsToggleButton()
        {
            if (briefMode)
                detailsToggleButton.Text = "▾ More details";
            else
                detailsToggleButton.Text = "▴ Fewer details";
        }

        private void SetBriefMode(bool newBriefMode)
        {
            briefMode = newBriefMode;
            RefreshDetailsToggleButton();

            //do this before adjusting the window size so we can base it on column widths
            AutoSizeListViewColumns();

            if (briefMode)
            {
                HideAdvancedPanel();
                WindowState = FormWindowState.Normal;

                int newWidth = 0;

                foreach (ColumnHeader column in deviceListView.Columns)
                    newWidth += column.Width;
                newWidth += 40; //needs to be pretty wide for e.g. Aero Basic

                newWidth = Math.Max(newWidth, 300);

                Size = new Size(newWidth, 400);

            } else
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    //use Math.Max so it won't size smaller to show more
                    Size = new Size(Math.Max(Size.Width, 720), Math.Max(Size.Height, 500));
                }
            }

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

        private void SetupStatusBarLabelLayouts()
        {
            sha256RateLabel.AutoSize = true;
            sha256RateLabel.Spring = true;

            scryptRateLabel.AutoSize = true;
            scryptRateLabel.Padding = new Padding(12, 0, 0, 0);

            deviceTotalLabel.AutoSize = true;
            deviceTotalLabel.Padding = new Padding(12, 0, 0, 0);
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

                if (view == View.Details)
                    AutoSizeListViewColumns();
            }
            finally
            {
                updatingListView = false;
            }
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
                e.Value = GetReallyShortDateTimeFormat((DateTime)e.Value);
                e.FormattingApplied = true;
            }
        }

        private static string GetReallyShortDateTimeFormat(DateTime dateTime)
        {
            return String.Format("{0} {1}", dateTime.ToReallyShortDateString(), dateTime.ToShortTimeString());
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (e.Value != null)
                {
                    e.Value = GetReallyShortDateTimeFormat((DateTime)e.Value);
                    e.FormattingApplied = true;
                }
            }
        }

        private void apiLogGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Value = GetReallyShortDateTimeFormat((DateTime)e.Value);
                e.FormattingApplied = true;
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            AutoSizeListViewColumns();
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            StopMining();

            //refresh stats from Coin API so the Restart button can be used as a way to
            //force MultiMiner to apply updated mining strategies
            RefreshCoinStats();
            
            StartMining();
        }

        private void perksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void ConfigurePerks()
        {
            PerksForm perksForm = new PerksForm(perksConfiguration);
            DialogResult dialogResult = perksForm.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                perksConfiguration.SavePerksConfiguration();
                if (perksConfiguration.PerksEnabled && perksConfiguration.ShowExchangeRates)
                    RefreshExchangeRates();
                LoadListViewValuesFromCoinStats();
                RefreshIncomeSummary();
                AutoSizeListViewColumns();
            }
            else
                perksConfiguration.LoadPerksConfiguration();
        }

        private void perksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConfigurePerks();
        }

        private void exchangeRateTimer_Tick(object sender, EventArgs e)
        {
            RefreshExchangeRates();
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
    }
}
