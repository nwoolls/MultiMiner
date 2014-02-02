using MultiMiner.Win.Configuration;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Win.Extensions
{
    public static class IPEndPointExtensions
    {
        public static List<NetworkDevicesConfiguration.NetworkDevice> ToNetworkDevices(this List<IPEndPoint> endpoints)
        {
            List<NetworkDevicesConfiguration.NetworkDevice> networkDevices = new List<NetworkDevicesConfiguration.NetworkDevice>();

            foreach (IPEndPoint endpoint in endpoints)
            {
                networkDevices.Add(new NetworkDevicesConfiguration.NetworkDevice
                {
                    IPAddress = endpoint.Address.ToString(),
                    Port = endpoint.Port
                });
            }

            return networkDevices;
        }
    }
}
