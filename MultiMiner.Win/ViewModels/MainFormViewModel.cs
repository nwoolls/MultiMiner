using MultiMiner.Coin.Api;
using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Win.ViewModels
{
    class MainFormViewModel
    {
        public List<DeviceViewModel> Devices { get; set; }

        public MainFormViewModel()
        {
            Devices = new List<DeviceViewModel>();
        }

        public void ApplyDeviceModels(List<Device> deviceModels)
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
            }
        }

        public void ApplyCoinInformationModels(List<CoinInformation> coinInformationModels)
        {
            foreach (DeviceViewModel deviceViewModel in Devices)
            {
                CoinInformation coinInformationModel = coinInformationModels.SingleOrDefault(c => c.Symbol.Equals(deviceViewModel.Coin.Symbol, StringComparison.OrdinalIgnoreCase));
                if (coinInformationModel != null)
                {
                    string oldName = deviceViewModel.Name;
                    ObjectCopier.CopyObject(coinInformationModel, deviceViewModel, true);
                    deviceViewModel.Name = oldName;
                }
            }
        }

        public void ClearDeviceInformationResponseModel()
        {
            foreach (DeviceViewModel deviceViewModel in Devices)
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
            }
        }

        public void ApplyDeviceInformationResponseModel(Device deviceModel, DeviceInformationResponse deviceInformationResponseModel)
        {
            DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(deviceModel));
            if (deviceViewModel != null)
            {
                string oldName = deviceViewModel.Name;
                try
                {
                    if (deviceModel.Kind == DeviceKind.PXY)
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
                    }
                    else
                    {
                        ObjectCopier.CopyObject(deviceInformationResponseModel, deviceViewModel, true);
                    }
                }
                finally
                {
                    deviceViewModel.Name = oldName;
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
