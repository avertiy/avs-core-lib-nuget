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

        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is double d)
            {
                switch (format.ToLower())
                {
                    case "price":
                    case "p":
                        return d.FormatAsPrice();
                    case "n":
                    case "normalized":
                        return d.ToStringNormalized();
                    default:
                        return d.FormatNumber();
                }
            }
            else if (arg is decimal dec)
            {
                switch (format)
                {
                    case "price":
                    case "p":
                        return dec.FormatAsPrice();
                    case "N":
                    case "normalized":
                        return dec.ToStringNormalized();
                    default:
                        return dec.FormatNumber();
                }
            }
            return arg?.ToString();
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