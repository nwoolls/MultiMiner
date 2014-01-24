using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public sealed class ApplicationProxy
    {
        //events
        //delegate declarations
        public delegate void RemoteEventHandler(object sender, RemoteCommandEventArgs ea);

        //event declarations        
        public event RemoteEventHandler StartMiningRequested;
        public event RemoteEventHandler StopMiningRequested;        
        public event RemoteEventHandler RestartMiningRequested;

        private bool mining;
        private List<Data.Transfer.Device> devices;
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
    }
}
