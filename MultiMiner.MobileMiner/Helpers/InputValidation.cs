using System.Text.RegularExpressions;

namespace MultiMiner.MobileMiner.Helpers
{
    public static class InputValidation
    {
        public static bool IsValidApplicationKey(string text)
        {
            // const string appKey
            const string appKeyPattern = @"^[a-z0-9]{4}\-[a-z0-9]{4}\-[a-z0-9]{4}$";
            var regex = new Regex(appKeyPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(text);
        }

        public static bool IsValidEmailAddress(string text)
        {
            const string emailPattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            var regex = new Regex(emailPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(text);
        }
    }
}
