using System;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class RoundDateExtensions
    {
        public static DateTime RoundDown(this DateTime date, TimeFrame timeFrame)
        {
            var ts = TimeSpan.FromSeconds((double)timeFrame);
            var ticks = date.Ticks / ts.Ticks;
            return new DateTime(ticks * ts.Ticks);
        }
    }
}