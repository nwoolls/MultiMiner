using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine
{
    public class KnownCoins
    {
        public KnownCoins()
        {
            this.Coins = new List<CryptoCoin>();
            SeedKnownCoins();
        }

        private void SeedKnownCoins()
        {
            CryptoCoin coin = new CryptoCoin() 
            { 
                Name = "Bitcoin", 
                Symbol = "BTC", 
                Algorithm = CoinAlgorithm.SHA256 
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "Litecoin",
                Symbol = "LTC",
                Algorithm = CoinAlgorithm.Scrypt
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "Novacoin",
                Symbol = "NVC",
                Algorithm = CoinAlgorithm.Scrypt
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "Feathercoin",
                Symbol = "FTC",
                Algorithm = CoinAlgorithm.Scrypt
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "Megacoin",
                Symbol = "MEC",
                Algorithm = CoinAlgorithm.Scrypt
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "BBQCoin",
                Symbol = "BQC",
                Algorithm = CoinAlgorithm.Scrypt
            };

            this.Coins.Add(coin);

            coin = new CryptoCoin()
            {
                Name = "Terracoin",
                Symbol = "TRC",
                Algorithm = CoinAlgorithm.SHA256
            };

            this.Coins.Add(coin);
        }

        public List<CryptoCoin> Coins { get; set; }
    }
}
