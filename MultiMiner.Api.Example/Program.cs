using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MultiMiner.Api.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //examples of using MultiMiner.Xgminer.dll and MultiMiner.Xgminer.Api.dll

            //download and install the latest version of bfgminer
            const string executablePath = @"D:\bfgminer\";
            const string executableName = "bfgminer.exe";            
            MinerBackend minerBackend = MinerBackend.Bfgminer;

            Console.WriteLine("Downloading and installing {0} from {1} to the directory {2}",
                executableName, Xgminer.Installer.GetMinerDownloadRoot(minerBackend), executablePath);

            //download and install bfgminer from the official website
            Xgminer.Installer.InstallMiner(minerBackend, executablePath);
            try
            {
                //create an instance of Miner with the downloaded executable
                MinerConfiguration minerConfiguration = new MinerConfiguration()
                {
                    MinerBackend = minerBackend,
                    ExecutablePath = Path.Combine(executablePath, executableName)
                };
                Miner miner = new Miner(minerConfiguration);

                //use it to iterate through devices
                List<Device> deviceList = miner.ListDevices();

                Console.WriteLine("Using {0} to list available mining devices", executableName);

                //output devices
                foreach (Device device in deviceList)
                    Console.WriteLine("Device detected: {0}\t{1}\t{2}", device.Kind, device.Driver, device.Identifier);

                //start mining if there are devices
                if (deviceList.Count > 0)
                {
                    Console.WriteLine("{0} device(s) detected, mining Bitcoin on Bitminter using all devices", deviceList.Count);

                    //setup a pool
                    MiningPool pool = new MiningPool()
                    {
                        Host = "mint.bitminter.com",
                        Port = 3333,
                        Username = "nwoolls_deepcore",
                        Password = "deepcore"
                    };
                    minerConfiguration.Pools.Add(pool);

                    //specify algorithm
                    minerConfiguration.Algorithm = CoinAlgorithm.SHA256;

                    //disable GPU mining
                    minerConfiguration.DisableGpu = true;

                    //specify device indexes to use
                    for (int i = 0; i < deviceList.Count; i++)
                        minerConfiguration.DeviceIndexes.Add(i);

                    //enable RPC API
                    minerConfiguration.ApiListen = true;
                    minerConfiguration.ApiPort = 4028;

                    Console.WriteLine("Launching {0}", executableName);

                    //start mining
                    miner = new Miner(minerConfiguration);
                    System.Diagnostics.Process minerProcess = miner.Launch();
                    try
                    {
                        //get an API context
                        Xgminer.Api.ApiContext apiContext = new Xgminer.Api.ApiContext(minerConfiguration.ApiPort);
                        try
                        {
                            //mine for one minute, monitoring hashrate via the API
                            for (int i = 0; i < 6; i++)
                            {
                                Thread.Sleep(1000 * 10); //sleep 10s

                                //query the miner process via its RPC API for device information
                                List<Xgminer.Api.Responses.DeviceInformationResponse> deviceInformation = apiContext.GetDeviceInformation();

                                //output device information
                                foreach (Xgminer.Api.Responses.DeviceInformationResponse item in deviceInformation)
                                    Console.WriteLine("Hasrate for device {0}: {1} current, {2} average", item.Index,
                                            item.CurrentHashrate, item.AverageHashrate);
                            }
                        }
                        finally
                        {
                            Console.WriteLine("Quitting mining via the RPC API");

                            //stop mining, try the API first
                            apiContext.QuitMining();
                        }
                    }
                    finally
                    {
                        Console.WriteLine("Killing any remaining process");

                        //then kill the process
                        try
                        {
                            minerProcess.Kill();
                            minerProcess.WaitForExit();
                            minerProcess.Close();
                        }
                        catch (InvalidOperationException ex)
                        {
                            //already closed
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No devices capable of mining detected");
                }
            }
            finally
            {
                Console.WriteLine("Cleaning up, deleting directory {0}", executablePath);
                Directory.Delete(executablePath, true);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
