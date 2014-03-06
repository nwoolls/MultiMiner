using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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

            string domainName;

            if (!host.Contains(":"))
                host = "http://" + host;

            Uri uri = new Uri(host);

            domainName = uri.Host;

            if (uri.HostNameType == UriHostNameType.Dns)
            {
                //remove subdomain if there is one
                if (domainName.Split('.').Length > 2)
                {
                    int index = domainName.IndexOf(".") + 1;
                    domainName = domainName.Substring(index, domainName.Length - index);
                }

                //remove TLD
                domainName = Path.GetFileNameWithoutExtension(domainName);
            }

            hostDomainNames[host] = domainName;

            return domainName;
        }

        public static string ToSpaceDelimitedWords(this string text)
        {
            return Regex.Replace(text, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
    }
}
