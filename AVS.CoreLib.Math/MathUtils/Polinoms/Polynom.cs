using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.Polinoms
{
    public sealed class Polynom
    {
        private const string POLYNOM_REGEX = "((?<member>(((-)\\s)?[\\dx^\\*/a-z])+)[\\s+]*)+=\\s?(?<r>([-/\\d])+)";

        public Polynom(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException($"{nameof(n)} must be >= 0");
            Members = new PolynomMember[n];
            Roots = new Fraction?[n - 1];
        }

        public Polynom(PolynomMember[] terms)
        {
            Members = terms;
            var n = terms.Where(x => x.N > 0).Select(x => x.N).Distinct().Count();
            Roots = new Fraction?[n];
        }

        public Polynom(Fraction[] coefficients)
        {
            if (coefficients.Length == 0)
                throw new ArgumentOutOfRangeException($"{nameof(coefficients)} must be not empty array");
            Members = new PolynomMember[coefficients.Length];
            Roots = new Fraction?[coefficients.Length - 1];
            var n = coefficients.Length;
            for (var i = 0; i < coefficients.Length; i++)
            {
                Members[i] = new PolynomMember(coefficients[i], n - i);
            }
        }

        public PolynomMember this[int index]
        {
            get => Members[index];
            set => Members[index] = value;
        }

        public PolynomMember[] Members { get; }

        /// <summary>
        /// polynomial degree
        /// </summary>
        public Fraction N => Members.Max(x => x.N);

        /// <summary>
        /// the last coefficient i.e. k*x^0
        /// </summary>
        public PolynomMember K
        {
            get => Members.Last();
            set => Members[^1] = value;
        }

        public Fraction R { get; set; } = 0;

        public Fraction?[] Roots { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Members.Length; i++)
            {
                if (Members[i].IsZero)
                {
                    continue;
                }
                sb.Append(Members[i]);
                sb.Append(" + ");
            }
            sb.Length -= 3;
            sb.Replace("+ -", "- ");
            sb.Append($" = {R}");
            return sb.ToString();
        }

        public Polynom Reduce()
        {
            if (!Members[0].IsDefined)
                return this;

            var a = Members[0].A;
            foreach (var member in Members)
            {
                member.A = (member.A / a)?.Reduce();
            }

            return this;
        }

        public bool IsOptimized => Members.Any() && Members[0].A == 1 &&
                                   Members.Select(x => x.N).Distinct().Count() == Members.Length;

        public bool IsDefined => Members.All(x => x.IsDefined);

        public static bool TryParse(string equation, out Polynom polynom)
        {
            polynom = null;
            var re = new Regex(POLYNOM_REGEX);
            var match = re.Match(equation);
            if (!match.Success)
            {
                return false;
            }

            var rightPart = match.Groups["r"].Captures.First(x => !string.IsNullOrEmpty(x.Value)).Value;
            PolynomMember rMember = null;
            if (rightPart != "0")
            {
                if (!PolynomMember.TryParse(rightPart, out rMember) || !rMember.IsDefined)
                    return false;
                rMember.A *= -1;
            }

            if (!PolynomMember.TryParse(rightPart, out PolynomMember k) || !k.IsDefined || k.N != 0)
            {
                return false;
            }

            var members = match.Groups["member"].Captures.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => x.Value).ToList();

            var n = members.Count + (rMember == null ? 0 : 1);
            var p = new Polynom(n);
            for (var i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (PolynomMember.TryParse(member, out PolynomMember m))
                {
                    p[i] = m;
                }
                else
                {
                    return false;
                }
            }

            if (rMember != null)
            {
                p[members.Count] = rMember;
            }

            p.R = 0;
            polynom = p;
            return true;
        }

        public static implicit operator string(Polynom expr)
        { return expr.ToString(); }
    }

    public static class PolynomExtensions
    {
        public static Polynom Optimize(this Polynom polynom)
        {
            return polynom.IsOptimized ? polynom : polynom.CombineLikeTerms().Reduce();
        }

        public static Polynom CombineLikeTerms(this Polynom polynom)
        {
            var members = new List<PolynomMember>();
            for (var i = 0; i < polynom.Members.Length; i++)
            {
                if (members.Any(x => x.N == polynom.Members[i].N))
                    continue;

                var arr = polynom.Members.Skip(i).Where(x => x.N == polynom.Members[i].N).ToArray();
                if (arr.Length == 1)
                {
                    members.Add(arr[0]);
                    continue;
                }

                var m = arr[0];

                for (var k = 1; k < arr.Length; k++)
                {
                    if (m.IsDefined && arr[k].IsDefined)
                    {
                        m.A += arr[k].A;
                    }
                }

                members.Add(m);
            }

            var p = new Polynom(members.OrderByDescending(x => x.N).ToArray());
            return p;
        }

        public static string ToString2(this Polynom polynom)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < polynom.Members.Length - 1; i++)
            {
                if (polynom.Members[i].IsZero)
                {
                    continue;
                }
                sb.Append(polynom.Members[i]);
                sb.Append(" + ");
            }
            sb.Length -= 3;
            sb.Replace("+ -", "- ");
            var r = polynom.K.A * -1;
            sb.Append($" = {r}");
            return sb.ToString();
        }

        public static bool TestRoot(this Polynom polynom, Fraction x)
        {
            Fraction n = 0;
            foreach (var member in polynom.Members)
            {
                var m = member.Calc(x);
                n += m;
            }

            var res = n == polynom.R;
            return res;
        }

        public static Binom ToBinom(this Polynom polynom)
        {
            var p = polynom.Optimize();
            if (p.N != 2 || !p.IsDefined)
                throw new InvalidCastException($"Polynom [{polynom}] can't be converted to Binom");

            var x2 = p.Members.FirstOrDefault(x => x.N == 2);
            var x1 = p.Members.FirstOrDefault(x => x.N == 1);
            var x0 = p.Members.FirstOrDefault(x => x.N == 0);

            var binom = new Binom()
            {
                A = x2 == null ? 0 : x2.A ?? 0,
                B = x1 == null ? 0 : x1.A ?? 0,
                C = x0 == null ? 0 : x0.A ?? 0,
            };
            return binom;
        }

        public static Fraction[] FindRoots(this Polynom polynom, int from = -255, int to = 255, int maxFraction = 255)
        {
            var p = polynom.Optimize();
            if (!p.IsDefined)
                throw new InvalidOperationException($"Polynom must be defined [all members must have defined coefficient A]");

            var n = polynom.N;
            if (n == 2)
            {
                var binom = polynom.ToBinom();
                return binom.GetRoots();
            }

            return p.TryRoots(-255, 255, 255);
        }

        private static Fraction[] TryRoots(this Polynom polynom, int from, int to, int maxFraction)
        {
            var roots = new List<Fraction>();
            var n = polynom.N.ToInt64();
            for (var x = from; x <= to; x++)
            {
                for (var d = 2; d <= maxFraction; d++)
                {
                    if (x % d == 0)
                        continue;
                    var f = (new Fraction(x, d)).Reduce();
                    if (roots.Contains(f))
                        continue;
                    if (polynom.TestRoot(f))
                    {
                        roots.Add(f);
                        if (roots.Count == n)
                            return roots.ToArray();
                    }
                }

                if (polynom.TestRoot(x))
                {
                    roots.Add(x);
                    if (roots.Count == n)
                        break;
                }

            }
            return roots.ToArray();
        }
    }
}