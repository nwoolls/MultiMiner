using MultiMiner.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer
{
    public static class Installer
    {
        public static void InstallMiner(string destinationFolder)
        {
            //support Windows and OS X for now, we'll go for Linux in the future
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                throw new NotImplementedException();

            string minerUrl = GetMinerDownloadUrl();

            if (!String.IsNullOrEmpty(minerUrl))
            {
                string minerDownloadFile = Path.Combine(Path.GetTempPath(), "miner.zip");
                File.Delete(minerDownloadFile);

                new WebClient().DownloadFile(new Uri(minerUrl), minerDownloadFile);
                try
                {
                    //first delete the folder contents. this became necessary with cgminer 3.8.0 because
                    //ck stopped shipping cgminer-nogpu.exe, which would leave an old executable behind
                    //and gum up the works later (running an older exe to find the installed version)
                    DeleteFolderContents(destinationFolder);

                    Unzipper.UnzipFileToFolder(minerDownloadFile, destinationFolder);
                }
                finally
                {
                    File.Delete(minerDownloadFile);
                }
            }
        }

        private static void DeleteFolderContents(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                //necessary or an Exception is thrown under Mono for OS X
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                fileInfo.Delete();

            foreach (DirectoryInfo di in directoryInfo.GetDirectories())
            {
                DeleteFolderContents(di.FullName);
                di.Delete();
            }
        }

        public static string GetAvailableMinerVersion()
        {
            string version = String.Empty;

            string minerDownloadUrl = GetMinerDownloadUrl();            
            const string pattern = @".+/.+?-(.+?)-.+\..+$";
            Match match = Regex.Match(minerDownloadUrl, pattern);
            if (match.Success)
                version = match.Groups[1].Value;

            return version;
        }

        public static string GetInstalledMinerVersion(string executablePath)
        {
            string version = String.Empty;

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, "--version");

            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            startInfo.Arguments = String.Format("{0} {1}", startInfo.Arguments, MinerParameter.ScanSerialOpenCLNoAuto);

            Process process = Process.Start(startInfo);

            string processOutput = process.StandardOutput.ReadToEnd();

            string pattern = String.Format(@"^.+ (.+\..+){0}", Environment.NewLine);
            Match match = Regex.Match(processOutput, pattern);
            if (match.Success)
                version = match.Groups[1].Value;

            return version;
        }

        private static string GetMinerDownloadUrl()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
            { return GetXgminerDownloadUrl("bfgminer"); }
            else
                return GetBfgminerWindowsDownloadUrl();
        }

        private static string GetBfgminerWindowsDownloadUrl()
        {
            string downloadRoot = GetMinerDownloadRoot();
            const string downloadPath = "/programs/bitcoin/files/bfgminer/latest/";
            string availableDownloadsHtml = new WebClient().DownloadString(String.Format("{0}{1}", downloadRoot, downloadPath));
            const string pattern = @".*<a href=""(bfgminer-.+?-win32.zip)";
            Match match = Regex.Match(availableDownloadsHtml, pattern);
            if (match.Success)
            {
                string minerFileName = match.Groups[1].Value;
                string minerUrl = String.Format("{0}{1}{2}", downloadRoot, downloadPath, minerFileName);
                return minerUrl;
            }

            return "";
        }
        
        private static string GetXgminerDownloadUrl(string minerName)
        {
            string downloadUrl = String.Empty;

            string downloadRoot = GetMinerDownloadRoot();
            const string downloadPath = "/releases/";
            string availableDownloadsHtml = new WebClient().DownloadString(String.Format("{0}{1}", downloadRoot, downloadPath));
            string pattern = String.Format(@".*<a href="".+/xgminer-osx/releases/(.+/{0}-.+?-osx64.tar.gz)", minerName);
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
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
                //use https - we'll just get redirected
                return "https://github.com/nwoolls/xgminer-osx";
            else
                return "http://luke.dashjr.org";
        }
    }
}
