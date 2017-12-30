using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Xgminer.Data;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class MiningPoolTests
    {
        [TestMethod]
        public void BuildPoolUri_Succeeds()
        {
            // arrange
            MiningPool pool = new MiningPool();
            pool.Host = "stratum+tcp://example.com";
            pool.Port = 3333;

            // act
            string poolUri = pool.BuildPoolUri();

            // assert
            Assert.AreEqual("stratum+tcp://example.com:3333", poolUri);
        }

        [TestMethod]
        public void BuildPoolUri_UnescapesData()
        {
            // arrange
            MiningPool pool = new MiningPool();
            pool.Host = "stratum+tcp://example.com/#fragment";
            pool.Port = 3333;

            // act
            string poolUri = pool.BuildPoolUri();

            // assert
            Assert.AreEqual("stratum+tcp://example.com:3333/#fragment", poolUri);
        }
    }
}
