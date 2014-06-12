using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace MultiMiner.Win.Extensions
{
    static class StringExtensions
    {
        private readonly static Dictionary<string, string> hostDomainNames = new Dictionary<string, string>();

        public static string DomainFromHost(this string host)
        {
            if (String.IsNullOrEmpty(host))
                return String.Empty;

            if (hostDomainNames.ContainsKey(host))
                return hostDomainNames[host];

            string domainName = host.Trim();

            if (!host.Contains(":"))
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

        public static string ShortHostFromHost(this string host)
        {
            return host.Replace("http://", "").Replace("stratum+tcp://", "");
        }

        public static string ToSpaceDelimitedWords(this string text)
        {
            return Regex.Replace(Regex.Replace(text, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
        
        public static bool ParseHostAndPort(this string hostAndPort, out string host, out int port)
        {
            const char Separator = ':';
            host = String.Empty;
            port = 0;

            if (hostAndPort.Contains(Separator))
            {
                string[] parts = hostAndPort.Split(Separator);
                int newPort = 0;

                if (Int32.TryParse(parts.Last(), out newPort))
                {
                    string newHost = String.Empty;

                    //loop through all but last (- 1)
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        if (!String.IsNullOrEmpty(newHost))
                            newHost = newHost + Separator;
                        newHost = newHost + parts[i];
                    }

                    host = newHost;
                    port = newPort;
                    
                    return true;
                }
            }

            return false;
        }

    }
}
