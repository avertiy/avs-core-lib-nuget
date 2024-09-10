using System;

namespace AVS.CoreLib.Extensions
{
    public static class RandomExtensions
    {
        public static decimal NextDecimal(this Random rand)
        {
            return (decimal)rand.NextDouble();
        }

        public static decimal NextDecimal(this Random rand, decimal from, decimal to)
        {
            var d = (decimal)rand.NextDouble();

            start:

            if (d >= from && d <= to)
                return d;

            if (d > to)
            {
                var r = d % (from + to) / 2;
                d = from + r;
                goto start;
            }

            if (d < from)
            {
                var r = (from + to) / 2 % d;
                d = from + r;
                goto start;
            }

            return d;
        }
    }
}