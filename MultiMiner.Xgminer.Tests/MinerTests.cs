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
            MinerConfig minerConfig = new MinerConfig();
            minerConfig.ExecutablePath = @"C:\Users\Nathanial\Documents\visual studio 2012\Projects\MultiMiner\MultiMiner.Win\bin\Debug\Miners\cgminer\cgminer.exe";
            Miner miner = new Miner(minerConfig);

        }
    }
}
