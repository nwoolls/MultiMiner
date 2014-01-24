using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace MultiMiner.Discovery
{
    class Sender
    {
        public static void Send(IPAddress ipAddress, string verb, int fingerprint)
        {
            using (UdpClient client = new UdpClient())
            {
                Packet packet = new Packet();
                packet.MachineName = Environment.MachineName;
                packet.Fingerprint = fingerprint;
                packet.Verb = verb;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(packet);
                byte[] bytes = Encoding.ASCII.GetBytes(jsonData);

                IPEndPoint ip = new IPEndPoint(ipAddress, Config.Port);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
