using MultiMiner.Utility.Net;
using System;
using System.Collections.Generic;
using System.Net;
using MultiMiner.Xgminer.Api;

namespace MultiMiner.Xgminer.Discovery
{
    public class MinerFinder
    {
        public static List<IPEndPoint> Find(IPAddress startingIp, IPAddress endingIp, int startingPort, int endingPort)
        {
            if (startingPort > endingPort)
                throw new ArgumentException();

            List<IPEndPoint> endpoints = PortScanner.Find(startingIp, endingIp, startingPort, endingPort);
            List<IPEndPoint> miners = new List<IPEndPoint>();

            foreach (IPEndPoint ipEndpoint in endpoints)
                if (EndpointIsMiner(ipEndpoint))
                    miners.Add(ipEndpoint);

            return miners;
        }

        public static List<IPEndPoint> Check(List<IPEndPoint> possibleMiners)
        {
            List<IPEndPoint> actualMiners = new List<IPEndPoint>();

            foreach (IPEndPoint possibleMiner in possibleMiners)
                if (EndpointIsMiner(possibleMiner))
                    actualMiners.Add(possibleMiner);

            return actualMiners;
        }

        private static bool EndpointIsMiner(IPEndPoint ipEndpoint)
        {
            bool endpointIsMiner = false;

            ApiContext context = new ApiContext(ipEndpoint.Port, ipEndpoint.Address.ToString());

            string response = null;
            try
            {
                //give the call more time than default (500 ms)
                //we want to minimize removing valid endpoints due to
                //device resource limitations
                const int TimeoutMs = 3000;
                response = context.GetResponse(ApiVerb.Version, TimeoutMs);
            }
            catch (Exception)
            {
                response = null;
            }

            if (!String.IsNullOrEmpty(response) && response.Contains("VERSION"))
                endpointIsMiner = true;

            return endpointIsMiner;
        }
    }
}
