using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.CoinApi;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.CoinApi.Data;

namespace MultiMiner.CoinWarz.Api.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void GetCoinInformation_ReturnsCoinInformation()
        {
            //act
            List<CoinInformation> coinInformation = new ApiContext(Properties.Settings.Default.CoinWarzApiKey).GetCoinInformation().ToList();

            //assert
            Assert.IsTrue(coinInformation.Count > 0);
            Assert.IsTrue(coinInformation.Count(c => c.AdjustedProfitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Price > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Profitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.AverageProfitability > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Difficulty > 0.00) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Algorithm.Equals("Scrypt")) > 0);
            Assert.IsTrue(coinInformation.Count(c => c.Algorithm.Equals("SHA-256")) > 0);
        }

        [TestMethod]
        public void GetCoinInformation_InvalidApiKey_ThrowsCoinApiException()
        {
            //act
            try
            {
                new ApiContext(String.Empty).GetCoinInformation().ToList();
                Assert.Fail("No Exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is CoinApiException);
            }
        }

        [TestMethod]
        public void GetInfoUrl_ReturnsUrl()
        {
            //arrange
            string url;

            //act
            url = new ApiContext(Properties.Settings.Default.CoinWarzApiKey).GetInfoUrl();

            //assert
            Assert.IsTrue(url.StartsWith("http"));
        }

        [TestMethod]
        public void GetApiName_ReturnsValue()
        {
            //arrange
            string name;

            //act
            name = new ApiContext(Properties.Settings.Default.CoinWarzApiKey).GetApiName();

            //assert
            Assert.IsFalse(String.IsNullOrEmpty(name));
        }
    }
}
