using MultiMiner.Utility.OS;
using MultiMiner.Utility.Parsers;
using System;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Utility.Net
{
    public class LocalNetwork
    {
        public static string GetLocalIPAddress()
        {
            return GetLocalIPAddresses().FirstOrDefault();
        }

        public static List<string> GetLocalIPAddresses()
        {
            List<string> result = new List<string>();

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if ((ip.AddressFamily == AddressFamily.InterNetwork) &&
                    //don't include loopback address, e.g. 127.0.0.1
                    (!IPAddress.IsLoopback(ip)))
                    result.Add(ip.ToString());
            }

            return result;
        }

        public static string GetLocalIPAddressRange(string localIpAddress)
        {
            if (String.IsNullOrEmpty(localIpAddress))
                return String.Empty;

            string[] portions = localIpAddress.Split('.');
            portions[portions.Length - 1] = "1-255";

            return String.Join(".", portions);
        }

        public static string GetLocalIPAddressRange()
        {
            return GetLocalIPAddressRange(GetLocalIPAddress());
        }

        public static List<string> GetLocalIPAddressRanges()
        {
            List<string> result = new List<string>();

            List<string> localIPAddresses = GetLocalIPAddresses();
            foreach (string localIPAddress in localIPAddresses)
                result.Add(GetLocalIPAddressRange(localIPAddress));

            return result;
        }

        public static string GetWorkGroupName()
        {
            string result = String.Empty;
            if (OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX)
            {
                //OS X
                string configFilePath = @"/Library/Preferences/SystemConfiguration/com.apple.smb.server.plist";
                PlistParser parser = new PlistParser(configFilePath);
                result = parser["Workgroup"].ToString();
            }
            else if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
            {
                //Linux
                string configFilePath = @"/etc/samba/smb.conf";
                IniFileParser parser = new IniFileParser(configFilePath);
                result = parser.GetValue("Global", "Workgroup");
            }
            else
            {
                //Windows
                using (ManagementObject managementObject = new ManagementObject(String.Format("Win32_ComputerSystem.Name='{0}'", Environment.MachineName)))
                {
                    object workgroup = managementObject["Workgroup"];

                    //Workgroup is NULL under XP and Server OS
                    //instead read Domain
                    if (workgroup == null)
                    {
                        workgroup = managementObject["Domain"];
                        const string LocalSuffix = ".local";
                        if (workgroup != null)
                        {
                            string domain = (string)workgroup;
                            if (domain.EndsWith(LocalSuffix))
                                domain = Path.GetFileNameWithoutExtension(domain);
                            workgroup = domain;
                        }
                    }

                    result = workgroup.ToString();
                }
            }
            return result;
        }
    }
}
