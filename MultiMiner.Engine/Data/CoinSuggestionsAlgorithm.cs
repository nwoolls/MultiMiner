using System;

namespace MultiMiner.Engine.Data
{
    [Flags]
    public enum CoinSuggestionsAlgorithm
    {
        None = 0x0,
        SHA256 = 0x1,
        Scrypt = 0x2
    }
}
