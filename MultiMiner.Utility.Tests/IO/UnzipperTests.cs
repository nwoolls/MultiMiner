using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MultiMiner.Utility.IO.Tests
{
    [TestClass]
    public class UnzipperTests
    {
        [TestMethod]
        public void UnzipFileToFolder_Succeeds()
        {
            //arrange
            string tempPath = Path.GetTempPath();
            string executingPath = Path.GetFullPath("App_Data");
            const string fileName = "Unzipper-Test.zip";
            string tempFilePath = Path.Combine(tempPath, Path.ChangeExtension(fileName, "txt"));
            string zipFilePath = Path.Combine(executingPath, fileName);
            File.Delete(tempFilePath);

            //act
            Unzipper.UnzipFileToFolder(zipFilePath, tempPath);

            //assert
            Assert.IsTrue(File.Exists(zipFilePath));
            string contents = File.ReadAllText(tempFilePath);
            Assert.IsTrue(contents.Equals("Unzipper Test"));

            File.Delete(tempFilePath);            
        }
    }
}
