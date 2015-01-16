using System;
using System.Globalization;

namespace MultiMiner.UX.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToReallyShortDateString(this DateTime dateTime)
        {
            //short date no year
            string shortDateString = dateTime.ToShortDateString();

            //year could be at beginning (JP) or end (EN)
            string dateSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;
            string value1 = dateTime.Year + dateSeparator;
            string value2 = dateSeparator + dateTime.Year;

            return shortDateString.Replace(value1, String.Empty).Replace(value2, String.Empty);
        }

        public static string ToReallyShortDateTimeString(this DateTime dateTime)
        {
            return String.Format("{0} {1}", dateTime.ToReallyShortDateString(), dateTime.ToShortTimeString());
        }
    }
}
