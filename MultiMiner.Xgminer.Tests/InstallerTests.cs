using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class InstallerTests
    {
        [TestMethod]
        public void InstallMiner_Bfgminer_InstallsBfgminer()
        {
            //arrange
            string tempPath = Path.GetTempPath();
            string minerPath = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(minerPath);
            string executablePath = Path.Combine(minerPath, "bfgminer.exe");

            //act
            Xgminer.Installer.InstallMiner(minerPath);

            //assert
            Assert.IsTrue(File.Exists(executablePath));

            //cleanup
            Directory.Delete(minerPath, true);
        }

        [TestMethod]
        public void GetAvailableMinerVersion_Succeeds()
        {
            //arrange
            string availableMinerVersion;
                
            //act
            Version version = null;
            availableMinerVersion = Xgminer.Installer.GetAvailableMinerVersion();
            bool success = Version.TryParse(availableMinerVersion, out version);

            //assert
            Assert.IsTrue(success);
        }
    }
}
