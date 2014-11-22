using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace MultiMiner.Services.Tests
{
    [TestClass]
    public class DevicesServiceTests
    {
        // Use ClassInitialize to run code before running the first test in the class
        private static string executablePath;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            string executingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string solutionDirectory = new DirectoryInfo(executingDirectory).Parent.Parent.Parent.FullName;
            string minersDirectory = Path.Combine(Path.Combine(Path.Combine(Path.Combine(solutionDirectory, "MultiMiner.Win"), "bin"), "debug"), "Miners");

            executablePath = Path.Combine(Path.Combine(minersDirectory, "BFGMiner"), "bfgminer.exe");
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
    }
}
