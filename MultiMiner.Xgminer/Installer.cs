using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer
{
    public static class Installer
    {
        private const string cgminerDomain = "ck.kolivas.org";

        public static void InstallMiner(MinerBackend minerBackend, string destinationFolder)
        {
            if (minerBackend == MinerBackend.Bfgminer)
                throw new NotImplementedException();

            string minerUrl = GetLatestCgminerDownloadUrl();
            if (!String.IsNullOrEmpty(minerUrl))
            {
                string minerDownloadFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "miner.zip");
                File.Delete(minerDownloadFile);

                new WebClient().DownloadFile(new Uri(minerUrl), minerDownloadFile);
                try
                {
                    UnzipFileToFolder(minerDownloadFile, destinationFolder, false, true);
                }
                finally
                {
                    File.Delete(minerDownloadFile);
                }
            }
        }

        private static string GetLatestCgminerDownloadUrl()
        {
            string availableDownloadsHtml = new WebClient().DownloadString("http://" + cgminerDomain + "/apps/cgminer/");
            const string pattern = @".*<a href=""(cgminer-.+?-windows.zip)";
            Match match = Regex.Match(availableDownloadsHtml, pattern);
            if (match.Success)
            {
                string minerFileName = match.Groups[1].Value;
                string minerUrl = String.Format("http://ck.kolivas.org/apps/cgminer/{0}", minerFileName);
                return minerUrl;
            }

            return "";
        }

        private static void UnzipFileToFolder(string zipFilePath, string destionationFolder, bool showProgress, bool yesToAll)
        {
            Shell32.Folder sourceFolder = GetShell32NameSpace(zipFilePath);
            Directory.CreateDirectory(destionationFolder);
            Shell32.Folder destinationFolder = GetShell32NameSpace(destionationFolder);
            Shell32.FolderItems sourceFolderItems = sourceFolder.Items();
            Shell32.FolderItem rootItem = sourceFolderItems.Item(0);

            int options = 0;
            if (!showProgress)
                options += 4;
            if (yesToAll)
                options += 16;

            destinationFolder.CopyHere(((Shell32.Folder)rootItem.GetFolder).Items(), options);
        }

        //used instead of shellClass.NameSpace() for compatibility with various Windows OS's
        //http://techitongue.blogspot.com/2012/06/shell32-code-compiled-on-windows-7.html
        public static Shell32.Folder GetShell32NameSpace(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }
    }
}
