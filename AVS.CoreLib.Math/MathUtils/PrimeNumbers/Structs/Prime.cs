using System;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Triplets;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs
{
    public readonly struct Prime
    {
        private Prime(ulong prime, string path = null)
        {
            N = prime;
            PathStr = path;
        }

        public ulong N { get; }
        public string PathStr { get; }
        public int LastDigit => Convert.ToByte(N % 10);
        public int SumOfDigits => N.SumOfDigits();
        //public int Index => N < 65536 ? SmallPrimes.Set65536.IndexOf(Convert.ToInt32(N)) : -1;

        public Prime Next => this.GetNextPrime();

        public Prime Prev => this.GetPrevPrime();

        //public BitArray Path => this.GetPath();

        public bool IsSpecial => N.IsSpecialPrime();

        //public Triplet Triplet => this.TryGetTriplet(out var triplet) ? triplet : null;

        public bool IsTripletMember => this.GetTripletMemberIndex() >= 0;

        //public Triplet PrevTriplet => this.GetPrevTriplet();
        //public Triplet NextTriplet => this.GetNextTriplet();

        public static implicit operator ulong(Prime p) => p.N;
        public static implicit operator uint(Prime p) => (uint)p.N;
        public static implicit operator int(Prime p) => Convert.ToInt32(p.N);
        public static implicit operator Prime(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException($"n {n} must be positive number");
            }

            if (!n.IsPrime())
            {
                throw new ArgumentException($"{n} is not prime!");
            }

            return new Prime((ulong)n);
        }
        public static implicit operator Prime(ulong n)
        {
            if (!n.IsPrime())
            {
                throw new ArgumentException($"{n} is not prime!");
            }

            return new Prime(n);
        }
        public static implicit operator Prime(uint n)
        {
            if (!n.IsPrime())
            {
                throw new ArgumentException($"{n} is not prime!");
            }

            return new Prime(n);
        }
        public static implicit operator Prime(ushort n)
        {
            if (!n.IsPrime())
            {
                throw new ArgumentException($"{n} is not prime!");
            }

            return new Prime(n);
        }

        public static Prime FromNumber(ulong number)
        {
            var prime = number.GetNearestPrime();
            return new Prime(prime);
        }

        public static Prime FromPrimeNumber(ulong primeNumber)
        {
            var path = primeNumber.GetPath();
            return new Prime(primeNumber, string.Join("", path));
        }

        public override string ToString()
        {
            return N.ToString();
        }
    }
}