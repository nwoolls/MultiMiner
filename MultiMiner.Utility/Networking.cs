using System;
using System.Management;

namespace MultiMiner.Utility
{
    public class Networking
    {
        public static string GetWorkGroup()
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
                    result = managementObject["Workgroup"].ToString();
                }
            }
            return result;
        }
    }
}
