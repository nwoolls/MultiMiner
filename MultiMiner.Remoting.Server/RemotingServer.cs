using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace MultiMiner.Remoting.Server
{
    public class RemotingServer
    {
        public void Initialize()
        {
            const int UserPortMin = 49152;
            int port = UserPortMin + 1473;

            ChannelServices.RegisterChannel(new TcpChannel(port), true);

            WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(typeof(MultiMiner.Xgminer.Device), "DummyClass", WellKnownObjectMode.SingleCall);
            RemotingConfiguration.ApplicationName = "MultiMiner.Remoting.Server";
            RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
        }
    }
}
