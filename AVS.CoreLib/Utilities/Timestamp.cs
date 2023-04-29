using System;

namespace AVS.CoreLib.Utilities
{
    public static class Timestamp
    {
        public static DateTime GetLatest(this DateTime timestamp, DateTime other)
        {
            return timestamp > other ? timestamp : other;
        }

        public static long GetLatest(this long timestamp, long other)
        {
            return timestamp > other ? timestamp : other;
        }
    }
}