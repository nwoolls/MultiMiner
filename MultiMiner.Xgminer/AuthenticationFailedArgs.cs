using System;

namespace MultiMiner.Xgminer
{
    public class AuthenticationFailedArgs : EventArgs
    {
        public string Reason { get; set; }
    }
}
