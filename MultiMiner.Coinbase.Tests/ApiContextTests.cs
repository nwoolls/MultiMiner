using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Coinbase.Data;
using System.Collections.Generic;
using MultiMiner.ExchangeApi.Data;
using System.Linq;

namespace MultiMiner.Coinbase.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void GetSellPrices_ReturnsSellPrices()
        {
            //arrange
            IEnumerable<ExchangeInformation> exchangeInformation;
            
            //act
            exchangeInformation = (new ApiContext()).GetExchangeInformation();

            //assert
            Assert.IsTrue(exchangeInformation.Sum(sp => sp.ExchangeRate) > 0);
        }

        [TestMethod]
        public void GetInfoUrl_ReturnsUrl()
        {
            //arrange
            string url;

            //act
            url = (new ApiContext()).GetInfoUrl();

            //assert
            Assert.IsTrue(url.StartsWith("http"));
        }

        [TestMethod]
        public void GetApiName_ReturnsValue()
        {
            //arrange
            string name;

            //act
            name = (new ApiContext()).GetApiName();

            //assert
            Assert.IsFalse(String.IsNullOrEmpty(name));
        }
    }
}
