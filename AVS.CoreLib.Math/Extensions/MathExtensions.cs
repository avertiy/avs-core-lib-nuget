namespace AVS.CoreLib.Math.Extensions
{
    public static class MathExtensions
    {
        public static double Log2(this int number)
        {
            return System.Math.Log(number, 2);
        }

        public static double Log(this int number, double @base)
        {
            return System.Math.Log(number, @base);
        }

        public static double Log(this uint number, double @base)
        {
            return System.Math.Log(number, @base);
        }

        public static double Log(this ulong number, double @base)
        {
            return System.Math.Log(number, @base);
        }

        public static uint AbsDiff(this uint a, uint b)
        {
            return a >= b ? a - b : b - a;
        }

        public static ulong AbsDiff(this ulong a, ulong b)
        {
            return a >= b ? a - b : b - a;
        }

        public static int AbsDiff(this int a, int b)
        {
            return a >= b ? a - b : b - a;
        }

        public static int AbsDiff(this uint a, int b)
        {
            return (int)(a >= b ? a - b : b - a);
        }
    }
}