using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions
{
    public static class PrimeExtensions
    {
        public static NumberInfo GetNumberInfo(this ulong n) => new NumberInfo(n);

        public static Prime GetNextPrime(this Prime prime, params int[] lastDigits)
        {
            return lastDigits.Any() ? prime.N.GetNextPrime(lastDigits) : prime.N.GetNextPrime();
        }

        public static Prime GetPrevPrime(this Prime prime, params int[] lastDigits)
        {
            var n = prime.N - 1;
            return lastDigits.Any() ? n.GetNearestPrime(lastDigits) : n.GetNearestPrime();
        }

        private static int GetBit(ulong n, int digit)
        {
            return n.GetLastDigit() == digit ? 1 : 0;
        }

        public static int[] GetPath(this Prime prime)
        {
            if (prime.N < 255)
                return new[] { 0, 0, 0, 0, 0, 0, 0 };

            var d1 = 1;
            var d2 = 7;
            //var d = prime.LastDigit;
            //var d1 = 7;
            //if (d == 7)
            //{
            //    d1 = 3;
            //}

            var next = prime.GetNextPrime(d1, d2);
            var next1 = next.GetNextPrime(d1, d2);
            var prev1 = prime.GetPrevPrime(d1, d2);
            var prev2 = prev1.GetPrevPrime(d1, d2);
            var prev3 = prev2.GetPrevPrime(d1, d2);
            var prev4 = prev3.GetPrevPrime(d1, d2);
            var prev5 = prev4.GetPrevPrime(d1, d2);

            var path = new List<int>
            {
                GetBit(prev5.N, d1),
                GetBit(prev4.N, d1),
                GetBit(prev3.N, d1),
                GetBit(prev2.N, d1),
                GetBit(prev1.N, d1),
                GetBit(next.N, d1),
                GetBit(next1.N, d1)
            };

            return path.ToArray();
        }

        public static int[] GetPath(this ulong n)
        {
            if (n < 255)
                return new[] { 0, 0, 0, 0, 0, 0, 0 };

            var d1 = 1;
            var d2 = 7;
            var next = n.GetNextPrime(d1, d2);
            var next1 = next.GetNextPrime(d1, d2);
            var prev1 = (n - 1).GetPrevPrime(d1, d2);
            var prev2 = prev1.GetPrevPrime(d1, d2);
            var prev3 = prev2.GetPrevPrime(d1, d2);
            var prev4 = prev3.GetPrevPrime(d1, d2);
            var prev5 = prev4.GetPrevPrime(d1, d2);

            var path = new List<int>
            {
                GetBit(prev5,d1),
                GetBit(prev4,d1),
                GetBit(prev3,d1),
                GetBit(prev2,d1),
                GetBit(prev1,d1),
                GetBit(next,d1),
                GetBit(next1,d1),
            };

            return path.ToArray();
        }


        public static int GetN6(this Prime p)
        {
            if (p <= 3)
            {
                throw new ArgumentException("p must be greater than 3");
            }

            if ((p + 1) % 6 == 0)
            {
                return (p + 1) / 6;
            }

            return (p - 1) / 6;
        }

        public static Prime RestorePrimeFromN6(this int n)
        {
            var p1 = n * 6 + 1;
            var p2 = n * 6 - 1;
            var isPrime1 = p1.IsPrime();
            return isPrime1 ? p1 : p2;
        }
    }
}