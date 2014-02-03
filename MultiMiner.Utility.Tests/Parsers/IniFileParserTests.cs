using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MultiMiner.Utility.Parsers.Tests
{
    [TestClass]
    public class IniFileParserTests
    {
        [TestMethod]
        public void IniFileParser_ParsesFile()
        {
            //arrange
            string executingPath = Path.GetFullPath("App_Data");
            const string fileName = "system.ini";
            string iniFilePath = Path.Combine(executingPath, fileName);

            //act
            IniFileParser parser = new IniFileParser(iniFilePath);

            //assert
            Assert.IsTrue(parser.GetValue("drivers", "wave").Equals("mmdrv.dll"));
            Assert.IsTrue(parser.GetValue("drivers", "timer").Equals("timer.drv"));
        }
    }
}
