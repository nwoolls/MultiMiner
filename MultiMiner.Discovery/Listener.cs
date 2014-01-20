using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiMiner.Discovery
{
    public class Listener
    {
        private readonly UdpClient udpClient = new UdpClient(Keys.Port);

        private readonly List<string> instances = new List<string>();

        public void Listen()
        {
            udpClient.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Keys.Port);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            Listen();

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
                }
            }
        }
    }
}
