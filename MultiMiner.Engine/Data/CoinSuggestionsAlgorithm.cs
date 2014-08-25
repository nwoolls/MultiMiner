using System;

namespace MultiMiner.Engine.Data
{
    [Flags]
    public enum CoinSuggestionsAlgorithm
    {
        None = 0,
        SHA256 = 1 << 0,
        Scrypt = 1 << 1
    }
}
