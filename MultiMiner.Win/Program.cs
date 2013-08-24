using MultiMiner.Utility;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
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
            Application.Run(new MainForm());
        }
    }
}
