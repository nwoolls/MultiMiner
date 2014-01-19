using MultiMiner.Engine;
using MultiMiner.Engine.Configuration;
using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMiner.Services
{
    public class DevicesService : MarshalByRefObject
    {
        private readonly XgminerConfiguration xgminerConfiguration = null;
        public DevicesService(XgminerConfiguration minerConfiguration)
        {
            this.xgminerConfiguration = minerConfiguration;
        }

        public DevicesService()
        {
            this.xgminerConfiguration = new XgminerConfiguration();
            this.xgminerConfiguration.LoadMinerConfiguration();
        }

        public List<Device> GetDevices()
        {
            MinerConfiguration minerConfiguration = new MinerConfiguration()
            {
                ExecutablePath = MinerPath.GetPathToInstalledMiner(),
                DisableGpu = xgminerConfiguration.DisableGpu,
                DisableUsbProbe = xgminerConfiguration.DisableUsbProbe,
                ScanArguments = xgminerConfiguration.ScanArguments
            };

            Miner miner = new Miner(minerConfiguration);

            List<Device> detectedDevices = miner.ListDevices(true);

            if (xgminerConfiguration.StratumProxy)
            {
                detectedDevices.Add(new Device()
                {
                    Kind = DeviceKind.PXY,
                    Driver = "proxy",
                    Name = "Stratum Proxy"
                });
            }

            SortDevices(detectedDevices);

            return detectedDevices;
        }

        private static void SortDevices(List<Device> detectedDevices)
        {
            detectedDevices.Sort((d1, d2) =>
            {
                int result = 0;

                result = d1.Kind.CompareTo(d2.Kind);

                if (result == 0)
                    result = d1.Driver.CompareTo(d2.Driver);

                if (result == 0)
                    result = d1.Name.CompareTo(d2.Name);

                if (result == 0)
                    result = d1.Path.CompareTo(d2.Path);

                if (result == 0)
                    result = d1.RelativeIndex.CompareTo(d2.RelativeIndex);

                return result;
            });
        }
    }
}
