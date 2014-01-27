using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine
{
    public static class DonationPools
    {
        public static void Seed(List<CoinConfiguration> configurations)
        {
            //BTC
            CoinConfiguration donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "BTC";

            MiningPool donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.mining.eligius.st",
                Port = 3334,
                Username = "1LKwyLK4KhojsJUEvUx8bEmnmjohNMjRDM",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://mint.bitminter.com",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.bitcoin.cz",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //LTC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "LTC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://world.wemineltc.com",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://primary.coinhuntr.com",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //BQC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "BQC";

            donationPool = new MiningPool()
            {
                Host = "http://de1.bigbbq.cc",
                Port = 8446,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //FTC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "FTC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.wemineftc.com",
                Port = 4444,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "http://feathercoin.is-a-geek.com",
                Port = 8341,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //MEC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "MEC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://us.miningpool.co",
                Port = 9002,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://au1.miningpool.co",
                Port = 9001,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //PPC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "PPC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.d7.lt",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            //NVC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "NVC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.khore.org",
                Port = 3334,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //CAP
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "CAP";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://cap.coinmine.pl",
                Port = 8102,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //ZET
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "ZET";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://mine1.coinmine.pl",
                Port = 6000,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //UNO
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "UNO";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de1.miningpool.co",
                Port = 10701,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //DOGE
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "DOGE";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.dogehouse.org",
                Port = 3333,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //DOG
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "DOG";
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //ASC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "ASC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de1.miningpool.co",
                Port = 10601,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //DGC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "DGC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://us.miningpool.co",
                Port = 9102,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //FST
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "FST";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de1.miningpool.co",
                Port = 9203,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //FRK
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "FRK";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de2.miningpool.co",
                Port = 4101,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //GLC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "GLC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de3.miningpool.co",
                Port = 3905,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //GDC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "GDC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://us.miningpool.co",
                Port = 10001,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //XJO
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "XJO";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://de1.miningpool.co",
                Port = 11001,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //MOON
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "MOON";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://ca1.miningpool.co",
                Port = 9999,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);

            //SBC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "SBC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://au1.miningpool.co",
                Port = 9203,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            configurations.Add(donationConfiguration);
        }
    }
}
