using MultiMiner.Utility;
using System;
using System.IO;

namespace MultiMiner.Engine
{
    public static class MinerPath
    {
        private const string BfgminerName = "bfgminer";

        public static string GetPathToInstalledMiner()
        {
            string executablePath = string.Empty;
            string minerName = GetMinerName();

            switch (OSVersionPlatform.GetConcretePlatform())
            {
                case PlatformID.MacOSX:
                    //try local path first
                    executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Miners/{0}/bin/{0}", minerName));
                                        
                    if (!File.Exists(executablePath))
                        //try global path (Homebrew)
                        executablePath = string.Format(@"/usr/local/bin/{0}", minerName);

                    break;

                //support Unix - there is no bin folder for the executables like on Mac OS X
                case PlatformID.Unix:
                    //try local path first
                    executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Miners/{0}/{0}", minerName));

                    if (!File.Exists(executablePath))
                        //try /usr/local/bin
                        executablePath = string.Format(@"/usr/local/bin/{0}", minerName);

                    if (!File.Exists(executablePath))
                        //try /usr/bin
                        executablePath = string.Format(@"/usr/bin/{0}", minerName);

                    break;

                default:
                    executablePath = string.Format(@"Miners\{0}\{0}.exe", minerName);
                    break;
            }

            return executablePath;
        }

        public static string GetMinerName()
        {
            return BfgminerName;
        }
    }
}
