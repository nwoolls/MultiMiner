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

namespace MultiMiner.Win
{
    public partial class MainForm : Form
    {
        private List<Coinchoose.Api.CoinInformation> coinInformation;
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
        private Dictionary<int, int> lastAcceptedShares = new Dictionary<int,int>();
        private bool settingsLoaded = false;

        public MainForm()
        {
            InitializeComponent();
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
            CalculateAcceptedSharesForProcess(ea);

            logProcessCloseArgsBindingSource.Position = logProcessCloseArgsBindingSource.Add(ea);

            while (logProcessCloseArgsBindingSource.Count > 1000)
                logProcessCloseArgsBindingSource.RemoveAt(0);

            LogProcessCloseToFile(ea);
        }

        private void CalculateAcceptedSharesForProcess(LogProcessCloseArgs ea)
        {
            int acceptedShares = 0;
            foreach (int deviceIndex in ea.DeviceIndexes)
            {
                //key won't be there if process is stopped before it starts accepting shares
                if (lastAcceptedShares.ContainsKey(deviceIndex))
                    acceptedShares += lastAcceptedShares[deviceIndex];
            }
            ea.AcceptedShares = acceptedShares;
        }

        private void LogProcessCloseToFile(LogProcessCloseArgs ea)
        {
            const string logFileName = "MiningLog.json";
            LogObjectToFile(ea, logFileName);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupGridColumns();

            //something in the VS IDE keeps moving this
            enabledColumn.DisplayIndex = 0;            

            logLaunchArgsBindingSource.DataSource = logCloseEntries;

            const int mobileMinerInterval = 32; //seconds
            mobileMinerTimer.Interval = mobileMinerInterval * 1000;

            engineConfiguration.LoadStrategyConfiguration(); //needed before refreshing coins
            engineConfiguration.LoadCoinConfigurations(); //needed before refreshing coins
            applicationConfiguration.LoadApplicationConfiguration(); //needed before refreshing coins
            SetupNotificationsControl(); //needed before refreshing coins
            RefreshCoinStats();

            CheckAndShowGettingStarted();
            
            LoadSettings();

            RefreshBackendLabel();

            RefreshCoinComboBox();

            PositionCoinchooseLabels();

            apiLogEntryBindingSource.DataSource = apiLogEntries;

            miningEngine.LogProcessClose += LogProcessClose;
            miningEngine.LogProcessLaunch += LogProcessLaunch;
            miningEngine.ProcessLaunchFailed += ProcessLaunchFailed;
            miningEngine.ProcessAuthenticationFailed += ProcessAuthenticationFailed;
            logLaunchArgsBindingSource.DataSource = logLaunchEntries;
            logProcessCloseArgsBindingSource.DataSource = logCloseEntries;

            saveButton.Enabled = false;
            cancelButton.Enabled = false;

            PositionCoinStatsLabel();

            if (!HasMinersInstalled())
                CancelMiningOnStartup();

            //check for disowned miners before refreshing devices
            if (applicationConfiguration.DetectDisownedMiners)
                CheckForDisownedMiners();

            CheckAndDownloadMiners();
            
            SetupAutoUpdates();

            RefreshDevices();
            
            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];

            formLoaded = true;
        }

        private void PositionCoinStatsLabel()
        {
            //manually position - IDE has screwed this up repeatedly
            const int padding = 4;
            coinStatsLabel.Location = new Point(footerPanel.Width - coinStatsLabel.Size.Width - padding, coinChoosePrefixLabel.Location.Y);
            coinStatsLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        }

        private void SetupAutoUpdates()
        {
            updateCheckTimer.Interval = 3600000; //1h
            updateCheckTimer.Enabled = true;
            CheckForUpdates();
        }

        private void SetupNotificationsControl()
        {
            notificationsControl = new NotificationsControl();
            notificationsControl.Visible = false;
            notificationsControl.Height = 143;
            notificationsControl.Width = 320;
            notificationsControl.NotificationsChanged += notificationsControl1_NotificationsChanged;
            notificationsControl.Parent = advancedAreaContainer.Panel1;
            const int offset = 2;

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                //adjust for different metrics/layout under OS X/Unix
                notificationsControl.Width += 50;

            notificationsControl.Left = notificationsControl.Parent.Width - notificationsControl.Width - offset;
            notificationsControl.Top = notificationsControl.Parent.Height - notificationsControl.Height - offset;
            notificationsControl.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        }

