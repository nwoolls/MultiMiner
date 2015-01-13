using MultiMiner.Xgminer.Data;
using System.Collections.Generic;
using MultiMiner.Engine.Data.Configuration;

namespace MultiMiner.Engine.Helpers
{
    public static class DonationPools
    {
        public static void Seed(List<Data.Configuration.Coin> configurations)
        {
            //BTC
            Coin donationConfiguration = CreateCoinConfiguration("BTC", "stratum+tcp://stratum.mining.eligius.st", 3334, "1LKwyLK4KhojsJUEvUx8bEmnmjohNMjRDM");
            configurations.Add(donationConfiguration);

            //LTC
            donationConfiguration = CreateCoinConfiguration("LTC", "stratum+tcp://usa.wemineltc.com", 3335);
            configurations.Add(donationConfiguration);

            //BQC
            donationConfiguration = CreateCoinConfiguration("BQC", "stratum+tcp://www.bbqpool.net", 3333);
            configurations.Add(donationConfiguration);

            //FTC
            donationConfiguration = CreateCoinConfiguration("FTC", "stratum+tcp://stratum.wemineftc.com", 4444);
            configurations.Add(donationConfiguration);

            //MEC
            donationConfiguration = CreateCoinConfiguration("MEC", "stratum+tcp://us3.miningpool.co", 4202);
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
            donationConfiguration = CreateCoinConfiguration("DOGE", "stratum+tcp://shibe.dogehouse.org", 3333);
            configurations.Add(donationConfiguration);

            //DOG
            donationConfiguration = CreateCoinConfiguration("DOG", "stratum+tcp://shibe.dogehouse.org", 3333);
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
            
            //SPT
            donationConfiguration = CreateCoinConfiguration("SPT", "stratum+tcp://spt.coinmine.pl", 9108);
            configurations.Add(donationConfiguration);

            //SRC
            donationConfiguration = CreateCoinConfiguration("SRC", "stratum+tcp://mine2.coinmine.pl", 6020);
            configurations.Add(donationConfiguration);

            //WDC
            donationConfiguration = CreateCoinConfiguration("WDC", "stratum+tcp://wdc.coinmine.pl", 9090);
            configurations.Add(donationConfiguration);

            //YBC
            donationConfiguration = CreateCoinConfiguration("YBC", "stratum+tcp://ybc.coinmine.pl", 9104);
            configurations.Add(donationConfiguration);

            //LEAF
            donationConfiguration = CreateCoinConfiguration("LEAF", "stratum+tcp://de3.miningpool.co", 3931);
            configurations.Add(donationConfiguration);

            //CNC
            donationConfiguration = CreateCoinConfiguration("CNC", "stratum+tcp://cnc.blocksolved.com", 3301);
            configurations.Add(donationConfiguration);

            //BEC
            donationConfiguration = CreateCoinConfiguration("BEC", "stratum+tcp://ca1.miningpool.co", 20001);
            configurations.Add(donationConfiguration);

            //BIC
            donationConfiguration = CreateCoinConfiguration("BIC", "stratum+tcp://de2.miningpool.co", 3388);
            configurations.Add(donationConfiguration);

            //DGB
            donationConfiguration = CreateCoinConfiguration("DGB", "stratum+tcp://de2.miningpool.co", 3399);
            configurations.Add(donationConfiguration);

            //ICO
            donationConfiguration = CreateCoinConfiguration("ICO", "stratum+tcp://de2.miningpool.co", 3391);
            configurations.Add(donationConfiguration);

            //KDC
            donationConfiguration = CreateCoinConfiguration("KDC", "stratum+tcp://de3.miningpool.co", 3902);
            configurations.Add(donationConfiguration);

            //MINT
            donationConfiguration = CreateCoinConfiguration("MINT", "stratum+tcp://de3.miningpool.co", 3356);
            configurations.Add(donationConfiguration);

            //MRY
            donationConfiguration = CreateCoinConfiguration("MRY", "stratum+tcp://ca1.miningpool.co", 14322);
            configurations.Add(donationConfiguration);

            //PLT
            donationConfiguration = CreateCoinConfiguration("PLT", "stratum+tcp://de2.miningpool.co", 9501);
            configurations.Add(donationConfiguration);

            //SMC
            donationConfiguration = CreateCoinConfiguration("SMC", "stratum+tcp://de3.miningpool.co", 9057);
            configurations.Add(donationConfiguration);

            //TEA
            donationConfiguration = CreateCoinConfiguration("TEA", "stratum+tcp://de3.miningpool.co", 3921);
            configurations.Add(donationConfiguration);

            //NEC
            donationConfiguration = CreateCoinConfiguration("NEC", "stratum+tcp://de1.miningpool.co", 19664);
            configurations.Add(donationConfiguration);

            //TAG
            donationConfiguration = CreateCoinConfiguration("TAG", "stratum+tcp://tag.hashfaster.com", 3335);
            configurations.Add(donationConfiguration);

            //BEE
            donationConfiguration = CreateCoinConfiguration("BEE", "stratum+tcp://ca1.miningpool.co", 14804);
            configurations.Add(donationConfiguration);
            
            //AIR
            donationConfiguration = CreateCoinConfiguration("AIR", "stratum+tcp://us4.miningpool.co", 4252);
            configurations.Add(donationConfiguration);

            //888
            donationConfiguration = CreateCoinConfiguration("888", "stratum+tcp://ca1.miningpool.co", 8888);
            configurations.Add(donationConfiguration);

            //GLD
            donationConfiguration = CreateCoinConfiguration("GLD", "http://gld.minepool.net", 3333);
            configurations.Add(donationConfiguration);

            //RDD
            donationConfiguration = CreateCoinConfiguration("RDD", "stratum+tcp://redd.oakpool.com", 3333);
            configurations.Add(donationConfiguration);

            //EMC2
            donationConfiguration = CreateCoinConfiguration("EMC2", "stratum+tcp://emc2.cryptopools.com", 3006);
            configurations.Add(donationConfiguration);

            //MEOW
            donationConfiguration = CreateCoinConfiguration("MEOW", "stratum+tcp://stratum-eu.meow.luckyminers.com", 3316);
            configurations.Add(donationConfiguration);

            //HBN
            donationConfiguration = CreateCoinConfiguration("HBN", "stratum+tcp://hbn.stratum.smartmining.net", 17373);
            configurations.Add(donationConfiguration);

            //EZC
            donationConfiguration = CreateCoinConfiguration("EZC", "stratum+tcp://mp.ultimatecoinpool.com", 6000);
            configurations.Add(donationConfiguration);

            //ELC
            donationConfiguration = CreateCoinConfiguration("ELC", "stratum+tcp://elc.blocksolved.com", 3304);
            configurations.Add(donationConfiguration);

            //EAC
            donationConfiguration = CreateCoinConfiguration("EAC", "stratum+tcp://us.lifeforce.info", 9090);
            configurations.Add(donationConfiguration);

            //CARB
            donationConfiguration = CreateCoinConfiguration("CARB", "stratum+tcp://us4.miningpool.co", 15615);
            configurations.Add(donationConfiguration);

            //MZC
            donationConfiguration = CreateCoinConfiguration("MZC", "stratum+tcp://de3.miningpool.co", 18383);
            configurations.Add(donationConfiguration);

            //SPC
            donationConfiguration = CreateCoinConfiguration("SPC", "stratum+tcp://us4.miningpool.co", 18665);
            configurations.Add(donationConfiguration);

            //KGC
            donationConfiguration = CreateCoinConfiguration("KGC", "stratum+tcp://mining.botpool.net", 5888);
            configurations.Add(donationConfiguration);

            //CAI
            donationConfiguration = CreateCoinConfiguration("CAI", "stratum+tcp://caishen.nitro.org", 8888);
            configurations.Add(donationConfiguration);

            //BTB
            donationConfiguration = CreateCoinConfiguration("BTB", "stratum+tcp://btb.scrypthp.com", 3333);
            configurations.Add(donationConfiguration);

            //VTC
            donationConfiguration = CreateCoinConfiguration("VTC", "stratum+tcp://pool.verters.com", 4444);
            configurations.Add(donationConfiguration);

            //DRK
            donationConfiguration = CreateCoinConfiguration("DRK", "stratum+tcp://us-east1.darkcoin.miningpoolhub.com", 20465);
            configurations.Add(donationConfiguration);

            //GPUC
            donationConfiguration = CreateCoinConfiguration("GPUC", "stratum+tcp://gpu-stratum.hashfever.com", 3262);
            configurations.Add(donationConfiguration);

            //EXE
            donationConfiguration = CreateCoinConfiguration("EXE", "stratum+tcp://stratum.execoin.net", 3333);
            configurations.Add(donationConfiguration);

            //SPA
            donationConfiguration = CreateCoinConfiguration("SPA", "stratum+tcp://east1.us.stratum.dedicatedpool.com", 3372);
            configurations.Add(donationConfiguration);

            //HIRO
            donationConfiguration = CreateCoinConfiguration("HIRO", "stratum+tcp://stratum.forkpool.com", 6347);
            configurations.Add(donationConfiguration);

            //MAX
            donationConfiguration = CreateCoinConfiguration("MAX", "stratum+tcp://us-east1.maxcoin.miningpoolhub.com", 20461);
            configurations.Add(donationConfiguration);

            //Bitmark
            donationConfiguration = CreateCoinConfiguration("BTM", "stratum+tcp://de3.miningpool.co", 22011);
            configurations.Add(donationConfiguration);

            //Cannabiscoin
            donationConfiguration = CreateCoinConfiguration("CANN", "stratum+tcp://de2.miningpool.co", 42000);
            configurations.Add(donationConfiguration);

            //Syscoin
            donationConfiguration = CreateCoinConfiguration("SYS", "stratum+tcp://de2.miningpool.co", 22222);
            configurations.Add(donationConfiguration);

            //Terracoin
            donationConfiguration = CreateCoinConfiguration("TRC", "stratum+tcp://trc.coin-pool.org", 3340);
            configurations.Add(donationConfiguration);

            //eMark
            donationConfiguration = CreateCoinConfiguration("DEM", "stratum+tcp://dem.coin-pool.org", 3453);
            configurations.Add(donationConfiguration);

            //TEKcoin
            donationConfiguration = CreateCoinConfiguration("TEK", "stratum+tcp://pool.dsync.net", 6666);
            configurations.Add(donationConfiguration);

            //Takcoin
            donationConfiguration = CreateCoinConfiguration("TAK", "stratum+tcp://tak.minebig.com", 3339);
            configurations.Add(donationConfiguration);

            //YAC
            donationConfiguration = CreateCoinConfiguration("YAC", "stratum+tcp://yac.coinmine.pl", 9088);
            configurations.Add(donationConfiguration);

            //GRS
            donationConfiguration = CreateCoinConfiguration("GRS", "stratum+tcp://grs.suprnova.cc", 5544);
            configurations.Add(donationConfiguration);

            //NiceHash / WestHash
            //Scrypt
            donationConfiguration = CreateCoinConfiguration("NiceHash:Scrypt", "stratum+tcp://stratum.westhash.com/#xnsub", 3333, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //SHA256
            donationConfiguration = CreateCoinConfiguration("NiceHash:SHA256", "stratum+tcp://stratum.westhash.com/#xnsub", 3334, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //ScryptN
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptN", "stratum+tcp://stratum.westhash.com/#xnsub", 3335, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X11
            donationConfiguration = CreateCoinConfiguration("NiceHash:X11", "stratum+tcp://stratum.westhash.com/#xnsub", 3365, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X13
            donationConfiguration = CreateCoinConfiguration("NiceHash:X13", "stratum+tcp://stratum.westhash.com/#xnsub", 3337, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Keccak
            donationConfiguration = CreateCoinConfiguration("NiceHash:Keccak", "stratum+tcp://stratum.westhash.com/#xnsub", 3338, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X15
            donationConfiguration = CreateCoinConfiguration("NiceHash:X15", "stratum+tcp://stratum.westhash.com/#xnsub", 3339, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //TIT
            donationConfiguration = CreateCoinConfiguration("TIT", "stratum+tcp://titcoin.slugmonkeypool.net", 3336);
            configurations.Add(donationConfiguration);

            //ANC
            donationConfiguration = CreateCoinConfiguration("ANC", "stratum+tcp://anc.fusionhash.com", 3337);
            configurations.Add(donationConfiguration);

            //SXC
            donationConfiguration = CreateCoinConfiguration("SXC", "stratum+tcp://us-east1.sexcoin.miningpoolhub.com", 20463);
            configurations.Add(donationConfiguration);

            //EMD
            donationConfiguration = CreateCoinConfiguration("EMD", "stratum+tcp://us.lifeforce.info", 9090);
            configurations.Add(donationConfiguration);

            //42
            donationConfiguration = CreateCoinConfiguration("42", "stratum+tcp://42.mastermining.net", 4242);
            configurations.Add(donationConfiguration);

            //ACOIN
            donationConfiguration = CreateCoinConfiguration("ACOIN", "stratum+tcp://stratum.coin-miners.info", 3522);
            configurations.Add(donationConfiguration);

            //BCX
            donationConfiguration = CreateCoinConfiguration("BCX", "stratum+tcp://bcx.coin-pool.org", 3332);
            configurations.Add(donationConfiguration);

            //MYR
            donationConfiguration = CreateCoinConfiguration("MYR", "stratum+tcp://mine1.myr.nonce-pool.com", 3360);
            configurations.Add(donationConfiguration);

            //QRK
            donationConfiguration = CreateCoinConfiguration("QRK", "stratum+tcp://mine1coinmine.pl", 6010);
            configurations.Add(donationConfiguration);
        }

        private static Data.Configuration.Coin CreateCoinConfiguration(string coinSymbol, string host, int port, string username = "nwoolls.mmdonations")
        {
            Data.Configuration.Coin donationConfiguration = new Data.Configuration.Coin();
            donationConfiguration.PoolGroup.Id = coinSymbol;

            MiningPool donationPool = new MiningPool()
            {
                Host = host + "/#skipcbcheck",
                Port = port,
                Username = username,
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            return donationConfiguration;
        }
    }
}
