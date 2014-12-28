using MultiMiner.UX.Data.Configuration;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.UX.Extensions
{
    public static class IPEndPointExtensions
    {
        public static List<NetworkDevices.NetworkDevice> ToNetworkDevices(this List<IPEndPoint> endpoints)
        {
            List<NetworkDevices.NetworkDevice> networkDevices = new List<NetworkDevices.NetworkDevice>();

            foreach (IPEndPoint endpoint in endpoints)
            {
                networkDevices.Add(new NetworkDevices.NetworkDevice
                {
                    IPAddress = endpoint.Address.ToString(),
                    Port = endpoint.Port
                });
            }

            return networkDevices;
        }
    }
}
