using MultiMiner.Engine;
using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public class RemoteCommandEventArgs : EventArgs
    {
        public string IpAddress { get; set; }
        public string Signature { get; set; }
    }

    public class ModelRequestEventArgs : RemoteCommandEventArgs
    {
        public ModelRequestEventArgs()
        {
            Devices = new List<Data.Transfer.Device>();
            ConfiguredCoins = new List<CryptoCoin>();
        }

        public List<Data.Transfer.Device> Devices { get; set; }
        public List<CryptoCoin> ConfiguredCoins { get; set; }
        public bool Mining { get; set; }
        public bool HasChanges { get; set; }
        public bool DynamicIntensity { get; set; }
    }
}
