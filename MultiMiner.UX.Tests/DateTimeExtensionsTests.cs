using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.UX.Extensions;
using System.Globalization;
using System.Threading;

namespace MultiMiner.UX.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void ToReallyShortDateString_CurrentCulture_Works()
        {
            //arrange
            DateTime now = DateTime.Now;
            string shortDateString = now.ToShortDateString();
            string dateSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;
            string value1 = now.Year + dateSeparator;
            string value2 = dateSeparator + now.Year;
            string expectedValue = shortDateString.Replace(value1, String.Empty).Replace(value2, String.Empty);

            //act
            string reallyShortDateString = now.ToReallyShortDateString();

            //assert
            Assert.AreEqual(reallyShortDateString, expectedValue);
        }

        [TestMethod]
        public void ToReallyShortDateString_RussianCulture_Works()
        {
            //arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            //act & assert
            ToReallyShortDateString_CurrentCulture_Works();
        }

        [TestMethod]
        public void ToReallyShortDateString_JapaneseCulture_Works()
        {
            //arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");

            //act & assert
            ToReallyShortDateString_CurrentCulture_Works();
        }
    }
}
