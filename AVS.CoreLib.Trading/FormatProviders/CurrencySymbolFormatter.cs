using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class CurrencySymbolFormatter : CustomFormatter
    {
        /// <summary>
        /// qualifiers: "$|symbol; i|iso"
        /// </summary>
        public static string GetQualifiers => "$|symbol; i|iso;";
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case string currency:
                    switch (format)
                    {
                        case "$":
                        case "symbol":
                            return CoinHelper.GetCurrencySymbol(currency);
                        case "i":
                        case "iso":
                            return currency;
                        default:
                            return currency;
                    }
                default:
                    return arg?.ToString();
            }
        }

        protected override bool Match(string format)
        {
            switch (format)
            {
                case "$":
                case "symbol":
                case "i":
                case "iso":
                    return true;
                default:
                    return false;
            }
        }
    }
}