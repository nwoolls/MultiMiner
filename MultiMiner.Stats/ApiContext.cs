using MultiMiner.Utility.Net;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace MultiMiner.Stats
{
    public class ApiContext
    {
        public static void SubmitMinerStatistics(string url, Data.Machine machine)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            string fullUrl = String.Format("{0}machines", url);
            using (WebClient client = new ApiWebClient())
            {
                //specify UTF8 so devices with Unicode characters are posted up properly
                client.Encoding = Encoding.UTF8;
                                
                string jsonData = JsonConvert.SerializeObject(machine);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.UploadString(fullUrl, jsonData);
            }
        }
    }
}
