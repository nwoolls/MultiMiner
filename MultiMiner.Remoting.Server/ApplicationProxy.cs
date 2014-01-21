using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public sealed class ApplicationProxy
    {
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
    }
}
