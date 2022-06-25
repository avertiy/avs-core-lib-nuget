using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public static class Primes
    {
        public static List<int> GeneratePrimeNumbers(int n)
        {
            var r = from i in Enumerable.Range(2, n - 1).AsParallel()
                    where Enumerable.Range(1, (int)System.Math.Sqrt(i)).All(j => j == 1 || i % j != 0)
                    select (int)i;
            return r.OrderBy(x => x).ToList();
        }

        public static List<uint> GenerateUInt32Primes(int n)
        {
            var r = from i in Enumerable.Range(2, n - 1).AsParallel()
                    where Enumerable.Range(1, (int)System.Math.Sqrt(i)).All(j => j == 1 || i % j != 0)
                    select (uint)i;
            return r.OrderBy(x => x).ToList();
        }

        public static List<int> GeneratePrimeNumbers(int @from, int to)
        {
            var r = Enumerable.Range(@from, to - @from)
                .AsParallel()
                .Where(i => Enumerable.Range(1, (int)System.Math.Sqrt(i)).All(j => j == 1 || i % j != 0));
            return r.OrderBy(x => x).ToList();
        }
    }
}