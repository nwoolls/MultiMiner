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
        private const int apiSecondsError = 10;

        [TestMethod]
        public void SubmitMiningStatistics_Succeeds()
        {
            List<MiningStatistics> miningStatistics = new List<MiningStatistics>();
            miningStatistics.Add(new MiningStatistics()
            {
                MachineName = Environment.MachineName,
                Algorithm = "Scrypt",
                CoinName = "Coin",
                CoinSymbol = "COIN",
                FullName = "FullName",
                Kind = "PGA",
                MinerName = "UnitTests",
                Name = "Name"
            });

            DateTime start = DateTime.Now;
            ApiContext.SubmitMiningStatistics(apiUrl, devApiKey, userEmail, userAppKey, miningStatistics, false);
            Assert.IsTrue((DateTime.Now - start).TotalSeconds < apiSecondsError);
        }

        [TestMethod]
        public void SubmitNotifications_Succeeds()
        {
            List<Notification> notifications = new List<Notification>();
            notifications.Add(new Notification()
            {
                MachineName = Environment.MachineName,
                NotificationKind = NotificationKind.Information,
                NotificationText = String.Empty
            });

            DateTime start = DateTime.Now;
            ApiContext.SubmitNotifications(apiUrl, devApiKey, userEmail, userAppKey, notifications);
            Assert.IsTrue((DateTime.Now - start).TotalSeconds < apiSecondsError);
        }

        [TestMethod]
        public void GetCommands_ReturnsCommands()
        {
            DateTime start = DateTime.Now;
            List<RemoteCommand> commands = ApiContext.GetCommands(apiUrl, devApiKey, userEmail, userAppKey, new List<string>() { Environment.MachineName });
            Assert.IsTrue((DateTime.Now - start).TotalSeconds < apiSecondsError);
        }

        [TestMethod]
        public void DeleteCommand_InvalidId_ReturnsNotFound()
        {
            try
            {
                DateTime start = DateTime.Now;
                ApiContext.DeleteCommand(apiUrl, devApiKey, userEmail, userAppKey, Environment.MachineName, 0);
                Assert.IsTrue((DateTime.Now - start).TotalSeconds < apiSecondsError);

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
