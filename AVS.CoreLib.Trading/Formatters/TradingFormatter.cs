using System;
using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Text.Formatters;

namespace AVS.CoreLib.Trading.Formatters
{
    /// <summary>
    /// Combines a set of custom trading formatters such as
    /// <see cref="CurrencySymbolFormatter"/>
    /// <see cref="PriceFormatter"/>
    /// <see cref="PairStringFormatter"/>
    /// <see cref="OhlcFormatter"/>
    /// used by X.Format util <seealso cref="XFormatProvider"/>
    /// TradingFormatter is registered by <see cref="ServiceCollectionExtension.AddTradingCore"/>
    /// </summary>
    [Obsolete("User service extensions AddTradingFormatters()")]
    class TradingFormatter : CustomFormatter
    {
        private readonly CustomFormatter _formatter;
        public TradingFormatter()
        {
            _formatter = new CurrencySymbolFormatter();
            _formatter.AppendFormatter(new PriceFormatter());
            _formatter.AppendFormatter(new PairStringFormatter());
            _formatter.AppendFormatter(new OhlcFormatter());

        }

        public override string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return _formatter.Format(format, arg, formatProvider);
        }
    }
}