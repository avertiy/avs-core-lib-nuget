using System;
using System.Linq;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions
{
    public static class NextPrimeExtensions
    {
        public static ulong GetNextPrime(this ulong number)
        {
            while (true)
            {
                bool isPrime = true;
                //increment the number by 1 each time
                number = number + 1;

                if (number % 2 == 0)
                    continue;

                ulong squaredNumber = (ulong)System.Math.Sqrt(number);
                //start at 2 and increment by 1 until it gets to the squared number
                for (ulong i = 3; i <= squaredNumber; i += 2)
                {
                    //how do I check all i's?
                    if (number % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                    return number;
            }
        }

        public static int GetNextPrime(this int number)
        {
            return Convert.ToInt32(GetNextPrime((ulong)number));
        }

        public static uint GetNextPrime(this uint number)
        {
            return Convert.ToUInt32(GetNextPrime((ulong)number));
        }

        public static ulong GetNextPrime(this ulong number, params int[] lastDigits)
        {
            while (true)
            {
                bool isPrime = true;
                //increment the number by 1 each time
                number = number + 1;
                if (number % 2 == 0)
                    continue;

                var d = (int)number % 10;
                if (lastDigits.All(x => x != d))
                    continue;

                var squaredNumber = (ulong)System.Math.Sqrt(number);

                //start at 2 and increment by 1 until it gets to the squared number
                for (ulong i = 3; i <= squaredNumber; i += 2)
                {
                    //how do I check all i's?
                    if (number % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                    return number;
            }

        }
    }
}