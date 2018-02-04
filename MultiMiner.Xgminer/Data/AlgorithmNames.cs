namespace MultiMiner.Xgminer.Data
{
    public static class AlgorithmNames
    {
        public const string SHA256 = "SHA256";
        public const string Scrypt = "Scrypt";
        public const string ScryptN = "ScryptN";
        public const string Quark = "Quark";
        public const string Groestl = "Groestl";
        public const string X11 = "X11";
        public const string X13 = "X13";
        public const string X14 = "X14";
        public const string X15 = "X15";
        public const string ScryptJane = "ScryptJane";
        public const string Keccak = "Keccak";
        public const string Nist5 = "Nist5";
        public const string NeoScrypt = "NeoScrypt";
        public const string Lyra2RE = "Lyra2RE";
        public const string Ethash = "Ethash";
        public const string CryptoNight = "CryptoNight";
        public const string Equihash = "Equihash";
        public const string Pascal = "Pascal";
        public const string LBRY = "LBRY";
        public const string Lyra2REv2 = "Lyra2REv2";
    }

    public static class AlgorithmFullNames
    {
        public const string SHA256 = "SHA-256";
        public const string Scrypt = "Scrypt";
        public const string ScryptN = "Scrypt-Adaptive-Nfactor";
        public const string Quark = "Quark";
        public const string Groestl = "Groestl";
        public const string X11 = "X11";
        public const string X13 = "X13";
        public const string X14 = "X14";
        public const string X15 = "X15";
        public const string ScryptJane = "Scrypt-Jane";
        public const string Keccak = "Keccak";
        public const string Nist5 = "Nist5";
        public const string NeoScrypt = "NeoScrypt";
        public const string Lyra2RE = "Lyra2RE";
        public const string Ethash = "Ethash";
        public const string CryptoNight = "CryptoNight";
        public const string Equihash = "Equihash";
        public const string Pascal = "Pascal";
        public const string LBRY = "LBRY";
        public const string Lyra2REv2 = "Lyra2REv2";
    }

    //how difficult is each algo on the same hardware relative to SHA256
    //used for determining profitability with Multipools
    //reference at https://www.nicehash.com/index.jsp?p=multialgo
    public static class AlgorithmMultipliers
    {
        public const double SHA256 = 1.0;
        public const double Scrypt = 0.0010;
        public const double ScryptN = 0.0005;
        public const double Quark = 0.0034;
        public const double Groestl = 0.02;
        public const double X11 = 0.0044;
        public const double X13 = 0.0042;
        public const double X14 = 0.0040;
        public const double X15 = 0.0038;
        public const double ScryptJane = 0.0005;
        public const double Keccak = 0.5;
        public const double Nist5 = 0.015;
        public const double NeoScrypt = 0.0003;
        public const double Lyra2RE = 0.00125;
    }
}
