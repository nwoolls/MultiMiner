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
        public delegate void InstanceChangedHandler(object sender, InstanceDiscoveredArgs ea);

        //event declarations        
        public event InstanceChangedHandler InstanceOnline;
        public event InstanceChangedHandler InstanceOffline;

        private UdpClient udpClient;
        public bool listening { get; set; }

        private readonly List<string> instances = new List<string>();

        public void Listen()
        {
            if (udpClient == null)
                udpClient = new UdpClient(Config.Port);
            udpClient.BeginReceive(Receive, null);
            listening = true;
        }

        public void Stop()
        {
            //set first
            listening = false;

            if (udpClient != null)
                udpClient.Close();
        }

        private void Receive(IAsyncResult ar)
        {
            if (!listening)
                return;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Config.Port);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            udpClient.BeginReceive(Receive, null);

            ProcessReceived(ip, bytes);
        }

        private void ProcessReceived(IPEndPoint source, byte[] bytes)
        {
            string verb = Encoding.ASCII.GetString(bytes);
            if (verb.Equals(Verbs.Online))
            {
                string ipAddress = source.Address.ToString();
                if (!instances.Contains(ipAddress))
                {
                    instances.Add(ipAddress);
                    Sender.Send(source.Address, Verbs.Online);

                    if (InstanceOnline != null)
                        InstanceOnline(this, new InstanceDiscoveredArgs { IpAddress = ipAddress });
                }
            }
            else if (verb.Equals(Verbs.Offline))
            {
                string ipAddress = source.Address.ToString();
                if (instances.Contains(ipAddress))
                {
                    instances.Remove(ipAddress);

                    if (InstanceOffline != null)
                        InstanceOffline(this, new InstanceDiscoveredArgs { IpAddress = ipAddress });
                }
            }
        }
    }
}
