using System;
using System.Linq;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions
{
    public static class NearestPrimeExtensions
    {
        public static int GetNearestPrime(this int n)
        {
            return Convert.ToInt32(GetNearestPrime(Convert.ToUInt64(n)));
        }

        public static int GetNearestPrime(this int n, int lastDigit)
        {
            return Convert.ToInt32(GetNearestPrime(Convert.ToUInt64(n), lastDigit));
        }

        public static uint GetNearestPrime(this uint n)
        {
            return Convert.ToUInt32(GetNearestPrime((ulong)n));
        }

        public static ulong GetNearestPrime(this ulong n, params int[] lastDigits)
        {
            //we assume 0, 1 as primes
            if (n <= 2)
                return n;

            // All prime numbers are odd
            if (n % 2 == 0)
                n--;

            ulong i, j;
            for (i = n; i >= 2; i -= 2)
            {
                var d = (int)i % 10;
                if (!lastDigits.Contains(d))
                    continue;

                for (j = 3; j <= System.Math.Sqrt(i); j += 2)
                {
                    if (i % j == 0)
                        break;
                }
                if (j > System.Math.Sqrt(i))
                    return i;
            }

            // It will only be executed when n is 3
            return 2;

        }

        public static ulong GetNearestPrime(this ulong n)
        {
            //we assume 0, 1 as primes
            if (n <= 2)
                return n;
            // All prime numbers are odd
            if (n % 2 == 0)
                n--;

            ulong i, j;
            for (i = n; i >= 2; i -= 2)
            {
                if (i % 2 == 0)
                    continue;
                for (j = 3; j <= System.Math.Sqrt(i); j += 2)
                {
                    if (i % j == 0)
                        break;
                }
                if (j > System.Math.Sqrt(i))
                    return i;
            }

            // It will only be executed when n is 3
            return 2;

        }

        public static ulong GetPrevPrime(this ulong n, params int[] lastDigits)
        {
            return GetNearestPrime(n - 1, lastDigits);
        }
    }
}