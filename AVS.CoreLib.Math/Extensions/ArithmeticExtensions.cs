using System;
using System.Collections.Generic;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers;

namespace AVS.CoreLib.Math.Extensions
{
    public static class ArithmeticExtensions
    {
        public static bool TryPickPrimeNumberForIntSqrt(this ulong n, out uint prime)
        {
            var primes = SmallPrimes.First1000;
            foreach (var x in primes)
            {
                var sqrt = System.Math.Sqrt(((double)n * x));
                if (sqrt.IsIntegerValue())
                {
                    prime = (uint)x;
                    return true;
                }
            }

            prime = 0;
            return false;
        }

        public static long[] SplitOnMultipliers(this long n)
        {
            if (n < 0)
                n *= -1;
            if (n <= 3)
                return new[] { n };

            var result = new List<long>();
            var div = 2L;
            while (n % div == 0)
            {
                result.Add(div);
                n /= div;
            }

            div = 3;

            while (System.Math.Pow(div, 2) <= n)
            {
                if (n % div == 0)
                {
                    result.Add(div);
                    n /= div;
                }
                else
                {
                    div += 2;
                }
            }

            if (n > 1)
            {
                result.Add(n);
            }

            return result.ToArray();
        }

        public static ulong[] SplitOnMultipliers(this ulong n)
        {
            if (n <= 3)
                return new[] { n };

            var result = new List<ulong>();
            var div = 2UL;
            while (n % div == 0)
            {
                result.Add(div);
                n /= div;
            }

            div = 3;

            while (System.Math.Pow(div, 2) <= n)
            {
                if (n % div == 0)
                {
                    result.Add(div);
                    n /= div;
                }
                else
                {
                    div += 2;
                }
            }

            if (n > 1)
            {
                result.Add(n);
            }

            return result.ToArray();
        }

        public static ulong ComputeFactorialUL(this int n)
        {
            if (n <= 0)
            {
                return 0UL;
            }

            var f = 1UL;
            var i = (uint)n;
            while (i > 1)
            {
                f = f * i;
                i--;
            }

            return f;
        }

        public static int ComputeFactorial(this int n)
        {
            if (n <= 0)
            {
                return 0;
            }

            var f = 1;
            while (n > 1)
            {
                f = f * n;
                n--;
            }

            return f;
        }

        public static int ComputePartialFactorial(this int n, int count)
        {
            if (n < count)
                throw new ArgumentException($"Count {count} should not exceed N {n}");

            if (n <= 0)
                throw new ArgumentException($"Count {count} should be positive number");

            var f = 1;
            while (n > 1 && count > 0)
            {
                f = f * n;
                n--;
                count--;
            }

            return f;
        }

        public static int ComputePartialSumFactorial(this int n, int count)
        {
            if (count == 0)
                return 0;

            if (n < count)
                throw new ArgumentException($"Count {count} should not exceed N {n}");

            if (n <= 0)
                throw new ArgumentException($"Count {count} should be positive number");

            var f = 0;
            while (n > 1 && count > 0)
            {
                f += n;
                count--;
            }

            return f;
        }

        /// <summary>
        /// sum factorial N = N +(N-1)+(N-2) .. + 1 = (n*n+n)/2;
        /// </summary>
        public static int ComputeSumFactorial(this int n)
        {
            return ComputeTriangularNumber(n);
        }

        /// <summary>
        /// same as factorial calculation but instead of '*' the '+' operand is applied.
        /// e.g. triangular number of 5 = 5+4+3+2+1 = 15
        /// </summary>
        public static int ComputeTriangularNumber(this int n)
        {
            return (n * n + n) / 2;
        }

        /// <summary>
        /// evaluate where value is whether it is closer to a or to b or it is somewhere in between a and b
        /// </summary>
        /// <returns>
        /// -1 - if the value is closer to a
        /// 0  - if the value is somewhere in between  a and b
        /// 1  - if the value is closer to b
        /// </returns>
        public static int EvaluateValue(int value, int a, int b)
        {
            var median = (b - a) / 2;
            var k = 0.25;
            var medianLowBorder = median * (1 - k);
            var medianUpperBorder = median * (1 + k);

            if (value < medianLowBorder)
                return -1;

            return value < medianUpperBorder ? 0 : 1;
        }

        /// <summary>
        /// check whether the value is about equal i.e. value+- % == valueToCompare  
        /// </summary>
        public static bool IsAboutEqualTo(this int value, int valueToCompare, double k = 0.1)
        {
            if (value == valueToCompare)
                return true;

            var min = valueToCompare * (1 - k);
            var max = valueToCompare * (1 + k);
            return value >= min && value <= max;
        }

        public static int Pow(this int n, int pow)
        {
            return (int)System.Math.Pow(n, pow);
        }
    }
}