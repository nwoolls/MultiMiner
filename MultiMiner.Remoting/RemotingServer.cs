using MultiMiner.Utility.OS;
using System;
using System.Net;
using System.ServiceModel;

namespace MultiMiner.Remoting
{
    public class RemotingServer
    {
        private bool serviceStarted = false;
        private ServiceHost myServiceHost = null;

        public void Startup()
        {
            //use Dns.GetHostName() instead of localhost for compatibility with Mono+Linux
            //https://github.com/nwoolls/MultiMiner/issues/62
            string hostname = Dns.GetHostName();

            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
                //otherwise Windows -> OS X gets connection refused (though Linux -> OS X works)
                hostname = "localhost";

            Uri baseAddress = new Uri(String.Format("net.tcp://{0}:{1}/RemotingService", hostname, Config.RemotingPort));

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
