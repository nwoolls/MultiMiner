using MultiMiner.Engine.Installers;
using MultiMiner.Xgminer.Data;
using System;
using System.Linq;
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

            Xgminer.Miner miner = new Xgminer.Miner(minerConfiguration, false);

            Version minerVersion = new Version(MinerInstaller.GetInstalledMinerVersion(executablePath, false));
            List<Device> detectedDevices = miner.ListDevices(true, minerVersion, logging: true);

            SortDevices(detectedDevices);

            return detectedDevices;
        }

        public void UpdateDevicesForProxySettings(List<Device> devices, bool mining)
        {
            if (xgminerConfiguration.StratumProxy)
            {
                for (int i = 0; i < xgminerConfiguration.StratumProxies.Count; i++)
                {
                    Engine.Data.Configuration.Xgminer.ProxyDescriptor proxyDescriptor = xgminerConfiguration.StratumProxies[i];
                    Xgminer.Data.Device proxyDevice = new Xgminer.Data.Device()
                    {
                        Kind = DeviceKind.PXY,
                        Driver = "proxy",
                        Name = String.Format("Stratum Proxy #{0}", (i + 1)),
                        //we want the path in the ViewModel for Remoting
                        //can't rely on having the Stratum Proxy settings
                        Path = String.Format("{0}:{1}", proxyDescriptor.GetworkPort, proxyDescriptor.StratumPort),
                        RelativeIndex = i
                    };

                    if (!devices.Any(d => d.Equals(proxyDevice)))
                        devices.Add(proxyDevice);
                }
            }

            if (!mining)
            {
                devices.RemoveAll(d =>
                    (d.Kind == DeviceKind.PXY) &&
                    (
                        (!xgminerConfiguration.StratumProxy) ||
                        (!xgminerConfiguration.StratumProxies.Any(sp => String.Format("{0}:{1}", sp.GetworkPort, sp.StratumPort).Equals(d.Path)))
                    )
                );
            }

            SortDevices(devices);
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
