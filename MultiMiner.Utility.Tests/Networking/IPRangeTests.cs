using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using MultiMiner.Utility.Net;

namespace MultiMiner.Utility.Networking.Tests
{
    [TestClass]
    public class IPRangeTests
    {
        [TestMethod]
        public void GetIPAddresses_SimpleRange_Succeeds()
        {
            IPRange range = new IPRange("192.168.0.0-255");
            IEnumerable<IPAddress> ipAddresses = range.GetIPAddresses();
            int actualCount = ipAddresses.Count();
            Assert.IsTrue(actualCount == 256);
        }

        [TestMethod]
        public void GetIPAddresses_CIDRRange_Succeeds()
        {
            IPRange range = new IPRange("192.168.0.0/24");
            IEnumerable<IPAddress> ipAddresses = range.GetIPAddresses();
            int actualCount = ipAddresses.Count();
            Assert.IsTrue(actualCount == 256);
        }
    }
}
