using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiMiner.Utility.Networking.Tests
{
    [TestClass]
    public class LocalNetworkTests
    {
        [TestMethod]
        public void GetLocalIPAddress_Succeeds()
        {
            string localIPAddress = String.Empty;
            
            localIPAddress = Utility.Networking.LocalNetwork.GetLocalIPAddress();

            Assert.IsTrue(!String.IsNullOrEmpty(localIPAddress));
        }

        [TestMethod]
        public void GetLocalIPAddressRange_Succeeds()
        {
            string localIPAddressRange = String.Empty;

            localIPAddressRange = Utility.Networking.LocalNetwork.GetLocalIPAddressRange();

            Assert.IsTrue(!String.IsNullOrEmpty(localIPAddressRange));
        }

        [TestMethod]
        public void GetWorkGroupName_Succeeds()
        {
            string workGroupName = String.Empty;

            workGroupName = Utility.Networking.LocalNetwork.GetWorkGroupName();

            Assert.IsTrue(!String.IsNullOrEmpty(workGroupName));
        }
    }
}
