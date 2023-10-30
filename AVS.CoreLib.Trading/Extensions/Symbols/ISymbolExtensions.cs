using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class ISymbolExtensions
    {
        public static string QuoteCurrency(this ISymbol obj)
        {
            return obj.Symbol.Q();
        }

        public static string BaseCurrency(this ISymbol obj)
        {
            return obj.Symbol.B();
        }
    }
}