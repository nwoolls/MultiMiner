using System;
using System.Collections.Generic;
using System.IO;

namespace MultiMiner.Win.Extensions
{
    static class StringExtensions
    {
        private readonly static Dictionary<string, string> hostDomainNames = new Dictionary<string, string>();

        public static string DomainFromHost(this string host)
        {
            if (hostDomainNames.ContainsKey(host))
                return hostDomainNames[host];

            string domainName;

            if (!host.Contains(":"))
                host = "http://" + host;

            Uri uri = new Uri(host);

            domainName = uri.Host;

            //remove subdomain if there is one
            if (domainName.Split('.').Length > 2)
            {
                int index = domainName.IndexOf(".") + 1;
                domainName = domainName.Substring(index, domainName.Length - index);
            }

            //remove TLD
            domainName = Path.GetFileNameWithoutExtension(domainName);

            hostDomainNames[host] = domainName;

            return domainName;
        }
    }
}
