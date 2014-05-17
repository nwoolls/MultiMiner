using MultiMiner.Utility.Net;
using System;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

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

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(machine);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.UploadString(fullUrl, jsonData);
            }
        }
    }
}
