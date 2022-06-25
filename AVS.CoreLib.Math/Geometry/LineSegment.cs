using System;

namespace AVS.CoreLib.Math.Geometry
{
    public readonly struct LineSegment
    {
        public Point A { get; }
        public Point B { get; }

        public double Length => A.GetDistanceTo(B);

        public LineSegment(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public LineSegment(int x1, int y1, int x2, int y2)
        {
            A = new Point(x1, y1);
            B = new Point(x2, y2);
        }

        public override string ToString()
        {
            var l = Length;
            ulong ul = 0;
            if (l < ulong.MaxValue)
            {
                ul = Convert.ToUInt64(l);
            }
            return $"AB {A}-{B} (L={l}; L`={ul})";
        }
    }
}