using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class PairStringFormatter : CustomFormatter
    {
        /// <summary>
        /// qualifiers: "q|quote; b|base; p|pair; Q|B|symbol"
        /// </summary>
        public static string GetQualifiers => "q|quote; b|base; Q|B; p|pair; s|symbol;";
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case string symbol:
                    switch (format)
                    {
                        case "b":
                        case "base":
                            return new Symbol(symbol).Base;
                        case "q":
                        case "quote":
                            return new Symbol(symbol).Quote;
                        case "Q":
                            return CoinHelper.GetCurrencySymbol(new Symbol(symbol).Quote);
                        case "B":
                            return CoinHelper.GetCurrencySymbol(new Symbol(symbol).Base);
                        case "p":
                        case "pair":
                            return new Symbol(symbol).ToTradingPair();
                        default:
                            return symbol;
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
                        case "B":
                            return CoinHelper.GetCurrencySymbol(cp.BaseCurrency);

                        case "p":
                        case "pair":
                            return cp.ToTradingPair();
                        case "s":
                        case "symbol":
                            return cp.ToSymbol();
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
                        case "B":
                            return CoinHelper.GetCurrencySymbol(new CurrencyPair(pairString.Value).BaseCurrency);
                        case "p":
                        case "pair":
                            return pairString.ToTradingPair();
                        case "s":
                        case "symbol":
                            return pairString.ToSymbol();
                        default:
                            return pairString.Value;
                    }
                case Symbol symbol:
                    switch (format)
                    {
                        case "b":
                        case "base":
                            return symbol.Base;
                        case "q":
                        case "quote":
                            return symbol.Quote;
                        case "Q":
                            return CoinHelper.GetCurrencySymbol(symbol.Quote);
                        case "B":
                            return CoinHelper.GetCurrencySymbol(symbol.Base);
                        case "p":
                        case "pair":
                            return new Symbol(symbol).ToTradingPair();
                        default:
                            return symbol.Value;
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
                case "s":
                case "symbol":
                    return true;
                default:
                    return false;
            }
        }
    }
}