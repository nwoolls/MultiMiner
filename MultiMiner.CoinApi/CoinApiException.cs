using System;

namespace MultiMiner.CoinApi
{
    [Serializable]
    public class CoinApiException : Exception
    {
        public CoinApiException(string message)
            : base(message)
        {
        }
    }
}
