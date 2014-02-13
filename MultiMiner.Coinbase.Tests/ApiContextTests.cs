using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Coinbase.Data;

namespace MultiMiner.Coinbase.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void GetSellPrices_ReturnsSellPrices()
        {
            //arrange
            SellPrices sellPrices;
            
            //act
            sellPrices = ApiContext.GetSellPrices();

            //assert
            Assert.IsTrue(sellPrices.Subtotal.Amount > 0);
        }

        [TestMethod]
        public void GetSellPrices_SubtotalGreaterThanTotal()
        {
            //arrange
            SellPrices sellPrices;

            //act
            sellPrices = ApiContext.GetSellPrices();

            //assert
            Assert.IsTrue(sellPrices.Subtotal.Amount > sellPrices.Total.Amount);
        }

        [TestMethod]
        public void GetInfoUrl_ReturnsUrl()
        {
            //arrange
            string url;

            //act
            url = ApiContext.GetInfoUrl();

            //assert
            Assert.IsTrue(url.StartsWith("http"));
        }

        [TestMethod]
        public void GetApiName_ReturnsValue()
        {
            //arrange
            string name;

            //act
            name = ApiContext.GetApiName();

            //assert
            Assert.IsFalse(String.IsNullOrEmpty(name));
        }
    }
}
