using System;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    public class RemotingServer
    {
        private bool serviceStarted = false;
        private ServiceHost myServiceHost = null;
        private const int UserPortMin = 49152;

        
        public const int Port = UserPortMin + 1473;

        public void Startup()
        {
            Uri baseAddress = new Uri("net.tcp://localhost:" + Port + "/RemotingService");

            NetTcpBinding binding = new NetTcpBinding();

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
