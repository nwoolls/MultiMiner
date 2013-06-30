using MultiMiner.Xgminer.Api.Parsers;
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
        private TcpClient tcpClient;
        public ApiContext(int port)
        {
            tcpClient = new TcpClient("127.0.0.1", port);
        }

        public List<DeviceInformation> GetDeviceInformation()
        {
            string textResponse = GetResponse(ApiVerb.Devs);
            List<DeviceInformation> result = new List<DeviceInformation>();
            DeviceInformationParser.ParseTextForDeviceInformation(textResponse, result);
            return result;
        }

        public void QuitMining()
        {
            GetResponse(ApiVerb.Quit);
        }

        private string GetResponse(string apiVerb)
        {
            NetworkStream stream = tcpClient.GetStream();
            Byte[] request = System.Text.Encoding.ASCII.GetBytes(apiVerb);
            stream.Write(request, 0, request.Length);

            Byte[] responseBuffer = new Byte[4096];
            string response = string.Empty;
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            response = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
            return response;
        } 
    }
}
