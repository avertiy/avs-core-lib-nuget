using System;
using System.Linq;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public class OddView
    {
        public ulong N0 { get; }
        public int N => Muls.Last();
        public int D { get; }
        public OddType Type { get; }

        public int[] Muls { get; }

        public int Pow2 { get; }

        public int L => Muls.Length;

        public bool IsOdd => N0 % 2 == 1;
        public bool IsPrime => N.IsPrime();

        public OddView(ulong n)
        {
            N0 = n;
            Muls = n.GetMultipliers();
            if (N > 2)
            {
                D = N % 10;
            }
            else
            {
                D = 1;
            }

            Type = GetOddType(D, IsPrime);
        }

        public override string ToString()
        {
            return $"{N0} => {Muls.AsArrayString("x")}; Type: {Type};";
        }

        private OddType GetOddType(int d, bool isPrime)
        {
            if (isPrime)
            {
                switch (d)
                {
                    case 1:
                        return OddType.Prime1;
                    case 3:
                    case 9:
                        return OddType.Prime39;
                    case 5:
                    case 7:
                        return OddType.Prime57;
                }
            }

            return d switch
            {
                1 => OddType.D1,
                3 => OddType.D3,
                5 => OddType.D5,
                7 => OddType.D7,
                9 => OddType.D9,
                _ => throw new ArgumentException($"{nameof(d)} must be 1 3 5 7 or 9")
            };
        }
    }

    public enum OddType
    {
        D1 = 0,
        D3 = 1,
        D5 = 2,
        D7 = 3,
        D9 = 4,
        Prime1 = 5,
        Prime39 = 6,
        Prime57 = 7,
    }


}