using System;
using System.Text.RegularExpressions;
using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.Polinoms
{
    /// <summary>
    /// fraction*x^n
    /// </summary>
    public class PolynomMember
    {
        private const string MEMBER_REGEX = "(?<sign>-)?\\s?(?<a>((\\d/?)+)|([a-wyz]))?\\*?(?<x>[x])?((\\^(?<n>(\\d/?)+))?)";
        public bool IsDefined => A.HasValue;

        public string Character { get; set; }

        public Fraction N { get; set; }

        public Fraction? A { get; set; }
        public bool IsZero => A.HasValue && A.Value.IsZero;

        public PolynomMember(Fraction n, string character)
        {
            N = n;
            Character = character;
        }

        public PolynomMember(Fraction a)
        {
            A = a;
            N = 1L;
        }

        public PolynomMember(Fraction a, Fraction n)
        {
            A = a;
            N = n;
        }

        public override string ToString()
        {
            if (A.HasValue)
            {
                if (A == 0)
                {
                    return "0";
                }

                if (A.Value.Reduce() == 1)
                {
                    return N == 0 ? "1" : N == 1 ? "x" : $"x^{N}";
                }

                return N == 0 ? A.ToString() : N == 1 ? $"{A}x" : $"{A}x^{N}";

            }

            return N == 0 ? Character : N == 1 ? $"{Character}x" : $"{Character}x^{N}";
        }

        public static bool TryParse(string str, out PolynomMember member)
        {
            member = null;
            var re = new Regex(MEMBER_REGEX);
            var match = re.Match(str);
            if (match.Success)
            {
                Fraction n, a;
                if (match.Groups["a"].Success)
                {
                    if (!Fraction.TryParse(match.Groups["a"].Value, out a))
                    {
                        return false;
                    }
                }
                else if (match.Groups["x"].Success)
                {
                    a = 1;
                }
                else
                {
                    return false;
                }

                if (match.Groups["x"].Success)
                {
                    if (match.Groups["n"].Success)
                    {
                        if (!Fraction.TryParse(match.Groups["n"].Value, out n))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        n = 1;
                    }
                }
                else
                {
                    n = 0;
                }

                var sign = 1;
                if (match.Groups["sign"].Success)
                {
                    sign = -1;
                }

                member = new PolynomMember(sign * a, n);
                return true;
            }

            return false;
        }

        public static implicit operator string(PolynomMember member)
        { return member.ToString(); }

        public Fraction Calc(Fraction x)
        {
            if (!A.HasValue)
                throw new InvalidOperationException("A is not defined");
            if (N.IsZero)
                return A.Value;

            var res = A.Value * x.Pow(N);
            return res;
        }
    }
}