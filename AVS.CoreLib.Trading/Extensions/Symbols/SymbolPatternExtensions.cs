using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class SymbolPatternExtensions
    {
        /// <summary>
        /// match symbol (e.g. `BTC_USDT`) with a symbol pattern (e.g. `USDT*`) <see cref="SymbolPattern"/>
        /// </summary>
        public static bool Match(this string symbol, SymbolPattern pattern)
        {
            return pattern.Match(symbol);
        }

        /// <summary>
        /// if any symbol matches pattern <see cref="SymbolPattern"/> returns true, otherwise false 
        /// </summary>
        public static bool Match(this string[] symbols, SymbolPattern pattern)
        {
            return pattern.MatchAny(symbols);           
        }

        /// <summary>
        /// filter symbols applying symbol pattern (filter) <see cref="SymbolPattern"/>
        /// </summary>        
        public static string[] Filter(this string[] symbols, string pattern)
        {
            return SymbolPattern.From(pattern).Filter(symbols);          
        }
    }
}