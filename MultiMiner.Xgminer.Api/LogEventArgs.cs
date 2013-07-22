using System;

namespace MultiMiner.Xgminer.Api
{
    public class LogEventArgs : EventArgs
    {
        public DateTime DateTime { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}
