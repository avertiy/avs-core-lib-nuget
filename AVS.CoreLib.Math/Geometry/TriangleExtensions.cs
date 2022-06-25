using System;
using System.Linq;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.Geometry
{
    public static class TriangleExtensions
    {
        public static bool IsSimilar(this Triangle t, Triangle triangle)
        {
            //1. ABC ~ ABC
            if (System.Math.Abs(t.AB / triangle.AB - t.BC / triangle.BC) < Double.MinValue &&
                System.Math.Abs(t.AC / triangle.AC - t.BC / triangle.BC) < Double.MinValue)
                return true;
            //2. ABC ~ BAC
            if (System.Math.Abs(t.AB / triangle.BC - t.AC / triangle.AB) < Double.MinValue &&
                System.Math.Abs(t.BC / triangle.AC - t.AB / triangle.BC) < Double.MinValue)
                return true;
            //3. ABC ~ CAB
            return System.Math.Abs(t.AB / triangle.AC - t.AC / triangle.BC) < Double.MinValue &&
                   System.Math.Abs(t.BC / triangle.AB - t.AB / triangle.AC) < Double.MinValue;
        }

        public static double GetSquare(this Triangle triangle)
        {
            checked
            {
                var a = triangle.AB;
                var b = triangle.BC;
                var c = triangle.AC;
                //var a = Convert.ToDecimal(triangle.AB);
                //var b = Convert.ToDecimal(triangle.BC);
                //var c = Convert.ToDecimal(triangle.AC);
                var p = (a + b + c) / 2;
                var s2 = p * (p - a) * (p - b) * (p - c);
                return System.Math.Sqrt(s2);
            }
        }

        public static double GetCircleRadius(this Triangle triangle)
        {
            var a = triangle.AB;
            var b = triangle.BC;
            var c = triangle.AC;
            var p = (a + b + c) / 2;

            var s21 = p * (p - a);
            var s22 = (p - b) * (p - c);
            var s2 = s21 * s22;
            var r = a * b * c / (System.Math.Sqrt(s2) * 4);
            return r;
        }

        public static bool FitIntoCircle(this Triangle triangle, int radius)
        {
            var a = triangle.AB;
            var b = triangle.BC;
            var c = triangle.AC;
            var p = (a + b + c) / 2;
            var p1 = p * (p - a);
            var p2 = (p - b) * (p - c);

            var n1 = a * b / 4 / p1 * c;
            var n2 = a * b / 4 / p2 * c;
            var r2 = n1 * n2;
            var r = Convert.ToDouble(radius) * radius;
            return r2 < r;
        }

        public static bool IsShrinkable(this Triangle triangle)
        {
            var abShrinkable = !triangle.AB.Length.N.IsPrime;
            var acShrinkable = !triangle.AC.Length.N.IsPrime;
            var bcShrinkable = !triangle.BC.Length.N.IsPrime;
            return abShrinkable && bcShrinkable && acShrinkable;
        }

        public static Triangle Shrink(this Triangle triangle)
        {
            if (!triangle.IsShrinkable())
                return triangle;

            var n1 = triangle.AB.Length.N;
            var n2 = triangle.AC.Length.N;
            var n3 = triangle.BC.Length.N;
            var mul1 = n1.Numerator.SplitOnMultipliers();
            var mul2 = n2.Numerator.SplitOnMultipliers();
            var mul3 = n3.Numerator.SplitOnMultipliers();

            var intersect = mul1.Intersect(mul2).Intersect(mul3).ToArray();

            return null;

            if (triangle.AB > triangle.AC)
            {
                var ac = new LineEquation(triangle.A, triangle.C);
                var x = short.MaxValue;
                if (ac.K.Denominator > 1)
                {
                    var rest = (short)(x % Convert.ToInt16(ac.K.Denominator * ac.B.Denominator));
                    x = (short)(x - rest);
                }

                var fy = ac.CalcY((uint)x);
                if (fy.IsFractional)
                {
                    throw new Exception("y shouldn't be fractional");
                }

                var y = fy.ToInt64();
                if (y > short.MaxValue)
                {
                    throw new Exception("y shouldn't be > short");
                }

                var C = new Point(x, Convert.ToInt32(y));

                return new Triangle(triangle.A, triangle.B, C);
            }
            else
            {

            }

            return triangle;
        }
    }

    public readonly struct SimilarTriangle
    {
        public SimilarTriangle(Triangle source, int k)
        {
            Source = source;
            K = k;
        }

        public Triangle Source { get; }
        public int K { get; }

        //public static SimilarTriangle From(Triangle triangle)
        //{
        //    var n1 = triangle.AB.Length.N;
        //    var n2 = triangle.AC.Length.N;
        //    var n3 = triangle.BC.Length.N;
        //    if(n1.IsFractional)
        //        throw new Exception("unexpected");
        //    var mul1 = n1.Numerator.SplitOnMultipliers();
        //    var mul2 = n2.Numerator.SplitOnMultipliers();
        //    var mul3 = n3.Numerator.SplitOnMultipliers();



        //}
    }
}