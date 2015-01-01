using System.Net;

namespace MultiMiner.Utility.Net.Extensions
{
    static class IPAddressExtensions
    {
        public static SubnetClass GetSubnetClass(this IPAddress subnetMask)
        {
            if (subnetMask.Address <= 255)
                return SubnetClass.A;

            if (subnetMask.Address < 16777215)
                return SubnetClass.B;

            return SubnetClass.C;
        }
    }
}
