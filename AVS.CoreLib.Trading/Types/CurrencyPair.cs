using System;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Trading.Types
{
    /// <summary>
    /// in `BTC_USDT`, BTC is a base currency, USDT is a quote currency
    /// </summary>
    public class CurrencyPair
    {
        public string QuoteCurrency { get; }
        public string BaseCurrency { get; }

        public CurrencyPair(string baseCurrency, string quoteCurrency)
        {
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
        }

        public CurrencyPair(string value, bool isBaseCurrencyFirst = true)
        {
            var parts = value.Split('_', '/');
            if (parts.Length != 2)
                throw new ArgumentException($"`{value}` invalid {nameof(CurrencyPair)}");

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
            return BaseCurrency == p2.BaseCurrency && QuoteCurrency == p2.QuoteCurrency;
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
            // BTC_USDT
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

        /// <summary>
        /// Parse <see cref="CurrencyPair"/> instance from symbol/pair string
        /// </summary>
        /// <param name="value">symbols like `BTC_USDT` or pairs like 'BTC/USDT' </param>
        /// <param name="isBaseCurrencyFirst">base currency in `BTC_USDT` is BTC</param>
        public static CurrencyPair Parse(string value, bool isBaseCurrencyFirst = true)
        {
            //BTC_USDT or BTC/USDT
            var parts = value.Split('_','/');
            if (parts.Length != 2)
                throw new ArgumentException($"`{value}` invalid currency pair");

            return isBaseCurrencyFirst ? new CurrencyPair(parts[0], parts[1]) : new CurrencyPair(parts[1], parts[0]);
        }
    }
}