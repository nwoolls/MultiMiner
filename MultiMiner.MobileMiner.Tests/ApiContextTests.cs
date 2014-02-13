using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using MultiMiner.MobileMiner.Data;

namespace MultiMiner.MobileMiner.Api.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        private const string devApiKey = "VmBUF7VA2cems5";
        private const string userEmail = "user@example.org";
        private const string userAppKey = "c5qN-zRmU-CFuw";
        private const string apiUrl = "http://mobileminer.azurewebsites.net/api";

        [TestMethod]
        public void SubmitMiningStatistics_Succeeds()
        {
            List<MiningStatistics> miningStatistics = new List<MiningStatistics>();
            miningStatistics.Add(new MiningStatistics()
            {
                Algorithm = "Scrypt",
                CoinName = "Coin",
                CoinSymbol = "COIN",
                FullName = "FullName",
                Kind = "PGA",
                MinerName = "UnitTests",
                Name = "Name"
                
            });
            ApiContext.SubmitMiningStatistics(apiUrl, devApiKey, userEmail, userAppKey, Environment.MachineName, miningStatistics);
        }

        [TestMethod]
        public void SubmitMiningStatistics_ZeroLength_ReturnsBadRequest()
        {
            List<MiningStatistics> miningStatistics = new List<MiningStatistics>();
            try
            {
                ApiContext.SubmitMiningStatistics(apiUrl, devApiKey, userEmail, userAppKey, Environment.MachineName, miningStatistics);
                Assert.Fail("No Exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is WebException);
                Assert.IsTrue(((WebException)ex).Status == WebExceptionStatus.ProtocolError);
            }
        }

        [TestMethod]
        public void SubmitNotifications_Succeeds()
        {
            List<string> notifications = new List<string>();
            notifications.Add(String.Empty);
            ApiContext.SubmitNotifications(apiUrl, devApiKey, userEmail, userAppKey, notifications);
        }

        [TestMethod]
        public void GetCommands_ReturnsCommands()
        {
            List<RemoteCommand> commands = ApiContext.GetCommands(apiUrl, devApiKey, userEmail, userAppKey, Environment.MachineName);
        }

        [TestMethod]
        public void DeleteCommand_InvalidId_ReturnsNotFound()
        {
            try
            {
                ApiContext.DeleteCommand(apiUrl, devApiKey, userEmail, userAppKey, Environment.MachineName, 0);
                Assert.Fail("No Exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is WebException);
                Assert.IsTrue(((WebException)ex).Status == WebExceptionStatus.ProtocolError);
            }
        }
    }
}
