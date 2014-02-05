using System;
using System.Net;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    public class RemotingServer
    {
        private bool serviceStarted = false;
        private ServiceHost myServiceHost = null;
        
        public void Startup()
        {
            //use Dns.GetHostName() instead of localhost for compatibility with Mono+Linux
            //https://github.com/nwoolls/MultiMiner/issues/62
            Uri baseAddress = new Uri(String.Format("net.tcp://{0}:{1}/RemotingService", Dns.GetHostName(), Config.RemotingPort));

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            myServiceHost = new ServiceHost(typeof(RemotingService), baseAddress);
            myServiceHost.AddServiceEndpoint(typeof(IRemotingService), binding, baseAddress);

            myServiceHost.Open();

            serviceStarted = true;
        }

        public void Shutdown()
        {
            if (!serviceStarted)
                return;

            myServiceHost.Close();
            myServiceHost = null;
            serviceStarted = false;
        }
    }
}
