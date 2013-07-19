using MultiMiner.Xgminer;
using System;

namespace MultiMiner.Engine
{
    public static class MinerPath
    {
        private const string CgminerName = "cgminer";
        private const string BfgminerName = "bfgminer";

        public static string GetPathToInstalledMiner(MinerBackend minerBackend)
        {
            string executablePath = string.Empty;
            string minerName = GetMinerName(minerBackend);

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    executablePath = string.Format(@"/usr/local/bin/{0}", minerName);
                    break;
                default:
                    executablePath = string.Format(@"Miners\{0}\{0}.exe", minerName);
                    break;
            }

            return executablePath;
        }

        public static string GetMinerName(MinerBackend minerBackend)
        {
            string minerName = CgminerName;

            if (minerBackend == MinerBackend.Bfgminer)
                minerName = BfgminerName;

            return minerName;
        }
    }
}
