using System.Collections.Generic;

namespace MultiMiner.Coin.Api
{
    public interface IApiContext
    {
        IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "");
        string GetApiUrl();
        string GetInfoUrl();
        string GetApiName();
    }
}
