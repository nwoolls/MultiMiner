using MultiMiner.CoinApi.Data;
using MultiMiner.Xgminer.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Win.Data.Configuration;
using MultiMiner.Win.Extensions;
using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
using MultiMiner.Engine.Data;
using MultiMiner.Win.Data;

namespace MultiMiner.Win.ViewModels
{
    class MinerFormViewModel
    {
        public List<DeviceViewModel> Devices { get; set; }
        public List<CryptoCoin> ConfiguredCoins { get; set; }
        public bool HasChanges { get; set; }
        public bool DynamicIntensity { get; set; }

        public MinerFormViewModel()
        {
            Devices = new List<DeviceViewModel>();
            ConfiguredCoins = new List<CryptoCoin>();
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
                foreach (NetworkDevices.NetworkDevice networkDeviceModel in networkDeviceModels)
                {
                    DeviceViewModel deviceViewModel = networkDeviceModel.ToViewModel();

                    if (Devices.SingleOrDefault(d => d.Equals(deviceViewModel)) == null)
                    {
                        deviceViewModel.Visible = false;
                        Devices.Add(deviceViewModel);
                    }
                }
            }

            //remove entries from Devices not found in deviceModels or  networkDeviceModels
            foreach (DeviceViewModel deviceViewModel in Devices.ToList())
            {
                bool found = true;

                if (deviceViewModel.Kind == DeviceKind.NET)
                    found = networkDeviceModels.Any(d => d.ToViewModel().Equals(deviceViewModel));
                else
                    found = deviceModels.Any(d => d.Equals(deviceViewModel));

                if (!found)
                    Devices.Remove(deviceViewModel);
            }

            //apply metadata
            foreach (Metadata.DeviceMetadata metadata in deviceMetadata)
            {
                DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(metadata));
                if (deviceViewModel != null)
                    ObjectCopier.CopyObject(metadata, deviceViewModel);
            }
        }

        public void ApplyCoinConfigurationModels(List<Engine.Data.Configuration.Coin> configurationModels)
        {
            ConfiguredCoins.Clear();
            foreach (Engine.Data.Configuration.Coin configurationModel in configurationModels.Where(c => c.Enabled))
                ConfiguredCoins.Add(configurationModel.CryptoCoin);
        }

        public void ApplyCoinInformationModels(List<CoinInformation> coinInformationModels)
        {
            //check for Coin != null, device may not have a coin configured
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Coin != null))
            {
                string coinSymbol = deviceViewModel.Coin.Symbol;
                ApplyCoinInformationToViewModel(coinInformationModels, coinSymbol, deviceViewModel);
            }

            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Kind == DeviceKind.NET))
            {
                const string coinSymbol = KnownCoins.BitcoinSymbol;
                ApplyCoinInformationToViewModel(coinInformationModels, coinSymbol, deviceViewModel);
            }
        }

        private static void ApplyCoinInformationToViewModel(List<CoinInformation> coinInformationModels, string coinSymbol, DeviceViewModel deviceViewModel)
        {
            CoinInformation coinInformationModel = coinInformationModels.GetCoinInformationForSymbol(coinSymbol);
            if (coinInformationModel != null)
                ObjectCopier.CopyObject(coinInformationModel, deviceViewModel, "Name", "Exchange");
        }

        public void ClearDeviceInformationFromViewModel()
        {
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Kind != DeviceKind.NET))
                ClearDeviceInformation(deviceViewModel);
        }

        public void ClearNetworkDeviceInformationFromViewModel()
        {
            foreach (DeviceViewModel deviceViewModel in Devices.Where(d => d.Kind == DeviceKind.NET))
                ClearDeviceInformation(deviceViewModel);
        }

        private static void ClearDeviceInformation(DeviceViewModel deviceViewModel)
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
                    deviceViewModel.RejectedSharesPercent += deviceInformationResponseModel.RejectedSharesPercent;
                    deviceViewModel.HardwareErrorsPercent += deviceInformationResponseModel.HardwareErrorsPercent;

                    //now add as a worker
                    DeviceViewModel workerViewModel = new DeviceViewModel();
                    ObjectCopier.CopyObject(deviceInformationResponseModel, workerViewModel, excludedProperties);
                    workerViewModel.WorkerName = deviceInformationResponseModel.Name; //set a default until (if) we get details
                    deviceViewModel.Workers.Add(workerViewModel);
                }
                else
                {
                    ObjectCopier.CopyObject(deviceInformationResponseModel, deviceViewModel, excludedProperties);
                }
            }
            return deviceViewModel;
        }

        public void ApplyPoolInformationResponseModels(string coinSymbol, List<PoolInformation> poolInformationResonseModels)
        {
            IEnumerable<DeviceViewModel> relevantDevices = Devices.Where(d => (d.Coin != null) && d.Coin.Symbol.Equals(coinSymbol));
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

        public void ApplyDeviceDetailsResponseModels(string coinSymbol, List<DeviceDetails> deviceDetailsList)
        {
            //for getting Proxy worker names
            DeviceViewModel proxyDevice = Devices.SingleOrDefault(d => (d.Kind == DeviceKind.PXY) && (d.Coin != null) && d.Coin.Symbol.Equals(coinSymbol));

            if (proxyDevice != null)
            {
                foreach (DeviceDetails deviceDetailsResponse in deviceDetailsList)
                {
                    if (deviceDetailsResponse.Name.Equals("PXY"))
                    {
                        DeviceViewModel worker = proxyDevice.Workers.SingleOrDefault(w => w.Index == deviceDetailsResponse.Index);
                        if (worker != null)
                            worker.WorkerName = deviceDetailsResponse.DevicePath;
                    }
                }
            }
        }

        public void ApplyDeviceConfigurationModels(List<Engine.Data.Configuration.Device> deviceConfigurations, List<Engine.Data.Configuration.Coin> coinConfigurations)
        {
            foreach (DeviceViewModel deviceViewModel in Devices)
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
                            cc => cc.CryptoCoin.Symbol.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                        if (coinConfiguration != null)
                            deviceViewModel.Coin = coinConfiguration.CryptoCoin;
                    }
                }
                else
                {
                    deviceViewModel.Enabled = true;
                    Engine.Data.Configuration.Coin coinConfiguration = coinConfigurations.SingleOrDefault(
                        cc => cc.CryptoCoin.Symbol.Equals(KnownCoins.BitcoinSymbol, StringComparison.OrdinalIgnoreCase));
                    if (coinConfiguration != null)
                        deviceViewModel.Coin = coinConfiguration.CryptoCoin;
                }
            }
        }
    }
}
