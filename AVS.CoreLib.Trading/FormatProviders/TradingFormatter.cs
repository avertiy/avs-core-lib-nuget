using System;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class TradingFormatter : CustomFormatter
    {
        public static TradingFormatter Instance = new TradingFormatter();
        private readonly CustomFormatter _formatter;
        public TradingFormatter()
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