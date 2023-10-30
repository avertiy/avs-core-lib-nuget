using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Types
{
    [TypeConverter(typeof(PairTypeConverter))]
    [Obsolete("use string symbol/pair with a corresponding extensions")]
    public class Pair
    {
        private readonly string _pair;
        private string _quote;
        private string _base;

        public string QuoteCurrency
        {
            get
            {
                if (_quote == null)
                {
                    InitBaseAndQuote();
                }

                return _base;
            }
        }

        public string BaseCurrency
        {
            get
            {
                if (_base == null)
                {
                    InitBaseAndQuote();
                }

                return _base;
            }
        }

        [JsonIgnore]
        public bool HasValue => _pair != null || _base != null;

        public Pair(string pair, bool isBaseCurrencyFirst = true)
        {
            if (isBaseCurrencyFirst)
            {
                _pair = pair;
            }
            else
            {
                var parts = pair.Split('_');
                if (parts.Length != 2)
                    throw new ArgumentException($"Pair string '{pair}' could not be spliced on base and quote currencies");
                _base = parts[1];
                _quote = parts[0];
            }
        }

        public Pair(string quoteCurrency, string baseCurrency)
        {
            _quote = quoteCurrency;
            _base = baseCurrency;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var p2 = (Pair)obj;
            return ToString() == p2.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public string ToDisplayString(char separator)
        {
            return QuoteCurrency + separator + BaseCurrency;
        }

        public override string ToString()
        {
            return _pair ?? _base + "_" + _quote;
        }

        private void InitBaseAndQuote()
        {
            if (_pair != null)
            {
                var arr = _pair.Split('_');
                if (arr.Length >= 2)
                {
                    _base = arr[0];
                    _quote = arr[1];
                }
            }
        }

        public static implicit operator string(Pair s)
        {
            return s.ToString();
        }
        public static implicit operator Pair(string pair)
        {
            return new Pair(pair);
        }

        public static bool operator ==(Pair p1, Pair p2)
        {
            return p1 is { } && p1.Equals(p2);
        }

        public static bool operator !=(Pair p1, Pair p2)
        {
            return !(p1 == p2);
        }

        public static Pair Parse(string pair, bool isBaseCurrencyFirst)
        {
            var str = pair.ToUpper();
            Pair cp = default;
            if (str.Contains("_"))
            {
                cp = new Pair(str, isBaseCurrencyFirst);
            }
            else
            {
                if (str.StartsWith("USDC", "USDT"))
                {
                    cp = new Pair(str.Substring(4, str.Length - 4), str.Substring(0, 4));
                }
                else if (str.StartsWith("BTC", "USD", "UAH", "ETH", "EUR", "RUB", "DAI"))
                {
                    cp = new Pair(str.Substring(3, str.Length - 3), str.Substring(0, 3));
                }
            }

            return cp;
        }
    }

    public class PairTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Pair(value.ToString().ToUpper());
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}