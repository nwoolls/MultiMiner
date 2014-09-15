using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMiner.ExchangeApi
{
    public interface IApiContext
    {
        IEnumerable<Data.ExchangeInformation> GetExchangeInformation();
        string GetApiUrl();
        string GetInfoUrl();
        string GetApiName();
    }
}
