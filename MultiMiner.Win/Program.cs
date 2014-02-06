using MultiMiner.Utility.OS;
using MultiMiner.Win.Data.Configuration;
using MultiMiner.Win.Forms;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    static class Program
    {
        private static string appGuid = "E78D8F73-9241-4BE2-800D-DCD01259BB97";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    PathConfiguration pathConfig = new PathConfiguration();
                    pathConfig.LoadPathConfiguration();
                    ApplicationConfiguration appConfig = new ApplicationConfiguration();
                    appConfig.LoadApplicationConfiguration(pathConfig.SharedConfigPath);
                    if (!appConfig.AllowMultipleInstances)
                        return;
                }

                RunApplication();
            }
        }

        private static void RunApplication()
        {
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                    {
                        foreach (X509ChainStatus status in chain.ChainStatus)
                            if (status.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                                return false;
                    }
                    return true;
                };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MinerForm());
        }
    }
}
