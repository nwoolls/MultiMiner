using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Xgminer.Discovery.Tests
{
    [TestClass]
    public class MinerFinderTests
    {
        [TestMethod]
        public void Find_FindsMiners()
        {
            const int times = 2;

            for (int i = 0; i < times; i++)
            {
                List<IPEndPoint> miners = MinerFinder.Find(IPAddress.Parse("192.168.0.199"), IPAddress.Parse("192.168.0.200"), 4028, 4028);
                Assert.IsTrue(miners.Count > 0);
            }
        }

        [TestMethod]
        public void Check_ChecksMiners()
        {
            List<IPEndPoint> miners = MinerFinder.Find(IPAddress.Parse("192.168.0.199"), IPAddress.Parse("192.168.0.200"), 4028, 4028);
            int goodCount = miners.Count;

            miners.Add(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 1000));

            int checkCount = MinerFinder.Check(miners).Count;

            Assert.IsTrue(checkCount == goodCount);
        }
    }
}
