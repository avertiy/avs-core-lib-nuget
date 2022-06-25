namespace AVS.CoreLib.Math.Geometry
{
    public readonly struct Point
    {
        public int X { get; }
        public int Y { get; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"[{X};{Y}]";
        }


    }

    public static class PointExtensions
    {
        public static double GetDistanceTo(this Point A, Point B)
        {
            var a = ((double)B.X - A.X);
            var b = ((double)B.Y - A.Y);
            return System.Math.Sqrt(a * a + b * b);
        }

        public static bool IsShrinkable(this Point p)
        {
            return p.X <= short.MaxValue && p.Y <= short.MaxValue;
        }
    }
}