using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    /// <summary>
    /// Represents a symbol pattern which could be on of 7 pattern types <see cref="PatternType"/>
    /// e.g. `*`, `USD*`, `FIAT` (means fiat currencies), `ETH`, `*_ETH`, `ETH_*`, `BTC_USDT,ETH_USDT`, `ETH_USDT`
    /// SymbolPattern is designed to filter/match symbol(s) 
    /// </summary>
    public class SymbolPattern
    {
        /// <summary>
        /// * means any symbol is ok 
        /// </summary>
        public const string ANY = "*";
        /// <summary>
        /// USD* means USD + USDT,USDC,BUSD,DAI etc. 
        /// </summary>
        public const string USD_LIKE = "USD*";
        /// <summary>
        /// BTC* means BTC + ETH,BNB,TRX etc. i.e. any TOP symbol that might serve as a quote symbol in trading pairs
        /// </summary>
        public const string TOP_COINS = "BTC*";
        /// <summary>
        /// USDT* means USDT + USDC,BUSD,USDJ,USDD,TUSD,VAI,DAI etc. i.e. any stable or pegged coin
        /// </summary>
        public const string STABLECOIN = "USDT*";
        /// <summary>
        /// FIAT means USD,EUR,UAH etc. <see cref="Fiat"/>
        /// </summary>
        public const string FIAT = "FIAT";

        public string Pattern { get; private set; }
        public PatternType Type { get; private set; }

        public SymbolPattern(string pattern)
        {
            Pattern = pattern;
            Type = GetType(pattern);
        }

        public bool Match(string symbol)
        {
            return Type switch
            {
                PatternType.Any => true,
                PatternType.Symbol => symbol == Pattern,
                PatternType.MultiSymbol => Pattern.Contains(symbol),
                PatternType.Literal => MatchLiteral(Pattern, symbol),
                PatternType.Asset => symbol.StartsWith(Pattern) || symbol.EndsWith(Pattern),
                PatternType.QuoteAsset => symbol.EndsWith(Pattern.Substring(1)),
                PatternType.BaseAsset => symbol.StartsWith(Pattern.Substring(0, Pattern.Length - 2)),
                _ => false,
            };
        }

        /// <summary>
        /// if any match found returns true, otherwise false
        /// </summary>
        public bool MatchAny(IEnumerable<string> symbols)
        {
            return symbols.Any(x => Match(x));
        }

        public string[] Filter(IEnumerable<string> symbols)
        {
            return symbols.Where(x => Match(x)).ToArray();
        }

        public static PatternType GetType(string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || pattern == ANY)
                return PatternType.Any;

            if (IsLiteral(pattern))
                return PatternType.Literal;

            if (pattern.StartsWith('*'))
                return PatternType.QuoteAsset;

            if (pattern.EndsWith('*'))
                return PatternType.BaseAsset;

            if (pattern.Contains(','))
                return PatternType.MultiSymbol;

            if (pattern.Length < 6 && !pattern.Contains('_'))
                return PatternType.Asset;

            return PatternType.Symbol;
        }

        public static bool IsLiteral(string str)
        {
            return str switch
            {
                ANY => true,
                USD_LIKE => true,
                STABLECOIN => true,
                FIAT => true,
                TOP_COINS => true,
                _ => false
            };
        }

        /// <summary>
        /// match given symbol with one of literals (<see cref="ANY"/>, <see cref="FIAT"/> etc.)
        /// returns true if matches, otherwise - false
        /// </summary>
        public static bool MatchLiteral(string literal, string symbol)
        {
            return literal switch
            {
                ANY => true,
                USD_LIKE => symbol.EndsWith("USD") || CoinHelper.StableCoins.Any(symbol.EndsWith),
                STABLECOIN => CoinHelper.StableCoins.Any(symbol.EndsWith),
                FIAT => CoinHelper.Fiat.Any(symbol.EndsWith),
                TOP_COINS => CoinHelper.Top.Any(symbol.EndsWith),
                _ => symbol == literal || symbol.EndsWith(literal)
            };
        }

        public static implicit operator SymbolPattern(string pattern)
        {
            return new SymbolPattern(pattern);
        }

        public static implicit operator string(SymbolPattern pattern)
        {
            return pattern.Pattern;
        }

        public static SymbolPattern From(string pattern)
        {
            return new SymbolPattern(pattern);
        }

        public enum PatternType
        {
            /// <summary>
            /// *
            /// </summary>
            Any = 0,
            /// <summary>
            /// FIAT or USD*
            /// </summary>
            Literal = 1,
            /// <summary>
            /// exact symbol e.g. BTC_USD
            /// </summary>
            Symbol = 2,
            /// <summary>
            /// comma-separated symbols e.g. BTC_USDT,ETH_BTC
            /// </summary>
            MultiSymbol = 3,
            /// <summary>
            /// e.g.`ETH` means any pair with ETH as a base or quote asset
            /// </summary>
            Asset = 4,
            /// <summary>
            /// *_USDT means any pair quoted to USDT 
            /// </summary>
            QuoteAsset = 5,
            /// <summary>
            /// ATOM_* means any pair with ATOM as a base asset
            /// </summary>
            BaseAsset = 6
        }
    }
}