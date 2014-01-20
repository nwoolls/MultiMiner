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
        private TcpChannel tcpChannel;

        public void Startup()
        {
            tcpChannel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(tcpChannel, true);

            Type remotable = typeof(DevicesService);

            WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(remotable, remotable.Name, WellKnownObjectMode.SingleCall);
            if (String.IsNullOrEmpty(RemotingConfiguration.ApplicationName))
                RemotingConfiguration.ApplicationName = "MultiMiner.Remoting.Server";
            RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
        }

        public void Shutdown()
        {
            if (tcpChannel != null)
            {
                ChannelServices.UnregisterChannel(tcpChannel);
                tcpChannel.StopListening(null);
                tcpChannel = null;
            }
        }
    }
}
