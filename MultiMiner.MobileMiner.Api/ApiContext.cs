using System.Net;
using System.Xml.Serialization;

namespace MultiMiner.MobileMiner.Api
{
    public class ApiContext
    {
        ////User
        //public virtual string EmailAddress { get; set; }
        //public virtual string ApplicationKey { get; set; }

        static public void SubmitMiningStatistics(string url, MiningStatistics miningStatistics)
        {
            // http://local_ip/api/accounts


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.ContentLength = 800;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(MiningStatistics));
            xmlSerializer.Serialize(request.GetRequestStream(), miningStatistics);

            var response = (HttpWebResponse)request.GetResponse();

            XmlSerializer serializer = new XmlSerializer(typeof(MiningStatistics));

            var newRecord = (MiningStatistics)serializer.Deserialize(response.GetResponseStream());

        }
    }
}
