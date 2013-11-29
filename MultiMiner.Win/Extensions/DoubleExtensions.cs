using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMiner.Win.Extensions
{
    static class DoubleExtensions
    {
        public static string ToDifficultyString(this double difficulty)
        {
            string suffix = "";
            double shortened = difficulty;

            if (shortened > 1000)
            {
                shortened /= 1000;
                suffix = "K";
            }

            if (shortened > 1000)
            {
                shortened /= 1000;
                suffix = "M";
            }

            if (shortened > 1000)
            {
                shortened /= 1000;
                suffix = "B";
            }

            if (shortened > 1000)
            {
                shortened /= 1000;
                suffix = "T";
            }

            return String.Format("{0:0.##} {1}", shortened, suffix).TrimEnd();            
        }

        public static string ToHashrateString(this double hashrate)
        {
            string suffix = "K";
            double shortrate = hashrate;

            if (shortrate > 1000)
            {
                shortrate /= 1000;
                suffix = "M";
            }

            if (shortrate > 1000)
            {
                shortrate /= 1000;
                suffix = "G";
            }

            if (shortrate > 1000)
            {
                shortrate /= 1000;
                suffix = "T";
            }

            if (shortrate > 1000)
            {
                shortrate /= 1000;
                suffix = "P";
            }

            return String.Format("{0:0.##} {1}h/s", shortrate, suffix);
        }
    }
}
