using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace MultiMiner.Utility.Net
{
    public class PortScanner
    {
        public static List<IPEndPoint> Find(IPAddress startingIp, IPAddress endingIp, int startingPort, int endingPort, int connectTimeout = 100)
        {
            if (startingPort > endingPort)
                throw new ArgumentException();

            List<IPEndPoint> endpoints = new List<IPEndPoint>();

            List<IPAddress> ipAddresses = new IPRange(startingIp, endingIp).GetIPAddresses().ToList();
            
            //do not remove local IP addresses
            //we want the ability to discover other miners on the same PC

            foreach (IPAddress ipAddress in ipAddresses)
                for (int currentPort = startingPort; currentPort <= endingPort; currentPort++)
                    if (IsPortOpen(ipAddress, currentPort, connectTimeout))
                        endpoints.Add(new IPEndPoint(ipAddress, currentPort));

            return endpoints;
        }

        private static bool IsPortOpen(IPAddress ipAddress, int currentPort, int connectTimeout)
        {
            bool portIsOpen = false;

            //use raw Sockets
            //using TclClient along with IAsyncResult can lead to ObjectDisposedException on Linux+Mono
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);

                IAsyncResult result = socket.BeginConnect(ipAddress.ToString(), currentPort, null, null);
                result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectTimeout), true);

                portIsOpen = socket.Connected;
            }
            catch
            {
            }
            finally
            {
                if (socket != null)
                    socket.Close();
            }

            return portIsOpen;
        }
    }
}
