﻿using System;

namespace AVS.CoreLib.Dates
{
    public static class UnixEpoch
    {
        public static readonly DateTime DateTimeUnixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime FromUnixTimeStamp(this ulong unixTimeStamp)
        {
            return DateTimeUnixEpochStart.AddSeconds(unixTimeStamp);
        }

        public static DateTime FromUnixTimeStampMs(this ulong unixTimeStampMilliseconds)
        {
            return DateTimeUnixEpochStart.AddMilliseconds(unixTimeStampMilliseconds);
        }

        public static DateTime GetDateFromUnixTime(long unixTimeStamp, bool milliseconds = true)
        {
            return milliseconds ? DateTimeUnixEpochStart.AddMilliseconds(unixTimeStamp) : DateTimeUnixEpochStart.AddSeconds(unixTimeStamp);
        }

        public static DateTime GetDateFromUnixTime(ulong unixTimeStamp, bool milliseconds = true)
        {
            return milliseconds ? DateTimeUnixEpochStart.AddMilliseconds(unixTimeStamp) : DateTimeUnixEpochStart.AddSeconds(unixTimeStamp);
        }

        /// <summary>
        /// to unix time stamp in seconds from unix epoch start
        /// </summary>
        public static ulong ToUnixTime(this DateTime dateTime)
        {
            return (ulong)Math.Floor(dateTime.Subtract(DateTimeUnixEpochStart).TotalSeconds);
        }

        /// <summary>
        /// to unix time stamp in milliseconds from unix epoch start
        /// </summary>
        public static ulong ToUnixTimeMs(this DateTime dateTime)
        {
            return (ulong)Math.Floor(dateTime.Subtract(DateTimeUnixEpochStart).TotalMilliseconds);
        }
    }
}