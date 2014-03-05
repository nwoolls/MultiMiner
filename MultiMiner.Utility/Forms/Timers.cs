using System;
using System.Collections.Generic;

namespace MultiMiner.Utility.Forms
{
    public class Timers
    {
        private const int minutesPerHour = 60;
        private const int secondsPerMinute = 60;
        private const int msPerSecond = 1000;

        public const int OneSecondInterval = msPerSecond;
        public const int FiveSecondInterval = msPerSecond * 5;
        public const int TenSecondInterval = FiveSecondInterval * 2;
        public const int ThirtySecondInterval = TenSecondInterval * 3;
        public const int OneMinuteInterval = msPerSecond * secondsPerMinute;
        public const int TenMinuteInterval = OneMinuteInterval * 10;
        public const int ThirtyMinuteInterval = 3 * TenMinuteInterval;
        public const int OneHourInterval = OneMinuteInterval * minutesPerHour;

        private List<System.Windows.Forms.Timer> timers = new List<System.Windows.Forms.Timer>();

        public System.Windows.Forms.Timer CreateTimer(int interval, EventHandler eventHandler)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
            {
                Interval = interval,
                Enabled = true
            };
            timer.Tick += eventHandler;

            timers.Add(timer);

            return timer;
        }
    }
}
