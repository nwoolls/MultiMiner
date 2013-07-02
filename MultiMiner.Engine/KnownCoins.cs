using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System.Collections.Generic;
using System.IO;

namespace MultiMiner.Engine
{
    public class KnownCoins
    {
        public KnownCoins()
        {
            this.Coins = new List<CryptoCoin>();
            LoadFromFile();
        }
        
        public void LoadFromFile()
        {
            Coins = ConfigurationReaderWriter.ReadConfiguration<List<CryptoCoin>>(KnownCoinsFileName());
        }

        private static string AppPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
        
        private static string KnownCoinsFileName()
        {
            return Path.Combine(AppPath(), "KnownCoins.xml");
        }
        
        public List<CryptoCoin> Coins { get; set; }
    }
}
