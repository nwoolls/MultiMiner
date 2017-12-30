using System;

namespace MultiMiner.Xgminer.Data
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    public class MiningPool
    {
        public MiningPool()
        {
            //set defaults
            Host = String.Empty;
            Username = String.Empty;
            Password = String.Empty;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Quota { get; set; } //see bfgminer README about quotas
        public bool QuotaEnabled { get; set; }
        public string MinerFlags { get; set; }

        public string BuildPoolUri()
        {
            //trim Host to ensure proper formatting
            //don't just concatenate - we need to support URI paths and #fragments
            string hostText = Host.Trim();

            UriBuilder builder = new UriBuilder(hostText);
            builder.Port = Port;
            string poolUri = builder.Uri
                .ToString()
                .TrimEnd('/');

            // any fragment at this point will be URI encoded - e.g. %23xnsub - but BFGMiner only looks for #
            poolUri = Uri.UnescapeDataString(poolUri);

            return poolUri;
        }
    }
}
