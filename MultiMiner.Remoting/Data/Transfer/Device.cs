using MultiMiner.Engine.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Device
    {
        public Device()
        {
            Workers = new List<Device>();
            Coin = new PoolGroup();
        }

        //device descriptor
        [DataMember]
        public DeviceKind Kind { get; set; }

        [DataMember]
        public int RelativeIndex { get; set; }

        [DataMember]
        public string Driver { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Serial { get; set; }

        //device info
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DevicePlatform Platform { get; set; }

        [DataMember]
        public int ProcessorCount { get; set; }

        //coin info
        [DataMember]
        public PoolGroup Coin { get; set; }

        [DataMember]
        public double Difficulty { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public double Profitability { get; set; }

        [DataMember]
        public double AdjustedProfitability { get; set; }

        [DataMember]
        public double AverageProfitability { get; set; }

        //calculated
        [DataMember]
        public string Pool { get; set; }

        [DataMember]
        public double Exchange { get; set; }

        [DataMember]
        public double EffectiveHashRate { get; set; }

        //device stats
        [DataMember]
        public double Temperature { get; set; }

        [DataMember]
        public int FanPercent { get; set; }

        [DataMember]
        public double AverageHashrate { get; set; }

        [DataMember]
        public double CurrentHashrate { get; set; }

        [DataMember]
        public int AcceptedShares { get; set; }

        [DataMember]
        public int RejectedShares { get; set; }

        [DataMember]
        public int HardwareErrors { get; set; }

        [DataMember]
        public double Utility { get; set; }

        [DataMember]
        public double WorkUtility { get; set; }

        [DataMember]
        public string Intensity { get; set; } //string, might be D

        [DataMember]
        public int PoolIndex { get; set; }

        [DataMember]
        public double RejectedSharesPercent { get; set; }

        [DataMember]
        public double HardwareErrorsPercent { get; set; }

        //pool info
        [DataMember]
        public double LastShareDifficulty { get; set; }

        [DataMember]
        public DateTime? LastShareTime { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public double BestShare { get; set; }

        [DataMember]
        public double PoolStalePercent { get; set; }

        //worker info
        [DataMember]
        public string WorkerName { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public List<Device> Workers { get; set; }

        //ViewModel specific
        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public string FriendlyName { get; set; }
    }
}
