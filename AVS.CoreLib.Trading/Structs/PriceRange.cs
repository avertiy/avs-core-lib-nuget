using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.Trading.Structs
{
    public readonly struct PriceRange
    {
        public PriceRange(decimal min, decimal max)
        {
            Min = min;
            Max = max;
            if (Max < min)
                throw new ArgumentException("Max must be greater than Min");
        }

        public decimal Min { get; }
        public decimal Max { get; }
        [JsonIgnore]
        public decimal Avg => (Max + Min) / 2;

        public bool Match(decimal price)
        {
            return Min <= price && price <= Max;
        }

        public decimal GetDiff(decimal price)
        {
            if (price > Max)
                return Math.Round((price - Max) / Max, 3);
            if (price < Min)
                return Math.Round((Min - price) / Min, 3);
            return 0.0m;
        }

        public bool Contains(PriceRange range)
        {
            return Min <= range.Min && Max >= range.Max;
        }

        public decimal GetLength() => Math.Round((Max - Min) / Min * 100, 2);

        #region comparison operators
        public static bool operator <(PriceRange range1, PriceRange range2)
        {
            return range1.Avg < range2.Avg;
        }

        public static bool operator >(PriceRange range1, PriceRange range2)
        {
            return range1.Avg > range2.Avg;
        }

        public static bool operator <=(decimal price, PriceRange range)
        {
            return price <= range.Max;
        }

        public static bool operator >=(decimal price, PriceRange range)
        {
            return price >= range.Min;
        }
        #endregion

        public override string ToString()
        {
            return X.Format($"[{Min:price};{Max:price}]");
        }

        public PriceRange[] Split(decimal step)
        {
            decimal price = Min;
            int n = (int)(GetLength() / step);
            var list = new List<PriceRange>(n);
            do
            {
                var range = new PriceRange(price, price * (1 + step));
                list.Add(range);
                price = range.Max;
            } while (price <= Max);

            return list.ToArray();
        }

        public static implicit operator PriceRange((decimal, decimal) tuple)
        {
            return new PriceRange(tuple.Item1, tuple.Item2);
        }

        public static PriceRange Create(decimal min, decimal max)
        {
            return new PriceRange(min, max);
        }
    }
}