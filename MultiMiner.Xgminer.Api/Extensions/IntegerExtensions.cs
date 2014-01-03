using System;

namespace MultiMiner.Xgminer.Api.Extensions
{
    public static class IntegerExtensions
    {
        public static DateTime UnixTimeToDateTime(this int seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        }
    }
}
