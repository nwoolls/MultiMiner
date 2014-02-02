using MultiMiner.Coin.Api;
using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Win.Configuration;

namespace MultiMiner.Win.ViewModels
{
    class MainFormViewModel
    {
        public List<DeviceViewModel> Devices { get; set; }
        public List<CryptoCoin> ConfiguredCoins { get; set; }
        public bool HasChanges { get; set; }
        public bool DynamicIntensity { get; set; }

        public MainFormViewModel()
        {
            Devices = new List<DeviceViewModel>();
            ConfiguredCoins = new List<CryptoCoin>();
        }

        public void ApplyDeviceModels(List<Device> deviceModels, List<NetworkDevicesConfiguration.NetworkDevice> networkDeviceModels)
        {
            if (deviceModels != null)
            {
                foreach (Device deviceModel in deviceModels)
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

            if (networkDeviceModels != null)
            {
                foreach (NetworkDevicesConfiguration.NetworkDevice networkDeviceModel in networkDeviceModels)
                {
                    DeviceViewModel deviceViewModel = new DeviceViewModel
                    {
                        Kind = DeviceKind.NET,
                        Path = String.Format("{0}:{1}", networkDeviceModel.IPAddress, networkDeviceModel.Port),
                        Name = networkDeviceModel.IPAddress,
                        Driver = "network"
                    };

                    if (Devices.SingleOrDefault(d => d.Equals(deviceViewModel)) == null)
                    {
                        deviceViewModel.Visible = false;
                        Devices.Add(deviceViewModel);
                    }
                }
            }
        }

        public void ApplyCoinConfigurationModels(List<CoinConfiguration> configurationModels)
        {
            ConfiguredCoins.Clear();
            foreach (CoinConfiguration configurationModel in configurationModels.Where(c => c.Enabled))
                ConfiguredCoins.Add(configurationModel.Coin);
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
                string coinSymbol = "BTC";
                ApplyCoinInformationToViewModel(coinInformationModels, coinSymbol, deviceViewModel);
            }
        }

        private static void ApplyCoinInformationToViewModel(List<CoinInformation> coinInformationModels, string coinSymbol, DeviceViewModel deviceViewModel)
        {
            CoinInformation coinInformationModel = coinInformationModels.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (coinInformationModel != null)
            {
                ObjectCopier.CopyObject(coinInformationModel, deviceViewModel, "Name", "Exchange");
            }
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

        public DeviceViewModel ApplyDeviceInformationResponseModel(DeviceDescriptor deviceModel, DeviceInformationResponse deviceInformationResponseModel)
        {
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
                    deviceViewModel.CurrentHashrate += deviceInformationResponseModel.AverageHashrate;
                    deviceViewModel.AcceptedShares += deviceInformationResponseModel.AcceptedShares;
                    deviceViewModel.RejectedShares += deviceInformationResponseModel.RejectedShares;
                    deviceViewModel.HardwareErrors += deviceInformationResponseModel.HardwareErrors;
                    deviceViewModel.Utility += deviceInformationResponseModel.Utility;
                    deviceViewModel.WorkUtility += deviceInformationResponseModel.WorkUtility;
                    deviceViewModel.RejectedSharesPercent += deviceInformationResponseModel.RejectedSharesPercent;
                    deviceViewModel.HardwareErrorsPercent += deviceInformationResponseModel.HardwareErrorsPercent;

                    //now add as a worker
                    DeviceViewModel workerViewModel = new DeviceViewModel();
                    ObjectCopier.CopyObject(deviceInformationResponseModel, workerViewModel, "Name", "Kind");
                    deviceViewModel.Workers.Add(workerViewModel);
                }
                else
                {
                    ObjectCopier.CopyObject(deviceInformationResponseModel, deviceViewModel, "Name", "Kind");
                }
            }
            return deviceViewModel;
        }

        public void ApplyPoolInformationResponseModels(string coinSymbol, List<PoolInformationResponse> poolInformationResonseModels)
        {
            IEnumerable<DeviceViewModel> relevantDevices = Devices.Where(d => (d.Coin != null) && d.Coin.Symbol.Equals(coinSymbol));
            foreach (DeviceViewModel relevantDevice in relevantDevices)
            {
            	PoolInformationResponse poolInformation = poolInformationResonseModels.SingleOrDefault(p => p.Index == relevantDevice.PoolIndex);
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

        public void ApplyDeviceDetailsResponseModels(string coinSymbol, List<DeviceDetailsResponse> deviceDetailsList)
        {
            //for getting Proxy worker names
            DeviceViewModel proxyDevice = Devices.SingleOrDefault(d => (d.Kind == DeviceKind.PXY) && (d.Coin != null) && d.Coin.Symbol.Equals(coinSymbol));

            if (proxyDevice != null)
            {
                foreach (DeviceDetailsResponse deviceDetailsResponse in deviceDetailsList)
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

        public void ApplyDeviceConfigurationModels(List<DeviceConfiguration> deviceConfigurations, List<CoinConfiguration> coinConfigurations)
        {
            foreach (DeviceViewModel deviceViewModel in Devices)
            {
                DeviceConfiguration deviceConfiguration = deviceConfigurations.SingleOrDefault(dc => dc.Equals(deviceViewModel));
                if (deviceConfiguration != null)
                {
                    deviceViewModel.Enabled = deviceConfiguration.Enabled;
                    if (!String.IsNullOrEmpty(deviceConfiguration.CoinSymbol))
                    {
                        CoinConfiguration coinConfiguration = coinConfigurations.SingleOrDefault(
                            cc => cc.Coin.Symbol.Equals(deviceConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));
                        if (coinConfiguration != null)
                            deviceViewModel.Coin = coinConfiguration.Coin;
                    }
                }
                else
                {
                    deviceViewModel.Enabled = true;
                    CoinConfiguration coinConfiguration = coinConfigurations.SingleOrDefault(
                        cc => cc.Coin.Symbol.Equals("BTC", StringComparison.OrdinalIgnoreCase));
                    if (coinConfiguration != null)
                        deviceViewModel.Coin = coinConfiguration.Coin;
                }
            }
        }
    }
}
