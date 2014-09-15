using System;

namespace MultiMiner.ExchangeApi
{
    [Serializable]
    public class ExchangeApiException : Exception
    {
        public ExchangeApiException(string message)
            : base(message)
        {
        }
    }
}
