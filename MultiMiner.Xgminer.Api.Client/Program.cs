using System;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 4028;
            if (args.Length > 0)
                int.TryParse(args.First(), out port);

            string ipAddress = "127.0.0.1";
            if (args.Length > 1)
                ipAddress = args[1];

            Console.WriteLine(String.Format("Enter BFGMiner API command verbs to send them to port {0}.", port));
            Console.WriteLine("QUIT will quit both BFGMiner and this utility.");
            Console.WriteLine("EXIT will quit only this utility.");
            Console.WriteLine(String.Empty);
            Console.WriteLine("Ensure BFGMiner is running and try DEVS or POOLS to get started.");
            Console.WriteLine(String.Empty);
            
            while (true)
            {
                string apiVerb = Console.ReadLine();

                if (apiVerb.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                string response = new ApiContext(port, ipAddress).GetResponse(apiVerb.ToLower(), ApiContext.LongCommandTimeoutMs);
                Console.WriteLine(String.Empty);
                Console.WriteLine(String.Format("{0} => {1}", apiVerb, response));
                Console.WriteLine(String.Empty);

                if (apiVerb.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    break;
            }
        }
    }
}
