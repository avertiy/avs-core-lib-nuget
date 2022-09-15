using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Trading.Structs
{
    /// <summary>
    /// Symbol represent a trade instrument
    /// Symbol convention is [base_currency]_[quote_currency] currencies supposed to be in upper case with underscore separator, e.g. BTC_USDT
    /// quote_currency [USDT] is used to determine the value of the base_currency [BTC],
    /// so in `BTC_USDT` we buy or sell BTC for USDT)
    /// </summary>
    /// <remarks>
    /// In previous terminology i used terms of base/quote currencies in a wrong way, 
    /// it might be opposite to <see cref="AVS.CoreLib.Trading.Structs.PairString"/> it might be marked later as obsolete
    /// due to many exchanges use symbol terminology i decided to add Symbol struct
    /// </remarks>
    [TypeConverter(typeof(SymbolTypeConverter))]
    [DebuggerDisplay("Symbol: {Value}")]
    public struct Symbol : IHasValue
    {
        public Symbol(string pair)
        {
            Value = pair;
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