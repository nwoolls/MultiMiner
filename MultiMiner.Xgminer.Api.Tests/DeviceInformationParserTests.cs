using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using MultiMiner.Xgminer.Api.Parsers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using MultiMiner.Xgminer.Api.Data;

namespace MultiMiner.Xgminer.Api.Tests
{
    [TestClass]
    public class DeviceInformationParserTests
    {
        [TestMethod]
        public void ParseText_RussianCulture_Succeeds()
        {
            //arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            const string apiText = @"STATUS=S,When=1373818935,Code=9,Msg=2 GPU(s) - 0 ASC(s) - 0 PGA(s) - ,Description=cgminer 3.3.1|GPU=0,Enabled=Y,Status=Alive,Temperature=66.00,Fan Speed=3257,Fan Percent=70,GPU Clock=1065,Memory Clock=1650,GPU Voltage=1.090,GPU Activity=99,Powertune=0,MHS av=0.64,MHS 1s=0.58,Accepted=24,Rejected=0,Hardware Errors=0,Utility=145.71,Intensity=20,Last Share Pool=0,Last Share Time=1373818934,Total MH=6.2915,Diff1 Work=85,Difficulty Accepted=90.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=2.00000000,Last Valid Work=1373818934|GPU=1,Enabled=Y,Status=Alive,Temperature=75.00,Fan Speed=3242,Fan Percent=70,GPU Clock=1065,Memory Clock=1650,GPU Voltage=1.090,GPU Activity=99,Powertune=0,MHS av=0.64,MHS 1s=0.58,Accepted=30,Rejected=0,Hardware Errors=0,Utility=182.14,Intensity=20,Last Share Pool=0,Last Share Time=1373818934,Total MH=6.2915,Diff1 Work=95,Difficulty Accepted=102.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=2.00000000,Last Valid Work=1373818934|";
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();
            const int logInterval = 1;
            
            //act
            DeviceInformationParser.ParseTextForDeviceInformation(apiText, deviceInformation);

            //assert
            Assert.IsTrue(deviceInformation.Count > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AverageHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.CurrentHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AcceptedShares > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.Temperature > 0.00) > 0);
        }
        
