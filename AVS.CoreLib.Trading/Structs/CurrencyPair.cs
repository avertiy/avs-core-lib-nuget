using System;
using AVS.CoreLib.Extensions;

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
                throw new ArgumentException($"Pair string '{pair}' could not be spliced on base and quote currencies");

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

        public string ToDisplayString(char separator)
        {
            return QuoteCurrency + separator + BaseCurrency;
        }

        public override string ToString()
        {
            return $"{BaseCurrency}_{QuoteCurrency}";
        }

        public static CurrencyPair Parse(string pair, bool isBaseCurrencyFirst)
        {
            var str = pair.ToUpper();
            CurrencyPair cp = default;
            if (str.Contains("_"))
            {
                cp = new CurrencyPair(str, isBaseCurrencyFirst);
            }
            else
            {
                if (str.StartsWith("USDC", "USDT"))
                {
                    cp = new CurrencyPair(str.Substring(4, str.Length - 4), str.Substring(0, 4));
                }
                else if (str.StartsWith("BTC", "USD", "UAH", "ETH", "EUR", "RUB", "DAI"))
                {
                    cp = new CurrencyPair(str.Substring(3, str.Length - 3), str.Substring(0, 3));
                }
            }

            return cp;
        }
    }
}