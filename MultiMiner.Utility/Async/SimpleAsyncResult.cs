using System;
using System.Threading;

namespace MultiMiner.Utility.Async
{
    public class SimpleAsyncResult : IAsyncResult
    {
        private object state;
        
        public bool IsCompleted { get; set; }
        
        public WaitHandle AsyncWaitHandle { get; internal set; }
        
        public object AsyncState
        {
            get
            {
                if (Exception != null) throw Exception;
                return state;
            }
            internal set
            {
                state = value;
            }
        }
        
        public bool CompletedSynchronously { get { return IsCompleted; } }
        
        internal Exception Exception { get; set; }
    }
}
