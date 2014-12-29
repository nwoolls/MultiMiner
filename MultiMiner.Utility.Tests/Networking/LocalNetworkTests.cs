using System;
using System.Linq;
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
            
            localIPAddress = Utility.Net.LocalNetwork.GetLocalIPAddress();

            Assert.IsTrue(!String.IsNullOrEmpty(localIPAddress));
        }

        [TestMethod]
        public void GetLocalNetworkInterfaces_Succeeds()
        {
            System.Collections.Generic.List<MultiMiner.Utility.Net.LocalNetwork.NetworkInterfaceInfo> localNetworkInterfaces = null;

            localNetworkInterfaces = Utility.Net.LocalNetwork.GetLocalNetworkInterfaces();

            Assert.IsTrue(localNetworkInterfaces.Any());
        }

        [TestMethod]
        public void GetWorkGroupName_Succeeds()
        {
            string workGroupName = String.Empty;

            workGroupName = Utility.Net.LocalNetwork.GetWorkGroupName();

            Assert.IsTrue(!String.IsNullOrEmpty(workGroupName));
        }
    }
}
