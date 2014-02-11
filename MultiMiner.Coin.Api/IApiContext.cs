using System.Collections.Generic;

namespace MultiMiner.Coin.Api
{
    public interface IApiContext
    {
        IEnumerable<Data.CoinInformation> GetCoinInformation(string userAgent = "");
        string GetApiUrl();
        string GetInfoUrl();
        string GetApiName();
    }
}
