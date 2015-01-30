using System;
using System.ComponentModel;
using System.Threading;

namespace MultiMiner.Utility.Async
{
    public class SimpleSyncObject : ISynchronizeInvoke
    {
        private readonly object sync = new object();
        
        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            var result = new SimpleAsyncResult();

            ThreadPool.QueueUserWorkItem((object state) =>
            {
                result.AsyncWaitHandle = new ManualResetEvent(false);
                try
                {
                    result.AsyncState = Invoke(method, args);
                }
                catch (Exception exception)
                {
                    result.Exception = exception;
                }
                result.IsCompleted = true;
            });
            
            return result;
        }
        
        public object EndInvoke(IAsyncResult result)
        {
            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();
            
            return result.AsyncState;
        }
        
        public object Invoke(Delegate method, object[] args)
        {
            lock (sync)
                return method.DynamicInvoke(args);
        }
        
        public bool InvokeRequired { get { return true; } }
    }
}
