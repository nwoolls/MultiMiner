using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace MultiMiner.Discovery
{
    public class Listener
    {
        //events
        //delegate declarations
        public delegate void InstanceChangedHandler(object sender, InstanceChangedArgs ea);

        //event declarations        
        public event InstanceChangedHandler InstanceOnline;
        public event InstanceChangedHandler InstanceOffline;

        private UdpClient udpClient;
        private int fingerprint;
        private bool listening { get; set; }

        private readonly List<Data.Instance> instances = new List<Data.Instance>();

        public void Listen(int fingerprint)
        {
            if (udpClient == null)
            {
                const int MaxTries = 5;
                TryToAllocateClient(MaxTries);
            }
            this.fingerprint = fingerprint;
            udpClient.BeginReceive(Receive, null);
            listening = true;
        }

        private void TryToAllocateClient(int maxTries)
        {
            int tryCount = 0;
            while (udpClient == null) //try twice
            {
                try
                {
                    tryCount++;
                    udpClient = new UdpClient(Config.Port);
                }
                catch (SocketException)
                {
                    if (tryCount == maxTries)
                        throw;
                    else
                        Thread.Sleep(500);
                }
            }
        }

        public void Stop()
        {
            //set first
            listening = false;

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }

        private void Receive(IAsyncResult ar)
        {
            if (!listening)
                return;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Config.Port);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);

            //both checks necessary
            if (!listening)
                return;

            udpClient.BeginReceive(Receive, null);

            ProcessReceived(ip, bytes);
        }

        private void ProcessReceived(IPEndPoint source, byte[] bytes)
        {
            //only response to discovery on the local network since we transfer fingerprints
            if ((source.AddressFamily != AddressFamily.InterNetwork) &&
                (source.AddressFamily != AddressFamily.InterNetworkV6))
                return;

            string jsonData = Encoding.ASCII.GetString(bytes);

            Data.Packet packet = JsonConvert.DeserializeObject<Data.Packet>(jsonData);

            if (packet.Verb.Equals(Verbs.Online))
            {
                //lock for thread-safety, otherwise the dupe check below may not be sound
                lock (instances)
                {
                    //search by MachineName and Fingerprint - these are unique while IP address may not be
                    //reasoning - the same machine may have multiple IP addresses as discovery supports multiple interfaces
                    if (!instances.Any(i => i.MachineName.Equals(packet.MachineName) && (i.Fingerprint == packet.Fingerprint)))
                    {
                        Data.Instance instance = new Data.Instance 
                        { 
                            IpAddress = source.Address.ToString(), 
                            MachineName = packet.MachineName, 
                            Fingerprint = packet.Fingerprint 
                        };

                        instances.Add(instance);

                        Sender.Send(source.Address, Verbs.Online, fingerprint);

                        if (InstanceOnline != null)
                            InstanceOnline(this, new InstanceChangedArgs { Instance = instance });
                    }
                }
            }
            else if (packet.Verb.Equals(Verbs.Offline))
            {
                string ipAddress = source.Address.ToString();

                //lock for thread-safety - collection may be modified
                lock (instances)
                {
                    //search by MachineName and Fingerprint - these are unique while IP address may not be
                    //reasoning - the same machine may have multiple IP addresses as discovery supports multiple interfaces
                    Data.Instance instance = instances.SingleOrDefault(i => i.MachineName.Equals(packet.MachineName) && (i.Fingerprint == packet.Fingerprint));
                    if (instance != null)
                    {
                        instances.Remove(instance);

                        if (InstanceOffline != null)
                            InstanceOffline(this, new InstanceChangedArgs { Instance = instance });
                    }
                }
            }
        }
    }
}
