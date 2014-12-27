using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Utility.Net
{
    public class IPRange
    {
        private byte[] beginIp;
        private byte[] endIp;

        public IPRange(string ipRange)
        {
            if (ipRange == null)
                throw new ArgumentNullException();

            if (!TryParseCIDRNotation(ipRange) && !TryParseSimpleRange(ipRange))
                throw new ArgumentException();
        }

        public IPRange(IPAddress startingIp, IPAddress endingIp)
        {
            beginIp = startingIp.GetAddressBytes();
            endIp = endingIp.GetAddressBytes();
        }

        public IEnumerable<IPAddress> GetIPAddresses()
        {
            int capacity = 1;
            for (int i = 0; i < 4; i++)
                capacity *= endIp[i] - beginIp[i] + 1;

            List<IPAddress> ips = new List<IPAddress>(capacity);
            for (int i = beginIp[0]; i <= endIp[0]; i++)
                for (int j = beginIp[1]; j <= endIp[1]; j++)
                    for (int k = beginIp[2]; k <= endIp[2]; k++)
                        for (int l = beginIp[3]; l <= endIp[3]; l++)
                            ips.Add(new IPAddress(new byte[] { (byte)i, (byte)j, (byte)k, (byte)l }));

            return ips;
        }

        /// <summary>
        /// Parse IP-range string in CIDR notation.
        /// For example "12.15.0.0/16".
        /// </summary>
        /// <param name="ipRange"></param>
        /// <returns></returns>
        private bool TryParseCIDRNotation(string ipRange)
        {
            string[] x = ipRange.Split('/');

            if (x.Length != 2)
                return false;

            byte bits = byte.Parse(x[1]);
            uint ip = 0;
            String[] ipParts0 = x[0].Split('.');
            for (int i = 0; i < 4; i++)
            {
                ip = ip << 8;
                ip += uint.Parse(ipParts0[i]);
            }

            byte shiftBits = (byte)(32 - bits);
            uint ip1 = (ip >> shiftBits) << shiftBits;

            if (ip1 != ip) // Check correct subnet address
                return false;

            uint ip2 = ip1 >> shiftBits;
            for (int k = 0; k < shiftBits; k++)
            {
                ip2 = (ip2 << 1) + 1;
            }

            beginIp = new byte[4];
            endIp = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                beginIp[i] = (byte)((ip1 >> (3 - i) * 8) & 255);
                endIp[i] = (byte)((ip2 >> (3 - i) * 8) & 255);
            }

            return true;
        }

        /// <summary>
        /// Parse IP-range string "12.15-16.1-30.10-255"
        /// </summary>
        /// <param name="ipRange"></param>
        /// <returns></returns>
        private bool TryParseSimpleRange(string ipRange)
        {
            String[] ipParts = ipRange.Split('.');

            beginIp = new byte[4];
            endIp = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                string[] rangeParts = ipParts[i].Split('-');

                if (rangeParts.Length < 1 || rangeParts.Length > 2)
                    return false;

                beginIp[i] = byte.Parse(rangeParts[0]);
                endIp[i] = (rangeParts.Length == 1) ? beginIp[i] : byte.Parse(rangeParts[1]);
            }

            return true;
        }
    }
}
