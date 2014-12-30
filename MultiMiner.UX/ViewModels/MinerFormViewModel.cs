using MultiMiner.CoinApi.Data;
using MultiMiner.Xgminer.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.Extensions;
using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
using MultiMiner.Engine.Data;

namespace MultiMiner.UX.ViewModels
{
    public class MinerFormViewModel
    {
        public List<DeviceViewModel> Devices { get; set; }
        public List<PoolGroup> ConfiguredCoins { get; set; }
        public bool HasChanges { get; set; }
        public bool DynamicIntensity { get; set; }

        public MinerFormViewModel()
        {
            Devices = new List<DeviceViewModel>();
            ConfiguredCoins = new List<PoolGroup>();
        }

        public void ApplyDeviceModels(List<Xgminer.Data.Device> deviceModels, List<NetworkDevices.NetworkDevice> networkDeviceModels,
            List<Metadata.DeviceMetadata> deviceMetadata)
        {
            //add/update Devices from deviceModels
            if (deviceModels != null)
            {
                foreach (Xgminer.Data.Device deviceModel in deviceModels)
                {
                    DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(deviceModel));
                    if (deviceViewModel == null)
                    {
                        deviceViewModel = new DeviceViewModel();
                        Devices.Add(deviceViewModel);
                    }

                    ObjectCopier.CopyObject(deviceModel, deviceViewModel);

                    deviceViewModel.Visible = true;
                }
            }

            //add/update Devices from networkDeviceModels
            if (networkDeviceModels != null)
            {
                foreach (NetworkDevices.NetworkDevice networkDeviceModel in networkDeviceModels.Where(nd => !nd.Hidden))
                {
                    DeviceViewModel deviceViewModel = networkDeviceModel.ToViewModel();

                    if (Devices.SingleOrDefault(d => d.Equals(deviceViewModel)) == null)
                    {
                        //set Visible to false until we have details
                        deviceViewModel.Visible = false;

                        //network devices always enabled
                        deviceViewModel.Enabled = true;

                        //assume BTC until we have pool info
                        deviceViewModel.Coin = new PoolGroup()
                        {
                            Name = KnownCoins.BitcoinName,
                            Id = KnownCoins.BitcoinSymbol,
                            Algorithm = AlgorithmNames.SHA256
                        };

                        Devices.Add(deviceViewModel);
                    }
                }
            }

            //apply metadata ASAP for MobileMiner
            foreach (Metadata.DeviceMetadata metadata in deviceMetadata)
            {
                DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(metadata));
                if (deviceViewModel != null)
                    //only assigned customized values, rest are keys for Equals()
                    deviceViewModel.FriendlyName = metadata.FriendlyName;
            }

