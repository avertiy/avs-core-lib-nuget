using System;

namespace AVS.CoreLib.Dates
{
    public static class UnixEpoch
    {
        public static readonly DateTime Start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        /// <summary>
        /// to unix time stamp in seconds from unix epoch start
        /// </summary>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long)Math.Floor(dateTime.Subtract(Start).TotalSeconds);
        }

        /// <summary>
        /// to unix time stamp in milliseconds from unix epoch start
        /// </summary>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            return (long)Math.Floor(dateTime.Subtract(Start).TotalMilliseconds);
        }
    }
}