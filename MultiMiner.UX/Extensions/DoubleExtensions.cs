using System;

namespace MultiMiner.UX.Extensions
{
    public static class DoubleExtensions
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
        
        public static string ToFriendlyString(this double value, bool currency = false)
        {
            if (currency)
            {
                if (value >= 10)
                    return value.ToString("0.00");
                if (value >= 1)
                    return value.ToString("0.00#");

                return RoundToSignificantDigits(value, 3).ToString("0.00##############");
            }

            if (value >= 100)
                return value.ToString("#.#");
            if (value >= 10)
                return value.ToString("#.##");
            if (value >= 1)
                return value.ToString("#.###");

            return RoundToSignificantDigits(value, 3).ToString(".################");
        }

        private static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }
    }
}
