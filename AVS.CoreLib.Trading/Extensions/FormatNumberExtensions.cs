using System;
using System.Globalization;
using AVS.CoreLib.Trading.Constants;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class FormatNumberExtensions
    {
        /// <summary>
        /// format numbers to string using n.FormatPrice()
        /// leading zeros will be replaced by replacement
        /// (e.g. 0.0001234 => 0..1234) 
        /// </summary>
        public static string[] FormatPrices(this double[] numbers, string replacement = "0..")
        {
            if (numbers.Length < 2)
                throw new ArgumentException("At least 2 numbers are expected");
            var arr = new string[numbers.Length];
            var str = numbers[0].FormatAsPrice();
            var ind = str.IndexOfAny(new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            string strToReplace = String.Empty;
            if (ind > 4)
                strToReplace = str.Substring(0, ind);
            //arr[0] = str;
            for (var i = 0; i < numbers.Length; i++)
            {
                str = numbers[i].FormatAsPrice();
                if (ind > 4)
                    arr[i] = str.Replace(strToReplace, replacement);
                else
                    arr[i] = str;
            }

            return arr;
        }

        public static string FormatAsPrice(this double value, int? decimalPlaces = null)
        {
            return FormatAsPrice((decimal)value, decimalPlaces);
        }

        public static string FormatAsPrice(this decimal value, int? decimalPlaces = null)
        {
            if (value >= 1)
            {
                if (value < 100)
                    return Format(value, decimalPlaces ?? 3, string.Empty);

                if (value < 100000)
                    return Format(value, decimalPlaces ?? 2, string.Empty);

                var million = 1000000m;
                if (value < million)
                {
                    var k = value / 1000;
                    return Format(k, decimalPlaces ?? 3, "K");
                }

                if (value < (million * 1000))
                {
                    var m = value / million;
                    return Format(m, decimalPlaces ?? 3, "M");
                }

                if (value < (million * 1000 * 1000))
                {
                    var b = value / (million * 1000);
                    return Format(b, decimalPlaces ?? 3, "B");
                }

                var t = value / (million * 1000 * 1000);
                return Format(t, decimalPlaces ?? 3, "T");
            }
            else
            {
                //0.100456 => 100.4 => 0.456
                var v = value * 1000;
                var diff = v - Math.Floor(v);
                if (diff > 0)
                {
                    v = value * 10000;
                    diff = v - Math.Floor(v);
                    if (diff > 0)
                        return $@"{value:N8}";
                    return $@"{value:N4}";
                }
                return $@"{value:N3}";
            }
        }

        public static string FormatAsTotal(this decimal value, int? decimalPlaces = null)
        {
            return FormatNumber(value, decimalPlaces ?? 2);
        }

        public static string FormatNumber(this decimal value, int? decimalPlaces = null)
        {
            if (value >= 1)
            {
                if (value < 100)
                    return Format(value, decimalPlaces ?? 3, string.Empty);

                var million = 1000000m;
                if (value < million)
                {
                    var k = value / 1000;
                    return Format(k, decimalPlaces ?? 3, "K");
                }

                if (value < (million * 1000))
                {
                    var m = value / million;
                    return Format(m, decimalPlaces ?? 3, "M");
                }

                if (value < (million * 1000 * 1000))
                {
                    var b = value / (million * 1000);
                    return Format(b, decimalPlaces ?? 3, "B");
                }

                var t = value / (million * 1000 * 1000);
                return Format(t, decimalPlaces ?? 3, "T");
            }

            if (value >= 0)
            {
                if (value < TradingConstants.OneSatoshi / 10)
                    return "0";

                var million = 1000000m;
                if (value < 0.000009m)
                    return Format(value*million, decimalPlaces ?? 3,"mk");

                if (value < 0.01m)
                    return Format(value * 1000, decimalPlaces ?? 3, "m");

                return Format(value, decimalPlaces ?? 3, string.Empty);
            }
            else
            {
                return "-" + FormatNumber(-1 * value, decimalPlaces);
            }
        }

        public static string FormatNumber(this double value, int? decimalPlaces = null)
        {
            return FormatNumber((decimal)value, decimalPlaces);
        }
        private static string Format(decimal d, int n, string suffix)
        {
            return n switch
            {
                0 => $"{d:N0}{suffix}",
                1 => $"{d:N1}{suffix}",
                2 => $"{d:N2}{suffix}",
                3 => $"{d:N3}{suffix}",
                _ => $"{d:N4}{suffix}"
            };
        }
    }
}