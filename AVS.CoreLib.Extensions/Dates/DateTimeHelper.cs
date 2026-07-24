using System;
using System.Globalization;

namespace AVS.CoreLib.Dates
{
    public static class DateTimeHelper
    {
        public const long MILI_SECONDS_THRESHOLD = 9_999_999_999;
        public const long MICRO_SECONDS_THRESHOLD = 9_999_999_999_999;
        public const int SECONDS_IN_DAY = 86_400; 

        public static DateTime ParseUtcDateTime(string dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.SpecifyKind(DateTime.ParseExact(dateTime, format,
                CultureInfo.InvariantCulture), DateTimeKind.Utc);
        }

        public static DateTime ParseUnixTimestamp(string value)
        {
            var val = long.Parse(value);
            return FromUnixTimestamp(val);
        }

        public static DateTime FromUnixTimestamp(long timestamp)
        {
            return timestamp < MILI_SECONDS_THRESHOLD
                ? UnixEpoch.Start.AddSeconds(timestamp)
                : UnixEpoch.Start.AddMilliseconds(timestamp);
        }

        public static DateTime FromUnixTimestamp(double value)
        {
            return value > MILI_SECONDS_THRESHOLD
                ? UnixEpoch.Start.AddMilliseconds(value)
                : UnixEpoch.Start.AddSeconds(value);
        }

        public static DateTime FromUnixTimestamp(ulong value)
        {
            return value < MILI_SECONDS_THRESHOLD 
                ? UnixEpoch.Start.AddSeconds(value)
                : UnixEpoch.Start.AddMilliseconds(value);
        }

        public static bool IsInMilliseconds(long time)
        {
            return time > MILI_SECONDS_THRESHOLD;
        }

        public static TimeUnit GetTimeUnit(long time)
        {
            if (time > MICRO_SECONDS_THRESHOLD)
                return TimeUnit.Microseconds;

            if (time > MILI_SECONDS_THRESHOLD)
                return TimeUnit.Milliseconds;

            return TimeUnit.Seconds;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TimeUnit
    {
        Seconds = 0,
        Milliseconds = 1,
        Microseconds = 2,
    }
}