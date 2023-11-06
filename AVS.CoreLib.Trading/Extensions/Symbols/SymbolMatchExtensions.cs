using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class SymbolMatchExtensions
    {
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
    }
}