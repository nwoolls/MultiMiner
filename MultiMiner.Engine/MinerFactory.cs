using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Engine.Data;
using MultiMiner.Utility.Serialization;

namespace MultiMiner.Engine
{
    public sealed class MinerFactory
    {
        
        private static volatile MinerFactory instance;
        private readonly static object syncRoot = new Object();

        private MinerFactory() { }

        public static MinerFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new MinerFactory();
                    }
                }

                return instance;
            }
        }

        public readonly List<MinerDescriptor> Miners = new List<MinerDescriptor>();
        public readonly Dictionary<CoinAlgorithm, MinerDescriptor> DefaultMiners = new Dictionary<CoinAlgorithm, MinerDescriptor>();

        public MinerDescriptor GetMiner(CoinAlgorithm algorithm, SerializableDictionary<CoinAlgorithm, string> miners)
        {
            if (miners.ContainsKey(algorithm))
                return Miners.Single(m => m.Name.Equals(miners[algorithm], StringComparison.OrdinalIgnoreCase));
            else
                return DefaultMiners[algorithm];
        }

        public MinerDescriptor GetDefaultMiner()
        {
            return Miners.First();
        }

        public MinerDescriptor RegisterMiner(string name, string fileName, bool legacyApi)
        {
            MinerDescriptor miner = new MinerDescriptor()
            {
                Name = name,
                FileName = fileName,
                LegacyApi = legacyApi
            };
            Miners.Add(miner);
            return miner;
        }
    }
}
