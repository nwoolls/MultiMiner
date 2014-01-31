using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Xgminer.Discovery.Tests
{
    [TestClass]
    public class MinerFinderTests
    {
        [TestMethod]
        public void MinerFinder_FindsMiners()
        {
            const int times = 10;
            for (int i = 0; i < times; i++)
            {
                List<IPEndPoint> miners = MinerFinder.Find("192.168.0.29-100", 4028, 4029);
                Assert.IsTrue(miners.Count >= 4);
            }
        }

        [TestMethod]
        public void MinerFinder_ChecksMiners()
        {
            List<IPEndPoint> miners = MinerFinder.Find("192.168.0.29-100", 4028, 4029);
            int goodCount = miners.Count;

            miners.Add(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 1000));

            int checkCount = MinerFinder.Check(miners).Count;

            Assert.IsTrue(checkCount == goodCount);
        }
    }
}
