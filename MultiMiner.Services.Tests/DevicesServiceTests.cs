using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace MultiMiner.Services.Tests
{
    [TestClass]
    public class DevicesServiceTests
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
        public void GetDevices_ReturnsDevices()
        {
            Engine.Data.Configuration.Xgminer minerConfiguration = new Engine.Data.Configuration.Xgminer();
            minerConfiguration.StratumProxy = true;

            DevicesService service = new DevicesService(minerConfiguration);
            List<Xgminer.Data.Device> devices = service.GetDevices(executablePath);

            Assert.IsTrue(devices.Count >= 1);
        }
    }
}
