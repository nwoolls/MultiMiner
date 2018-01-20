using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Discovery;
using MultiMiner.Discovery.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Engine.Installers;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.MobileMiner.Data;
using MultiMiner.MultipoolApi.Data;
using MultiMiner.Remoting;
using MultiMiner.Remoting.Broadcast;
using MultiMiner.Services;
using MultiMiner.Stats.Data;
using MultiMiner.Utility.Timers;
using MultiMiner.Utility.Net;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Discovery;
using Newtonsoft.Json;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Timers;
using Application = MultiMiner.UX.Data.Configuration.Application;
using Broadcaster = MultiMiner.Remoting.Broadcast.Broadcaster;
using ConfigurationEventArgs = MultiMiner.UX.Data.ConfigurationEventArgs;
using Device = MultiMiner.Xgminer.Data.Device;
using Listener = MultiMiner.Discovery.Listener;
using Path = MultiMiner.Remoting.Data.Transfer.Configuration.Path;
using Perks = MultiMiner.UX.Data.Configuration.Perks;
using System.Globalization;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        #region Constants
        private const int BfgMinerNotificationId = 100;
        private const int MultiMinerNotificationId = 102;
        public const string PrimaryAlgorithm = AlgorithmNames.SHA256;
        private readonly double DifficultyMuliplier = Math.Pow(2, 32);
        #endregion

        #region Events
        //delegate declarations
        public delegate void NotificationEventHandler(object sender, NotificationEventArgs ea);
        public delegate void CredentialsEventHandler(object sender, CredentialsEventArgs ea);
        public delegate void ProgressStartEventHandler(object sender, ProgressEventArgs ea);
        public delegate void ConfigurationEventHandler(object sender, ConfigurationEventArgs ea);
        public delegate void RemoteInstanceEventHandler(object sender, InstanceChangedArgs ea);
        public delegate void InstanceModifiedEventHander(object sender, RemotingEventArgs ea);
        public delegate void PromptEventHandler(object sender, PromptEventArgs e);

        //event declarations        
        public event NotificationEventHandler NotificationReceived;
        public event NotificationEventHandler NotificationDismissed;
        public event CredentialsEventHandler CredentialsRequested;
        public event EventHandler MobileMinerAuthFailed;
        public event EventHandler DataModified;
        public event ProgressStartEventHandler ProgressStarted;
        public event EventHandler ProgressCompleted;
        public event EventHandler ConfigurationModified;
        public event ConfigurationEventHandler OnConfigureSettings;
        public event ConfigurationEventHandler OnConfigurePerks;
        public event ConfigurationEventHandler OnConfigureStrategies;
        public event ConfigurationEventHandler OnConfigurePools;
        public event RemoteInstanceEventHandler RemoteInstanceRegistered;
        public event RemoteInstanceEventHandler RemoteInstanceUnregistered;
        public event InstanceModifiedEventHander RemoteInstanceModified;
        public event EventHandler RemoteInstancesUnregistered;
        public event PromptEventHandler PromptReceived;
        #endregion

        #region Fields
        //coalesced timers
        private readonly Timers timers = new Timers();
        private readonly System.Timers.Timer coinStatsTimer = new System.Timers.Timer();
        private readonly System.Timers.Timer restartTimer = new System.Timers.Timer();
        private readonly System.Timers.Timer networkRestartTimer = new System.Timers.Timer();
        private readonly System.Timers.Timer networkScanTimer = new System.Timers.Timer();

        //configuration
        public readonly Engine.Data.Configuration.Engine EngineConfiguration = new Engine.Data.Configuration.Engine();
        public readonly Application ApplicationConfiguration = new Application();
        public readonly Paths PathConfiguration = new Paths();
        public readonly Perks PerksConfiguration = new Perks();
        public readonly NetworkDevices NetworkDevicesConfiguration = new NetworkDevices();
        private readonly Metadata metadataConfiguration = new Metadata();

        //Coin API contexts
        private IApiContext coinWarzApiContext;
        private IApiContext whatToMineApiContext;

        //Coin API information
        private readonly List<CoinInformation> coinApiInformation = new List<CoinInformation>();

        //Exchange API information

        //view models
        public readonly MinerFormViewModel LocalViewModel = new MinerFormViewModel();

        //hardware information

        //RPC API information
        private readonly Dictionary<MinerProcess, List<DeviceDetails>> processDeviceDetails = new Dictionary<MinerProcess, List<DeviceDetails>>();
        private readonly Dictionary<string, double> minerNetworkDifficulty = new Dictionary<string, double>();
        private readonly Dictionary<DeviceViewModel, string> lastDevicePoolMapping = new Dictionary<DeviceViewModel, string>();
        public readonly Dictionary<string, List<PoolInformation>> NetworkDevicePools = new Dictionary<string, List<PoolInformation>>();
        private readonly Dictionary<string, List<MinerStatistics>> networkDeviceStatistics = new Dictionary<string, List<MinerStatistics>>();
        private readonly Dictionary<string, VersionInformation> networkDeviceVersions = new Dictionary<string, VersionInformation>();
        public readonly Dictionary<DeviceViewModel, DeviceDetails> DeviceDetailsMapping = new Dictionary<DeviceViewModel, DeviceDetails>();

        //mining logic
        private readonly MiningEngine miningEngine = new MiningEngine();
        private List<PoolGroup> knownCoins = new List<PoolGroup>();

        //MobileMiner API information
        private readonly List<int> processedCommandIds = new List<int>();
        private readonly List<Notification> queuedNotifications = new List<Notification>();

        //data sources
        public readonly BindingList<ApiLogEntry> ApiLogEntries = new BindingList<ApiLogEntry>();
        public readonly BindingList<LogLaunchArgs> LogLaunchEntries = new BindingList<LogLaunchArgs>();
        public readonly BindingList<LogProcessCloseArgs> LogCloseEntries = new BindingList<LogProcessCloseArgs>();

        //remoting
        private RemotingServer remotingServer;
        private Listener discoveryListener;
        private Remoting.Broadcast.Listener broadcastListener;
        private int fingerprint;
        private readonly Random random = new Random();
        private Remoting.Data.Transfer.Configuration.Perks remotePerksConfig;
        private Path remotePathConfig;
        private Remoting.Data.Transfer.Configuration.Engine remoteEngineConfig;
        private Remoting.Data.Transfer.Configuration.Application remoteApplicationConfig;
        public readonly InstanceManager InstanceManager = new InstanceManager();
        #endregion

        #region Properties
        private IApiContext successfulApiContext { get; set; }
        public List<CoinInformation> CoinApiInformation { get { return coinApiInformation; } }
        public List<PoolGroup> KnownCoins { get { return knownCoins; } }
        public IEnumerable<ExchangeInformation> SellPrices { get; private set; }
        public MiningEngine MiningEngine { get { return miningEngine; } }
        public List<Device> Devices { get; private set; }
        public int StartupMiningCountdownSeconds { get; private set; }
        public int CoinStatsCountdownMinutes { get; private set; }

        //remoting
        public Instance SelectedRemoteInstance { get; private set; }
        public bool RemotingEnabled { get; private set; }
        public bool RemoteInstanceMining { get; private set; }

        //view models
        public MinerFormViewModel RemoteViewModel { get; set; }
        
        //threading
        public ISynchronizeInvoke Context { get; set; }

        //currently mining information
        private List<Engine.Data.Configuration.Device> miningDeviceConfigurations { get; set; }
        private List<Coin> miningCoinConfigurations { get; set; }
        #endregion

        #region Constructor
        public ApplicationViewModel()
        {
            coinStatsTimer.Elapsed += coinStatsTimer_Tick;
            restartTimer.Elapsed += restartTimer_Tick;
            networkRestartTimer.Elapsed += networkRestartTimer_Tick;
            networkScanTimer.Elapsed += networkScanTimer_Tick;
            SetupNetworkRestartTimer();
            SetupNetworkScanTimer();

            RemoteViewModel = new MinerFormViewModel();
        }
        #endregion

        #region Coin API
        public void SetupCoinApi()
        {
            coinWarzApiContext = new CoinWarz.ApiContext(ApplicationConfiguration.CoinWarzApiKey, ApplicationConfiguration.CoinWarzUrlParms);
            whatToMineApiContext = new WhatToMine.ApiContext(ApplicationConfiguration.WhatToMineUrlParms);
        }

        private void RefreshAllCoinStats()
        {
            //always load known coins from file
            //CoinChoose may not show coins it once did if there are no orders
            LoadKnownCoinsFromFile();

            RefreshSingleCoinStats();
            RefreshMultiCoinStats();
        }

        public IApiContext GetEffectiveApiContext()
        {
            if (successfulApiContext != null)
                return successfulApiContext;
            if (ApplicationConfiguration.UseCoinWarzApi)
                return coinWarzApiContext;

            return whatToMineApiContext;
        }

        private void RefreshSingleCoinStats()
        {
            IApiContext preferredApiContext, backupApiContext;
            if (ApplicationConfiguration.UseCoinWarzApi)
            {
                preferredApiContext = coinWarzApiContext;
                backupApiContext = whatToMineApiContext;
            }
            else
            {
                preferredApiContext = whatToMineApiContext;
                backupApiContext = coinWarzApiContext;
            }

            bool success = RefreshSingleCoinStats(preferredApiContext);
            if (!success &&
                //don't try to use CoinWarz as a backup unless the user has entered an API key for CoinWarz
                ((backupApiContext == whatToMineApiContext) || !String.IsNullOrEmpty(ApplicationConfiguration.CoinWarzApiKey)))
                RefreshSingleCoinStats(backupApiContext);

            ApplyCoinInformationToViewModel();

            FixCoinSymbolDiscrepencies();
        }

        private void RefreshMultiCoinStats()
        {
            RefreshNiceHashStats();
        }

        private void RefreshNiceHashStats()
        {
            const string prefix = "NiceHash";

            //the NiceHash API is slow
            //only fetch from them if:
            //1. We have no NiceHash coins in KnownCoins
            //2. Or we have a Multipool setup for NiceHash
            bool initialLoad = !KnownCoins.Any(kc => kc.Id.Contains(prefix));
            bool miningNiceHash = EngineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Contains(prefix) && cc.Enabled);
            if (!initialLoad && !miningNiceHash)
            {
                return;
            }

            IEnumerable<MultipoolInformation> multipoolInformation = GetNiceHashInformation();

            //we're offline or the API is offline
            if (multipoolInformation == null)
                return;

            CoinApiInformation.RemoveAll(c => c.IsMultiCoin);
            CoinApiInformation.AddRange(multipoolInformation
                .Select(mpi => new CoinInformation
                {
                    Symbol = String.Format("{0}:{1}", prefix, mpi.Algorithm),
                    Name = String.Format("{0} - {1}", prefix, mpi.Algorithm),
                    Profitability = mpi.Profitability,
                    AverageProfitability = mpi.Profitability,
                    AdjustedProfitability = mpi.Profitability,
                    Price = mpi.Price,
                    Algorithm = KnownAlgorithms.Algorithms.Single(ka => ka.Name.Equals(mpi.Algorithm)).FullName,
                    IsMultiCoin = true
                }));
        }

        private IEnumerable<MultipoolInformation> GetNiceHashInformation()
        {
            IEnumerable<MultipoolInformation> multipoolInformation;
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
                    if (ApplicationConfiguration.ShowApiErrors)
                    {
                        ShowMultipoolApiErrorNotification(apiContext, ex);
                    }
                    return null;
                }
                throw;
            }
            return multipoolInformation;
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
                .SingleOrDefault(c => !String.IsNullOrEmpty(c.Symbol) && c.Symbol.Equals(Engine.Data.KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
            {
                CoinInformation goodCoin = coinApiInformation
                    .ToList() //get a copy - populated async & collection may be modified
                    .SingleOrDefault(c => !String.IsNullOrEmpty(c.Symbol) && c.Symbol.Equals(Engine.Data.KnownCoins.DogecoinSymbol, StringComparison.OrdinalIgnoreCase));
                if (goodCoin == null)
                    badCoin.Symbol = Engine.Data.KnownCoins.DogecoinSymbol;
                else
                    coinApiInformation.Remove(badCoin);
            }
        }

        private void FixKnownCoinSymbolDiscrepencies()
        {
            PoolGroup badCoin = knownCoins.SingleOrDefault(c => c.Id.Equals(Engine.Data.KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
            {
                PoolGroup goodCoin = knownCoins.SingleOrDefault(c => c.Id.Equals(Engine.Data.KnownCoins.DogecoinSymbol, StringComparison.OrdinalIgnoreCase));
                if (goodCoin == null)
                    badCoin.Id = Engine.Data.KnownCoins.DogecoinSymbol;
                else
                    knownCoins.Remove(badCoin);
            }
        }

        private bool RefreshSingleCoinStats(IApiContext apiContext)
        {
            try
            {
                //remove dupes by Symbol in case the Coin API returns them - seen from user
                IEnumerable<CoinInformation> newCoinInformation = apiContext.GetCoinInformation(UserAgent.AgentString).GroupBy(c => c.Symbol).Select(g => g.First()).ToList();

                //do not set / overwrite CoinApiInformation - update the contents
                //otherwise we remove valid MultiCoin information that may fail to be
                //refreshed in RefreshMultiCoinStats()
                CoinApiInformation.RemoveAll(c => !c.IsMultiCoin);
                CoinApiInformation.AddRange(newCoinInformation);

                successfulApiContext = apiContext;
            }
            catch (Exception ex)
            {
                //don't crash if website cannot be resolved or JSON cannot be parsed
                if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException) || (ex is CoinApiException) ||
                    (ex is JsonReaderException))
                {
                    if (ApplicationConfiguration.ShowApiErrors)
                    {
                        ShowCoinApiErrorNotification(apiContext, ex);
                    }
                    return false;
                }
                throw;
            }

            return true;
        }

        private void ShowCoinApiErrorNotification(IApiContext apiContext, Exception ex)
        {
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message, summary, () =>
            {
                MessageBoxShow(String.Format("{0}: {1}", summary, details), "Error", PromptButtons.OK, PromptIcon.Error);
            }, NotificationKind.Warning, apiUrl);
        }

        private PromptResult MessageBoxShow(string text, string caption, PromptButtons buttons, PromptIcon icon)
        {
            PromptEventArgs e = new PromptEventArgs
            {
                Caption = caption,
                Text = text,
                Buttons = buttons,
                Icon = icon
            };
            PromptReceived(this, e);
            return e.Result;
        }

        private void ShowMultipoolApiErrorNotification(MultipoolApi.IApiContext apiContext, Exception ex)
        {
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message, summary, () =>
            {
                MessageBoxShow(String.Format("{0}: {1}", summary, details), "Error", PromptButtons.OK, PromptIcon.Error);
            }, NotificationKind.Warning, apiUrl);
        }
        #endregion

        #region Settings dialogs
        public void ConfigureSettings()
        {
            if (SelectedRemoteInstance == null)
                ConfigureSettingsLocally();
            else
                ConfigureSettingsRemotely();
        }

        public void ConfigureSettingsLocally()
        {
            bool oldNetworkDeviceDetection = ApplicationConfiguration.NetworkDeviceDetection;
            bool oldCoinWarzValue = ApplicationConfiguration.UseCoinWarzApi;
            bool oldWhatMineValue = ApplicationConfiguration.UseWhatMineApi;
            bool oldWhatToMineValue = ApplicationConfiguration.UseWhatToMineApi;
            string oldCoinWarzKey = ApplicationConfiguration.CoinWarzApiKey;
            string oldWhatMineKey = ApplicationConfiguration.WhatMineApiKey;

            string oldConfigPath = PathConfiguration.SharedConfigPath;
            
            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = ApplicationConfiguration,
                Engine = EngineConfiguration,
                Paths = PathConfiguration,
                Perks = PerksConfiguration
            };
            if (OnConfigureSettings != null) OnConfigureSettings(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                PathConfiguration.SavePathConfiguration();

                //save settings as the "shared" config path may have changed
                //these are settings not considered machine/device-specific
                //e.g. no device settings, no miner settings
                string newConfigPath = PathConfiguration.SharedConfigPath;
                MigrateSettingsToNewFolder(oldConfigPath, newConfigPath);

                ApplicationConfiguration.SaveApplicationConfiguration(newConfigPath);
                PerksConfiguration.SavePerksConfiguration(newConfigPath);
                EngineConfiguration.SaveCoinConfigurations(newConfigPath);
                EngineConfiguration.SaveStrategyConfiguration(newConfigPath);
                EngineConfiguration.SaveMinerConfiguration();
                MiningEngine.SaveAlgorithmConfigurations();
                SaveKnownCoinsToFile();

                //don't refresh coin stats excessively
                if ((oldCoinWarzValue != ApplicationConfiguration.UseCoinWarzApi) ||
                    !oldCoinWarzKey.Equals(ApplicationConfiguration.CoinWarzApiKey) ||
                    (oldWhatToMineValue != ApplicationConfiguration.UseWhatToMineApi) ||
                    (oldWhatMineValue != ApplicationConfiguration.UseWhatMineApi) ||
                    !oldWhatMineKey.Equals(ApplicationConfiguration.WhatMineApiKey))
                {
                    SetupCoinApi(); //pickup API key changes
                    RefreshCoinStatsAsync();
                }

                //if we are not detecting Network Devices, start the async checks
                if (ApplicationConfiguration.NetworkDeviceDetection &&
                    (!oldNetworkDeviceDetection))
                    RunNetworkDeviceScan();

                //SubmitMobileMinerPools();

                UpdateDevicesForProxySettings();
                ApplyModelsToViewModel();
                if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());
            }
            else
            {
                EngineConfiguration.LoadMinerConfiguration();
                PathConfiguration.LoadPathConfiguration();
                ApplicationConfiguration.LoadApplicationConfiguration(PathConfiguration.SharedConfigPath);
            }
        }

        public void ConfigurePools()
        {
            if (SelectedRemoteInstance == null)
                ConfigurePoolsLocally();
            else
                ConfigurePoolsRemotely();
        }

        private void ConfigurePoolsLocally()
        {
            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = ApplicationConfiguration,
                Engine = EngineConfiguration,
                Perks = PerksConfiguration
            };
            if (OnConfigurePools != null) OnConfigurePools(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                FixMisconfiguredDevices();
                ConfigureUnconfiguredDevices();

                EngineConfiguration.SaveCoinConfigurations();
                EngineConfiguration.SaveDeviceConfigurations();
                ApplicationConfiguration.SaveApplicationConfiguration();

                //may be able to auto-assign more devices now that coins are setup
                AddMissingDeviceConfigurations();

                ApplyModelsToViewModel();
                if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());

                //SubmitMobileMinerPools();

                if (ApplicationConfiguration.SaveCoinsToAllMachines && PerksConfiguration.PerksEnabled && PerksConfiguration.EnableRemoting)
                    SetCoinConfigurationOnAllRigs(EngineConfiguration.CoinConfigurations.ToArray());
            }
            else
            {
                EngineConfiguration.LoadCoinConfigurations(PathConfiguration.SharedConfigPath);
                ApplicationConfiguration.LoadApplicationConfiguration(PathConfiguration.SharedConfigPath);
            }
        }

        private void ConfigureUnconfiguredDevices()
        {
            foreach (Engine.Data.Configuration.Device deviceConfiguration in EngineConfiguration.DeviceConfigurations)
            {
                bool configured = !String.IsNullOrEmpty(deviceConfiguration.CoinSymbol);
                bool misConfigured = configured &&
                    !EngineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));

                if (!configured || misConfigured)
                {
                    Coin coinConfiguration = null;
                    if (deviceConfiguration.Kind == DeviceKind.GPU)
                        coinConfiguration = EngineConfiguration.CoinConfigurations.FirstOrDefault(cc => cc.PoolGroup.Algorithm.Equals(AlgorithmNames.Scrypt, StringComparison.OrdinalIgnoreCase));
                    if (coinConfiguration == null)
                        coinConfiguration = EngineConfiguration.CoinConfigurations.FirstOrDefault(cc => cc.PoolGroup.Algorithm.Equals(AlgorithmNames.SHA256, StringComparison.OrdinalIgnoreCase));

                    if (coinConfiguration != null)
                        deviceConfiguration.CoinSymbol = coinConfiguration.PoolGroup.Id;
                }
            }
        }

        private void FixMisconfiguredDevices()
        {
            foreach (Engine.Data.Configuration.Device deviceConfiguration in EngineConfiguration.DeviceConfigurations)
            {
                bool misconfigured = !String.IsNullOrEmpty(deviceConfiguration.CoinSymbol) &&
                    !EngineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));

                if (misconfigured)
                    deviceConfiguration.CoinSymbol = String.Empty;
            }
        }

        public void ConfigurePerks()
        {
            if (SelectedRemoteInstance == null)
                ConfigurePerksLocally();
            else
                ConfigurePerksRemotely();
        }

        public void ConfigurePerksLocally()
        {
            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Perks = PerksConfiguration
            };
            if (OnConfigurePerks != null) OnConfigurePerks(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                bool miningWithMultipleProxies = MiningEngine.Mining && (Devices.Count(d => d.Kind == DeviceKind.PXY) > 1);
                if (!PerksConfiguration.PerksEnabled && miningWithMultipleProxies)
                    throw new Exception(MiningEngine.AdvancedProxiesRequirePerksMessage);

                PerksConfiguration.SavePerksConfiguration();

                SetupRemoting();
                if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());
            }
            else
                PerksConfiguration.LoadPerksConfiguration(PathConfiguration.SharedConfigPath);
        }


        private void ConfigureStrategiesLocally()
        {
            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = ApplicationConfiguration,
                Engine = EngineConfiguration
            };
            if (OnConfigureStrategies != null) OnConfigureStrategies(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                EngineConfiguration.SaveStrategyConfiguration();
                ApplicationConfiguration.SaveApplicationConfiguration();

                if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());
            }
            else
            {
                EngineConfiguration.LoadStrategyConfiguration(PathConfiguration.SharedConfigPath);
                ApplicationConfiguration.LoadApplicationConfiguration(PathConfiguration.SharedConfigPath);
            }
        }

        public void ConfigureStrategies()
        {
            if (SelectedRemoteInstance == null)
                ConfigureStrategiesLocally();
            else
                ConfigureStrategiesRemotely();
        }
        #endregion

        #region Settings logic
        public void LoadSettings()
        {
            EngineConfiguration.LoadAllConfigurations(PathConfiguration.SharedConfigPath);
            PerksConfiguration.LoadPerksConfiguration(PathConfiguration.SharedConfigPath);
            metadataConfiguration.LoadDeviceMetadataConfiguration();
            NetworkDevicesConfiguration.LoadNetworkDevicesConfiguration();
            LoadKnownDevicesFromFile();

            SetupCoinApi();
            RefreshExchangeRates();
            SetupCoinStatsTimer();
            ClearPoolsFlaggedDown();
            ApplyModelsToViewModel();

            LocalViewModel.DynamicIntensity = EngineConfiguration.XgminerConfiguration.DesktopMode;
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

        private void SaveKnownCoinsToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(knownCoins, KnownCoinsFileName());
        }

        private string KnownCoinsFileName()
        {
            string filePath = String.IsNullOrEmpty(PathConfiguration.SharedConfigPath) ? ApplicationPaths.AppDataPath() : PathConfiguration.SharedConfigPath;
            return System.IO.Path.Combine(filePath, "KnownCoinsCache.xml");
        }

        private static void RemoveBunkCoins(List<PoolGroup> knownCoins)
        {
            //CoinChoose.com served up ButterFlyCoin as BOC, and then later as BFC
            PoolGroup badCoin = knownCoins.SingleOrDefault(c => c.Id.Equals("BOC", StringComparison.OrdinalIgnoreCase));
            if (badCoin != null)
                knownCoins.Remove(badCoin);
        }

        private void ConfigureDevicesForNewUser()
        {
            Coin coinConfiguration = EngineConfiguration.CoinConfigurations.Single();

            foreach (Device device in Devices)
            {
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device
                {
                    CoinSymbol = coinConfiguration.PoolGroup.Id,
                    Enabled = true
                };

                deviceConfiguration.Assign(device);
                EngineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            EngineConfiguration.SaveDeviceConfigurations();
        }
        
        //each device needs to have a DeviceConfiguration
        //this will add any missing ones after populating devices
        //for instance if the user starts up the app with a new device
        public void AddMissingDeviceConfigurations()
        {
            bool hasBtcConfigured = EngineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.PoolGroup.Id.Equals(Engine.Data.KnownCoins.BitcoinSymbol, StringComparison.OrdinalIgnoreCase));
            bool hasLtcConfigured = EngineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.PoolGroup.Id.Equals(Engine.Data.KnownCoins.LitecoinSymbol, StringComparison.OrdinalIgnoreCase));

            foreach (Device device in Devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));
                if (existingConfiguration == null)
                {
                    Engine.Data.Configuration.Device newConfiguration = new Engine.Data.Configuration.Device();

                    newConfiguration.Assign(device);

                    if (device.SupportsAlgorithm(AlgorithmNames.Scrypt) && hasLtcConfigured &&
                        (device.Kind != DeviceKind.PXY)) //don't default Proxies to Litecoin
                        newConfiguration.CoinSymbol = Engine.Data.KnownCoins.LitecoinSymbol;
                    else if (device.SupportsAlgorithm(AlgorithmNames.SHA256) && hasBtcConfigured)
                        newConfiguration.CoinSymbol = Engine.Data.KnownCoins.BitcoinSymbol;

                    //don't enable newly added Proxies if we're mining
                    newConfiguration.Enabled = (device.Kind != DeviceKind.PXY) || (!miningEngine.Mining);

                    EngineConfiguration.DeviceConfigurations.Add(newConfiguration);
                }
            }
        }

        //try to match up devices without configurations with configurations without devices
        //could happen if, for instance, a COM port changes for a device
        private void FixOrphanedDeviceConfigurations()
        {
            foreach (Device device in Devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));

                //if there is no configuration specifically for the device
                if (existingConfiguration == null)
                {
                    //find a configuration that uses the same driver and that, itself, has no specifically matching device
                    Engine.Data.Configuration.Device orphanedConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                        c => c.Driver.Equals(device.Driver, StringComparison.OrdinalIgnoreCase) &&
                                !Devices.Exists(d => d.Equals(c)));

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
            EngineConfiguration.DeviceConfigurations.RemoveAll(c => !Devices.Exists(d => d.Equals(c)));
        }

        private static string KnownDevicesFileName()
        {
            string filePath = ApplicationPaths.AppDataPath();
            return System.IO.Path.Combine(filePath, "KnownDevicesCache.xml");
        }

        private void LoadKnownDevicesFromFile()
        {
            string knownDevicesFileName = KnownDevicesFileName();
            if (File.Exists(knownDevicesFileName))
            {
                Devices = ConfigurationReaderWriter.ReadConfiguration<List<Device>>(knownDevicesFileName);
                UpdateDevicesForProxySettings();
                ApplyModelsToViewModel();
            }
        }

        private void SaveKnownDevicesToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(Devices, KnownDevicesFileName());
        }

        private void SaveChangesLocally()
        {
            SaveViewModelValuesToConfiguration();
            EngineConfiguration.SaveDeviceConfigurations();

            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);

            SetHasChangesLocally(false);
        }

        private void SetHasChanges(bool hasChanges)
        {
            if (SelectedRemoteInstance == null)
                SetHasChangesLocally(hasChanges);
            else
                SetHasChangesRemotely(hasChanges);
        }

        public void SetHasChangesLocally(bool hasChanges)
        {
            LocalViewModel.HasChanges = hasChanges;
            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void SetHasChangesRemotely(bool hasChanges)
        {
            RemoteViewModel.HasChanges = hasChanges;

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void SaveViewModelValuesToConfiguration()
        {
            EngineConfiguration.DeviceConfigurations.Clear();

            foreach (Device device in Devices)
            {
                //don't assume 1-to-1 of Devices and ViewModel.Devices
                //Devices doesn't include Network Devices
                DeviceViewModel viewModel = LocalViewModel.Devices.Single(vm => vm.Equals(device));

                //pull this from coin configurations, not known coins, may not be in CoinChoose
                PoolGroup coin = viewModel.Coin;
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device();
                deviceConfiguration.Assign(viewModel);
                deviceConfiguration.Enabled = viewModel.Enabled;
                deviceConfiguration.CoinSymbol = coin == null ? string.Empty : coin.Id;
                EngineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }
        }

        public bool MiningConfigurationValid()
        {
            bool miningConfigurationValid = EngineConfiguration.DeviceConfigurations.Count(DeviceConfigurationValid) > 0;
            if (!miningConfigurationValid)
            {
                miningConfigurationValid = EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                    (EngineConfiguration.CoinConfigurations.Count(c => c.Enabled) > 0) &&
                    (EngineConfiguration.DeviceConfigurations.Count(c => c.Enabled) > 0);
            }
            return miningConfigurationValid;
        }

        private bool DeviceConfigurationValid(Engine.Data.Configuration.Device deviceConfiguration)
        {
            bool result = deviceConfiguration.Enabled && !string.IsNullOrEmpty(deviceConfiguration.CoinSymbol);
            if (result)
            {
                Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                result = coinConfiguration != null && coinConfiguration.Pools.Any(p => !String.IsNullOrEmpty(p.Host) && !String.IsNullOrEmpty(p.Username));
            }
            return result;
        }
        #endregion

        #region Model / ViewModel behavior

        private void ApplyModelsToViewModel()
        {
            ApplyDevicesToViewModel();
            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);
            ApplyCoinInformationToViewModel();
            LocalViewModel.ApplyCoinConfigurationModels(EngineConfiguration.CoinConfigurations);
        }

        private void ApplyDevicesToViewModel()
        {
            //ApplyDeviceModels() ensures we have a 1-to-1 with listview items
            LocalViewModel.ApplyDeviceModels(Devices, NetworkDevicesConfiguration.Devices, metadataConfiguration.Devices);
        }

        private void ApplyCoinInformationToViewModel()
        {
            if (coinApiInformation != null)
                LocalViewModel.ApplyCoinInformationModels(
                    coinApiInformation.ToList() //get a copy - populated async & collection may be modified)
                );

            LocalViewModel.Devices.ForEach((d) =>
            {
                //apply real network difficulty (reported by miners) where possible                
                //we may have known difficulties that aren't int he Coin API info
                CalculateDifficulty(d);

                //calculate daily income
                CalculateDailyIncome(d);
            });
        }

        private void CalculateDifficulty(DeviceViewModel device)
        {
            var difficulty = GetCachedNetworkDifficulty(device.Pool ?? String.Empty);
            if (difficulty != 0.0)
                device.Difficulty = difficulty;
        }

        private void CalculateDailyIncome(DeviceViewModel device)
        {
            if (device.Coin == null) return;

            CoinInformation info = CoinApiInformation
                .ToList() //get a copy - populated async & collection may be modified
                .SingleOrDefault(c => c.Symbol.Equals(device.Coin.Id, StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                if (device.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin)
                {
                    CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(device.Coin.Algorithm);

                    double hashrate = device.CurrentHashrate * 1000;
                    double fullDifficulty = info.Difficulty * algorithm.DifficultyMultiplier;
                    double secondsToCalcShare = fullDifficulty / hashrate;
                    const double secondsPerDay = 86400;
                    double sharesPerDay = secondsPerDay / secondsToCalcShare;
                    double coinsPerDay = sharesPerDay * info.Reward;

                    device.Daily = coinsPerDay;
                }
                else
                {
                    //info.Price is in BTC/Ghs/Day
                    device.Daily = info.Price * device.CurrentHashrate / 1000 / 1000;
                }
            };
        }

        public DeviceViewModel GetDeviceById(string deviceId)
        {
            var index = -1;
            var valid = int.TryParse(deviceId.Substring(1), out index);

            if (!valid) return null;

            index--;

            var devices = GetVisibleDevices();
            var kindId = deviceId.First().ToString();
            var kindDevices = devices
                .Where(d => d.Kind.ToString().StartsWith(kindId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if ((index >= 0) && (index < kindDevices.Count))
                return kindDevices[index];

            return null;
        }
        #endregion

        #region Exchange API
        public void RefreshExchangeRates()
        {
            if (PerksConfiguration.PerksEnabled && PerksConfiguration.ShowExchangeRates)
            {
                try
                {
                    SellPrices = new Blockchain.ApiContext().GetExchangeInformation();
                }
                catch (Exception ex)
                {
                    //don't crash if website cannot be resolved or JSON cannot be parsed
                    if ((ex is WebException) || (ex is InvalidCastException) || (ex is FormatException))
                    {
                        if (ApplicationConfiguration.ShowApiErrors)
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

            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message,
                String.Format(summary, apiName), () =>
                {
                    MessageBoxShow(String.Format("{0}: {1}", summary, details), "Error", PromptButtons.OK, PromptIcon.Error);
                },
                NotificationKind.Warning, apiUrl);
        }
        #endregion

        #region Network devices
        public void SetupNetworkDeviceDetection()
        {
            //network devices
            if (ApplicationConfiguration.NetworkDeviceDetection)
                RunNetworkDeviceScan();
        }

        private void CheckNetworkDevicesAsync()
        {
            Action asyncAction = CheckNetworkDevices;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (Context == null) return;

                    Context.BeginInvoke((Action)(HandleNetworkDeviceDiscovery), null);

                }, null);
        }

        private void HandleNetworkDeviceDiscovery()
        {
            ApplyModelsToViewModel();

            //after the above call, no devices in the ViewModel have stats
            //refresh them
            if (LocalViewModel.Devices.Count(d => d.Kind != DeviceKind.NET) > 0)
                RefreshAllDeviceStats();

            if (LocalViewModel.Devices.Count(d => d.Kind == DeviceKind.NET) > 0)
                RefreshNetworkDeviceStatsAsync();
        }

        private void RefreshNetworkDeviceStatsAsync()
        {
            Action asyncAction = RefreshAllNetworkDeviceStats;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (Context == null) return;

                    Context.BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        ApplyCoinInformationToViewModel();

                        RemoveSelfReferencingNetworkDevices();

                        if (DataModified != null) DataModified(this, new EventArgs());
                    }), null);

                }, null);
        }

        private void RemoveSelfReferencingNetworkDevices()
        {
            List<string> localIpAddresses = LocalNetwork.GetLocalIPAddresses();
            IEnumerable<DeviceViewModel> networkDevices = LocalViewModel.Devices.Where(d => d.Kind == DeviceKind.NET).ToList();
            foreach (DeviceViewModel networkDevice in networkDevices)
            {
                if (localIpAddresses.Contains(networkDevice.Pool.DomainFromHost()))
                    //actually remove rather than setting Visible = false
                    //Visible = false items still get fetched with RefreshNetworkDeviceStats()
                    LocalViewModel.Devices.Remove(networkDevice);
            }
        }

        private void RefreshNetworkDeviceStatsAsync(DeviceViewModel deviceViewModel)
        {
            string[] portions = deviceViewModel.Path.Split(':');
            string ipAddress = portions[0];
            int port = int.Parse(portions[1]);

            List<DeviceInformation> deviceInformationList = GetDeviceInfoFromAddress(ipAddress, port);

            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
            if (Context == null) return;

            //first clear stats for each row
            //this is because the NET row stats get summed 
            Context.BeginInvoke((Action)(() =>
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
                        LocalViewModel.ApplyDeviceInformationResponseModel(deviceViewModel, deviceInformation);
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
            }), null);
        }

        private static double AdjustWorkUtilityForPoolMultipliers(double workUtility, string algorithmName)
        {
            CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
            return workUtility / algorithm.PoolMultiplier;
        }

        private VersionInformation GetCachedMinerVersionFromAddress(string ipAddress, int port)
        {
            VersionInformation versionInfo;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (networkDeviceVersions.ContainsKey(key))
                versionInfo = networkDeviceVersions[key];
            else
            {
                versionInfo = GetVersionInfoFromAddress(ipAddress, port);
                //don't cache null so we'll retry
                if (versionInfo != null) networkDeviceVersions[key] = versionInfo;
            }
            return versionInfo;
        }

        private void RefreshAllNetworkDeviceStats()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            IEnumerable<DeviceViewModel> networkDevices = LocalViewModel.Devices.Where(d => d.Kind == DeviceKind.NET).ToList();

            foreach (DeviceViewModel deviceViewModel in networkDevices)
            {
                //is the app closing?
                //otherwise a check for formHandleValid is needed before the call to this.BeginInvoke below
                //this makes more sense though and has other benefits
                if (Context == null)
                    return;

                RefreshNetworkDeviceStatsAsync(deviceViewModel);
            }
        }

        public void RefreshAllDeviceStats()
        {
            //first clear stats for each row
            //this is because the PXY row stats get summed 
            LocalViewModel.ClearDeviceInformationFromViewModel();

            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<MinerProcess> minerProcesses = MiningEngine.MinerProcesses.ToList();
            foreach (MinerProcess minerProcess in minerProcesses)
                RefreshProcessDeviceStats(minerProcess);

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void FindNetworkDevicesAsync()
        {
            Action asyncAction = FindNetworkDevices;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);
                    

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (Context == null) return;

                    Context.BeginInvoke((Action)(() =>
                    {
                        //code to update UI
                        HandleNetworkDeviceDiscovery();
                        //re-enable network scan timer, we're done scanning
                        networkScanTimer.Enabled = true;
                    }), null);

                }, null);
        }

        private void FindNetworkDevices()
        {
            SubnetClass subnetClasses = SubnetClass.C;
            if (ApplicationConfiguration.NetworkDeviceScanClassB)
                subnetClasses = subnetClasses | SubnetClass.B;
            if (ApplicationConfiguration.NetworkDeviceScanClassA)
                subnetClasses = subnetClasses | SubnetClass.A;

            List<LocalNetwork.NetworkInterfaceInfo> localIpRanges = LocalNetwork.GetLocalNetworkInterfaces(subnetClasses);
            if (localIpRanges.Count == 0)
                return; //no network connection

            const int startingPort = 4028;
            const int endingPort = 4030;

            foreach (LocalNetwork.NetworkInterfaceInfo interfaceInfo in localIpRanges)
            {
                List<IPEndPoint> miners = MinerFinder.Find(interfaceInfo.RangeStart, interfaceInfo.RangeEnd, startingPort, endingPort);

                //do not remove local IP addresses
                //we want the ability to discover other miners on the same PC
                //our processes only whitelist 127.0.0.1, not the network facing IP

                List<NetworkDevices.NetworkDevice> newDevices = miners.ToNetworkDevices();

                //merge in miners, don't remove miners here
                //let CheckNetworkDevices() remove miners since it does not depend on port scanning
                //some users are manually entering devices in the XML
                List<NetworkDevices.NetworkDevice> existingDevices = NetworkDevicesConfiguration.Devices;
                newDevices = newDevices
                    .Where(d1 => !existingDevices.Any(d2 => d2.IPAddress.Equals(d1.IPAddress) && (d2.Port == d1.Port)))
                    .ToList();
                NetworkDevicesConfiguration.Devices.AddRange(newDevices);
            }

            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        private void CheckNetworkDevices()
        {
            List<IPEndPoint> endpoints = NetworkDevicesConfiguration.Devices.ToIPEndPoints();

            //remove own miners
            endpoints.RemoveAll(m => LocalNetwork.GetLocalIPAddresses().Contains(m.Address.ToString()));

            endpoints = MinerFinder.Check(endpoints);

            List<NetworkDevices.NetworkDevice> existingDevices = NetworkDevicesConfiguration.Devices;
            List<NetworkDevices.NetworkDevice> prunedDevices = endpoints.ToNetworkDevices();
            //add in Sticky devices not already in the pruned devices
            //Sticky devices allow users to mark Network Devices that should never be removed
            prunedDevices.AddRange(
                existingDevices
                    .Where(d1 => d1.Sticky && !prunedDevices.Any(d2 => d2.IPAddress.Equals(d1.IPAddress) && (d2.Port == d1.Port)))
            );

            //filter the devices by prunedDevices - do not assign directly as we need to
            //preserve properties on the existing configurations - e.g. Sticky
            NetworkDevicesConfiguration.Devices =
                NetworkDevicesConfiguration.Devices
                .Where(ed => prunedDevices.Any(pd => pd.IPAddress.Equals(ed.IPAddress) && (pd.Port == ed.Port)))
                .ToList();

            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        private void RunNetworkDeviceScan()
        {
            //disable network scan timer, scan starting
            //scan may take longer than networkScanTimer.Interval
            networkScanTimer.Enabled = false;

            CheckNetworkDevicesAsync();
            FindNetworkDevicesAsync();
        }

        public void SetNetworkDevicePool(DeviceViewModel networkDevice, string poolUrl)
        {
            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = NetworkDevicePools[networkDevice.Path];

            if (poolInformation == null)
                //RPC API call timed out
                return;

            int poolIndex = poolInformation.FindIndex(pi => pi.Url.Equals(poolUrl));

            if (poolIndex == -1)
            {
                //device doesn't have pool
                return;
            }

            SetNetworkDevicePoolIndex(networkDevice, poolIndex);
        }

        public bool SetNetworkDevicePoolIndex(DeviceViewModel networkDevice, int poolIndex)
        {
            if (poolIndex < 0)
                return false; //invalid index

            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = NetworkDevicePools[networkDevice.Path];

            if ((poolInformation != null) && (poolIndex >= poolInformation.Count))
                return false; //invalid index

            Uri uri = new Uri("http://" + networkDevice.Path);
            ApiContext apiContext = new ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            apiContext.SwitchPool(poolIndex);

            return true;
        }

        private List<PoolInformation> GetCachedPoolInfoFromAddress(string ipAddress, int port)
        {
            List<PoolInformation> poolInformationList;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (NetworkDevicePools.ContainsKey(key))
                poolInformationList = NetworkDevicePools[key];
            else
            {
                poolInformationList = GetPoolInfoFromAddress(ipAddress, port);
                //don't cache null so we'll retry
                if (poolInformationList != null) NetworkDevicePools[key] = poolInformationList;
            }
            return poolInformationList;
        }

        private List<PoolInformation> GetCachedPoolInfoFromAddress(string address)
        {
            string[] parts = address.Split(':');
            return GetCachedPoolInfoFromAddress(parts[0], int.Parse(parts[1]));
        }

        private List<PoolInformation> GetPoolInfoFromAddress(string ipAddress, int port)
        {
            ApiContext apiContext = new ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<PoolInformation> poolInformation;
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

        public void ToggleNetworkDeviceHidden(DeviceViewModel deviceViewModel)
        {
            bool hidden;
            ToggleNetworkDeviceHidden(deviceViewModel, out hidden);
        }

        public void ToggleNetworkDeviceHidden(DeviceViewModel deviceViewModel, out bool hidden)
        {
            NetworkDevices.NetworkDevice deviceConfiguration = NetworkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Hidden = !deviceConfiguration.Hidden;
            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();

            ApplyDevicesToViewModel();

            hidden = deviceConfiguration.Hidden;
        }

        public void ToggleNetworkDeviceSticky(DeviceViewModel deviceViewModel)
        {
            bool sticky;
            ToggleNetworkDeviceSticky(deviceViewModel, out sticky);
        }

        public void ToggleNetworkDeviceSticky(DeviceViewModel deviceViewModel, out bool sticky)
        {
            NetworkDevices.NetworkDevice deviceConfiguration = NetworkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Sticky = !deviceConfiguration.Sticky;
            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();

            sticky = deviceConfiguration.Sticky;
        }

        public bool RestartNetworkDevice(DeviceViewModel networkDevice)
        {
            Uri uri = new Uri("http://" + networkDevice.Path);
            ApiContext apiContext = new ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            string response = apiContext.RestartMining();
            bool result = !response.ToLower().Contains("STATUS=E".ToLower());

            if (result) networkDevice.LastRestart = DateTime.Now;

            return result;
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
            List<MinerStatistics> minerStatisticsList;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (networkDeviceStatistics.ContainsKey(key))
                minerStatisticsList = networkDeviceStatistics[key];
            else
            {
                minerStatisticsList = GetMinerStatisticsFromAddress(ipAddress, port);
                //don't cache null so we'll retry
                if (minerStatisticsList != null) networkDeviceStatistics[key] = minerStatisticsList;
            }
            return minerStatisticsList;
        }

        private List<MinerStatistics> GetMinerStatisticsFromAddress(string ipAddress, int port)
        {
            ApiContext apiContext = new ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<MinerStatistics> minerStatistics;
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

        public bool StartNetworkDevice(DeviceViewModel networkDevice)
        {
            return ToggleNetworkDevicePools(networkDevice, true);
        }

        public bool StopNetworkDevice(DeviceViewModel networkDevice)
        {
            return ToggleNetworkDevicePools(networkDevice, false);
        }

        private bool ToggleNetworkDevicePools(DeviceViewModel networkDevice, bool enabled)
        {
            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = GetCachedPoolInfoFromAddress(networkDevice.Path);

            if (poolInformation == null)
                //RPC API call timed out
                return false;

            Uri uri = new Uri("http://" + networkDevice.Path);
            ApiContext apiContext = new ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            string verb = enabled ? "enablepool" : "disablepool";

            for (int i = 0; i < poolInformation.Count; i++)
            {
                string response = apiContext.GetResponse(String.Format("{0}|{1}", verb, i));
                if (!response.ToLower().Contains("STATUS=S".ToLower()))
                    return false;
            }

            //remove cached data for pools
            NetworkDevicePools.Remove(networkDevice.Path);

            return true;
        }

        public bool ExecuteNetworkDeviceCommand(DeviceViewModel deviceViewModel, string commandText, bool unattended = false)
        {
            NetworkDevices.NetworkDevice networkDevice = GetNetworkDeviceByPath(deviceViewModel.Path);

            string username = networkDevice.Username;
            string password = networkDevice.Password;
            string devicePath = deviceViewModel.Path;
            string deviceName = devicePath;
            if (!String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                deviceName = deviceViewModel.FriendlyName;

            bool success = false;
            bool stop = false;
            bool prompt = String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password);

            if (unattended && prompt)
                return false;

            while (!success && !stop)
            {
                if (prompt)
                {
                    CredentialsEventArgs ea = new CredentialsEventArgs
                    {
                        ProtectedResource = deviceName,
                        Username = username,
                        Password = password
                    };

                    if (CredentialsRequested != null) CredentialsRequested(this, ea);

                    if (ea.CredentialsProvided)
                    {
                        username = ea.Username;
                        password = ea.Password;
                    }
                    else
                    {
                        stop = true;
                    }
                }

                if (!stop)
                {
                    Uri uri = new Uri("http://" + devicePath);
                    using (SshClient client = new SshClient(uri.Host, username, password))
                    {
                        try
                        {
                            client.Connect();
                        }
                        catch (Exception ex)
                        {
                            if (ex is SshAuthenticationException)
                                prompt = true;
                            else if ((ex is SocketException) || (ex is SshOperationTimeoutException))
                            {
                                stop = true;
                                PostNotification(String.Format("{0}: {1}", deviceName, ex.Message), NotificationKind.Danger);
                            }
                            else throw;
                        }

                        if (client.IsConnected)
                        {
                            try
                            {
                                stop = true;
                                success = ExecuteSshCommand(deviceName, client, commandText);
                            }
                            finally
                            {
                                client.Disconnect();
                            }
                        }
                    }
                }
            }

            if (success)
            {
                networkDevice.Username = username;
                networkDevice.Password = password;
                NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
            }

            return success;
        }

        public NetworkDevices.NetworkDevice GetNetworkDeviceByPath(string path)
        {
            Uri uri = new Uri("http://" + path);
            return NetworkDevicesConfiguration.Devices.SingleOrDefault(nd => (nd.Port == uri.Port) && (nd.IPAddress == uri.Host));
        }

        public bool RebootNetworkDevice(DeviceViewModel deviceViewModel, bool unattended = false)
        {
            bool result = ExecuteNetworkDeviceCommand(deviceViewModel, "reboot", unattended);
            if (result)
            {
                //do not just flag this ViewModel rebooted
                //there may be several ViewModels with this IP, but only one
                //will be send the RebootNetworkDevice command
                string prefix = deviceViewModel.Path.Split(':').First() + ":";
                List<DeviceViewModel> ipDevices = LocalViewModel.Devices
                    .Where(d => (d.Kind == DeviceKind.NET) && d.Path.StartsWith(prefix))
                    .ToList();
                ipDevices.ForEach(d => d.LastReboot = DateTime.Now);
            }
            return result;
        }

        private bool ExecuteSshCommand(string deviceName, SshClient client, string commandText)
        {
            SshCommand command = client.RunCommand(commandText);
            var success = command.ExitStatus == 0;

            if (!success)
                PostNotification(string.Format("{0}: {1}", deviceName, command.Error), NotificationKind.Danger);

            return success;
        }

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

            //track this internally as well - in practice we cannot trust our
            //cached miner statistics data above 100%, especially in cases of a reboot
            if (warm)
            {
                warm = ((deviceViewModel.LastRestart == null) || ((DateTime.Now - deviceViewModel.LastRestart).Value.TotalSeconds > MiningEngine.SecondsToWarmUpMiner))
                    && ((deviceViewModel.LastReboot == null) || ((DateTime.Now - deviceViewModel.LastReboot).Value.TotalSeconds > MiningEngine.SecondsToWarmUpMiner));
            }

            return warm;
        }

        private void RestartNetworkDevicesForSubparHashrate()
        {
            IEnumerable<DeviceViewModel> suspectNetworkDevices =
                LocalViewModel
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
                LocalViewModel
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

        //this is a high-traffic call, guard against making repeated RPC calls
        public bool NetworkDeviceWasStopped(DeviceViewModel networkDevice)
        {
            List<PoolInformation> poolInformation = null;

            //networkDevicePools is keyed by IP:port, use .Path
            //do not call GetCachedPoolInfoFromAddress() as it may make an RPC call
            //see method warning above
            if (NetworkDevicePools.ContainsKey(networkDevice.Path))
                poolInformation = NetworkDevicePools[networkDevice.Path];

            if (poolInformation == null)
                //RPC API call timed out
                return false;

            return poolInformation.All(pool => pool.Status.Equals("Disabled"));
        }

        private void RestartSuspectNetworkDevice(DeviceViewModel networkDevice, string reason)
        {
            //don't restart a Network Device we've stopped
            if (NetworkDeviceWasStopped(networkDevice))
                return;

            string message = String.Format("Restarting {0} ({1})", networkDevice.FriendlyName, reason);
            try
            {
                if (!RestartNetworkDevice(networkDevice))
                {
                    if (!ApplicationConfiguration.ShowApiErrors)
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
            PostNotification(message, NotificationKind.Danger);
        }

        private static void ClearChainStatus(DeviceViewModel networkDevice)
        {
            for (int i = 0; i < networkDevice.ChainStatus.Length; i++)
                networkDevice.ChainStatus[i] = String.Empty;
        }
        #endregion

        #region Notifications
        private void PostNotification(string id, string text, Action clickHandler, NotificationKind kind, string informationUrl)
        {
            NotificationEventArgs notification = new NotificationEventArgs
            {
                Id = id,
                Text = text,
                ClickHandler = clickHandler,
                Kind = kind,
                InformationUrl = informationUrl
            };

            if (NotificationReceived != null) NotificationReceived(this, notification);
        }

        private void PostNotification(string text, NotificationKind icon, string informationUrl = "")
        {
            PostNotification(text, text, () => { }, icon, informationUrl);
        }

        private void PostNotification(string text, Action clickHandler, NotificationKind icon, string informationUrl = "")
        {
            PostNotification(text, text, clickHandler, icon, informationUrl);
        }
        #endregion

        #region Mining logic
        public void StartMining()
        {
            if (SelectedRemoteInstance == null)
                StartMiningLocally();
            else
                StartMiningRemotely(SelectedRemoteInstance);
        }

        public void StopMining()
        {
            if (SelectedRemoteInstance == null)
                StopMiningLocally();
            else
                StopMiningRemotely(SelectedRemoteInstance);
        }

        public void SetAllDevicesToCoin(string coinSymbol, bool disableStrategies)
        {
            if (SelectedRemoteInstance == null)
                SetAllDevicesToCoinLocally(coinSymbol, disableStrategies);
            else
                SetAllDevicesToCoinRemotely(SelectedRemoteInstance, coinSymbol, disableStrategies);
        }

        public void SetAllDevicesToCoinOnAllRigs(string coinSymbol, bool disableStrategies)
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = InstanceManager.Instances.Where(i => i != InstanceManager.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
                SetAllDevicesToCoinRemotely(instance, coinSymbol, disableStrategies);
        }

        public void RenameDevice(DeviceViewModel deviceViewModel, string name)
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

        private void CheckIdleTimeForDynamicIntensity(double timerInterval)
        {
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                return; //idle detection code uses User32.dll

            if (ApplicationConfiguration.AutoSetDesktopMode)
            {
                TimeSpan idleTimeSpan = TimeSpan.FromMilliseconds(Environment.TickCount - IdleTimeFinder.GetLastInputTime());

                const int idleMinutesForDesktopMode = 2;

                //if idle for more than 1 minute, disable Desktop Mode
                if (idleTimeSpan.TotalMinutes > idleMinutesForDesktopMode)
                {
                    if (EngineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        ToggleDynamicIntensityLocally(false);
                        RestartMiningLocallyIfMining();
                    }
                }
                //else if idle for less than the idleTimer interval, enable Desktop Mode
                else if (idleTimeSpan.TotalMilliseconds <= timerInterval)
                {
                    if (!EngineConfiguration.XgminerConfiguration.DesktopMode)
                    {
                        ToggleDynamicIntensityLocally(true);
                        RestartMiningLocallyIfMining();
                    }
                }
            }
        }

        private void RestartMiningLocallyIfMining()
        {
            if (MiningEngine.Mining)
            {
                StopMiningLocally();
                StartMiningLocally();
            }
        }

        public bool ScanHardwareLocally()
        {
            if (ProgressStarted != null)
                ProgressStarted(this, new ProgressEventArgs
                {
                    Text = "Scanning hardware for devices capable of mining. Please be patient."
                });
            try
            {
                try
                {
                    DevicesService devicesService = new DevicesService(EngineConfiguration.XgminerConfiguration);
                    MinerDescriptor defaultMiner = MinerFactory.Instance.GetDefaultMiner();
                    Devices = devicesService.GetDevices(MinerPath.GetPathToInstalledMiner(defaultMiner));

                    //pull in virtual Proxy Devices
                    UpdateDevicesForProxySettings();

                    //safe to do here as we are Scanning Hardware - we are not mining
                    //no data to lose in the ViewModel
                    //clearing means our sort order within the ListView is preserved
                    //and things like selecting the first item work better
                    //http://social.msdn.microsoft.com/Forums/windows/en-US/8a81c5a6-251c-4bf9-91c5-a937b5cfe9f3/possible-bug-in-listview-control-topitem-property-doesnt-work-with-groups
                    LocalViewModel.Devices.Clear();

                    ApplyModelsToViewModel();
                }
                catch (Win32Exception)
                {
                    //miner not installed/not launched
                    Devices = new List<Device>(); //dummy empty device list

                    return false;
                }

                if ((Devices.Count > 0) && (EngineConfiguration.DeviceConfigurations.Count == 0) &&
                    (EngineConfiguration.CoinConfigurations.Count == 1))
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
                EngineConfiguration.RemoveDuplicateDeviceConfigurations();

                //clean up mappings from previous device list
                DeviceDetailsMapping.Clear();

                //cache devices
                SaveKnownDevicesToFile();
            }
            finally
            {
                if (ProgressCompleted != null) ProgressCompleted(this, new EventArgs());
            }
            
            return true;
        }

        private void UpdateDevicesForProxySettings()
        {
            //devices not populated / loaded, e.g. remoting error loading app (socket in use)
            if (Devices == null)
                return;

            DevicesService service = new DevicesService(EngineConfiguration.XgminerConfiguration);
            service.UpdateDevicesForProxySettings(Devices, miningEngine.Mining);
            AddMissingDeviceConfigurations();
        }

        private void StartMiningLocally()
        {
            //do not set Dynamic Intensity here - may have already been set by idleTimer_Tick
            //don't want to override

            CancelMiningOnStartup(); //in case clicked during countdown

            SaveChangesLocally();

            if (!MiningConfigurationValid())
                return;

            if (MiningEngine.Mining)
                return;

            //download miners BEFORE checking for config files
            DownloadRequiredMiners();

            if (!ConfigFileHandled())
                return;
            
            int donationPercent = 0;
            if (PerksConfiguration.PerksEnabled)
                donationPercent = PerksConfiguration.DonationPercent;

            try
            {
                MiningEngine.StartMining(EngineConfiguration, Devices, 
                    CoinApiInformation.ToList(), //get a copy - populated async & collection may be modified
                    donationPercent);
            }
            catch (MinerLaunchException ex)
            {
                MessageBoxShow(ex.Message, "Error Launching Miner", PromptButtons.OK, PromptIcon.Error);
                return;
            }

            //do this AFTER we start mining to pick up any Auto-Mining changes

            //create a deep clone of the mining & device configurations
            //this is so we can accurately display e.g. the currently mining pools
            //even if the user changes pool info without restartinging mining
            miningCoinConfigurations = ObjectCopier.DeepCloneObject<List<Coin>, List<Coin>>(EngineConfiguration.CoinConfigurations);
            miningDeviceConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Device>, List<Engine.Data.Configuration.Device>>(EngineConfiguration.DeviceConfigurations);

            EngineConfiguration.SaveDeviceConfigurations(); //save any changes made by the engine

            //update ViewModel with potential changes 
            ApplyModelsToViewModel();

            if (DataModified != null) DataModified(this, new EventArgs());

            SaveOwnedProcesses();

            //so restart timer based on when mining started, not a constantly ticking timer
            //see https://bitcointalk.org/index.php?topic=248173.msg4593795#msg4593795
            SetupRestartTimer();

            if (EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins &&
                // if no Internet / network connection, we did not Auto-Mine
                (CoinApiInformation != null))
                ShowCoinChangeNotification();
        }

        private void SetAllDevicesToCoinLocally(string coinSymbol, bool disableStrategies)
        {
            Coin coinConfiguration = EngineConfiguration
                .CoinConfigurations
                .SingleOrDefault(c => c.PoolGroup.Id.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));

            if (coinConfiguration == null)
            {
                //try short-hand
                coinConfiguration = EngineConfiguration
                    .CoinConfigurations
                    .SingleOrDefault(c => c.PoolGroup.Id.ShortCoinSymbol().Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            }

            //no such coin symbol
            if (coinConfiguration == null) return;

            bool wasMining = MiningEngine.Mining;
            StopMiningLocally();
            
            EngineConfiguration.DeviceConfigurations.Clear();

            foreach (Device device in Devices)
            {
                //don't assume 1-to-1 of Devices and ViewModel.Devices
                //Devices doesn't include Network Devices
                DeviceViewModel viewModel = LocalViewModel.Devices.Single(vm => vm.Equals(device));

                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device();
                deviceConfiguration.Assign(viewModel);
                if (viewModel.Kind == DeviceKind.NET)
                {
                    //assume BTC for Network Devices (for now)
                    deviceConfiguration.CoinSymbol = Engine.Data.KnownCoins.BitcoinSymbol;
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

                EngineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);

            EngineConfiguration.SaveDeviceConfigurations();
            
            if (wasMining)
            {
                bool wasAutoMining = EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins;
                if (wasAutoMining)
                    EnableMiningStrategies(false);

                StartMiningLocally();

                //only re-enable if they were enabled before
                if (!disableStrategies && wasAutoMining)
                    EnableMiningStrategies();
            }
            else
            {
                if (disableStrategies)
                    EnableMiningStrategies(false);
            }

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        public Dictionary<string, double> GetIncomeForCoins()
        {
            Dictionary<string, double> coinsIncome = new Dictionary<string, double>();

            MinerFormViewModel viewModelToView = GetViewModelToView();

            foreach (DeviceViewModel deviceViewModel in viewModelToView.Devices)
            {
                //check for Coin != null, device may not have a coin configured
                if (deviceViewModel.Coin != null)
                {
                    string coinSymbol = Engine.Data.KnownCoins.BitcoinSymbol;
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

        private void EnableMiningStrategies(bool enabled = true)
        {
            EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = enabled;
            EngineConfiguration.SaveStrategyConfiguration();
        }

        //download miners required for configured coins / algorithms
        private void DownloadRequiredMiners()
        {
            IEnumerable<string> configuredAlgorithms = EngineConfiguration.CoinConfigurations
                .Where(config => config.Enabled)
                .Select(config => config.PoolGroup.Algorithm)
                .Distinct();

            SerializableDictionary<string, string> algorithmMiners = EngineConfiguration.XgminerConfiguration.AlgorithmMiners;

            foreach (string algorithmName in configuredAlgorithms)
            {
                //safe to assume we are downloading GPU miners here
                MinerDescriptor miner = MinerFactory.Instance.GetMiner(DeviceKind.GPU, algorithmName, algorithmMiners);

                //is miner configured for algorithm?
                if ((miner != null)
                    //is miner available for download on this OS?
                    && !String.IsNullOrEmpty(miner.Url))
                {
                    CheckAndDownloadMiner(miner);
                }
            }
        }

        private void CheckAndDownloadMiner(MinerDescriptor miner)
        {
            string installedFilePath = MinerPath.GetPathToInstalledMiner(miner);
            if (!File.Exists(installedFilePath))
                InstallBackendMinerLocally(miner);
        }

        public void InstallBackendMinerLocally(MinerDescriptor miner)
        {
            string minerName = miner.Name;

            if (ProgressStarted != null)
                ProgressStarted(this, new ProgressEventArgs
                {
                    Text = String.Format("Downloading and installing {0} from {1}", minerName, new Uri(miner.Url).Authority)
                });

            try
            {
                string minerPath = System.IO.Path.Combine("Miners", minerName);
                string destinationFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
                try
                {
                    MinerInstaller.InstallMiner(UserAgent.AgentString, miner, destinationFolder);
                    //may have been installed via Remoting - dismiss notification
                    if (NotificationDismissed != null)
                        NotificationDismissed(this, new NotificationEventArgs
                        {
                            Id = BfgMinerNotificationId.ToString()
                        });
                }
                catch (NotImplementedException)
                {
                    //don't crash on *nix when downloads get triggered
                    PostNotification("Auto installation not supported for your OS", () => 
                    {
                        MessageBoxShow(
                            String.Format("You must install {0} for your OS manually.", miner.Name),
                            "Not Implemented",
                            PromptButtons.OK, 
                            PromptIcon.Warning);
                    }, NotificationKind.Warning);
                }
            }
            finally
            {
                if (ProgressCompleted != null) ProgressCompleted(this, new EventArgs());
            }
        }

        public void CancelMiningOnStartup()
        {
            StartupMiningCountdownSeconds = 0;
            if (DataModified != null) DataModified(this, new EventArgs());
        }

        public void StopMiningLocally()
        {
            CancelMiningOnStartup(); //in case called during countdown

            miningEngine.StopMining();

            LocalViewModel.ClearDeviceInformationFromViewModel();

            UpdateDevicesForProxySettings();
            ApplyModelsToViewModel();
            ClearCachedNetworkDifficulties();
            processDeviceDetails.Clear();
            lastDevicePoolMapping.Clear();

            ClearPoolsFlaggedDown();
            SaveOwnedProcesses();

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void CheckAndNotifyFoundBlocks(MinerProcess minerProcess, long foundBlocks)
        {
            //started mining but haven't yet assigned mining members
            if (miningCoinConfigurations == null)
                return;

            string coinName = minerProcess.MinerConfiguration.CoinName;
            //reference miningCoinConfigurations so that we get access to the mining coins
            Coin configuration = miningCoinConfigurations.SingleOrDefault(c => c.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnBlockFound2 && (foundBlocks > minerProcess.FoundBlocks))
            {
                minerProcess.FoundBlocks = foundBlocks;

                PostNotification(String.Format("Block(s) found for {0} (block {1})",
                    coinName, minerProcess.FoundBlocks), NotificationKind.Information);
            }
        }

        private void CheckAndNotifyAcceptedShares(MinerProcess minerProcess, long acceptedShares)
        {
            //started mining but haven't yet assigned mining members
            if (miningCoinConfigurations == null)
                return;

            string coinName = minerProcess.MinerConfiguration.CoinName;
            //reference miningCoinConfigurations so that we get access to the mining coins
            Coin configuration = miningCoinConfigurations.SingleOrDefault(c => c.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase));
            if (configuration == null)
                return;

            if (configuration.NotifyOnShareAccepted2 && (acceptedShares > minerProcess.AcceptedShares))
            {
                minerProcess.AcceptedShares = acceptedShares;

                PostNotification(String.Format("Share(s) accepted for {0} (share {1})",
                    coinName, minerProcess.AcceptedShares), NotificationKind.Information);
            }
        }

        private void ClearPoolsFlaggedDown()
        {
            foreach (Coin coinConfiguration in EngineConfiguration.CoinConfigurations)
                coinConfiguration.PoolsDown = false;
            EngineConfiguration.SaveCoinConfigurations();
        }

        //https://github.com/nwoolls/MultiMiner/issues/152
        //http://social.msdn.microsoft.com/Forums/vstudio/en-US/94ba760c-7080-4614-8a56-15582c48f900/child-process-keeps-parents-socket-open-diagnosticsprocess-and-nettcplistener?forum=netfxbcl
        //keep track of processes we've launched so we can kill them later
        private void SaveOwnedProcesses()
        {
            OwnedProcesses.SaveOwnedProcesses(MiningEngine.MinerProcesses.Select(mp => mp.Process), GetOwnedProcessFilePath());
        }

        private static string GetOwnedProcessFilePath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetTempPath(), "MultiMiner.Processes.xml");
        }

        public static bool KillOwnedProcesses()
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

        private bool ConfigFileHandled()
        {
            return MinerFactory.Instance.Miners.All(ConfigFileHandledForMiner);
        }

        private bool ConfigFileHandledForMiner(MinerDescriptor miner)
        {
            const string bakExtension = ".mmbak";
            string minerName = miner.Name;
            string minerExecutablePath = MinerPath.GetPathToInstalledMiner(miner);
            string confFileFilePath;

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
            {
                string minerFolderName = "." + minerName;
                string minerFileName = minerName + ".conf";
                confFileFilePath = System.IO.Path.Combine(System.IO.Path.Combine(OSVersionPlatform.GetHomeDirectoryPath(), minerFolderName), minerFileName);
            }
            else
            {
                confFileFilePath = System.IO.Path.ChangeExtension(minerExecutablePath, ".conf");
            }

            if (File.Exists(confFileFilePath))
            {
                string confFileName = System.IO.Path.GetFileName(confFileFilePath);
                string confBakFileName = confFileName + bakExtension;

                PromptResult dialogResult = MessageBoxShow(String.Format("A {0} file has been detected in your miner directory. This file interferes with the arguments supplied by MultiMiner. Can MultiMiner rename this file to {1}?",
                    confFileName, confBakFileName), "External Configuration Detected", PromptButtons.YesNo, PromptIcon.Warning);
                if (dialogResult == PromptResult.No)
                    return false;

                string confBakFileFilePath = confFileFilePath + bakExtension;
                File.Delete(confBakFileFilePath);
                File.Move(confFileFilePath, confBakFileFilePath);
            }

            return true;
        }

        private void CheckMiningOnStartupStatus()
        {
            if (StartupMiningCountdownSeconds > 0)
            {
                StartupMiningCountdownSeconds--;
                if (StartupMiningCountdownSeconds == 0)
                    StartMiningLocally();

                if (DataModified != null) DataModified(this, new EventArgs());
            }
        }

        public void SetupMiningOnStartup()
        {
            if (ApplicationConfiguration.StartMiningOnStartup)
            {
                //minimum 1s delay for mining on startup - 0 not allowed
                StartupMiningCountdownSeconds = Math.Max(1, ApplicationConfiguration.StartupMiningDelay);
                if (DataModified != null) DataModified(this, new EventArgs());
            }
        }

        private void SetDevicesToCoinLocally(IEnumerable<DeviceDescriptor> devices, string coinName)
        {
            foreach (DeviceDescriptor device in devices)
            {
                DeviceViewModel deviceViewModel = LocalViewModel.Devices.SingleOrDefault(dvm => dvm.Equals(device));
                if ((deviceViewModel != null) && (deviceViewModel.Kind != DeviceKind.NET))
                    deviceViewModel.Coin = EngineConfiguration.CoinConfigurations.Single(cc => cc.PoolGroup.Name.Equals(coinName)).PoolGroup;
            }
            ApplyCoinInformationToViewModel();

            SetHasChangesLocally(true);
        }

        public void RestartMining()
        {
            if (SelectedRemoteInstance == null)
                RestartMiningLocally();
            else
                RestartMiningRemotely(SelectedRemoteInstance);
        }

        private void RestartMiningLocally()
        {
            StopMiningLocally();

            //refresh stats from Coin API so the Restart button can be used as a way to
            //force MultiMiner to apply updated mining strategies
            RefreshCoinStats();

            StartMiningLocally();
        }

        public void RefreshCoinStats()
        {
            RefreshAllCoinStats();
            UpdateApplicationFromCoinStats();
        }

        private void RefreshCoinStatsAsync()
        {
            Action asyncAction = RefreshAllCoinStats;
            asyncAction.BeginInvoke(
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (Context == null) return;

                    Context.BeginInvoke((Action)(UpdateApplicationFromCoinStats), null);

                }, null);
        }

        private void UpdateApplicationFromCoinStats()
        {
            SaveCoinStatsToKnownCoins();
            SuggestCoinsToMine();
            CheckAndApplyMiningStrategy();

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void CheckAndApplyMiningStrategy()
        {
            if (MiningEngine.Mining && EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
            {
                bool changed;
                try
                {
                    changed = MiningEngine.ApplyMiningStrategy(CoinApiInformation
                        .ToList()); //get a copy - populated async & collection may be modified)
                }
                catch (MinerLaunchException ex)
                {
                    MessageBoxShow(ex.Message, "Error Launching Miner", PromptButtons.OK, PromptIcon.Error);
                    return;
                }

                //save any changes made by the engine
                EngineConfiguration.SaveDeviceConfigurations();

                //create a deep clone of the mining & device configurations
                //this is so we can accurately display e.g. the currently mining pools
                //even if the user changes pool info without restarting mining
                miningCoinConfigurations = ObjectCopier.DeepCloneObject<List<Coin>, List<Coin>>(EngineConfiguration.CoinConfigurations);
                miningDeviceConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Device>, List<Engine.Data.Configuration.Device>>(EngineConfiguration.DeviceConfigurations);

                //update the ViewModel
                ApplyModelsToViewModel();

                if (changed)
                {
                    ShowCoinChangeNotification();
                    SaveOwnedProcesses();
                }

                //to get changes from strategy config
                //to refresh coin stats due to changed coin selections
                if (DataModified != null) DataModified(this, new EventArgs());
            }
        }

        private void ShowCoinChangeNotification()
        {
            IEnumerable<string> coinList = MiningEngine.MinerProcesses
                .Select(mp => mp.CoinSymbol)
                // there may be multiple processes for one coin symbol
                .Distinct()
                // sort the symbols
                .OrderBy(cs => cs);

            string id = Guid.NewGuid().ToString();
            string text = String.Format("Mining switched to {0} based on {1}",
                String.Join(", ", coinList.ToArray()),
                EngineConfiguration.StrategyConfiguration.MiningBasis);
            string url = successfulApiContext.GetInfoUrl();

            PostNotification(id, text, ConfigureStrategies, NotificationKind.Information, url);
        }

        public void SuggestCoinsToMine()
        {
            if (!ApplicationConfiguration.SuggestCoinsToMine)
                return;
            if (ApplicationConfiguration.SuggestionsAlgorithm == CoinSuggestionsAlgorithm.None)
                return;
            if (CoinApiInformation == null) //no network connection
                return;
            if (successfulApiContext == null) //no network connection / coin API offline
                return;

            IEnumerable<CoinInformation> coinsToMine = CoinSuggestions.GetCoinsToMine(
                CoinApiInformation
                    .ToList(), //get a copy - populated async & collection may be modified) 
                ApplicationConfiguration.SuggestionsAlgorithm,
                EngineConfiguration.StrategyConfiguration,
                EngineConfiguration.CoinConfigurations);

            foreach (CoinInformation coin in coinsToMine)
                NotifyCoinToMine(successfulApiContext, coin);
        }

        private void NotifyCoinToMine(IApiContext apiContext, CoinInformation coin)
        {
            string value = coin.AverageProfitability.ToString(".#") + "%";
            string noun = "average profitability";

            switch (EngineConfiguration.StrategyConfiguration.MiningBasis)
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
                }, NotificationKind.Information, infoUrl);
        }

        private void SaveCoinStatsToKnownCoins()
        {
            foreach (CoinInformation item in CoinApiInformation)
            {
                //find existing known coin or create a knew one
                PoolGroup knownCoin = KnownCoins.SingleOrDefault(c => c.Id.Equals(item.Symbol));
                if (knownCoin == null)
                {
                    knownCoin = new PoolGroup();
                    KnownCoins.Add(knownCoin);
                }

                knownCoin.Id = item.Symbol;
                knownCoin.Name = item.Name;
                knownCoin.Algorithm = item.Algorithm;
                knownCoin.Kind = item.IsMultiCoin ? PoolGroup.PoolGroupKind.MultiCoin : PoolGroup.PoolGroupKind.SingleCoin;
            }
            SaveKnownCoinsToFile();
        }

        public void CancelChanges()
        {
            if (SelectedRemoteInstance == null)
                CancelChangesLocally();
            else
                CancelChangesRemotely(SelectedRemoteInstance);
        }

        private void CancelChangesLocally()
        {
            EngineConfiguration.LoadDeviceConfigurations();

            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);
            ApplyCoinInformationToViewModel();

            SetHasChangesLocally(false);
        }

        public void ToggleDynamicIntensityLocally(bool enabled)
        {
            EngineConfiguration.XgminerConfiguration.DesktopMode = enabled;
            EngineConfiguration.SaveMinerConfiguration();
            GetViewModelToView().DynamicIntensity = enabled;

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        public void ToggleDynamicIntensity(bool enabled)
        {
            if (SelectedRemoteInstance == null)
                ToggleDynamicIntensityLocally(enabled);
            else
                ToggleDynamicIntensityRemotely(SelectedRemoteInstance, enabled);
        }

        private void ToggleDevicesLocally(IEnumerable<DeviceDescriptor> descriptors, bool enabled)
        {
            foreach (DeviceDescriptor descriptor in descriptors)
            {
                DeviceViewModel viewModel = LocalViewModel.Devices.SingleOrDefault(dvm => dvm.Equals(descriptor));
                if (viewModel != null)
                    viewModel.Enabled = enabled;
            }

            SetHasChangesLocally(true);

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        public void ToggleDevices(IEnumerable<DeviceDescriptor> devices, bool enabled)
        {
            if (SelectedRemoteInstance == null)
                ToggleDevicesLocally(devices, enabled);
            else
                ToggleDevicesRemotely(SelectedRemoteInstance, devices, enabled);
        }

        public string GetCurrentCultureCurrency()
        {
            string currencySymbol = RegionInfo.CurrentRegion.ISOCurrencySymbol;
            if ((SellPrices != null) && (SellPrices.SingleOrDefault(sp => sp.TargetCurrency.Equals(currencySymbol)) == null))
                currencySymbol = "USD";

            return currencySymbol;
        }

        private bool ShouldShowExchangeRates()
        {
            //check .Mining to allow perks for Remoting when local PC is not mining
            return ((MiningEngine.Donating || !MiningEngine.Mining) && PerksConfiguration.ShowExchangeRates
                //ensure Exchange prices are available:
                && (SellPrices != null));
        }

        public string GetExchangeRate(DeviceViewModel device)
        {
            var exchange = String.Empty;
            if ((device.Coin != null) && (device.Coin.Kind == PoolGroup.PoolGroupKind.SingleCoin))
            {
                if (ShouldShowExchangeRates())
                {
                    ExchangeInformation exchangeInformation = SellPrices.Single(er => er.TargetCurrency.Equals(GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
                    double btcExchangeRate = exchangeInformation.ExchangeRate;

                    double coinExchangeRate = device.Price * btcExchangeRate;

                    exchange = String.Format("{0}{1}", exchangeInformation.TargetSymbol, coinExchangeRate.ToFriendlyString(true));
                }
            }
            return exchange;
        }

        public string GetHashRateStatusText()
        {
            MinerFormViewModel viewModel = GetViewModelToView();
            string hashRateText = String.Empty;
            IList<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            foreach (CoinAlgorithm algorithm in algorithms)
            {
                double hashRate = GetVisibleInstanceHashrate(algorithm.Name, viewModel == LocalViewModel);
                if (hashRate > 0.00)
                {
                    if (!String.IsNullOrEmpty(hashRateText))
                        hashRateText = hashRateText + "   ";
                    hashRateText = String.Format("{0}{1}: {2}", hashRateText, algorithm.Name, hashRate.ToHashrateString());
                }
            }
            return hashRateText;
        }

        public List<DeviceViewModel> GetVisibleDevices()
        {
            var minerForm = GetViewModelToView();
            var devices = minerForm.Devices
                .Where(d => d.Visible)
                .ToList();
            return devices;
        }

        public int GetVisibleDeviceCount()
        {
            MinerFormViewModel viewModel = GetViewModelToView();
            //don't include Network Devices in the count for Remote ViewModels
            return viewModel.Devices.Count(d => d.Visible && ((viewModel == LocalViewModel) || (d.Kind != DeviceKind.NET)));
        }

        public string GetIncomeSummaryText()
        {
            //no internet or error parsing API
            if (SellPrices == null) return String.Empty;

            //no internet or error parsing API
            if (CoinApiInformation == null) return String.Empty;

            //check .Mining to allow perks for Remoting when local PC is not mining
            if ((!MiningEngine.Donating && MiningEngine.Mining) || !PerksConfiguration.ShowIncomeRates)
                return String.Empty;

            Dictionary<string, double> incomeForCoins = GetIncomeForCoins();

            if (incomeForCoins.Count == 0) return String.Empty;

            string summary = String.Empty;

            const string addition = " + ";
            double usdTotal = 0.00;
            string usdSymbol = "$";
            foreach (string coinSymbol in incomeForCoins.Keys)
            {
                double coinIncome = incomeForCoins[coinSymbol];
                CoinInformation coinInfo = CoinApiInformation
                    .ToList() //get a copy - populated async & collection may be modified
                    .SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
                if (coinInfo != null)
                {
                    ExchangeInformation exchangeInformation = SellPrices.Single(er => er.TargetCurrency.Equals(GetCurrentCultureCurrency()) && er.SourceCurrency.Equals("BTC"));
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

                if (PerksConfiguration.ShowExchangeRates)
                    summary = String.Format("{0} = {1}{2} / day", summary, usdSymbol, usdTotal.ToFriendlyString(true));
            }

            return summary;
        }

        public bool RemoveExistingPool(string coinSymbol, string url, string user)
        {
            Engine.Data.Configuration.Coin coinConfig = EngineConfiguration.CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (coinConfig == null) return false;

            MiningPool poolConfig = FindPoolConfiguration(coinConfig, url, user);

            if (poolConfig == null) return false;

            coinConfig.Pools.Remove(poolConfig);
            EngineConfiguration.SaveCoinConfigurations();

            return true;
        }

        public MiningPool FindPoolConfiguration(Coin coinConfig, string url, string user)
        {
            MiningPool poolConfig = coinConfig.Pools.FirstOrDefault((p) =>
            {
                //url may or may not have protocol, but URI ctor requires one
                //so strip any that exists with ShortHostFromHost() then add a dummy
                Uri uri = new Uri("http://" + url.ShortHostFromHost());

                var cleanUri = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port,
                               UriFormat.UriEscaped);

                var portsEqual = (p.Port == uri.Port);
                var hostsEqual = (p.Host.ShortHostFromHost().Equals(cleanUri.ShortHostFromHost(), StringComparison.OrdinalIgnoreCase));
                var usersEqual = (string.IsNullOrEmpty(user) || p.Username.Equals(user, StringComparison.OrdinalIgnoreCase));

                return portsEqual
                    && hostsEqual
                    && usersEqual;
            });
            return poolConfig;
        }

        public MiningPool AddNewPool(CoinApi.Data.CoinInformation coin, string url, string user, string pass)
        {
            Engine.Data.Configuration.Coin coinConfig = EngineConfiguration.CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(coin.Symbol, StringComparison.OrdinalIgnoreCase));
            if (coinConfig == null)
            {
                coinConfig = new Engine.Data.Configuration.Coin();
                var algorithm = coin.Algorithm;

                //we don't want the FullName
                KnownAlgorithm knownAlgorithm = KnownAlgorithms.Algorithms.SingleOrDefault(a => a.FullName.Equals(algorithm, StringComparison.OrdinalIgnoreCase));
                if (knownAlgorithm != null) algorithm = knownAlgorithm.Name;

                coinConfig.PoolGroup = new Engine.Data.PoolGroup
                {
                    Algorithm = algorithm,
                    Id = coin.Symbol,
                    Kind = coin.Symbol.Contains(':') ? Engine.Data.PoolGroup.PoolGroupKind.MultiCoin : Engine.Data.PoolGroup.PoolGroupKind.SingleCoin,
                    Name = coin.Name
                };
                EngineConfiguration.CoinConfigurations.Add(coinConfig);
            }

            Uri uri = new Uri(url);
            Xgminer.Data.MiningPool poolConfig = new Xgminer.Data.MiningPool
            {
                Host = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped),
                Port = uri.Port,
                Password = pass,
                Username = user
            };
            coinConfig.Pools.Add(poolConfig);
            EngineConfiguration.SaveCoinConfigurations();

            return poolConfig;
        }
        #endregion

        #region Stats API
        //public void SubmitMultiMinerStatistics()
        //{
        //    string installedVersion = MultiMinerInstaller.GetInstalledMinerVersion();
        //    if (installedVersion.Equals(ApplicationConfiguration.SubmittedStatsVersion))
        //        return;

        //    Machine machineStat = new Machine
        //    {
        //        Name = Environment.MachineName,
        //        MinerVersion = installedVersion
        //    };

        //    if (submitMinerStatisticsDelegate == null)
        //        submitMinerStatisticsDelegate = SubmitMinerStatistics;

        //    submitMinerStatisticsDelegate.BeginInvoke(machineStat, submitMinerStatisticsDelegate.EndInvoke, null);
        //}
        //private Action<Machine> submitMinerStatisticsDelegate;

        //private void SubmitMinerStatistics(Machine machineStat)
        //{
        //    try
        //    {
        //        //plain text so users can see what we are posting - transparency
        //        Stats.ApiContext.SubmitMinerStatistics("http://multiminerstats.azurewebsites.net/api/", machineStat);
        //        ApplicationConfiguration.SubmittedStatsVersion = machineStat.MinerVersion;
        //    }
        //    catch (WebException)
        //    {
        //        //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
        //    }
        //}
        #endregion

        #region Mining event handlers
        private void LogApiEvent(object sender, LogEventArgs eventArgs)
        {
            ApiLogEntry logEntry = new ApiLogEntry
            {
                DateTime = eventArgs.DateTime,
                Request = eventArgs.Request,
                Response = eventArgs.Response
            };

            ApiContext apiContext = (ApiContext)sender;
            logEntry.CoinName = GetCoinNameForApiContext(apiContext);
            logEntry.Machine = String.Format("{0}:{1}", apiContext.IpAddress, apiContext.Port);

            //make sure BeginInvoke is allowed
            if (Context != null)
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    //remove then add so BindingList position is on latest entry
                    while (ApiLogEntries.Count > MaxLogEntriesOnScreen)
                        ApiLogEntries.RemoveAt(0);
                    ApiLogEntries.Add(logEntry);
                }), null);
            }

            LogApiEventToFile(logEntry);
        }

        private string GetCoinNameForApiContext(ApiContext apiContext)
        {
            string coinName = string.Empty;

            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
            {
                ApiContext loopContext = minerProcess.ApiContext;
                if (loopContext == apiContext)
                {
                    coinName = minerProcess.MinerConfiguration.CoinName;
                    break;
                }
            }

            return coinName;
        }
        #endregion

        #region Logging logic
        private void LogApiEventToFile(ApiLogEntry logEntry)
        {
            const string logFileName = "ApiLog.json";
            LogObjectToFile(logEntry, logFileName);
        }

        private void LogObjectToFile(object objectToLog, string logFileName)
        {
            string logDirectory = GetLogDirectory();
            string logFilePath = System.IO.Path.Combine(logDirectory, logFileName);
            ObjectLogger logger = new ObjectLogger(ApplicationConfiguration.RollOverLogFiles, ApplicationConfiguration.OldLogFileSets);
            logger.LogObjectToFile(objectToLog, logFilePath);
        }

        public string GetLogDirectory()
        {
            string logDirectory = ApplicationPaths.AppDataPath();
            if (!String.IsNullOrEmpty(ApplicationConfiguration.LogFilePath))
            {
                Directory.CreateDirectory(ApplicationConfiguration.LogFilePath);
                if (Directory.Exists(ApplicationConfiguration.LogFilePath))
                    logDirectory = ApplicationConfiguration.LogFilePath;
            }
            return logDirectory;
        }
        #endregion

        #region MobileMiner API
        //private string GetMobileMinerUrl()
        //{
        //    string prefix = "https://";
        //    if (!ApplicationConfiguration.MobileMinerUsesHttps)
        //        prefix = "http://";

        //    //custom domain makes it easier to migrate hosts if needed
        //    string result = prefix + "api.mobileminerapp.com";

        //    if (!OSVersionPlatform.IsWindowsVistaOrHigher())
        //        //SNI SSL not supported on XP
        //        result = prefix + "mobileminer.azurewebsites.net/api";

        //    return result;
        //}

        //private const string MobileMinerApiKey = "P3mVX95iP7xfoI";

        //don't show a dialog for a 403 after successful submissions.
        //it's not ideal but there have been two reports now of this
        //being triggered by someone who has valid credentials, and
        //i've seen it myself as well
        //private bool mobileMinerSuccess;
        //private void SubmitMobileMinerStatistics()
        //{
        //    //are remote monitoring enabled?
        //    if (!ApplicationConfiguration.MobileMinerMonitoring)
        //        return;

        //    //is MobileMiner configured?
        //    if (!ApplicationConfiguration.IsMobileMinerConfigured())
        //        return;

        //    List<MiningStatistics> statisticsList = new List<MiningStatistics>();

        //    Action<List<MiningStatistics>> asyncAction = AddAllMinerStatistics;
        //    asyncAction.BeginInvoke(statisticsList,
        //        ar =>
        //        {
        //            asyncAction.EndInvoke(ar);

        //            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
        //            if (Context == null) return;

        //            Context.BeginInvoke((Action)(() =>
        //            {
        //                //code to update UI

        //                if (statisticsList.Count > 0)
        //                {
        //                    if (submitMiningStatisticsDelegate == null)
        //                        submitMiningStatisticsDelegate = SubmitMiningStatistics;

        //                    submitMiningStatisticsDelegate.BeginInvoke(statisticsList, submitMiningStatisticsDelegate.EndInvoke, null);
        //                }

        //            }), null);

        //        }, null);
        //}

        //private void AddAllMinerStatistics(List<MiningStatistics> statisticsList)
        //{
        //    if (!ApplicationConfiguration.MobileMinerNetworkMonitorOnly)
        //        AddLocalMinerStatistics(statisticsList);

        //    AddAllNetworkMinerStatistics(statisticsList);
        //}

        //private void AddAllNetworkMinerStatistics(List<MiningStatistics> statisticsList)
        //{
        //    //is Network Device detection enabled?
        //    if (!ApplicationConfiguration.NetworkDeviceDetection)
        //        return;

        //    //call ToList() so we can get a copy - otherwise risk:
        //    //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
        //    List<NetworkDevices.NetworkDevice> networkDevices = NetworkDevicesConfiguration.Devices.ToList();

        //    foreach (NetworkDevices.NetworkDevice networkDevice in networkDevices)
        //        AddNetworkMinerStatistics(networkDevice, statisticsList);
        //}

        //private void AddNetworkMinerStatistics(NetworkDevices.NetworkDevice networkDevice, List<MiningStatistics> statisticsList)
        //{
        //    List<DeviceInformation> deviceInformationList = GetDeviceInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

        //    if (deviceInformationList == null) //handled failure getting API info
        //        return;

        //    List<PoolInformation> poolInformationList = GetCachedPoolInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

        //    VersionInformation versionInformation = GetVersionInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

        //    //we cannot continue without versionInformation as the MinerName is required by MobileMiner or it returns HTTP 400
        //    if (versionInformation == null) //handled failure getting API info
        //        return;

        //    foreach (DeviceInformation deviceInformation in deviceInformationList)
        //    {
        //        string devicePath = String.Format("{0}:{1}", networkDevice.IPAddress, networkDevice.Port);

        //        //don't submit stats until we have a valid ViewModel for the Network Device
        //        DeviceViewModel deviceViewModel = LocalViewModel.Devices.SingleOrDefault(d => d.Path.Equals(devicePath));
        //        if (deviceViewModel == null)
        //            continue;

        //        MiningStatistics miningStatistics = new MiningStatistics
        //        {
        //            // submit the Friendly device / machine name
        //            MachineName = LocalViewModel.GetFriendlyDeviceName(devicePath, devicePath),

        //            // versionInformation may be null if the read timed out
        //            MinerName = versionInformation.Name,
        //            CoinName = Engine.Data.KnownCoins.BitcoinName,
        //            CoinSymbol = Engine.Data.KnownCoins.BitcoinSymbol,
        //            Algorithm = AlgorithmFullNames.SHA256,
        //            Appliance = true
        //        };

        //        miningStatistics.PopulateFrom(deviceInformation);

        //        //ensure poolIndex is valid for poolInformationList
        //        //user(s) reported index errors so we can't out on the RPC API here
        //        //https://github.com/nwoolls/MultiMiner/issues/64
        //        if ((deviceInformation.PoolIndex >= 0) &&
        //            // poolInformationList may be null if an RPC API call timed out
        //            (poolInformationList != null) &&
        //            (deviceInformation.PoolIndex < poolInformationList.Count))
        //        {
        //            string poolUrl = poolInformationList[deviceInformation.PoolIndex].Url;
        //            miningStatistics.PoolName = poolUrl.DomainFromHost();

        //            Coin coinConfiguration = CoinConfigurationForPoolUrl(poolUrl);
        //            if (coinConfiguration != null)
        //            {
        //                miningStatistics.CoinName = coinConfiguration.PoolGroup.Name;
        //                miningStatistics.CoinSymbol = coinConfiguration.PoolGroup.Id;
        //                CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(coinConfiguration.PoolGroup.Algorithm);

        //                //MobileMiner is only SHA & Scrypt for now
        //                if ((algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA2) ||
        //                    (algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA3))
        //                    miningStatistics.Algorithm = AlgorithmFullNames.SHA256;
        //                else
        //                    miningStatistics.Algorithm = AlgorithmFullNames.Scrypt;
        //            }
        //        }

        //        statisticsList.Add(miningStatistics);
        //    }
        //}

        private Coin CoinConfigurationForPoolUrl(string poolUrl)
        {
            Coin coinConfiguration =
                EngineConfiguration.CoinConfigurations
                    .FirstOrDefault(cc =>
                        cc.Pools
                            .Any((p) => {
                                UriBuilder poolUri = new UriBuilder(poolUrl);

                                UriBuilder compareUri;
                                try
                                {
                                    compareUri = new UriBuilder(p.Host);
                                }
                                catch (UriFormatException)
                                {
                                    return false;
                                }

                                int poolPort = poolUri.Port;
                                int comparePort = p.Port;
                                string poolHost = poolUri.Host;
                                string compareHost = compareUri.Host;
                                return poolPort == comparePort &&
                                    poolHost.Equals(compareHost, StringComparison.OrdinalIgnoreCase);
                            })
                    );

            return coinConfiguration;
        }

        private List<DeviceInformation> GetDeviceInfoFromAddress(string ipAddress, int port)
        {
            ApiContext apiContext = new ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceInformation> deviceInformationList;
            try
            {
                try
                {
                    //some Network Devices don't have the horsepower to return API results immediately
                    const int commandTimeoutMs = 3000;
                    deviceInformationList = apiContext.GetDeviceInformation(commandTimeoutMs).Where(d => d.Enabled).ToList();
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

        private string GetFriendlyDeviceName(Device device)
        {
            string result = device.Name;

            DeviceViewModel deviceViewModel = LocalViewModel.Devices.SingleOrDefault(d => d.Equals(device));
            if ((deviceViewModel != null) && !String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                result = deviceViewModel.FriendlyName;

            return result;
        }

        private VersionInformation GetVersionInfoFromAddress(string ipAddress, int port)
        {
            ApiContext apiContext = new ApiContext(port, ipAddress);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            VersionInformation versionInformation;
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

        //private void AddLocalMinerStatistics(List<MiningStatistics> statisticsList)
        //{
        //    //call ToList() so we can get a copy - otherwise risk:
        //    //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
        //    List<MinerProcess> minerProcesses = MiningEngine.MinerProcesses.ToList();

        //    foreach (MinerProcess minerProcess in minerProcesses)
        //    {
        //        List<DeviceInformation> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);

        //        if (deviceInformationList == null) //handled failure getting API info
        //            continue;

        //        //starting with bfgminer 3.7 we need the DEVDETAILS response to tie things from DEVS up with -d? details
        //        List<DeviceDetails> processDevices = GetProcessDeviceDetails(minerProcess, deviceInformationList);

        //        if (processDevices == null) //handled failure getting API info
        //            continue;

        //        foreach (DeviceInformation deviceInformation in deviceInformationList)
        //        {
        //            MiningStatistics miningStatistics = new MiningStatistics { MachineName = Environment.MachineName };
        //            PopulateMobileMinerStatistics(miningStatistics, deviceInformation, GetCoinNameForApiContext(minerProcess.ApiContext));

        //            DeviceDetails deviceDetails = processDevices.SingleOrDefault(d => d.Name.Equals(deviceInformation.Name, StringComparison.OrdinalIgnoreCase)
        //                && (d.ID == deviceInformation.ID));
        //            int deviceIndex = GetDeviceIndexForDeviceDetails(deviceDetails, minerProcess);
        //            Device device = Devices[deviceIndex];
        //            Coin coinConfiguration = CoinConfigurationForDevice(device);

        //            miningStatistics.FullName = GetFriendlyDeviceName(device);

        //            miningStatistics.PoolName = GetPoolNameByIndex(coinConfiguration, deviceInformation.PoolIndex).DomainFromHost();

        //            statisticsList.Add(miningStatistics);
        //        }
        //    }
        //}

        private static string GetPoolNameByIndex(Coin coinConfiguration, int poolIndex)
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

        private Coin CoinConfigurationForDevice(Device device)
        {
            //get the actual device configuration, text in the ListViewItem may be unsaved
            Engine.Data.Configuration.Device deviceConfiguration;
            if (MiningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningDeviceConfigurations
                (miningDeviceConfigurations != null))
                deviceConfiguration = miningDeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));
            else
                deviceConfiguration = EngineConfiguration.DeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));

            if (deviceConfiguration == null)
                return null;

            string itemCoinSymbol = deviceConfiguration.CoinSymbol;

            List<Coin> configurations;
            if (MiningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningCoinConfigurations
                (miningCoinConfigurations != null))
                configurations = miningCoinConfigurations;
            else
                configurations = EngineConfiguration.CoinConfigurations;

            Coin coinConfiguration = configurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(itemCoinSymbol, StringComparison.OrdinalIgnoreCase));
            return coinConfiguration;
        }

        private int GetDeviceIndexForDeviceDetails(DeviceDetails deviceDetails, MinerProcess minerProcess)
        {
            int result = Devices
                .FindIndex(device => device.Driver.Equals(deviceDetails.Driver, StringComparison.OrdinalIgnoreCase)
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
                                         ));

            return result;
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

        private List<DeviceDetails> GetDeviceDetailsFromProcess(MinerProcess minerProcess)
        {
            ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceDetails> deviceDetailsList;
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


        private List<DeviceInformation> GetDeviceInfoFromProcess(MinerProcess minerProcess)
        {
            ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<DeviceInformation> deviceInformationList;
            try
            {
                try
                {
                    deviceInformationList = apiContext.GetDeviceInformation().Where(d => d.Enabled).ToList();
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

        //private void PopulateMobileMinerStatistics(MiningStatistics miningStatistics, DeviceInformation deviceInformation,
        //    string coinName)
        //{
        //    miningStatistics.MinerName = "MultiMiner";
        //    miningStatistics.CoinName = coinName;
        //    Coin coinConfiguration = EngineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Name.Equals(coinName));
        //    PoolGroup coin = coinConfiguration.PoolGroup;

        //    //don't send non-coin Ids to MobileMiner
        //    if (coin.Kind != PoolGroup.PoolGroupKind.MultiCoin)
        //        miningStatistics.CoinSymbol = coin.Id;

        //    CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(coin.Algorithm);

        //    //MobileMiner currently only supports SHA and Scrypt
        //    //attempt to treat them as "Families" for now
        //    if ((algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA2) ||
        //        (algorithm.Family == CoinAlgorithm.AlgorithmFamily.SHA3))
        //        //SHA family algorithms grouped together
        //        miningStatistics.Algorithm = AlgorithmFullNames.SHA256;
        //    else
        //        //assume Scrypt for rest until MobileMiner supports more
        //        miningStatistics.Algorithm = AlgorithmFullNames.Scrypt;

        //    miningStatistics.PopulateFrom(deviceInformation);
        //}

        //private Action<List<MiningStatistics>> submitMiningStatisticsDelegate;

        //private void SubmitMiningStatistics(List<MiningStatistics> statisticsList)
        //{
        //    try
        //    {
        //        //submit statistics
        //        List<RemoteCommand> commands = MobileMiner.ApiContext.SubmitMiningStatistics(GetMobileMinerUrl(), MobileMinerApiKey,
        //            ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
        //            statisticsList, ApplicationConfiguration.MobileMinerRemoteCommands);

        //        //process commands
        //        if (ApplicationConfiguration.MobileMinerRemoteCommands)
        //        {
        //            //if we aren't mining we need another fetch for this machine
        //            if (!statisticsList.Any(sl => sl.MachineName.Equals(Environment.MachineName)))
        //            {
        //                commands.AddRange(MobileMiner.ApiContext.GetCommands(GetMobileMinerUrl(), MobileMinerApiKey,
        //                    ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
        //                    new List<string> { Environment.MachineName }));
        //            }

        //            Context.BeginInvoke((Action<List<RemoteCommand>>)ProcessRemoteCommands, new object[]{ commands });
        //        }

        //        mobileMinerSuccess = true;
        //    }
        //    catch (WebException ex)
        //    {
        //        //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
        //        HandleMobileMinerWebException(ex);
        //    }
        //}

        public void QueueMobileMinerNotification(string text, NotificationKind kind)
        {
            Notification notification = new Notification
            {
                NotificationText = text,
                MachineName = Environment.MachineName,
                NotificationKind = kind
            };
            queuedNotifications.Add(notification);
        }

        //private void SubmitMobileMinerNotifications()
        //{
        //    //are remote notifications enabled?
        //    if (!ApplicationConfiguration.MobileMinerPushNotifications)
        //        return;

        //    //is MobileMiner configured?
        //    if (!ApplicationConfiguration.IsMobileMinerConfigured())
        //        return;

        //    //do we have notifications to push?
        //    if (queuedNotifications.Count == 0)
        //        return;

        //    if (submitNotificationsDelegate == null)
        //        submitNotificationsDelegate = SubmitNotifications;

        //    submitNotificationsDelegate.BeginInvoke(submitNotificationsDelegate.EndInvoke, null);
        //}

        //private Action submitNotificationsDelegate;

        //private void SubmitNotifications()
        //{
        //    try
        //    {
        //        MobileMiner.ApiContext.SubmitNotifications(GetMobileMinerUrl(), MobileMinerApiKey,
        //                ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
        //                queuedNotifications);
        //        queuedNotifications.Clear();
        //    }
        //    catch (Exception ex)
        //    {
        //        if ((ex is WebException) || (ex is ArgumentException))
        //        {
        //            //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
        //            //could also be a json parsing error
        //            return;
        //        }
        //        throw;
        //    }
        //}

        //public void SubmitMobileMinerPools()
        //{
        //    //are remote commands enabled?
        //    if (!ApplicationConfiguration.MobileMinerRemoteCommands)
        //        return;

        //    //is MobileMiner configured?
        //    if (!ApplicationConfiguration.IsMobileMinerConfigured())
        //        return;

        //    Dictionary<string, List<string>> machinePools = new Dictionary<string, List<string>>();
        //    List<string> pools = EngineConfiguration.CoinConfigurations.Where(cc => cc.Enabled).Select(coin => coin.PoolGroup.Name).ToList();
        //    machinePools[Environment.MachineName] = pools;

        //    foreach (KeyValuePair<string, List<PoolInformation>> networkDevicePool in NetworkDevicePools)
        //    {
        //        //ipAddress:port
        //        string machinePath = networkDevicePool.Key;
        //        string machineName = LocalViewModel.GetFriendlyDeviceName(machinePath, machinePath);
        //        // poolInformationList may be null if an RPC API call timed out
        //        if (networkDevicePool.Value != null)
        //        {
        //            machinePools[machineName] = networkDevicePool.Value
        //                .Select(pi => pi.Url.DomainFromHost()).ToList();
        //        }
        //    }

        //    if (submitPoolsDelegate == null)
        //        submitPoolsDelegate = SubmitPools;

        //    submitPoolsDelegate.BeginInvoke(machinePools, submitPoolsDelegate.EndInvoke, null);
        //}

        //private Action<Dictionary<string, List<string>>> submitPoolsDelegate;

        //private void SubmitPools(Dictionary<string, List<string>> machinePools)
        //{
        //    try
        //    {
        //        MobileMiner.ApiContext.SubmitMachinePools(GetMobileMinerUrl(), MobileMinerApiKey,
        //                ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
        //                machinePools);
        //    }
        //    catch (Exception ex)
        //    {
        //        if ((ex is WebException) || (ex is ArgumentException))
        //        {
        //            //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
        //            //could also be a json parsing error
        //            return;
        //        }
        //        throw;
        //    }
        //}

        //private void HandleMobileMinerWebException(WebException webException)
        //{
        //    HttpWebResponse response = webException.Response as HttpWebResponse;
        //    if (response != null)
        //    {
        //        if (response.StatusCode == HttpStatusCode.Forbidden)
        //        {
        //            if (!mobileMinerSuccess)
        //            {
        //                ApplicationConfiguration.MobileMinerRemoteCommands = false;
        //                ApplicationConfiguration.SaveApplicationConfiguration();
        //                if (MobileMinerAuthFailed != null) MobileMinerAuthFailed(this, new EventArgs());
        //            }
        //        }
        //        else if (ApplicationConfiguration.ShowApiErrors)
        //        {
        //            ShowMobileMinerApiErrorNotification(webException);
        //        }
        //    }
        //}

        //private void ProcessRemoteCommands(List<RemoteCommand> commands)
        //{
        //    List<RemoteCommand> machineCommands = commands
        //        .GroupBy(c => c.Machine.Name)
        //        .Select(c => c.First())
        //        .ToList();

        //    if (machineCommands.Count > 0)
        //    {
        //        foreach (RemoteCommand command in machineCommands)
        //            ProcessRemoteCommand(command);
        //    }
        //}

        //private void ProcessRemoteCommand(RemoteCommand command)
        //{
        //    //check this before actually executing the command
        //    //point being, say for some reason it takes 2 minutes to restart mining
        //    //if we check for commands again in that time, we don't want to process it again
        //    if (processedCommandIds.Contains(command.Id))
        //        return;

        //    processedCommandIds.Add(command.Id);
        //    string commandText = command.CommandText;
        //    string machineName = command.Machine.Name;

        //    if (deleteRemoteCommandDelegate == null)
        //        deleteRemoteCommandDelegate = DeleteRemoteCommand;

        //    if (machineName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        //    {
        //        ProcessOwnRemoteCommand(commandText);
        //        deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
        //    }
        //    else
        //    {
        //        DeviceViewModel networkDevice = LocalViewModel.GetNetworkDeviceByFriendlyName(machineName);
        //        if (networkDevice != null)
        //        {
        //            ProcessNetworkDeviceRemoteCommand(commandText, networkDevice);
        //            deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
        //        }
        //    }
        //}

        private void ProcessNetworkDeviceRemoteCommand(string commandText, DeviceViewModel networkDevice)
        {
            Uri uri = new Uri("http://" + networkDevice.Path);
            ApiContext apiContext = new ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            string action = "accessing " + networkDevice.FriendlyName;

            try
            {
                if (commandText.StartsWith(RemoteCommandText.Switch, StringComparison.OrdinalIgnoreCase))
                {
                    action = "switching pools on " + networkDevice.FriendlyName;
                    string[] parts = commandText.Split('|');
                    string poolName = parts[1];

                    //we may not have the pool info cached yet / anymore
                    if (NetworkDevicePools.ContainsKey(networkDevice.Path))
                    {
                        List<PoolInformation> pools = NetworkDevicePools[networkDevice.Path];
                        int poolIndex = pools.FindIndex(pi => pi.Url.DomainFromHost().Equals(poolName, StringComparison.OrdinalIgnoreCase));

                        apiContext.SwitchPool(poolIndex);
                    }
                }
                else if (commandText.Equals(RemoteCommandText.Restart, StringComparison.OrdinalIgnoreCase))
                {
                    RestartNetworkDevice(networkDevice);
                }
                else if (commandText.Equals(RemoteCommandText.Stop, StringComparison.OrdinalIgnoreCase))
                {
                    StopNetworkDevice(networkDevice);
                }
                else if (commandText.Equals(RemoteCommandText.Start, StringComparison.OrdinalIgnoreCase))
                {
                    StartNetworkDevice(networkDevice);
                }
            }
            catch (SocketException ex)
            {
                PostNotification(String.Format("Error {0}: {1}", action, ex.Message), NotificationKind.Danger);
            }
        }

        private void ProcessOwnRemoteCommand(string commandText)
        {
            if (commandText.Equals(RemoteCommandText.Start, StringComparison.OrdinalIgnoreCase))
            {
                SaveChangesLocally(); //necessary to ensure device configurations exist for devices
                StartMiningLocally();
            }
            else if (commandText.Equals(RemoteCommandText.Stop, StringComparison.OrdinalIgnoreCase))
                StopMiningLocally();
            else if (commandText.Equals(RemoteCommandText.Restart, StringComparison.OrdinalIgnoreCase))
            {
                StopMiningLocally();
                SaveChangesLocally(); //necessary to ensure device configurations exist for devices
                StartMiningLocally();
            }
            else if (commandText.StartsWith(RemoteCommandText.Switch, StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = commandText.Split('|');
                string coinName = parts[1];
                Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Name.Equals(coinName));
                if (coinConfiguration != null)
                    SetAllDevicesToCoinLocally(coinConfiguration.PoolGroup.Id, true);
            }
        }

        private void ClearCachedNetworkDifficulties()
        {
            minerNetworkDifficulty.Clear();
        }

        //private Action<RemoteCommand> deleteRemoteCommandDelegate;

        //private void DeleteRemoteCommand(RemoteCommand command)
        //{
        //    try
        //    {
        //        MobileMiner.ApiContext.DeleteCommand(GetMobileMinerUrl(), MobileMinerApiKey,
        //                            ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
        //                            command.Machine.Name, command.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        if ((ex is WebException) || (ex is ArgumentException))
        //        {
        //            //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
        //            //could also be a json parsing error
        //            return;
        //        }
        //        throw;
        //    }
        //}

        private void ShowMobileMinerApiErrorNotification(WebException ex)
        {
            PostNotification(ex.Message,
                String.Format("Error accessing the MobileMiner API ({0})", (int)((HttpWebResponse)ex.Response).StatusCode), () =>
                {
                    Process.Start("http://mobileminercom");
                },
                NotificationKind.Warning, "");
        }
        #endregion

        #region RPC API
        private void RefreshPoolInfo()
        {
            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
            {
                List<PoolInformation> poolInformation = GetPoolInfoFromProcess(minerProcess);

                if (poolInformation == null) //handled failure getting API info
                {
                    minerProcess.MinerIsFrozen = true;
                    continue;
                }

                LocalViewModel.ApplyPoolInformationResponseModels(minerProcess.CoinSymbol, poolInformation);
            }
            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private List<PoolInformation> GetPoolInfoFromProcess(MinerProcess minerProcess)
        {
            ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            List<PoolInformation> poolInformation;
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

        private void PopulateSummaryInfoFromProcesses()
        {
            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
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

        private SummaryInformation GetSummaryInfoFromProcess(MinerProcess minerProcess)
        {
            ApiContext apiContext = minerProcess.ApiContext;

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            SummaryInformation summaryInformation;
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

        private void RefreshProcessDeviceStats(MinerProcess minerProcess)
        {
            ClearSuspectProcessFlags(minerProcess);

            List<DeviceInformation> deviceInformationList = GetDeviceInfoFromProcess(minerProcess);
            if (deviceInformationList == null) //handled failure getting API info
            {
                minerProcess.MinerIsFrozen = true;
                return;
            }
            
            //starting with bfgminer 3.7 we need the DEVDETAILS response to tie things from DEVS up with -d? details
            List<DeviceDetails> processDevices = GetProcessDeviceDetails(minerProcess, deviceInformationList);
            if (processDevices == null) //handled failure getting API info
            {
                minerProcess.MinerIsFrozen = true;
                return;
            }

            //clear accepted shares as we'll be summing that as well
            minerProcess.AcceptedShares = 0;

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

                    Device device = Devices[deviceIndex];
                    Coin coinConfiguration = CoinConfigurationForDevice(device);

                    if (minerProcess.Miner.LegacyApi && (coinConfiguration != null))
                        deviceInformation.WorkUtility = AdjustWorkUtilityForPoolMultipliers(deviceInformation.WorkUtility, coinConfiguration.PoolGroup.Algorithm);

                    DeviceViewModel deviceViewModel = LocalViewModel.ApplyDeviceInformationResponseModel(device, deviceInformation);
                    DeviceDetailsMapping[deviceViewModel] = deviceDetails;

                    if (coinConfiguration != null)
                    {
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

            LocalViewModel.ApplyDeviceDetailsResponseModels(minerProcess.MinerConfiguration.DeviceDescriptors, processDevices);
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
                !EngineConfiguration.XgminerConfiguration.DesktopMode)
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

        public double WorkUtilityToHashrate(double workUtility)
        {
            //this will be wrong for Scrypt until 3.10.1
            const double multiplier = 71582;
            double hashrate = workUtility * multiplier;
            return hashrate;
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

        //cache based on pool URI rather than coin symbol
        //the coin symbol may be guessed wrong for Network Devices
        //this can (and has) resulting in wildly inaccurate income estimates
        public double GetCachedNetworkDifficulty(string poolUri)
        {
            double result = 0.0;
            if (minerNetworkDifficulty.ContainsKey(poolUri))
                result = minerNetworkDifficulty[poolUri];
            return result;
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
                ApiContext apiContext = new ApiContext(port, ipAddress);
                difficulty = GetNetworkDifficultyFromMiner(apiContext);
                SetCachedNetworkDifficulty(poolUri, difficulty);
            }
        }

        private double GetNetworkDifficultyFromMiner(ApiContext apiContext)
        {
            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            NetworkCoinInformation coinInformation;

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
        #endregion

        #region MultiMiner Remoting
        public void SetupRemoting()
        {
            if (PerksConfiguration.EnableRemoting && PerksConfiguration.PerksEnabled && !RemotingEnabled)
                EnableRemoting();
            else if ((!PerksConfiguration.EnableRemoting || !PerksConfiguration.PerksEnabled) && RemotingEnabled)
                DisableRemoting();
        }

        private double GetLocalInstanceHashrate(string algorithm, bool includeNetworkDevices)
        {
            return GetTotalHashrate(LocalViewModel, algorithm, includeNetworkDevices);
        }

        public double GetVisibleInstanceHashrate(string algorithm, bool includeNetworkDevices)
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
                Broadcaster.Broadcast(machine);
            }
            catch (SocketException ex)
            {
                //e.g. no network connection on Linux
                ShowMultiMinerRemotingError(ex);
            }
        }

        public void PopulateLocalMachineHashrates(Remoting.Data.Transfer.Machine machine, bool includeNetworkDevices)
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

        public void DisableRemoting()
        {
            StopDiscovery();

            if (remotingServer != null)
                remotingServer.Shutdown();

            if (broadcastListener != null)
                broadcastListener.Stop();
            
            RemotingEnabled = false;

            if (RemoteInstancesUnregistered != null) RemoteInstancesUnregistered(this, new EventArgs());
            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private void PerformRequestedCommand(string clientAddress, string signature, Action action)
        {
            Instance remoteInstance = InstanceManager.Instances.SingleOrDefault(i => i.IpAddress.Equals(clientAddress));
            if (remoteInstance == null)
                return;

            string expectedSignature = GetReceivingSignature(remoteInstance);
            if (!expectedSignature.Equals(signature))
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    PostNotification("MultiMiner Remoting signature verification failed", NotificationKind.Danger);
                }), null);

                return;
            }

            action();
        }

        private void StartMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(StartMiningLocally), null);
            });
        }

        private void StopMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(StopMiningLocally), null);
            });
        }

        private void RestartMiningRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(RestartMiningLocally), null);
            });
        }

        private void ScanHardwareRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ScanHardwareLocally();
                }), null);
            });
        }

        private void SetAllDevicesToCoinRequested(object sender, string coinSymbol, bool disableStrategies, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    SetAllDevicesToCoinLocally(coinSymbol, disableStrategies);
                }), null);
            });
        }

        private void SetDeviceToCoinRequested(object sender, IEnumerable<DeviceDescriptor> devices, string coinSymbol, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    SetDevicesToCoinLocally(devices, coinSymbol);
                }), null);
            });
        }

        private void SaveChangesRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(SaveChangesLocally), null);
            });
        }

        private void CancelChangesRequested(object sender, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(CancelChangesLocally), null);
            });
        }

        private void ToggleDevicesRequested(object sender, IEnumerable<DeviceDescriptor> devices, bool enabled, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ToggleDevicesLocally(devices, enabled);
                }), null);
            });
        }

        private void ToggleDynamicIntensityRequested(object sender, bool enabled, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ToggleDynamicIntensityLocally(enabled);
                }), null);
            });
        }

        private void GetModelRequested(object sender, ModelEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ea.Devices = GetDeviceTransferObjects();
                ea.ConfiguredCoins = LocalViewModel.ConfiguredCoins.ToList();
                ea.Mining = MiningEngine.Mining;
                ea.DynamicIntensity = LocalViewModel.DynamicIntensity;
                ea.HasChanges = LocalViewModel.HasChanges;
            });
        }

        private List<Remoting.Data.Transfer.Device> GetDeviceTransferObjects()
        {
            List<Remoting.Data.Transfer.Device> newList = new List<Remoting.Data.Transfer.Device>();
            foreach (DeviceViewModel viewModel in LocalViewModel.Devices)
            {
                Remoting.Data.Transfer.Device dto = new Remoting.Data.Transfer.Device();
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

        private void GetConfigurationRequested(object sender, Remoting.ConfigurationEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ObjectCopier.CopyObject(ApplicationConfiguration.ToTransferObject(), ea.Application);
                ObjectCopier.CopyObject(EngineConfiguration.ToTransferObject(), ea.Engine);
                ObjectCopier.CopyObject(PathConfiguration, ea.Path);
                ObjectCopier.CopyObject(PerksConfiguration, ea.Perks);
            });
        }

        private void SetCoinConfigurationsRequested(object sender, Coin[] coinConfigurations, RemoteCommandEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                ObjectCopier.CopyObject(coinConfigurations, EngineConfiguration.CoinConfigurations);
                EngineConfiguration.SaveCoinConfigurations();

                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    LocalViewModel.ApplyCoinConfigurationModels(EngineConfiguration.CoinConfigurations);
                    LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                        EngineConfiguration.CoinConfigurations);
                    if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());
                }), null);
            });
        }

        private void SetConfigurationRequested(object sender, Remoting.ConfigurationEventArgs ea)
        {
            PerformRequestedCommand(ea.IpAddress, ea.Signature, () =>
            {
                string oldConfigPath = PathConfiguration.SharedConfigPath;

                if (ea.Application != null)
                {
                    ObjectCopier.CopyObject(ea.Application.ToModelObject(), ApplicationConfiguration);
                    ApplicationConfiguration.SaveApplicationConfiguration();
                }

                if (ea.Engine != null)
                {
                    ObjectCopier.CopyObject(ea.Engine.ToModelObject(), EngineConfiguration);
                    EngineConfiguration.SaveAllConfigurations();
                }

                if (ea.Path != null)
                {
                    ObjectCopier.CopyObject(ea.Path, PathConfiguration);
                    PathConfiguration.SavePathConfiguration();
                }

                if (ea.Perks != null)
                {
                    ObjectCopier.CopyObject(ea.Perks, PerksConfiguration);
                    PerksConfiguration.SavePerksConfiguration();
                }

                //save settings as the "shared" config path may have changed
                //these are settings not considered machine/device-specific
                //e.g. no device settings, no miner settings
                string newConfigPath = PathConfiguration.SharedConfigPath;
                MigrateSettingsToNewFolder(oldConfigPath, newConfigPath);

                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    LocalViewModel.ApplyCoinConfigurationModels(EngineConfiguration.CoinConfigurations);
                    LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                        EngineConfiguration.CoinConfigurations);
                    if (ConfigurationModified != null) ConfigurationModified(this, new EventArgs());
                }), null);
            });
        }

        private void MigrateSettingsToNewFolder(string oldConfigPath, string newConfigPath)
        {
            //if the shared config path changed, and there are already settings
            //in that path, load those settings (so they aren't overwritten)
            //idea being the user has shared settings already there they want to use
            if (!Equals(oldConfigPath, newConfigPath))
            {
                if (File.Exists(System.IO.Path.Combine(newConfigPath, System.IO.Path.GetFileName(ApplicationConfiguration.ApplicationConfigurationFileName()))))
                    ApplicationConfiguration.LoadApplicationConfiguration(newConfigPath);
                if (File.Exists(System.IO.Path.Combine(newConfigPath, System.IO.Path.GetFileName(PerksConfiguration.PerksConfigurationFileName()))))
                    PerksConfiguration.LoadPerksConfiguration(newConfigPath);
                if (File.Exists(System.IO.Path.Combine(newConfigPath, System.IO.Path.GetFileName(EngineConfiguration.CoinConfigurationsFileName()))))
                    EngineConfiguration.LoadCoinConfigurations(newConfigPath);
                if (File.Exists(System.IO.Path.Combine(newConfigPath, System.IO.Path.GetFileName(EngineConfiguration.StrategyConfigurationsFileName()))))
                    EngineConfiguration.LoadStrategyConfiguration(newConfigPath);
            }
        }

        private void UpgradeMultiMinerRequested(object sender, RemoteCommandEventArgs ea)
        {
            string installedVersion, availableVersion;
            bool updatesAvailable = MultiMinerHasUpdates(out availableVersion, out installedVersion);
            if (!updatesAvailable)
                return;

            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                bool wasMining = MiningEngine.Mining;

                if (wasMining)
                    StopMiningLocally();

                //this will restart the app
                InstallMultiMinerLocally();
            }), null);
        }

        private void UpgradeBackendMinerRequested(object sender, RemoteCommandEventArgs ea)
        {
            string installedVersion, availableVersion;
            bool updatesAvailable = BackendMinerHasUpdates(out availableVersion, out installedVersion);
            if (!updatesAvailable)
                return;

            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                bool wasMining = MiningEngine.Mining;

                if (wasMining)
                    StopMiningLocally();

                InstallBackendMinerLocally(MinerFactory.Instance.GetDefaultMiner());

                //only start mining if we stopped mining
                if (wasMining)
                    StartMiningLocally();
            }), null);
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

            if (availableVersion.VersionIsGreater(installedVersion))
                return true;

            return false;
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
                workGroupName = LocalNetwork.GetWorkGroupName();

            //start Broadcast Listener before Discovery so we can
            //get initial info (hashrates) sent by other instances
            broadcastListener = new Remoting.Broadcast.Listener();
            broadcastListener.PacketReceived += HandlePacketReceived;
            broadcastListener.Listen();

            SetupDiscovery();

            remotingServer = new RemotingServer();
            remotingServer.Startup();

            if (DataModified != null) DataModified(this, new EventArgs());

            RemotingEnabled = true;
        }

        private void HandlePacketReceived(object sender, PacketReceivedArgs ea)
        {
            Type type = typeof(Remoting.Data.Transfer.Machine);
            if (ea.Packet.Descriptor.Equals(type.FullName))
            {
                Remoting.Data.Transfer.Machine dto = JsonConvert.DeserializeObject<Remoting.Data.Transfer.Machine>(ea.Packet.Payload);

                if ((InstanceManager.ThisPCInstance != null) &&
                    (InstanceManager.ThisPCInstance.IpAddress.Equals(ea.IpAddress)))
                    //don't process packets broadcast by This PC
                    //for instance we don't broadcast out hashrate for Network Devices and
                    //so don't want to process the packet
                    return;

                //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                if (Context == null) return;

                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    if (RemoteInstanceModified != null)
                        RemoteInstanceModified(this, new RemotingEventArgs
                        {
                            IpAddress = ea.IpAddress,
                            Machine = dto
                        });
                }), null);
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
                Discovery.Broadcaster.Broadcast(Verbs.Online, fingerprint);
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
            {
                discoveryListener.Stop();

                //broadcast after so we don't needlessly process our own message
                try
                {
                    Discovery.Broadcaster.Broadcast(Verbs.Offline, fingerprint);
                }
                catch (SocketException ex)
                {
                    //e.g. no network connection on Linux
                    ShowMultiMinerRemotingError(ex);
                }
            }
        }

        private void HandleInstanceOnline(object sender, InstanceChangedArgs ea)
        {
            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
            if (Context == null) return;

            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                if (RemoteInstanceRegistered != null) RemoteInstanceRegistered(this, ea);
                if (DataModified != null) DataModified(this, new EventArgs());
            }), null);

            //send our hashrate back to the machine that is now Online
            if (!ea.Instance.MachineName.Equals(Environment.MachineName))
                SendHashrate(ea.Instance.IpAddress);
        }

        private void SendHashrate(string ipAddress)
        {
            Remoting.Data.Transfer.Machine machine = new Remoting.Data.Transfer.Machine();
            PopulateLocalMachineHashrates(machine, false);
            Sender.Send(IPAddress.Parse(ipAddress), machine);
        }

        private void HandleInstanceOffline(object sender, InstanceChangedArgs ea)
        {
            //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
            if (Context == null) return;

            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                if (RemoteInstanceUnregistered != null) RemoteInstanceUnregistered(this, ea);
                if (DataModified != null) DataModified(this, new EventArgs());
            }), null);
        }
        
        public void SelectRemoteInstance(Instance instance)
        {
            bool isThisPc = instance.MachineName.Equals(Environment.MachineName);

            if (!isThisPc)
                GetRemoteApplicationModels(instance);

            //don't set flags until remote VM is fetched
            SelectedRemoteInstance = isThisPc ? null : instance;

            if (DataModified != null) DataModified(this, new EventArgs());

            SetHasChanges(GetViewModelToView().HasChanges);
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

        private void GetRemoteApplicationModels(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                Remoting.Data.Transfer.Device[] devices;
                PoolGroup[] configurations;
                bool mining, hasChanges, dynamicIntensity;

                //set some safe defaults in case the call fails
                RemoteViewModel = new MinerFormViewModel();

                service.GetApplicationModels(GetSendingSignature(instance), out devices, out configurations, out mining, out hasChanges, out dynamicIntensity);

                RemoteInstanceMining = mining;
                RemoteViewModel.HasChanges = hasChanges;
                RemoteViewModel.DynamicIntensity = dynamicIntensity;
                SaveDeviceTransferObjects(devices);
                SaveCoinTransferObjects(configurations);
            });
        }

        private void GetRemoteApplicationConfiguration(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
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
            RemoteViewModel.ConfiguredCoins = configurations.ToList();
        }

        private void ShowMultiMinerRemotingError(SystemException ex)
        {
            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                const string message = "MultiMiner Remoting communication failed";
                PostNotification(message, () =>
                {
                    MessageBoxShow(ex.Message, "Error", PromptButtons.OK, PromptIcon.Error);
                }, NotificationKind.Danger);
            }), null);
        }

        private const uint RemotingPepper = 4108157753;
        private string workGroupName;

        private static string GetStringHash(string text)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);

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
                fingerprint,
                destination.MachineName,
                destination.Fingerprint,
                RemotingPepper,
                workGroupName,
                PerksConfiguration.RemotingPassword);
            return GetStringHash(signature);
        }

        private string GetReceivingSignature(Instance source)
        {
            string signature = String.Format("{0}{1}{2}{3}{4}{5}{6}",
                source.MachineName,
                source.Fingerprint,
                Environment.MachineName,
                fingerprint,
                RemotingPepper,
                workGroupName,
                PerksConfiguration.RemotingPassword);
            return GetStringHash(signature);
        }

        private void StartMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.StartMining(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void SaveChangesRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.SaveChanges(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void CancelChangesRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.CancelChanges(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void UpdateViewFromRemoteInTheFuture(int seconds)
        {
            timer = new System.Threading.Timer(
                                state =>
                                {
                                    Context.BeginInvoke((Action)(() =>
                                    {
                                        //code to update UI
                                        UpdateLocalViewFromRemoteInstance();
                                        timer.Dispose();
                                    }), null);
                                }
                                , null // no state required
                                , TimeSpan.FromSeconds(seconds) // Do it in x seconds
                                , TimeSpan.FromMilliseconds(-1)); // don't repeat
        }

        private void UpdateLocalViewFromRemoteInstance()
        {
            if (!RemotingEnabled)
                return;

            if (SelectedRemoteInstance == null)
                return;

            GetRemoteApplicationModels(SelectedRemoteInstance);
            SetHasChanges(RemoteViewModel.HasChanges);

            if (DataModified != null) DataModified(this, new EventArgs());
        }

        private System.Threading.Timer timer;
        private void StopMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.StopMining(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void RestartMiningRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.RestartMining(GetSendingSignature(instance));
            });
        }

        public void ScanHardwareRemotely(Instance instance)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.ScanHardware(GetSendingSignature(instance));
                UpdateViewFromRemoteInTheFuture(5);
            });
        }

        private void SetAllDevicesToCoinRemotely(Instance instance, string coinSymbol, bool disableStrategies)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.SetAllDevicesToCoin(GetSendingSignature(instance), coinSymbol, disableStrategies);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void SetDevicesToCoinRemotely(Instance instance, IEnumerable<DeviceDescriptor> devices, string coinName)
        {
            PerformRemoteCommand(instance, service =>
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
            PerformRemoteCommand(instance, service =>
            {
                List<DeviceDescriptor> descriptors = CloneDescriptors(devices);
                service.ToggleDevices(GetSendingSignature(instance), descriptors.ToArray(), enabled);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void ToggleDynamicIntensityRemotely(Instance instance, bool enabled)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.ToggleDynamicIntensity(GetSendingSignature(instance), enabled);
                UpdateViewFromRemoteInTheFuture(2);
            });
        }

        private void SetCoinConfigurationOnAllRigs(Coin[] coinConfigurations)
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = InstanceManager.Instances.Where(i => i != InstanceManager.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                Instance closedInstance = instance;
                PerformRemoteCommand(instance, service =>
                {
                    service.SetCoinConfigurations(GetSendingSignature(closedInstance), coinConfigurations);
                });
            }
        }

        private void SetConfigurationRemotely(
            Instance instance,
            Remoting.Data.Transfer.Configuration.Application application,
            Remoting.Data.Transfer.Configuration.Engine engine,
            Path path,
            Remoting.Data.Transfer.Configuration.Perks perks)
        {
            PerformRemoteCommand(instance, service =>
            {
                service.SetApplicationConfiguration(
                    GetSendingSignature(instance),
                    application,
                    engine,
                    path,
                    perks);
            });
        }

        public void SetDevicesToCoin(List<DeviceDescriptor> devices, string coinName)
        {
            if (SelectedRemoteInstance == null)
                SetDevicesToCoinLocally(devices, coinName);
            else
                SetDevicesToCoinRemotely(SelectedRemoteInstance, devices, coinName);
        }

        private void SaveDeviceTransferObjects(IEnumerable<Remoting.Data.Transfer.Device> devices)
        {
            RemoteViewModel.Devices.Clear();
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
                RemoteViewModel.Devices.Add(viewModel);
            }
        }

        private void InstallBackendMinerRemotely()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = InstanceManager.Instances.Where(i => i != InstanceManager.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                Instance closedInstance = instance;
                PerformRemoteCommand(instance, service =>
                {
                    service.UpgradeBackendMiner(GetSendingSignature(closedInstance));
                });
            }
        }

        private void InstallMultiMinerRemotely()
        {
            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<Instance> instancesCopy = InstanceManager.Instances.Where(i => i != InstanceManager.ThisPCInstance).ToList();
            foreach (Instance instance in instancesCopy)
            {
                Instance closedInstance = instance;
                PerformRemoteCommand(instance, service =>
                {
                    service.UpgradeMultiMiner(GetSendingSignature(closedInstance));
                });
            }
        }

        private void ConfigureSettingsRemotely()
        {
            Application workingApplicationConfiguration = new Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();
            Paths workingPathConfiguration = new Paths();
            Perks workingPerksConfiguration = new Perks();

            GetRemoteApplicationConfiguration(SelectedRemoteInstance);

            ObjectCopier.CopyObject(remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(remoteEngineConfig.ToModelObject(), workingEngineConfiguration);
            ObjectCopier.CopyObject(remotePathConfig, workingPathConfiguration);
            ObjectCopier.CopyObject(remotePerksConfig, workingPerksConfiguration);

            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = workingApplicationConfiguration,
                Engine = workingEngineConfiguration,
                Paths = workingPathConfiguration,
                Perks = workingPerksConfiguration
            };
            if (OnConfigureSettings != null) OnConfigureSettings(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), remoteEngineConfig);
                ObjectCopier.CopyObject(workingPathConfiguration, remotePathConfig);
                ObjectCopier.CopyObject(workingPerksConfiguration, remotePerksConfig);

                SetConfigurationRemotely(SelectedRemoteInstance, remoteApplicationConfig, remoteEngineConfig, remotePathConfig, null);
            }
        }

        private void ConfigurePerksRemotely()
        {
            Perks workingPerksConfiguration = new Perks();

            GetRemoteApplicationConfiguration(SelectedRemoteInstance);

            ObjectCopier.CopyObject(remotePerksConfig, workingPerksConfiguration);

            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Perks = workingPerksConfiguration
            };
            if (OnConfigurePerks != null) OnConfigurePerks(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                ObjectCopier.CopyObject(workingPerksConfiguration, remotePerksConfig);
                SetConfigurationRemotely(SelectedRemoteInstance, null, null, null, remotePerksConfig);
            }
        }

        private void ConfigureStrategiesRemotely()
        {
            Application workingApplicationConfiguration = new Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();

            GetRemoteApplicationConfiguration(SelectedRemoteInstance);

            ObjectCopier.CopyObject(remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(remoteEngineConfig.ToModelObject(), workingEngineConfiguration);

            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = workingApplicationConfiguration,
                Engine = workingEngineConfiguration
            };
            if (OnConfigureStrategies != null) OnConfigureStrategies(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), remoteEngineConfig);

                SetConfigurationRemotely(SelectedRemoteInstance, remoteApplicationConfig, remoteEngineConfig, null, null);
            }
        }

        private void ConfigurePoolsRemotely()
        {
            Application workingApplicationConfiguration = new Application();
            Engine.Data.Configuration.Engine workingEngineConfiguration = new Engine.Data.Configuration.Engine();

            GetRemoteApplicationConfiguration(SelectedRemoteInstance);

            ObjectCopier.CopyObject(remoteApplicationConfig.ToModelObject(), workingApplicationConfiguration);
            ObjectCopier.CopyObject(remoteEngineConfig.ToModelObject(), workingEngineConfiguration);

            ConfigurationEventArgs eventArgs = new ConfigurationEventArgs
            {
                Application = workingApplicationConfiguration,
                Engine = workingEngineConfiguration,
                Perks = PerksConfiguration
            };
            if (OnConfigurePools != null) OnConfigurePools(this, eventArgs);

            if (eventArgs.ConfigurationModified)
            {
                ObjectCopier.CopyObject(workingApplicationConfiguration.ToTransferObject(), remoteApplicationConfig);
                ObjectCopier.CopyObject(workingEngineConfiguration.ToTransferObject(), remoteEngineConfig);
                SetConfigurationRemotely(SelectedRemoteInstance, remoteApplicationConfig, remoteEngineConfig, null, null);

                if (ApplicationConfiguration.SaveCoinsToAllMachines && PerksConfiguration.PerksEnabled && PerksConfiguration.EnableRemoting)
                    SetCoinConfigurationOnAllRigs(remoteEngineConfig.CoinConfigurations);
            }
        }
        #endregion

        #region View / ViewModel behavior
        public void SaveChanges()
        {
            if (SelectedRemoteInstance == null)
                SaveChangesLocally();
            else
                SaveChangesRemotely(SelectedRemoteInstance);
        }

        public MinerFormViewModel GetViewModelToView()
        {
            MinerFormViewModel viewModelToView = LocalViewModel;
            if (SelectedRemoteInstance != null)
                viewModelToView = RemoteViewModel;
            return viewModelToView;
        }
        #endregion

        #region Application logic
        private static bool MultiMinerHasUpdates(out string availableVersion, out string installedVersion)
        {
            availableVersion = String.Empty;
            installedVersion = String.Empty;

            try
            {
                availableVersion = MultiMinerInstaller.GetAvailableMinerVersion();
            }
            catch (WebException)
            {
                //downloads website is down
                return false;
            }

            if (String.IsNullOrEmpty(availableVersion))
                return false;

            installedVersion = MultiMinerInstaller.GetInstalledMinerVersion();

            if (availableVersion.VersionIsGreater(installedVersion))
                return true;

            return false;
        }

        private void InstallMultiMinerLocally()
        {
            if (ProgressStarted != null)
                ProgressStarted(this, new ProgressEventArgs
                {
                    Text = "Downloading and installing MultiMiner from " + MultiMinerInstaller.GetMinerDownloadRoot()
                });
            try
            {
                MultiMinerInstaller.InstallMiner(AppDomain.CurrentDomain.BaseDirectory);
            }
            finally
            {
                if (ProgressCompleted != null) ProgressCompleted(this, new EventArgs());
            }
        }

        public static bool MinerIsInstalled(MinerDescriptor miner)
        {
            string path = MinerPath.GetPathToInstalledMiner(miner);
            return File.Exists(path);
        }
        #endregion

        #region Application startup / setup
        //required for GPU mining
        public void SetGpuEnvironmentVariables()
        {
            if (ApplicationConfiguration.SetGpuEnvironmentVariables)
            {
                const string gpuMaxAllocPercent = "GPU_MAX_ALLOC_PERCENT";
                const string gpuUseSyncObjects = "GPU_USE_SYNC_OBJECTS";

                SetEnvironmentVariableIfNotSet(gpuMaxAllocPercent, "100");
                SetEnvironmentVariableIfNotSet(gpuUseSyncObjects, "1");
            }
        }

        private static void SetEnvironmentVariableIfNotSet(string name, string value)
        {
            string currentValue = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (String.IsNullOrEmpty(currentValue))
                Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
        }

        public void SetupMiningEngineEvents()
        {
            MiningEngine.LogProcessClose += LogProcessClose;
            MiningEngine.LogProcessLaunch += LogProcessLaunch;
            MiningEngine.ProcessLaunchFailed += ProcessLaunchFailed;
            MiningEngine.ProcessAuthenticationFailed += ProcessAuthenticationFailed;
        }

        public void LoadPreviousHistory()
        {
            const string logFileName = "MiningLog.json";
            string logDirectory = ApplicationPaths.AppDataPath();
            if (Directory.Exists(ApplicationConfiguration.LogFilePath))
                logDirectory = ApplicationConfiguration.LogFilePath;
            string logFilePath = System.IO.Path.Combine(logDirectory, logFileName);
            if (File.Exists(logFilePath))
            {
                try
                {
                    List<LogProcessCloseArgs> previousHistory = ObjectLogger.LoadLogFile<LogProcessCloseArgs>(logFilePath).ToList();
                    
                    //fix dates = they are serialized as UTC
                    previousHistory.ForEach((hist) =>
                    {
                        hist.StartDate = DateTime.SpecifyKind(hist.StartDate, DateTimeKind.Utc).ToLocalTime();
                        hist.EndDate = DateTime.SpecifyKind(hist.EndDate, DateTimeKind.Utc).ToLocalTime();
                    });

                    //only load MaxHistoryOnScreen
                    previousHistory.RemoveRange(0, Math.Max(0, previousHistory.Count - MaxLogEntriesOnScreen));

                    foreach (LogProcessCloseArgs logProcessCloseArgs in previousHistory)
                        LogCloseEntries.Add(logProcessCloseArgs);
                }
                catch (SystemException)
                {
                    //old MiningLog.json file - wrong format serialized
                    //MiningLog.json rolls over so we will eventually be able to
                    //load the previous log file
                    //seen as both ArgumentException and InvalidCastException - catching SystemException
                }
            }
        }

        public void CheckForDisownedMiners()
        {
            //to-do: detect disowned miners for all types of running miners

            if (DisownedMinersDetected())
            {
                PromptResult messageBoxResult = MessageBoxShow("MultiMiner has detected running miners that it does not own. Would you like to kill them?",
                    "Disowned Miners Detected", PromptButtons.YesNo, PromptIcon.Question);

                if (messageBoxResult == PromptResult.Yes)
                    KillDisownedMiners();
            }
        }

        private bool DisownedMinersDetected()
        {
            return MinerFactory.Instance.Miners.Any(miner => DisownedMinersDetected(miner.FileName));
        }

        private bool DisownedMinersDetected(string minerName)
        {
            List<Process> disownedMiners = Process.GetProcessesByName(minerName).ToList();

            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
                disownedMiners.Remove(minerProcess.Process);

            return disownedMiners.Count > 0;
        }

        private void KillDisownedMiners()
        {
            //declare a local to avoid a bug under Mono when enumerating with a Singleton
            List<MinerDescriptor> miners = MinerFactory.Instance.Miners;
            foreach (MinerDescriptor miner in miners)
                KillDisownedMiners(miner.FileName);
        }

        private void KillDisownedMiners(string fileName)
        {
            List<Process> disownedMiners = Process.GetProcessesByName(fileName).ToList();

            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
                disownedMiners.Remove(minerProcess.Process);

            foreach (Process disownedMiner in disownedMiners)
                MinerProcess.KillProcess(disownedMiner);
        }
        #endregion

        #region Mining event handlers
        private void LogProcessLaunch(object sender, LogLaunchArgs ea)
        {
            //remove then add so BindingList position is on latest entry
            while (LogLaunchEntries.Count > MaxLogEntriesOnScreen)
                LogLaunchEntries.RemoveAt(0);
            LogLaunchEntries.Add(ea);

            LogProcessLaunchToFile(ea);
        }

        private void LogProcessLaunchToFile(LogLaunchArgs ea)
        {
            const string logFileName = "ProcessLog.json";
            LogObjectToFile(ea, logFileName);
        }

        private const int MaxLogEntriesOnScreen = 1000;
        private void LogProcessClose(object sender, LogProcessCloseArgs ea)
        {
            //remove then add so BindingList position is on latest entry
            while (LogCloseEntries.Count > MaxLogEntriesOnScreen)
                LogCloseEntries.RemoveAt(0);
            LogCloseEntries.Add(ea);

            LogProcessCloseToFile(ea);
        }

        private void LogProcessCloseToFile(LogProcessCloseArgs ea)
        {
            const string logFileName = "MiningLog.json";
            //log an anonymous type so MinerConfiguration is ommitted
            LogObjectToFile(
                new
                {
                    ea.StartDate, ea.EndDate, ea.CoinName, ea.CoinSymbol, ea.StartPrice, ea.EndPrice, ea.AcceptedShares, ea.DeviceDescriptors
                }, logFileName);
        }

        public void LogNotificationToFile(string text)
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
            Context.BeginInvoke((Action)(() =>
            {
                if (EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
                {
                    string notificationReason;

                    int enabledConfigurationCount = EngineConfiguration.CoinConfigurations.Count(c => c.Enabled);

                    //only disable the configuration if there are others enabled - otherwise left idling
                    if (enabledConfigurationCount > 1)
                    {
                        //if auto mining is enabled, flag pools down in the coin configuration and display a notification
                        Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(config => config.PoolGroup.Name.Equals(ea.CoinName, StringComparison.OrdinalIgnoreCase));
                        if (coinConfiguration != null) coinConfiguration.PoolsDown = true;

                        EngineConfiguration.SaveCoinConfigurations();

                        //if no enabled configurations, stop mining
                        int enabledConfigurations = EngineConfiguration.CoinConfigurations.Count(config => config.Enabled && !config.PoolsDown);
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

                    PostNotification(notificationReason, ConfigurePoolsLocally, NotificationKind.Danger);
                }
                else
                {
                    if (!ApplicationConfiguration.RestartCrashedMiners)
                    {
                        //if we are not restarting miners, display a dialog
                        MessageBoxShow(ea.Reason, "Launching Miner Failed", PromptButtons.OK, PromptIcon.Error);
                    }
                    else
                    {
                        //just notify - relaunching option will take care of the rest
                        PostNotification(String.Format("All pools for {0} configuration are down", ea.CoinName), 
                            ConfigurePoolsLocally, NotificationKind.Danger);
                    }
                }
            }), null);
        }

        private void ProcessAuthenticationFailed(object sender, AuthenticationFailedArgs ea)
        {
            Context.BeginInvoke((Action)(() =>
            {
                //code to update UI
                PostNotification(ea.Reason, ConfigurePoolsLocally, NotificationKind.Danger);
            }), null);
        }
        #endregion

        #region Application logic
        public void CheckAndDownloadMiners()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                return; //can't auto download binaries on Linux

            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
            if (!MinerIsInstalled(miner) &&
                //may not have a Url for the miner if call to server failed
                !String.IsNullOrEmpty(miner.Url))
                InstallBackendMinerLocally(miner);
        }

        public void CheckForUpdates()
        {
            PlatformID concretePlatform = OSVersionPlatform.GetConcretePlatform();

            CheckForMultiMinerUpdates();

            //we cannot auto install miners on Unix (yet)
            if (ApplicationConfiguration.CheckForMinerUpdates && (concretePlatform != PlatformID.Unix))
                TryToCheckForMinerUpdates();
        }

        private void CheckForMultiMinerUpdates()
        {
            string availableVersion, installedVersion;

            bool updatesAvailable = MultiMinerHasUpdates(out availableVersion, out installedVersion);

            if (updatesAvailable)
                DisplayMultiMinerUpdateNotification(availableVersion, installedVersion);
        }

        private void DisplayMultiMinerUpdateNotification(string availableMinerVersion, string installedMinerVersion)
        {
            PostNotification(MultiMinerNotificationId.ToString(),
                String.Format("MultiMiner version {0} is available ({1} installed)",
                    availableMinerVersion, installedMinerVersion),
                () =>
                {
                    bool allRigs = ShouldUpdateAllRigs();

                    bool wasMining = MiningEngine.Mining;

                    if (wasMining)
                        StopMiningLocally();

                    //remote first as we'll be restarting
                    if (allRigs)
                        InstallMultiMinerRemotely();

                    //this will restart the app
                    InstallMultiMinerLocally();
                }, NotificationKind.Information, "http://releases.multiminerapp.com");
        }

        private bool ShouldUpdateAllRigs()
        {
            bool allRigs = false;
            if (RemotingEnabled && (InstanceManager.Instances.Count > 1))
            {
                PromptResult dialogResult = MessageBoxShow("Would you like to apply this update to all of your online rigs?",
                    "MultiMiner Remoting", PromptButtons.YesNo, PromptIcon.Question);
                if (dialogResult == PromptResult.Yes)
                    allRigs = true;
            }
            return allRigs;
        }

        private void TryToCheckForMinerUpdates()
        {
            try
            {
                CheckForBackendMinerUpdates();
            }
            catch (ArgumentException)
            {
                PostNotification(String.Format("Error checking for {0} updates",
                    MinerFactory.Instance.GetDefaultMiner().Name), NotificationKind.Warning);
            }
        }

        private void CheckForBackendMinerUpdates()
        {
            string availableVersion, installedVersion;

            bool updatesAvailable = BackendMinerHasUpdates(out availableVersion, out installedVersion);

            if (updatesAvailable)
                DisplayBackendMinerUpdateNotification(availableVersion, installedVersion);
        }

        private void DisplayBackendMinerUpdateNotification(string availableMinerVersion, string installedMinerVersion)
        {
            const int notificationId = BfgMinerNotificationId;

            const string informationUrl = "https://github.com/luke-jr/bfgminer/blob/bfgminer/NEWS";

            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner();
            string minerName = miner.Name;

            PostNotification(notificationId.ToString(),
                String.Format("{0} version {1} is available ({2} installed)",
                    minerName, availableMinerVersion, installedMinerVersion),
                () =>
                {
                    bool allRigs = ShouldUpdateAllRigs();

                    bool wasMining = MiningEngine.Mining;

                    if (wasMining)
                        StopMiningLocally();

                    if (allRigs)
                        InstallBackendMinerRemotely();

                    InstallBackendMinerLocally(miner);

                    //only start mining if we stopped mining
                    if (wasMining)
                        StartMiningLocally();
                }, NotificationKind.Information, informationUrl);
        }

        public void UpdateBackendMinerAvailability()
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
                //declare a local to avoid a bug under Mono when enumerating with a Singleton
                List<MinerDescriptor> miners = MinerFactory.Instance.Miners;
                foreach (MinerDescriptor minerDescriptor in miners)
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

        private void ShowMinerCheckErrorNotification(WebException ex)
        {
            string notificationText = "Error checking for backend miner availability";
            //ensure Response is HttpWebResponse to avoid NullReferenceException
            if (ex.Response is HttpWebResponse)
                notificationText = String.Format("{1} ({0})", (int)((HttpWebResponse)ex.Response).StatusCode, notificationText);

            PostNotification(ex.Message,
                notificationText, () =>
                {
                    MessageBoxShow(ex.Message, "Error", PromptButtons.OK, PromptIcon.Error);
                },
                NotificationKind.Warning, "");
        }
        #endregion

        #region Timers
        public void SetupRestartTimer()
        {
            //if enabled, we want to restart it so this can be used when we start mining
            restartTimer.Enabled = false;
            restartTimer.Interval = ApplicationConfiguration.ScheduledRestartMiningInterval.ToMinutes() * 60 * 1000; //dynamic
            restartTimer.Enabled = ApplicationConfiguration.ScheduledRestartMining;
        }

        public void SetupNetworkRestartTimer()
        {
            networkRestartTimer.Interval = ApplicationConfiguration.ScheduledRestartNetworkDevicesInterval.ToMinutes() * 60 * 1000; //dynamic
            networkRestartTimer.Enabled = ApplicationConfiguration.ScheduledRestartNetworkDevices;
        }

        private void SetupNetworkScanTimer()
        {
            networkScanTimer.Interval = 15 * 60 * 1000;
            networkScanTimer.Enabled = false;
        }

        private void networkRestartTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (ApplicationConfiguration.ScheduledRebootNetworkDevices)
                RebootAllNetworkDevicesAsync();
            else
                RestartAllNetworkDevicesAsync();
        }

        private void networkScanTimer_Tick(object sender, ElapsedEventArgs e)
        {
            ClearCachedNetworkDifficulties();

            if (ApplicationConfiguration.NetworkDeviceDetection)
                RunNetworkDeviceScan();
        }

        private void RebootAllNetworkDevicesAsync()
        {
            Action asyncAction = RebootAllNetworkDevices;
            asyncAction.BeginInvoke(asyncAction.EndInvoke, null);
        }

        private void RebootAllNetworkDevices()
        {
            //there may be more than once miner process per IP
            //we only want to send the reboot command once per-IP
            List<DeviceViewModel> distinctNetworkDevices = LocalViewModel.Devices
                .Where(d => d.Kind == DeviceKind.NET)
                .GroupBy(d => d.Path.Split(':').First())
                .Select(g => g.First())
                .ToList();

            distinctNetworkDevices.ForEach(d => RebootNetworkDevice(d, true));
        }

        private void RestartAllNetworkDevicesAsync()
        {
            Action asyncAction = RestartAllNetworkDevices;
            asyncAction.BeginInvoke(asyncAction.EndInvoke, null);
        }

        private void RestartAllNetworkDevices()
        {
            LocalViewModel.Devices.Where(d => d.Kind == DeviceKind.NET).ToList().ForEach((d) =>
            {
                try
                {
                    RestartNetworkDevice(d);
                }
                catch (SocketException)
                {
                    //device is not online / RPC API is down
                }
            });
        }
        
        private void restartTimer_Tick(object sender, EventArgs e)
        {
            RestartMiningLocallyIfMining();
        }

        public void SetupCoinStatsTimer()
        {
            Application.TimerInterval timerInterval = ApplicationConfiguration.StrategyCheckInterval;

            int coinStatsMinutes = timerInterval.ToMinutes();

            coinStatsTimer.Enabled = false;

            CoinStatsCountdownMinutes = coinStatsMinutes;
            coinStatsTimer.Interval = coinStatsMinutes * 60 * 1000; //dynamic

            coinStatsTimer.Enabled = true;
        }

        private void coinStatsTimer_Tick(object sender, ElapsedEventArgs e)
        {
            RefreshCoinStatsAsync();

            CoinStatsCountdownMinutes = (int)coinStatsTimer.Interval / 1000 / 60;
        }

        public void SetupCoalescedTimers()
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
            timers.CreateTimer(Timers.ThirtyMinuteInterval, thirtyMinuteTimer_Tick);
            timers.CreateTimer(Timers.OneSecondInterval, oneSecondTimer_Tick);
            timers.CreateTimer(Timers.TwelveHourInterval, twelveHourTimer_Tick);
        }
        #endregion