        private void ProcessLaunchFailed(object sender, LaunchFailedArgs ea)
        {
            this.BeginInvoke((Action)(() =>
            {
                //code to update UI
                StopMining();
                MessageBox.Show(ea.Reason, "Launching Miner Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                wizardForm.CreateConfigurations(out newEngineConfiguration, out newApplicationConfiguration);
                
                this.engineConfiguration = newEngineConfiguration;
                this.applicationConfiguration = newApplicationConfiguration;

                this.engineConfiguration.SaveCoinConfigurations();
                this.engineConfiguration.SaveMinerConfiguration();
                this.applicationConfiguration.SaveApplicationConfiguration();
            }
        }

        private void CheckAndDownloadMiners()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                return; //can't auto download binaries on Linux

            bool hasMiners = HasMinersInstalled();

            if (!hasMiners)
            {
                InstallMinerForm minerForm = new InstallMinerForm();

                DialogResult messageBoxResult = minerForm.ShowDialog();
                if (messageBoxResult == System.Windows.Forms.DialogResult.Yes)
                {
                    if ((minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Cgminer) ||
                        (minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Both))
                    {
                        MinerBackend minerBackend = MinerBackend.Cgminer;
                        InstallMiner(minerBackend);
                    }

                    if ((minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Bfgminer) ||
                        (minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Both))
                    {
                        MinerBackend minerBackend = MinerBackend.Bfgminer;
                        InstallMiner(minerBackend);
                    }

                    if ((minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Cgminer) ||
                        (minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Both))
                    {
                        engineConfiguration.XgminerConfiguration.MinerBackend = MinerBackend.Cgminer;
                    }
                    else if (minerForm.SelectedOption == InstallMinerForm.MinerInstallOption.Bfgminer)
                    {
                        engineConfiguration.XgminerConfiguration.MinerBackend = MinerBackend.Bfgminer;
                    }

                    engineConfiguration.SaveMinerConfiguration();
                }
            }
        }
        
        private static void InstallMiner(MinerBackend minerBackend)
        {
            string minerName = MinerPath.GetMinerName(minerBackend);

            ProgressForm progressForm = new ProgressForm("Downloading and installing " + minerName + " from " + Xgminer.Installer.GetMinerDownloadRoot(minerBackend));
            progressForm.Show();

            //for Mono - show the UI
            Application.DoEvents();
            Thread.Sleep(25); 
            Application.DoEvents();
            try
            {
                string minerPath = Path.Combine("Miners", minerName);
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
                Xgminer.Installer.InstallMiner(minerBackend, destinationFolder);
            }
            finally
            {
                progressForm.Close();
            }
        }

        private static bool HasMinersInstalled()
        {
            bool hasMiners = MinerIsInstalled(MinerBackend.Cgminer);

            if (!hasMiners)
                hasMiners = MinerIsInstalled(MinerBackend.Bfgminer);

            return hasMiners;
        }
        
        private static bool MinerIsInstalled(MinerBackend minerBackend)
        {
            string path = MinerPath.GetPathToInstalledMiner(minerBackend);
            return File.Exists(path);
        }

        private void CheckForDisownedMiners()
        {
            string minerName = MinerPath.GetMinerName(engineConfiguration.XgminerConfiguration.MinerBackend);

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
            //customized FillWeight doesn't behave properly under Mono on OS X
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
                foreach (DataGridViewColumn column in deviceGridView.Columns)
                    column.FillWeight = 100;
        }

        private void PositionCoinchooseLabels()
        {
            //so things align correctly under Mono
            coinChooseLinkLabel.Left = coinChoosePrefixLabel.Left + coinChoosePrefixLabel.Width;
            coinChooseSuffixLabel.Left = coinChooseLinkLabel.Left + coinChooseLinkLabel.Width;
        }

        private void RefreshDevices()
        {
            try
            {
                devices = GetDevices();
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

            //there needs to be a device config for each device
            AddMissingDeviceConfigurations();

            deviceBindingSource.DataSource = devices;
            LoadGridValuesFromConfiguration();
            CheckAndHideNameColumn();
            deviceTotalLabel.Text = String.Format("{0} device(s)", devices.Count);
        }

        //each device needs to have a DeviceConfiguration
        //this will add any missing ones after populating devices
        //for instance if the user starts up the app with a new device
        private void AddMissingDeviceConfigurations()
        {
            for (int i = engineConfiguration.DeviceConfigurations.Count; i < devices.Count; i++)
            {
                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();
                deviceConfiguration.DeviceIndex = i;
                deviceConfiguration.Enabled = true;
                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }
        }
        
        private void ShowNotInstalledMinerWarning()
        {
            bool showWarning = true;

            if (OSVersionPlatform.GetConcretePlatform() != PlatformID.Unix)
            {
                MinerBackend minerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
                string minerName = MinerPath.GetMinerName(minerBackend);

                DialogResult dialogResult = MessageBox.Show(String.Format(
                    "The miner specified in your settings, {0}, is not installed. " +
                    "Would you like to download and install {0} now?", minerName), "Miner Not Found",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    InstallMiner(minerBackend);
                    RefreshDevices();
                    showWarning = false;
                }
            }

            if (showWarning)
            {                
                MessageBox.Show("The miner specified in your settings was not found. Please go to https://github.com/nwoolls/multiminer for instructions on installing either cgminer or bfgminer.",
                    "Miner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigureDevicesForNewUser()
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single();

            for (int i = 0; i < devices.Count; i++)
            {
                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();
                deviceConfiguration.CoinSymbol = coinConfiguration.Coin.Symbol;
                deviceConfiguration.DeviceIndex = i;
                deviceConfiguration.Enabled = true;
                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            engineConfiguration.SaveDeviceConfigurations();
            UpdateMiningButtons();
        }

        private void CheckAndHideNameColumn()
        {
            nameColumn.Visible = devices.Count(d => !string.IsNullOrEmpty(d.Name)) > 0;
        }

        private void LoadSettings()
        {
            engineConfiguration.LoadCoinConfigurations();
            engineConfiguration.LoadDeviceConfigurations();
            engineConfiguration.LoadMinerConfiguration();
            engineConfiguration.LoadStrategyConfiguration();

            coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
            coinColumn.DisplayStyle = coinColumn.ReadOnly ? DataGridViewComboBoxDisplayStyle.Nothing : DataGridViewComboBoxDisplayStyle.DropDownButton;

            RefreshStrategiesLabel();
            RefreshStrategiesCountdown();

            desktopModeButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;

            applicationConfiguration.LoadApplicationConfiguration();

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

        private void RefreshCountdownLabel()
        {
            countdownLabel.Text = string.Format("Mining will start automatically in {0} seconds...", startupMiningCountdownSeconds);    
        }

        private List<Device> GetDevices()
        {
            MinerConfiguration minerConfiguration = new MinerConfiguration();

            minerConfiguration.MinerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
            minerConfiguration.ExecutablePath = MinerPath.GetPathToInstalledMiner(minerConfiguration.MinerBackend);
            minerConfiguration.ErupterDriver = engineConfiguration.XgminerConfiguration.ErupterDriver;
            
            minerConfiguration.DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu;

            Miner miner = new Miner(minerConfiguration);
            return miner.DeviceList();
        }

        private void ConfigureCoins()
        {
            deviceGridView.EndEdit(); //so the coin combo is immediately refreshed even if focused
            CoinsForm coinsForm = new CoinsForm(engineConfiguration.CoinConfigurations, knownCoins);
            DialogResult dialogResult = coinsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                engineConfiguration.SaveCoinConfigurations();
                RefreshCoinComboBox();
                //SaveChanges() will restart mining if needed
                SaveChanges();
            }
            else
                engineConfiguration.LoadCoinConfigurations();
        }

        private void RefreshCoinComboBox()
        {
            //remove any Coin values from the grid that may now be invalid
            RemoveInvalidCoinValues();
                        
            //remove any Coin combo items that are no longer valid
            //cannot clear as there are values bound in the grid
            RemoveInvalidCoinComboItems();

            AddMissingCoinComboItems();

            coinColumn.Items.Add(string.Empty);
        }

        private void AddMissingCoinComboItems()
        {
            foreach (CoinConfiguration configuration in engineConfiguration.CoinConfigurations.Where(c => c.Enabled))
                if (!coinColumn.Items.Contains(configuration.Coin.Name))
                    coinColumn.Items.Add(configuration.Coin.Name);
        }

        private void RemoveInvalidCoinComboItems()
        {
            for (int i = coinColumn.Items.Count - 1; i >= 0; i--)
                if (engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Enabled && c.Coin.Name.Equals(coinColumn.Items[i])) == null)
                    coinColumn.Items.RemoveAt(i);
        }

        private void RemoveInvalidCoinValues()
        {
            foreach (DataGridViewRow row in deviceGridView.Rows)
                if (engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Enabled && c.Coin.Name.Equals(row.Cells[coinColumn.Index].Value)) == null)
                    row.Cells[coinColumn.Index].Value = string.Empty;

            ClearCoinStatsForDisabledCoins();
        }

        private void deviceGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (deviceGridView.CurrentCell.RowIndex >= 0)
                if (!deviceGridView.CurrentCell.ReadOnly)
                    deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            //SaveChanges() will restart mining if needed
            SaveChanges();
        }

        private void SaveChanges()
        {
            SaveGridValuesToConfiguration();
            engineConfiguration.SaveDeviceConfigurations();
            LoadGridValuesFromConfiguration();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;

            RestartMiningIfMining();

            UpdateMiningButtons();
            ClearMinerStatsForDisabledCoins();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.LoadDeviceConfigurations();
            LoadGridValuesFromConfiguration();

            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void deviceGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!deviceGridView.Columns[e.ColumnIndex].ReadOnly)
            {
                saveButton.Enabled = true;
                cancelButton.Enabled = true;

                if (this.coinInformation != null)
                    LoadGridValuesFromCoinStats();

                UpdateMiningButtons();
            }
        }

        private void SaveGridValuesToConfiguration()
        {
            deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            engineConfiguration.DeviceConfigurations.Clear();

            for (int i = 0; i < devices.Count; i++)
            {
                DataGridViewRow gridRow = deviceGridView.Rows[i];

                //pull this from coin configurations, not known coins, may not be in CoinChoose
                string gridRowValue = (string)gridRow.Cells[coinColumn.Index].Value;
                CryptoCoin coin = null;
                if (!String.IsNullOrEmpty(gridRowValue))
                    coin = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(gridRowValue)).Coin;

                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                deviceConfiguration.DeviceIndex = i;
                deviceConfiguration.CoinSymbol = coin == null ? string.Empty : coin.Symbol;
                object cellValue = gridRow.Cells[enabledColumn.Index].Value;
                deviceConfiguration.Enabled = cellValue == null ? true : (bool)cellValue;

                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);

            }
        }

