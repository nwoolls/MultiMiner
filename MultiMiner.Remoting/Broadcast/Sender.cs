using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Broadcast
{
    public class Sender
    {
        public static void Send(IPAddress ipAddress, object payload)
        {
            //send from each local interface
            //reasoning: virtual network adapters may be the chosen interface otherwise
            List<string> localIPAddresses = Utility.Net.LocalNetwork.GetLocalIPAddresses();
            foreach (string localIPAddress in localIPAddresses)
                Send(IPAddress.Parse(localIPAddress), ipAddress, payload);
        }

        private static void Send(IPAddress source, IPAddress destination, object payload)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(source, 0)))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string jsonPayload = serializer.Serialize(payload);

                Packet packet = new Packet();
                packet.Descriptor = payload.GetType().FullName;
                packet.Payload = jsonPayload;

                string jsonPacket = serializer.Serialize(packet);

                byte[] bytes = Encoding.ASCII.GetBytes(jsonPacket);

                IPEndPoint ip = new IPEndPoint(destination, Config.BroadcastPort);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
