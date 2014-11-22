using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MultiMiner.Discovery.Tests
{
    [TestClass]
    public class BroadcasterTests
    {
        private static readonly Listener listener = new Listener();

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            listener.Stop();
        }

        [TestMethod]
        public void Broadcaster_BroadcastOnline_SendsUdpPacket()
        {
            string verb = Verbs.Online;
            bool online, offline;

            Broadcaster_BroadcastVerb_SendsUdpPacket(verb, out online, out offline);

            Assert.IsTrue(online);
            Assert.IsFalse(offline);
        }

        [TestMethod]
        public void Broadcaster_BroadcastUdpPacket()
        {
            bool online, offline;

            Broadcaster_BroadcastVerb_SendsUdpPacket(Verbs.Online, out online, out offline); //need an online first
            Broadcaster_BroadcastVerb_SendsUdpPacket(Verbs.Offline, out online, out offline);

            Assert.IsTrue(offline);
            Assert.IsFalse(online);
        }

        private void Broadcaster_BroadcastVerb_SendsUdpPacket(string verb, out bool online, out bool offline)
        {
            online = false;
            offline = false;
            bool flaggedOnline = false, flaggedOffline = false;

            listener.Listen(0);
            try
            {
                listener.InstanceOnline += (sender, ea) => flaggedOnline = true;
                listener.InstanceOffline += (sender, ea) => flaggedOffline = true;

                Broadcaster.Broadcast(verb, 0);
                Thread.Sleep(100);
            }
            finally
            {
                listener.Stop();
            }

            online = flaggedOnline;
            offline = flaggedOffline;
        }
    }
}
