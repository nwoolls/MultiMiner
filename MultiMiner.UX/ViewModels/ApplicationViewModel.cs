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
using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Discovery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        #region Events
        //delegate declarations
        public delegate void NotificationEventHandler(object sender, Notification ea);

        //event declarations        
        public event NotificationEventHandler NotificationReceived;
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

        //logic
        private readonly MiningEngine miningEngine = new MiningEngine();
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
        #endregion

        #region Notifications
        private void PostNotification(string id, string text, Action clickHandler, ToolTipIcon kind, string informationUrl)
        {
            Notification notification = new Notification()
            {
                Id = id,
                Text = text,
                ClickHandler = clickHandler,
                Kind = kind,
                InformationUrl = informationUrl
            };

            NotificationReceived(this, notification);
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
    }
}
