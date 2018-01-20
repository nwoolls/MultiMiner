using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MultiMiner.UX.Extensions
{
    public static class StringExtensions
    {
        public static bool VersionIsGreater(this string thisVersion, string thatVersion)
        {
            Version thisVersionObj = new Version(thisVersion);
            Version thatVersionObj = new Version(thatVersion);

            return thisVersionObj > thatVersionObj;
        }

        private readonly static Dictionary<string, string> hostDomainNames = new Dictionary<string, string>();

        public static string DomainFromHost(this string host)
        {
            if (String.IsNullOrEmpty(host))
                return String.Empty;

            if (hostDomainNames.ContainsKey(host))
                return hostDomainNames[host];

            string domainName = host.Trim();

            if (!host.Contains("://"))
                host = "http://" + host;

            try
            {
                Uri uri = new Uri(host);

                domainName = uri.Host;

                if (uri.HostNameType == UriHostNameType.Dns)
                {
                    // remove subdomain if there is one
                    if (domainName.Split('.').Length > 2)
                    {
                        int index = domainName.IndexOf(".") + 1;
                        domainName = domainName.Substring(index, domainName.Length - index);
                    }

                    // remove TLD
                    if (domainName.Length > 7)
                        domainName = Path.GetFileNameWithoutExtension(domainName);
                }
            }
            catch (UriFormatException)
            {
                // System.UriFormatException: Invalid URI: The hostname could not be parsed.
                // don't crash - fall back on domainName = host (initialized above)
            }

            hostDomainNames[host] = domainName;

            return domainName;
        }

        private readonly static Dictionary<string, int> hostPorts = new Dictionary<string, int>();

        public static int? PortFromHost(this string host)
        {
            if (String.IsNullOrEmpty(host))
                return null;

            if (hostPorts.ContainsKey(host))
                return hostPorts[host];

            int? port = null;

            if (!host.Contains("://"))
                host = "http://" + host;

            try
            {
                Uri uri = new Uri(host);
                if (uri.Port >= 0)
                {
                    port = uri.Port;
                }
            }
            catch (UriFormatException)
            {
                // System.UriFormatException: Invalid URI: The hostname could not be parsed.
                // don't crash - fall back on domainName = host (initialized above)
            }

            return port;
        }

        public static string ShortHostFromHost(this string host)
        {
            return host.Replace("http://", "").Replace("stratum+tcp://", "").TrimEnd('/');
        }

        private static string StripLowerChars(this string text)
        {
            return new String(text.Where(c => Char.IsNumber(c) || Char.IsUpper(c)).ToArray());
        }

        public static string FitCurrency(this string amount, int totalWidth)
        {
            var result = amount.PadCurrency();
            if (result.Length > totalWidth) result = result.Substring(0, totalWidth);
            return result;
        }

        public static string PadCurrency(this string amount)
        {
            if (String.IsNullOrEmpty(amount)) return String.Empty;

            var result = amount;

            var symbol = amount[0];
            var parts = amount.Remove(0, 1).Split(new string[] { CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);

            result = symbol + parts[0].PadLeft(4) + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + parts[1].PadRight(3);

            return result;
        }

        public static string ShortCoinSymbol(this string coinSymbol)
        {
            //may be BTC or, for MultiCoin groups, NiceHash:X11, NiceHash:NeoScrypt, Other:SHA256
            string result = coinSymbol;
            const char Colon = ':';
            if (result.Contains(Colon))
            {
                var parts = coinSymbol.Split(Colon);
                result = String.Format("{0}:{1}", parts[0].StripLowerChars(), parts[1].StripLowerChars());
                //get SHA256 shorter
                result = result.Replace(AlgorithmNames.SHA256, "SHA2");
            }
            return result;
        }

        public static string EllipsisString(this string text, int totalWidth, string ellipsis)
        {
            var result = text;
            
            if (text.Length > totalWidth)
            {
                var part1Len = (int)Math.Floor((totalWidth - ellipsis.Length) / 2.0);
                var part2Len = (int)Math.Ceiling((totalWidth - ellipsis.Length) / 2.0);
                result = String.Format("{0}{1}{2}",
                    text.Substring(0, part1Len),
                    ellipsis,
                    text.Substring(text.Length - part2Len));
            }

            return result;
        }

        private const string Ellipsis = "..";

        public static string FitLeft(this string text, int totalWidth, string ellipsis = Ellipsis)
        {
            return text.EllipsisString(totalWidth, ellipsis).PadLeft(totalWidth);
        }

        public static string FitRight(this string text, int totalWidth, string ellipsis = Ellipsis)
        {
            return text.EllipsisString(totalWidth, ellipsis).PadRight(totalWidth);
        }

        public static string PadFitRight(this string text, int totalWidth, string ellipsis = Ellipsis)
        {
            return text.EllipsisString(totalWidth - 1, ellipsis).PadRight(totalWidth);
        }

        public static string PadFitLeft(this string text, int totalWidth, string ellipsis = Ellipsis)
        {
            return text.EllipsisString(totalWidth - 1, ellipsis).PadLeft(totalWidth);
        }

        public static bool ParseHostAndPort(this string hostAndPort, out string host, out int port)
        {
            host = null;
            port = -1;
            string prefix = "";
            if (!hostAndPort.Contains("://"))
            {
                prefix = "dummy://";
            }
            Uri uri;
            try
            {
                uri = new Uri(prefix + hostAndPort);
            }
            catch (UriFormatException)
            {
                return false;
            }
            if (uri.Port == -1)
            {
                return false;
            }

            port = uri.Port;
            host = uri.Host + uri.PathAndQuery + uri.Fragment;

            if (String.IsNullOrEmpty(prefix))
            {
                host = uri.Scheme + "://" + host;
            }

            host = host.TrimEnd('/');

            // #A/#B going in would come out #A/%23B under .NET 4 (but not 4.5)
            host = Uri.UnescapeDataString(host);

            return true;
        }

    }
}
