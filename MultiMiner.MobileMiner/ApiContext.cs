using System.Collections.Generic;
using System.Net;
using System;
using System.Text;
using System.Threading;
using MultiMiner.Utility.Net;
using Newtonsoft.Json;

namespace MultiMiner.MobileMiner
{
    public class ApiContext
    {
        public static List<Data.RemoteCommand> SubmitMiningStatistics(string url, string apiKey, string emailAddress, string applicationKey, 
            List<Data.MiningStatistics> miningStatistics, bool fetchCommands)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}MiningStatisticsInput?emailAddress={1}&applicationKey={2}&apiKey={3}&fetchCommands={4}", 
                url, emailAddress, applicationKey, apiKey, fetchCommands);
            using (WebClient client = new ApiWebClient())
            {
                //specify UTF8 so devices with Unicode characters are posted up properly
                client.Encoding = Encoding.UTF8;

                string jsonData = JsonConvert.SerializeObject(miningStatistics);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                string response = ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });

                return JsonConvert.DeserializeObject<List<Data.RemoteCommand>>(response);
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
                string jsonData = JsonConvert.SerializeObject(notifications);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });
            }
        }

        public static List<Data.RemoteCommand> GetCommands(string url, string apiKey, string emailAddress, string applicationKey, List<string> machineNames)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            
            string jsonData = JsonConvert.SerializeObject(machineNames);

            string fullUrl = String.Format("{0}RemoteCommands?emailAddress={1}&applicationKey={2}&apiKey={4}",
                url, emailAddress, applicationKey, jsonData, apiKey);

            using (WebClient client = new ApiWebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                string response = ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });

                return JsonConvert.DeserializeObject<List<Data.RemoteCommand>>(response);
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

                return JsonConvert.DeserializeObject<Data.RemoteCommand>(response);
            }
        }

        public static void SubmitMachinePools(string url, string apiKey, string emailAddress, string applicationKey,
            Dictionary<string, List<string>> machinePools)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}PoolsInput?emailAddress={1}&applicationKey={2}&apiKey={3}",
                url, emailAddress, applicationKey, apiKey);
            using (WebClient client = new ApiWebClient())
            {
                string jsonData = JsonConvert.SerializeObject(machinePools);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                ExecuteWebAction(() =>
                {
                    return client.UploadString(fullUrl, jsonData);
                });
            }
        }
    }
}
