using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Dates
{
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

        public bool HasValue => To > From;
        public double TotalDays => (To - From).TotalDays;
        public double TotalSeconds => (To - From).TotalSeconds;

        public int Days => Convert.ToInt32(TotalDays);
        public int Hours => Convert.ToInt32(TotalSeconds / 3600);
        public int Seconds => Convert.ToInt32(TotalSeconds);

        public DateRange(DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentException($"From date must be less or equal to date (({from:g}) <= {to:g})");

            From = from;
            To = to;
            
        }

        public DateRange(DateRange other)
        {
            From = other.From;
            To = other.To;
        }

        public bool Contains(DateTime date)
        {
            return From <= date && To >= date;
        }

        public bool Contains(DateRange range)
        {
            return From <= range.From && To >= range.To;
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

        public override bool Equals(object obj)
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
        ///   </list>
        /// </param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return $"[{From.ToString(format)};{To.ToString(format)}]";
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
        
        public static DateRange FromNow(int seconds, bool useUtcTime = false)
        {
            var now = useUtcTime ? DateTime.UtcNow : DateTime.Now;
            if (seconds < 0)
            {
                return new DateRange(now.AddSeconds(seconds), now);
            }
            else
            {
                return new DateRange(now, now.AddSeconds(seconds));
            }
        }

        public static DateRange FromNow(DateTime from, bool useUtcTime = false)
        {
            var now = useUtcTime ? DateTime.UtcNow : DateTime.Now;
            return new DateRange(from, now);
        }

        public static DateRange Create(DateTime from, int days)
        {
            return new DateRange(from, from.AddDays(days));
        }

        public static DateRange FromSource<T>(IList<T> source, Func<T,DateTime> selector)
        {
            var from = source.Min(selector);
            var to = source.Max(selector);
            return new DateRange(from, to);
        }

        public static DateRange FromToday(DateTime from)
        {
            return new DateRange(from, DateTime.Today);
        }

        public static DateRange FromToday(int days)
        {
            if (days < 0)
                return new DateRange(DateTime.Today.AddDays(days), DateTime.Today);
            else
                return new DateRange(DateTime.Today, DateTime.Today.AddDays(days));
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

        public static IEnumerable<DateRange> Iterate(this DateRange range, int seconds)
        {
            for (var i = range.From; i < range.To;)
            {
                var next = i.AddSeconds(seconds);

                if (next > range.To)
                    next = range.To;

                yield return new DateRange(i, next);
                i = next;
            }
        }

        public static IEnumerable<DateRange> IterateByMonths(this DateRange range, int months = 1, OrderBy orderBy = OrderBy.Asc)
        {
            if (orderBy == OrderBy.Asc)
            {
                var i = range.From;
                var startOfMonth = i.StartOfMonth();
                DateTime next;
                if (i > startOfMonth)
                {
                    next = startOfMonth.AddMonths(months).AddDays(-1);
                    yield return new DateRange(i, next);
                }

                while (true)
                {
                    next = i.AddMonths(months);

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

        public static IEnumerable<DateRange> IterateByDays(this DateRange range, int days, OrderBy orderBy = OrderBy.Asc)
        {
            if (orderBy == OrderBy.Asc)
            {
                var i = range.From;
                while (true)
                {
                    var next = i.AddDays(days);

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
                    var prev = i.AddDays(-days);

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

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
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

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return base.GetStandardValues(context);
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

            writer.WriteStartObject();
            writer.WritePropertyName("from");
            writer.WriteStringValue(value.From.ToString("G", CultureInfo.InvariantCulture));
            writer.WritePropertyName("to");
            writer.WriteStringValue(value.To.ToString("G", CultureInfo.InvariantCulture));
            writer.WriteEndObject();
        }
    }
}