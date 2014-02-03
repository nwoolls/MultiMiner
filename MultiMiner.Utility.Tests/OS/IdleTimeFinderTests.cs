using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiMiner.Utility.OS.Tests
{
    [TestClass]
    public class IdleTimeFinderTests
    {
        [TestMethod]
        public void GetLastInputTime_Succeeds()
        {
            //arrange
            uint lastInputTicks;
            
            //act
            lastInputTicks = IdleTimeFinder.GetLastInputTime();
            long lastInputMs = Environment.TickCount - lastInputTicks;
            TimeSpan idleTimeSpan = TimeSpan.FromMilliseconds(lastInputMs);

            //assert
            //will fail if you were using the kb/mouse actively during test
            //what can I say it's a kvicky
            Assert.IsTrue(idleTimeSpan.TotalMilliseconds > 0);
        }
    }
}
