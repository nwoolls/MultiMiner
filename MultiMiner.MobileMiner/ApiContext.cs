using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using System;
using System.Text;
using System.Threading;

namespace MultiMiner.MobileMiner
{
    public class ApiContext
    {
        private class ApiWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                //default is 100s - far too long for our API calls
                //if API is being flakey we don't want calls taking 100s to timeout
                //lets go with 10s
                
#if !DEBUG
                w.Timeout = 10 * 1000;
#endif
                return w;
            }
        }

        public static void SubmitMiningStatistics(string url, string apiKey, string emailAddress, string applicationKey, List<Data.MiningStatistics> miningStatistics)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}MiningStatisticsInput?emailAddress={1}&applicationKey={2}&apiKey={3}", 
                url, emailAddress, applicationKey, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                //specify UTF8 so devices with Unicode characters are posted up properly
                client.Encoding = Encoding.UTF8;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(miningStatistics);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";


                ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });

            }
        }

        private static string ExecuteWebAction(Func<string> action)
        {
            string response = String.Empty;

            try
            {
                response = action();
            }
            catch (WebException ex)
            {
                if ((ex.Status == WebExceptionStatus.ProtocolError) &&
                    (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadGateway))
                {
                    //retry 1 time for error 502 Invalid Gateway
                    Thread.Sleep(750);
                    response = action();
                }
                else
                    throw;
            }

            return response;
        }

        public static void SubmitNotifications(string url, string apiKey, string emailAddress, string applicationKey, List<Data.Notification> notifications)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}NotificationsInput?emailAddress={1}&applicationKey={2}&apiKey={3}&detailed=true",
                url, emailAddress, applicationKey, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(notifications);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });
            }
        }

        public static List<Data.RemoteCommand> GetCommands(string url, string apiKey, string emailAddress, string applicationKey, string machineName)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}&apiKey={4}", 
                url, emailAddress, applicationKey, machineName, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                string response = ExecuteWebAction(() =>
                {
                    return client.DownloadString(fullUrl);
                });

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<List<Data.RemoteCommand>>(response);
            }
        }

        public static Data.RemoteCommand DeleteCommand(string url, string apiKey, string emailAddress, string applicationKey, string machineName, long commandId)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}RemoteCommands?emailAddress={1}&applicationKey={2}&machineName={3}&commandId={4}&apiKey={5}",
                url, emailAddress, applicationKey, machineName, commandId, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                string response = ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, "DELETE", "");
                });

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<Data.RemoteCommand>(response);
            }
        }

        public static void SubmitMachinePools(string url, string apiKey, string emailAddress, string applicationKey, 
            string machineName, List<string> pools)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}PoolsInput?emailAddress={1}&applicationKey={2}&apiKey={3}&machineName={4}",
                url, emailAddress, applicationKey, apiKey, machineName);
            using (WebClient client = new ApiWebClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(pools);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });
            }
        }
    }
}
