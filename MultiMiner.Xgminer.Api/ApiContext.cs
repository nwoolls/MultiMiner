using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer.Api
{
    public class ApiContext
    {
        private static TcpClient tcpClient;
        public ApiContext(int port)
        {
            tcpClient = new TcpClient("127.0.0.1", port);
        }

        public string GetDeviceInformation()
        {
            string apiVerb = ApiVerb.Devs;

            string response = GetResponse(apiVerb);

            return response;
        }

        private static string GetResponse(string apiVerb)
        {
            NetworkStream stream = tcpClient.GetStream();
            Byte[] request = System.Text.Encoding.ASCII.GetBytes(apiVerb);
            stream.Write(request, 0, request.Length);

            Byte[] responseBuffer = new Byte[1024];
            string response = string.Empty;
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            response = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
            return response;
        } 
    }
}
