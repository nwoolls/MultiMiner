using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using System;

namespace MultiMiner.MobileMiner.Api
{
    public class ApiContext
    {
        public static void SubmitMiningStatistics(string url, List<MiningStatistics> miningStatistics)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = url + "api/MiningStatisticsInput";
            using (WebClient client = new WebClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(miningStatistics);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.UploadString(fullUrl, jsonData);
            }
        }

        public static List<RemoteCommand> GetCommands(string url, string emailAddress, string applicationKey, string machineName)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}api/RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}", 
                url, emailAddress, applicationKey, machineName);
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(fullUrl);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<List<RemoteCommand>>(response);
            }
        }

        public static RemoteCommand DeleteCommand(string url, string emailAddress, string applicationKey, string machineName, long commandId)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}api/RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}&commandId={4}",
                url, emailAddress, applicationKey, machineName, commandId);
            using (WebClient client = new WebClient())
            {
                string response = client.UploadString(fullUrl, "DELETE", "");
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<RemoteCommand>(response);
            }
        }
    }
}
