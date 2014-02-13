using MultiMiner.Xgminer.Data;
using System.Collections.Generic;

namespace MultiMiner.Services
{
    public class DevicesService
    {
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer xgminerConfiguration = null;
        public DevicesService(MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration)
        {
            this.xgminerConfiguration = minerConfiguration;
        }

        public DevicesService()
        {
            this.xgminerConfiguration = new MultiMiner.Engine.Data.Configuration.Xgminer();
            this.xgminerConfiguration.LoadMinerConfiguration();
        }

        public List<Device> GetDevices(string executablePath)
        {
            Xgminer.Data.Configuration.Miner minerConfiguration = new Xgminer.Data.Configuration.Miner()
            {
                ExecutablePath = executablePath,
                DisableGpu = xgminerConfiguration.DisableGpu,
                DisableUsbProbe = xgminerConfiguration.DisableUsbProbe,
                ScanArguments = xgminerConfiguration.ScanArguments
            };

            Xgminer.Miner miner = new Xgminer.Miner(minerConfiguration);

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
