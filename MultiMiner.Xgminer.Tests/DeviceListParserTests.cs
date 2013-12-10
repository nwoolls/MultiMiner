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
        public void PraseTextForDevices_UserData1_Succeeds()
        {
            const string inputText =
@"[2013-12-09 12:39:26] Started bfgminer 3.8.0
 [2013-12-09 12:39:28] Devices detected:
 [2013-12-09 12:39:28]  AMD Radeon HD 5800 Series (driver=opencl; procs=1)

1 devices listed";

            List<string> inputLines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            List<Device> deviceList = new List<Device>();

            DeviceListParser.ParseTextForDevices(inputLines, deviceList);

            Assert.IsTrue(deviceList.Count == 1);
        }

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

        [TestMethod]
        public void ParseTextForDevices_Over50Devices_Succeeds()
        {
            const string inputText =
@" [2013-09-01 09:12:43] Started bfgminer 3.1.4
[2013-09-01 09:12:43] Failed to load OpenCL library, no GPUs usab
 
[2013-09-01 09:12:51] Do not have user privileges required to ope
 
[2013-09-01 09:12:51] Devices detected:
[2013-09-01 09:12:51]   0. ICA 0  (driver: icarus)
[2013-09-01 09:12:51]   1. ICA 1  (driver: icarus)
[2013-09-01 09:12:51]   2. ICA 2  (driver: icarus)
[2013-09-01 09:12:51]   3. ICA 3  (driver: icarus)
[2013-09-01 09:12:51]   4. ICA 4  (driver: icarus)
[2013-09-01 09:12:51]   5. ICA 5  (driver: icarus)
[2013-09-01 09:12:51]   6. ICA 6  (driver: icarus)
[2013-09-01 09:12:51]   7. ICA 7  (driver: icarus)
[2013-09-01 09:12:51]   8. ICA 8  (driver: icarus)
[2013-09-01 09:12:51]   9. ICA 9  (driver: icarus)
[2013-09-01 09:12:51]  10. ICA10  (driver: icarus)
[2013-09-01 09:12:51]  11. ICA11  (driver: icarus)
[2013-09-01 09:12:51]  12. ICA12  (driver: icarus)
[2013-09-01 09:12:51]  13. ICA13  (driver: icarus)
[2013-09-01 09:12:51]  14. ICA14  (driver: icarus)
[2013-09-01 09:12:51]  15. ICA15  (driver: icarus)
[2013-09-01 09:12:51]  16. ICA16  (driver: icarus)
[2013-09-01 09:12:51]  17. ICA17  (driver: icarus)
[2013-09-01 09:12:51]  18. ICA18  (driver: icarus)
[2013-09-01 09:12:51]  19. ICA19  (driver: icarus)
[2013-09-01 09:12:51]  20. ICA20  (driver: icarus)
[2013-09-01 09:12:51]  21. ICA21  (driver: icarus)
[2013-09-01 09:12:51]  22. ICA22  (driver: icarus)
[2013-09-01 09:12:51]  23. ICA23  (driver: icarus)
[2013-09-01 09:12:51]  24. ICA24  (driver: icarus)
[2013-09-01 09:12:51]  25. ICA25  (driver: icarus)
[2013-09-01 09:12:51]  26. ICA26  (driver: icarus)
[2013-09-01 09:12:51]  27. ICA27  (driver: icarus)
[2013-09-01 09:12:51]  28. ICA28  (driver: icarus)
[2013-09-01 09:12:51]  29. ICA29  (driver: icarus)
[2013-09-01 09:12:51]  30. ICA30  (driver: icarus)
[2013-09-01 09:12:51]  31. ICA31  (driver: icarus)
[2013-09-01 09:12:51]  32. ICA32  (driver: icarus)
[2013-09-01 09:12:51]  33. ICA33  (driver: icarus)
[2013-09-01 09:12:51]  34. ICA34  (driver: icarus)
[2013-09-01 09:12:51]  35. ICA35  (driver: icarus)
[2013-09-01 09:12:51]  36. ICA36  (driver: icarus)
[2013-09-01 09:12:51]  37. ICA37  (driver: icarus)
[2013-09-01 09:12:51]  38. ICA38  (driver: icarus)
[2013-09-01 09:12:51]  39. ICA39  (driver: icarus)
[2013-09-01 09:12:51]  40. ICA40  (driver: icarus)
[2013-09-01 09:12:51]  41. ICA41  (driver: icarus)
[2013-09-01 09:12:51]  42. ICA42  (driver: icarus)
[2013-09-01 09:12:51]  43. ICA43  (driver: icarus)
[2013-09-01 09:12:51]  44. ICA44  (driver: icarus)
[2013-09-01 09:12:51]  45. ICA45  (driver: icarus)
[2013-09-01 09:12:51]  46. ICA46  (driver: icarus)
[2013-09-01 09:12:51]  47. ICA47  (driver: icarus)
[2013-09-01 09:12:51]  48. ICA48  (driver: icarus)
[2013-09-01 09:12:51]  49. ICA49  (driver: icarus)
[2013-09-01 09:12:51]  50. ICA50  (driver: icarus)
[2013-09-01 09:12:51]  51. ICA51  (driver: icarus)
[2013-09-01 09:12:51]  52. ICA52  (driver: icarus)
[2013-09-01 09:12:51]  53. ICA53  (driver: icarus)
[2013-09-01 09:12:51]  54. ICA54  (driver: icarus)
[2013-09-01 09:12:51]  55. ICA55  (driver: icarus)
[2013-09-01 09:12:51]  56. ICA56  (driver: icarus)
[2013-09-01 09:12:51]  57. ICA57  (driver: icarus)
[2013-09-01 09:12:51]  58. ICA58  (driver: icarus)
[2013-09-01 09:12:51]  59. ICA59  (driver: icarus)
[2013-09-01 09:12:51]  60. ICA60  (driver: icarus)
[2013-09-01 09:12:51]  61. ICA61  (driver: icarus)
[2013-09-01 09:12:51]  62. ICA62  (driver: icarus)
[2013-09-01 09:12:51]  63. ICA63  (driver: icarus)
[2013-09-01 09:12:52]  64. ICA64  (driver: icarus)
[2013-09-01 09:12:52]  65. ICA65  (driver: icarus)
[2013-09-01 09:12:52]  66. ICA66  (driver: icarus)
[2013-09-01 09:12:52]  67. ICA67  (driver: icarus)
[2013-09-01 09:12:52]  68. ICA68  (driver: icarus)
[2013-09-01 09:12:52]  69. ICA69  (driver: icarus)
70 devices listed";

            List<string> inputLines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            List<Device> deviceList = new List<Device>();

            DeviceListParser.ParseTextForDevices(inputLines, deviceList);

            Assert.IsTrue(deviceList.Count == 70);
        }
    }
}
