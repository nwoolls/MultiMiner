using System;
using System.Runtime.Serialization;

namespace MultiMiner.Xgminer
{
    [Serializable()]
    public class MinerLaunchException : Exception
    {
        public MinerLaunchException(string message) : base(message) { }
        protected MinerLaunchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
