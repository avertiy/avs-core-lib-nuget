namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions
{
    public static class HopsExtensions
    {
        public static int GetHops(this int n, int lastDigit)
        {
            var d = lastDigit;
            var prime = n.GetNextPrime();
            var i = 1;
            while (prime % 10 != d && prime < 65521)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            if (prime % 10 == d)
                return i;
            return -1;
        }

        public static int GetHops(this int n, int digit1, int digit2)
        {
            var hops = GetHops(n, digit1);
            var prime = n.GetNextPrime();
            var i = 1;
            while (i < hops)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            prime = prime.GetNextPrime();
            i++;
            while (prime % 10 != digit2 && prime < 65521)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            if (prime % 10 == digit2)
                return i;
            return -1;
        }

        public static int GetHops(this int n, int digit1, int digit2, int digit3)
        {
            var hops = GetHops(n, digit1, digit2);
            var prime = n.JumpToNextPrime(hops + 1);
            var i = hops + 1;
            while (prime % 10 != digit3 && prime < 65521)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            if (prime % 10 == digit2)
                return i;
            return -1;
        }

        public static int GetHops4(this int n, int digit)
        {
            var hops = GetHops(n, digit, digit, digit);
            var prime = n.JumpToNextPrime(hops + 1);
            var i = hops + 1;
            while (prime % 10 != digit && prime < 65521)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            if (prime % 10 == digit)
                return i;
            return -1;
        }

        public static int GetHops5(this int n, int digit)
        {
            var hops = GetHops4(n, digit);
            var prime = n.JumpToNextPrime(hops + 1);
            var i = hops + 1;
            while (prime % 10 != digit && prime < 65521)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            if (prime % 10 == digit)
                return i;
            return -1;
        }

        public static int JumpToNextPrime(this int n, int hops)
        {
            var prime = n.GetNextPrime();
            var i = 1;
            while (i < hops)
            {
                prime = prime.GetNextPrime();
                i++;
            }

            return prime;
        }
    }
}