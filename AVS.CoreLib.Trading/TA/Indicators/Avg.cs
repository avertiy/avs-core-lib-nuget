#nullable enable
using System.Collections.Generic;

namespace AVS.CoreLib.Trading.TA.Indicators
{
    public class AvgCalculator
    {
        private Queue<decimal> Values { get; set; }
        private decimal _sum;
        public int Length { get; set; }

        public AvgCalculator(int length)
        {
            Length = length;
            Values = new Queue<decimal>(length);
        }

        public decimal Process(decimal src)
        {
            if (Values.Count == Length)
            {
                var removedValue = Values.Dequeue();
                _sum -= removedValue;
            }

            Values.Enqueue(src);
            _sum += src;

            var ma = _sum / Length;
            return ma;
        }
    }
}