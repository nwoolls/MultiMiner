using System;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace MultiMiner.Stats.Api
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
                w.Timeout = 10 * 1000;
                return w;
            }
        }

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
