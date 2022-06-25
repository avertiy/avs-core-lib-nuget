using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public static class SmallPrimes
    {
        private static List<int> _first1000Primes;

        public static List<int> First1000
        {
            get { return _first1000Primes ??= Primes.GeneratePrimeNumbers(1000); }
        }


        private static List<int> _first6542Primes;

        public static List<int> Primes6542
        {
            get { return _first6542Primes ??= Primes.GeneratePrimeNumbers(Set65536.Count); }
        }


        private static List<int> _first65536Primes;

        public static List<int> Set65536
        {
            get { return _first65536Primes ??= Primes.GeneratePrimeNumbers(65536); }
        }


        private static List<int> _set3B;

        public static List<int> Set3B
        {
            get { return _set3B ??= Primes.GeneratePrimeNumbers(65536 * 256); }
        }

        public static List<int> Set4B { get; set; }

        public static int[] GetPrimesEndWith(string str)
        {
            return Set65536.Where(x => x.ToString().EndsWith(str)).ToArray();
        }

        public static int[] GetPrimesByLastDigit(char lastDigit)
        {
            return Set65536.Where(x => x.ToString().EndsWith(lastDigit)).ToArray();
        }

        public static List<int> GetPrimesByLastDigit(int lastDigit)
        {
            return Set65536.Where(x => x % 10 == lastDigit).ToList();
        }

        public static List<int> GetPrimesByIndexDigit(int indDigit, int primeLastDigit)
        {
            var list = new List<int>();
            for (var i = 0; i < Set65536.Count; i++)
            {
                if (i % 10 != indDigit)
                    continue;
                var p = Set65536[i];
                if (p % 10 == primeLastDigit)
                {
                    list.Add(p);
                }
            }

            return list;
        }

        public static int[] GetPrimes(int len, char lastDigit)
        {
            return Set65536
                .Where(x => x.ToString().Length == len && x.ToString()
                    .EndsWith(lastDigit)).ToArray();
        }

        public static List<int> GetPrimes(int lastDigit, int hops1, int hops2)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops(lastDigit) == hops1 && x.GetHops(lastDigit, lastDigit) == hops2).ToList();
        }

        public static int GetNearestPrimeIndex(this int n)
        {
            var prime = n.GetNearestPrime();
            var ind = Set65536.IndexOf(prime);
            return ind;
        }

        #region hops & jumps
        public static List<int> GetPrimesByHops1(int lastDigit, int hops1)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops(lastDigit) == hops1).ToList();
        }

        public static List<int> GetPrimesByHops2(int lastDigit, int hops2)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops(lastDigit, lastDigit) == hops2).ToList();
        }

        public static List<int> GetPrimesByHops3(int lastDigit, int hops3)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops(lastDigit, lastDigit, lastDigit) == hops3).ToList();
        }

        public static List<int> GetPrimesByHops4(int lastDigit, int hops)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops4(lastDigit) == hops).ToList();
        }

        public static List<int> GetPrimesByHops5(int lastDigit, int hops)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.GetHops5(lastDigit) == hops).ToList();
        }

        public static List<int> GetPrimesByJump(int lastDigit, int jump, int digit2)
        {
            return Set65536.Where(x => x % 10 == lastDigit && x.JumpToNextPrime(jump) % 10 == digit2).ToList();
        }
        #endregion


    }
}