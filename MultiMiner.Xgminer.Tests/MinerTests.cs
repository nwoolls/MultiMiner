using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Xgminer.Api;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class MinerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            MinerConfiguration minerConfig = new MinerConfiguration();
            minerConfig.ExecutablePath = @"C:\Users\Nathanial\Documents\visual studio 2012\Projects\MultiMiner\MultiMiner.Win\bin\Debug\Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);
            System.Collections.Generic.List<Device> devices = miner.GetDevices();
        }
    }
}
