using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Coinchoose.Api
{
    public static class ApiContext
    {
        public static JArray GetCoinInformation()
        {
            WebClient client = new WebClient();
            string s = client.DownloadString("http://www.coinchoose.com/api.php");
            JArray jsonArray = JArray.Parse(s);
            return jsonArray;
        }
    }
}
