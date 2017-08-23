using System;

namespace XayahBot.Utility
{
    public static class TimeUtil
    {
        public static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        public static DateTime InDays(int amount)
        {
            DateTime now = Now();
            return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(amount);
        }
    }
}
