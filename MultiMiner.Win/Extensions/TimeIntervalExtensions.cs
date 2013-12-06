using MultiMiner.Win.Configuration;

namespace MultiMiner.Win.Extensions
{
    static class TimeIntervalExtensions
    {
        public static int ToMinutes(this ApplicationConfiguration.TimerInterval timerInterval)
        {
            int coinStatsMinutes;
            switch (timerInterval)
            {
                case ApplicationConfiguration.TimerInterval.FiveMinutes:
                    coinStatsMinutes = 5;
                    break;
                case ApplicationConfiguration.TimerInterval.ThirtyMinutes:
                    coinStatsMinutes = 30;
                    break;
                case ApplicationConfiguration.TimerInterval.OneHour:
                    coinStatsMinutes = 1 * 60;
                    break;
                case ApplicationConfiguration.TimerInterval.TwoHours:
                    coinStatsMinutes = 2 * 60;
                    break;
                case ApplicationConfiguration.TimerInterval.ThreeHours:
                    coinStatsMinutes = 3 * 60;
                    break;
                case ApplicationConfiguration.TimerInterval.SixHours:
                    coinStatsMinutes = 6 * 60;
                    break;
                case ApplicationConfiguration.TimerInterval.TwelveHours:
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
