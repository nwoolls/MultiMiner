using MultiMiner.Utility.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using MultiMiner.Xgminer.Api;

namespace MultiMiner.Xgminer.Discovery
{
    public class MinerFinder
    {
        public static List<IPEndPoint> Find(string ipRange, int startingPort, int endingPort)
        {
            if (startingPort >= endingPort)
                throw new ArgumentException();

            List<IPEndPoint> endpoints = PortScanner.Find(ipRange, startingPort, endingPort);

            List<IPEndPoint> miners = new List<IPEndPoint>();

            foreach (IPEndPoint ipEndpoint in endpoints)
            {
                ApiContext context = new ApiContext(ipEndpoint.Port, ipEndpoint.Address.ToString());
                string response = null;
                try
                {
                    response = context.GetResponse("version");
                }
                catch (Exception ex)
                {
                    response = null;
                }
                if (!String.IsNullOrEmpty(response))
                    miners.Add(ipEndpoint);
            }

            return miners;
        }
    }
}
