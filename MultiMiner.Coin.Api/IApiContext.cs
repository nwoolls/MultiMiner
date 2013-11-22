using System.Collections.Generic;

namespace MultiMiner.Coin.Api
{
    public interface IApiContext
    {
        IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "", BaseCoin profitabilityBasis = BaseCoin.Bitcoin);
        string GetApiUrl(BaseCoin profitabilityBasis);
    }
}
