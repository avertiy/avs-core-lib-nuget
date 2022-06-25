using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Triplets
{
    public static class PrimeTripletsExtensions
    {
        public static int GetTripletMemberIndex(this Prime prime)
        {
            var n = prime.N;
            var next = prime.Next;
            if ((n + 2) == next.N || (n + 4) == next.N)
            {
                var next2 = next.Next;
                if ((n + 6) == next2.N)
                {
                    return 0;
                }

                var prev = prime.Prev;
                if ((prev.N + 2 == n || prev.N + 4 == n) && prev.N + 6 == next.N)
                {
                    return 1;
                }
            }
            else
            {
                var prev = prime.Prev;
                if (prev.N + 2 == n || prev.N + 4 == n)
                {
                    var prev2 = prev.Prev;
                    if ((prev2.N + 2 == prev.N || prev2.N + 4 == prev.N) && prev2.N + 6 == n)
                    {
                        return 2;
                    }
                }
            }

            return -1;
        }

        public static bool TryGetTriplet(this Prime prime, out Triplet triplet)
        {
            var n = prime.N;
            var next = prime.Next;
            if ((n + 2) == next.N || (n + 4) == next.N)
            {
                var next2 = next.Next;
                if ((n + 6) == next2.N)
                {
                    triplet = Triplet.Create(prime, next, next2);
                    return true;
                }

                var prev = prime.Prev;
                if ((prev.N + 2 == n || prev.N + 4 == n) && prev.N + 6 == next.N)
                {
                    triplet = Triplet.Create(prev, prime, next);
                    return true;
                }
            }
            else
            {
                var prev = prime.Prev;
                if (prev.N + 2 == n || prev.N + 4 == n)
                {
                    var prev2 = prev.Prev;
                    if ((prev2.N + 2 == prev.N || prev2.N + 4 == prev.N) && prev2.N + 6 == n)
                    {
                        triplet = Triplet.Create(prev, prev2, prime);
                        return true;
                    }
                }
            }

            triplet = null;
            return false;
        }

        public static Triplet GetPrevTriplet(this Prime prime)
        {
            var n = prime.N;
            if (prime.TryGetTriplet(out var triplet))
            {
                n = triplet.P1.N;
            }

            while (n > 3)
            {
                var prev = (Prime)n.GetNearestPrime();
                if (prev.TryGetTriplet(out triplet))
                {
                    return triplet;
                }

                n = prev.N;
            }

            return null;
        }

        public static Triplet GetNextTriplet(this Prime prime)
        {
            var n = prime.N;
            if (prime.TryGetTriplet(out var triplet))
            {
                n = triplet.P3.N;
            }

            while (n > 3)
            {
                var next = (Prime)n.GetNextPrime();
                if (next.TryGetTriplet(out triplet))
                {
                    return triplet;
                }

                n = next.N;
            }

            return null;
        }
    }
}