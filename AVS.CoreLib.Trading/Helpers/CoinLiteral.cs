using System.Linq;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
    /// <summary>
    /// Literals for wide quote filter <see cref="SymbolFilterExtensions.Filter"/>
    /// e.g. USDT* means match either USDT/USDC/BUSD etc. 
    /// </summary>
    public static class CoinLiteral
    {
        /// <summary>
        /// * means any symbol is ok 
        /// </summary>
        public const string ANY = "*";
        /// <summary>
        /// USD* means USD + USDT,USDC,BUSD,DAI etc. 
        /// </summary>
        public const string ANY_USD_LIKE = "USD*";
        /// <summary>
        /// BTC* means BTC + ETH,BNB,TRX etc. i.e. any TOP symbol that might serve as a quote symbol in trading pairs
        /// </summary>
        public const string ANY_TOP = "BTC*";
        /// <summary>
        /// USDT* means USDT + USDC,BUSD,USDJ,USDD,TUSD,VAI,DAI etc. i.e. any stable or pegged coin
        /// </summary>
        public const string ANY_STABLE = "USDT*";
        /// <summary>
        /// FIAT means USD,EUR,UAH etc. <see cref="Fiat"/>
        /// </summary>
        public const string ANY_FIAT = "FIAT";

        public static bool IsLiteral(string str)
        {
            return str switch
            {
                ANY => true,
                ANY_USD_LIKE => true,
                ANY_STABLE => true,
                ANY_FIAT => true,
                ANY_TOP => true,
                _ => false
            };
        }

        
    }

    public static class SymbolFilterExtensions
    {
        /// <summary>
        /// returns true if filter is <see cref="CoinLiteral"/> that matches the symbol
        /// </summary>
        public static bool Match(this string symbol, string filter)
        {
            return filter switch
            {
                CoinLiteral.ANY_USD_LIKE => symbol.EndsWith("USD") || CoinHelper.StableCoins.Any(symbol.EndsWith),
                CoinLiteral.ANY_STABLE => CoinHelper.StableCoins.Any(symbol.EndsWith),
                CoinLiteral.ANY_FIAT => CoinHelper.Fiat.Any(symbol.EndsWith),
                CoinLiteral.ANY_TOP => CoinHelper.Top.Any(symbol.EndsWith),
                CoinLiteral.ANY => true,
                _ => false
            };
        }

        /// <summary>
        /// match symbol with symbols, if any match found returns true, otherwise false.
        /// support
        /// (i) normal symbols e.g. [`BTC_USDT`,`BTC_BUSD`] match `BTC_USDT` => true
        /// (ii) wide symbols with `*` placeholder: e.g. `*_USDT`, `ATOM_*`
        ///  [`BTC_USDT`,`BTC_BUSD`] match `BTC_*` => true  or [`BTC_USDT`,`BTC_BUSD`] match `*_USDT` => true
        ///  [`BTC_*`,`*_BUSD`] match `BTC_USDT` => true 
        /// (iii) coin literals e.g. `ATOM_USDT*`, `ATOM_FIAT` <see cref="CoinLiteral"/>
        /// </summary>
        public static bool Match(this string[] symbols, string symbol)
        {
            if (symbol == "*")
                return true;

            if (CoinLiteral.IsLiteral(symbol) && symbols.Any(x => x.Match(symbol)))
            {
                return true;
            }

            foreach (var s in symbols)
            {
                if (s == symbol)
                    return true;

                if (!s.Contains('*'))
                    continue;

                var parts = s.Split('_');
                if (parts.Length == 1 && parts[0] == "*")
                    return true;

                if (parts.Length != 2)
                    continue;

                if (parts[0] == "*" && symbol.EndsWith(parts[1]))
                    return true;

                if (parts[1] == "*" && symbol.StartsWith(parts[0]))
                    return true;

                if (CoinLiteral.IsLiteral(parts[1]) && symbol.Match(parts[1]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// filter symbols, the filter might be a concrete symbol(s) comma-separated or quote asset (e.g. BTC, USDT etc.) or wide quote assets <see cref="CoinLiteral"/>
        /// </summary>
        /// <param name="symbols">symbols to filter</param>
        /// <param name="filter">
        ///  - `XXX` - a certain asset e.g. BTC, UAH etc.
        ///  - `*` or `null` - return all <see cref="symbols"/>
        ///  - `USD*` - symbols with quote asset match either USD, USDT, BUSD, DAI etc.
        ///  - `BTC*`  - symbols with quote asset match <see cref="CoinHelper.Top"/> currencies
        ///  - `FIAT`  - symbols with quote asset match <see cref="CoinHelper.Fiat"/> currencies
        /// </param>
        public static string[] Filter(this string[] symbols, string filter)
        {
            if (string.IsNullOrEmpty(filter) || filter == CoinLiteral.ANY)
                return symbols;

            if (CoinLiteral.IsLiteral(filter))
            {
                return symbols.Where(x => Match(x, filter)).ToArray();
            }

            // concrete quote asset: BTC, TRX, USDT, UAH etc.
            if (!filter.Contains('_'))
                return symbols.Where(x => x.EndsWith(filter)).ToArray();

            var arr = filter.Contains(',') ? filter.Split(',') : new[] { filter };
            return symbols.Where(x => arr.Contains(x)).ToArray();
        }
    }
}