#if DEBUG
        private void debugOneSecondTimer_Tick(object sender, ElapsedEventArgs e)
        {
            //updates in order to try to reproduce threading issues
            //if (DataModified != null) DataModified(this, new EventArgs());
        }
#endif

        private void oneHourTimer_Tick(object sender, ElapsedEventArgs e)
        {
            ClearPoolsFlaggedDown();
        }

        private void oneMinuteTimer_Tick(object sender, ElapsedEventArgs e)
        {
            ////if we do this with the Settings dialog open the user may have partially entered credentials
            //SubmitMobileMinerStatistics();

            //only broadcast if there are other instances (not just us)
            if (RemotingEnabled && PerksConfiguration.EnableRemoting && (InstanceManager.Instances.Count > 1))
            {
                //broadcast 0 (e.g. even if not mining)
                BroadcastHashrate();
            }

            //coin stats countdown
            CoinStatsCountdownMinutes--;
            if (DataModified != null) DataModified(this, new EventArgs());

            PopulateSummaryInfoFromProcesses();

#if DEBUG
            //SubmitMobileMinerNotifications();
#endif

            //restart suspect network devices
            if (ApplicationConfiguration.RestartCrashedMiners)
                RestartSuspectNetworkDevices();
        }

        private void twelveHourTimer_Tick(object sender, ElapsedEventArgs e)
        {
            UpdateBackendMinerAvailability();
            CheckForUpdates();
        }

        private void fiveMinuteTimer_Tick(object sender, ElapsedEventArgs e)
        {
            ////submit queued notifications to MobileMiner
            //SubmitMobileMinerNotifications();

            ////submit pools to MobileMiner before clearing cache
            //SubmitMobileMinerPools();

            //clear cached information for Network Devices
            //(so we pick up changes)
            NetworkDevicePools.Clear();
            networkDeviceStatistics.Clear();
            networkDeviceVersions.Clear();
        }

        private void thirtySecondTimer_Tick(object sender, ElapsedEventArgs e)
        {
            UpdateLocalViewFromRemoteInstance();

            if (ApplicationConfiguration.RestartCrashedMiners && MiningEngine.RelaunchCrashedMiners())
            {
                //clear any details stored correlated to processes - they could all be invalid after this
                processDeviceDetails.Clear();
                SaveOwnedProcesses();

                if (DataModified != null) DataModified(this, new EventArgs());
            }

            if (MiningEngine.Mining)
                RefreshPoolInfo();
        }

        private void fifteenSecondTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (ApplicationConfiguration.NetworkDeviceDetection)
                RefreshNetworkDeviceStatsAsync();
        }

        private void tenSecondTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (MiningEngine.Mining)
            {
                double timerInterval = ((System.Timers.Timer)sender).Interval;
                CheckIdleTimeForDynamicIntensity(timerInterval);
                RefreshAllDeviceStats();
            }
        }

        private void thirtyMinuteTimer_Tick(object sender, ElapsedEventArgs e)
        {
            RefreshExchangeRates();
        }

        private void oneSecondTimer_Tick(object sender, ElapsedEventArgs e)
        {
            CheckMiningOnStartupStatus();
        }
    }
}
