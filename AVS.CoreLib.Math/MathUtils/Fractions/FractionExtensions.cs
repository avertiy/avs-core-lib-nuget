using System;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.MathUtils.Fractions
{
    public static class FractionExtensions
    {
        public static Fraction Abs(this Fraction f)
        {
            return f.Value >= 0 ? f : (f * -1).Reduce();
        }

        /// <summary>
        /// The function returns the inverse of a Fraction object
        /// </summary>
        public static Fraction Inverse(this Fraction fraction)
        {
            if (fraction.Numerator == 0)
                throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
            return new Fraction(fraction.Denominator, fraction.Numerator);
        }

        public static Fraction Reduce(this Fraction fraction)
        {
            try
            {
                if (fraction.Numerator == 0)
                {
                    return new Fraction(0);
                }

                if (fraction.Numerator == fraction.Denominator)
                {
                    return new Fraction(1, 1UL, fraction.Sign);
                }

                if (fraction.Denominator == 1L || fraction.Numerator % fraction.Denominator == 0)
                {
                    return new Fraction(fraction.Numerator / fraction.Denominator, 1UL, fraction.Sign);
                }

                var n = fraction.Numerator;
                var d = fraction.Denominator;

                var iGCD = GCD(n, d);
                n /= iGCD;
                d /= iGCD;

                //if (d < 0)   // if sign in denominator
                //{
                //    //pass sign to numerator
                //    n *= -1;
                //    d *= -1;
                //}

                return new Fraction(n, d, fraction.Sign);
            }
            catch (Exception exp)
            {
                throw new FractionException("Cannot reduce Fraction: " + exp.Message);
            }

        }

        /// <summary>
        /// The function returns GCD of two numbers (used for reducing a Fraction)
        /// </summary>
        private static ulong GCD(ulong iNo1, ulong iNo2)
        {
            //// take absolute values
            //if (iNo1 < 0) iNo1 = -iNo1;
            //if (iNo2 < 0) iNo2 = -iNo2;

            do
            {
                if (iNo1 < iNo2)
                {
                    ulong tmp = iNo1;  // swap the two operands
                    iNo1 = iNo2;
                    iNo2 = tmp;
                }
                iNo1 = iNo1 % iNo2;
            } while (iNo1 != 0);
            return iNo2;
        }

        public static Fraction Pow(this Fraction fraction, Fraction pow, bool @checked = true)
        {
            if (pow == 0 || fraction == 1)
                return 1;
            if (pow == 1)
                return fraction;
            if (!pow.IsFractional || @checked == false)
            {
                var n = fraction.Numerator;
                var d = fraction.Denominator;
                var powN = System.Math.Pow(n, pow.Value);
                n = Convert.ToUInt64(powN);
                var powD = System.Math.Pow(d, pow.Value);
                d = Convert.ToUInt64(powD);
                var sign = (int)System.Math.Pow(fraction.Sign, pow.Value);
                return new Fraction(n, d, sign).Reduce();
            }
            else
            {
                throw new InvalidOperationException("Operation result might lead to loss [real number will be round to integer number]");
            }
        }

        public static Fraction Pow2(this Fraction fraction, Fraction pow)
        {
            if (pow == 0 || fraction == 1)
                return 1;
            if (pow == 1)
                return fraction;
            if (!pow.IsFractional)
            {
                var n = fraction.Numerator;
                var d = fraction.Denominator;
                var powN = System.Math.Pow(n, pow.Value);
                var powD = System.Math.Pow(d, pow.Value);

                if (!powN.IsIntegerValue() || !powD.IsIntegerValue())
                {
                    throw new InvalidOperationException("Operation result might lead to loss [real number will be round to integer number]");
                }

                n = Convert.ToUInt64(powN);
                d = Convert.ToUInt64(powD);
                return new Fraction(n, d).Reduce();
            }
            else
            {
                throw new InvalidOperationException("Operation result might lead to loss [real number will be round to integer number]");
            }
        }

        public static Sqrt.Sqrt Sqrt(this Fraction fraction)
        {
            return new Sqrt.Sqrt(fraction);
        }

        public static Fraction? TrySqrt(this Fraction fraction)
        {
            if (fraction.IsZero || fraction.IsOne)
                return fraction.Reduce();
            if (fraction.Sign < 0)
                return null;
            var sqrtN = System.Math.Sqrt(fraction.Numerator);
            if (!sqrtN.IsIntegerValue())
                return null;
            var sqrtD = System.Math.Sqrt(fraction.Denominator);
            return sqrtD.IsIntegerValue() ?
                new Fraction(Convert.ToUInt64(sqrtN), Convert.ToUInt64(sqrtD)).Reduce() :
                (Fraction?)null;
        }
    }
}