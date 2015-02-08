using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiMiner.Remoting.Broadcast
{
    public class Listener
    {
        //events
        //delegate declarations
        public delegate void PacketReceivedHandler(object sender, PacketReceivedArgs ea);

        //event declarations        
        public event PacketReceivedHandler PacketReceived;

        private UdpClient udpClient;
        private bool listening { get; set; }

        public void Listen()
        {
            if (udpClient == null)
            {
                const int MaxTries = 5;
                TryToAllocateClient(MaxTries);
            }
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
                    udpClient = new UdpClient(Config.BroadcastPort);
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
                udpClient.Close();
        }

        private void Receive(IAsyncResult ar)
        {
            if (!listening)
                return;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Config.BroadcastPort);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            udpClient.BeginReceive(Receive, null);

            ProcessReceived(ip, bytes);
        }

        private void ProcessReceived(IPEndPoint source, byte[] bytes)
        {
            string jsonData = Encoding.ASCII.GetString(bytes);
                        
            Packet packet = JsonConvert.DeserializeObject<Packet>(jsonData);

            if (PacketReceived != null)
            {
                PacketReceivedArgs args = new PacketReceivedArgs() 
                { 
                    IpAddress = source.Address.ToString(), 
                    Packet = packet 
                };
                PacketReceived(this, args);
            }
        }
    }
}
