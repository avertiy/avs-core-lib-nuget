using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.FormatProviders;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Extensions
{
    /// <summary>
    /// supposed to deal with a normalized symbol like BTC_USDT (i.e. currencies are upper case, underscore separator)  
    /// </summary>
    public  static partial class SymbolExtensions
    {
        /// <summary>
        /// returns quote currency for BTC_USDT => USDT
        /// </summary>
        public static string QuoteCurrency(this string symbol)
        {
            return symbol.Split('_')[1];
        }

        /// <summary>
        /// returns base currency for BTC_USDT => BTC
        /// </summary>
        public static string BaseCurrency(this string symbol)
        {
            return symbol.Split('_')[0];
        }

        /// <summary>
        /// returns quote currency, when possible replace currency iso code with symbol e.g. USDT => $
        /// <seealso cref="CoinHelper.GetCurrencySymbol"/>
        /// </summary>
        public static string Q(this string symbol, bool replaceIsoCodeWithCurrencySymbol = true)
        {
            return CoinHelper.GetCurrencySymbol(QuoteCurrency(symbol));
        }

        /// <summary>
        /// returns base currency for BTC_USDT => BTC
        /// </summary>
        public static string B(this string symbol)
        {
            return symbol.BaseCurrency();
        }

        public static bool MatchBaseCurrency(this string symbol, string currency)
        {
            return symbol.StartsWith(currency + "_");
        }

        public static bool MatchQuoteCurrency(this string symbol, string currency)
        {
            return symbol.EndsWith("_" + currency);
        }

        public static bool MatchQuoteCurrency(this IEnumerable<string> symbols, string symbol)
        {
            return symbols.Any(x => symbol.EndsWith("_" + x));
        }

        public static bool MatchBaseCurrency(this IEnumerable<string> symbols, string symbol)
        {
            return symbols.Any(x => symbol.StartsWith(x + "_"));
        }

        public static string Normalize(string symbol, bool isBaseCurrencyFirst)
        {
            var s = symbol.ToUpper();
            if (s.Length == 6)
                s = s.Insert(3, "_");

            return isBaseCurrencyFirst ? s : s.Swap('_');
        }
    }

    public  static partial class SymbolExtensions
    {
        public static bool IsBtcSymbol(this string symbol)
        {
            return symbol.StartsWith("BTC_");
        }

        public static bool IsBnbSymbol(this string symbol)
        {
            return symbol.StartsWith("BNB_");
        }

        public static bool IsEthSymbol(this string symbol)
        {
            return symbol.StartsWith("ETH_");
        }

        public static bool IsUahSymbol(this string symbol)
        {
            return symbol.StartsWith("UAH_");
        }

        public static bool IsUsdSymbol(this string symbol)
        {
            return symbol.Split('_')[0].Contains("USD");
        }

        public static bool IsUsdPeggedPair(this string symbol)
        {
            var quote = symbol.QuoteCurrency();
            return quote.Contains("USD") || quote.Either("DAI", "PAX");
        }
    }
}