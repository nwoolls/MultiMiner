using System;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    public class RemotingServer
    {
        private bool serviceStarted = false;
        private ServiceHost myServiceHost = null;
        
        public void Startup()
        {
            Uri baseAddress = new Uri("net.tcp://localhost:" + Config.RemotingPort + "/RemotingService");

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            myServiceHost = new ServiceHost(typeof(RemotingService), baseAddress);
            myServiceHost.AddServiceEndpoint(typeof(IRemotingService), binding, baseAddress);

            myServiceHost.Open();

            serviceStarted = true;
        }

        public void Shutdown()
        {
            myServiceHost.Close();
            myServiceHost = null;
            serviceStarted = false;
        }
    }
}
