using System.Linq;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Types;

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