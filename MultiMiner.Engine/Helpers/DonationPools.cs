using MultiMiner.Xgminer.Data;
using System.Collections.Generic;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Utility.OS;
using System;

namespace MultiMiner.Engine.Helpers
{
    public static class DonationPools
    {
        public static void Seed(List<Data.Configuration.Coin> configurations)
        {
            //BTC
            Coin donationConfiguration = CreateCoinConfiguration("BTC", "stratum+tcp://connect.pool.bitcoin.com", 3333);
            configurations.Add(donationConfiguration);

            //LTC
            donationConfiguration = CreateCoinConfiguration("LTC", "stratum+tcp://us.litecoinpool.org", 3333, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //BQC
            donationConfiguration = CreateCoinConfiguration("BQC", "stratum+tcp://www.bbqpool.net", 3333);
            configurations.Add(donationConfiguration);

            //Coinmine.pl
            //ZET
            donationConfiguration = CreateCoinConfiguration("ZET", "stratum+tcp://mine1.coinmine.pl", 6000, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //SRC
            donationConfiguration = CreateCoinConfiguration("SRC", "stratum+tcp://mine2.coinmine.pl", 6020, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //DCR
            donationConfiguration = CreateCoinConfiguration("DCR", "stratum+tcp://dcr.coinmine.pl", 2222, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //LBC
            donationConfiguration = CreateCoinConfiguration("LBC", "stratum+tcp://lbc.coinmine.pl", 8787, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //PPC
            donationConfiguration = CreateCoinConfiguration("PPC", "stratum+tcp://mine1.coinmine.pl", 6050, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //UBQ
            donationConfiguration = CreateCoinConfiguration("UBQ", "stratum+tcp://ubiq.mixpools.org", 2120, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //MiningPoolHub
            //DRK
            donationConfiguration = CreateCoinConfiguration("DRK", "stratum+tcp://us-east1.darkcoin.miningpoolhub.com", 20465);
            configurations.Add(donationConfiguration);

            //MAX
            donationConfiguration = CreateCoinConfiguration("MAX", "stratum+tcp://us-east1.maxcoin.miningpoolhub.com", 20461);
            configurations.Add(donationConfiguration);

            //SXC
            donationConfiguration = CreateCoinConfiguration("SXC", "stratum+tcp://us-east1.sexcoin.miningpoolhub.com", 20463);
            configurations.Add(donationConfiguration);

            //ETH
            donationConfiguration = CreateCoinConfiguration("ETH", "stratum+tcp://us-east.ethash-hub.miningpoolhub.com", 20535);
            configurations.Add(donationConfiguration);

            //ADZ
            donationConfiguration = CreateCoinConfiguration("ADZ", "stratum+tcp://hub.miningpoolhub.com", 20529);
            configurations.Add(donationConfiguration);

            //AUR
            donationConfiguration = CreateCoinConfiguration("AUR", "stratum+tcp://hub.miningpoolhub.com", 20592);
            configurations.Add(donationConfiguration);

            //BTG
            donationConfiguration = CreateCoinConfiguration("BTG", "stratum+tcp://us-east.equihash-hub.miningpoolhub.com", 20595);
            configurations.Add(donationConfiguration);

            //DASH
            donationConfiguration = CreateCoinConfiguration("DASH", "stratum+tcp://hub.miningpoolhub.com", 20465);
            configurations.Add(donationConfiguration);

            //ETN
            donationConfiguration = CreateCoinConfiguration("ETN", "stratum+tcp://us-east.cryptonight-hub.miningpoolhub.com", 20596);
            configurations.Add(donationConfiguration);

            //ETC
            donationConfiguration = CreateCoinConfiguration("ETC", "stratum+tcp://us-east.ethash-hub.miningpoolhub.com", 20555);
            configurations.Add(donationConfiguration);

            //EXP
            donationConfiguration = CreateCoinConfiguration("EXP", "stratum+tcp://us-east.ethash-hub.miningpoolhub.com", 20565);
            configurations.Add(donationConfiguration);

            //FTC
            donationConfiguration = CreateCoinConfiguration("FTC", "stratum+tcp://hub.miningpoolhub.com", 20510);
            configurations.Add(donationConfiguration);

            //GAME
            donationConfiguration = CreateCoinConfiguration("GAME", "stratum+tcp://hub.miningpoolhub.com", 20576);
            configurations.Add(donationConfiguration);

            //GEO
            donationConfiguration = CreateCoinConfiguration("GEO", "stratum+tcp://hub.miningpoolhub.com", 20524);
            configurations.Add(donationConfiguration);

            //BSTY
            donationConfiguration = CreateCoinConfiguration("BSTY", "stratum+tcp://hub.miningpoolhub.com", 20543);
            configurations.Add(donationConfiguration);

            //GRS
            donationConfiguration = CreateCoinConfiguration("GRS", "stratum+tcp://hub.miningpoolhub.com", 20486);
            configurations.Add(donationConfiguration);

            //GRS
            donationConfiguration = CreateCoinConfiguration("GRS", "stratum+tcp://hub.miningpoolhub.com", 20486);
            configurations.Add(donationConfiguration);

            //MONA
            donationConfiguration = CreateCoinConfiguration("MONA", "stratum+tcp://hub.miningpoolhub.com", 20593);
            configurations.Add(donationConfiguration);

            //XMR
            donationConfiguration = CreateCoinConfiguration("XMR", "stratum+tcp://us-east.cryptonight-hub.miningpoolhub.com", 20580);
            configurations.Add(donationConfiguration);

            //MUSIC
            donationConfiguration = CreateCoinConfiguration("MUSIC", "stratum+tcp://us-east.ethash-hub.miningpoolhub.com", 20585);
            configurations.Add(donationConfiguration);

            //SC
            donationConfiguration = CreateCoinConfiguration("SC", "stratum+tcp://hub.miningpoolhub.com", 20550);
            configurations.Add(donationConfiguration);

            //START
            donationConfiguration = CreateCoinConfiguration("START", "stratum+tcp://hub.miningpoolhub.com", 20509);
            configurations.Add(donationConfiguration);

            //XVG
            donationConfiguration = CreateCoinConfiguration("XVG", "stratum+tcp://hub.miningpoolhub.com", 20523);
            configurations.Add(donationConfiguration);

            //VTC
            donationConfiguration = CreateCoinConfiguration("VTC", "stratum+tcp://hub.miningpoolhub.com", 20507);
            configurations.Add(donationConfiguration);

            //ZEC
            donationConfiguration = CreateCoinConfiguration("ZEC", "stratum+tcp://us-east.equihash-hub.miningpoolhub.com", 20570);
            configurations.Add(donationConfiguration);

            //ZCL
            donationConfiguration = CreateCoinConfiguration("ZCL", "stratum+tcp://us-east.equihash-hub.miningpoolhub.com", 20575);
            configurations.Add(donationConfiguration);

            //XZC
            donationConfiguration = CreateCoinConfiguration("XZC", "stratum+tcp://us-east.lyra2z-hub.miningpoolhub.com", 20581);
            configurations.Add(donationConfiguration);

            //ZEN
            donationConfiguration = CreateCoinConfiguration("ZEN", "stratum+tcp://us-east.equihash-hub.miningpoolhub.com", 20594);
            configurations.Add(donationConfiguration);

            //NiceHash
            //Scrypt
            donationConfiguration = CreateCoinConfiguration("NiceHash:Scrypt", "stratum+tcp://scrypt.usa.nicehash.com/#xnsub", 3333, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //SHA256
            donationConfiguration = CreateCoinConfiguration("NiceHash:SHA256", "stratum+tcp://sha256.usa.nicehash.com/#xnsub", 3334, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //ScryptN
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptN", "stratum+tcp://scryptnf.usa.nicehash.com/#xnsub", 3335, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //ScryptNf
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptNf", "stratum+tcp://scryptnf.usa.nicehash.com/#xnsub", 3335, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X11
            donationConfiguration = CreateCoinConfiguration("NiceHash:X11", "stratum+tcp://x11.usa.nicehash.com/#xnsub", 3336, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X13
            donationConfiguration = CreateCoinConfiguration("NiceHash:X13", "stratum+tcp://x13.usa.nicehash.com/#xnsub", 3337, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Keccak
            donationConfiguration = CreateCoinConfiguration("NiceHash:Keccak", "stratum+tcp://keccak.usa.nicehash.com/#xnsub", 3338, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X15
            donationConfiguration = CreateCoinConfiguration("NiceHash:X15", "stratum+tcp://x15.usa.nicehash.com/#xnsub", 3339, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Nist5
            donationConfiguration = CreateCoinConfiguration("NiceHash:Nist5", "stratum+tcp://nist5.usa.nicehash.com/#xnsub", 3340, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //NeoScrypt
            donationConfiguration = CreateCoinConfiguration("NiceHash:NeoScrypt", "stratum+tcp://neoscrypt.usa.nicehash.com/#xnsub", 3341, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Lyra2RE
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lyra2RE", "stratum+tcp://lyra2re.usa.nicehash.com/#xnsub", 3342, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //WhirlpoolX
            donationConfiguration = CreateCoinConfiguration("NiceHash:WhirlpoolX", "stratum+tcp://whirlpoolx.usa.nicehash.com/#xnsub", 3343, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Qubit
            donationConfiguration = CreateCoinConfiguration("NiceHash:Qubit", "stratum+tcp://qubit.usa.nicehash.com/#xnsub", 3344, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Quark
            donationConfiguration = CreateCoinConfiguration("NiceHash:Quark", "stratum+tcp://quark.usa.nicehash.com/#xnsub", 3345, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Axiom
            donationConfiguration = CreateCoinConfiguration("NiceHash:Axiom", "stratum+tcp://axiom.usa.nicehash.com/#xnsub", 3346, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Lyra2REv2
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lyra2REv2", "stratum+tcp://lyra2rev2.usa.nicehash.com/#xnsub", 3347, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //ScryptJaneNf16
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptJaneNf16", "stratum+tcp://scryptjanenf16.usa.nicehash.com/#xnsub", 3348, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Blake256r8
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r8", "stratum+tcp://blake256r8.usa.nicehash.com/#xnsub", 3349, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Blake256r14
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r14", "stratum+tcp://blake256r14.usa.nicehash.com/#xnsub", 3350, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Blake256r8vnl
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r8vnl", "stratum+tcp://blake256r8vnl.usa.nicehash.com/#xnsub", 3351, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Hodl
            donationConfiguration = CreateCoinConfiguration("NiceHash:Hodl", "stratum+tcp://hodl.usa.nicehash.com/#xnsub", 3352, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Hodl
            donationConfiguration = CreateCoinConfiguration("NiceHash:Hodl", "stratum+tcp://hodl.usa.nicehash.com/#xnsub", 3352, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //DaggerHashimoto
            donationConfiguration = CreateCoinConfiguration("NiceHash:DaggerHashimoto", "stratum+tcp://daggerhashimoto.usa.nicehash.com/#xnsub", 3353, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Decred
            donationConfiguration = CreateCoinConfiguration("NiceHash:Decred", "stratum+tcp://decred.usa.nicehash.com/#xnsub", 3354, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //CryptoNight
            donationConfiguration = CreateCoinConfiguration("NiceHash:CryptoNight", "stratum+tcp://cryptonight.usa.nicehash.com/#xnsub", 3355, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Lbry
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lbry", "stratum+tcp://lbry.usa.nicehash.com/#xnsub", 3356, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Equihash
            donationConfiguration = CreateCoinConfiguration("NiceHash:Equihash", "stratum+tcp://equihash.usa.nicehash.com/#xnsub", 3357, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Pascal
            donationConfiguration = CreateCoinConfiguration("NiceHash:Pascal", "stratum+tcp://pascal.usa.nicehash.com/#xnsub", 3358, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //X11Gost
            donationConfiguration = CreateCoinConfiguration("NiceHash:X11Gost", "stratum+tcp://x11gost.usa.nicehash.com/#xnsub", 3359, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Sia
            donationConfiguration = CreateCoinConfiguration("NiceHash:Sia", "stratum+tcp://sia.usa.nicehash.com/#xnsub", 3360, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Blake2s
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake2s", "stratum+tcp://blake2s.usa.nicehash.com/#xnsub", 3361, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //Skunk
            donationConfiguration = CreateCoinConfiguration("NiceHash:Skunk", "stratum+tcp://skunk.usa.nicehash.com/#xnsub", 3362, "1LRtJBNQm9ALYt9gQjVK1TdRyQ6UPGUNCw");
            configurations.Add(donationConfiguration);

            //BCH
            donationConfiguration = CreateCoinConfiguration("BCH", "stratum+tcp://stratum.bcc.pool.bitcoin.com", 3333);
            configurations.Add(donationConfiguration);

            //BCC
            donationConfiguration = CreateCoinConfiguration("BCC", "stratum+tcp://stratum.bcc.pool.bitcoin.com", 3333);
            configurations.Add(donationConfiguration);

            //NVC
            donationConfiguration = CreateCoinConfiguration("NVC", "stratum+tcp://stratum.khore.org", 3335);
            configurations.Add(donationConfiguration);

        }

        private static Data.Configuration.Coin CreateCoinConfiguration(string coinSymbol, string host, int port, string username = "nwoolls")
        {
            return CreateCoinConfiguration(coinSymbol, host, port, username, GetWorkerName());
        }

        private static Data.Configuration.Coin CreateCoinConfiguration(string coinSymbol, string host, int port, string username, string workerName)
        {
            Data.Configuration.Coin donationConfiguration = new Data.Configuration.Coin();
            donationConfiguration.PoolGroup.Id = coinSymbol;

            MiningPool donationPool = new MiningPool()
            {
                Host = host + "/#skipcbcheck",
                Port = port,
                Username = username + "." + workerName,
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            return donationConfiguration;
        }

        private static string GetWorkerName()
        {
            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                return "mmdonations";
            else
                return Fingerprint.Value().Replace("-", "");
        }
    }
}
