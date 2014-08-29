using System;
using System.Net.Sockets;

namespace MultiMiner.Xgminer.Api.Extensions
{
    static class TcpClientExtensions
    {
        public static void Connect(this TcpClient tcpClient, string ipAddress, int port, int connectTimeoutMS)
        {            
            IAsyncResult result = tcpClient.BeginConnect(ipAddress, port, null, null);
            result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectTimeoutMS));
            if (!tcpClient.Connected)
                throw new SocketException((int)SocketError.TimedOut);

            // we have connected
            tcpClient.EndConnect(result);
        }
    }
}
