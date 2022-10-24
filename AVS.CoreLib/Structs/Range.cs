using System;
using System.Globalization;

namespace AVS.CoreLib.Structs
{
    public readonly struct Range<T> where T : IComparable<T>
    {
        public T Min { get; }
        public T Max { get; }

        public decimal Avg => (Convert.ToDecimal(Max) + Convert.ToDecimal(Min)) / 2;

        public decimal Spread =>
            Math.Round(((Convert.ToDecimal(Max) - Convert.ToDecimal(Min)) / Convert.ToDecimal(Max)) * 100, 4,
                MidpointRounding.AwayFromZero);

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
            if (Max.CompareTo(Min) < 0)
                throw new ArgumentException("Max must be greater than Min");
        }

        public Range(Range<T> other)
        {
            this = other;
        }

        public bool Contains(T value, bool inclusiveRange = true)
        {
            if(inclusiveRange)
                return Min.CompareTo(value) <= 0 && Max.CompareTo(value) >= 0;
            else
                return Min.CompareTo(value) < 0 && Max.CompareTo(value) > 0;
        }

        public bool Contains(Range<T> range, bool inclusiveRange = true)
        {
            if (inclusiveRange)
                return Min.CompareTo(range.Min) <= 0 && Max.CompareTo(range.Max) >= 0;
            else
                return Min.CompareTo(range.Min) < 0 && Max.CompareTo(range.Max) > 0;
        }

        public override string ToString()
        {
            return $"[{Min};{Max}]";
        }

        #region comparison operators
        public static bool operator <(Range<T> range1, Range<T> range2)
        {
            return range1.Avg < range2.Avg;
        }

        public static bool operator >(Range<T> range1, Range<T> range2)
        {
            return range1.Avg > range2.Avg;
        }

        public static bool operator <=(T price, Range<T> range)
        {
            return price.CompareTo(range.Max) <= 0;
        }

        public static bool operator >=(T price, Range<T> range)
        {
            return price.CompareTo(range.Min) >= 0;
        }
        #endregion

        public static bool TryParse(string str, out Range<decimal> value)
        {
            value = default;
            if (string.IsNullOrEmpty(str))
                return false;

            var parts = str.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;
            if (decimal.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal min) &&
                decimal.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal max))
            {
                value = new Range<decimal>(min, max);
                return true;
            }
            return false;
        }

        public static implicit operator Range<T>((T, T) tuple)
        {
            return new Range<T>(tuple.Item1, tuple.Item2);
        }

        public static Range<T> Create(T min, T max)
        {
            return new Range<T>(min, max);
        }
    }
}