using System.Collections.Generic;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public static class PrimeCandles
    {
        public static List<Prime> GetCandles(this List<int> primes, int distance)
        {
            var candles = new List<Prime>();
            int prev = 0;
            for (var i = 0; i < primes.Count - 2; i++)
            {
                if (primes[i] < distance)
                    continue;

                if (prev > 0 && primes[i] - prev < distance)
                    continue;
                var prime = Prime.FromPrimeNumber((ulong)primes[i]);
                candles.Add(prime);
                prev = primes[i];
            }

            return candles;
        }

        private static List<Prime> _candles;

        public static List<Prime> Candles
        {
            get { return _candles ??= GetCandles(SmallPrimes.Set65536, 255); }
        }

    }
}