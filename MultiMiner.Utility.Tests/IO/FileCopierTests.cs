using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MultiMiner.Utility.IO.Tests
{
    [TestClass]
    public class FileCopierTests
    {
        [TestMethod]
        public void CopyFilesToFolder_Succeeds()
        {
            //arrange
            string tempPath = Path.GetTempPath();
            string destinationPath = Path.Combine(tempPath, Guid.NewGuid().ToString());
            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);
            Directory.CreateDirectory(destinationPath);
            string sourcePath = Path.GetFullPath("App_Data");
            int sourceFileCount = Directory.GetFiles(sourcePath).Length;
            
            //act
            FileCopier.CopyFilesToFolder(sourcePath, destinationPath, "*.*");            
            int destinationFileCount = Directory.GetFiles(destinationPath).Length;
            Directory.Delete(destinationPath, true);

            //assert
            Assert.AreEqual(sourceFileCount, destinationFileCount);
        }
    }
}
