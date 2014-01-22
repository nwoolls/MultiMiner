using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace MultiMiner.Remoting.Server.Broadcast
{
    public class Listener
    {
        //events
        //delegate declarations
        public delegate void PacketReceivedHandler(object sender, PacketReceivedArgs ea);

        //event declarations        
        public event PacketReceivedHandler PacketReceived;

        private UdpClient udpClient;
        public bool listening { get; set; }
        
        public void Listen()
        {
            if (udpClient == null)
                udpClient = new UdpClient(Config.BroadcastPort);
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

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Config.BroadcastPort);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            udpClient.BeginReceive(Receive, null);

            ProcessReceived(ip, bytes);
        }

        private void ProcessReceived(IPEndPoint source, byte[] bytes)
        {
            string jsonData = Encoding.ASCII.GetString(bytes);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Packet packet = serializer.Deserialize<Packet>(jsonData);

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
