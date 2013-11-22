using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using MultiMiner.Coin.Api;

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
        public void GetCoinInformation_BitcoinBasis_IsBasedOnBitcoin()
        {
            //act
            List<CoinInformation> coinInformation = new ApiContext().GetCoinInformation().ToList();

            //assert
            CoinInformation coin = coinInformation.Single(c => c.Symbol.Equals("BTC"));
            Assert.AreEqual(coin.Profitability, 100);
            coin = coinInformation.Single(c => c.Symbol.Equals("LTC"));
            Assert.AreNotEqual(coin.Profitability, 100);
        }

        [TestMethod]
        public void GetCoinInformation_BitcoinBasis_IsBasedOnLitecoin()
        {
            //act
            List<CoinInformation> coinInformation = new ApiContext().GetCoinInformation("", BaseCoin.Litecoin).ToList();

            //assert
            CoinInformation coin = coinInformation.Single(c => c.Symbol.Equals("LTC"));
            Assert.AreEqual(coin.Profitability, 100);
            coin = coinInformation.Single(c => c.Symbol.Equals("BTC"));
            Assert.AreNotEqual(coin.Profitability, 100);
        }
    }
}
