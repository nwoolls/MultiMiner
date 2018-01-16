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
            Coin donationConfiguration = CreateCoinConfiguration("BTC", "stratum+tcp://stratum.slushpool.com", 3333);
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
            donationConfiguration = CreateCoinConfiguration("NiceHash:Scrypt", "stratum+tcp://scrypt.usa.nicehash.com/#xnsub", 3333, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //SHA256
            donationConfiguration = CreateCoinConfiguration("NiceHash:SHA256", "stratum+tcp://sha256.usa.nicehash.com/#xnsub", 3334, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //ScryptN
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptN", "stratum+tcp://scryptnf.usa.nicehash.com/#xnsub", 3335, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //ScryptNf
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptNf", "stratum+tcp://scryptnf.usa.nicehash.com/#xnsub", 3335, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //X11
            donationConfiguration = CreateCoinConfiguration("NiceHash:X11", "stratum+tcp://x11.usa.nicehash.com/#xnsub", 3336, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //X13
            donationConfiguration = CreateCoinConfiguration("NiceHash:X13", "stratum+tcp://x13.usa.nicehash.com/#xnsub", 3337, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Keccak
            donationConfiguration = CreateCoinConfiguration("NiceHash:Keccak", "stratum+tcp://keccak.usa.nicehash.com/#xnsub", 3338, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //X15
            donationConfiguration = CreateCoinConfiguration("NiceHash:X15", "stratum+tcp://x15.usa.nicehash.com/#xnsub", 3339, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Nist5
            donationConfiguration = CreateCoinConfiguration("NiceHash:Nist5", "stratum+tcp://nist5.usa.nicehash.com/#xnsub", 3340, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //NeoScrypt
            donationConfiguration = CreateCoinConfiguration("NiceHash:NeoScrypt", "stratum+tcp://neoscrypt.usa.nicehash.com/#xnsub", 3341, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Lyra2RE
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lyra2RE", "stratum+tcp://lyra2re.usa.nicehash.com/#xnsub", 3342, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //WhirlpoolX
            donationConfiguration = CreateCoinConfiguration("NiceHash:WhirlpoolX", "stratum+tcp://whirlpoolx.usa.nicehash.com/#xnsub", 3343, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Qubit
            donationConfiguration = CreateCoinConfiguration("NiceHash:Qubit", "stratum+tcp://qubit.usa.nicehash.com/#xnsub", 3344, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Quark
            donationConfiguration = CreateCoinConfiguration("NiceHash:Quark", "stratum+tcp://quark.usa.nicehash.com/#xnsub", 3345, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Axiom
            donationConfiguration = CreateCoinConfiguration("NiceHash:Axiom", "stratum+tcp://axiom.usa.nicehash.com/#xnsub", 3346, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Lyra2REv2
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lyra2REv2", "stratum+tcp://lyra2rev2.usa.nicehash.com/#xnsub", 3347, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //ScryptJaneNf16
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptJane", "stratum+tcp://scryptjanenf16.usa.nicehash.com/#xnsub", 3348, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //ScryptJaneNf16
            donationConfiguration = CreateCoinConfiguration("NiceHash:ScryptJaneNf16", "stratum+tcp://scryptjanenf16.usa.nicehash.com/#xnsub", 3348, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Blake256r8
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r8", "stratum+tcp://blake256r8.usa.nicehash.com/#xnsub", 3349, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Blake256r14
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r14", "stratum+tcp://blake256r14.usa.nicehash.com/#xnsub", 3350, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Blake256r8vnl
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake256r8vnl", "stratum+tcp://blake256r8vnl.usa.nicehash.com/#xnsub", 3351, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Hodl
            donationConfiguration = CreateCoinConfiguration("NiceHash:Hodl", "stratum+tcp://hodl.usa.nicehash.com/#xnsub", 3352, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //DaggerHashimoto
            donationConfiguration = CreateCoinConfiguration("NiceHash:DaggerHashimoto", "stratum+tcp://daggerhashimoto.usa.nicehash.com/#xnsub", 3353, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Ethash
            donationConfiguration = CreateCoinConfiguration("NiceHash:Ethash", "stratum+tcp://daggerhashimoto.usa.nicehash.com/#xnsub", 3353, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Decred
            donationConfiguration = CreateCoinConfiguration("NiceHash:Decred", "stratum+tcp://decred.usa.nicehash.com/#xnsub", 3354, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //CryptoNight
            donationConfiguration = CreateCoinConfiguration("NiceHash:CryptoNight", "stratum+tcp://cryptonight.usa.nicehash.com/#xnsub", 3355, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Lbry
            donationConfiguration = CreateCoinConfiguration("NiceHash:Lbry", "stratum+tcp://lbry.usa.nicehash.com/#xnsub", 3356, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Equihash
            donationConfiguration = CreateCoinConfiguration("NiceHash:Equihash", "stratum+tcp://equihash.usa.nicehash.com/#xnsub", 3357, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Pascal
            donationConfiguration = CreateCoinConfiguration("NiceHash:Pascal", "stratum+tcp://pascal.usa.nicehash.com/#xnsub", 3358, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //X11Gost
            donationConfiguration = CreateCoinConfiguration("NiceHash:X11Gost", "stratum+tcp://x11gost.usa.nicehash.com/#xnsub", 3359, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Sia
            donationConfiguration = CreateCoinConfiguration("NiceHash:Sia", "stratum+tcp://sia.usa.nicehash.com/#xnsub", 3360, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Blake2s
            donationConfiguration = CreateCoinConfiguration("NiceHash:Blake2s", "stratum+tcp://blake2s.usa.nicehash.com/#xnsub", 3361, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //Skunk
            donationConfiguration = CreateCoinConfiguration("NiceHash:Skunk", "stratum+tcp://skunk.usa.nicehash.com/#xnsub", 3362, "3HunzSEmwwYjcQPJWLyLf6MX8UyQVPFbj9");
            configurations.Add(donationConfiguration);

            //AkaiPool
            //MUN
            donationConfiguration = CreateCoinConfiguration("MUN", "stratum+tcp://stratum.aikapool.com", 7914);
            configurations.Add(donationConfiguration);

            //MDC
            donationConfiguration = CreateCoinConfiguration("MDC", "stratum+tcp://stratum.aikapool.com", 7981);
            configurations.Add(donationConfiguration);

            //LINX
            donationConfiguration = CreateCoinConfiguration("LINX", "stratum+tcp://stratum.aikapool.com", 7980);
            configurations.Add(donationConfiguration);

            //PINK
            donationConfiguration = CreateCoinConfiguration("PINK", "stratum+tcp://stratum.aikapool.com", 7918);
            configurations.Add(donationConfiguration);

            //BELA
            donationConfiguration = CreateCoinConfiguration("BELA", "stratum+tcp://stratum.aikapool.com", 7978);
            configurations.Add(donationConfiguration);

            //BLAZR
            donationConfiguration = CreateCoinConfiguration("BLAZR", "stratum+tcp://stratum.aikapool.com", 7979);
            configurations.Add(donationConfiguration);

            //DOGE - defunct 1/9/2018, contacted pool support
            donationConfiguration = CreateCoinConfiguration("DOGE", "stratum+tcp://stratum.aikapool.com", 7915);
            configurations.Add(donationConfiguration);

            //GEERT
            donationConfiguration = CreateCoinConfiguration("GEERT", "stratum+tcp://stratum.aikapool.com", 7936);
            configurations.Add(donationConfiguration);

            //LUNA
            donationConfiguration = CreateCoinConfiguration("LUNA", "stratum+tcp://stratum.aikapool.com", 7933);
            configurations.Add(donationConfiguration);

            //MEC
            donationConfiguration = CreateCoinConfiguration("MEC", "stratum+tcp://stratum.aikapool.com", 7988);
            configurations.Add(donationConfiguration);

            //$$$
            donationConfiguration = CreateCoinConfiguration("$$$", "stratum+tcp://stratum.aikapool.com", 7963);
            configurations.Add(donationConfiguration);

            //MOON
            donationConfiguration = CreateCoinConfiguration("MOON", "stratum+tcp://stratum.aikapool.com", 7975);
            configurations.Add(donationConfiguration);

            //SKR
            donationConfiguration = CreateCoinConfiguration("SKR", "stratum+tcp://stratum.aikapool.com", 7917);
            configurations.Add(donationConfiguration);

            //SFC
            donationConfiguration = CreateCoinConfiguration("SFC", "stratum+tcp://stratum.aikapool.com", 7935);
            configurations.Add(donationConfiguration);

            //WDC
            donationConfiguration = CreateCoinConfiguration("WDC", "stratum+tcp://stratum.aikapool.com", 7908);
            configurations.Add(donationConfiguration);

            //SNG
            donationConfiguration = CreateCoinConfiguration("SNG", "stratum+tcp://stratum.aikapool.com", 9928);
            configurations.Add(donationConfiguration);

            //VOT
            donationConfiguration = CreateCoinConfiguration("VOT", "stratum+tcp://stratum.aikapool.com", 9927);
            configurations.Add(donationConfiguration);

            //BTCZ
            donationConfiguration = CreateCoinConfiguration("BTCZ", "stratum+tcp://stratum.aikapool.com", 9925);
            configurations.Add(donationConfiguration);

            //HUSH
            donationConfiguration = CreateCoinConfiguration("HUSH", "stratum+tcp://stratum.aikapool.com", 9921);
            configurations.Add(donationConfiguration);

            //KMD
            donationConfiguration = CreateCoinConfiguration("KMD", "stratum+tcp://stratum.aikapool.com", 9922);
            configurations.Add(donationConfiguration);

            //SHF
            donationConfiguration = CreateCoinConfiguration("SHF", "stratum+tcp://stratum.aikapool.com", 9887);
            configurations.Add(donationConfiguration);

            //ELLA
            donationConfiguration = CreateCoinConfiguration("ELLA", "stratum+tcp://stratum.aikapool.com", 9888);
            configurations.Add(donationConfiguration);

            //PGC
            donationConfiguration = CreateCoinConfiguration("PGC", "stratum+tcp://stratum.aikapool.com", 9889);
            configurations.Add(donationConfiguration);

            //PIRL
            donationConfiguration = CreateCoinConfiguration("PIRL", "stratum+tcp://stratum.aikapool.com", 9890);
            configurations.Add(donationConfiguration);

            //WHL
            donationConfiguration = CreateCoinConfiguration("WHL", "stratum+tcp://stratum.aikapool.com", 9891);
            configurations.Add(donationConfiguration);

            //DBIX
            donationConfiguration = CreateCoinConfiguration("DBIX", "stratum+tcp://stratum.aikapool.com", 9892);
            configurations.Add(donationConfiguration);

            //ELE
            donationConfiguration = CreateCoinConfiguration("ELE", "stratum+tcp://stratum.aikapool.com", 9898);
            configurations.Add(donationConfiguration);

            //SWP
            donationConfiguration = CreateCoinConfiguration("SWP", "stratum+tcp://stratum.aikapool.com", 7957);
            configurations.Add(donationConfiguration);

            //KZC
            donationConfiguration = CreateCoinConfiguration("KZC", "stratum+tcp://stratum.aikapool.com", 7955);
            configurations.Add(donationConfiguration);

            //ERY
            donationConfiguration = CreateCoinConfiguration("ERY", "stratum+tcp://stratum.aikapool.com", 7931);
            configurations.Add(donationConfiguration);

            //KURT
            donationConfiguration = CreateCoinConfiguration("KURT", "stratum+tcp://stratum.aikapool.com", 7944);
            configurations.Add(donationConfiguration);

            //MAR
            donationConfiguration = CreateCoinConfiguration("MAR", "stratum+tcp://stratum.aikapool.com", 7932);
            configurations.Add(donationConfiguration);

            //OLIT
            donationConfiguration = CreateCoinConfiguration("OLIT", "stratum+tcp://stratum.aikapool.com", 7983);
            configurations.Add(donationConfiguration);

            //OZC
            donationConfiguration = CreateCoinConfiguration("OZC", "stratum+tcp://stratum.aikapool.com", 7976);
            configurations.Add(donationConfiguration);

            //PXI
            donationConfiguration = CreateCoinConfiguration("PXI", "stratum+tcp://stratum.aikapool.com", 7942);
            configurations.Add(donationConfiguration);

            //QBC
            donationConfiguration = CreateCoinConfiguration("QBC", "stratum+tcp://stratum.aikapool.com", 7950);
            configurations.Add(donationConfiguration);

            //ORB
            donationConfiguration = CreateCoinConfiguration("ORB", "stratum+tcp://stratum.aikapool.com", 7911);
            configurations.Add(donationConfiguration);

            //GBX
            donationConfiguration = CreateCoinConfiguration("GBX", "stratum+tcp://stratum.aikapool.com", 7906);
            configurations.Add(donationConfiguration);

            //INN
            donationConfiguration = CreateCoinConfiguration("INN", "stratum+tcp://stratum.aikapool.com", 7909);
            configurations.Add(donationConfiguration);

            //VIVO
            donationConfiguration = CreateCoinConfiguration("VIVO", "stratum+tcp://stratum.aikapool.com", 7905);
            configurations.Add(donationConfiguration);

            //DSR
            donationConfiguration = CreateCoinConfiguration("DSR", "stratum+tcp://stratum.aikapool.com", 7907);
            configurations.Add(donationConfiguration);

            //DIME
            donationConfiguration = CreateCoinConfiguration("DIME", "stratum+tcp://stratum.aikapool.com", 7965);
            configurations.Add(donationConfiguration);

            //QRK - defunct 1/9/2018, contacted pool support
            donationConfiguration = CreateCoinConfiguration("QRK", "stratum+tcp://stratum.aikapool.com", 7966);
            configurations.Add(donationConfiguration);

            //UBQ
            donationConfiguration = CreateCoinConfiguration("UBQ", "stratum+tcp://ubiq.mixpools.org", 2120, "0x7fc389fe473278b14a62ddb3e2d73170905e4c97", "");
            configurations.Add(donationConfiguration);

            //BCH
            donationConfiguration = CreateCoinConfiguration("BCH", "stratum+tcp://stratum.bcc.pool.bitcoin.com", 3333);
            configurations.Add(donationConfiguration);

            //BCC
            donationConfiguration = CreateCoinConfiguration("BCC", "stratum+tcp://stratum.bcc.pool.bitcoin.com", 3333);
            configurations.Add(donationConfiguration);

            //NVC
            donationConfiguration = CreateCoinConfiguration("NVC", "stratum+tcp://stratum.khore.org", 3335, "nwoolls", "mmdonations");
            configurations.Add(donationConfiguration);

            //DEM
            donationConfiguration = CreateCoinConfiguration("DEM", "stratum+tcp://gcpool.eu", 3333);
            configurations.Add(donationConfiguration);

            //ETP
            donationConfiguration = CreateCoinConfiguration("ETP", "stratum+tcp://etp.dodopool.com", 8008, "MLmsFE9UPCQCfrtzB4BmBoSjQHWarRSYT7");
            configurations.Add(donationConfiguration);

            //XJO
            donationConfiguration = CreateCoinConfiguration("XJO", "stratum+tcp://thecoin.pw", 3680, "nwoolls", "mmdonations");
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
                Username = username,
                Password = "X"
            };

            if (!String.IsNullOrEmpty(workerName))
            {
                donationPool.Username += "." + workerName;
            }

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