        //one device has duplicate keys for Hardware Errors, duplicating a bfgminer bug
        //note this tests when there are multiple duplicate key-value pairs, not if there are
        //multiple duplicate keys with different values
        [TestMethod]
        public void ParseText_DuplicateKeyValues_Succeeds()
        {
            //arrange
            const string apiText = @"STATUS=S,When=1375833030,Code=9,Msg=3 GPU(s) - 16 PGA(s),Description=bfgminer 3.1.3|GPU=0,Name=OCL,ID=0,ProcID=0,Enabled=N,Status=Alive,Temperature=51.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832874,Fan Speed=1153,Fan Percent=30,GPU Clock=1050,Memory Clock=1450,GPU Voltage=1.250,GPU Activity=96,Powertune=20,Intensity=D|GPU=1,Name=OCL,ID=1,ProcID=0,Enabled=N,Status=Alive,Temperature=52.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832874,Fan Speed=1099,Fan Percent=30,GPU Clock=1050,Memory Clock=1450,GPU Voltage=1.250,GPU Activity=96,Powertune=20,Intensity=D|GPU=2,Name=OCL,ID=2,ProcID=0,Enabled=N,Status=Alive,Temperature=3.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832874,Fan Speed=-1,Fan Percent=-1,GPU Clock=304,Memory Clock=667,GPU Voltage=0.937,GPU Activity=0,Powertune=0,Intensity=D|PGA=0,Name=ICA,ID=0,ProcID=0,Enabled=Y,Status=Alive,MHS av=340.649,MHS 5s=221.067,Accepted=9,Rejected=0,Hardware Errors=1,Utility=3.997,Last Share Pool=0,Last Share Time=1375833026,Total MH=46027.3340,Diff1 Work=10,Difficulty Accepted=9.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833026|PGA=1,Name=ICA,ID=1,ProcID=0,Enabled=Y,Status=Alive,MHS av=337.982,MHS 5s=208.247,Accepted=15,Rejected=0,Hardware Errors=1,Utility=6.670,Last Share Pool=0,Last Share Time=1375833029,Total MH=45603.5042,Diff1 Work=16,Difficulty Accepted=15.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833028|PGA=2,Name=ICA,ID=2,ProcID=0,Enabled=Y,Status=Alive,MHS av=350.839,MHS 5s=214.519,Accepted=9,Rejected=0,Hardware Errors=1,Utility=4.008,Last Share Pool=0,Last Share Time=1375833025,Total MH=47271.8785,Diff1 Work=10,Difficulty Accepted=9.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833024|PGA=3,Name=ICA,ID=3,ProcID=0,Enabled=Y,Status=Alive,MHS av=319.447,MHS 5s=207.144,Accepted=10,Rejected=0,Hardware Errors=1,Utility=4.456,Last Share Pool=0,Last Share Time=1375833017,Total MH=43008.9243,Diff1 Work=11,Difficulty Accepted=10.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833017|PGA=4,Name=ICA,ID=4,ProcID=0,Enabled=Y,Status=Alive,MHS av=349.226,MHS 5s=209.267,Accepted=9,Rejected=0,Hardware Errors=0,Utility=4.126,Last Share Pool=0,Last Share Time=1375833027,Total MH=45701.6753,Diff1 Work=9,Difficulty Accepted=9.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833027|PGA=5,Name=ICA,ID=5,ProcID=0,Enabled=Y,Status=Alive,MHS av=343.349,MHS 5s=214.790,Accepted=8,Rejected=0,Hardware Errors=0,Utility=3.671,Last Share Pool=0,Last Share Time=1375833022,Total MH=44897.5644,Diff1 Work=8,Difficulty Accepted=8.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833022|PGA=6,Name=ICA,ID=6,ProcID=0,Enabled=Y,Status=Alive,MHS av=313.481,MHS 5s=201.904,Accepted=15,Rejected=0,Hardware Errors=0,Utility=6.892,Last Share Pool=0,Last Share Time=1375833016,Total MH=40939.2524,Diff1 Work=15,Difficulty Accepted=15.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833016|PGA=7,Name=ICA,ID=7,ProcID=0,Enabled=Y,Status=Alive,MHS av=348.244,MHS 5s=215.738,Accepted=6,Rejected=0,Hardware Errors=0,Utility=2.757,Last Share Pool=0,Last Share Time=1375833021,Total MH=45471.7777,Diff1 Work=6,Difficulty Accepted=6.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833021|PGA=8,Name=ICA,ID=8,ProcID=0,Enabled=Y,Status=Alive,MHS av=348.954,MHS 5s=222.509,Accepted=12,Rejected=0,Hardware Errors=0,Utility=5.515,Last Share Pool=0,Last Share Time=1375833026,Total MH=45557.9005,Diff1 Work=12,Difficulty Accepted=12.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833026|PGA=9,Name=ICA,ID=9,ProcID=0,Enabled=Y,Status=Alive,MHS av=320.507,MHS 5s=205.080,Accepted=10,Rejected=0,Hardware Errors=0,Utility=4.596,Last Share Pool=0,Last Share Time=1375833017,Total MH=41837.2451,Diff1 Work=10,Difficulty Accepted=10.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=1.00000000,Last Valid Work=1375833017|PGA=10,Name=ICA,ID=10,ProcID=0,Enabled=N,Status=Initialising,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832879|PGA=11,Name=ICA,ID=11,ProcID=0,Enabled=N,Status=Initialising,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832879|PGA=12,Name=MMQ,ID=0,ProcID=0,Enabled=N,Status=Initialising,Temperature=26.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=1375832886,Frequency=210.000,Cool Max Frequency=250.000,Max Frequency=250.000,Hardware Errors=0,Valid Nonces=0|PGA=13,Name=MMQ,ID=0,ProcID=1,Enabled=N,Status=Initialising,Temperature=26.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=0,Frequency=210.000,Cool Max Frequency=250.000,Max Frequency=250.000,Hardware Errors=0,Valid Nonces=0|PGA=14,Name=MMQ,ID=0,ProcID=2,Enabled=N,Status=Initialising,Temperature=26.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=0,Frequency=210.000,Cool Max Frequency=250.000,Max Frequency=250.000,Hardware Errors=0,Valid Nonces=0|PGA=15,Name=MMQ,ID=0,ProcID=3,Enabled=N,Status=Initialising,Temperature=25.00,MHS av=0.000,MHS 5s=0.000,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.000,Last Share Pool=-1,Last Share Time=0,Total MH=0.0000,Diff1 Work=0,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=0.00000000,Last Valid Work=0,Frequency=210.000,Cool Max Frequency=250.000,Max Frequency=250.000,Hardware Errors=0,Valid Nonces=0|";
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();
            
            //act
            DeviceInformationParser.ParseTextForDeviceInformation(apiText, deviceInformation);

            //assert
            Assert.IsTrue(deviceInformation.Count > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AverageHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.CurrentHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AcceptedShares > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.Temperature > 0.00) > 0);
        }
        
        //note this tests when there are multiple duplicate keys with different values
        [TestMethod]
        public void ParseText_DuplicateKeys_Succeeds()
        {
            //arrange
            const string apiText = @"STATUS=S,When=1514500412,Code=9,Msg=1 PGA(s),Description=bfgminer 5.4.2|PGA=0,Name=BFL,ID=0,Enabled=Y,Status=Alive,Temperature=39.00,Device Elapsed=182027,MHS av=471938.936,MHS 20s=471703.417,MHS rolling=471703.417,Accepted=40074,Rejected=129,Hardware Errors=1094474,Hardware Errors=0,Utility=13.209,Stale=0,Last Share Pool=0,Last Share Time=1514500407,Total MH=85905857070.5446,Diff1 Work=17708083,Work Utility=5836.948,Difficulty Accepted=17741956.00192903,Difficulty Rejected=57018.00000622,Difficulty Stale=0,Last Share Difficulty=442.00000005,Last Valid Work=1514500412,Device Hardware%=5.8209,Device Rejected%=0.3220|\0";
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();

            //act
            DeviceInformationParser.ParseTextForDeviceInformation(apiText, deviceInformation);

            //assert
            Assert.IsTrue(deviceInformation.Count > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AverageHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.CurrentHashrate > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.AcceptedShares > 0.00) > 0);
            Assert.IsTrue(deviceInformation.Count(d => d.Temperature > 0.00) > 0);
        }

        //one device has duplicate keys for Hardware Errors, duplicating a bfgminer bug
        [TestMethod]
        public void ParseText_LogFiles_Succeeds()
        {
            string[] files = Directory.GetFiles("App_Data", "ApiLog.*", SearchOption.AllDirectories);
            foreach (string file in files)
                ParseText_LogFile_Succeeds(file);
        }

        private static void ParseText_LogFile_Succeeds(string apiLogFileName)
        {
            string line;
            StreamReader file = new StreamReader(apiLogFileName);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("\"devs\""))
                {
                    string apiText = line;
                    List<DeviceInformation> deviceInformation = new List<DeviceInformation>();

                    //act
                    DeviceInformationParser.ParseTextForDeviceInformation(apiText, deviceInformation);

                    //assert
                    Assert.IsTrue(deviceInformation.Count > 0);
                }
            }
        }
    }
}
