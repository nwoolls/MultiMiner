using System.Net;
using System.Web.Script.Serialization;

namespace MultiMiner.MobileMiner.Api
{
    public class ApiContext
    {
        static public void SubmitMiningStatistics(string url, MiningStatistics miningStatistics)
        {
            string fullUrl = url + "/api/MiningStatisticsInput";
            using (WebClient client = new WebClient())
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(miningStatistics);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.UploadString(fullUrl, jsonData);
            }
        }
    }
}
