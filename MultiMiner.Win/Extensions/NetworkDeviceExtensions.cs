using MultiMiner.Win.Configuration;
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
    }
}
