using System;

namespace MultiMiner.Coin.Api
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
