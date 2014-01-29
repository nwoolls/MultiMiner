using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;

namespace MultiMiner.Discovery.Tests
{
    [TestClass]
    public class ListenerTests
    {
        [TestMethod]
        public void Listener_ListenTwice_ThrowsSocketException()
        {
            Listener listener = new Listener();
            listener.Listen(0);
            try
            {
                try
                {
                    new Listener().Listen(0);
                    Assert.Fail("No Exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is SocketException);
                }                
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
