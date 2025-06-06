﻿using System;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Dates
{
    internal static class DateRangeHelper
    {
        internal static bool TryParseExact(string str, out DateRange range)
        {
            range = new DateRange();
            string[]? parts = null;

            if (str.Contains(" - "))
            {
                parts = str.Split(" - ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;
            }

            if (str.Contains(';') || str.Contains('-'))
            {
                //01/10/2019 - 02/11/2019
                parts = str.Split(new[] { ';', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;
            }

            if (parts == null)
                return false;

            var fromStr = parts[0].Trim();
            var toStr = parts[1].Trim();

            if (fromStr.Contains(':'))
            {
                var formats = new[] {
                    "yyyy-MM-ddTHH:mm:ss.fffZ",
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy HH:mm",
                    "MM/dd/yyyy HH:mm:ss",
                    "MM/dd/yyyy HH:mm",
                };

                if (DateTime.TryParseExact(fromStr, formats, null, DateTimeStyles.None, out var fromDate) &&
                    DateTime.TryParseExact(toStr, formats, null, DateTimeStyles.None, out var toDate))
                {
                    range = new DateRange(fromDate, toDate);
                    return true;
                }
            }
            else
            {
                var formats = new[] {
                    "dd/MM/yyyy",
                    "MM/dd/yyyy",
                    "yyyy-MM-dd",
                };

                if (DateTime.TryParseExact(fromStr, formats, null, DateTimeStyles.None, out var fromDate) &&
                    DateTime.TryParseExact(toStr, formats, null, DateTimeStyles.None, out var toDate))
                {
                    range = new DateRange(fromDate, toDate);
                    return true;
                }
            }

            if (DateTime.TryParse(fromStr, CultureInfo.CurrentCulture, DateTimeStyles.None, out var from) &&
                DateTime.TryParse(toStr, CultureInfo.CurrentCulture, DateTimeStyles.None, out var to)
               )
            {
                range = new DateRange(from, to);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try parse date range from string literal
        /// </summary>
        /// <code>
        ///     (i)  standard date & time literals e.g. s - second, m - minute, M - month etc.
        ///     (ii)  N+standard literal e.g. 30s - 30 seconds, 5M - 5 months etc.
        ///     (iii) extra literals like recent (means last 7 days), today, yesterday, past week, prev-month etc.
        ///
        ///   modifiers:
        ///    /now - DateTime.Now instead of DateTime.Today (for periods greater than 1D by default use DateTime.Today)
        ///    /utc - DateTime.UtcNow instead of DateTime.Today
        ///    /1   - means staring not from today but from the 1st day of the month/quarter/year
        /// </code>
        internal static bool TryParseFromLiterals(string input, out DateRange range)
        {
            var modifier = input.GetModifier();
            var str = input.TrimModifier(modifier);

            if (TryParseSimpleLiteral(str, modifier, out range))
                return true;

            str = str.ToLower();
            var date = GetDate(modifier, str);
            switch (str)
            {
                case "YTD":
                    range = new DateRange(DateTime.Today.StartOfYear(), DateTime.Today);
                    return true;
                case "recent":
                    range = new DateRange(date.Date.AddDays(-7), date);
                    return true;
                case "today":
                    range = new DateRange(date, DateTime.Now);
                    return true;
                case "yesterday":
                    range = new DateRange(date.Date.AddDays(-1), date);
                    return true;
                case "day":
                    range = new DateRange(date.Date.AddDays(-1), date);
                    return true;
                case "past-week":
                    range = new DateRange(date.Date.StartOfWeek(), date);
                    return true;
                case "prev-week":
                    range = DateRange.Create(date.Date.StartOfWeek().AddDays(-7), 7);
                    return true;
                case "week":
                    range = new DateRange(date.Date.AddDays(-7), date);
                    return true;
                case "month":
                    range = new DateRange(date.Date.AddMonths(-1), date);
                    return true;
                case "past-month":
                    range = new DateRange(date.Date.StartOfMonth().AddMonths(-1), date);
                    return true;
                case "prev-month":
                    range = DateRange.Create(date.Date.StartOfMonth().AddMonths(-1), 30);
                    return true;
                case "quarter":
                    range = new DateRange(date.Date.AddMonths(-3), date);
                    return true;
                case "half-year":
                    range = new DateRange(date.Date.AddMonths(-6), date);
                    return true;
                case "year":
                    range = new DateRange(date.Date.AddMonths(-12), date);
                    return true;
                case "prev-year":
                    range = DateRange.Create(date.Date.StartOfYear().AddYears(-1), 365);
                    return true;
                default:
                {
                    range = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// try parse literals like 1m, 24H, 365D, 3600s etc.
        /// </summary>
        private static bool TryParseSimpleLiteral(string str, Modifier modifier, out DateRange range)
        {
            if (str.Length == 0 || str.Length > 8)
            {
                range = default;
                return false;
            }

            var letters = new[] { 'y', 'q', 'm', 'w', 'd', 'h', 's', 'Y', 'Q', 'M', 'W','D','H' };
            var letter = str[^1];

            if (!letters.Contains(letter))
            {
                range = default;
                return false;
            }

            if (str.Length == 1)
            {
                range = CreateDateRange(1, letter, modifier);
                return true;
            }
            
            if (int.TryParse(str.Substring(0, str.Length - 1), out var n))
            {
                range = CreateDateRange(n, letter, modifier);
                return true;
            }

            range = default;
            return false;
        }

        internal static DateRange CreateDateRange(int n, char letter, Modifier modifier)
        {
            var toDate = GetDate(modifier, letter);
            return letter switch
            {
                'd' => new DateRange(toDate.Date.AddDays(-n), toDate),
                'D' => new DateRange(toDate.Date.AddDays(-n), toDate),
                'w' => new DateRange(toDate.Date.AddDays(-n * 7), toDate),
                'W' => new DateRange(toDate.Date.AddDays(-n * 7), toDate),
                'M' => new DateRange(toDate.Date.AddMonths(-n), toDate),
                'q' => new DateRange(toDate.Date.AddMonths(-n * 3), toDate),
                'Q' => new DateRange(toDate.Date.AddMonths(-n * 3), toDate),
                'y' => new DateRange(toDate.Date.AddYears(-n), toDate),
                'Y' => new DateRange(toDate.Date.AddYears(-n), toDate),
                's' => new DateRange(toDate.AddSeconds(-n), toDate),
                'm' => new DateRange(toDate.AddMinutes(-n), toDate),
                'h' => new DateRange(toDate.AddHours(-n), toDate),
                'H' => new DateRange(toDate.AddHours(-n), toDate),
                _ => throw new ArgumentException($"Unable to create DateRange from `{n}{letter}`")
            };
        }

        private static DateTime GetDate(Modifier modifier, string literal)
        {
            switch (modifier)
            {
                case Modifier.Now:
                    return DateTime.Now;
                case Modifier.UtcNow:
                    return DateTime.UtcNow;
                case Modifier.FirstDay:
                    switch (literal)
                    {
                        case "month":
                            return DateTime.Today.StartOfMonth();
                        case "quarter":
                            return DateTime.Today.StartOfQuarter();
                        case "year":
                            return DateTime.Today.StartOfYear();
                    }
                    break;
            }
            return DateTime.Today;
        }

        private static DateTime GetDate(Modifier modifier, char letter)
        {
            switch (modifier)
            {
                case Modifier.Now:
                    return DateTime.Now;
                case Modifier.UtcNow:
                    return DateTime.UtcNow;
                case Modifier.FirstDay:
                    switch (letter)
                    {
                        case 'M':
                            return DateTime.Today.StartOfMonth();
                        case 'q':
                        case 'Q':
                            return DateTime.Today.StartOfQuarter();
                        case 'y':
                        case 'Y':
                            return DateTime.Today.StartOfYear();
                    }
                    break;
            }
            return DateTime.Today;
        }

        internal static Modifier GetModifier(this string str)
        {
            if (str.EndsWith("/now", StringComparison.OrdinalIgnoreCase) || str.EndsWith("-now", StringComparison.OrdinalIgnoreCase))
            {
                return Modifier.Now;
            }
            else if (str.EndsWith("/utc", StringComparison.OrdinalIgnoreCase))
            {
                return Modifier.UtcNow;
            }
            else if (str.EndsWith("/1", StringComparison.OrdinalIgnoreCase))
            {
                return Modifier.FirstDay;
            }

            return Modifier.None;
        }

        internal static string TrimModifier(this string str, Modifier modifier)
        {
            switch (modifier)
            {
                case Modifier.Now:
                    return str.Substring(0, str.Length - "/now".Length);
                case Modifier.UtcNow:
                    return str.Substring(0, str.Length - "/utc".Length);
                case Modifier.FirstDay:
                    return str.Substring(0, str.Length - "/1".Length);
                default:
                    return str;
            }
        }

        internal enum Modifier
        {
            None = 0,
            Now,
            UtcNow,
            FirstDay,
        }
    }
}