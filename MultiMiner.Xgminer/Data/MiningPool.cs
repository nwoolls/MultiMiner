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
    }
}
