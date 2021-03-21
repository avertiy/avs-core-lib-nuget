using System;
using System.Globalization;
using AVS.CoreLib.Dates;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ParseUtcDateTime(string dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.SpecifyKind(DateTime.ParseExact(dateTime, format,
                CultureInfo.InvariantCulture), DateTimeKind.Utc);
        }

        public static DateTime ParseUtcDateTimeFromUnixTimestamp(string value, bool milliseconds = false)
        {
            var val = ulong.Parse(value);
            return milliseconds ? val.FromUnixTimeStampMs() : val.FromUnixTimeStamp();
        }
    }
}