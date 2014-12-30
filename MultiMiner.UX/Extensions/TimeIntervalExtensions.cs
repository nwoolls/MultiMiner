using MultiMiner.UX.Data.Configuration;

namespace MultiMiner.UX.Extensions
{
    public static class TimeIntervalExtensions
    {
        public static int ToMinutes(this Application.TimerInterval timerInterval)
        {
            int minutes;
            switch (timerInterval)
            {
                case Application.TimerInterval.FiveMinutes:
                    minutes = 5;
                    break;
                case Application.TimerInterval.ThirtyMinutes:
                    minutes = 30;
                    break;
                case Application.TimerInterval.OneHour:
                    minutes = 1 * 60;
                    break;
                case Application.TimerInterval.TwoHours:
                    minutes = 2 * 60;
                    break;
                case Application.TimerInterval.ThreeHours:
                    minutes = 3 * 60;
                    break;
                case Application.TimerInterval.SixHours:
                    minutes = 6 * 60;
                    break;
                case Application.TimerInterval.TwelveHours:
                    minutes = 12 * 60;
                    break;
                case Application.TimerInterval.TwentyFourHours:
                    minutes = 24 * 60;
                    break;
                case Application.TimerInterval.FortyEightHours:
                    minutes = 48 * 60;
                    break;
                default:
                    minutes = 15;
                    break;
            }
            return minutes;
        }
    }
}
