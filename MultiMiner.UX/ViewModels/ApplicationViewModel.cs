using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.MultipoolApi.Data;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Data;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Discovery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        #endregion

        #region Properties
        public IApiContext SuccessfulApiContext { get { return successfulApiContext; } }
        public List<CoinInformation> CoinApiInformation { get { return coinApiInformation; } }
        public List<PoolGroup> KnownCoins { get { return knownCoins; } }
        public IEnumerable<ExchangeInformation> SellPrices { get { return sellPrices; } }
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
        #endregion

        #region Model / ViewModel behavior
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
    }
}
