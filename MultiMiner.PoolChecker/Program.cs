using MultiMiner.Xgminer.Data;
using MultiMiner.Engine.Helpers;
using MultiMiner.Engine.Data.Configuration;
using System.Collections.Generic;
using System;
using System.Net.Sockets;

namespace MultiMiner.PoolChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan timeout = new TimeSpan(0, 0, 5);
            List<Coin> configurations = new List<Coin>(); ;
            DonationPools.Seed(configurations);
            foreach (Coin coin in configurations)
            {
                foreach (MiningPool pool in coin.Pools)
                {
                    Uri uri = new Uri(pool.Host);

                    if (!IsPortOpen(uri.Host, pool.Port, timeout))
                    {
                        Console.WriteLine(String.Format("{0} pool {1} (port {2}) is defunct.", coin.PoolGroup.Id, pool.Host, pool.Port));
                    }
                    
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    if (!success)
                    {
                        return false;
                    }

                    client.EndConnect(result);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
