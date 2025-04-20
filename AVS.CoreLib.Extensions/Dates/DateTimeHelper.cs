using System;
using System.Globalization;
using System.Linq;

namespace AVS.CoreLib.Dates
{
    public static class DateTimeHelper
    {
        public const long MILLISECONDS_THRESHOLD = 9_999_999_999;
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

        public static DateTime FromUnixTimestamp(long value)
        {
            return value < MILLISECONDS_THRESHOLD
                ? UnixEpoch.Start.AddSeconds(value)
                : UnixEpoch.Start.AddMilliseconds(value);
        }

        public static DateTime FromUnixTimestamp(double value)
        {
            return value > MILLISECONDS_THRESHOLD
                ? UnixEpoch.Start.AddMilliseconds(value)
                : UnixEpoch.Start.AddSeconds(value);
        }

        public static DateTime FromUnixTimestamp(ulong value)
        {
            return value < MILLISECONDS_THRESHOLD 
                ? UnixEpoch.Start.AddSeconds(value)
                : UnixEpoch.Start.AddMilliseconds(value);
        }

        public static bool IsInMilliseconds(long time)
        {
            return time > MILLISECONDS_THRESHOLD;
        }
    }
}