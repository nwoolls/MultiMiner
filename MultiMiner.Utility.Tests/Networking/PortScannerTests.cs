using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;
using MultiMiner.Utility.Net;

namespace MultiMiner.Utility.Networking.Tests
{
    [TestClass]
    public class PortScannerTests
    {
        [TestMethod]
        public void Find_Succeeds()
        {
            const int times = 2;

            for (int i = 0; i < times; i++)
            {
                List<IPEndPoint> endpoints = PortScanner.Find(IPAddress.Parse("192.168.0.254"), IPAddress.Parse("192.168.0.255"), 4028, 4028);
                Assert.IsTrue(endpoints.Count > 0);
            }
        }
    }
}
