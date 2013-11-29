using System;
using System.Globalization;

namespace MultiMiner.Win.Extensions
{
    static class DateTimeExtensions
    {
        public static string ToReallyShortDateString(this DateTime dateTime)
        {
            //short date no year
            string shortDateValue = dateTime.ToShortDateString();
            int lastIndex = shortDateValue.LastIndexOf(CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator);
            return shortDateValue.Remove(lastIndex);
        }
    }
}
