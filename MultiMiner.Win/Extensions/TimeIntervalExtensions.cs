using MultiMiner.Win.Data.Configuration;

namespace MultiMiner.Win.Extensions
{
    static class TimeIntervalExtensions
    {
        public static int ToMinutes(this Application.TimerInterval timerInterval)
        {
            int coinStatsMinutes;
            switch (timerInterval)
            {
                case Application.TimerInterval.FiveMinutes:
                    coinStatsMinutes = 5;
                    break;
                case Application.TimerInterval.ThirtyMinutes:
                    coinStatsMinutes = 30;
                    break;
                case Application.TimerInterval.OneHour:
                    coinStatsMinutes = 1 * 60;
                    break;
                case Application.TimerInterval.TwoHours:
                    coinStatsMinutes = 2 * 60;
                    break;
                case Application.TimerInterval.ThreeHours:
                    coinStatsMinutes = 3 * 60;
                    break;
                case Application.TimerInterval.SixHours:
                    coinStatsMinutes = 6 * 60;
                    break;
                case Application.TimerInterval.TwelveHours:
                    coinStatsMinutes = 12 * 60;
                    break;
                default:
                    coinStatsMinutes = 15;
                    break;
            }
            return coinStatsMinutes;
        }
    }
}
