using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using MultiMiner.Xgminer.Api.Parsers;
using System.Collections.Generic;
using System.Globalization;

namespace MultiMiner.Xgminer.Api.Tests
{
    [TestClass]
    public class DeviceInformationParserTests
    {
        [TestMethod]
        public void ParseText_RussianCulture_Succeeds()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            const string apiText = @"STATUS=S,When=1373818935,Code=9,Msg=2 GPU(s) - 0 ASC(s) - 0 PGA(s) - ,Description=cgminer 3.3.1|GPU=0,Enabled=Y,Status=Alive,Temperature=66.00,Fan Speed=3257,Fan Percent=70,GPU Clock=1065,Memory Clock=1650,GPU Voltage=1.090,GPU Activity=99,Powertune=0,MHS av=0.64,MHS 1s=0.58,Accepted=24,Rejected=0,Hardware Errors=0,Utility=145.71,Intensity=20,Last Share Pool=0,Last Share Time=1373818934,Total MH=6.2915,Diff1 Work=85,Difficulty Accepted=90.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=2.00000000,Last Valid Work=1373818934|GPU=1,Enabled=Y,Status=Alive,Temperature=75.00,Fan Speed=3242,Fan Percent=70,GPU Clock=1065,Memory Clock=1650,GPU Voltage=1.090,GPU Activity=99,Powertune=0,MHS av=0.64,MHS 1s=0.58,Accepted=30,Rejected=0,Hardware Errors=0,Utility=182.14,Intensity=20,Last Share Pool=0,Last Share Time=1373818934,Total MH=6.2915,Diff1 Work=95,Difficulty Accepted=102.00000000,Difficulty Rejected=0.00000000,Last Share Difficulty=2.00000000,Last Valid Work=1373818934|";
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();
            DeviceInformationParser.ParseTextForDeviceInformation(apiText, deviceInformation);
        }
    }
}
