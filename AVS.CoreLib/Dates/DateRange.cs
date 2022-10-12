using System;
using System.ComponentModel;
using System.Globalization;

namespace AVS.CoreLib.Dates
{
    /// <summary>
    /// represents period from - to 
    /// support string literals e.g. today, yesterday, day, week, month, 15m, 30m, 1h, 24h, 1d,2d, 1M, 1Q, 1Y etc.
    /// </summary>
    [TypeConverter(typeof(DateRangeTypeConverter))]
    public readonly struct DateRange
    {
        public DateTime From { get; }

        public DateTime To { get; }

        public double TotalDays => (To - From).TotalDays;
        public double TotalSeconds => (To - From).TotalSeconds;

        public DateRange(DateRange other)
        {
            From = other.From;
            To = other.To;
        }

        public DateRange(DateTime from, DateTime to)
        {
            From = from;
            To = to;
            if (To < From)
                throw new ArgumentException("To must be greater than From");
        }

        public bool Contains(DateTime date)
        {
            return From <= date && To >= date;
        }

        public bool Contains(DateRange range)
        {
            return From <= range.From && To >= range.To;
        }

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

        #region Compare operators overloading
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

        public static bool operator >=(DateRange dateRange, int periodInSeconds)
        {
            return dateRange.TotalSeconds >= periodInSeconds;
        }

        public static bool operator <=(DateRange dateRange, int periodInSeconds)
        {
            return dateRange.TotalSeconds <= periodInSeconds;
        }

       
        #endregion

        public override string ToString()
        {
            return $"[{From:d};{To:d}]";
        }

        public string ToString(string dateFormat)
        {
            return $"[{From.ToString(dateFormat)};{To.ToString(dateFormat)}]";
        }

        /// <summary>
        /// returns all supported literals parseable into date range
        /// </summary>
        public static string[] GetLiterals()
        {
            return new[]
            {
                "today",
                "yesterday",
                "day",
                "week",
                "month",
                "quarter",
                "year",
                "1m",
                "5m",
                "15m",
                "30m",
                "60m",
                "1h",
                "2h",
                "4h",
                "12h",
                "24h",
                "48h",
                "1d",
                "2d",
                "3d",
                "7d",
                "1w",
                "2w",
                "3w",
                "4w",
                "1M",
                "3M",
                "6M",
                "9M",
                "12M",
                "1Q",
                "2Q",
                "3Q",
                "4Q",
                "1Y",
                "2Y",
                "3Y",
                "5Y",
                "10Y",

            };
        }

        public static bool TryParse(string str, out DateRange range)
        {
            range = new DateRange();
            if (string.IsNullOrEmpty(str))
                return false;

            switch (str.Trim())
            {
                case "today":
                    range = new DateRange(DateTime.Today, DateTime.Now);
                    return true;
                case "yesterday":
                    range = new DateRange(DateTime.Today.AddDays(-1), DateTime.Now);
                    return true;
                case "1m":
                    range = new DateRange(DateTime.Now.AddMinutes(-1), DateTime.Now);
                    return true;
                case "5m":
                    range = new DateRange(DateTime.Now.AddMinutes(-5), DateTime.Now);
                    return true;
                case "15m":
                    range = new DateRange(DateTime.Now.AddMinutes(-15), DateTime.Now);
                    return true;
                case "30m":
                    range = new DateRange(DateTime.Now.AddMinutes(-30), DateTime.Now);
                    return true;
                case "60m":
                case "1h":
                    range = new DateRange(DateTime.Now.AddMinutes(-60), DateTime.Now);
                    return true;
                case "4h":
                    range = new DateRange(DateTime.Now.AddHours(-4), DateTime.Now);
                    return true;
                case "12h":
                    range = new DateRange(DateTime.Now.AddHours(-12), DateTime.Now);
                    return true;
                case "24h":
                    range = new DateRange(DateTime.Now.AddHours(-24), DateTime.Now);
                    return true;
                case "day":
                case "1d":
                    range = new DateRange(DateTime.Today.AddDays(-1), DateTime.Now);
                    return true;
                case "48h":
                case "2d":
                    range = new DateRange(DateTime.Today.AddDays(-2), DateTime.Now);
                    return true;
                case "72h":
                case "3d":
                    range = new DateRange(DateTime.Today.AddDays(-3), DateTime.Now);
                    return true;
                case "7d":
                case "week":
                case "1w":
                    range = new DateRange(DateTime.Today.AddDays(-7), DateTime.Now);
                    return true;
                case "2w":
                    range = new DateRange(DateTime.Today.AddDays(-7 * 2), DateTime.Now);
                    return true;
                case "3w":
                    range = new DateRange(DateTime.Today.AddDays(-7 * 3), DateTime.Now);
                    return true;
                case "4w":
                    range = new DateRange(DateTime.Today.AddDays(-7 * 4), DateTime.Now);
                    return true;
                case "1M":
                case "month":
                    range = new DateRange(DateTime.Today.AddMonths(-1), DateTime.Now);
                    return true;
                case "3M":
                case "quarter":
                case "1Q":
                    range = new DateRange(DateTime.Today.AddMonths(-3), DateTime.Now);
                    return true;
                case "6M":
                case "2Q":
                    range = new DateRange(DateTime.Today.AddMonths(-6), DateTime.Now);
                    return true;
                case "9M":
                case "3Q":
                    range = new DateRange(DateTime.Today.AddMonths(-9), DateTime.Now);
                    return true;
                case "12M":
                case "4Q":
                case "year":
                case "1Y":
                    range = new DateRange(DateTime.Today.AddMonths(-12), DateTime.Now);
                    return true;
                case "2Y":
                    range = new DateRange(DateTime.Today.AddYears(-2), DateTime.Now);
                    return true;
                case "3Y":
                    range = new DateRange(DateTime.Today.AddYears(-3), DateTime.Now);
                    return true;
                case "5Y":
                    range = new DateRange(DateTime.Today.AddYears(-5), DateTime.Now);
                    return true;
                case "10Y":
                    range = new DateRange(DateTime.Today.AddYears(-10), DateTime.Now);
                    return true;
                default:
                    {
                        string[] parts = null;

                        if (str.StartsWith("[") && str.EndsWith("]") && str.Contains(";"))
                        {
                            parts = str.Split(';');
                            if (parts.Length != 2)
                                return false;
                        }
                        else if (str.Contains(" - "))
                        {
                            //01/10/2019 - 02/11/2019
                            parts = str.Split(" - ");
                            if (parts.Length != 2)
                                return false;
                        }

                        if (parts == null)
                            return false;

                        var parse1 = DateTime.TryParse(parts[0], CultureInfo.CurrentCulture, DateTimeStyles.None,
                            out DateTime from);

                        var parse2 = DateTime.TryParse(parts[1], CultureInfo.CurrentCulture, DateTimeStyles.None,
                            out DateTime to);

                        if (parse1 && parse2)
                        {
                            range = new DateRange(from, to);
                            return true;
                        }

                        return false;
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
                if (TryParse((string)value, out DateRange range))
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
}