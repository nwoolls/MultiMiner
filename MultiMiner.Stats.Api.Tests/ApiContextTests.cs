using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiMiner.Stats.Api.Tests
{
    [TestClass]
    public class ApiContextTests
    {
        [TestMethod]
        public void SubmitMinerStatistics_Succeeds()
        {
            //arrange
            Machine machineStat = new Machine() 
            { 
                MinerVersion = "1.0.0", 
                Name = Environment.MachineName 
            };

            //act
            ApiContext.SubmitMinerStatistics("http://multiminerstats.azurewebsites.net/api/", machineStat);   
        }
    }
}
