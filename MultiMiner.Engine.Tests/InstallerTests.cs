using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Installers;
using System.Linq;

namespace MultiMiner.Engine.Tests
{
    [TestClass]
    public class InstallerTests
    {
        const string UserAgent = "MultiMiner/V3-Example";

        [TestMethod]
        public void GetAvailableMiners_ReturnsBfgminerUrl()
        {
            //act
            List<AvailableMiner> availableMiners = AvailableMiners.GetAvailableMiners(UserAgent);
            AvailableMiner bfgminer = availableMiners.Single(am => am.Name.Equals(MinerNames.BFGMiner, StringComparison.OrdinalIgnoreCase));

            //assert
            Assert.IsTrue(!String.IsNullOrEmpty(bfgminer.Url));
        }

        [TestMethod]
        public void GetAvailableMiners_Succeeds()
        {                
            //act
            Version version = null;
            List<AvailableMiner> availableMiners = AvailableMiners.GetAvailableMiners(UserAgent);
            AvailableMiner bfgminer = availableMiners.Single(am => am.Name.Equals(MinerNames.BFGMiner, StringComparison.OrdinalIgnoreCase));
            bool success = Version.TryParse(bfgminer.Version, out version);

            //assert
            Assert.IsTrue(success);
        }
    }
}
