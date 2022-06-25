using System;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.Sqrt
{
    public readonly struct Sqrt
    {
        public readonly Fraction N;

        public Sqrt(Fraction n)
        {
            N = n.Reduce();
        }

        public bool IsInt => System.Math.Sqrt(N.Value).IsIntegerValue();
        public bool IsIntFraction => System.Math.Sqrt(N.Numerator).IsIntegerValue() && System.Math.Sqrt(N.Denominator).IsIntegerValue();

        public double Value => System.Math.Sqrt(N.Value);

        public Fraction? Fraction => N.TrySqrt();

        public bool IsComplex => N < 0;

        public override string ToString()
        {
            var expr = SqrtExpression.Create(N);
            return expr.ToString();
        }

        public Fraction Pow()
        {
            return N;
        }

        public static Sqrt operator *(Sqrt a, Sqrt b)
            => new Sqrt(a.N * b.N);

        public static Sqrt operator /(Sqrt a, Sqrt b)
        {
            if (b.N == 0)
            {
                throw new DivideByZeroException();
            }

            return new Sqrt(a.N / b.N);
        }

        public static double operator +(Sqrt a, Sqrt b) => a.Value + b.Value;
        public static double operator -(Sqrt a, Sqrt b) => a.Value - b.Value;

        public static explicit operator double(Sqrt sqrt)
        { return sqrt.Value; }

        public static bool operator <(Sqrt a, Sqrt b) => a.N < b.N;

        public static bool operator >(Sqrt a, Sqrt b) => a.N > b.N;

        public static bool operator <=(Sqrt a, Sqrt b) => a.N <= b.N;
        public static bool operator >=(Sqrt a, Sqrt b) => a.N >= b.N;

        public static bool operator ==(Sqrt a, Sqrt b) => a.N == b.N;

        public static bool operator !=(Sqrt a, Sqrt b) => !(a == b);
    }
}