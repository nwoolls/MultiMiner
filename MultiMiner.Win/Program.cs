using MultiMiner.Utility.OS;
using MultiMiner.UX.Data.Configuration;
using MultiMiner.Win.Forms;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

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
                    Paths pathConfig = new Paths();
                    pathConfig.LoadPathConfiguration();
                    UX.Data.Configuration.Application appConfig = new UX.Data.Configuration.Application();
                    appConfig.LoadApplicationConfiguration(pathConfig.SharedConfigPath);
                    if (!appConfig.AllowMultipleInstances)
                        return;
                }

                RunApplication();
            }
        }

        private static void RunApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            

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

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new MinerForm());
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("Application Error", (e.ExceptionObject as Exception).ToString(), EventLogEntryType.Error, 1000);
        }
    }
}
