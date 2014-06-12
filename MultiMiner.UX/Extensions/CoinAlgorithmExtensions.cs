using MultiMiner.Engine;
using MultiMiner.Xgminer.Data;

namespace MultiMiner.UX.Extensions
{
    public static class CoinAlgorithmExtensions
    {

        public static CoinAlgorithm ToAlgorithm(this string algorithmName)
        {
            string algorithm = algorithmName.ToLower();

            //needs to be a case insensitive check to work with both CoinChoose and CoinWarz
            if (algorithm.Contains(AlgorithmNames.Groestl.ToLower()))
                return CoinAlgorithm.Groestl;
            else if (algorithm.Contains(AlgorithmNames.Quark.ToLower()))
                return CoinAlgorithm.Quark;
            else if (algorithm.Contains(AlgorithmNames.ScryptN.ToLower()))
                return CoinAlgorithm.ScryptN;
            else if (algorithm.Contains(AlgorithmNames.Scrypt.ToLower()))
                return CoinAlgorithm.Scrypt;
            else if (algorithm.Contains(AlgorithmNames.X11.ToLower()))
                return CoinAlgorithm.X11;
            else if (algorithm.Contains(AlgorithmNames.X13.ToLower()))
                return CoinAlgorithm.X13;
            else if (algorithm.Contains(AlgorithmNames.ScryptJane.ToLower()))
                return CoinAlgorithm.ScryptJane;
            else if (algorithm.Contains(AlgorithmNames.Keccak.ToLower()))
                return CoinAlgorithm.Keccak;
            else
                return CoinAlgorithm.SHA256;
        }

        public static string ToAlgorithmName(this CoinAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case CoinAlgorithm.SHA256:
                    return AlgorithmNames.SHA256;
                case CoinAlgorithm.Scrypt:
                    return AlgorithmNames.Scrypt;
                case CoinAlgorithm.ScryptJane:
                    return AlgorithmNames.ScryptJane;
                case CoinAlgorithm.ScryptN:
                    return AlgorithmNames.ScryptN;
                case CoinAlgorithm.X11:
                    return AlgorithmNames.X11;
                case CoinAlgorithm.X13:
                    return AlgorithmNames.X13;
                case CoinAlgorithm.Quark:
                    return AlgorithmNames.Quark;
                case CoinAlgorithm.Groestl:
                    return AlgorithmNames.Groestl;
                case CoinAlgorithm.Keccak:
                    return AlgorithmNames.Keccak;
            }

            return AlgorithmNames.SHA256;
        }
    }
}
