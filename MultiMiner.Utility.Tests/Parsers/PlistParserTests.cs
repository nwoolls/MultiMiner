using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MultiMiner.Utility.Parsers.Tests
{
    [TestClass]
    public class PlistParserTests
    {
        [TestMethod]
        public void PlistParser_ParsesFile()
        {
            //arrange
            string executingPath = Path.GetFullPath("App_Data");
            const string fileName = "com.apple.smb.server.plist";
            string plistFilePath = Path.Combine(executingPath, fileName);

            //act
            PlistParser parser = new PlistParser(plistFilePath);

            //assert
            Assert.IsTrue(((string)parser["DOSCodePage"]).Equals("437"));
            Assert.IsTrue(((string)parser["NetBIOSName"]).Equals("NET-BIOS-NAME"));
            Assert.IsTrue(((string)parser["Workgroup"]).Equals("WORKGROUP"));
        }
    }
}
