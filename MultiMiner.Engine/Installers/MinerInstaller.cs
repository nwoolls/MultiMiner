using MultiMiner.Engine.Data;
using MultiMiner.Utility.IO;
using MultiMiner.Utility.OS;
using MultiMiner.Xgminer;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MultiMiner.Engine.Installers
{
    public class MinerInstaller
    {
        public static void InstallMiner(string userAgent, AvailableMiner miner, string destinationFolder)
        {
            //support Windows and OS X for now, we'll go for Linux in the future
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
                throw new NotImplementedException();

            string minerDownloadFile = Path.Combine(Path.GetTempPath(), "miner.zip");
            File.Delete(minerDownloadFile);

            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", userAgent);

            webClient.DownloadFile(new Uri(miner.Url), minerDownloadFile);
            try
            {
                //first delete the folder contents. this became necessary with cgminer 3.8.0 because
                //ck stopped shipping cgminer-nogpu.exe, which would leave an old executable behind
                //and gum up the works later (running an older exe to find the installed version)

                //only delete files and not folders though - we want to leave behind any OpenCL
                //kernels the user may have installed
                DeleteFolderContents(destinationFolder, true);

                Unzipper.UnzipFileToFolder(minerDownloadFile, destinationFolder);
            }
            finally
            {
                File.Delete(minerDownloadFile);
            }
        }

        private static void DeleteFolderContents(string folderPath, bool preserveKernels)
        {
            if (!Directory.Exists(folderPath))
                //necessary or an Exception is thrown under Mono for OS X
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Extension.Equals(".cl", StringComparison.OrdinalIgnoreCase) && preserveKernels)
                    continue;

                fileInfo.Delete();
            }
        }

        public static string GetInstalledMinerVersion(string executablePath, bool legacyApi)
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

            version = SemanticVersionToStandardVersion(version);

#if DEBUG
            if (!String.IsNullOrEmpty(version))
            {
                Version fuzzVersion = new Version(version);
                version = new Version(fuzzVersion.Major, Math.Max(0, fuzzVersion.Minor - 1), fuzzVersion.Build).ToString();
            }
#endif

            return version;
        }

        private static string SemanticVersionToStandardVersion(string semanticVersion)
        {
            string result = semanticVersion;

            const string Dash = "-";
            int dashIndex = semanticVersion.IndexOf(Dash);
            if (dashIndex >= 0)
            {
                //examples of versions built from Git
                //OS X : bfgminer 4.7.0-unknown
                //Linux: bfgminer 4.5.0-1-g989677a
                string primaryInfo = semanticVersion.Substring(0, dashIndex);
                int secondDashPos = semanticVersion.IndexOf(Dash, dashIndex + 1);
                if (secondDashPos > 0)
                {
                    string preReleaseInfo = semanticVersion.Substring(dashIndex + 1, secondDashPos - dashIndex - 1);
                    result = String.Format("{0}.{1}", primaryInfo, preReleaseInfo);
                }
                else
                {
                    //can't detect pre-release info on OS X (yet)
                    result = primaryInfo;
                }
            }

            return result;
        }
    }
}
