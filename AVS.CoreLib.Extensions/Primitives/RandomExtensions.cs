using System;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions
{
    public static class RandomExtensions
    {
        public static decimal NextDecimal(this Random rand)
        {
            return (decimal)rand.NextDouble();
        }
        
        public static decimal NextDecimal(this Random random, decimal minValue, decimal maxValue, bool inclusive = false)
        {
            Guard.Against.Null(random);
            Guard.MustBe.LessThanOrEqual(minValue, maxValue);

            // 1. Generate a random decimal between 0 and 1
            var sample = (decimal)random.NextDouble();

            // 2. Scale to desired range
            if (inclusive)
                return minValue + (sample * (maxValue - minValue));

            var epsilon = minValue / 100;
            return minValue + epsilon + (sample * (maxValue - minValue - epsilon));
        }

        public static decimal GetRandomPrice(this Random random, decimal minValue, decimal maxValue, bool inclusive = false)
        {
            var price = minValue <= maxValue
                ? random.NextDecimal(minValue, maxValue, inclusive)
                : random.NextDecimal(maxValue, minValue, inclusive);
            return price.RoundPrice();
        }
    }
}