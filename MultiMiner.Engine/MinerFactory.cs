using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Installers;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public MinerDescriptor GetMiner(CoinAlgorithm algorithm)
        {
            return Miners
                .SingleOrDefault(miner => miner.Algorithm == algorithm);
        }

        public MinerDescriptor GetDefaultMiner()
        {
            return GetMiner(CoinAlgorithm.SHA256);
        }

        public void RegisterMiner(CoinAlgorithm algorithm, string name, string fileName, IMinerInstaller installer, bool legacyApi)
        {
            if (GetMiner(algorithm) == null)
            {
                Miners.Add(new MinerDescriptor()
                {
                    Algorithm = algorithm,
                    Name = name,
                    FileName = fileName,
                    Installer = installer,
                    LegacyApi = legacyApi
                });
            }
            else throw new ArgumentException(algorithm + " is already registered");
        }
    }
}
