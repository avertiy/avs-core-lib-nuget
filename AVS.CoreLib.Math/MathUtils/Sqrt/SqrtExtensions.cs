using System;

namespace AVS.CoreLib.Math.MathUtils.Sqrt
{
    public static class SqrtExtensions
    {
        public static ulong Sqrt(this ulong n)
        {
            if (n <= 1)
                return n;

            var sqrtN = System.Math.Sqrt(n);
            return Convert.ToUInt64(sqrtN);
        }

        public static SqrtResult Sqrt(this ulong n, int rootExp)
        {
            if (n <= 1)
                return 1;

            var value = System.Math.Pow(n, 1.00 / rootExp);
            var intPart = Convert.ToUInt64(System.Math.Floor(value));
            var rest = n - Convert.ToUInt64(System.Math.Pow(intPart, rootExp));
            var res = new SqrtResult(intPart, rest);
            return res;
        }
    }
}