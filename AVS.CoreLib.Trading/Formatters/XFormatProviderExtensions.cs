using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Formatters;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class XFormatProviderExtensions
    {
        /// <summary>
        /// Add string formatters
        ///     <see cref="PriceFormatter"/>
        ///     <see cref="PairStringFormatter"/>
        ///     <see cref="OhlcFormatter"/>
        ///     <see cref="CurrencySymbolFormatter"/>
        /// and type formatters for enums:
        ///     <see cref="TradeType"/>
        ///     <see cref="OrderSide"/>
        ///     <see cref="PositionType"/>
        /// </summary>
        public static void AddTradingFormatters(this XFormatProvider provider)
        {
            provider.ConfigureCompositeFormatter(x => x.AddTradeTypeFormatter().AddOrderSideFormatter().AddPositionTypeFormatter());
            provider.AppendFormatter(new PriceFormatter());
            provider.AppendFormatter(new PairStringFormatter());
            provider.AppendFormatter(new OhlcFormatter());
            provider.AppendFormatter(new CurrencySymbolFormatter());
        }
    }
}