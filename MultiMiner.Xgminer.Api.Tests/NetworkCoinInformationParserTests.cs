using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Xgminer.Api.Parsers;

namespace MultiMiner.Xgminer.Api.Tests
{
    [TestClass]
    public class NetworkCoinInformationParserTests
    {
        [TestMethod]
        public void ParseTextForCoinNetworkInformation_Without_HashMethod_Succeeds()
        {
            // arrange
            var coinInformation = new NetworkCoinInformation();
            var apiText = @"STATUS=S,When=1516477474,Code=78,Msg=CGMiner coin,Description=cgminer 3.12.0|COIN,Current Block Time=1516477181.246917,Current Block Hash=000000000000000000475404549c265739ae2da0cd176f6a60a3f2cff8c4823c,LP=true,Network Difficulty=2227847638503.62830000|";

            // act
            NetworkCoinInformationParser.ParseTextForCoinNetworkInformation(apiText, coinInformation);

            // assert
            Assert.AreEqual(NetworkCoinInformation.UnknownAlgorithm, coinInformation.Algorithm);
        }
    }
}
