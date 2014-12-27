using MultiMiner.UX.Data.Configuration;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.UX.Extensions
{
    public static class NetworkDeviceExtensions
    {
        public static List<IPEndPoint> ToIPEndPoints(this List<NetworkDevices.NetworkDevice> networkDevices)
        {
            List<IPEndPoint> endpoints = new List<IPEndPoint>();

            foreach (NetworkDevices.NetworkDevice networkDevice in networkDevices)
                endpoints.Add(new IPEndPoint(IPAddress.Parse(networkDevice.IPAddress), networkDevice.Port));

            return endpoints;
        }

        public static DeviceViewModel ToViewModel(this NetworkDevices.NetworkDevice networkDevice)
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
