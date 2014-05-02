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
            string htmlRoot = GetMinerDownloadRoot();
            const string htmlPath = "/";
            const string filePattern = @".*<a href=""/downloads/(vertminer-.+?.zip)"">Windows</a>";
            const string downloadPath = "downloads/";

            return GetDownloadUrl(htmlRoot, htmlPath, filePattern, downloadPath);
        }

        public string GetMinerDownloadRoot()
        {
            return "http://vertcoin.org";
        }
    }
}
