using System;

namespace MultiMiner.MultipoolApi
{
    [Serializable]
    public class MultipoolApiException : Exception
    {
        public MultipoolApiException(string message)
            : base(message)
        {
        }
    }
}
