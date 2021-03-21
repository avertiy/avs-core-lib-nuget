using AVS.CoreLib.Extensions.Numbers;
using System;
using System.Globalization;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class NumericHelper
    {
        public static double ParseDouble(string value, NumberStyles style = NumberStyles.Float)
        {
            if (string.IsNullOrEmpty(value))
                return 0.0;
            return double.Parse(value, style, CultureInfo.InvariantCulture).Normalize();
        }

        public static decimal ParseDecimal(string value, NumberStyles style = NumberStyles.Float)
        {
            if (string.IsNullOrEmpty(value))
                return 0.0m;
            return decimal.Parse(value, style, CultureInfo.InvariantCulture);
        }

        public static bool TryParseDouble(string value, out double res)
        {
            res = 0;
            if (string.IsNullOrEmpty(value))
                return false;

            double k = 1;
            if (value.EndsWith("K"))
                k = 1000;
            if (value.EndsWith("M"))
                k = 1000 * 1000;

            var len = k > 1 ? value.Length - 1 : value.Length;

            if (double.TryParse(value.Substring(0, len), NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                if (k > 1)
                    res = (res * k).RoundUp(8);
                return true;
            }
            return false;
        }

        public static bool TryParseDecimal(string value, out decimal res)
        {
            res = 0;
            if (string.IsNullOrEmpty(value))
                return false;

            decimal k = 1.0m;
            if (value.EndsWith("K"))
                k = 1000;
            if (value.EndsWith("M"))
                k = 1000 * 1000;

            var len = k > 1 ? value.Length - 1 : value.Length;

            if (decimal.TryParse(value.Substring(0, len), NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                if (k > 1)
                    res = (res * k).RoundUp(8);
                return true;
            }
            return false;
        }

        public static bool AreSame(decimal price1, decimal price2)
        {
            return Math.Abs(price1 - price2) <= Constants.TradingConstants.OneSatoshi;
        }
    }
}