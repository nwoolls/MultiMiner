using MultiMiner.MultipoolApi.Data;
using MultiMiner.Xgminer.Data;
using Newtonsoft.Json.Linq;

namespace MultiMiner.NiceHash.Extensions
{
    public static class CoinInformationExtensions
    {
        public static bool PopulateFromJson(this MultipoolInformation multipoolInformation, JToken jToken)
        {
            multipoolInformation.Price = jToken.Value<double>("price");

            int algorithmIndex = jToken.Value<int>("algo");
            switch (algorithmIndex)
            {
                case 0:
                    multipoolInformation.Algorithm = AlgorithmNames.Scrypt;
                    break;
                case 1:
                    multipoolInformation.Algorithm = AlgorithmNames.SHA256;
                    //only algo returned in Th/s
                    multipoolInformation.Price /= 1000;
                    break;
                case 2:
                    multipoolInformation.Algorithm = AlgorithmNames.ScryptN;
                    break;
                case 3:
                    multipoolInformation.Algorithm = AlgorithmNames.X11;
                    break;
                case 4:
                    multipoolInformation.Algorithm = AlgorithmNames.X13;
                    break;
                case 5:
                    multipoolInformation.Algorithm = AlgorithmNames.Keccak;
                    break;
                case 6:
                    multipoolInformation.Algorithm = AlgorithmNames.X15;
                    break;
                case 7:
                    multipoolInformation.Algorithm = AlgorithmNames.Nist5;
                    break;
                case 8:
                    multipoolInformation.Algorithm = AlgorithmNames.NeoScrypt;
                    break;
                case 9:
                    multipoolInformation.Algorithm = AlgorithmNames.Lyra2RE;
                    break;
                default:
                    //unknown algo
                    return false;
            }
            
            return true;
        }
    }
}
