using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;

namespace MultiMiner.Utility.Networking.Tests
{
    [TestClass]
    public class PortScannerTests
    {
        [TestMethod]
        public void Find_Succeeds()
        {
            const int times = 3;

            for (int i = 0; i < times; i++)
            {
                List<IPEndPoint> endpoints = PortScanner.Find("192.168.0.29-100", 4028, 4029);
                Assert.IsTrue(endpoints.Count > 0);
            }
        }
    }
}
