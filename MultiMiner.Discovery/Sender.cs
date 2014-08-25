using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace MultiMiner.Discovery
{
    class Sender
    {
        public static void Send(IPAddress ipAddress, string verb, int fingerprint)
        {
            //send from each local interface
            //reasoning: virtual network adapters may be the chosen interface otherwise
            List<string> localIPAddresses = Utility.Net.LocalNetwork.GetLocalIPAddresses();
            foreach (string localIPAddress in localIPAddresses)
                Send(IPAddress.Parse(localIPAddress), ipAddress, verb, fingerprint);
        }

        public static void Send(IPAddress source, IPAddress destination, string verb, int fingerprint)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(source, 0)))
            {
                Data.Packet packet = new Data.Packet();
                packet.MachineName = Environment.MachineName;
                packet.Fingerprint = fingerprint;
                packet.Verb = verb;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(packet);
                byte[] bytes = Encoding.ASCII.GetBytes(jsonData);

                IPEndPoint ip = new IPEndPoint(destination, Config.Port);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
