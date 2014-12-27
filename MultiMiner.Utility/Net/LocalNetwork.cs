using MultiMiner.Utility.OS;
using MultiMiner.Utility.Parsers;
using System;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace MultiMiner.Utility.Net
{
    public class LocalNetwork
    {
        public struct NetworkInterfaceInfo
        {
            public IPAddress Host;
            public IPAddress Netmask;
            public IPAddress Broadcast;
            public IPAddress RangeStart;
            public IPAddress RangeEnd;
        }

        public static string GetLocalIPAddress()
        {
            return GetLocalIPAddresses().FirstOrDefault();
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));

            return new IPAddress(broadcastAddress);
        }

        public static List<NetworkInterfaceInfo> GetLocalNetworkInterfaces()
        {
            List<NetworkInterfaceInfo> result = new List<NetworkInterfaceInfo>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if ((networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel) ||
                    (networkInterface.OperationalStatus != OperationalStatus.Up))
                    continue;

                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ipInformation in ipProperties.UnicastAddresses)
                {
                    IPAddress ipAddress = ipInformation.Address;

                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        NetworkInterfaceInfo interfaceInfo = new NetworkInterfaceInfo();

                        interfaceInfo.Host = ipAddress;
                        interfaceInfo.Netmask = ipInformation.IPv4Mask;
                        interfaceInfo.Broadcast = GetBroadcastAddress(interfaceInfo.Host, interfaceInfo.Netmask);

                        //deprecations are known - we only support IPv4 scanning with this code
                        //see above check for AddressFamily.InterNetwork
                        long host = (long)(uint)IPAddress.NetworkToHostOrder((int)interfaceInfo.Host.Address);
                        long netmask = (long)(uint)IPAddress.NetworkToHostOrder((int)interfaceInfo.Netmask.Address);
                        long broadcast = (long)(uint)IPAddress.NetworkToHostOrder((int)interfaceInfo.Broadcast.Address);
                        long network = host & netmask;
                        long rangeStart = network + 1;
                        long rangeEnd = broadcast - 1;

                        interfaceInfo.RangeStart = new IPAddress((long)(uint)IPAddress.HostToNetworkOrder((int)rangeStart));
                        interfaceInfo.RangeEnd = new IPAddress((long)(uint)IPAddress.HostToNetworkOrder((int)rangeEnd));

                        result.Add(interfaceInfo);
                    }
                }
            }

            return result;
        }

        public static List<string> GetLocalIPAddresses()
        {
            return GetLocalNetworkInterfaces().Select(ni => ni.Host.ToString()).ToList();
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
