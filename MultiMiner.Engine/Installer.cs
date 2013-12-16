using MultiMiner.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MultiMiner.Engine
{
    public class Installer
    {
        public static string GetAvailableMinerVersion()
        {
            string version = String.Empty;

            string minerDownloadUrl = GetMinerDownloadUrl();
            const string pattern = @".+/.+-(.+)\.zip$";
            Match match = Regex.Match(minerDownloadUrl, pattern);
            if (match.Success)
                version = match.Groups[1].Value;

            return version;
        }

        public static string GetInstalledMinerVersion()
        {
            Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor,
                assemblyVersion.Build);
        }

        //begin to deprecate the -Debug packages 11/28
//        private static bool IsDebugRelease()
//        { 
//#if DEBUG
//            return true;
//#else
//            return false;
//#endif
//        }

        public static void InstallMiner(string destinationFolder)
        {
            string minerUrl = GetMinerDownloadUrl();

            if (!String.IsNullOrEmpty(minerUrl))
            {
                string minerDownloadFile = Path.Combine(Path.GetTempPath(), "miner.zip");
                File.Delete(minerDownloadFile);

                new WebClient().DownloadFile(new Uri(minerUrl), minerDownloadFile);
                try
                {
                    //if the executing assembly is in "destinationFolder" then
                    string executingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    if (destinationFolder.ToLower().Contains(executingPath.ToLower()))
                    {
                        //extract MultiMiner.Update.exe from the zip file, run it, and exit
                        //use a random directory - rare user report of being unable to delete contents
                        //if an existing directory is reused
                        string temporaryFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                        Unzipper.UnzipFileToFolder(minerDownloadFile, temporaryFolder);
                        string updaterFilePath = Path.Combine(temporaryFolder, "MultiMiner.Update.exe");
                        string updaterArguments = String.Format("\"{0}\" \"{1}\"", minerDownloadFile, Assembly.GetEntryAssembly().Location);

                        ProcessStartInfo startInfo = new ProcessStartInfo(updaterFilePath, updaterArguments);
                        startInfo.CreateNoWindow = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(startInfo);

                        Environment.Exit(0);

                        Directory.Delete(temporaryFolder, true);
                    }
                    else
                    {
                        //otherwise, unzip the archive directly into the folder
                        Unzipper.UnzipFileToFolder(minerDownloadFile, destinationFolder);
                    }                    
                }
                finally
                {
                    File.Delete(minerDownloadFile);
                }
            }
        }

        public static string GetMinerDownloadUrl()
        {
            string downloadUrl = String.Empty;
            
            string downloadRoot = GetMinerDownloadRoot();
            const string downloadPath = "/releases/";
            string availableDownloadsHtml = new WebClient().DownloadString(String.Format("{0}{1}", downloadRoot, downloadPath));

            string filePrefix = "MultiMiner";
            //begin to deprecate the -Debug packages 11/28
            //if (IsDebugRelease())
            //    filePrefix = filePrefix + "-Debug";

            string pattern = String.Format(@".*<a href="".+/MultiMiner/releases/(.+/{0}-\d+\.\d+\.\d+\.zip)", filePrefix);
            Match match = Regex.Match(availableDownloadsHtml, pattern);
            if (match.Success)
            {
                string minerFileName = match.Groups[1].Value;
                downloadUrl = String.Format("{0}{1}{2}", downloadRoot, downloadPath, minerFileName);
            }

            return downloadUrl;
        }

        public static string GetMinerDownloadRoot()
        {
            return "http://github.com/nwoolls/multiminer";
        }
    }
}
