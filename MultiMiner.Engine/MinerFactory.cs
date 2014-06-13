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

        public MinerDescriptor GetMiner(DeviceKind deviceKind, CoinAlgorithm algorithm, SerializableDictionary<CoinAlgorithm, string> miners)
        {
            if (deviceKind != DeviceKind.GPU)
                return GetDefaultMiner();

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
