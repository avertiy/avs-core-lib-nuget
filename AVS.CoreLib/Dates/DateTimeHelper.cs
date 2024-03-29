﻿using System;
using System.Globalization;

namespace AVS.CoreLib.Dates
{
    public static class DateTimeHelper
    {
        public static DateTime ParseUtcDateTime(string dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.SpecifyKind(DateTime.ParseExact(dateTime, format,
                CultureInfo.InvariantCulture), DateTimeKind.Utc);
        }

        public static DateTime ParseUnixTimestamp(string value)
        {
            var val = ulong.Parse(value);
            return FromUnixTimestamp(val);
        }

        public static DateTime FromUnixTimestamp(long value)
        {
            if (value < 9_999_999_999)
            {
                return UnixEpoch.Start.AddSeconds(value);
            }

            return UnixEpoch.Start.AddMilliseconds(value);
        }

        public static DateTime FromUnixTimestamp(ulong value)
        {
            if (value < 9_999_999_999)
            {
                return UnixEpoch.Start.AddSeconds(value);
            }

            return UnixEpoch.Start.AddMilliseconds(value);
        }
    }
}