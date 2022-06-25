using System;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.Sqrt
{
    public struct SqrtExpression
    {
        public bool IsComplex;
        public readonly Fraction? IntPart;
        public readonly Fraction? RootPart;

        public SqrtExpression(Fraction? intPart, Fraction? rootPart, bool isComplex = false)
        {
            IsComplex = isComplex;
            IntPart = intPart;
            RootPart = rootPart;
        }

        public override string ToString()
        {
            var intPartStr = IntPart.HasValue ? IntPart.Value.IsOne ? string.Empty : IntPart.ToString() : string.Empty;
            var rootPartStr = RootPart.HasValue ? $"√{RootPart}{(IsComplex ? "*√-1" : "")}" : string.Empty;
            if (!string.IsNullOrEmpty(intPartStr) && !string.IsNullOrEmpty(rootPartStr))
                return $"{intPartStr}*{rootPartStr}";
            return $"{intPartStr}{rootPartStr}";
        }

        public static SqrtExpression Create(Fraction n)
        {
            var complex = false;
            var abs = n.Reduce();
            if (n.Sign < 0)
            {
                complex = true;
            }

            var sqrt = abs.TrySqrt();
            if (sqrt != null)
            {
                return new SqrtExpression(sqrt, null, complex);
            }

            var sqrtD = System.Math.Sqrt(abs.Denominator);
            if (sqrtD.IsIntegerValue())
            {
                var d = Convert.ToUInt64(sqrtD);
                //√18 = √9 *√2 => 3/√2 
                if (abs.Numerator.TryPickPrimeNumberForIntSqrt(out uint prime))
                {
                    var num = abs.Numerator / prime;
                    var intPart = new Fraction(num.Sqrt(), d);
                    return new SqrtExpression(intPart.Reduce(), new Fraction(prime), complex);
                }

                return new SqrtExpression(new Fraction(1, d), new Fraction(abs.Numerator), complex);
            }

            var sqrtN = System.Math.Sqrt(abs.Numerator);
            if (sqrtN.IsIntegerValue())
            {
                var n1 = Convert.ToUInt64(sqrtN);
                //√9/2 = 3/√2 * √2/√2 => 3/2*√2 
                if (abs.Denominator.TryPickPrimeNumberForIntSqrt(out uint prime))
                {
                    var intPart = new Fraction(n1, abs.Denominator);
                    return new SqrtExpression(intPart.Reduce(), new Fraction(prime), complex);
                }

                return new SqrtExpression(new Fraction(n1, abs.Denominator), new Fraction(abs.Denominator), complex);
            }

            return new SqrtExpression(null, abs, complex);
            //var muls = abs.Numerator.SplitOnMultipliers();
            //foreach (var grouping in muls.GroupBy(x => x).Where(x=> x.Count() % 2 ==0))
            //{
            //    grouping.Key
            //}
        }

    }
}