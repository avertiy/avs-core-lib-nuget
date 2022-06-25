using AVS.CoreLib.Math.MathUtils.Fractions;
using AVS.CoreLib.Math.MathUtils.Sqrt;

namespace AVS.CoreLib.Math.Geometry
{
    public readonly struct Distance
    {
        public Sqrt Length { get; }

        public Distance(Point a, Point b)
        {
            var x = (long)(b.X - a.X);
            var y = (long)(b.Y - a.Y);
            Length = new Sqrt(x * x + y * y);
        }

        public Fraction Pow()
        {
            return Length.Pow();
        }

        public static implicit operator double(Distance d)
        { return d.Length.Value; }

        public override string ToString()
        {
            return $"{Length}";
        }

        public static double operator +(Distance a, Distance b) => a.Length + b.Length;

        public static double operator -(Distance a, Distance b) => a.Length - b.Length;

        public static bool operator <(Distance a, Distance b) => a.Length < b.Length;
        public static bool operator <=(Distance a, Distance b) => a.Length <= b.Length;
        public static bool operator >(Distance a, Distance b) => a.Length > b.Length;
        public static bool operator >=(Distance a, Distance b) => a.Length >= b.Length;
        public static bool operator ==(Distance a, Distance b) => a.Length == b.Length;
        public static bool operator !=(Distance a, Distance b) => !(a == b);
    }
}