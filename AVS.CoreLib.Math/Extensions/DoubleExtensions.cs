namespace AVS.CoreLib.Math.Extensions
{
    public static class DoubleExtensions
    {
        public static bool IsIntegerValue(this double d)
        {
            return System.Math.Abs(d % 1) <= (double.Epsilon * 100);
        }

        public static int GetPrecisionLength(this double d)
        {
            var floor = System.Math.Floor(d);
            var rest = d - floor;
            var str = rest.ToString("N60").Trim('0');
            return str.Length - 1;
        }
        public static double Floor(this double d)
        {
            return System.Math.Floor(d);
        }

        public static double RoundDown(this double d, int decimalPlaces)
        {
            return System.Math.Floor(d * System.Math.Pow(10, decimalPlaces)) / System.Math.Pow(10, decimalPlaces);
        }
        public static string ToString2(this double d)
        {
            return d.ToString("N60").TrimEnd('0', '.');
        }
    }
}