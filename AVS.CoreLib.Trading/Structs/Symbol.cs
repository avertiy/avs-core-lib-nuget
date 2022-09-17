using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Trading.Structs
{
    /// <summary>
    /// Symbol represent a trade instrument, most exchanges operate symbols 
    /// <see cref="Symbol"/> format convention is [base_currency]_[quote_currency] e.g. BTC_USDT
    /// </summary>
    /// <remarks>
    /// this type replace <see cref="PairString"/>  pair `USDT_BTC` pair is a `BTC_USDT` symbol, i.e. swap quote and base currencies 
    /// </remarks>
    [TypeConverter(typeof(SymbolTypeConverter))]
    [DebuggerDisplay("Symbol: {Value}")]
    public struct Symbol : IHasValue
    {
        [DebuggerStepThrough]
        public Symbol(string value)
        {
            Value = value;
        }

        public string Value;

        [JsonIgnore]
        public bool HasValue => !string.IsNullOrEmpty(Value);
        [JsonIgnore]
        public bool HasSeparator => Value.Contains("_");
        [JsonIgnore]
        public string Base => GetBaseCurrency();
        [JsonIgnore]
        public string Quote => GetQuoteCurrency();

        public override string ToString()
        {
            return Value;
        }
       
        public static implicit operator string(Symbol s)
        {
            return s.Value;
        }

        public static implicit operator Symbol(string value)
        {
            return new Symbol(value);
        }

        public string GetBaseCurrency()
        {
            var parts = Value.Split('_');
            if (parts.Length == 2)
                return parts[0];
            throw new ArgumentException($"Symbol {this} is not valid");
        }

        public string GetQuoteCurrency()
        {
            var parts = Value.Split('_');
            if (parts.Length == 2)
                return parts[1];
            throw new ArgumentException($"Symbol {this} is not valid");
        }

        public string ToTradingPair()
        {
            return Value.Replace('_', '/');
        }
    }

    public static class SymbolExtensions
    {
        public static XSymbol ToXSymbol(this Symbol symbol, string exchange)
        {
            return new XSymbol(symbol.Value, exchange);
        }
    }

    public class SymbolTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Symbol(value.ToString().ToUpper());
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}