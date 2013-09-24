using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using System;

namespace MultiMiner.MobileMiner.Api
{
    public class ApiContext
    {
        private class ApiWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                //default is 100s - far too long for our API calls
                //if Azure is being flakey we don't want calls taking 100s to timeout
                //lets go with 10s
                w.Timeout = 10 * 1000;
                return w;
            }
        }

        public static void SubmitMiningStatistics(string url, string apiKey, string emailAddress, string applicationKey, string machineName, List<MiningStatistics> miningStatistics)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}MiningStatisticsInput?emailAddress={1}&applicationKey={2}&machineName={3}&apiKey={4}", 
                url, emailAddress, applicationKey, machineName, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(miningStatistics);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.UploadString(fullUrl, jsonData);
            }
        }

        public static List<RemoteCommand> GetCommands(string url, string apiKey, string emailAddress, string applicationKey, string machineName)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}&apiKey={4}", 
                url, emailAddress, applicationKey, machineName, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                string response = client.DownloadString(fullUrl);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<List<RemoteCommand>>(response);
            }
        }

        public static RemoteCommand DeleteCommand(string url, string apiKey, string emailAddress, string applicationKey, string machineName, long commandId)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}&commandId={4}&apiKey={5}",
                url, emailAddress, applicationKey, machineName, commandId, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                string response = client.UploadString(fullUrl, "DELETE", "");
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<RemoteCommand>(response);
            }
        }
    }
}
