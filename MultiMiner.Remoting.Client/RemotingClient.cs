using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Runtime.Remoting.Channels.Tcp;
using MultiMiner.Services;

namespace MultiMiner.Remoting.Client
{
    public class RemotingClient
    {
        public void Initialize()
        {
            const int UserPortMin = 49152;
            int port = UserPortMin + 1473;

            Type remotable = typeof(DevicesService);

            string remoteUri = String.Format("tcp://127.0.0.1:{0}/{1}", port, remotable.Name);
            WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(remotable, remoteUri, WellKnownObjectMode.SingleCall);
            RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
        }
    }
}
