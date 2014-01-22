using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace MultiMiner.Remoting.Server.Broadcast
{
    class Sender
    {
        public static void Send(IPAddress ipAddress, object payload)
        {
            using (UdpClient client = new UdpClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string jsonPayload = serializer.Serialize(payload);

                Packet packet = new Packet();
                packet.Descriptor = payload.GetType().FullName;
                packet.Payload = jsonPayload;

                string jsonPacket = serializer.Serialize(packet);

                byte[] bytes = Encoding.ASCII.GetBytes(jsonPacket);

                IPEndPoint ip = new IPEndPoint(ipAddress, Config.BroadcastPort);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
