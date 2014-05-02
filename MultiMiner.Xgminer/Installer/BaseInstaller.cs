using MultiMiner.Utility.IO;
using MultiMiner.Utility.OS;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer.Installer
{
    public class BaseInstaller
    {
        public string GetInstalledMinerVersion(string executablePath, bool legacyApi)
        {
            string version = String.Empty;

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, "--version");

            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            startInfo.Arguments = String.Format("{0} {1}", startInfo.Arguments, legacyApi ? String.Empty : MinerParameter.ScanSerialOpenCLNoAuto);

            Process process = Process.Start(startInfo);

            string processOutput = process.StandardOutput.ReadToEnd();

            string pattern = String.Format(@"^.+ (.+\..+){0}", Environment.NewLine);
            Match match = Regex.Match(processOutput, pattern);
            if (match.Success)
                version = match.Groups[1].Value;

#if DEBUG
            Version fuzzVersion = new Version(version);
            version = new Version(fuzzVersion.Major, fuzzVersion.Minor - 1, fuzzVersion.Build).ToString();
#endif

            return version;
        }

        protected string GetDownloadUrl(string htmlRoot, string htmlPath, string filePattern, string downloadPath = "")
        {
            string availableDownloadsHtml = new WebClient().DownloadString(String.Format("{0}{1}", htmlRoot, htmlPath));
            Match match = Regex.Match(availableDownloadsHtml, filePattern);
            if (match.Success)
            {
                string minerFileName = match.Groups[1].Value;
                string minerUrl = String.Format("{0}{1}{3}{2}", htmlRoot, htmlPath, minerFileName, downloadPath);
                return minerUrl;
            }

            return "";
        }

        protected static string GetAvailableMinerVersion(string minerDownloadUrl)
        {
            string version = String.Empty;

            const string pattern = @".+/.+?-(.+?)-.+\..+$";
            Match match = Regex.Match(minerDownloadUrl, pattern);
            if (match.Success)
                version = match.Groups[1].Value;

            return version;
        }

        protected static void InstallMiner(string downloadUrl, string destinationFolder)
        {
            //support Windows and OS X for now, we'll go for Linux in the future
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                throw new NotImplementedException();

            if (!String.IsNullOrEmpty(downloadUrl))
            {
                string minerDownloadFile = Path.Combine(Path.GetTempPath(), "miner.zip");
                File.Delete(minerDownloadFile);

                new WebClient().DownloadFile(new Uri(downloadUrl), minerDownloadFile);
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
    }
}
