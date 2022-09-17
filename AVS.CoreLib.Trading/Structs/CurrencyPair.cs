using System;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Trading.Structs
{
    public readonly struct CurrencyPair
    {
        public string QuoteCurrency { get; }
        public string BaseCurrency { get; }

        public CurrencyPair(string quoteCurrency, string baseCurrency)
        {
            QuoteCurrency = quoteCurrency;
            BaseCurrency = baseCurrency;
        }

        public CurrencyPair(string pair, bool isBaseCurrencyFirst = true)
        {
            var parts = pair.Split('_');
            if (parts.Length != 2)
                throw new ArgumentException($"`{pair}` invalid currency pair");

            if (isBaseCurrencyFirst)
            {
                BaseCurrency = parts[0];
                QuoteCurrency = parts[1];
            }
            else
            {
                BaseCurrency = parts[1];
                QuoteCurrency = parts[0];
            }
        }

        [JsonIgnore]
        public bool HasValue => BaseCurrency != null && QuoteCurrency != null;

        public static explicit operator CurrencyPair(string pair)
        {
            return new CurrencyPair(pair, true);
        }

        public static bool operator ==(CurrencyPair p1, CurrencyPair p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(CurrencyPair p1, CurrencyPair p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var p2 = (CurrencyPair)obj;
            return (BaseCurrency == p2.BaseCurrency && QuoteCurrency == p2.QuoteCurrency);
        }

        public override int GetHashCode()
        {
            return BaseCurrency.GetHashCode() ^ QuoteCurrency.GetHashCode();
        }

        public string ToTradingPair()
        {
            return $"{BaseCurrency}/{QuoteCurrency}";
        }

        public string ToSymbol()
        {
            return $"{BaseCurrency}_{QuoteCurrency}";
        }

        public string ToString(char separator)
        {
            return $"{BaseCurrency}{separator}{QuoteCurrency}";
        }

        public override string ToString()
        {
            return $"{BaseCurrency}_{QuoteCurrency}";
        }

        public static CurrencyPair Parse(string pair, bool isBaseCurrencyFirst = true)
        {
            var parts = pair.Split('_');
            if (parts.Length != 2)
                throw new ArgumentException($"`{pair}` invalid currency pair");

            return isBaseCurrencyFirst ? new CurrencyPair(parts[1], parts[0]) : new CurrencyPair(parts[0], parts[1]);
        }
    }
}