using System;
using MultiMiner.UX.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiMiner.UX.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void ParseHostAndPort_A_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://us-east.equihash-hub.miningpoolhub.com:1";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("stratum+tcp://us-east.equihash-hub.miningpoolhub.com", host);
            Assert.AreEqual(1, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseHostAndPort_B_Succeeds()
        {
            // arrange
            var hostAndPort = "us-east.equihash-hub.miningpoolhub.com:2";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("us-east.equihash-hub.miningpoolhub.com", host);
            Assert.AreEqual(2, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseHostAndPort_C_Succeeds()
        {
            // arrange
            var hostAndPort = "us-east.equihash-hub.miningpoolhub.com:3/";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("us-east.equihash-hub.miningpoolhub.com", host);
            Assert.AreEqual(3, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseHostAndPort_D_Succeeds()
        {
            // arrange
            var hostAndPort = "us-east.equihash-hub.miningpoolhub.com:4/#xnsub";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("us-east.equihash-hub.miningpoolhub.com/#xnsub", host);
            Assert.AreEqual(4, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseHostAndPort_E_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://us-east.equihash-hub.miningpoolhub.com:5/#xnsub";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("stratum+tcp://us-east.equihash-hub.miningpoolhub.com/#xnsub", host);
            Assert.AreEqual(5, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseHostAndPort_F_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://us-east.equihash-hub.miningpoolhub.com";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParseHostAndPort_G_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://us-east.equihash-hub.miningpoolhub.com/#xnsub";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParseHostAndPort_H_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://stratum.kano.is:81/#xnsub/#skipcbcheck";

            // act
            string host;
            int port;
            bool result = hostAndPort.ParseHostAndPort(out host, out port);

            // assert
            Assert.AreEqual("stratum+tcp://stratum.kano.is/#xnsub/#skipcbcheck", host);
            Assert.AreEqual(81, port);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PortFromHost_WithPort_ReturnsPort()
        {
            // arrange
            var hostAndPort = "stratum+tcp://stratum.kano.is:81/#xnsub/#skipcbcheck";

            // act
            int? port = hostAndPort.PortFromHost();

            // assert
            Assert.AreEqual(81, port);
        }

        [TestMethod]
        public void PortFromHost_WithoutPort_ReturnsNull()
        {
            // arrange
            var hostAndPort = "stratum+tcp://stratum.kano.is/#xnsub/#skipcbcheck";

            // act
            int? port = hostAndPort.PortFromHost();

            // assert
            Assert.IsFalse(port.HasValue);
        }

        [TestMethod]
        public void DomainFromHost_WithSubdomain_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://stratum.slushpool.com";

            // act
            var host = hostAndPort.DomainFromHost();

            // assert
            Assert.AreEqual("slushpool", host);
        }

        [TestMethod]
        public void DomainFromHost_WithoutSubdomain_Succeeds()
        {
            // arrange
            var hostAndPort = "stratum+tcp://omegapool.cc";

            // act
            var host = hostAndPort.DomainFromHost();

            // assert
            Assert.AreEqual("omegapool", host);
        }
    }
}
