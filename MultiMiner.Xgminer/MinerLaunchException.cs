using System;
using System.Runtime.Serialization;

namespace MultiMiner.Xgminer
{
    [Serializable()]
    public class MinerLaunchException : Exception, ISerializable
    {
        public MinerLaunchException() : base() { }
        public MinerLaunchException(string message) : base(message) { }
        public MinerLaunchException(string message, Exception inner) : base(message, inner) { }
        public MinerLaunchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
