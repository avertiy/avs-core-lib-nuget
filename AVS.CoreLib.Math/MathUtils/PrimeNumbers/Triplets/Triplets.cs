using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Triplets
{
    public static class Triplets
    {
        public static List<Triplet> GetTriplets(this List<int> primes)
        {
            var triplets = new List<Triplet>();
            for (var i = 0; i < primes.Count - 2; i++)
            {
                if (primes[i] < 7)
                    continue;
                //var f2 = primes[i] + 4 == primes[i + 1] && primes[i] + 6 == primes[i + 2];
                var f1 = primes[i] + 2 == primes[i + 1] && primes[i] + 6 == primes[i + 2];
                //if (!f1 && !f2)
                if (!f1)
                    continue;

                var last = triplets.LastOrDefault();
                var triplet = Triplet.Create(primes[i], primes[i + 1], primes[i + 2]);
                triplet.Prev = last;
                triplets.Add(triplet);

                if (last != null)
                {
                    last.Next = triplet;
                }
            }

            return triplets;
        }

        private static List<Triplet> _set2b;

        public static List<Triplet> Set
        {
            get { return _set2b ??= SmallPrimes.Set65536.GetTriplets(); }
        }

        private static List<Triplet> _set3b;

        public static List<Triplet> Set3B
        {
            get { return _set3b ??= SmallPrimes.Set3B.GetTriplets(); }
        }

        private static List<Triplet> _set4b;

        public static List<Triplet> Set4B => _set4b ??= PrimesInitializer.InitPrimesAndTriplets(int.MaxValue);
    }

    public static class PrimesInitializer
    {
        public static List<Triplet> InitPrimesAndTriplets(uint max)
        {
            //PowerConsole.PrintHeader($"Initializing prime sets [N={max}]");
            int capacity = Convert.ToInt32(max / 10);
            var primes = Primes.GeneratePrimeNumbers(capacity * 5);
            var task1 = Task.Run(() => primes.GetTriplets());
            var task2 = Task.Run(() =>
            {
                var list = new List<int>(capacity);
                for (int i = primes.Last() + 2; i <= max; i += 2)
                {
                    if (i.AnyDividers(primes) || i.AnyDividers(list))
                    {
                        continue;
                    }

                    list.Add(i);
                }

                return list;
            });

            // PowerConsole.Print($"running task1 and task2...");

            var primes2 = task2.Result;

            // PowerConsole.Print($"task2 completed, running task3...");

            var task3 = Task.Run(() => primes2.GetTriplets());

            primes.AddRange(primes2);
            SmallPrimes.Set4B = primes;
            var triplets = task1.Result;
            //  PowerConsole.Print($"task1 completed");
            var triplets2 = task3.Result;
            //   PowerConsole.Print($"task2 completed");
            triplets.AddRange(triplets2);
            //     PowerConsole.Print($"Primes #{primes.Count}");
            //     PowerConsole.Print($"Triplets #{triplets.Count}");
            return triplets;
        }

    }
}