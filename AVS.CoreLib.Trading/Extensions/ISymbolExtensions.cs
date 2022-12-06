using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class ISymbolExtensions
    {
        public static CurrencyPair ToCurrencyPair(this ISymbol obj)
        {
            return new CurrencyPair(obj.Symbol, true);
        }

        public static string QuoteCurrency(this ISymbol obj)
        {
            return obj.Symbol.QuoteCurrency();
        }

        public static string BaseCurrency(this ISymbol obj)
        {
            return obj.Symbol.BaseCurrency();
        }
    }
}