using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Xgminer.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class DeviceListParserTests
    {
        [TestMethod]
        public void ParseTextForDevices_Over10Devices_Succeeds()
        {
            const string inputText =
@" [2013-08-06 21:01:57] Started bfgminer 3.1.3
 [2013-08-06 21:02:04] Do not have user privileges required to open \\\.\COM1

 [2013-08-06 21:02:10] Devices detected:
 [2013-08-06 21:02:10]   0. OCL 0 : AMD Radeon HD 7900 Series (driver: opencl)

 [2013-08-06 21:02:10]   1. OCL 1 : AMD Radeon HD 7900 Series (driver: opencl)

 [2013-08-06 21:02:10]   2. OCL 2 : AMD Radeon HD 7480D (driver: opencl)

 [2013-08-06 21:02:10]   3. ICA 0  (driver: icarus)
 [2013-08-06 21:02:10]   4. ICA 1  (driver: icarus)
 [2013-08-06 21:02:10]   5. ICA 2  (driver: icarus)
 [2013-08-06 21:02:10]   6. ICA 3  (driver: icarus)
 [2013-08-06 21:02:10]   7. ICA 4  (driver: icarus)
 [2013-08-06 21:02:10]   8. ICA 5  (driver: icarus)
 [2013-08-06 21:02:10]   9. ICA 6  (driver: icarus)
 [2013-08-06 21:02:10]  10. ICA 7  (driver: icarus)
 [2013-08-06 21:02:10]  11. ICA 8  (driver: icarus)
 [2013-08-06 21:02:10]  12. ICA 9  (driver: icarus)
 [2013-08-06 21:02:10]  13. ICA10  (driver: icarus)
 [2013-08-06 21:02:10]  14. ICA11  (driver: icarus)
 [2013-08-06 21:02:10]  15. MMQ 0a: ModMiner Quad v0.4-ljr-alpha (driver: modminer)
 [2013-08-06 21:02:10]  16. MMQ 0b: ModMiner Quad v0.4-ljr-alpha (driver: modminer)
 [2013-08-06 21:02:10]  17. MMQ 0c: ModMiner Quad v0.4-ljr-alpha (driver: modminer)
 [2013-08-06 21:02:10]  18. MMQ 0d: ModMiner Quad v0.4-ljr-alpha (driver: modminer)
19 devices listed";

            List<string> inputLines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            List<Device> deviceList = new List<Device>();

            DeviceListParser.ParseTextForDevices(inputLines, deviceList);

            Assert.IsTrue(deviceList.Count == 19);
        }
    }
}
