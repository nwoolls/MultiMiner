using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine
{
    public static class DonationPools
    {
        public static void Seed(List<CoinConfiguration> configurations)
        {
            CoinConfiguration donationConfiguration;

            //BTC
            donationConfiguration = CreateCoinConfiguration("BTC", "stratum+tcp://stratum.mining.eligius.st", 3334, "1LKwyLK4KhojsJUEvUx8bEmnmjohNMjRDM");
            configurations.Add(donationConfiguration);


            //LTC
            donationConfiguration = CreateCoinConfiguration("LTC", "stratum+tcp://world.wemineltc.com", 3333);
            configurations.Add(donationConfiguration);

            //BQC
            donationConfiguration = CreateCoinConfiguration("BQC", "http://de1.bigbbq.cc", 8446);
            configurations.Add(donationConfiguration);

            //FTC
            donationConfiguration = CreateCoinConfiguration("FTC", "stratum+tcp://stratum.wemineftc.com", 4444);
            configurations.Add(donationConfiguration);

            //MEC
            donationConfiguration = CreateCoinConfiguration("MEC", "stratum+tcp://us.miningpool.co", 9002);
            configurations.Add(donationConfiguration);

            //PPC
            donationConfiguration = CreateCoinConfiguration("PPC", "stratum+tcp://stratum.d7.lt", 3333);
            configurations.Add(donationConfiguration);

            //NVC
            donationConfiguration = CreateCoinConfiguration("NVC", "stratum+tcp://stratum.khore.org", 3334);
            configurations.Add(donationConfiguration);

            //CAP
            donationConfiguration = CreateCoinConfiguration("CAP", "stratum+tcp://cap.coinmine.pl", 8102);
            configurations.Add(donationConfiguration);

            //ZET
            donationConfiguration = CreateCoinConfiguration("ZET", "stratum+tcp://mine1.coinmine.pl", 6000);
            configurations.Add(donationConfiguration);

            //UNO
            donationConfiguration = CreateCoinConfiguration("UNO", "stratum+tcp://de1.miningpool.co", 10701);
            configurations.Add(donationConfiguration);

            //DOGE
            donationConfiguration = CreateCoinConfiguration("DOGE", "stratum+tcp://stratum.dogehouse.org", 3333);
            configurations.Add(donationConfiguration);

            //DOG
            donationConfiguration = CreateCoinConfiguration("DOG", "stratum+tcp://stratum.dogehouse.org", 3333);
            configurations.Add(donationConfiguration);

            //ASC
            donationConfiguration = CreateCoinConfiguration("ASC", "stratum+tcp://de1.miningpool.co", 10601);
            configurations.Add(donationConfiguration);

            //DGC
            donationConfiguration = CreateCoinConfiguration("DGC", "stratum+tcp://us.miningpool.co", 9102);
            configurations.Add(donationConfiguration);

            //FST
            donationConfiguration = CreateCoinConfiguration("FST", "stratum+tcp://de1.miningpool.co", 9203);
            configurations.Add(donationConfiguration);

            //FRK
            donationConfiguration = CreateCoinConfiguration("FRK", "stratum+tcp://de2.miningpool.co", 4101);
            configurations.Add(donationConfiguration);

            //GLC
            donationConfiguration = CreateCoinConfiguration("GLC", "stratum+tcp://de3.miningpool.co", 3905);
            configurations.Add(donationConfiguration);

            //GDC
            donationConfiguration = CreateCoinConfiguration("GDC", "stratum+tcp://us.miningpool.co", 10001);
            configurations.Add(donationConfiguration);

            //XJO
            donationConfiguration = CreateCoinConfiguration("XJO", "stratum+tcp://de1.miningpool.co", 11001);
            configurations.Add(donationConfiguration);

            //MOON
            donationConfiguration = CreateCoinConfiguration("MOON", "stratum+tcp://ca1.miningpool.co", 9999);
            configurations.Add(donationConfiguration);

            //SBC
            donationConfiguration = CreateCoinConfiguration("SBC", "stratum+tcp://au1.miningpool.co", 9203);
            configurations.Add(donationConfiguration);
        }

        private static CoinConfiguration CreateCoinConfiguration(string coinSymbol, string host, int port, string username = "nwoolls.mmdonations")
        {
            CoinConfiguration donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = coinSymbol;

            MiningPool donationPool = new MiningPool()
            {
                Host = host,
                Port = port,
                Username = username,
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            return donationConfiguration;
        }
    }
}
