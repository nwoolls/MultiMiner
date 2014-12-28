using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.MultipoolApi.Data;
using MultiMiner.Services;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        #region Events
        //delegate declarations
        public delegate void NotificationEventHandler(object sender, NotificationEventArgs ea);
        public delegate void CredentialsEventHandler(object sender, CredentialsEventArgs ea);

        //event declarations        
        public event NotificationEventHandler NotificationReceived;
        public event CredentialsEventHandler CredentialsRequested;
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

        //logic
        private List<PoolGroup> knownCoins = new List<PoolGroup>();

        //view models
        public readonly MinerFormViewModel LocalViewModel = new MinerFormViewModel();

        //hardware information
        private List<Xgminer.Data.Device> devices;
        public readonly Dictionary<string, List<PoolInformation>> NetworkDevicePools = new Dictionary<string, List<PoolInformation>>();
        public readonly Dictionary<string, List<MinerStatistics>> NetworkDeviceStatistics = new Dictionary<string, List<MinerStatistics>>();

        //logic
        private readonly MiningEngine miningEngine = new MiningEngine();

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
        public List<Xgminer.Data.Device> Devices { get{ return devices; } }

        //view models
        public MinerFormViewModel RemoteViewModel { get; set; } = new MinerFormViewModel();
        
        //threading
        public Control Context { get; set; }
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

        public void SaveKnownDevicesToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(devices, KnownDevicesFileName());
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

        public void ApplyDevicesToViewModel()
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

        public List<MinerStatistics> GetCachedMinerStatisticsFromViewModel(DeviceViewModel deviceViewModel)
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

        public void PostNotification(string text, ToolTipIcon icon, string informationUrl = "")
        {
            PostNotification(text, text, () => { }, icon, informationUrl);
        }

        public void PostNotification(string id, string text, ToolTipIcon icon, string informationUrl = "")
        {
            PostNotification(id, text, () => { }, icon, informationUrl);
        }

        public void PostNotification(string text, Action clickHandler, ToolTipIcon icon, string informationUrl = "")
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

        public string GetCoinNameForApiContext(Xgminer.Api.ApiContext apiContext)
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
    }
}
