namespace MultiMiner.Xgminer.Data
{
    public static class KnownAlgorithms
    {
        public static readonly KnownAlgorithm[] Algorithms =
        {
            new KnownAlgorithm
            {
                Name = AlgorithmNames.SHA256,
                FullName = AlgorithmFullNames.SHA256,
                Multiplier = AlgorithmMultipliers.SHA256
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Scrypt,
                FullName = AlgorithmFullNames.Scrypt,
                Multiplier = AlgorithmMultipliers.Scrypt
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.ScryptN,
                FullName = AlgorithmFullNames.ScryptN,
                Multiplier = AlgorithmMultipliers.ScryptN
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Quark,
                FullName = AlgorithmFullNames.Quark,
                Multiplier = AlgorithmMultipliers.Quark
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Groestl,
                FullName = AlgorithmFullNames.Groestl,
                Multiplier = AlgorithmMultipliers.Groestl
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.X11,
                FullName = AlgorithmFullNames.X11,
                Multiplier = AlgorithmMultipliers.X11
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.X13,
                FullName = AlgorithmFullNames.X13,
                Multiplier = AlgorithmMultipliers.X13
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.X14,
                FullName = AlgorithmFullNames.X14,
                Multiplier = AlgorithmMultipliers.X14
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.X15,
                FullName = AlgorithmFullNames.X15,
                Multiplier = AlgorithmMultipliers.X15
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.ScryptJane,
                FullName = AlgorithmFullNames.ScryptJane,
                Multiplier = AlgorithmMultipliers.ScryptJane
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Keccak,
                FullName = AlgorithmFullNames.Keccak,
                Multiplier = AlgorithmMultipliers.Keccak
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Nist5,
                FullName = AlgorithmFullNames.Nist5,
                Multiplier = AlgorithmMultipliers.Nist5
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.NeoScrypt,
                FullName = AlgorithmFullNames.NeoScrypt,
                Multiplier = AlgorithmMultipliers.NeoScrypt
            },
            new KnownAlgorithm
            {
                Name = AlgorithmNames.Lyra2RE,
                FullName = AlgorithmFullNames.Lyra2RE,
                Multiplier = AlgorithmMultipliers.Lyra2RE
            }
        };
    }
}
