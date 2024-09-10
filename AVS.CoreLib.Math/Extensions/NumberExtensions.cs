using System;
using System.Collections.Generic;
using System.Numerics;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers;

namespace AVS.CoreLib.Math.Extensions
{
    public static class NumberExtensions
    {
        #region SumOfDigits
        public static int SumOfDigits(this BigInteger n)
        {
            var sum = 0;
            while (n != 0)
            {
                sum += Convert.ToInt32(n % 10);
                n /= 10;
            }
            return sum;
        }

        public static int SumOfDigits(this long n)
        {
            var sum = 0;
            while (n != 0)
            {
                sum += Convert.ToInt32(n % 10);
                n /= 10;
            }
            return sum;
        }

        public static int SumOfDigits(this int n)
        {
            var sum = 0;
            while (n != 0)
            {
                sum += Convert.ToInt32(n % 10);
                n /= 10;
            }
            return sum;
        }

        public static int SumOfDigits(this ulong n)
        {
            var sum = 0;
            while (n != 0)
            {
                sum += Convert.ToInt32(n % 10);
                n /= 10;
            }
            return sum;
        }

        public static int SumOfDigits(this uint n)
        {
            return SumOfDigits((ulong)n);
        }

        public static int SumOfDigits(this ushort n)
        {
            return SumOfDigits((ulong)n);
        }
        #endregion

        public static bool IsSpecialPrime(this ulong number)
        {
            var str = number.ToString();
            for (var i = 0; i < str.Length; i++)
            {
                for (var k = 0; k < 10; k++)
                {
                    string str2;
                    if (i + 1 < str.Length)
                    {
                        str2 = str.Substring(0, i) + k + str.Substring(i + 1, str.Length - 1 - i);
                    }
                    else
                    {
                        str2 = str.Substring(0, i) + k;
                    }

                    var L = ulong.Parse(str2);
                    if (L.IsPrime() && L != number)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsSpecialPrime(this int number)
        {
            return IsSpecialPrime((ulong)number);
        }

        #region IsPrime
        public static bool IsPrime(this ulong number)
        {
            //assume 0 and 1 as primes
            if (number <= 1)
                return true;
            if (number == 2)
                return true;
            if (number % 2 == 0)
                return false;

            var boundary = (uint)System.Math.Floor(System.Math.Sqrt(number));

            for (uint i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        public static bool IsPrime(this uint number)
        {
            return IsPrime(Convert.ToUInt64(number));
        }

        public static bool IsPrime(this ushort number)
        {
            return IsPrime(Convert.ToUInt64(number));
        }

        public static bool IsPrime(this int number)
        {
            return IsPrime(Convert.ToUInt64(number));
        }
        #endregion

        public static int GetLastDigit(this ulong n)
        {
            return Convert.ToInt32(n % 10);
        }

        public static bool AnyDividers(this int number, params int[] arr)
        {
            //if (number % 2 == 0) return true;
            var boundary = (int)System.Math.Sqrt(number);
            foreach (var e in arr)
            {
                if (number % e == 0)
                {
                    return true;
                }

                if (e > boundary)
                    break;
            }

            return false;
        }

        public static bool AnyDividers(this int number, IEnumerable<int> arr)
        {
            //if (number % 2 == 0) return true;
            var boundary = (int)System.Math.Sqrt(number);
            foreach (var e in arr)
            {
                if (number % e == 0)
                {
                    return true;
                }

                if (e > boundary)
                    break;
            }

            return false;
        }

        public static bool IsPrimeTriplet(this ulong n1, ulong n2, ulong n3)
        {
            return n1 + 2 == n2 && n1 + 6 == n3;
        }

        public static bool IsPrimeTriplet(this int n1, int n2, int n3)
        {
            return n1 + 2 == n2 && n1 + 6 == n3;
        }

        public static int[] GetMultipliers(this ulong number)
        {
            //assume 0 and 1 as primes
            if (number <= 3)
                return new int[] { (int)number };

            var boundary = (int)System.Math.Floor(System.Math.Sqrt(number)) + 1;
            var primes = Primes.GeneratePrimeNumbers(2, boundary);
            var list = new List<int>();
            var n = number;
            foreach (var prime in primes)
            {
                var p = (ulong)prime;
                start:
                if (n % p == 0)
                {
                    n /= p;
                    list.Add(prime);
                    goto start;
                }
            }

            if (n > 1)
            {
                list.Add((int)n);
            }

            return list.ToArray();
        }

        public static byte[] ToBytesAsShort(this int value)
        {
            return BitConverter.GetBytes((short)value);
        }

        public static byte[] ToBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

    }
}