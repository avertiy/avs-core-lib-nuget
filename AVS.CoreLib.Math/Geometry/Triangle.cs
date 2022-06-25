using static System.Double;

namespace AVS.CoreLib.Math.Geometry
{
    public class Triangle
    {
        public Point A { get; }
        public Point B { get; }
        public Point C { get; }

        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
            AB = new Distance(A, B);
            AC = new Distance(A, C);
            BC = new Distance(B, C);
        }

        public int[] X => new int[] { A.X, B.X, C.X };
        public int[] Y => new int[] { A.Y, B.Y, C.Y };

        public Distance AB { get; }
        public Distance BC { get; }
        public Distance AC { get; }
        public double P => checked(AB + BC + AC);

        public double R => System.Math.Floor(this.GetCircleRadius());

        public bool IsRightAngled
        {
            get
            {
                var a = AB.Pow();
                var b = BC.Pow();
                var c = AC.Pow();
                return a + b - c == 0 || a + c - b == 0 ||
                       b + c - a == 0;
                //var a = (double)AB;
                //var b = (double)BC;
                //var c = (double)AC;
                //a *= a;
                //b *= b;
                //c *= c;
                //return Math.Abs(a + b - c) < MinValue || Math.Abs(a + c - b) < MinValue ||
                //       Math.Abs(b + c - a) < MinValue;
            }
        }
        /// <summary>
        /// ravnostoronniy
        /// </summary>
        public bool IsEquilateral => System.Math.Abs(AB - BC) < MinValue && System.Math.Abs(AC - AB) < MinValue;

        /// <summary>
        /// ravnobedrenniy
        /// </summary>
        public bool IsIsosceles => System.Math.Abs(AB - BC) < MinValue || System.Math.Abs(AB - AC) < MinValue;

        public override string ToString()
        {
            return $"△ABC {A};{B};{C} [AB:{AB:N};BC:{BC:N};AC:{AC:N}]";
        }
    }
}