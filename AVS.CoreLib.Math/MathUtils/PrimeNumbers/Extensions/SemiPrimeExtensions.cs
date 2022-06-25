namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions
{
    public static class SemiPrimeExtensions
    {
        public static ulong GetNextSemiPrime(this ulong number)
        {
            while (true)
            {
                bool isPrime = true;
                //increment the number by 1 each time
                number = number + 1;
                if (number % 2 == 0)
                {
                    continue;
                }

                var boundary = (ulong)System.Math.Floor(System.Math.Sqrt(number));
                boundary = boundary < 1000 ? boundary : 1000UL;

                //start at 2 and increment by 1 until it gets to the squared number
                for (ulong i = 3; i <= boundary; i += 2)
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

        public static ulong GetNextSemiPrime(this uint number)
        {
            return GetNextSemiPrime((ulong)number);
        }

        public static ulong GetNearestSemiPrime(this uint n)
        {
            return GetNearestSemiPrime((ulong)n);
        }

        public static ulong GetNearestSemiPrime(this ulong n)
        {
            //we assume 0, 1 as primes
            if (n <= 2)
                return n;
            // All prime numbers are odd
            if (n % 2 == 0)
                n--;

            ulong i, j;
            var boundary = (ulong)System.Math.Floor(System.Math.Sqrt(n));
            boundary = boundary < 1000 ? boundary : 1000UL;

            for (i = n; i >= 2; i -= 2)
            {
                if (i % 2 == 0)
                    continue;
                for (j = 3; j <= boundary; j += 2)
                {
                    if (i % j == 0)
                        break;
                }
                if (j > boundary)
                    return i;
            }

            // It will only be executed when n is 3
            return 2;

        }
    }
}