using MultiMiner.Utility.OS;
using System;

namespace MultiMiner.Xgminer.Installer
{
    public class BFGMinerInstaller : BaseInstaller, IMinerInstaller
    {
        public void InstallMiner(string destinationFolder)
        {
            InstallMiner(GetMinerDownloadUrl(), destinationFolder);
        }
        
        public string GetAvailableMinerVersion()
        {
            return GetAvailableMinerVersion(GetMinerDownloadUrl());
        }

        private string GetMinerDownloadUrl()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
            { return GetMacOSXDownloadUrl("bfgminer"); }
            else
                return GetWindowsDownloadUrl();
        }

        private string GetWindowsDownloadUrl()
        {
            string downloadRoot = GetMinerDownloadRoot();
            const string downloadPath = "/programs/bitcoin/files/bfgminer/latest/";
            const string filePattern = @".*<a href=""(bfgminer-.+?-win32.zip)";

            return GetDownloadUrl(downloadRoot, downloadPath, filePattern);
        }
        
        private string GetMacOSXDownloadUrl(string minerName)
        {
            string downloadRoot = GetMinerDownloadRoot();
            const string downloadPath = "/releases/";
            string filePattern = String.Format(@".*<a href="".+/xgminer-osx/releases/(.+/{0}-.+?-osx64.tar.gz)", minerName);

            return GetDownloadUrl(downloadRoot, downloadPath, filePattern);
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
