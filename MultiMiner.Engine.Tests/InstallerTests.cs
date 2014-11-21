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
        public void InstallMiner_Bfgminer_InstallsBfgminer()
        {
            //arrange
            string tempPath = Path.GetTempPath();
            string minerPath = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(minerPath);
            string executablePath = Path.Combine(minerPath, "bfgminer.exe");

            //act
            List<AvailableMiner> availableMiners = AvailableMiners.GetAvailableMiners(UserAgent);
            AvailableMiner bfgminer = availableMiners.Single(am => am.Name.Equals(MinerNames.BFGMiner, StringComparison.OrdinalIgnoreCase));
            MinerInstaller.InstallMiner(UserAgent, bfgminer, minerPath);

            //assert
            Assert.IsTrue(File.Exists(executablePath));

            //cleanup
            Directory.Delete(minerPath, true);
        }

        [TestMethod]
        public void GetAvailableMinerVersion_Succeeds()
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
