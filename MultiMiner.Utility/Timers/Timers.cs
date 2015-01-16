using System.Collections.Generic;

namespace MultiMiner.Utility.Timers
{
    public class Timers
    {
        private const int minutesPerHour = 60;
        private const int secondsPerMinute = 60;
        private const int msPerSecond = 1000;

        public const int OneSecondInterval = msPerSecond;
        private const int FiveSecondInterval = msPerSecond * 5;
        public const int TenSecondInterval = FiveSecondInterval * 2;
        public const int FifteenSecondInterval = FiveSecondInterval * 3;
        public const int ThirtySecondInterval = TenSecondInterval * 3;
        public const int OneMinuteInterval = msPerSecond * secondsPerMinute;
        public const int FifteenMinuteInterval = OneMinuteInterval * 15;
        public const int FiveMinuteInterval = OneMinuteInterval * 5;
        public const int ThirtyMinuteInterval = 3 * FifteenMinuteInterval;
        public const int OneHourInterval = OneMinuteInterval * minutesPerHour;
        public const int TwelveHourInterval = OneHourInterval * 12;

        private List<System.Timers.Timer> timers = new List<System.Timers.Timer>();

        public System.Timers.Timer CreateTimer(int interval, System.Timers.ElapsedEventHandler eventHandler)
        {
            System.Timers.Timer timer = new System.Timers.Timer()
            {
                Interval = interval,
                Enabled = true
            };
            timer.Elapsed += eventHandler;

            timers.Add(timer);

            return timer;
        }
    }
}
