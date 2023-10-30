using System;

namespace AVS.CoreLib.Trading.Formatters
{
    /// <summary>
    /// takes FormattableString and applies PriceFormatProvider to it => str.ToString(PriceFormatProvider.Instance)
    /// usage: X.Format($"amount {1.25:a}; price {9.99:price}"); 
    /// </summary>
    public class PriceFormattedString
    {
        public string Value { get; }

        private PriceFormattedString(string str)
        {
            Value = str;
        }

        public static explicit operator PriceFormattedString(FormattableString str)
        {
            return new PriceFormattedString(str.ToString(PriceFormatter.Instance));
        }
        /// <summary>
        /// this is to return PriceFormattedString as a string
        /// </summary>
        public static implicit operator string(PriceFormattedString str)
        {
            return str?.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
