using System;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.MathUtils.Factorials
{
    /// <summary>
    /// number factorization M=N!+R, where R=N1!+R1, where R1=N2!+R2 etc. 
    /// </summary>
    public class Factorization
    {
        public int N { get; private set; }

        public int K { get; private set; } = 1;

        public ulong CalcRest(ulong n)
        {
            if (K > 0)
            {
                return n - N.ComputeFactorialUL() * (ulong)K;
            }

            return N.ComputeFactorialUL() - n;
        }

        public ulong Value => N.ComputeFactorialUL();

        public Factorization Rest { get; private set; }

        public override string ToString()
        {
            var kStr = K == 1 ? "" : K + "*";
            return Rest == null ? $"{kStr}{N}!" : $"{kStr}{N}! + {Rest}";
        }

        public static Factorization From(ulong n)
        {
            var f1 = Factorial.CalcBase(n);
            var fact1 = (f1.N).ComputeFactorialUL();
            int N = f1.N;
            int K = Convert.ToInt32(n / fact1);

            var fac = new Factorization
            {
                K = K,
                N = N,
            };

            var rest = fac.CalcRest(n);
            if (rest > 0)
            {
                fac.Rest = From(rest);
            }

            return fac;
        }
    }
}