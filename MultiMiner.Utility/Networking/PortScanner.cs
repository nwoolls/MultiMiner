using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MultiMiner.Utility.Networking
{
    public class PortScanner
    {
        public static List<IPEndPoint> Find(string ipRange, int startingPort, int endingPort, int connectTimeout = 50)
        {
            if (startingPort >= endingPort)
                throw new ArgumentException();

            List<IPEndPoint> endpoints = new List<IPEndPoint>();

            IEnumerable<IPAddress> ipAddresses = new IPRange(ipRange).GetIPAddresses();

            foreach (IPAddress ipAddress in ipAddresses)
                for (int currentPort = startingPort; currentPort <= endingPort; currentPort++)
                    if (IsPortOpen(ipAddress, currentPort, connectTimeout))
                        endpoints.Add(new IPEndPoint(ipAddress, currentPort));

            return endpoints;
        }

        private static bool IsPortOpen(IPAddress ipAddress, int currentPort, int connectTimeout)
        {
            bool portIsOpen = false;

            using (var tcp = new TcpClient())
            {
                IAsyncResult ar = tcp.BeginConnect(ipAddress, currentPort, null, null);
                using (ar.AsyncWaitHandle)
                {
                    //Wait connectTimeout ms for connection.
                    if (ar.AsyncWaitHandle.WaitOne(connectTimeout, false))
                    {
                        try
                        {
                            tcp.EndConnect(ar);
                            portIsOpen = true;
                            //Connect was successful.
                        }
                        catch
                        {
                            //Server refused the connection.
                        }
                    }
                }
            }

            return portIsOpen;
        }
    }
}
