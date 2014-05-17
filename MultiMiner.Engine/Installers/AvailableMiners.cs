using MultiMiner.Engine.Data;
using MultiMiner.Utility.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MultiMiner.Engine.Installers
{
    public static class AvailableMiners
    {
        public static List<AvailableMiner> GetAvailableMiners()
        {
            WebClient webClient = new ApiWebClient();

            string hash = GetSimpleDeterrent();
            //include www. to avoid redirect
            string url = "http://www.multiminerapp.com/miners?id=" + Uri.EscapeDataString(hash);
            string response = webClient.DownloadString(new Uri(url));

            List<AvailableMiner> availableMiners = JsonConvert.DeserializeObject<List<AvailableMiner>>(response);
            return availableMiners;
        }

        private static string GetSimpleDeterrent()
        {
            DateTime today = DateTime.UtcNow.Date;
            string todayString = today.ToString("MM/dd/yyyy");
            int dayOfWeek = (int)today.DayOfWeek;

            HashAlgorithm algo;

            if (dayOfWeek >= 3)
                algo = SHA1.Create();
            else
                algo = MD5.Create();

            string hash = Convert.ToBase64String(algo.ComputeHash(Encoding.UTF8.GetBytes(todayString)));
            return hash;
        }

    }
}
