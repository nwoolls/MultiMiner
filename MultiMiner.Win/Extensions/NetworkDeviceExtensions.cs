using MultiMiner.Win.Configuration;
using MultiMiner.Win.ViewModels;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Win.Extensions
{
    public static class NetworkDeviceExtensions
    {
        public static List<IPEndPoint> ToIPEndPoints(this List<NetworkDevicesConfiguration.NetworkDevice> networkDevices)
        {
            List<IPEndPoint> endpoints = new List<IPEndPoint>();

            foreach (NetworkDevicesConfiguration.NetworkDevice networkDevice in networkDevices)
                endpoints.Add(new IPEndPoint(IPAddress.Parse(networkDevice.IPAddress), networkDevice.Port));

            return endpoints;
        }

        public static DeviceViewModel ToViewModel(this NetworkDevicesConfiguration.NetworkDevice networkDevice)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel
            {
                Kind = DeviceKind.NET,
                Path = String.Format("{0}:{1}", networkDevice.IPAddress, networkDevice.Port),
                Name = networkDevice.IPAddress,
                Driver = "network"
            };

            return deviceViewModel;
        }
    }
}
