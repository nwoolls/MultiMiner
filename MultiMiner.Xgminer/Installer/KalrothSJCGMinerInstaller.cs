namespace MultiMiner.Xgminer.Installer
{
    public class KalrothSJCGMinerInstaller : BaseInstaller, IMinerInstaller
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
            string htmlRoot = GetMinerDownloadRoot();
            const string htmlPath = "/";
            const string filePattern = @".*<a href=""https://sites.google.com/site/rmdavidson/(cgminer-.+?-kalroth-sj0.03.zip)";
            const string downloadPath = "";

            return GetDownloadUrl(htmlRoot, htmlPath, filePattern, downloadPath);
        }

        public string GetMinerDownloadRoot()
        {
            return "https://sites.google.com/site/rmdavidson";
        }
    }
}
