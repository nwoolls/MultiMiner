using MultiMiner.Utility.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

//simple program to bootstrap an update process
//main app downloads an update (zip file), then call this
//app passing in path to zip file and path to the calling exe
//this app waits for the calling exe to exit, unzips the update
//and re-launches the calling exe

namespace MultiMiner.Update
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ValidateArguments(args))
            {
                OutputProgramUsage();
                return;
            }
            
            string zipFilePath = args[0];
            string executableFilePath = args[1];

            List<Process> runningProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executableFilePath)).ToList();

            int retryCount = 0;
            const int maxRetries = 20;
            while ((runningProcesses.Count > 0) && (retryCount < maxRetries))
            {
                Thread.Sleep(500);
                retryCount++;
                runningProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executableFilePath)).ToList();
            }

            if (runningProcesses.Count > 0)
                throw new Exception(String.Format("Timeout waiting for {0} to exit.", executableFilePath));

            //let resources be freed
            Thread.Sleep(1000);

            //unzip the update
            Unzipper.UnzipFileToFolder(zipFilePath, Path.GetDirectoryName(executableFilePath));
            
            //relaunch the calling executable
            Process.Start(executableFilePath);
        }

        private static bool ValidateArguments(string[] args)
        {
            return (args.Length == 2) && File.Exists(args[0]) && File.Exists(args[1]);
        }

        private static void OutputProgramUsage()
        {
            Console.WriteLine(String.Format("{0}.exe <Zip File Path> <Executable File Path>",
                typeof(Program).Assembly.GetName().Name));
        }
    }
}
