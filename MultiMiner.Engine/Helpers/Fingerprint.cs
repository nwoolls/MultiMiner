using System;
using System.Security.Cryptography;
using System.Text;

namespace MultiMiner.Engine.Helpers
{
    public class Fingerprint
    {
        private static string fingerPrint = String.Empty;

        public static string Value()
        {
            if (String.IsNullOrEmpty(fingerPrint))
            {
                fingerPrint = GetHash(
                    "CPU >> " + GetProcessorID() + 
                    "\nBIOS >> " + GetBIOSID() + 
                    "\nBASE >> " + GetBaseBoardID() +
                    "\nDISK >> " + GetDiskDriveID() + 
                    "\nVIDEO >> " + GetVideoControllerID() + 
                    "\nMAC >> " + GetMACAddressID()
                    );
            }
            return fingerPrint;
        }

        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(byte[] bt)
        {
            Guid guid = new Guid(bt);
            return guid.ToString();
        }
        
        //Return a hardware identifier
        private static string GetWMIIdentifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    object propVal = mo[wmiProperty];
                    if (propVal != null)
                    {
                        return propVal.ToString();
                    }
                }
            }
            return "";
        }

        //Return a hardware identifier
        private static string GetWMIIdentifier(string wmiClass, string wmiProperty)
        {
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                object propVal = mo[wmiProperty];
                if (propVal != null)
                {
                    return propVal.ToString();
                }
            }
            return "";
        }

        private static string GetProcessorID()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = GetWMIIdentifier("Win32_Processor", "UniqueId");

            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = GetWMIIdentifier("Win32_Processor", "ProcessorId");
            }

            if (retVal == "") //If no ProcessorId, use Name
            {
                retVal = GetWMIIdentifier("Win32_Processor", "Name");
            }

            if (retVal == "") //If no Name, use Manufacturer
            {
                retVal = GetWMIIdentifier("Win32_Processor", "Manufacturer");
            }

            //Add clock speed for extra security
            retVal += GetWMIIdentifier("Win32_Processor", "MaxClockSpeed");

            return retVal;
        }

        //BIOS Identifier
        private static string GetBIOSID()
        {
            return GetWMIIdentifier("Win32_BIOS", "Manufacturer")
            + GetWMIIdentifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + GetWMIIdentifier("Win32_BIOS", "IdentificationCode")
            + GetWMIIdentifier("Win32_BIOS", "SerialNumber")
            + GetWMIIdentifier("Win32_BIOS", "ReleaseDate")
            + GetWMIIdentifier("Win32_BIOS", "Version");
        }

        //Main physical hard drive ID
        private static string GetDiskDriveID()
        {
            return GetWMIIdentifier("Win32_DiskDrive", "Model")
            + GetWMIIdentifier("Win32_DiskDrive", "Manufacturer")
            + GetWMIIdentifier("Win32_DiskDrive", "Signature")
            + GetWMIIdentifier("Win32_DiskDrive", "TotalHeads");
        }

        //Motherboard ID
        private static string GetBaseBoardID()
        {
            return GetWMIIdentifier("Win32_BaseBoard", "Model")
            + GetWMIIdentifier("Win32_BaseBoard", "Manufacturer")
            + GetWMIIdentifier("Win32_BaseBoard", "Name")
            + GetWMIIdentifier("Win32_BaseBoard", "SerialNumber");
        }

        //Primary video controller ID
        private static string GetVideoControllerID()
        {
            return GetWMIIdentifier("Win32_VideoController", "DriverVersion")
            + GetWMIIdentifier("Win32_VideoController", "Name");
        }

        //First enabled network card ID
        private static string GetMACAddressID()
        {
            return GetWMIIdentifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
    }
}