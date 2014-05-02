namespace MultiMiner.Xgminer.Installer
{
    public class DarkcoinSGMinerInstaller : BaseInstaller, IMinerInstaller
    {
        public void InstallMiner(string destinationFolder)
        {
            InstallMiner(GetMinerDownloadUrl(), destinationFolder);
        }

        public string GetAvailableMinerVersion()
        {
            //unknown - no way to tell
            return null;
        }

        private string GetMinerDownloadUrl()
        {
            return "http://www.darkcoin.io/downloads/darkcoin-sgminer-windows.zip";
        }
    }
}
