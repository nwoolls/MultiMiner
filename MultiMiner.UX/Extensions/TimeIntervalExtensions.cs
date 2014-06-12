namespace MultiMiner.UX.Extensions
{
    public static class TimeIntervalExtensions
    {
        public static int ToMinutes(this Data.Configuration.Application.TimerInterval timerInterval)
        {
            int coinStatsMinutes;
            switch (timerInterval)
            {
                case Data.Configuration.Application.TimerInterval.FiveMinutes:
                    coinStatsMinutes = 5;
                    break;
                case Data.Configuration.Application.TimerInterval.ThirtyMinutes:
                    coinStatsMinutes = 30;
                    break;
                case Data.Configuration.Application.TimerInterval.OneHour:
                    coinStatsMinutes = 1 * 60;
                    break;
                case Data.Configuration.Application.TimerInterval.TwoHours:
                    coinStatsMinutes = 2 * 60;
                    break;
                case Data.Configuration.Application.TimerInterval.ThreeHours:
                    coinStatsMinutes = 3 * 60;
                    break;
                case Data.Configuration.Application.TimerInterval.SixHours:
                    coinStatsMinutes = 6 * 60;
                    break;
                case Data.Configuration.Application.TimerInterval.TwelveHours:
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
