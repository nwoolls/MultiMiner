using MultiMiner.Utility.OS;
using MultiMiner.Utility.Parsers;
using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace MultiMiner.Utility.Networking
{
    public class LocalNetwork
    {
        public static string GetLocalIPAddress()
        {
            string result = String.Empty;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    result = ip.ToString();
                    break;
                }
            }

            return result;
        }

        public static string GetLocalIPAddressRange()
        {
            string localIpAddress = GetLocalIPAddress();
            
            string[] portions = localIpAddress.Split('.');
            portions[portions.Length - 1] = "0/24";

            return String.Join(".", portions);
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
                    object workgroup;
                    //Workgroup is NULL under XP
                    if (OSVersionPlatform.IsWindowsVistaOrHigher())
                        workgroup = managementObject["Workgroup"];
                    else
                        workgroup = managementObject["Domain"];

                    result = workgroup.ToString();
                }
            }
            return result;
        }
    }
}
