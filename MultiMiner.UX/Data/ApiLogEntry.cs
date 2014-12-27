using System;

namespace MultiMiner.UX.Data
{
    public class ApiLogEntry
    {
        public DateTime DateTime { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string CoinName { get; set; }
        public string Machine { get; set; }
    }
}
