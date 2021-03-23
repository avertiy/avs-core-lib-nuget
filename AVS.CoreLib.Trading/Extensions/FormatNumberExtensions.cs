using System;
using System.Globalization;

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

        public static string FormatAsPrice(this double value)
        {
            var M = 1000 * 1000;
            var K = 1000;

            if (value > M)
                return $@"{value / M:N5}M";

            if (value > K)
                return $@"{value / K:N5}K";

            if (value > 10)
                return $@"{value:N4}";

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

        public static string FormatAsPrice(this decimal value)
        {
            if (value > 999999999)
            {
                //billions
                return value.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else if (value > 999999)
            {
                //millions
                return value.ToString("0,,.###M", CultureInfo.InvariantCulture);
            }
            else if (value > 99999)
            {
                return value.ToString("0,.##K", CultureInfo.InvariantCulture);
            }
            else if (value > 9999)
            {
                return value.ToString("0,0.", CultureInfo.InvariantCulture);
            }
            else if (value > 999)
            {
                return value.ToString("0,0.00", CultureInfo.InvariantCulture);
            }
            else if (value > 10)
            {
                return value.ToString("0,0.000", CultureInfo.InvariantCulture);
            }


            //if (value > 10)
            //    return $@"{value:N4}";

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

        /// <summary>
        /// formats value as quantity i.e. keeping only meaningful digits
        /// when value is greater than billion - 1.123 B 
        /// when value is greater than million - 1.123 M 
        /// when value is greater than thousand - 1.123 K
        /// </summary>
        public static string FormatNumber(this double value)
        {
            if (value > 10000)
                return value.FormatBigNumber();
            return value.FormatSmallNumber();
        }
        /// <summary>
        /// formats value as quantity i.e. keeping only meaningful digits
        /// when value is greater than billion - 1.123 B 
        /// when value is greater than million - 1.123 M 
        /// when value is greater than thousand - 1.123 K
        /// </summary>
        public static string FormatNumber(this decimal value)
        {
            if (value > 10000)
                return value.FormatBigNumber();
            return value.FormatSmallNumber();
        }

        private static string FormatBigNumber(this double value)
        {
            if (value < 10000)
                throw new ArgumentException(nameof(value));

            var million = 1000 * 1000;
            if (value < million)
            {
                var k = value / 1000;
                return $@"{k:N3}K";
            }

            if (value < (million * 1000))
            {
                var m = value / million;
                return $@"{m:N3}M";
            }

            if (value < (million * 1000 * 1000))
            {
                var m = value / (million * 1000);
                return $@"{m:N3}B";
            }

            var b = value / (million * 1000 * 1000);
            return $@"{b:N3}T";

        }

        private static string FormatBigNumber(this decimal value)
        {
            if (value < 10000)
                throw new ArgumentException(nameof(value));

            var million = 1000 * 1000;
            if (value < million)
            {
                var k = value / 1000;
                return $@"{k:N3}K";
            }

            if (value < (million * 1000))
            {
                var m = value / million;
                return $@"{m:N3}M";
            }

            if (value < (million * 1000 * 1000))
            {
                var m = value / (million * 1000);
                return $@"{m:N3}B";
            }

            var b = value / (million * 1000 * 1000);
            return $@"{b:N3}T";

        }

        private static string FormatSmallNumber(this double value)
        {
            if (value > 10000 || value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"value must be within range [0;10000]");

            if (value >= 1)
            {
                if (value > 100)
                    return $@"{value:0.##}";
                if (value > 10)
                    return $@"{value:0.###}";

                return $@"{value:0.####}";
            }

            if (value < 0.000099)
                return $@"{value:0.########}";
            if (value < 0.00099)
                return $@"{value:0.#######}";
            if (value < 0.0099)
                return $@"{value:0.######}";
            if (value < 0.099)
                return $@"{value:0.#####}";
            if (value < 0.99)
                return $@"{value:0.####}";

            return $@"{value:0.########}";
        }

        private static string FormatSmallNumber(this decimal value)
        {
            if (value > 10000 || value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"value must be within range [0;10000]");

            if (value >= 1)
            {
                if (value > 100)
                    return $@"{value:0.##}";
                if (value > 10)
                    return $@"{value:0.###}";

                return $@"{value:0.####}";
            }

            if (value < 0.000099m)
                return $@"{value:0.########}";
            if (value < 0.00099m)
                return $@"{value:0.#######}";
            if (value < 0.0099m)
                return $@"{value:0.######}";
            if (value < 0.099m)
                return $@"{value:0.#####}";
            if (value < 0.99m)
                return $@"{value:0.####}";

            return $@"{value:0.########}";
        }
    }
}