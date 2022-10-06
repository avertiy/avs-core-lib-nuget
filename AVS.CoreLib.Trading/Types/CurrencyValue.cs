using System;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Types
{
    /// <summary>
    /// Represents values like 10.00 UAH, 2.2 XRP etc.
    /// </summary>
    public class CurrencyValue
    {
        public decimal Value { get; private set; }

        public string Currency { get; }

        public CurrencyValue(string currency, decimal value)
        {
            Currency = currency;
            Value = value;
        }

        /// <summary>
        /// parses string for the value
        /// </summary>
        /// <param name="str">"100.00 UAH"</param>
        public static CurrencyValue Parse(string str)
        {
            var parts = str.Split(' ');
            if (parts.Length != 2)
                throw new ArgumentException($"String '{str}' is not recognized as a valid currency value");

            if (NumericHelper.TryParseDecimal(parts[0], out var value))
                return new CurrencyValue(parts[1], value);

            throw new ArgumentException($"Unable to parse '{str}' into currency value");
        }

        public static implicit operator CurrencyValue(string str)
        {
            return Parse(str);
        }

        public static implicit operator CurrencyValue((string currency, decimal value) tuple)
        {
            return new CurrencyValue(tuple.currency, tuple.value);
        }

        public static implicit operator decimal(CurrencyValue obj)
        {
            return obj.Value;
        }

        public static CurrencyValue operator +(CurrencyValue obj, decimal addendum)
        {
            obj.Value += addendum;
            return obj;
        }

        public static CurrencyValue operator -(CurrencyValue obj, decimal addendum)
        {
            obj.Value -= addendum;
            return obj;
        }

        public override string ToString()
        {
            return $"{Value} {Currency}";
        }
    }
}