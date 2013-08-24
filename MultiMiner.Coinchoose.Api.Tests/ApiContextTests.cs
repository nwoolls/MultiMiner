using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MultiMiner.Coinchoose.Api.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void GetCoinInformation_ReturnsCoinInformation()
        {
            //act
            System.Collections.Generic.List<CoinInformation> coinInformation = ApiContext.GetCoinInformation();

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
    }
}
