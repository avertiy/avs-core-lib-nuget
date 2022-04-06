using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.FormatProviders
{
    /// <summary> 
    /// qualifiers: a|amount; p|price; q|qty|quantity; n|number 
    /// represents ICustomFormatter implementation (usually it is used by FormatProvider e.g. PriceFormatProvider) 
    /// usage:
    /// - string.Format(new PriceFormatProvider(), "amount: {1.25:a}; price: {9.99:price}");
    /// - X.Format($"amount {1.25:a}; price {9.99:price}"); 
    /// </summary>
    public class PriceFormatter : CustomFormatter
    {
        public static readonly PriceFormatter Instance = new PriceFormatter();

        /// <summary>
        /// qualifiers: a|amount; p|price; q|qty|quantity; n|number; t|total
        /// </summary>
        public static string GetQualifiers => "a|amount; p|price; q|qty|quantity; n|number; t|total; N|normalized";

        /// <summary>
        /// format double/decimal argument to string
        /// </summary>
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            return arg switch
            {
                double d => FormatDecimal(format, Convert.ToDecimal(d)),
                decimal dec => FormatDecimal(format, dec),
                _ => arg?.ToString()
            };
        }

        private static string FormatDecimal(string format, decimal d)
        {
            switch (format)
            {
                case "price":
                case "p":
                    return d.FormatAsPrice();
                case "total":
                case "t":
                    return d.FormatAsTotal();
                case "quantity":
                case "qty":
                case "q":
                    return d.FormatNumber(3);
                case "N":
                case "normalized":
                    return d.ToStringNormalized();
                default:
                    return d.FormatNumber();
            }
        }

        protected override bool Match(string format)
        {
            switch (format)
            {
                case "amount":
                case "a":
                case "price":
                case "p":
                case "quantity":
                case "q":
                case "qty":
                case "n":
                case "number":
                case "total":
                case "t":
                case "N":
                case "normalized":
                    return true;
                default:
                    return false;
            }
        }
    }
}