using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using MultiMiner.Utility.Networking;
using System.Collections.Generic;

namespace MultiMiner.Utility.Tests.Networking
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
                //MinePeon & S1
                Assert.IsTrue(endpoints.Count >= 4);
            }
        }
    }
}
