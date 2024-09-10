using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Extensions;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs
{
    public readonly struct PrimeInfo
    {
        public PrimeInfo(int n)
        {
            N = n;
        }

        public int N { get; }
        public int Prime => N.GetNearestPrime();
        public int LastDigit => Prime % 10;
        public int Index => N.GetNearestPrimeIndex();
        public int Rest => N - Prime;

        public override string ToString()
        {
            return $"{N} => {Prime} + {Rest}; i={Index}; d={LastDigit}";
        }
    }
}