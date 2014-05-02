using System;

namespace MultiMiner.Engine.Data
{
    [Flags]
    public enum CoinSuggestionsAlgorithm
    {
        None = 0,
        SHA256 = 1 << 0,
        Scrypt = 1 << 1,
        ScryptJane = 1 << 2,
        ScryptN = 1 << 3,
        X11 = 1 << 4
    }
}
