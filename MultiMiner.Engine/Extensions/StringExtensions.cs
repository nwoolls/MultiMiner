using System.Text.RegularExpressions;

namespace MultiMiner.Engine.Extensions
{
    public static class StringExtensions
    {
        public static string ToSpaceDelimitedWords(this string text)
        {
            return Regex.Replace(Regex.Replace(text, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
    }
}
