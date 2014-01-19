using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MultiMiner.Services;

namespace MultiMiner.Remoting.Server
{
    public class RemotingServer
    {
        private const int UserPortMin = 49152;
        public const int Port = UserPortMin + 1473;

        public void Initialize()
        {

            ChannelServices.RegisterChannel(new TcpChannel(Port), true);

            Type remotable = typeof(DevicesService);

            WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(remotable, remotable.Name, WellKnownObjectMode.SingleCall);
            RemotingConfiguration.ApplicationName = "MultiMiner.Remoting.Server";
            RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
        }
    }
}
