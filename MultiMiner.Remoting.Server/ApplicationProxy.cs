using MultiMiner.Engine;
using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public sealed class ApplicationProxy
    {
        //events
        //delegate declarations
        public delegate void RemoteEventHandler(object sender, RemoteCommandEventArgs ea);
        public delegate void CoinEventHandler(object sender, string coinSymbol, RemoteCommandEventArgs ea);

        //event declarations        
        public event RemoteEventHandler StartMiningRequested;
        public event RemoteEventHandler StopMiningRequested;
        public event RemoteEventHandler RestartMiningRequested;
        public event RemoteEventHandler ScanHardwareRequested;
        public event CoinEventHandler SetAllDevicesToCoinRequested;

        private bool mining;
        private List<Data.Transfer.Device> devices;
        private List<CryptoCoin> configuredCoins;
        private static volatile ApplicationProxy instance;
        private static object syncRoot = new Object();

        private ApplicationProxy() { }

        public static ApplicationProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ApplicationProxy();
                    }
                }

                return instance;
            }
        }

        public List<Data.Transfer.Device> Devices
        {
            get
            {
                lock (syncRoot)
                {
                    return devices;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    devices = value;
                }
            }
        }

        public List<CryptoCoin> ConfiguredCoins
        {
            get
            {
                lock (syncRoot)
                {
                    return configuredCoins;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    configuredCoins = value;
                }
            }
        }

        public bool Mining
        {
            get
            {
                lock (syncRoot)
                {
                    return mining;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    mining = value;
                }
            }
        }

        public void StopMining(RemotingService sender, string clientAddress, string signature)
        {
            if (StopMiningRequested != null)
                StopMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void StartMining(RemotingService sender, string clientAddress, string signature)
        {
            if (StartMiningRequested != null)
                StartMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void RestartMining(RemotingService sender, string clientAddress, string signature)
        {
            if (RestartMiningRequested != null)
                RestartMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void ScanHardware(RemotingService sender, string clientAddress, string signature)
        {
            if (ScanHardwareRequested != null)
                ScanHardwareRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void SetAllDevicesToCoin(RemotingService sender, string clientAddress, string signature, string coinSymbol)
        {
            if (SetAllDevicesToCoinRequested != null)
                SetAllDevicesToCoinRequested(sender, coinSymbol, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }
    }
}
