using AVS.CoreLib.Text.Formatters;
using System;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class TradingFormatProvider : CustomFormatter
    {
        public static TradingFormatProvider Instance = new TradingFormatProvider();
        private readonly CustomFormatter _formatter;
        public TradingFormatProvider()
        {
            var enumsFormatter = new TradingEnumsFormatter();
            var pairStringFormatter = new PairStringFormatter() { Next = enumsFormatter };
            var priceFormatter = new PriceFormatter() { Next = pairStringFormatter };
            var symbolFormatter = new CurrencySymbolFormatter() { Next = priceFormatter };
            _formatter = new NotEmptyFormatter() { Next = symbolFormatter };
        }

        public override string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return _formatter.Format(format, arg, formatProvider);
        }
    }
}