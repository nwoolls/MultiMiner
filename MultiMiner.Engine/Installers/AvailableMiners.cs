using MultiMiner.Engine.Data;
using MultiMiner.Utility.Net;
using MultiMiner.Utility.OS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Engine.Installers
{
    public static class AvailableMiners
    {
        public static List<AvailableMiner> GetAvailableMiners(string userAgent)
        {
            WebClient webClient = new ApiWebClient();
            webClient.Headers.Add("user-agent", userAgent);

            string platform = "win32";
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
                platform = "osx64";
            
            //include www. to avoid redirect
            string url = "http://www.multiminerapp.com/miners?platform=" + platform;
            string response = webClient.DownloadString(new Uri(url));

            List<AvailableMiner> availableMiners = JsonConvert.DeserializeObject<List<AvailableMiner>>(response);
            return availableMiners;
        }
    }
}
