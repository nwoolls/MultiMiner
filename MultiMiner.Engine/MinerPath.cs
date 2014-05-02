using MultiMiner.Utility.OS;
using MultiMiner.Xgminer.Data;
using System;
using System.IO;

namespace MultiMiner.Engine
{
    public static class MinerPath
    {
        public static string GetPathToInstalledMiner(MinerDescriptor miner)
        {
            string executablePath = string.Empty;

            switch (miner.Algorithm)
            {
                case CoinAlgorithm.SHA256:
                    executablePath = GetPathToBFGMiner(miner.FileName);
                    break;
                case CoinAlgorithm.Scrypt:
                    executablePath = GetPathToBFGMiner(miner.FileName);
                    break;
                case CoinAlgorithm.ScryptJane:
                    executablePath = GetPathToMinerOnWindows(miner.Name, miner.FileName);
                    break;
                case CoinAlgorithm.ScryptN:
                    executablePath = GetPathToMinerOnWindows(miner.Name, miner.FileName);
                    break;
                case CoinAlgorithm.X11:
                    executablePath = GetPathToMinerOnWindows(miner.Name, miner.FileName);
                    break;
            }

            return executablePath;
        }

        private static string GetPathToBFGMiner(string minerName)
        {
            string executablePath;

            switch (OSVersionPlatform.GetConcretePlatform())
            {
                case PlatformID.MacOSX:
                    executablePath = GetPathToMinerOnMacOSX(minerName);
                    break;

                //support Unix - there is no bin folder for the executables like on Mac OS X
                case PlatformID.Unix:
                    executablePath = GetPathToMinerOnLinux(minerName);
                    break;

                default:
                    executablePath = GetPathToMinerOnWindows(minerName, minerName);
                    break;
            }
            return executablePath;
        }

        private static string GetPathToMinerOnMacOSX(string minerName)
        {
            string executablePath;
            //try local path first
            executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Miners/{0}/bin/{0}", minerName));

            if (!File.Exists(executablePath))
                //try global path (Homebrew)
                executablePath = string.Format(@"/usr/local/bin/{0}", minerName);
            return executablePath;
        }

        private static string GetPathToMinerOnLinux(string minerName)
        {
            string executablePath;
            //try local path first
            executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Miners/{0}/{0}", minerName));

            if (!File.Exists(executablePath))
                //try /usr/local/bin
                executablePath = string.Format(@"/usr/local/bin/{0}", minerName);

            if (!File.Exists(executablePath))
                //try /usr/bin
                executablePath = string.Format(@"/usr/bin/{0}", minerName);
            return executablePath;
        }

        private static string GetPathToMinerOnWindows(string minerName, string minerFileName)
        {
            return Path.Combine(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Miners"), minerName), minerFileName + ".exe");
        }
    }
}
