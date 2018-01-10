using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.UX.ViewModels;

namespace MultiMiner.UX.Tests
{
    [TestClass]
    public class DeviceViewModelTests
    {
        [TestMethod]
        public void Set_LastShareDifficulty_Accepts_64bit_Value()
        {
            // arrange
            var model = new DeviceViewModel();
            var diffVal = 43000000000;

            // act
            model.LastShareDifficulty = diffVal;

            // assert
            Assert.AreEqual(diffVal, model.LastShareDifficulty);
        }
    }
}
