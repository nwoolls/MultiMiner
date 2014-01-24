using System;
using System.Management;

namespace MultiMiner.Utility
{
    public class Networking
    {
        public static string GetWorkGroup()
        {
            ManagementObject computer_system = new ManagementObject(
                        string.Format(
                        "Win32_ComputerSystem.Name='{0}'",
                        Environment.MachineName));

            object result = computer_system["Workgroup"];
            return result.ToString();
        }
    }
}
