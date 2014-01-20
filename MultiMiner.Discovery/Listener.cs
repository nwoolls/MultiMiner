using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiMiner.Discovery
{
    public class Listener
    {
        //events
        //delegate declarations
        public delegate void InstanceDiscoveredHandler(object sender, InstanceDiscoveredArgs ea);

        //event declarations        
        public event InstanceDiscoveredHandler InstanceDiscovered;

        private UdpClient udpClient;

        private readonly List<string> instances = new List<string>();

        public void Listen()
        {
            udpClient = new UdpClient(Keys.Port);
            udpClient.BeginReceive(Receive, null);
        }

        public void Stop()
        {
            if (udpClient != null)
                udpClient.Close();
            udpClient = null;
        }

        private void Receive(IAsyncResult ar)
        {
            if (udpClient == null)
                return;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Keys.Port);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            udpClient.BeginReceive(Receive, null);

            ProcessReceived(ip, bytes);
        }

        private void ProcessReceived(IPEndPoint source, byte[] bytes)
        {
            string message = Encoding.ASCII.GetString(bytes);
            if (message.Equals(Keys.Identifier))
            {
                string ipAddress = source.Address.ToString();
                if (!instances.Contains(ipAddress))
                {
                    instances.Add(ipAddress);
                    Sender.Send(source.Address);

                    if (InstanceDiscovered != null)
                        InstanceDiscovered(this, new InstanceDiscoveredArgs { IpAddress = ipAddress });
                }
            }
        }
    }
}
