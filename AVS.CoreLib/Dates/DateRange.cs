using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Dates
{
    //TODO Add slicing range on pieces by week number: week #10, week #11 etc.

    /// <summary>
    /// represents period from - to 
    /// support string literals e.g. today, yesterday, day, week, month, 15m, 30m, 1h, 24h, 1d,2d, 1M, 1Q, 1Y etc.
    /// </summary>
    [TypeConverter(typeof(DateRangeTypeConverter))]
    [JsonConverter(typeof(DateRangeJsonConverter))]
    [Description("Represent a date range [from;to] where from and to are DateTime values. For a convenience DateRange can be parsed from a string literals, short literals like 3h - 3 hours, 999D - 999 days etc. and text literals like : recent, today, yesterday, past-week, etc.")]
    public readonly struct DateRange
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public bool HasValue => TotalDays > 0;
        public double TotalDays => (To - From).TotalDays;
        public double TotalSeconds => (To - From).TotalSeconds;
        public int Days => Convert.ToInt32(TotalDays);
        public int Hours => Convert.ToInt32(TotalSeconds / 3600);
        public int Seconds => Convert.ToInt32(TotalSeconds);

        #region c-tors
        /// <summary>
        /// Creates DateRange from till <see cref="DateTime.Now"/>
        /// </summary>
        public DateRange(DateTime from)
        {
            var now = DateTime.Now;

            if (from > now)
                throw new ArgumentOutOfRangeException(nameof(from),$"From date ({from:g}) must be less than now ({now:g})");

            From = from;
            To = now;
        }

        public DateRange(DateTime from, int days)
        {
            if (days <= 0)
                throw new ArgumentOutOfRangeException(nameof(days), "Days number must be greater than 0");

            From = from;
            To = from.AddDays(days);
        }

        public DateRange(DateTime from, double milliseconds)
        {
            if (milliseconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds), "Days number must be greater than 0");

            From = from;
            To = from.AddMilliseconds(milliseconds);
        }

        public DateRange(DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentOutOfRangeException($"From date ({from:g}) must be less than To date ({to:g})");

            From = from;
            To = to;
        }

        public DateRange((DateTime from, DateTime to) range)
        {
            if (range.from > range.to)
                throw new ArgumentException($"From date ({range.from:g}) must be less than To date ({range.to:g})");

            From = range.from;
            To = range.to;
        }

        public DateRange(DateRange other)
        {
            From = other.From;
            To = other.To;
        } 

        #endregion

        public bool Contains(DateTime date)
        {
            return From <= date && To >= date;
        }

        public bool Contains(DateRange range)
        {
            return From <= range.From && To >= range.To;
        }

        /// <summary>
        /// Increase/decrease date range by adding/subtracting milliseconds from <see cref="To"/> property
        /// </summary>
        public DateRange Add(double milliseconds)
        {
            var to = To.AddMilliseconds(milliseconds);
            return new DateRange(From, to);
        }

        /// <summary>
        /// Increase/decrease date range by adding/subtracting milliseconds to <see cref="From"/> and <see cref="To"/> properties
        /// </summary>
        public DateRange Add(double millisecondsFrom, double millisecondsTo)
        {
            var from = From.AddMilliseconds(millisecondsFrom);
            var to = From.AddMilliseconds(millisecondsTo);
            return new DateRange(from, to);
        }

        public DateRange Shift(int days)
        {
            return new DateRange(From.AddDays(days), To.AddDays(days));
        }

        public DateRange Shift(double milliseconds)
        {
            return new DateRange(From.AddMilliseconds(milliseconds), To.AddMilliseconds(milliseconds));
        }

        #region implicit conversions
        public static implicit operator DateRange((DateTime, DateTime) tuple)
        {
            return new DateRange(tuple.Item1, tuple.Item2);
        }

        public static implicit operator DateRange(string value)
        {
            if (TryParse(value, out DateRange range))
                return range;

            throw new Exception($"String '{value}' is not valid DateRange value");
        }
        #endregion

        #region Compare operators overloading
        public bool Equals(DateRange other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object? obj)
        {
            return obj is DateRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }

        public static bool operator ==(DateRange dateRange, DateRange compare)
        {
            return Math.Abs(dateRange.TotalSeconds - compare.TotalSeconds) < 0.1;
        }

        public static bool operator !=(DateRange dateRange, DateRange compare)
        {
            return Math.Abs(dateRange.TotalSeconds - compare.TotalSeconds) > 0.1;
        }

        public static bool operator >=(DateRange dateRange, DateRange compare)
        {
            return dateRange.TotalSeconds >= compare.TotalSeconds;
        }

        public static bool operator <=(DateRange dateRange, DateRange compare)
        {
            return dateRange.TotalSeconds <= compare.TotalSeconds;
        }
        public static bool operator >(DateRange dateRange, DateRange compare)
        {
            return dateRange.TotalSeconds > compare.TotalSeconds;
        }

        public static bool operator <(DateRange dateRange, DateRange compare)
        {
            return dateRange.TotalSeconds < compare.TotalSeconds;
        }

        //public static bool operator >=(DateRange dateRange, int periodInSeconds)
        //{
        //    return dateRange.TotalSeconds >= periodInSeconds;
        //}

        //public static bool operator >(DateRange dateRange, int periodInSeconds)
        //{
        //    return dateRange.TotalSeconds > periodInSeconds;
        //}

        //public static bool operator <(DateRange dateRange, int periodInSeconds)
        //{
        //    return dateRange.TotalSeconds < periodInSeconds;
        //}

        //public static bool operator <=(DateRange dateRange, int periodInSeconds)
        //{
        //    return dateRange.TotalSeconds <= periodInSeconds;
        //}


        #endregion

        public override string ToString()
        {
            return $"[{From:G};{To:G}]";
        }

        /// <summary>
        /// Format DateRange to a string [From;To].
        /// Formatting date time values accordingly to the given date time format modifier
        /// </summary>
        /// <param name="format">support same format modifiers as for <see cref="DateTime.ToString()"/>:
        ///   <list type="table">
        ///     <item>
        ///       <term>D</term>
        ///       <description> - long date:  Friday, 18 April 2023</description>
        ///     </item>
        ///     <item>
        ///       <term>d</term>
        ///       <description> - short date:  04/18/2023</description>
        ///     </item>
        ///     <item>
        ///       <term>G</term>
        ///       <description> - general long:  04/18/2023 06:00:00</description>
        ///     </item>
        ///     <item>
        ///       <term>g</term>
        ///       <description> - general short:  04/18/2023 06:00 etc.</description>
        ///     </item>
        ///     <item>
        ///       <term>P - period</term>
        ///       <description> - general long e.g. 04/18/2023 06:00:00 AM - 31/12/2024 9:00:00 PM</description>
        ///     </item>
        ///     <item>
        ///   </list>
        /// </param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return format == "P" 
                ? $"[{From:G} - {To:G}]"
                : $"[{From.ToString(format)};{To.ToString(format)}]";
        }

        #region static methods

        /// <summary>
        /// parse date range almost from any possible literal:
        /// number 1-999 + letter (s)econds, (m)inutes, (h)ours, (d)ays, (w)eeks, (m)onths, (q)uaters, (y)ears e.g. 10M - 10 months
        /// literals such as day, week, month, quarter, year
        /// exact ranges: 01/10/2019 - 02/11/2019, 01/10/2019;02/11/2019 or [01/10/2019;02/11/2019] <seealso cref="DateTime.TryParse(string?, IFormatProvider?, DateTimeStyles, out DateTime)"/> 
        /// toDate modifier `-now` e.g. 2Q-now => [DateTime.Today.AddMonths(-6);DateTime.Now], by default it is till DateTime.Today 
        /// </summary>
        public static bool TryParse(string period, out DateRange range)
        {
            range = new DateRange();
            if (string.IsNullOrEmpty(period))
                return false;

            var str = period.Trim();

            //[01/01/2023;01/03/2023]
            if (str.StartsWith('[') && str.EndsWith(']'))
            {
                return DateRangeHelper.TryParseExact(str.Substring(1, period.Length - 2), out range);
            }

            return DateRangeHelper.TryParseFromLiterals(str, out range) || DateRangeHelper.TryParseExact(str, out range);
        }

        public static DateRange Create(DateTime from)
        {
            return new DateRange(from, DateTime.Today);
        }

        [Obsolete("Use c-tor: new DateRange(from, days);")]
        public static DateRange Create(DateTime from, int days)
        {
            return new DateRange(from, from.AddDays(days));
        }

        public static DateRange Create(DateTime from, double milliseconds)
        {
            return new DateRange(from, from.AddMilliseconds(milliseconds));
        }

        public static DateRange FromToday(int days)
        {
            return days < 0 
                ? new DateRange(DateTime.Today.AddDays(days), DateTime.Today)
                : new DateRange(DateTime.Today, DateTime.Today.AddDays(days));
        }

        public static DateRange FromNow(int seconds, bool utcTime = false)
        {
            var now = utcTime ? DateTime.UtcNow : DateTime.Now;
            if (seconds < 0)
            {
                return new DateRange(now.AddSeconds(seconds), now);
            }
            else
            {
                return new DateRange(now, now.AddSeconds(seconds));
            }
        }

        [Obsolete("Use c-tor: new DateRange(from)")]
        public static DateRange FromNow(DateTime from, bool utcTime = false)
        {
            var now = utcTime ? DateTime.UtcNow : DateTime.Now;
            return new DateRange(from, now);
        }

        [Obsolete("Use extension method for source.GetDataRange(selector)")]
        public static DateRange? FromSource<T>(IList<T> source, Func<T, DateTime> selector, Sort sort = Sort.None)
        {
            if (source.Count == 0)
                return null;

            switch (sort)
            {
                case Sort.Asc:
                    return new DateRange(from: selector(source[0]), to: selector(source[1]));
                case Sort.Desc:
                    return new DateRange(from: selector(source[^1]), to: selector(source[0]));
                case Sort.None:
                default:
                    var (from, to) = source.MinMax(selector);
                    return new DateRange(from, to);
            }
        }

        public static DateRange FromWeek(int weekNumber, int days = 6, int year = 0)
        {
            if (weekNumber < 1 || weekNumber > 53)
                throw new ArgumentOutOfRangeException(nameof(weekNumber), "Week number must be between 1 and 53.");

            if (days <= 0)
                throw new ArgumentOutOfRangeException(nameof(days), "Days number must be greater than 0");

            // Get the first day of the year
            var yy = year >= 0 ? year : DateTime.Today.Year;

            var firstDayOfYear = new DateTime(yy, 1, 1);

            // ISO 8601: Get the Thursday of the first week of the year
            // Thursday is used as a reference because it's the middle day of the week
            int daysToFirstThursday = (DayOfWeek.Thursday - firstDayOfYear.DayOfWeek + 7) % 7;
            var firstThursdayOfYear = firstDayOfYear.AddDays(daysToFirstThursday);

            // Calculate the first day of the specified week
            var firstDayOfWeek = firstThursdayOfYear.AddDays((weekNumber - 1) * 7 - 3);

            // The end date is 6 days after the start date
            var lastDayOfWeek = firstDayOfWeek.AddDays(days);

            return new DateRange(firstDayOfWeek, lastDayOfWeek);
        }

        public static DateRange Empty => new(DateTime.MinValue, DateTime.MinValue);
        public static DateRange Day => new(DateTime.Today.AddDays(-1), DateTime.Today);
        public static DateRange Week => new(DateTime.Today.AddDays(-7), DateTime.Today);
        public static DateRange TwoWeeks => new(DateTime.Today.AddDays(-14), DateTime.Today);
        public static DateRange Month => new(DateTime.Today.AddMonths(-1), DateTime.Today);
        public static DateRange Quarter => new(DateTime.Today.AddMonths(-3), DateTime.Today);
        public static DateRange Year => new(DateTime.Today.AddYears(-1), DateTime.Today);
        #endregion
    }


    public static class DateRangeExtensions
    {
        public static DateRange? GetDataRange<T>(this IList<T> source, Func<T, DateTime> selector, Sort sort = Sort.None)
        {
            if (source.Count == 0)
                return null;

            switch (sort)
            {
                case Sort.Asc:
                    return new DateRange(from: selector(source[0]), to: selector(source[1]));
                case Sort.Desc:
                    return new DateRange(from: selector(source[^1]), to: selector(source[0]));
                case Sort.None:
                default:
                    var (from, to) = source.MinMax(selector);
                    return new DateRange(from, to);
            }
        }

        /// <summary>
        /// converts date range (from/to) to UTC time
        /// </summary>
        public static DateRange ToUniversalTime(this DateRange range)
        {
            var utc = new DateRange(range.From.ToUniversalTime(), range.To.ToUniversalTime());
            return utc;
        }

        /// <summary>
        /// combine both ranges, taking the earliest From  and the latest To
        /// </summary>
        public static DateRange Combine(this DateRange range, DateRange other)
        {
            return new DateRange(range.From.TakeEarliest(other.From), range.To.TakeLatest(other.To));
        }

        public static DateRange[] Split(this DateRange range, int timespanInSeconds)
        {
            return range.Iterate(timespanInSeconds).ToArray();
        }

        public static DateRange[] SplitByDays(this DateRange range, int days)
        {
            return range.IterateByDays(days).ToArray();
        }

        public static IEnumerable<DateRange> Iterate(this DateRange range, int timespanInSeconds)
        {
            for (var i = range.From; i < range.To;)
            {
                var next = i.AddSeconds(timespanInSeconds);

                if (next > range.To)
                    next = range.To;

                yield return new DateRange(i, next);
                i = next;
            }
        }

        public static IEnumerable<DateRange> IterateByMonths(this DateRange range, int months = 1, Sort orderBy = Sort.Asc)
        {
            if (orderBy == Sort.Asc)
            {
                var i = range.From;
                var startOfMonth = i.StartOfMonth();
                DateTime next;
                if (i > startOfMonth)
                {
                    next = startOfMonth.AddMonths(months).AddDays(-1);
                    yield return new DateRange(i, next);
                    i = next;
                }

                while (true)
                {
                    next = i.AddMonths(months).StartOfMonth();

                    if (next > range.To)
                    {
                        yield return new DateRange(i, range.To);
                        break;
                    }

                    yield return new DateRange(i, next);
                    i = next;
                }
            }
            else
            {
                var i = range.To;
                var startOfMonth = i.StartOfMonth();
                if (i > startOfMonth)
                {
                    yield return new DateRange(startOfMonth, i);
                    i = startOfMonth;
                }

                while (true)
                {
                    var prev = i.AddMonths(-months);

                    if (prev < range.From)
                    {
                        yield return new DateRange(range.From, i);
                        break;
                    }

                    yield return new DateRange(prev, i);
                    i = prev;
                }
            }
        }

        public static IEnumerable<DateRange> IterateByDays(this DateRange range, int days, Sort orderBy = Sort.Asc)
        {
            if (orderBy == Sort.Asc)
            {
                var i = range.From;
                while (true)
                {
                    var next = i.AddDays(days).Date;

                    if (next > range.To)
                    {
                        yield return new DateRange(i, range.To);
                        break;
                    }

                    yield return new DateRange(i, next);
                    i = next;
                }
            }
            else
            {
                var i = range.To;
                while (true)
                {
                    var prev = i.AddDays(-days).Date;

                    if (prev < range.From)
                    {
                        yield return new DateRange(range.From, i);
                        break;
                    }

                    yield return new DateRange(prev, i);
                    i = prev;
                }
            }
        }
    }

    public class DateRangeTypeConverter : TypeConverter
    {
        protected virtual bool TryParse(string str, out DateRange range)
        {
            return DateRange.TryParse(str, out range);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context,
            CultureInfo? culture, object value)
        {
            if (value is string)
            {
                if (TryParse((string)value, out var range))
                {
                    return range;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DateRangeJsonConverter : JsonConverter<DateRange>
    {
        private DateRange ParseAsObject(ref Utf8JsonReader reader)
        {
            reader.Read();
            var prop1Name = reader.GetString();
            reader.Read();
            var prop1Value = reader.GetString();

            reader.Read();
            var prop2Name = reader.GetString();
            reader.Read();
            var prop2Value = reader.GetString();

            if (prop1Name?.ToLower() != "from")
                throw new JsonException($"Unable to parse {nameof(DateRange)} - expected prop name `from`");

            if (!DateTime.TryParse(prop1Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var from))
                throw new JsonException(
                    $"Unable to parse {nameof(DateRange)} - `from` value `{prop1Value}` is invalid DateTime value");

            if (prop2Name?.ToLower() != "to")
                throw new JsonException($"Unable to parse {nameof(DateRange)} - expected prop name `to`");

            if (!DateTime.TryParse(prop2Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var to))
                throw new JsonException(
                    $"Unable to parse {nameof(DateRange)} - `to` value `{prop2Value}` is invalid DateTime value");

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"Unable to parse {nameof(DateRange)} - end object token is expected at {reader.TokenStartIndex}");

            return new DateRange(from, to);
        }

        public override DateRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString()!;
                if (DateRange.TryParse(str, out var range))
                {
                    return range;
                }

                throw new JsonException($"Unable to parse {nameof(DateRange)} from `{str}`");
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                return ParseAsObject(ref reader);
            }

            throw new JsonException($"Unable to parse {nameof(DateRange)} - token type {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateRange value, JsonSerializerOptions options)
        {
            //var json = $"{{ \"from\": \"{value.From:G}\", \"to\": \"{value.To:G}\" }}";
            //writer.WriteStringValue(json);
            //var text = $"{value.From:dd/MM/yyyy HH:mm:ss} - {value.To:dd/MM/yyyy HH:mm:ss}";

            // JavaScript supports only one  date time string format of the ISO 8601 `YYYY-MM-DDTHH:mm:ss.sssZ`
            // which is in C# yyyy-MM-ddTHH:mm:ss.fffZ or simply :O 
            var text = $"{value.From:O} - {value.To:O}";
            writer.WriteStringValue(text);
        }
    }
}