using System.Collections.Generic;

namespace MultiMiner.CoinApi
{
    public interface IApiContext
    {
        IEnumerable<Data.CoinInformation> GetCoinInformation(string userAgent = "");
        string GetApiUrl();
        string GetInfoUrl();
        string GetApiName();
    }
}
