using AVS.CoreLib.Math.MathUtils.Fractions;

namespace AVS.CoreLib.Math.Geometry
{
    /// <summary>
    /// y = kx+b
    /// </summary>
    public readonly struct LineEquation
    {
        public Fraction K { get; }
        public Fraction B { get; }

        public LineEquation(uint x1, uint y1, uint x2, uint y2)
        {
            //k=(y2-y1)/(x2-x1)
            //b=-(x1*y2-x2*y1)/(x2-x1)
            K = new Fraction(y2 - y1, x2 - x1).Reduce();
            B = new Fraction(-((long)x1 * y2 - (long)x2 * y1) / (x2 - x1)).Reduce();
        }


        public LineEquation(Point x1, Point x2)
        {
            //k=(y2-y1)/(x2-x1)
            //b=-(x1*y2-x2*y1)/(x2-x1)
            K = new Fraction(x2.Y - x1.Y, x2.X - x1.X).Reduce();
            B = new Fraction((long)x1.X * x2.Y - (long)x2.X * x1.Y, x2.X - x1.X).Reduce();
        }

        public Fraction CalcY(in uint x)
        {
            return (K * x + B).Reduce();
        }

        public override string ToString()
        {
            return $"{K.ToString("d")}x+{B.ToString("d")}";
        }
    }
}