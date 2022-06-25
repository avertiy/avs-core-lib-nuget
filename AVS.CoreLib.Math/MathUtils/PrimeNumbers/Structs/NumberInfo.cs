using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.Factorials;
using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs
{
    public readonly struct NumberInfo
    {
        public NumberInfo(ulong n)
        {
            N = n;
        }

        public ulong N { get; }

        public bool IsPrime => N.IsPrime();
        public ulong[] Multipliers => N.SplitOnMultipliers();
        public Sqrt.Sqrt Sqrt => new Sqrt.Sqrt(new Fraction(N));
        public Factorization Factorization => Factorization.From(N);
    }
}