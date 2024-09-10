using System;

namespace AVS.CoreLib.Dates
{
    public static class UnixEpoch
    {
        public static readonly DateTime Start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// convert datetime to unix timestamp in seconds from unix epoch start
        /// </summary>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long)Math.Floor(dateTime.Subtract(Start).TotalSeconds);
        }

        /// <summary>
        /// convert datetime to unix timestamp in milliseconds from unix epoch start
        /// </summary>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            return (long)Math.Floor(dateTime.Subtract(Start).TotalMilliseconds);
        }

        /// <summary>
        /// convert datetime to unix timestamp either in seconds or in milliseconds 
        /// </summary>
        public static long ToUnixTime(this DateTime dateTime, bool milliseconds)
        {
            var value = milliseconds ? dateTime.Subtract(Start).TotalMilliseconds : dateTime.Subtract(Start).TotalSeconds;
            return (long)Math.Floor(value);
        }
    }
}