            //remove entries from Devices not found in deviceModels or  networkDeviceModels
            foreach (DeviceViewModel deviceViewModel in Devices.ToList())
            {
                bool found = true;

                if (deviceViewModel.Kind == DeviceKind.NET)
                    found = networkDeviceModels.Any(d => d.ToViewModel().Equals(deviceViewModel) && !d.Hidden);
                else
                    found = deviceModels.Any(d => d.Equals(deviceViewModel));

                if (!found)
                    Devices.Remove(deviceViewModel);
            }
        }

        public void ApplyCoinConfigurationModels(List<Engine.Data.Configuration.Coin> configurationModels)
        {
            ConfiguredCoins.Clear();
            foreach (Engine.Data.Configuration.Coin configurationModel in configurationModels.Where(c => c.Enabled))
                ConfiguredCoins.Add(configurationModel.PoolGroup);
        }

        public void ApplyCoinInformationModels(List<CoinInformation> coinInformationModels)
        {
            //check for Coin != null, device may not have a coin configured
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Coin != null))
            {
                string coinSymbol = deviceViewModel.Coin.Id;
                ApplyCoinInformationToViewModel(coinInformationModels, coinSymbol, deviceViewModel);
            }
        }

        private static void ApplyCoinInformationToViewModel(List<CoinInformation> coinInformationModels, string coinSymbol, DeviceViewModel deviceViewModel)
        {
            CoinInformation coinInformationModel = coinInformationModels.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (coinInformationModel != null)
                ObjectCopier.CopyObject(coinInformationModel, deviceViewModel, "Name", "Exchange");
            else
            {
                ClearCoinInformation(deviceViewModel);
            }
        }

        private static void ClearCoinInformation(DeviceViewModel deviceViewModel)
        {
            deviceViewModel.Price = 0;
            deviceViewModel.Profitability = 100;
            deviceViewModel.AdjustedProfitability = 100;
            deviceViewModel.AverageProfitability = 100;
            deviceViewModel.Exchange = 0;
        }

        public void ClearDeviceInformationFromViewModel()
        {
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Kind != DeviceKind.NET))
                ClearDeviceInformation(deviceViewModel);
        }
        
        public static void ClearDeviceInformation(DeviceViewModel deviceViewModel)
        {
            deviceViewModel.AverageHashrate = 0;
            deviceViewModel.CurrentHashrate = 0;
            deviceViewModel.AcceptedShares = 0;
            deviceViewModel.RejectedShares = 0;
            deviceViewModel.HardwareErrors = 0;
            deviceViewModel.Utility = 0;
            deviceViewModel.WorkUtility = 0;
            deviceViewModel.RejectedSharesPercent = 0;
            deviceViewModel.HardwareErrorsPercent = 0;

            deviceViewModel.Pool = String.Empty;
            deviceViewModel.PoolIndex = -1;
            deviceViewModel.FanPercent = 0;
            deviceViewModel.Temperature = 0;
            deviceViewModel.Intensity = String.Empty;

            deviceViewModel.Workers.Clear();
        }

        public DeviceViewModel ApplyDeviceInformationResponseModel(DeviceDescriptor deviceModel, DeviceInformation deviceInformationResponseModel)
        {
            string[] excludedProperties = 
            { 
                "Name",     //don't overwrite our "nice" name
                "Kind",     //we have our own enum Kind
                "Enabled"   //don't overwrite our own Enabled flag
            };

            DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(deviceModel));
            if (deviceViewModel != null)
            {
                if ((deviceModel.Kind == DeviceKind.PXY) || (deviceModel.Kind == DeviceKind.NET))
                {
                    deviceViewModel.PoolIndex = deviceInformationResponseModel.PoolIndex;

                    //we will get multiple deviceInformationResponseModels for the same deviceModel in the case of a Stratum Proxy
                    //bfgminer will report back for each Proxy Worker, but we only show a single entry in the ViewModel that rolls
                    //up the stats for individual Proxy Workers
                    deviceViewModel.AverageHashrate += deviceInformationResponseModel.AverageHashrate;
                    deviceViewModel.CurrentHashrate += deviceInformationResponseModel.CurrentHashrate;
                    deviceViewModel.AcceptedShares += deviceInformationResponseModel.AcceptedShares;
                    deviceViewModel.RejectedShares += deviceInformationResponseModel.RejectedShares;
                    deviceViewModel.HardwareErrors += deviceInformationResponseModel.HardwareErrors;
                    deviceViewModel.Utility += deviceInformationResponseModel.Utility;
                    deviceViewModel.WorkUtility += deviceInformationResponseModel.WorkUtility;

                    //now add as a worker
                    DeviceViewModel workerViewModel = new DeviceViewModel();
                    ObjectCopier.CopyObject(deviceInformationResponseModel, workerViewModel, excludedProperties);
                    workerViewModel.WorkerName = deviceInformationResponseModel.Name; //set a default until (if) we get details
                    deviceViewModel.Workers.Add(workerViewModel);

                    UpdateTemperaturesBasedOnWorkers(deviceViewModel);
                    //recalculate hardware and rejected share percentages - need to be weighted with worker hashrates
                    UpdatePercentagesBasedOnWorkers(deviceViewModel);
                }
                else
                {
                    ObjectCopier.CopyObject(deviceInformationResponseModel, deviceViewModel, excludedProperties);
                }
            }
            return deviceViewModel;
        }

        //update percentage-based device stats by weighing each worker
        private static void UpdatePercentagesBasedOnWorkers(DeviceViewModel deviceViewModel)
        {
            double totalHashrate = deviceViewModel.Workers.Sum(w => w.AverageHashrate);

            double rejectedPercent = 0;
            double errorPercent = 0;

            foreach (DeviceViewModel worker in deviceViewModel.Workers)
            {
                double workerWeight = worker.AverageHashrate / totalHashrate;
                errorPercent += worker.HardwareErrorsPercent * workerWeight;
                rejectedPercent += worker.RejectedSharesPercent * workerWeight;
            }

            deviceViewModel.HardwareErrorsPercent = errorPercent;
            deviceViewModel.RejectedSharesPercent = rejectedPercent;
        }

        //update temperatures by averaging worker temps
        private static void UpdateTemperaturesBasedOnWorkers(DeviceViewModel deviceViewModel)
        {
            IEnumerable<DeviceViewModel> workersWithData = deviceViewModel.Workers.Where(w => w.Temperature > 0);

            if (workersWithData.Count() == 0)
                return;

            deviceViewModel.Temperature = Math.Round(workersWithData.Average(w => w.Temperature), 1);
        }

        public void ApplyPoolInformationResponseModels(string coinSymbol, List<PoolInformation> poolInformationResonseModels)
        {
            //apply to non-Network Devices, those are populated separately
            IEnumerable<DeviceViewModel> relevantDevices = Devices.Where(d => (d.Kind != DeviceKind.NET) && (d.Coin != null) && d.Coin.Id.Equals(coinSymbol));
            foreach (DeviceViewModel relevantDevice in relevantDevices)
            {
            	PoolInformation poolInformation = poolInformationResonseModels.SingleOrDefault(p => p.Index == relevantDevice.PoolIndex);
                if (poolInformation == null)
                {
                    //device not mining, or crashed, or no pool details
                    relevantDevice.LastShareDifficulty = 0;
                    relevantDevice.LastShareTime = null;
                    relevantDevice.Url = String.Empty;
                    relevantDevice.BestShare = 0;
                    relevantDevice.PoolStalePercent = 0;
                }
                else
                {
                    relevantDevice.LastShareDifficulty = poolInformation.LastShareDifficulty;
                    relevantDevice.LastShareTime = poolInformation.LastShareTime;
                    relevantDevice.Url = poolInformation.Url;
                    relevantDevice.BestShare = poolInformation.BestShare;
                    relevantDevice.PoolStalePercent = poolInformation.PoolStalePercent;
                }
            }
        }

        public void ApplyDeviceDetailsResponseModels(List<DeviceDescriptor> processDevices, List<DeviceDetails> deviceDetailsList)
        {
            //for getting Proxy worker names
            DeviceViewModel proxyDevice = Devices.SingleOrDefault(d => d.Enabled && (d.Kind == DeviceKind.PXY) && (processDevices.Any(d2 => d2.Equals(d))));

            if (proxyDevice != null)
            {
                foreach (DeviceDetails deviceDetailsResponse in deviceDetailsList)
                {
                    if (deviceDetailsResponse.Name.Equals("PXY"))
                    {
                        //SingleOrDefault not a safe assumption here - rare
                        DeviceViewModel worker = proxyDevice.Workers.FirstOrDefault(w => w.ID == deviceDetailsResponse.ID);
                        if (worker != null)
                            worker.WorkerName = deviceDetailsResponse.DevicePath;
                    }
                }
            }
        }

        public void ApplyDeviceConfigurationModels(List<Engine.Data.Configuration.Device> deviceConfigurations, List<Engine.Data.Configuration.Coin> coinConfigurations)
        {
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Kind != DeviceKind.NET))
            {
                Engine.Data.Configuration.Device deviceConfiguration = deviceConfigurations.SingleOrDefault(dc => dc.Equals(deviceViewModel));
                if (deviceConfiguration != null)
                {
                    deviceViewModel.Enabled = deviceConfiguration.Enabled;
                    if (String.IsNullOrEmpty(deviceConfiguration.CoinSymbol))
                    {
                        deviceViewModel.Coin = null;
                    }
                    else
                    {
                        Engine.Data.Configuration.Coin coinConfiguration = coinConfigurations.SingleOrDefault(
                            cc => cc.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                        if (coinConfiguration != null)
                            deviceViewModel.Coin = coinConfiguration.PoolGroup;
                    }
                }
                else
                {
                    deviceViewModel.Enabled = true;

                    Engine.Data.Configuration.Coin btcConfiguration = coinConfigurations.SingleOrDefault(
                        cc => cc.PoolGroup.Id.Equals(KnownCoins.BitcoinSymbol, StringComparison.OrdinalIgnoreCase));
                    Engine.Data.Configuration.Coin ltcConfiguration = coinConfigurations.SingleOrDefault(
                        cc => cc.PoolGroup.Id.Equals(KnownCoins.LitecoinSymbol, StringComparison.OrdinalIgnoreCase));

                    if (deviceViewModel.SupportsAlgorithm(AlgorithmNames.Scrypt) && (ltcConfiguration != null))
                        deviceViewModel.Coin = ltcConfiguration.PoolGroup;
                    else if (deviceViewModel.SupportsAlgorithm(AlgorithmNames.SHA256) && (btcConfiguration != null))
                        deviceViewModel.Coin = btcConfiguration.PoolGroup;
                }
            }
        }

        public string GetFriendlyDeviceName(string deviceName, string devicePath)
        {
            string result = deviceName;

            DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Path.Equals(devicePath));
            if ((deviceViewModel != null) && !String.IsNullOrEmpty(deviceViewModel.FriendlyName))
                result = deviceViewModel.FriendlyName;

            return result;
        }

        public DeviceViewModel GetNetworkDeviceByFriendlyName(string friendlyDeviceName)
        {
            DeviceViewModel result = null;

            IEnumerable<DeviceViewModel> networkDevices = Devices.Where(d => d.Kind == DeviceKind.NET);
            foreach (DeviceViewModel item in networkDevices)
            {
                if (GetFriendlyDeviceName(item.Path, item.Path).Equals(friendlyDeviceName, StringComparison.OrdinalIgnoreCase))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }
    }
}
