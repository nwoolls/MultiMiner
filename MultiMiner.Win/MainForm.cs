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
using System.Web.Script.Serialization;

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

        public MainForm()
        {
            InitializeComponent();
        }

        private void LogProcessLaunch(object sender, LogLaunchArgs ea)
        {
            logLaunchArgsBindingSource.Add(ea);
            logLaunchArgsBindingSource.Position = logLaunchArgsBindingSource.IndexOf(ea);

            while (logLaunchArgsBindingSource.Count > 1000)
                logLaunchArgsBindingSource.RemoveAt(0);
        }

        private void LogProcessClose(object sender, LogProcessCloseArgs ea)
        {
            const string logFileName = "MiningLog.json";
            string logFilePath = Path.Combine(AppDataPath(), logFileName);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonData = serializer.Serialize(ea);

            File.AppendAllText(logFilePath, jsonData + Environment.NewLine);

            logProcessCloseArgsBindingSource.Add(ea);
            int index = logProcessCloseArgsBindingSource.IndexOf(ea);
            logProcessCloseArgsBindingSource.Position = index;

            //convert from device indexes (0 based) to device #'s (more human readable)
            List<string> deviceList = new List<string>();
            foreach (int deviceIndex in ea.DeviceIndexes)
                deviceList.Add(String.Format("#{0}", deviceIndex + 1));            
            string devices = String.Join(", ", deviceList.ToArray());

            historyGridView.Rows[index].Cells[devicesColumn.Index].Value = devices;

            TimeSpan timeSpan = ea.EndDate - ea.StartDate;
            historyGridView.Rows[index].Cells[durationColumn.Index].Value = String.Format("{0:0.##} minutes", timeSpan.TotalMinutes);

            while (logProcessCloseArgsBindingSource.Count > 1000)
                logProcessCloseArgsBindingSource.RemoveAt(0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupGridColumns();

            logLaunchArgsBindingSource.DataSource = logCloseEntries;

            const int mobileMinerInterval = 32; //seconds
            mobileMinerTimer.Interval = mobileMinerInterval * 1000;

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
            logLaunchArgsBindingSource.DataSource = logLaunchEntries;

            saveButton.Enabled = false;
            cancelButton.Enabled = false;

            if (!HasMinersInstalled())
                CancelMiningOnStartup();

            //check for disowned miners before refreshing devices
            if (applicationConfiguration.DetectDisownedMiners)
                CheckForDisownedMiners();

            CheckAndDownloadMiners();

            RefreshDevices();

            if (devices.Count > 0)
                deviceGridView.CurrentCell = deviceGridView.Rows[0].Cells[coinColumn.Index];

            formLoaded = true;
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

            ProgressForm progressForm = new ProgressForm("Downloading and installing " + minerName + " from " + Installer.GetMinerDownloadRoot(minerBackend));
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
            //customized FillWeight doesn't behave properly under Mono
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
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
                }
                else
                {
                    MessageBox.Show("The miner specified in your settings was not found. Please go to https://github.com/nwoolls/multiminer for instructions on installing either cgminer or bfgminer.",
                        "Miner Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }

            if ((devices.Count > 0) && (engineConfiguration.DeviceConfigurations.Count == 0) &&
                (engineConfiguration.CoinConfigurations.Count == 1))
            {
                //setup devices for a brand new user
                ConfigureDevicesForNewUser();
            }

            deviceBindingSource.DataSource = devices;
            LoadGridValuesFromConfiguration();
            CheckAndHideNameColumn();
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

            coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.MineProfitableCoins;
            RefreshStrategiesLabel();
            RefreshStrategiesCountdown();

            desktopModeButton.Checked = engineConfiguration.XgminerConfiguration.DesktopMode;

            applicationConfiguration.LoadApplicationConfiguration();

            if (applicationConfiguration.Maximized)
                this.WindowState = FormWindowState.Maximized;

            if (applicationConfiguration.LogAreaVisible)
                ShowApiMonitor();
            else
                HideAdvancedPanel();

            if (applicationConfiguration.StartMiningOnStartup)
            {
                startupMiningTimer.Interval = 1000 * applicationConfiguration.StartupMiningDelay;
                startupMiningCountdownSeconds = applicationConfiguration.StartupMiningDelay;
                startupMiningTimer.Enabled = true;
                startupMiningCountdownTimer.Enabled = true;
                RefreshCountdownLabel();
            }

            startupMiningPanel.Visible = applicationConfiguration.StartMiningOnStartup;

            crashRecoveryTimer.Enabled = applicationConfiguration.RestartCrashedMiners;

            SetupCoinStatsTimer();
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
                CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Name.Equals(gridRow.Cells[coinColumn.Index].Value));

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
                        CryptoCoin coin = knownCoins.SingleOrDefault(c => c.Symbol.Equals(deviceConfiguration.CoinSymbol));
                        if (coin != null)
                        {
                            //ensure the coin configuration still exists
                            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.SingleOrDefault(c => c.Coin.Symbol.Equals(coin.Symbol));
                            if (coinConfiguration != null)
                                gridRow.Cells[coinColumn.Index].Value = coin.Name;
                            else
                                gridRow.Cells[coinColumn.Index].Value = string.Empty;
                        }
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
                miningConfigurationValid = engineConfiguration.StrategyConfiguration.MineProfitableCoins &&
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
            SaveChanges();
            StartMining();
        }

        private void StartMining()
        {
            if (!MiningConfigurationValid())
                return;

            if (miningEngine.Mining)
                return;

            if (!confFileHandled())
                return;

            startButton.Enabled = false; //immediately disable, update after
            startMenuItem.Enabled = false;

            miningEngine.StartMining(engineConfiguration, devices, coinInformation);

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

        private bool confFileHandled()
        {
            const string bakExtension = ".mmbak";

            MinerBackend minerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
            string minerName = MinerPath.GetMinerName(minerBackend);
            string minerExecutablePath = MinerPath.GetPathToInstalledMiner(minerBackend);
            string confFileFilePath = Path.ChangeExtension(minerExecutablePath, ".conf");

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

            apiLogEntryBindingSource.Add(logEntry);
            apiLogEntryBindingSource.Position = apiLogEntryBindingSource.IndexOf(logEntry);

            while (apiLogEntryBindingSource.Count > 1000)
                apiLogEntryBindingSource.RemoveAt(0);
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
                minerProcess.HasFrozenDevice = false;

                List<MultiMiner.Xgminer.Api.DeviceInformation> deviceInformationList = GetDeviceInformationFromMinerProcess(minerProcess);

                if (deviceInformationList == null) //handled failure getting API info
                {
                    minerProcess.HasFrozenDevice = true;
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
            StopMining();
        }

        private void coinStatsTimer_Tick(object sender, EventArgs e)
        {
            RefreshCoinStats();

            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                miningEngine.ApplyMiningStrategy(devices, coinInformation);
                //save any changes made by the engine
                engineConfiguration.SaveDeviceConfigurations();
                //to get changes from strategy config
                LoadGridValuesFromConfiguration();
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
                coinInformation = Coinchoose.Api.ApiContext.GetCoinInformation(UserAgent.AgentString);
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if (ex is WebException)
                {
                    return;
                }
                throw;
            }

            LoadGridValuesFromCoinStats();
            LoadKnownCoinsFromCoinStats();
            RefreshCoinStatsLabel();
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
                knownCoins = ConfigurationReaderWriter.ReadConfiguration<List<CryptoCoin>>(knownCoinsFileName);
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

            foreach (Coinchoose.Api.CoinInformation coin in coinInformation)
                foreach (DataGridViewRow row in deviceGridView.Rows)
                    if (coin.Name.Equals((string)row.Cells[coinColumn.Index].Value, StringComparison.CurrentCultureIgnoreCase))
                        PopulateCoinStatsForRow(coin, row);
        }

        private void PopulateCoinStatsForRow(Coinchoose.Api.CoinInformation coin, DataGridViewRow row)
        {
            row.Cells[difficultyColumn.Index].Value = coin.Difficulty.ToString(".##########");
            row.Cells[priceColumn.Index].Value = coin.Price.ToString(".##########");

            switch (engineConfiguration.StrategyConfiguration.ProfitabilityBasis)
            {
                case StrategyConfiguration.CoinProfitabilityBasis.AdjustedProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.AdjustedProfitability, 2);
                    break;
                case StrategyConfiguration.CoinProfitabilityBasis.AverageProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.AverageProfitability, 2);
                    break;
                case StrategyConfiguration.CoinProfitabilityBasis.StraightProfitability:
                    row.Cells[profitabilityColumn.Index].Value = Math.Round(coin.Profitability, 2);
                    break;
            }
        }

        private void startupMiningTimer_Tick(object sender, EventArgs e)
        {
            startupMiningPanel.Visible = false;
            startupMiningTimer.Enabled = false;
            startupMiningCountdownTimer.Enabled = false;

            Application.DoEvents();

            StartMining();
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            startupMiningCountdownSeconds--;
            RefreshCountdownLabel();
        }

        private void cancelStartupMiningButton_Click(object sender, EventArgs e)
        {
            CancelMiningOnStartup();
        }

        private void CancelMiningOnStartup()
        {
            startupMiningTimer.Enabled = false;
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
            if (engineConfiguration.StrategyConfiguration.MineProfitableCoins)
                strategiesLabel.Text = "Strategies: enabled";
            else
                strategiesLabel.Text = "Strategies: disabled";
        }

        private void RefreshStrategiesCountdown()
        {
            //Time until strategy check: 60s
            if (miningEngine.Mining && engineConfiguration.StrategyConfiguration.MineProfitableCoins)
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
                coinColumn.ReadOnly = engineConfiguration.StrategyConfiguration.MineProfitableCoins;
                RefreshStrategiesLabel();
                LoadGridValuesFromCoinStats();
                UpdateMiningButtons();

                RestartMiningIfMining();
            }
            else
            {
                engineConfiguration.LoadStrategyConfiguration();
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
            splitContainer1.Panel2.Hide();
            splitContainer1.Panel2Collapsed = true;
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
            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel2.Show();
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
                else if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Visible = false;
                }

                //handling for saving Maximized state
                if (this.WindowState == FormWindowState.Maximized)
                    applicationConfiguration.Maximized = true;

                if (this.WindowState == FormWindowState.Normal) //don't set to false for minimizing
                    applicationConfiguration.Maximized = false;

                applicationConfiguration.SaveApplicationConfiguration();
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
                    CryptoCoin coin = knownCoins.Single(c => c.Name.Equals(miningStatistics.CoinName));
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
                }
                catch (WebException ex)
                {
                    //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down    
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            this.applicationConfiguration.MobileMinerMonitoring = false;
                            this.applicationConfiguration.SaveApplicationConfiguration();
                            MessageBox.Show("Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog." +
                                Environment.NewLine + Environment.NewLine +
                                "MobileMiner remote monitoring will now be disabled.", "Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ShowApplicationSettings();
                        }
                    }
                }
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

            List<MobileMiner.Api.RemoteCommand> commands = new List<MobileMiner.Api.RemoteCommand>();

            try
            {
                commands = MobileMiner.Api.ApiContext.GetCommands(mobileMinerUrl, mobileMinerApiKey,
                    applicationConfiguration.MobileMinerEmailAddress, applicationConfiguration.MobileMinerApplicationKey,
                    Environment.MachineName);
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
                                this.applicationConfiguration.MobileMinerRemoteCommands = false;
                                this.applicationConfiguration.SaveApplicationConfiguration();
                                MessageBox.Show("Your MobileMiner credentials are incorrect. Please check your MobileMiner settings in the Settings dialog." +
                                    Environment.NewLine + Environment.NewLine +
                                    "MobileMiner remote commands will now be disabled.", "Invalid Credentails", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ShowApplicationSettings();
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
    }
}
