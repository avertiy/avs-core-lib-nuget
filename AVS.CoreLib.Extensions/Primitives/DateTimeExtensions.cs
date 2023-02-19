using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// @usage: foreach (var day in start.EachDay(end)){..}
        /// </summary>
        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachWeek(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(7))
                yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(this DateTime from, DateTime to)
        {
            for (var month = from.Date; month.Date <= to.Date || month.Month == to.Month; month = month.AddMonths(1))
                yield return month;
        }
        /// <summary>
        /// returns 1/01/Year
        /// </summary>
        public static DateTime StartOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }
        /// <summary>
        /// returns 1st of Month/Year
        /// </summary>
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static bool WithinRange(this DateTime value, DateTime? from, DateTime? to)
        {
            if (from.HasValue && value < from.Value)
                return false;
            if (to.HasValue && value > to.Value)
                return false;
            return true;
        }
        public static bool WithinRange(this DateTime value, DateTime from, DateTime to)
        {
            if (value < from)
                return false;
            if (value > to)
                return false;
            return true;
        }

        /// <summary>
        /// Round date time value to the nearest timespan value
        /// <example>
        /// span = 1h: time 11:30 => 12:00; 11:29 => 11:00
        /// </example>
        /// </summary>
        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }
        /// <summary>
        /// Round down date time value to the nearest down value
        /// <example>span = 1h: 11:30 => 11:00;  11:59=> 11:00 </example> 
        /// </summary>
        public static DateTime RoundDown(this DateTime date, TimeSpan span)
        {
            var ticks = date.Ticks / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        public static DateTime Round(this DateTime date, double seconds)
        {
            return date.Round(TimeSpan.FromSeconds(seconds));
        }
        public static DateTime Round(this DateTime date, int seconds)
        {
            return date.Round(TimeSpan.FromSeconds(seconds));
        }

        public static DateTime RoundDown(this DateTime date, double seconds)
        {
            return date.RoundDown(TimeSpan.FromSeconds(seconds));
        }

        public static DateTime RoundDown(this DateTime date, int seconds)
        {
            return date.RoundDown(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// DateTime weekFloor = DateTime.Now.Floor(new TimeSpan(7,0,0,0));
        /// </summary>
        public static DateTime Floor(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        /// DateTime minuteCeiling = DateTime.Now.Ceil(new TimeSpan(0,1,0));
        /// </summary>
        public static DateTime Ceil(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        public static bool IsElapsed(this DateTime dateTime, int seconds)
        {
            return (DateTime.Now - dateTime).TotalSeconds > seconds;
        }
    }
}