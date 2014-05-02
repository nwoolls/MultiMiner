namespace MultiMiner.Xgminer.Installer
{
    public class VertminerInstaller : BaseInstaller, IMinerInstaller
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
            const string htmlRoot = "http://vertcoin.org";
            const string htmlPath = "/";
            const string filePattern = @".*<a href=""/downloads/(vertminer-.+?.zip)"">Windows</a>";
            const string downloadPath = "downloads/";

            return GetDownloadUrl(htmlRoot, htmlPath, filePattern, downloadPath);
        }
    }
}
