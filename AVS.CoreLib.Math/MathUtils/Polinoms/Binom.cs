using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.MathUtils.Polinoms
{
    public class Binom
    {
        public Fraction A { get; set; }
        public Fraction B { get; set; }
        public Fraction C { get; set; }

        public Fraction GetDiscriminant() => B * B - 4 * A * C;

        public Fraction[] GetRoots()
        {
            var d = GetDiscriminant();
            if (d < 0)
                return new Fraction[] { };

            if (d == 0)
            {
                var x = -B / (2 * A);
                return new[] { x };
            }

            var sqrtFromD = System.Math.Sqrt(d.Value);
            var f = Fraction.ToFraction(sqrtFromD);
            var x1 = (-B + f) / (2 * A);
            var x2 = (-B - f) / (2 * A);

            return new[] { x1, x2 };
        }

        public override string ToString()
        {
            return $"{A}x^2 + {B}x + {C} = 0".Replace("+ -", "- ");
        }
    }
}