using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class MinerTests
    {
        // Use ClassInitialize to run code before running the first test in the class
        private static string executablePath;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            //bfgminer is used because it works with Block Erupters in a VM
            string tempPath = Path.GetTempPath();
            string minerPath = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(minerPath);
            executablePath = Path.Combine(minerPath, "bfgminer.exe");
            Xgminer.Installer.InstallMiner(minerPath);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Directory.Delete(Path.GetDirectoryName(executablePath), true);
        }

        [TestMethod]
        //test setup is a single Block Erupter
        //GPU is disabled
        //should return 0 devices as --ndevs is used w/bfgminer
        public void EnumerateDevices_ReturnsNoDevices()
        {
            //arrange
            Data.Configuration.Miner minerConfiguration = new Data.Configuration.Miner();

            minerConfiguration.ExecutablePath = executablePath;
            minerConfiguration.DisableGpu = true;

            Miner miner = new Miner(minerConfiguration);

            //act
            List<Data.Device> devices = miner.EnumerateDevices();

            //assert
            Assert.IsTrue(devices.Count == 0);
        }

        [TestMethod]
        //test setup is a single Block Erupter
        //GPU is disabled
        //should return 1 device as -d? is used
        public void DeviceList_ReturnsDevices()
        {
            //arrange
            Data.Configuration.Miner minerConfiguration = new Data.Configuration.Miner();

            minerConfiguration.ExecutablePath = executablePath;
            minerConfiguration.DisableGpu = true;

            Miner miner = new Miner(minerConfiguration);

            //act
            List<Data.Device> devices = miner.ListDevices();

            //assert
            Assert.IsTrue(devices.Count > 0);
            Assert.IsTrue(devices.First().Kind == Data.DeviceKind.USB);
        }
    }
}
