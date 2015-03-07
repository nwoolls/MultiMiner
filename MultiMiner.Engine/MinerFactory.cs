using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Engine.Data;
using MultiMiner.Utility.Serialization;
using System.IO;

namespace MultiMiner.Engine
{
    public sealed class MinerFactory
    {
        
        private static volatile MinerFactory instance;
        private readonly static object SyncRoot = new Object();

        private MinerFactory() { }

        public static MinerFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                            instance = new MinerFactory();
                    }
                }

                return instance;
            }
        }

        public readonly List<MinerDescriptor> Miners = new List<MinerDescriptor>();
        public readonly List<CoinAlgorithm> Algorithms = new List<CoinAlgorithm>();

        public MinerDescriptor GetMiner(DeviceKind deviceKind, CoinAlgorithm algorithm, SerializableDictionary<string, string> miners)
        {
            if (deviceKind != DeviceKind.GPU)
                return GetDefaultMiner();

            string algorithmName = algorithm.Name;

			MinerDescriptor result = null;

            if (miners.ContainsKey(algorithmName))
				// SingleOrDefault - the user may have a config file with a backend
				// miner registered that no longer exists in their Miners\ folder
				result = Miners.SingleOrDefault(m => m.Name.Equals(miners[algorithmName], StringComparison.OrdinalIgnoreCase));
            if ((result == null) && (algorithm.DefaultMiner != null))
				result = Miners.Single(m => m.Name.Equals(algorithm.DefaultMiner, StringComparison.OrdinalIgnoreCase));

            return result;
        }

        public MinerDescriptor GetMiner(DeviceKind deviceKind, string algorithmName, SerializableDictionary<string, string> miners)
        {
            CoinAlgorithm algorithm = GetAlgorithm(algorithmName);

            if (algorithm == null)
                //algorithm for name not found
                return null;

            return GetMiner(deviceKind, algorithm, miners);
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

        public CoinAlgorithm GetAlgorithm(string name)
        {
            return Algorithms.SingleOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public CoinAlgorithm RegisterAlgorithm(string name, string fullName, CoinAlgorithm.AlgorithmFamily family)
        {
            CoinAlgorithm algorithm = new CoinAlgorithm()
            {
                Name = name,
                FullName = fullName,
                Family = family
            };
            Algorithms.Add(algorithm);
            return algorithm;
        }

        public void RegisterMiners(string directory)
        {
            if (!Directory.Exists(directory))
                //otherwise raises a DirectoryNotFoundException under Mono
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            foreach (DirectoryInfo subDirectoryInfo in subDirectories)
            {
                string minerName = subDirectoryInfo.Name;

                if (Miners.Any(m => m.Name.Equals(minerName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                string searchPattern = "*miner";
                if (Utility.OS.OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix)
                    searchPattern = "*miner.exe";

                FileInfo[] files = subDirectoryInfo.GetFiles(searchPattern);
                if (files.Length > 0)
                    RegisterMiner(minerName, Path.GetFileNameWithoutExtension(files[0].Name), true);
            }
        }
    }
}
