using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Extensions
{
    /// <summary>
    /// supposed to deal with a normalized symbol like BTC_USDT (i.e. currencies are upper case, underscore separator)  
    /// </summary>
    public static class SymbolExtensions
    {
        public static string GetBaseCurrency(this string symbol)
        {
            return symbol.Split('_')[0];
        }

        public static string GetQuoteCurrency(this string symbol)
        {
            return symbol.Split('_')[1];
        }
    }

    public static class PairExtensions
    {
        public static bool IsBtcPair(this string pair)
        {
            return pair.StartsWith("BTC_");
        }

        public static bool IsBnbPair(this string pair)
        {
            return pair.StartsWith("BNB_");
        }

        public static bool IsEthPair(this string pair)
        {
            return pair.StartsWith("ETH_");
        }

        public static bool IsUahPair(this string pair)
        {
            return pair.StartsWith("UAH_");
        }

        public static bool IsUsdPair(this string pair)
        {
            var cp = CurrencyPair.Parse(pair, true);
            return cp.BaseCurrency.Contains("USD");
        }

        public static bool IsUsdPeggedPair(this string pair)
        {
            var cp = CurrencyPair.Parse(pair, true);
            return cp.BaseCurrency.Contains("USD") || cp.BaseCurrency.Either("DAI", "PAX");
        }

        public static bool MatchBaseCurrency(this string pair, string currency)
        {
            return pair.StartsWith(currency + "_");
        }

        public static bool MatchQuoteCurrency(this string pair, string currency)
        {
            return pair.EndsWith("_" + currency);
        }

        public static bool MatchQuoteCurrency(this IEnumerable<string> symbols, string pair)
        {
            return symbols.Any(x => pair.EndsWith("_" + x));
        }

        public static bool MatchBaseCurrency(this IEnumerable<string> symbols, string pair)
        {
            return symbols.Any(x => pair.StartsWith(x + "_"));
        }

        public static CurrencyPair GetCurrencyPair(this IPair obj)
        {
            return new CurrencyPair(obj.Pair, true);
        }

        public static string QuoteCurrency(this IPair obj)
        {
            return obj.GetCurrencyPair().QuoteCurrency;
        }

        public static string BaseCurrency(this IPair obj)
        {
            return obj.GetCurrencyPair().BaseCurrency;
        }

        public static string QuoteCurrency(this string pair)
        {
            return new CurrencyPair(pair).QuoteCurrency;
        }

        public static string BaseCurrency(this string pair)
        {
            return new CurrencyPair(pair).BaseCurrency;
        }
    }
}