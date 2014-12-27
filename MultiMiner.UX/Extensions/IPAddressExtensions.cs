using System.Net;

namespace MultiMiner.UX.Extensions
{
    static class IPAddressExtensions
    {
        public static int CompareTo(this IPAddress source, IPAddress value)
        {
            byte[] bytes1 = source.GetAddressBytes();
            byte[] bytes2 = value.GetAddressBytes();

            const int SegmentCount = 4;
            for (int i = 0; i < SegmentCount; i++)
            {
                int result = bytes1[i].CompareTo(bytes2[i]);
                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}
