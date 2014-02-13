using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System;
using MultiMiner.CoinApi.Data;

namespace MultiMiner.CoinChoose.Api.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void GetCoinInformation_ReturnsCoinInformation()
        {
            //act
            List<CoinInformation> coinInformation = new ApiContext().GetCoinInformation().ToList();

            //assert
            Assert.IsTrue(coinInformation.Count > 0);
            Assert.IsTrue(coinInformation.Count(c => c.AdjustedProfitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Price > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Profitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.AverageProfitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Difficulty > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Algorithm.Equals("scrypt")) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Algorithm.Equals("SHA-256")) > 0);
        }

        [TestMethod]
        public void GetInfoUrl_ReturnsUrl()
        {
            //arrange
            string url;

            //act
            url = new ApiContext().GetInfoUrl();

            //assert
            Assert.IsTrue(url.StartsWith("http"));
        }

        [TestMethod]
        public void GetApiName_ReturnsValue()
        {
            //arrange
            string name;

            //act
            name = new ApiContext().GetApiName();

            //assert
            Assert.IsFalse(String.IsNullOrEmpty(name));
        }
    }
}
