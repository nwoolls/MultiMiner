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

            List<Data.Device> deviceList = new List<Data.Device>();

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
 [2013-08-06 21:02:10]  OCL 0 : AMD Radeon HD 7900 Series (driver=opencl; procs=1)

 [2013-08-06 21:02:10]  OCL 1 : AMD Radeon HD 7900 Series (driver=opencl; procs=1)

 [2013-08-06 21:02:10]  OCL 2 : AMD Radeon HD 7480D (driver=opencl; procs=1)

 [2013-08-06 21:02:10]  ICA 0  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 1  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 2  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 3  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 4  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 5  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 6  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 7  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 8  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA 9  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA10  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  ICA11  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
 [2013-08-06 21:02:10]  MMQ 0a: ModMiner Quad v0.4-ljr-alpha (driver=modminer; procs=1)
 [2013-08-06 21:02:10]  MMQ 0b: ModMiner Quad v0.4-ljr-alpha (driver=modminer; procs=1)
 [2013-08-06 21:02:10]  MMQ 0c: ModMiner Quad v0.4-ljr-alpha (driver=modminer; procs=1)
 [2013-08-06 21:02:10]  MMQ 0d: ModMiner Quad v0.4-ljr-alpha (driver=modminer; procs=1)
19 devices listed";

            List<string> inputLines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            List<Data.Device> deviceList = new List<Data.Device>();

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
[2013-09-01 09:12:51]  ICA 0  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 1  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 2  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 3  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 4  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 5  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 6  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 7  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 8  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA 9  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA10  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA11  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA12  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA13  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA14  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA15  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA16  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA17  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA18  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA19  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA20  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA21  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA22  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA23  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA24  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA25  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA26  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA27  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA28  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA29  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA30  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA31  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA32  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA33  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA34  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA35  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA36  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA37  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA38  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA39  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA40  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA41  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA42  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA43  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA44  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA45  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA46  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA47  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA48  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA49  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA50  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA51  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA52  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA53  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA54  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA55  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA56  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA57  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA58  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA59  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA60  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA61  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA62  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:51]  ICA63  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA64  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA65  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA66  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA67  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA68  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
[2013-09-01 09:12:52]  ICA69  (driver=icarus; procs=1; serial=0001; path=\\.\COM3)
70 devices listed";

            List<string> inputLines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            List<Data.Device> deviceList = new List<Data.Device>();

            DeviceListParser.ParseTextForDevices(inputLines, deviceList);

            Assert.IsTrue(deviceList.Count == 70);
        }
    }
}
