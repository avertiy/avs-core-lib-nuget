using System;
using AVS.CoreLib.Text.FormatProviders;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public static class T
    {
        private static XFormatProvider _formatProvider;
        /// <summary>
        /// Include formatters: PriceFormatter, PairStringFormatter, TradingEnumsFormatter, CurrencySymbolFormatter
        /// </summary>
        public static XFormatProvider FormatProvider
        {
            get
            {
                if (_formatProvider == null)
                {
                    _formatProvider = new XFormatProvider();
                    _formatProvider.AppendFormatter(new PriceFormatter());
                    _formatProvider.AppendFormatter(new PairStringFormatter());
                    _formatProvider.AppendFormatter(new OhlcFormatter());
                    _formatProvider.AppendFormatter(new TradingEnumsFormatter());
                    _formatProvider.AppendFormatter(new CurrencySymbolFormatter());
                }
                return _formatProvider;
            }

            set => _formatProvider = value;
        }

        /// <summary>
        /// Format string with <see cref="FormatProvider"/> matching trading formatters
        /// 
        /// PriceFormatter qualifiers:
        ///     a|amount - FormatNumber(default) [3-4 decimals];
        ///     p|price - FormatAsPrice();
        ///     q|qty|quantity - FormatNumber(3)
        ///     t|total - FormatNumber(2)
        ///     N|normalized - 0.######## [PrecisionDigits]
        ///
        /// PairString qualifiers:
        ///   q|quote - quote currency;
        ///   b|base - base currency;
        ///   p|pair - currency pair;
        ///   Q - quote currency symbol
        ///   B|$ - base currency symbol
        /// 
        /// TradingEnums qualifiers:
        ///     `+` - +/-;
        ///     c|character - buy/sell;
        ///     n|number - int value
        /// 
        /// CurrencySymbol qualifiers:
        ///     $|symbol - currency symbol;
        ///     i|iso - iso code;
        /// </summary>
        public static string Format(FormattableString str)
        {
            var result = str.ToString(FormatProvider);
            return result;
        }
    }
}