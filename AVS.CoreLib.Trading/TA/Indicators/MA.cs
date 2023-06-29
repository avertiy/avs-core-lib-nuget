#nullable enable
using AVS.CoreLib.Extensions;
using System.Collections.Generic;
using AVS.CoreLib.Trading.Abstractions.XBars;
using AVS.CoreLib.Trading.Enums.Bars;

namespace AVS.CoreLib.Trading.TA.Indicators
{
    public class MA : IMAValue
    {
        public decimal Average { get; set; }
        public decimal Distance { get; set; }
        public Color Color { get; set; }
    }

    public class SMACalculator<T> where T: class, IMAValue, new()
    {
        protected Queue<decimal> Values { get; set; }
        public int Length { get; set; }
        private decimal _sum;
        private decimal _prevMA;

        public SMACalculator(int length)
        {
            Length = length;
            Values = new Queue<decimal>(length);
        }

        public virtual T? Process(decimal price)
        {
            if (Values.Count == Length)
            {
                var removedValue = Values.Dequeue();
                _sum -= removedValue;
            }

            Values.Enqueue(price);
            _sum += price;
            if (Values.Count < Length)
            {
                return default;
            }

            var decimalPlaces = price.GetDecimalPlaces();
            var ma = _sum / Length;
            var dist = ma <= price ? (price - ma) / ma : -(ma - price) / price;

            var color = _prevMA switch
            {
                > 0 => _prevMA > ma ? Color.Red : Color.Green,
                _ => Color.None
            };

            var sma = new T()
            {
                Average = ma.Round(decimalPlaces),
                Distance = dist.Round(4),
                Color = color,
            };

            _prevMA = ma;
            return sma;
        }

        public virtual void Reset()
        {
            Values.Clear();
            _sum = 0;
            _prevMA = 0;
        }
    }
}