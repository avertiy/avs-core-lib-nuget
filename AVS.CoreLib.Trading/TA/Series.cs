#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.TA.Calculators;
using AVS.CoreLib.Trading.TA.Indicators;

namespace AVS.CoreLib.Trading.TA
{
    public class Series : Series<decimal>
    {
        public Series(int capacity = 100) : base(capacity)
        {
        }

    }

    public class Series<T> : IEnumerable<T?>
    {
        public List<T?> Items { get; set; }

        public int Count => Items.Count;

        public Series(int capacity = 100)
        {
            Items = new List<T?>(capacity);
        }

        public Series(IEnumerable<T> source)
        {
            Items = new List<T?>(source);
        }

        public T? this[int index] => Items[index];

        public void Add(T? value)
        {
            Items.Add(value);
        }

        public IEnumerator<T?> GetEnumerator()
        {
            foreach (var item in Items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator T?(Series<T> series)
        {
            return series.Items.FirstOrDefault();
        }

        public static implicit operator Series<T>(List<T> items)
        {
            var series = new Series<T>(items);
            return series;
        }
    }

    public static class SeriesExtensions
    {
        public static Series<MA> SMA(this Series source, int length)
        {
            var result = new Series<MA>(source.Count);
            var calc = new SMACalculator<MA>(length);
            foreach (var src in source)
            {
                var val = calc.Process(src);
                result.Add(val);
            }

            return result;
        }

        public static Series<BB> BB(this Series source, int length, decimal stdDev = 2.5m, decimal stdDevNarrow = 1)
        {
            var result = new Series<BB>(source.Count);
            var calc = new BBCalculator(length, stdDev, stdDevNarrow);
            foreach (var src in source)
            {
                var val = calc.Process(src);
                result.Add(val);
            }

            return result;
        }
    }
}