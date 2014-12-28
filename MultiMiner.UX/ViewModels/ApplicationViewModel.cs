using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Engine.Installers;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.MobileMiner.Data;
using MultiMiner.MultipoolApi.Data;
using MultiMiner.Services;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Discovery;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        #region Constants
        public const int BFGMinerNotificationId = 100;
        public const int MultiMinerNotificationId = 102;
        #endregion

        #region Events
        //delegate declarations
        public delegate void NotificationEventHandler(object sender, NotificationEventArgs ea);
        public delegate void CredentialsEventHandler(object sender, CredentialsEventArgs ea);
        public delegate void ProgressStartEventHandler(object sender, ProgressEventArgs ea);

        //event declarations        
        public event NotificationEventHandler NotificationReceived;
        public event NotificationEventHandler NotificationDismissed;
        public event CredentialsEventHandler CredentialsRequested;
        public event EventHandler MobileMinerAuthFailed;
        public event EventHandler DataModified;
        public event ProgressStartEventHandler ProgressStarted;
        public event EventHandler ProgressCompleted;
        #endregion

        #region Fields
        //configuration
        public readonly Engine.Data.Configuration.Engine EngineConfiguration = new Engine.Data.Configuration.Engine();
        public readonly Data.Configuration.Application ApplicationConfiguration = new Data.Configuration.Application();
        public readonly Paths PathConfiguration = new Paths();
        public readonly Perks PerksConfiguration = new Perks();
        public readonly NetworkDevices NetworkDevicesConfiguration = new NetworkDevices();
        public readonly Metadata MetadataConfiguration = new Metadata();

        //Coin API contexts
        private IApiContext coinChooseApiContext;
        private IApiContext coinWarzApiContext;
        private IApiContext whatMineApiContext;
        private IApiContext successfulApiContext;

        //Coin API information
        private List<CoinInformation> coinApiInformation = new List<CoinInformation>();

        //Exchange API information
        private IEnumerable<ExchangeInformation> sellPrices;

        //view models
        public readonly MinerFormViewModel LocalViewModel = new MinerFormViewModel();

        //hardware information
        private List<Xgminer.Data.Device> devices;

        //RPC API information
        public readonly Dictionary<MinerProcess, List<DeviceDetails>> ProcessDeviceDetails = new Dictionary<MinerProcess, List<DeviceDetails>>();
        private Dictionary<string, double> minerNetworkDifficulty = new Dictionary<string, double>();
        public readonly Dictionary<DeviceViewModel, string> LastDevicePoolMapping = new Dictionary<DeviceViewModel, string>();
        public readonly Dictionary<string, List<PoolInformation>> NetworkDevicePools = new Dictionary<string, List<PoolInformation>>();
        public readonly Dictionary<string, List<MinerStatistics>> NetworkDeviceStatistics = new Dictionary<string, List<MinerStatistics>>();

        //mining logic
        private readonly MiningEngine miningEngine = new MiningEngine();
        private int startupMiningCountdownSeconds = 0;
        private List<PoolGroup> knownCoins = new List<PoolGroup>();

        //MobileMiner API information
        private readonly List<int> processedCommandIds = new List<int>();
        private readonly List<MobileMiner.Data.Notification> queuedNotifications = new List<MobileMiner.Data.Notification>();

        //data sources
        private readonly List<ApiLogEntry> apiLogEntries = new List<ApiLogEntry>();
        public readonly BindingSource ApiLogEntryBindingSource = new BindingSource();
        #endregion

        #region Properties
        public IApiContext SuccessfulApiContext { get { return successfulApiContext; } }
        public List<CoinInformation> CoinApiInformation { get { return coinApiInformation; } }
        public List<PoolGroup> KnownCoins { get { return knownCoins; } }
        public IEnumerable<ExchangeInformation> SellPrices { get { return sellPrices; } }
        public MiningEngine MiningEngine { get { return miningEngine; } }
        public List<Xgminer.Data.Device> Devices { get { return devices; } }
        public int StartupMiningCountdownSeconds { get { return startupMiningCountdownSeconds; } }

        //view models
        public MinerFormViewModel RemoteViewModel { get; set; } = new MinerFormViewModel();
        
        //threading
        public Control Context { get; set; }

        //currently mining information
        public List<Engine.Data.Configuration.Device> MiningDeviceConfigurations { get; set; }
        public List<Engine.Data.Configuration.Coin> MiningCoinConfigurations { get; set; }
        #endregion

        #region Constructor
        public ApplicationViewModel()
        {
            ApiLogEntryBindingSource.DataSource = apiLogEntries;
        }
        #endregion

        #region Coin API
        public void SetupCoinApi()
        {
            this.coinWarzApiContext = new CoinWarz.ApiContext(ApplicationConfiguration.CoinWarzApiKey);
            this.coinChooseApiContext = new CoinChoose.ApiContext();
            this.whatMineApiContext = new WhatMine.ApiContext(ApplicationConfiguration.WhatMineApiKey);
        }

        public void RefreshAllCoinStats()
        {
            RefreshSingleCoinStats();
            RefreshMultiCoinStats();
        }

        public IApiContext GetEffectiveApiContext()
        {
            if (this.successfulApiContext != null)
                return this.successfulApiContext;
            else if (this.ApplicationConfiguration.UseCoinWarzApi)
                return this.coinWarzApiContext;
            else if (this.ApplicationConfiguration.UseWhatMineApi)
                return this.whatMineApiContext;
            else
                return this.coinChooseApiContext;
        }

        private void RefreshSingleCoinStats()
        {
            //always load known coins from file
            //CoinChoose may not show coins it once did if there are no orders
            LoadKnownCoinsFromFile();

            IApiContext preferredApiContext, backupApiContext;
            if (ApplicationConfiguration.UseCoinWarzApi)
            {
                preferredApiContext = this.coinWarzApiContext;
                backupApiContext = this.coinChooseApiContext;
            }
            else if (ApplicationConfiguration.UseWhatMineApi)
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
                ((backupApiContext == this.coinChooseApiContext) || !String.IsNullOrEmpty(ApplicationConfiguration.CoinWarzApiKey)))
                success = ApplyCoinInformationToViewModel(backupApiContext);

            FixCoinSymbolDiscrepencies();
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
            bool initialLoad = !KnownCoins.Any(kc => kc.Id.Contains(Prefix));
            bool miningNiceHash = EngineConfiguration.CoinConfigurations.Any(cc => cc.PoolGroup.Id.Contains(Prefix) && cc.Enabled);
            if (!initialLoad && !miningNiceHash)
            {
                return;
            }

            IEnumerable<MultipoolInformation> multipoolInformation = GetNiceHashInformation();

            //we're offline or the API is offline
            if (multipoolInformation == null)
                return;

            CoinApiInformation.AddRange(multipoolInformation
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
                MessageBox.Show(String.Format("{0}: {1}", summary, details), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }, ToolTipIcon.Warning, apiUrl);
        }
        private void ShowMultipoolApiErrorNotification(MultipoolApi.IApiContext apiContext, Exception ex)
        {
            string apiUrl = apiContext.GetApiUrl();
            string apiName = apiContext.GetApiName();

            string summary = String.Format("Error parsing the {0} JSON API", apiName);
            string details = ex.Message;

            PostNotification(ex.Message, summary, () =>
            {
                MessageBox.Show(String.Format("{0}: {1}", summary, details), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }, ToolTipIcon.Warning, apiUrl);
        }
        #endregion

        #region Settings logic
        private void LoadKnownCoinsFromFile()
        {
            string knownCoinsFileName = KnownCoinsFileName();
            if (File.Exists(knownCoinsFileName))
            {
                knownCoins = ConfigurationReaderWriter.ReadConfiguration<List<PoolGroup>>(knownCoinsFileName);
                RemoveBunkCoins(knownCoins);
            }
        }

        public void SaveKnownCoinsToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(knownCoins, KnownCoinsFileName());
        }

        private string KnownCoinsFileName()
        {
            string filePath;
            if (String.IsNullOrEmpty(PathConfiguration.SharedConfigPath))
                filePath = ApplicationPaths.AppDataPath();
            else
                filePath = PathConfiguration.SharedConfigPath;
            return Path.Combine(filePath, "KnownCoinsCache.xml");
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
            Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.Single();

            for (int i = 0; i < devices.Count; i++)
            {
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device()
                {
                    CoinSymbol = coinConfiguration.PoolGroup.Id,
                    Enabled = true
                };

                deviceConfiguration.Assign(devices[i]);
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

            foreach (Xgminer.Data.Device device in devices)
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
            foreach (Xgminer.Data.Device device in devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));

                //if there is no configuration specifically for the device
                if (existingConfiguration == null)
                {
                    //find a configuration that uses the same driver and that, itself, has no specifically matching device
                    Engine.Data.Configuration.Device orphanedConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
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
            EngineConfiguration.DeviceConfigurations.RemoveAll(c => !devices.Exists(d => d.Equals(c)));
        }

        private static string KnownDevicesFileName()
        {
            string filePath = ApplicationPaths.AppDataPath();
            return Path.Combine(filePath, "KnownDevicesCache.xml");
        }

        public void LoadKnownDevicesFromFile()
        {
            string knownDevicesFileName = KnownDevicesFileName();
            if (File.Exists(knownDevicesFileName))
            {
                devices = ConfigurationReaderWriter.ReadConfiguration<List<Xgminer.Data.Device>>(knownDevicesFileName);
                UpdateDevicesForProxySettings();
                ApplyModelsToViewModel();
            }
        }

        private void SaveKnownDevicesToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(devices, KnownDevicesFileName());
        }

        public void SaveChangesLocally()
        {
            SaveViewModelValuesToConfiguration();
            EngineConfiguration.SaveDeviceConfigurations();

            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);

            SetHasChangesLocally(false);            
        }

        public void SetHasChangesLocally(bool hasChanges)
        {
            LocalViewModel.HasChanges = hasChanges;
            DataModified(this, new EventArgs());
        }

        private void SaveViewModelValuesToConfiguration()
        {
            EngineConfiguration.DeviceConfigurations.Clear();

            foreach (Xgminer.Data.Device device in Devices)
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
            bool miningConfigurationValid = EngineConfiguration.DeviceConfigurations.Count(
                c => DeviceConfigurationValid(c)) > 0;
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
                Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                result = coinConfiguration == null ? false : coinConfiguration.Pools.Where(p => !String.IsNullOrEmpty(p.Host) && !String.IsNullOrEmpty(p.Username)).Count() > 0;
            }
            return result;
        }
        #endregion

        #region Model / ViewModel behavior
        public void ApplyModelsToViewModel()
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
            LocalViewModel.ApplyDeviceModels(devices, NetworkDevicesConfiguration.Devices, MetadataConfiguration.Devices);
        }

        public void ApplyCoinInformationToViewModel()
        {
            if (coinApiInformation != null)
                LocalViewModel.ApplyCoinInformationModels(coinApiInformation
                    .ToList()); //get a copy - populated async & collection may be modified)
        }
        #endregion

        #region Exchange API
        public void RefreshExchangeRates()
        {
            if (PerksConfiguration.PerksEnabled && PerksConfiguration.ShowExchangeRates)
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
                    MessageBox.Show(String.Format("{0}: {1}", summary, details), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                },
                ToolTipIcon.Warning, apiUrl);
        }
        #endregion

        #region Network devices
        public void FindNetworkDevices()
        {
            List<Utility.Net.LocalNetwork.NetworkInterfaceInfo> localIpRanges = Utility.Net.LocalNetwork.GetLocalNetworkInterfaces();
            if (localIpRanges.Count == 0)
                return; //no network connection

            const int startingPort = 4028;
            const int endingPort = 4030;

            foreach (Utility.Net.LocalNetwork.NetworkInterfaceInfo interfaceInfo in localIpRanges)
            {
                List<IPEndPoint> miners = MinerFinder.Find(interfaceInfo.RangeStart, interfaceInfo.RangeEnd, startingPort, endingPort);

                //remove own miners
                miners.RemoveAll(m => Utility.Net.LocalNetwork.GetLocalIPAddresses().Contains(m.Address.ToString()));

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

        public void CheckNetworkDevices()
        {
            List<IPEndPoint> endpoints = NetworkDevicesConfiguration.Devices.ToIPEndPoints();

            //remove own miners
            endpoints.RemoveAll(m => Utility.Net.LocalNetwork.GetLocalIPAddresses().Contains(m.Address.ToString()));

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

            Uri uri = new Uri("http://" + networkDevice.Path);
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

            //setup logging
            apiContext.LogEvent -= LogApiEvent;
            apiContext.LogEvent += LogApiEvent;

            apiContext.SwitchPool(poolIndex);
        }

        public List<PoolInformation> GetCachedPoolInfoFromAddress(string ipAddress, int port)
        {
            List<PoolInformation> poolInformationList = null;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (NetworkDevicePools.ContainsKey(key))
                poolInformationList = NetworkDevicePools[key];
            else
            {
                poolInformationList = GetPoolInfoFromAddress(ipAddress, port);
                NetworkDevicePools[key] = poolInformationList;
            }
            return poolInformationList;
        }

        public List<PoolInformation> GetCachedPoolInfoFromAddress(string address)
        {
            string[] parts = address.Split(':');
            return GetCachedPoolInfoFromAddress(parts[0], int.Parse(parts[1]));
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

        public void ToggleNetworkDeviceHidden(DeviceViewModel deviceViewModel)
        {
            NetworkDevices.NetworkDevice deviceConfiguration = NetworkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Hidden = !deviceConfiguration.Hidden;
            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();

            ApplyDevicesToViewModel();
        }

        public void ToggleNetworkDeviceSticky(DeviceViewModel deviceViewModel)
        {
            NetworkDevices.NetworkDevice deviceConfiguration = NetworkDevicesConfiguration.Devices.Single(
                cfg => String.Format("{0}:{1}", cfg.IPAddress, cfg.Port).Equals(deviceViewModel.Path));

            deviceConfiguration.Sticky = !deviceConfiguration.Sticky;
            NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
        }

        public bool RestartNetworkDevice(DeviceViewModel networkDevice)
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
                NetworkDeviceStatistics.Remove(networkDevice.Path);
            }

            return result;
        }

        private List<MinerStatistics> GetCachedMinerStatisticsFromViewModel(DeviceViewModel deviceViewModel)
        {
            string[] portions = deviceViewModel.Path.Split(':');
            string ipAddress = portions[0];
            int port = int.Parse(portions[1]);
            return GetCachedMinerStatisticsFromAddress(ipAddress, port);
        }

        public List<MinerStatistics> GetCachedMinerStatisticsFromAddress(string ipAddress, int port)
        {
            List<MinerStatistics> minerStatisticsList = null;
            string key = String.Format("{0}:{1}", ipAddress, port);
            if (NetworkDeviceStatistics.ContainsKey(key))
                minerStatisticsList = NetworkDeviceStatistics[key];
            else
            {
                minerStatisticsList = GetMinerStatisticsFromAddress(ipAddress, port);
                NetworkDeviceStatistics[key] = minerStatisticsList;
            }
            return minerStatisticsList;
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
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

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

        public bool ExecuteNetworkDeviceCommand(DeviceViewModel deviceViewModel, string commandText)
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

            while (!success && !stop)
            {
                if (prompt)
                {
                    CredentialsEventArgs ea = new CredentialsEventArgs()
                    {
                        ProtectedResource = deviceName,
                        Username = username,
                        Password = password
                    };

                    CredentialsRequested(this, ea);

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
                            if (ex is Renci.SshNet.Common.SshAuthenticationException)
                                prompt = true;
                            else if ((ex is SocketException) || (ex is Renci.SshNet.Common.SshOperationTimeoutException))
                            {
                                stop = true;
                                PostNotification(String.Format("{0}: {1}", deviceName, ex.Message), ToolTipIcon.Error);
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

        public bool RebootNetworkDevice(DeviceViewModel deviceViewModel)
        {
            string commandText = "reboot";
            return ExecuteNetworkDeviceCommand(deviceViewModel, commandText);
        }

        private bool ExecuteSshCommand(string deviceName, SshClient client, string commandText)
        {
            bool success;
            SshCommand command = client.RunCommand(commandText);
            success = command.ExitStatus == 0;

            if (!success)
                PostNotification(string.Format("{0}: {1}", deviceName, command.Error), ToolTipIcon.Error);

            return success;
        }

        public void RestartSuspectNetworkDevices()
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

        public bool NetworkDeviceWasStopped(DeviceViewModel networkDevice)
        {
            // networkDevicePools is keyed by IP:port, use .Path
            List<PoolInformation> poolInformation = GetCachedPoolInfoFromAddress(networkDevice.Path);

            if (poolInformation == null)
                //RPC API call timed out
                return false;

            const string Disabled = "Disabled";

            foreach (var pool in poolInformation)
                if (!pool.Status.Equals(Disabled))
                    return false;

            return true;
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
            PostNotification(message, ToolTipIcon.Error);
        }

        private static void ClearChainStatus(DeviceViewModel networkDevice)
        {
            for (int i = 0; i < networkDevice.ChainStatus.Length; i++)
                networkDevice.ChainStatus[i] = String.Empty;
        }
        #endregion

        #region Notifications
        private void PostNotification(string id, string text, Action clickHandler, ToolTipIcon kind, string informationUrl)
        {
            NotificationEventArgs notification = new NotificationEventArgs()
            {
                Id = id,
                Text = text,
                ClickHandler = clickHandler,
                Kind = kind,
                InformationUrl = informationUrl
            };

            NotificationReceived(this, notification);
        }

        private void PostNotification(string text, ToolTipIcon icon, string informationUrl = "")
        {
            PostNotification(text, text, () => { }, icon, informationUrl);
        }

        private void PostNotification(string id, string text, ToolTipIcon icon, string informationUrl = "")
        {
            PostNotification(id, text, () => { }, icon, informationUrl);
        }

        private void PostNotification(string text, Action clickHandler, ToolTipIcon icon, string informationUrl = "")
        {
            PostNotification(text, text, clickHandler, icon, informationUrl);
        }
        #endregion

        #region Mining logic
        public bool ScanHardwareLocally()
        {
            try
            {
                DevicesService devicesService = new DevicesService(EngineConfiguration.XgminerConfiguration);
                MinerDescriptor defaultMiner = MinerFactory.Instance.GetDefaultMiner();
                devices = devicesService.GetDevices(MinerPath.GetPathToInstalledMiner(defaultMiner));

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
                devices = new List<Xgminer.Data.Device>(); //dummy empty device list

                return false;
            }

            if ((devices.Count > 0) && (EngineConfiguration.DeviceConfigurations.Count == 0) &&
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

            //cache devices
            SaveKnownDevicesToFile();

            return true;
        }

        public void UpdateDevicesForProxySettings()
        {
            DevicesService service = new DevicesService(EngineConfiguration.XgminerConfiguration);
            service.UpdateDevicesForProxySettings(devices, miningEngine.Mining);
            AddMissingDeviceConfigurations();
        }

        public void StartMiningLocally()
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

            if (!ApplicationViewModel.ConfigFileHandled())
                return;
            
            int donationPercent = 0;
            if (PerksConfiguration.PerksEnabled)
                donationPercent = PerksConfiguration.DonationPercent;
            MiningEngine.StartMining(
                EngineConfiguration,
                Devices,
                CoinApiInformation
                    .ToList(), //get a copy - populated async & collection may be modified
                donationPercent);
        

            //do this AFTER we start mining to pick up any Auto-Mining changes

            //create a deep clone of the mining & device configurations
            //this is so we can accurately display e.g. the currently mining pools
            //even if the user changes pool info without restartinging mining
            MiningCoinConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Coin>, List<Engine.Data.Configuration.Coin>>(EngineConfiguration.CoinConfigurations);
            MiningDeviceConfigurations = ObjectCopier.DeepCloneObject<List<Engine.Data.Configuration.Device>, List<Engine.Data.Configuration.Device>>(EngineConfiguration.DeviceConfigurations);

            EngineConfiguration.SaveDeviceConfigurations(); //save any changes made by the engine

            //update ViewModel with potential changes 
            ApplyModelsToViewModel();
            
            DataModified(this, new EventArgs());

            SaveOwnedProcesses();
        }

        public void SetAllDevicesToCoinLocally(string coinSymbol, bool disableStrategies)
        {
            bool wasMining = MiningEngine.Mining;
            StopMiningLocally();

            Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(coinSymbol));

            EngineConfiguration.DeviceConfigurations.Clear();

            foreach (Xgminer.Data.Device device in Devices)
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
                    EnableMiningStrategies(true);
            }
            else
            {
                if (disableStrategies)
                    EnableMiningStrategies(false);
            }

            DataModified(this, new EventArgs());
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

        public void InstallBackendMinerLocally(MinerDescriptor miner)
        {
            string minerName = miner.Name;

            ProgressStarted(this, new ProgressEventArgs()
            {
                Text = String.Format("Downloading and installing {0} from {1}", minerName, new Uri(miner.Url).Authority)
            });
            
            try
            {
                string minerPath = Path.Combine("Miners", minerName);
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, minerPath);
                MinerInstaller.InstallMiner(UserAgent.AgentString, miner, destinationFolder);
                //may have been installed via Remoting - dismiss notification
                NotificationDismissed(this, new NotificationEventArgs()
                {
                    Id = BFGMinerNotificationId.ToString()
                });
            }
            finally
            {
                ProgressCompleted(this, new EventArgs());
            }
        }

        public void CancelMiningOnStartup()
        {
            startupMiningCountdownSeconds = 0;
            DataModified(this, new EventArgs());
        }

        public void StopMiningLocally()
        {
            LocalViewModel.ClearDeviceInformationFromViewModel();

            UpdateDevicesForProxySettings();
            ApplyModelsToViewModel();
            ClearCachedNetworkDifficulties();
            ProcessDeviceDetails.Clear();
            LastDevicePoolMapping.Clear();

            ClearPoolsFlaggedDown();
            SaveOwnedProcesses();

            DataModified(this, new EventArgs());
        }

        public void ClearPoolsFlaggedDown()
        {
            foreach (Engine.Data.Configuration.Coin coinConfiguration in EngineConfiguration.CoinConfigurations)
                coinConfiguration.PoolsDown = false;
            EngineConfiguration.SaveCoinConfigurations();
        }

        //https://github.com/nwoolls/MultiMiner/issues/152
        //http://social.msdn.microsoft.com/Forums/vstudio/en-US/94ba760c-7080-4614-8a56-15582c48f900/child-process-keeps-parents-socket-open-diagnosticsprocess-and-nettcplistener?forum=netfxbcl
        //keep track of processes we've launched so we can kill them later
        public void SaveOwnedProcesses()
        {
            OwnedProcesses.SaveOwnedProcesses(MiningEngine.MinerProcesses.Select(mp => mp.Process), GetOwnedProcessFilePath());
        }

        private static string GetOwnedProcessFilePath()
        {
            return Path.Combine(Path.GetTempPath(), "MultiMiner.Processes.xml");
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

        private static bool ConfigFileHandled()
        {
            foreach (MinerDescriptor miner in MinerFactory.Instance.Miners)
                if (!ConfigFileHandledForMiner(miner))
                    return false;

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

        public void CheckMiningOnStartupStatus()
        {
            if (StartupMiningCountdownSeconds > 0)
            {
                startupMiningCountdownSeconds--;
                if (StartupMiningCountdownSeconds == 0)
                {
                    System.Windows.Forms.Application.DoEvents();
                    StartMiningLocally();
                }
                DataModified(this, new EventArgs());
            }
        }

        public void SetupMiningOnStartup()
        {
            if (ApplicationConfiguration.StartMiningOnStartup)
            {
                //minimum 1s delay for mining on startup - 0 not allowed
                startupMiningCountdownSeconds = Math.Max(1, ApplicationConfiguration.StartupMiningDelay);
                DataModified(this, new EventArgs());
            }
        }
        #endregion

        #region Stats API
        public void SubmitMultiMinerStatistics()
        {
            string installedVersion = Engine.Installers.MultiMinerInstaller.GetInstalledMinerVersion();
            if (installedVersion.Equals(ApplicationConfiguration.SubmittedStatsVersion))
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
                ApplicationConfiguration.SubmittedStatsVersion = machineStat.MinerVersion;
            }
            catch (WebException)
            {
                //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
            }
        }
        #endregion

        #region Mining event handlers
        public void LogApiEvent(object sender, Xgminer.Api.LogEventArgs eventArgs)
        {
            ApiLogEntry logEntry = new ApiLogEntry();

            logEntry.DateTime = eventArgs.DateTime;
            logEntry.Request = eventArgs.Request;
            logEntry.Response = eventArgs.Response;
            Xgminer.Api.ApiContext apiContext = (Xgminer.Api.ApiContext)sender;
            logEntry.CoinName = GetCoinNameForApiContext(apiContext);
            logEntry.Machine = apiContext.IpAddress + ":" + apiContext.Port;

            //make sure BeginInvoke is allowed
            if (Context != null)
            {
                Context.BeginInvoke((Action)(() =>
                {
                    //code to update UI
                    ApiLogEntryBindingSource.Position = ApiLogEntryBindingSource.Add(logEntry);
                    while (ApiLogEntryBindingSource.Count > 1000)
                        ApiLogEntryBindingSource.RemoveAt(0);
                }));
            }

            LogApiEventToFile(logEntry);
        }

        private string GetCoinNameForApiContext(Xgminer.Api.ApiContext apiContext)
        {
            string coinName = string.Empty;

            foreach (MinerProcess minerProcess in MiningEngine.MinerProcesses)
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
        #endregion

        #region Logging logic
        private void LogApiEventToFile(ApiLogEntry logEntry)
        {
            const string logFileName = "ApiLog.json";
            LogObjectToFile(logEntry, logFileName);
        }

        public void LogObjectToFile(object objectToLog, string logFileName)
        {
            string logDirectory = GetLogDirectory();
            string logFilePath = Path.Combine(logDirectory, logFileName);
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
        private string GetMobileMinerUrl()
        {
            string prefix = "https://";
            if (!ApplicationConfiguration.MobileMinerUsesHttps)
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
        public void SubmitMobileMinerStatistics()
        {
            //are remote monitoring enabled?
            if (!ApplicationConfiguration.MobileMinerMonitoring)
                return;

            //is MobileMiner configured?
            if (!ApplicationConfiguration.IsMobileMinerConfigured())
                return;

            List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList = new List<MobileMiner.Data.MiningStatistics>();

            Action<List<MultiMiner.MobileMiner.Data.MiningStatistics>> asyncAction = AddAllMinerStatistics;
            asyncAction.BeginInvoke(statisticsList,
                ar =>
                {
                    asyncAction.EndInvoke(ar);

                    //System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
                    if (Context == null) return;

                    Context.BeginInvoke((Action)(() =>
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
            if (!ApplicationConfiguration.MobileMinerNetworkMonitorOnly)
                AddLocalMinerStatistics(statisticsList);

            AddAllNetworkMinerStatistics(statisticsList);
        }

        private void AddAllNetworkMinerStatistics(List<MultiMiner.MobileMiner.Data.MiningStatistics> statisticsList)
        {
            //is Network Device detection enabled?
            if (!ApplicationConfiguration.NetworkDeviceDetection)
                return;

            //call ToList() so we can get a copy - otherwise risk:
            //System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            List<NetworkDevices.NetworkDevice> networkDevices = NetworkDevicesConfiguration.Devices.ToList();

            foreach (UX.Data.Configuration.NetworkDevices.NetworkDevice networkDevice in networkDevices)
                AddNetworkMinerStatistics(networkDevice, statisticsList);
        }

        private void AddNetworkMinerStatistics(NetworkDevices.NetworkDevice networkDevice, List<MiningStatistics> statisticsList)
        {
            List<DeviceInformation> deviceInformationList = GetDeviceInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

            if (deviceInformationList == null) //handled failure getting API info
                return;

            List<PoolInformation> poolInformationList = GetCachedPoolInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

            Xgminer.Api.Data.VersionInformation versionInformation = GetVersionInfoFromAddress(networkDevice.IPAddress, networkDevice.Port);

            //we cannot continue without versionInformation as the MinerName is required by MobileMiner or it returns HTTP 400
            if (versionInformation == null) //handled failure getting API info
                return;

            foreach (DeviceInformation deviceInformation in deviceInformationList)
            {
                string devicePath = String.Format("{0}:{1}", networkDevice.IPAddress, networkDevice.Port);

                //don't submit stats until we have a valid ViewModel for the Network Device
                DeviceViewModel deviceViewModel = LocalViewModel.Devices.SingleOrDefault(d => d.Path.Equals(devicePath));
                if (deviceViewModel == null)
                    continue;

                MobileMiner.Data.MiningStatistics miningStatistics = new MobileMiner.Data.MiningStatistics()
                {
                    // submit the Friendly device / machine name
                    MachineName = LocalViewModel.GetFriendlyDeviceName(devicePath, devicePath),

                    // versionInformation may be null if the read timed out
                    MinerName = versionInformation == null ? String.Empty : versionInformation.Name,

                    CoinName = Engine.Data.KnownCoins.BitcoinName,
                    CoinSymbol = Engine.Data.KnownCoins.BitcoinSymbol,
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

        public Coin CoinConfigurationForPoolUrl(string poolUrl)
        {
            Coin coinConfiguration =
                EngineConfiguration.CoinConfigurations
                    .FirstOrDefault(cc =>
                        cc.Pools
                            .Any(p => String.Format("{0}:{1}", p.Host.ShortHostFromHost(), p.Port).Equals(poolUrl.ShortHostFromHost(), StringComparison.OrdinalIgnoreCase))
                    );

            return coinConfiguration;
        }

        public List<DeviceInformation> GetDeviceInfoFromAddress(string ipAddress, int port)
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

        private string GetFriendlyDeviceName(MultiMiner.Xgminer.Data.Device device)
        {
            string result = device.Name;

            DeviceViewModel deviceViewModel = LocalViewModel.Devices.SingleOrDefault(d => d.Equals(device));
            if ((deviceViewModel != null) && !String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                result = deviceViewModel.FriendlyName;

            return result;
        }

        public VersionInformation GetVersionInfoFromAddress(string ipAddress, int port)
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
            List<MinerProcess> minerProcesses = MiningEngine.MinerProcesses.ToList();

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
                    Xgminer.Data.Device device = Devices[deviceIndex];
                    Engine.Data.Configuration.Coin coinConfiguration = CoinConfigurationForDevice(device);

                    miningStatistics.FullName = GetFriendlyDeviceName(device);

                    miningStatistics.PoolName = GetPoolNameByIndex(coinConfiguration, deviceInformation.PoolIndex).DomainFromHost();

                    statisticsList.Add(miningStatistics);
                }
            }
        }

        public static string GetPoolNameByIndex(Engine.Data.Configuration.Coin coinConfiguration, int poolIndex)
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

        public Engine.Data.Configuration.Coin CoinConfigurationForDevice(Xgminer.Data.Device device)
        {
            //get the actual device configuration, text in the ListViewItem may be unsaved
            Engine.Data.Configuration.Device deviceConfiguration = null;
            if (MiningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningDeviceConfigurations
                (MiningDeviceConfigurations != null))
                deviceConfiguration = MiningDeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));
            else
                deviceConfiguration = EngineConfiguration.DeviceConfigurations.SingleOrDefault(dc => dc.Equals(device));

            if (deviceConfiguration == null)
                return null;

            string itemCoinSymbol = deviceConfiguration.CoinSymbol;

            List<Engine.Data.Configuration.Coin> configurations;
            if (MiningEngine.Mining &&
                // if the timing is right, we may be .Mining but not yet have data in miningCoinConfigurations
                (MiningCoinConfigurations != null))
                configurations = MiningCoinConfigurations;
            else
                configurations = EngineConfiguration.CoinConfigurations;

            Engine.Data.Configuration.Coin coinConfiguration = configurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(itemCoinSymbol, StringComparison.OrdinalIgnoreCase));
            return coinConfiguration;
        }

        public int GetDeviceIndexForDeviceDetails(DeviceDetails deviceDetails, MinerProcess minerProcess)
        {
            int result = Devices
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

        public List<DeviceDetails> GetProcessDeviceDetails(MinerProcess minerProcess, List<DeviceInformation> deviceInformationList)
        {
            List<DeviceDetails> processDevices = null;
            if (ProcessDeviceDetails.ContainsKey(minerProcess))
            {
                processDevices = ProcessDeviceDetails[minerProcess];

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
                    ProcessDeviceDetails[minerProcess] = processDevices;
            }
            return processDevices;
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


        public List<DeviceInformation> GetDeviceInfoFromProcess(MinerProcess minerProcess)
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

        private void PopulateMobileMinerStatistics(MultiMiner.MobileMiner.Data.MiningStatistics miningStatistics, DeviceInformation deviceInformation,
            string coinName)
        {
            miningStatistics.MinerName = "MultiMiner";
            miningStatistics.CoinName = coinName;
            Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Name.Equals(coinName));
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
                //submit statistics
                List<MobileMiner.Data.RemoteCommand> commands = MobileMiner.ApiContext.SubmitMiningStatistics(GetMobileMinerUrl(), mobileMinerApiKey,
                    ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
                    statisticsList, ApplicationConfiguration.MobileMinerRemoteCommands);

                //process commands
                if (ApplicationConfiguration.MobileMinerRemoteCommands)
                    Context.BeginInvoke((Action<List<MobileMiner.Data.RemoteCommand>>)((c) => ProcessRemoteCommands(c)), commands);

                mobileMinerSuccess = true;
            }
            catch (WebException ex)
            {
                //could be error 400, invalid app key, error 500, internal error, Unable to connect, endpoint down
                HandleMobileMinerWebException(ex);
            }
        }

        public void QueueMobileMinerNotification(string text, MobileMiner.Data.NotificationKind kind)
        {
            MobileMiner.Data.Notification notification = new MobileMiner.Data.Notification()
            {
                NotificationText = text,
                MachineName = Environment.MachineName,
                NotificationKind = kind
            };
            queuedNotifications.Add(notification);
        }

        public void SubmitMobileMinerNotifications()
        {
            //are remote notifications enabled?
            if (!ApplicationConfiguration.MobileMinerPushNotifications)
                return;

            //is MobileMiner configured?
            if (!ApplicationConfiguration.IsMobileMinerConfigured())
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
                        ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
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

        public void SubmitMobileMinerPools()
        {
            //are remote commands enabled?
            if (!ApplicationConfiguration.MobileMinerRemoteCommands)
                return;

            //is MobileMiner configured?
            if (!ApplicationConfiguration.IsMobileMinerConfigured())
                return;

            Dictionary<string, List<string>> machinePools = new Dictionary<string, List<string>>();
            List<string> pools = new List<string>();
            foreach (Coin coin in EngineConfiguration.CoinConfigurations.Where(cc => cc.Enabled))
                pools.Add(coin.PoolGroup.Name);
            machinePools[Environment.MachineName] = pools;

            foreach (KeyValuePair<string, List<PoolInformation>> networkDevicePool in NetworkDevicePools)
            {
                //ipAddress:port
                string machinePath = networkDevicePool.Key;
                string machineName = LocalViewModel.GetFriendlyDeviceName(machinePath, machinePath);
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
                        ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
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

        private void HandleMobileMinerWebException(WebException webException)
        {
            HttpWebResponse response = webException.Response as HttpWebResponse;
            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (!mobileMinerSuccess)
                    {
                        ApplicationConfiguration.MobileMinerRemoteCommands = false;
                        ApplicationConfiguration.SaveApplicationConfiguration();
                        MobileMinerAuthFailed(this, new EventArgs());
                    }
                }
                else if (ApplicationConfiguration.ShowApiErrors)
                {
                    ShowMobileMinerApiErrorNotification(webException);
                }
            }
        }

        private List<string> GetMobileMinerMachineNames()
        {
            List<string> machineNames = new List<string>();

            if (!ApplicationConfiguration.MobileMinerNetworkMonitorOnly)
                machineNames.Add(Environment.MachineName);

            IEnumerable<DeviceViewModel> networkDevices = LocalViewModel.Devices.Where(d => d.Kind == DeviceKind.NET);
            foreach (DeviceViewModel deviceViewModel in networkDevices)
            {
                string machineName = LocalViewModel.GetFriendlyDeviceName(deviceViewModel.Path, deviceViewModel.Path);
                machineNames.Add(machineName);
            }

            return machineNames;
        }

        private void ProcessRemoteCommands(List<MobileMiner.Data.RemoteCommand> commands)
        {
            List<MobileMiner.Data.RemoteCommand> machineCommands = commands
                .GroupBy(c => c.Machine.Name)
                .Select(c => c.First())
                .ToList();

            if (machineCommands.Count > 0)
            {
                foreach (MobileMiner.Data.RemoteCommand command in machineCommands)
                    ProcessRemoteCommand(command);
            }
        }

        private void ProcessRemoteCommand(RemoteCommand command)
        {
            //check this before actually executing the command
            //point being, say for some reason it takes 2 minutes to restart mining
            //if we check for commands again in that time, we don't want to process it again
            if (processedCommandIds.Contains(command.Id))
                return;

            processedCommandIds.Add(command.Id);
            string commandText = command.CommandText;
            string machineName = command.Machine.Name;

            if (deleteRemoteCommandDelegate == null)
                deleteRemoteCommandDelegate = DeleteRemoteCommand;

            if (machineName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
            {
                ProcessOwnRemoteCommand(commandText);
                deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
            }
            else
            {
                DeviceViewModel networkDevice = LocalViewModel.GetNetworkDeviceByFriendlyName(machineName);
                if (networkDevice != null)
                {
                    ProcessNetworkDeviceRemoteCommand(commandText, networkDevice);
                    deleteRemoteCommandDelegate.BeginInvoke(command, deleteRemoteCommandDelegate.EndInvoke, null);
                }
            }
        }

        private void ProcessNetworkDeviceRemoteCommand(string commandText, DeviceViewModel networkDevice)
        {
            Uri uri = new Uri("http://" + networkDevice.Path);
            Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(uri.Port, uri.Host);

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
                PostNotification(String.Format("Error {0}: {1}", action, ex.Message), ToolTipIcon.Error);
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
                Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.SingleOrDefault(cc => cc.PoolGroup.Name.Equals(coinName));
                if (coinConfiguration != null)
                    SetAllDevicesToCoinLocally(coinConfiguration.PoolGroup.Id, true);
            }
        }

        public void ClearCachedNetworkDifficulties()
        {
            minerNetworkDifficulty.Clear();
        }

        private Action<MobileMiner.Data.RemoteCommand> deleteRemoteCommandDelegate;

        private void DeleteRemoteCommand(MobileMiner.Data.RemoteCommand command)
        {
            try
            {
                MobileMiner.ApiContext.DeleteCommand(GetMobileMinerUrl(), mobileMinerApiKey,
                                    ApplicationConfiguration.MobileMinerEmailAddress, ApplicationConfiguration.MobileMinerApplicationKey,
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
                    Process.Start("http://mobileminercom");
                },
                ToolTipIcon.Warning, "");
        }
        #endregion
        
        #region RPC API
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

        public void CheckAndSetNetworkDifficulty(string ipAddress, int port, string poolUri)
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
        #endregion
    }
}
