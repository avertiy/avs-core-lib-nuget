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
            //var ohlcFormatter = new OhlcFormatter();
            //var enumsFormatter = new TradingEnumsFormatter() { Next =  ohlcFormatter};
            //var pairStringFormatter = new PairStringFormatter() { Next = enumsFormatter };
            //var priceFormatter = new PriceFormatter() { Next = pairStringFormatter };
            //var symbolFormatter = new CurrencySymbolFormatter() { Next = priceFormatter };
            _formatter = new NotEmptyFormatter();
            _formatter.AppendFormatter(new CurrencySymbolFormatter());
            _formatter.AppendFormatter(new PriceFormatter());
            _formatter.AppendFormatter(new PairStringFormatter());
            _formatter.AppendFormatter(new TradingEnumsFormatter());
            _formatter.AppendFormatter(new OhlcFormatter());

        }

        public override string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return _formatter.Format(format, arg, formatProvider);
        }
    }
}