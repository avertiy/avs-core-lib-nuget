using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class PairStringFormatter : CustomFormatter
    {
        /// <summary>
        /// qualifiers: "q|quote; b|base; p|pair; Q|B|symbol"
        /// </summary>
        public static string GetQualifiers => "q|quote; b|base; p|pair; Q|B|$";
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case string pair:
                    switch (format)
                    {
                        case "b":
                        case "base":
                            return new CurrencyPair(pair).BaseCurrency;
                        case "q":
                        case "quote":
                            return new CurrencyPair(pair).QuoteCurrency;
                        case "Q":
                            return CoinHelper.GetCurrencySymbol(new CurrencyPair(pair).QuoteCurrency);
                        case "$":
                        case "B":
                            return CoinHelper.GetCurrencySymbol(new CurrencyPair(pair).BaseCurrency);
                        default:
                            return pair;
                    }
                case CurrencyPair cp:
                    switch (format)
                    {
                        case "b":
                        case "base":
                            return cp.BaseCurrency;
                        case "q":
                        case "quote":
                            return cp.QuoteCurrency;
                        case "Q":
                            return CoinHelper.GetCurrencySymbol(cp.QuoteCurrency);
                        case "$":
                        case "B":
                            return CoinHelper.GetCurrencySymbol(cp.BaseCurrency);
                        default:
                            return cp.ToString();
                    }
                case PairString pairString:
                    switch (format)
                    {
                        case "b":
                        case "base":
                            return new CurrencyPair(pairString.Value).BaseCurrency;
                        case "q":
                        case "quote":
                            return new CurrencyPair(pairString.Value).QuoteCurrency;
                        case "Q":
                            return CoinHelper.GetCurrencySymbol(new CurrencyPair(pairString.Value).QuoteCurrency);
                        case "$":
                        case "B":
                            return CoinHelper.GetCurrencySymbol(new CurrencyPair(pairString.Value).BaseCurrency);
                        default:
                            return pairString.Value;
                    }
                default:
                    return arg?.ToString();
            }
        }

        protected override bool Match(string format)
        {
            switch (format)
            {
                case "b":
                case "B":
                case "p":
                case "q":
                case "Q":
                case "base":
                case "quote":
                case "pair":
                case "symbol":
                    return true;
                default:
                    return false;
            }
        }
    }
}