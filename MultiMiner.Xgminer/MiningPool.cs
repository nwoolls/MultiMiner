using System;

namespace MultiMiner.Xgminer
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    public class MiningPool
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
