using MultiMiner.UX.Extensions;
using MultiMiner.Engine.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.UX.ViewModels
{
    public class DeviceViewModel : DeviceDescriptor
    {
        public DeviceViewModel()
        {
            Workers = new List<DeviceViewModel>();
        }

        //device info
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public DevicePlatform Platform { get; set; }
        public int ProcessorCount { get; set; }

        //coin info
        public PoolGroup Coin { get; set; }
        public double Difficulty { get; set; }
        public double Price { get; set; }
        public double Profitability { get; set; }
        public double AdjustedProfitability { get; set; }
        public double AverageProfitability { get; set; }

        //calculated
        public string Pool { get; set; }
        public double Exchange { get; set; }
        public double EffectiveHashRate { get; set; }
        public double Daily { get; set; }

        //device stats
        public double Temperature { get; set; }
        public int FanPercent { get; set; }
        public double AverageHashrate { get; set; }
        public double CurrentHashrate { get; set; }
        public int AcceptedShares { get; set; }
        public int RejectedShares { get; set; }
        public int HardwareErrors { get; set; }
        public double Utility { get; set; }
        public double WorkUtility { get; set; }
        public string Intensity { get; set; } //string, might be D
        public int PoolIndex { get; set; }
        public double RejectedSharesPercent { get; set; }
        public double HardwareErrorsPercent { get; set; }

        //pool info
        private double lastShareDifficulty;
        public double LastShareDifficulty
        {
            get
            {
                return lastShareDifficulty;
            }
            set
            {
                double rootValue = Math.Truncate(value);

                if (value > (rootValue + 0.99))
                    lastShareDifficulty = rootValue + 1;
                else
                    lastShareDifficulty = value;
            }
        }

        public DateTime? LastShareTime { get; set; }
        public string Url { get; set; }
        public double BestShare { get; set; }
        public double PoolStalePercent { get; set; }

        //worker info
        public string WorkerName { get; set; }
        public int Index { get; set; }
        public List<DeviceViewModel> Workers { get; set; }
        public int ID { get; set; }

        //miner stats
        //AntMiner-specific
        public string[] ChainStatus = new string[16];
        public double Frequency { get; set; }

        //ViewModel specific
        public bool Visible { get; set; }
        public string FriendlyName { get; set; }
		public string EasyName { get { return String.IsNullOrEmpty(FriendlyName) ? Name : FriendlyName; } }
		public DateTime? LastRestart { get; set; }
        public DateTime? LastReboot { get; set; }

        public Int32 CompareTo(DeviceViewModel value)
        {
            if (this.Equals(value))
                return 0;

            DeviceViewModel d1 = this;
            DeviceViewModel d2 = value;

            int result = 0;

            result = d1.Kind.CompareTo(d2.Kind);

            if ((d1.Kind == DeviceKind.NET) && (d2.Kind == DeviceKind.NET))
            {
                string[] parts1 = d1.Path.Split(':');
                string[] parts2 = d2.Path.Split(':');

                IPAddress ip1 = IPAddress.Parse(parts1[0]);
                IPAddress ip2 = IPAddress.Parse(parts2[0]);

                if (ip1.Equals(ip2))
                    result = int.Parse(parts1[1]).CompareTo(int.Parse(parts2[1]));
                else
                    result = ip1.CompareTo(ip2);
            }

            if (result == 0)
                result = d1.Driver.CompareTo(d2.Driver);

            if (result == 0)
                result = d1.Name.CompareTo(d2.Name);

            if (result == 0)
                result = d1.Path.CompareTo(d2.Path);

            if (result == 0)
                result = d1.RelativeIndex.CompareTo(d2.RelativeIndex);

            return result;

        }
    }
}