        private void LoadGridValuesFromConfiguration()
        {
            bool saveEnabled = saveButton.Enabled;
            try
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    DataGridViewRow gridRow = deviceGridView.Rows[i];

                    DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(
                        c => (c.DeviceIndex == i));

                    if (deviceConfiguration != null)
                    {
                        //ensure the coin configuration still exists
                        CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Coin.Symbol.Equals(deviceConfiguration.CoinSymbol));
                        if (coinConfiguration != null)
                            gridRow.Cells[coinColumn.Index].Value = coinConfiguration.Coin.Name;
                        else
                            gridRow.Cells[coinColumn.Index].Value = string.Empty;

                        gridRow.Cells[enabledColumn.Index].Value = deviceConfiguration.Enabled;
                    }
                    else
                    {
                        gridRow.Cells[coinColumn.Index].Value = string.Empty;
                        gridRow.Cells[enabledColumn.Index].Value = true;
                    }
                }
            }
            finally
            {
                //restore button states after
                saveButton.Enabled = saveEnabled;
                cancelButton.Enabled = saveEnabled;
            }

            //leaving the enabledColumn focused can cause an artifact where it looks unchecked
            //but isn't
            if (deviceGridView.RowCount > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];

            RefreshGridColorsFromConfiguration();
        }

        private void RefreshGridColorsFromConfiguration()
        {
            for (int i = 0; i < deviceGridView.Rows.Count; i++)
            {
                DataGridViewRow gridRow = deviceGridView.Rows[i];
                DeviceConfiguration deviceConfiguration = engineConfiguration.DeviceConfigurations.SingleOrDefault(
                    c => (c.DeviceIndex == i));

                foreach (DataGridViewCell gridCell in gridRow.Cells)
                {
                    //deviceConfiguration may be null - may be a device that hasn't been configured
                    if ((deviceConfiguration != null) && !deviceConfiguration.Enabled)                        
                        gridCell.Style.ForeColor = SystemColors.GrayText;
                    else
                        gridCell.Style.ForeColor = SystemColors.WindowText;
                }
            }
        }

        private void UpdateMiningButtons()
        {
            startButton.Enabled = MiningConfigurationValid() && !miningEngine.Mining;

            stopButton.Enabled = miningEngine.Mining;
            startMenuItem.Enabled = startButton.Enabled;
            stopMenuItem.Enabled = stopButton.Enabled;
            //allow clicking Detect Devices with invalid configuration
            detectDevicesButton.Enabled = !miningEngine.Mining;

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
            miningEngine.StopMining();
            deviceStatsTimer.Enabled = false;
            coinStatsCountdownTimer.Enabled = false;
            RefreshStrategiesCountdown();
            scryptRateLabel.Text = string.Empty;
            sha256RateLabel.Text = string.Empty;
            notifyIcon1.Text = "MultiMiner - Stopped";
            UpdateMiningButtons();
            ClearAllMinerStats();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (applicationConfiguration.AutoSetDesktopMode)
                EnableDesktopMode(true);

            SaveChanges();
            StartMining();
        }

        private void StartMining()
        {
            if (!MiningConfigurationValid())
                return;

            if (miningEngine.Mining)
                return;

            if (!ConfigFileHandled())
                return;

            startButton.Enabled = false; //immediately disable, update after
            startMenuItem.Enabled = false;

            try
            {
                miningEngine.StartMining(engineConfiguration, devices, coinInformation);
            }
            catch (MinerLaunchException ex)
            {
                MessageBox.Show(ex.Message, "Error Launching Miner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            engineConfiguration.SaveDeviceConfigurations(); //save any changes made by the engine

            deviceStatsTimer.Enabled = true;
            coinStatsCountdownTimer.Enabled = true;
            RefreshStrategiesCountdown();

            //to get changes from strategy config
            LoadGridValuesFromConfiguration();
            //to get updated coin stats for coin changes
            LoadGridValuesFromCoinStats();

            UpdateMiningButtons();
        }

        private bool ConfigFileHandled()
        {
            const string bakExtension = ".mmbak";

            MinerBackend minerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
            string minerName = MinerPath.GetMinerName(minerBackend);
            string minerExecutablePath = MinerPath.GetPathToInstalledMiner(minerBackend);
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

        private void RefreshBackendLabel()
        {
            if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                backendLabel.Text = "Backend: bfgminer";
            else if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Cgminer)
                backendLabel.Text = "Backend: cgminer";
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            ShowApplicationSettings();
        }

        private void ShowApplicationSettings()
        {
            SettingsForm settingsForm = new SettingsForm(applicationConfiguration, engineConfiguration.XgminerConfiguration);
            DialogResult dialogResult = settingsForm.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                bool wasMining = miningEngine.Mining;
                StopMining(); // or USB devices may be in use for RefreshDevices() call below
                engineConfiguration.SaveMinerConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                RefreshDevices();
                RefreshBackendLabel();
                crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;
                if (wasMining)
                    StartMining();
            }
            else
            {
                engineConfiguration.LoadMinerConfiguration();
                applicationConfiguration.LoadApplicationConfiguration();
            }
        }
        
        private void statsTimer_Tick(object sender, EventArgs e)
        {
            ClearMinerStatsForDisabledCoins();
            PopulateStatsFromMinerProcesses();
        }

        private void ClearMinerStatsForDisabledCoins()
        {
            if (saveButton.Enabled) //otherwise cleared coin isn't saved yet
                return;

            foreach (DataGridViewRow row in deviceGridView.Rows)
                if (row.Cells[coinColumn.Index].Value == null)
                    ClearMinerStatsForRow(row);
        }

        private void ClearMinerStatsForRow(DataGridViewRow row)
        {
            row.Cells[temperatureColumn.Index].Value = null;
            row.Cells[hashRateColumn.Index].Value = null;
            row.Cells[acceptedColumn.Index].Value = null;
            row.Cells[rejectedColumn.Index].Value = null;
            row.Cells[errorsColumn.Index].Value = null;
            row.Cells[utilityColumn.Index].Value = null;
            row.Cells[intensityColumn.Index].Value = null;
        }

        private void PopulateMinerStatsForRow(MultiMiner.Xgminer.Api.DeviceInformation deviceInformation, DataGridViewRow row)
        {
            row.Cells[temperatureColumn.Index].Value = deviceInformation.Temperature;
            row.Cells[hashRateColumn.Index].Value = deviceInformation.AverageHashrate;
            row.Cells[acceptedColumn.Index].Value = deviceInformation.AcceptedShares;
            row.Cells[rejectedColumn.Index].Value = deviceInformation.RejectedShares;
            row.Cells[errorsColumn.Index].Value = deviceInformation.HardwareErrors;
            row.Cells[utilityColumn.Index].Value = deviceInformation.Utility;
            row.Cells[intensityColumn.Index].Value = deviceInformation.Intensity;
        }

        private void ClearAllMinerStats()
        {
            foreach (DataGridViewRow row in deviceGridView.Rows)
                ClearMinerStatsForRow(row);
        }

        private void ClearAllCoinStats()
        {
            foreach (DataGridViewRow row in deviceGridView.Rows)
                ClearCoinStatsForGridRow(row);
        }

        private void ClearCoinStatsForDisabledCoins()
        {
            foreach (DataGridViewRow row in deviceGridView.Rows)
                if (string.IsNullOrEmpty((string)row.Cells[coinColumn.Index].Value))
                    ClearCoinStatsForGridRow(row);
        }

        private void ClearCoinStatsForGridRow(DataGridViewRow gridRow)
        {
            gridRow.Cells[difficultyColumn.Index].Value = null;
            gridRow.Cells[priceColumn.Index].Value = null;
            gridRow.Cells[profitabilityColumn.Index].Value = null;
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
                MultiMiner.Xgminer.Api.ApiContext loopContext = minerProcess.ApiContext;
                if (loopContext == apiContext)
                {
                    coinName = minerProcess.MinerConfiguration.CoinName;
                    break;
                }
            }

            return coinName;
        }

        private void PopulateStatsFromMinerProcesses()
        {
            double totalScryptRate = 0;
            double totalSha256Rate = 0;

            bool hasTempValue = false;
            bool hasIntensityValue = false;

            foreach (MinerProcess minerProcess in miningEngine.MinerProcesses)
            {
                minerProcess.HasDeadDevice = false;
                minerProcess.HasSickDevice = false;
                minerProcess.HasZeroHashrateDevice = false;
                minerProcess.MinerIsFrozen = false;

                List<MultiMiner.Xgminer.Api.DeviceInformation> deviceInformationList = GetDeviceInformationFromMinerProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                foreach (MultiMiner.Xgminer.Api.DeviceInformation deviceInformation in deviceInformationList)
                {
                    if (deviceInformation.Status.ToLower().Contains("sick"))
                        minerProcess.HasSickDevice = true;
                    if (deviceInformation.Status.ToLower().Contains("dead"))
                        minerProcess.HasDeadDevice = true;
                    if (deviceInformation.CurrentHashrate == 0)
                        minerProcess.HasZeroHashrateDevice = true;

                    int rowIndex = GetRowIndexForDeviceInformation(deviceInformation);

                    if (rowIndex >= 0)
                    {
                        if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.Scrypt)
                            totalScryptRate += deviceInformation.AverageHashrate;
                        else if (minerProcess.MinerConfiguration.Algorithm == CoinAlgorithm.SHA256)
                            totalSha256Rate += deviceInformation.AverageHashrate;

                        PopulateMinerStatsForRow(deviceInformation, deviceGridView.Rows[rowIndex]);

                        if (deviceInformation.Temperature > 0)
                            hasTempValue = true;

                        if (!string.IsNullOrEmpty(deviceInformation.Intensity))
                            hasIntensityValue = true;

                        lastAcceptedShares[rowIndex] = deviceInformation.AcceptedShares;
                    }
                }
            }

            scryptRateLabel.Text = string.Format("Scrypt: {0} Kh/s", totalScryptRate);
            sha256RateLabel.Text = string.Format("SHA256: {0} Mh/s", totalSha256Rate / 1000); //Mh not mh, mh is milli
            notifyIcon1.Text = string.Format("MultiMiner - {0} {1}", scryptRateLabel.Text, sha256RateLabel.Text);

            //hide the temperature column if there are no tempts returned (USBs, OS X, etc)
            temperatureColumn.Visible = hasTempValue;

            //hide the intensity column if there are no intensities returned (USBs)
            intensityColumn.Visible = hasIntensityValue;
        }

        private List<MultiMiner.Xgminer.Api.DeviceInformation> GetDeviceInformationFromMinerProcess(MinerProcess minerProcess)
        {
            MultiMiner.Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<MultiMiner.Xgminer.Api.DeviceInformation> deviceInformationList = null;
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

        private int GetRowIndexForDeviceInformation(MultiMiner.Xgminer.Api.DeviceInformation deviceInformation)
        {
            int index = 0;
            int rowIndex = -1;

            for (int i = 0; i < devices.Count; i++)
            {
                if ((deviceInformation.Kind.Equals("GPU") && (devices[i].Kind == DeviceKind.GPU))
                    || (!deviceInformation.Kind.Equals("GPU") && (devices[i].Kind != DeviceKind.GPU)))
                {
                    if (index == deviceInformation.Index)
                    {
                        rowIndex = i;
                        break;
                    }
                    index++;
                }
            }

            return rowIndex;
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

            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                //ensure the user isn't editing settings
                !ShowingModalDialog())
            {
                miningEngine.ApplyMiningStrategy(devices, coinInformation);
                //save any changes made by the engine
                engineConfiguration.SaveDeviceConfigurations();
                //to get changes from strategy config
                LoadGridValuesFromConfiguration();
                //to refresh coin stats due to changed coin selections
                LoadGridValuesFromCoinStats();
            }

            coinStatsCountdownMinutes = coinStatsTimer.Interval / 1000 / 60;
        }

        private void RefreshCoinStats()
        {
            //always load known coins from file
            //CoinChoose may not shown coins it once did if there are no orders
            LoadKnownCoinsFromFile();

            try
            {
                coinInformation = Coinchoose.Api.ApiContext.GetCoinInformation(UserAgent.AgentString, 
                    engineConfiguration.StrategyConfiguration.BaseCoin);
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException))
                {
                    notificationsControl.AddNotification(ex.Message, 
                        "Error parsing the CoinChoose.com JSON API", () =>
                    {
                        Process.Start(MultiMiner.Coinchoose.Api.ApiContext.GetApiUrl(engineConfiguration.StrategyConfiguration.BaseCoin));
                    }, "http://coinchoose.com/");
                    return;
                }
                throw;
            }
            
            LoadGridValuesFromCoinStats();
            LoadKnownCoinsFromCoinStats();
            RefreshCoinStatsLabel();
            SuggestCoinsToMine();
        }

        private void SuggestCoinsToMine()
        {
            if (!applicationConfiguration.SuggestCoinsToMine)
                return;
            if (coinInformation == null) //no network connection
                return;
            
            IEnumerable<Coinchoose.Api.CoinInformation> orderedCoins = coinInformation.OrderByDescending(c => c.AverageProfitability);
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
            IEnumerable<Coinchoose.Api.CoinInformation> unconfiguredCoins = orderedCoins.Where(coin => !String.IsNullOrEmpty(coin.Symbol) && !engineConfiguration.CoinConfigurations.Any(config => config.Coin.Symbol.Equals(coin.Symbol, StringComparison.OrdinalIgnoreCase)));
            IEnumerable<Coinchoose.Api.CoinInformation> coinsToMine = unconfiguredCoins.Take(3);

            foreach (Coinchoose.Api.CoinInformation coin in coinsToMine)
                NotifyCoinToMine(coin);
        }

        private void NotifyCoinToMine(MultiMiner.Coinchoose.Api.CoinInformation coin)
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

            string infoUrl = "http://coinchoose.com/index.php";
            if (engineConfiguration.StrategyConfiguration.BaseCoin == Coinchoose.Api.BaseCoin.Litecoin)
                infoUrl = "http://coinchoose.com/litecoin.php";

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
            coinStatsLabel.Text = string.Format("Coin stats last fetched at {0}.", DateTime.Now.ToShortTimeString());
        }

        private void LoadKnownCoinsFromCoinStats()
        {
            foreach (Coinchoose.Api.CoinInformation item in coinInformation)
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
                if (item.Algorithm.Contains("scrypt"))
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

        private void RemoveBunkCoins(List<CryptoCoin> knownCoins)
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

        private void LoadGridValuesFromCoinStats()
        {
            //clear all coin stats first
            //there may be coins configured that are no longer returned in the stats
            ClearAllCoinStats();

            if (coinInformation != null) //null if no network connection
                foreach (Coinchoose.Api.CoinInformation coin in coinInformation)
                    foreach (DataGridViewRow row in deviceGridView.Rows)
                    {
                        string rowCoinName = (string)row.Cells[coinColumn.Index].Value;
                        CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Coin.Name.Equals(rowCoinName, StringComparison.OrdinalIgnoreCase));
                        if ((coinConfiguration != null) &&  coin.Symbol.Equals(coinConfiguration.Coin.Symbol, StringComparison.OrdinalIgnoreCase))
                            PopulateCoinStatsForRow(coin, row);
                    }
        }

        private void PopulateCoinStatsForRow(Coinchoose.Api.CoinInformation coin, DataGridViewRow row)
        {
            row.Cells[difficultyColumn.Index].Value = coin.Difficulty.ToString(".##########");
            row.Cells[priceColumn.Index].Value = coin.Price.ToString(".##########");

            switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
            {
                case StrategyConfiguration.CoinProfitabilityKind.AdjustedProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.AdjustedProfitability, 2);
                    break;
                case StrategyConfiguration.CoinProfitabilityKind.AverageProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.AverageProfitability, 2);
                    break;
                case StrategyConfiguration.CoinProfitabilityKind.StraightProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.Profitability, 2);
                    break;
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

        private void strategiesButton_Click(object sender, EventArgs e)
        {
            ConfigureStrategies();
        }

        private void RefreshStrategiesLabel()
        {
            if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                strategiesLabel.Text = "Strategies: enabled";
            else
                strategiesLabel.Text = "Strategies: disabled";
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
                engineConfiguration.SaveStrategyConfiguration();
                applicationConfiguration.SaveApplicationConfiguration();
                SetupCoinStatsTimer();
                
                coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
                coinColumn.DisplayStyle = coinColumn.ReadOnly ? DataGridViewComboBoxDisplayStyle.Nothing : DataGridViewComboBoxDisplayStyle.DropDownButton;

                //so updated profitability is shown
                RefreshCoinStats();

                RefreshStrategiesLabel();
                LoadGridValuesFromCoinStats();
                UpdateMiningButtons();

                RestartMiningIfMining();
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

            switch (applicationConfiguration.StrategyCheckInterval)
            {
                case ApplicationConfiguration.CoinStrategyCheckInterval.FiveMinutes:
                    coinStatsMinutes = 5;
                    break;
                case ApplicationConfiguration.CoinStrategyCheckInterval.ThirtyMinutes:
                    coinStatsMinutes = 30;
                    break;
                default:
                    coinStatsMinutes = 15;
                    break;
            }

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
            Process.Start("http://coinchoose.com/");
        }

        private void coinsButton_Click(object sender, EventArgs e)
        {
            ConfigureCoins();
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

        private void desktopModeButton_Click(object sender, EventArgs e)
        {
            engineConfiguration.XgminerConfiguration.DesktopMode = desktopModeButton.Checked;
            RestartMiningIfMining();
            engineConfiguration.SaveMinerConfiguration();
        }

        private void RestartMiningIfMining()
        {
            if (miningEngine.Mining)
            {
                StopMining();
                StartMining();
            }
        }

        private void mobileMinerTimer_Tick(object sender, EventArgs e)
        {
            //check for commands first so we can report mining activity after
            CheckForMobileMinerCommands();
            SubmitMobileMinerStats();
        }

        private const string mobileMinerApiKey = "P3mVX95iP7xfoI";
        private const string mobileMinerUrl = "https://api.mobileminerapp.com";
        
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
                List<MultiMiner.Xgminer.Api.DeviceInformation> deviceInformationList = GetDeviceInformationFromMinerProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                {
                    continue;
                }

                foreach (MultiMiner.Xgminer.Api.DeviceInformation deviceInformation in deviceInformationList)
                {
                    MultiMiner.MobileMiner.Api.MiningStatistics miningStatistics = new MobileMiner.Api.MiningStatistics();
                    
                    miningStatistics.MinerName = "MultiMiner";
                    miningStatistics.CoinName = GetCoinNameForApiContext(minerProcess.ApiContext);
                    CryptoCoin coin = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(miningStatistics.CoinName)).Coin;
                    miningStatistics.CoinSymbol = coin.Symbol;

                    if (coin.Algorithm == CoinAlgorithm.Scrypt)
                        miningStatistics.Algorithm = "scrypt";
                    else if (coin.Algorithm == CoinAlgorithm.SHA256)
                        miningStatistics.Algorithm = "SHA-256";
                    
                    PopulateMiningStatsFromDeviceInfo(miningStatistics, deviceInformation);

                    statisticsList.Add(miningStatistics);
                }
            }

            if (statisticsList.Count > 0)
            {
                try
                {
                    MobileMiner.Api.ApiContext.SubmitMiningStatistics(mobileMinerUrl, mobileMinerApiKey,
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
                                this.applicationConfiguration.MobileMinerMonitoring = false;
                                this.applicationConfiguration.SaveApplicationConfiguration();
                                MessageBox.Show("Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog." +
                                    Environment.NewLine + Environment.NewLine +
                                    "MobileMiner remote monitoring will now be disabled.", "Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                //check to make sure there are no modal windows already
                                if (!ShowingModalDialog())
                                    ShowApplicationSettings();
                            }
                        }
                    }
                }
            }
        }

        private bool ShowingModalDialog()
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

            List<MobileMiner.Api.RemoteCommand> commands = new List<MobileMiner.Api.RemoteCommand>();

            try
            {
                commands = MobileMiner.Api.ApiContext.GetCommands(mobileMinerUrl, mobileMinerApiKey,
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
                                        ShowApplicationSettings();
                                }
                            }
                        }
                    }

                    return;
                }
                throw;  
            }

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

                MobileMiner.Api.ApiContext.DeleteCommand(mobileMinerUrl, mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    Environment.MachineName, command.Id);
            }
        }

        private static void PopulateMiningStatsFromDeviceInfo(MultiMiner.MobileMiner.Api.MiningStatistics miningStatistics, MultiMiner.Xgminer.Api.DeviceInformation deviceInformation)
        {
            miningStatistics.AcceptedShares = deviceInformation.AcceptedShares;
            miningStatistics.AverageHashrate = deviceInformation.AverageHashrate;
            miningStatistics.CurrentHashrate = deviceInformation.CurrentHashrate;
            miningStatistics.Enabled = deviceInformation.Enabled;
            miningStatistics.FanPercent = deviceInformation.FanPercent;
            miningStatistics.FanSpeed = deviceInformation.FanSpeed;
            miningStatistics.GpuActivity = deviceInformation.GpuActivity;
            miningStatistics.GpuClock = deviceInformation.GpuClock;
            miningStatistics.GpuVoltage = deviceInformation.GpuVoltage;
            miningStatistics.HardwareErrors = deviceInformation.HardwareErrors;
            miningStatistics.Index = deviceInformation.Index;
            miningStatistics.Intensity = deviceInformation.Intensity;
            miningStatistics.Kind = deviceInformation.Kind;
            miningStatistics.MemoryClock = deviceInformation.MemoryClock;
            miningStatistics.PowerTune = deviceInformation.PowerTune;
            miningStatistics.RejectedShares = deviceInformation.RejectedShares;
            miningStatistics.Status = deviceInformation.Status;
            miningStatistics.Temperature = deviceInformation.Temperature;
            miningStatistics.Utility = deviceInformation.Utility;
        }

        private void processLogButton_Click(object sender, EventArgs e)
        {
            ShowAdvancedPanel();
            advancedTabControl.SelectedTab = processLogPage;

            applicationConfiguration.LogAreaVisible = true;
            applicationConfiguration.SaveApplicationConfiguration();
        }

        private void historyButton_Click(object sender, EventArgs e)
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

                //convert from device indexes (0 based) to device #'s (more human readable)
                List<string> deviceList = new List<string>();

                LogProcessCloseArgs ea = this.logCloseEntries[index];

                foreach (int deviceIndex in ea.DeviceIndexes)
                    deviceList.Add(String.Format("#{0}", deviceIndex + 1));
                string devices = String.Join(", ", deviceList.ToArray());

                historyGridView.Rows[index].Cells[devicesColumn.Index].Value = devices;

                TimeSpan timeSpan = ea.EndDate - ea.StartDate;
                historyGridView.Rows[index].Cells[durationColumn.Index].Value = String.Format("{0:0.##} minutes", timeSpan.TotalMinutes);
            }
        }

        private void quickSwitchItem_DropDownOpening(object sender, EventArgs e)
        {
            PopulateQuickSwitchMenu();
        }

        private void PopulateQuickSwitchMenu()
        {
            quickSwitchItem.DropDownItems.Clear();
            foreach (CoinConfiguration coinConfiguration in engineConfiguration.CoinConfigurations.Where(c => c.Enabled))
            {
                ToolStripMenuItem coinSwitchItem = new ToolStripMenuItem();

                coinSwitchItem.Text = coinConfiguration.Coin.Name;
                coinSwitchItem.Tag = coinConfiguration.Coin.Symbol;
                coinSwitchItem.Click += HandleQuickSwitchClick;

                quickSwitchItem.DropDownItems.Add(coinSwitchItem);
            }
        }

        private void HandleQuickSwitchClick(object sender, EventArgs e)
        {
            bool wasMining = miningEngine.Mining;
            StopMining();

            deviceGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            string coinSymbol = (string)((ToolStripMenuItem)sender).Tag;
            CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(coinSymbol));

            SetAllDevicesToCoin(coin);

            engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = false; 
            coinColumn.ReadOnly = false;
            coinColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;

            engineConfiguration.SaveDeviceConfigurations();
            engineConfiguration.SaveStrategyConfiguration();

            LoadGridValuesFromConfiguration();

            if (wasMining)
                StartMining();
        }

        private void SetAllDevicesToCoin(CryptoCoin coin)
        {
            engineConfiguration.DeviceConfigurations.Clear();

            for (int i = 0; i < devices.Count; i++)
            {
                DataGridViewRow gridRow = deviceGridView.Rows[i];
                Device device = devices[i];
                CryptoCoin currentCoin = knownCoins.SingleOrDefault(c => c.Name.Equals(gridRow.Cells[coinColumn.Index].Value));

                DeviceConfiguration deviceConfiguration = new DeviceConfiguration();

                deviceConfiguration.DeviceIndex = i;

                if (coin.Algorithm == CoinAlgorithm.Scrypt)
                {
                    if (device.Kind == DeviceKind.GPU)
                        deviceConfiguration.CoinSymbol = coin.Symbol;
                    else
                        deviceConfiguration.CoinSymbol = currentCoin == null ? String.Empty : currentCoin.Symbol;
                }
                else
                {
                    deviceConfiguration.CoinSymbol = coin.Symbol;
                }

                object cellValue = gridRow.Cells[enabledColumn.Index].Value;
                deviceConfiguration.Enabled = cellValue == null ? true : (bool)cellValue;

                engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }
        }

        private void advancedMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //use > 0, not > 1, so if a lot of devices have blank configs you can easily set them all
            quickSwitchItem.Enabled = engineConfiguration.CoinConfigurations.Where(c => c.Enabled).Count() > 0;
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
            if (concretePlatform != PlatformID.Unix)
            {
                MinerBackend minerBackend = MinerBackend.Cgminer;
                CheckForMinerUpdates(minerBackend);

                minerBackend = MinerBackend.Bfgminer;
                CheckForMinerUpdates(minerBackend);
            }
        }

        private const int bfgminerNotificationId = 100;
        private const int cgminerNotificationId = 101;
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

        private bool ThisVersionGreater(string thisVersion, string thatVersion)
        {
            Version thisVersionObj = new Version(thisVersion);
            Version thatVersionObj = new Version(thatVersion);

            return thisVersionObj > thatVersionObj;
        }

        private void CheckForMinerUpdates(MinerBackend minerBackend)
        {
            string minerName = MinerPath.GetMinerName(minerBackend);

            if (MinerIsInstalled(minerBackend))
            {
                string availableMinerVersion = String.Empty;
                try
                {
                    availableMinerVersion = Xgminer.Installer.GetAvailableMinerVersion(minerBackend);
                }
                catch (WebException ex)
                {
                    //downloads website is down
                    return;
                }

                string installedMinerVersion = Xgminer.Installer.GetInstalledMinerVersion(minerBackend, MinerPath.GetPathToInstalledMiner(minerBackend));
                if (ThisVersionGreater(availableMinerVersion, installedMinerVersion))
                {
                    int notificationId = minerBackend == MinerBackend.Bfgminer ? bfgminerNotificationId : cgminerNotificationId;

                    string informationUrl = "https://github.com/ckolivas/cgminer/blob/master/NEWS";
                    if (minerBackend == MinerBackend.Bfgminer)
                        informationUrl = "https://github.com/luke-jr/bfgminer/blob/bfgminer/NEWS";

                    notificationsControl.AddNotification(notificationId.ToString(),
                        String.Format("{0} version {1} is available ({2} installed)", 
                            minerName, availableMinerVersion, installedMinerVersion), () =>
                        {
                            bool wasMining = miningEngine.Mining;
                            StopMining();
                            InstallMiner(minerBackend);
                            if (wasMining)
                                StartMining();
                        }, informationUrl);
                }
            }
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
                    EnableDesktopMode(false);
                }
                //else if idle for less than the idleTimer interval, enable Desktop Mode
                else if (idleTimeSpan.TotalMilliseconds <= idleTimer.Interval)
                {
                    if (!engineConfiguration.XgminerConfiguration.DesktopMode)
                    EnableDesktopMode(true);
                }
            }
        }

        private void EnableDesktopMode(bool enabled)
        {
            engineConfiguration.XgminerConfiguration.DesktopMode = enabled;
            desktopModeButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;
            RestartMiningIfMining();
            engineConfiguration.SaveMinerConfiguration();
        }

        private void advancedAreaContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (settingsLoaded)
                applicationConfiguration.LogAreaDistance = e.SplitY;
        }
    }
}
