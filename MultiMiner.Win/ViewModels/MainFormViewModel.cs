using MultiMiner.Coin.Api;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void ApplyDeviceInformationResponseModel(Device deviceModel, DeviceInformationResponse deviceInformationResponseModel)
        {
            DeviceViewModel deviceViewModel = Devices.SingleOrDefault(d => d.Equals(deviceModel));
            if (deviceViewModel != null)
                ObjectCopier.CopyObject(deviceInformationResponseModel, deviceViewModel, true);         
        }
    }
}
