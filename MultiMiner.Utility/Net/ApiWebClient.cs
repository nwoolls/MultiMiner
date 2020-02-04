using System;
using System.Net;
using System.Threading;

namespace MultiMiner.Utility.Net
{
    public class ApiWebClient : WebClient
    {
        public ApiWebClient()
        {
            // enable TLS 1.1 and 1.2 - they are not enabled by default and required for the Blockchain.info API
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12
                    | SecurityProtocolType.Ssl3;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            //default is 100s - far too long for our API calls
            //if API is being flakey we don't want calls taking 100s to timeout
            //lets go with 10s
            w.Timeout = 10 * 1000;
            return w;
        }

        public string DownloadFlakyString(string address)
        {
            return DownloadFlakyString(new Uri(address));
        }

        public string DownloadFlakyString(Uri address)
        {
            string response;

            try
            {
                response = DownloadString(address);
            }
            catch (WebException)
            {
                //try again 1 more time if WebException
                Thread.Sleep(750);
                response = DownloadString(address);
            }

            return response;
        }
    }
}
