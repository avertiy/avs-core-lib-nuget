using System.Numerics;

namespace AVS.CoreLib.Math.MathUtils.Factorials
{
    public readonly struct Factorial
    {
        public int N { get; }
        public BigInteger Value { get; }

        public Factorial(int n, BigInteger value)
        {
            N = n;
            Value = value;
        }

        public static Factorial CalcBase(ulong n)
        {
            if (n == 0)
            {
                return new Factorial(0, 0);
            }

            var bi = new BigInteger(1);
            int i = 0;
            while (bi <= n)
            {
                i++;
                bi = bi * i;
            }

            bi = bi / i--;
            return new Factorial(i, bi);
        }

        public static Factorial Calc(int n)
        {
            if (n == 0)
            {
                return new Factorial(0, 0);
            }

            var bi = new BigInteger(1);
            int i = n;
            while (i > 1)
            {
                bi = bi * i;
                i--;
            }

            return new Factorial(n, bi);
        }

        public override string ToString()
        {
            return N.ToString();
        }
    }
}