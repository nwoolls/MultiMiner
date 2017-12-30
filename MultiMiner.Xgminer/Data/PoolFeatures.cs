using System;

namespace MultiMiner.Xgminer.Data
{
    public class PoolFeatures
    {
        public const string XNSubFragment = "#xnsub";
        public const string SkipCBCheckFragment = "#skipcbcheck";

        public static string UpdatePoolFeature(string host, string fragment, bool enabled)
        {
            string result = host;

            string uriSegment = "/" + fragment;

            if (enabled)
            {
                if (!result.Contains(uriSegment))
                {
                    result = result.TrimEnd('/') + uriSegment;
                }
            }
            else
                result = result.Replace(uriSegment, String.Empty);

            return result;
        }
    }
}
