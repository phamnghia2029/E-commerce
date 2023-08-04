using System;

namespace API.Utils
{
    public static class Moments
    {
        public static DateTime DayOfThisWeekOn(DayOfWeek dayOfweek)
        {
            return DateTime.Now.AddDays(dayOfweek - DateTime.Now.DayOfWeek);
        }

        public static DateTime StartDayOf(DateTime date)
        {
            return date
                .AddHours(-date.Hour)
                .AddMinutes(-date.Minute)
                .AddSeconds(-date.Second)
                .AddMilliseconds(-date.Millisecond);
        }

        public static DateTime StartDayOfThisYear()
        {
            DateTime date = DateTime.Now;
            return date
                .AddHours(-date.Hour)
                .AddMinutes(-date.Minute)
                .AddSeconds(-date.Second)
                .AddMilliseconds(-date.Millisecond)
                .AddDays(-date.DayOfYear);
        }

        public static DateTime? Of(String? value)
        {
            if (value == null)
            {
                return null;
            }
            return DateTime.Parse(value);
        }
    }
}
