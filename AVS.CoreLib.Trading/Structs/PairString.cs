using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Trading.Structs
{
    /// <summary>
    /// by default PairString supposed to be a normalized pair with a base currency in first position and '_' separator e.g. BTC_LTC
    /// </summary>
    [TypeConverter(typeof(PairStringTypeConverter))]
    public struct PairString : IHasValue
    {
        public PairString(string pair)
        {
            Value = pair;
        }

        public string Value;
        [JsonIgnore]
        public bool HasValue => !string.IsNullOrEmpty(Value);
        [JsonIgnore]
        public bool HasSeparator => Value.Contains("_");

        public static implicit operator string(PairString s)
        {
            return s.Value;
        }
        public static implicit operator PairString(string pair)
        {
            return new PairString(pair);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public static class PairStringExtensions
    {
        public static string GetBaseCurrency(this PairString pair)
        {
            var parts = pair.Value.Split('_');
            if (parts.Length == 2)
                return parts[1];
            throw new ArgumentException($"Pair {pair} is not valid");
        }

        public static string GetQuoteCurrency(this PairString pair)
        {
            var parts = pair.Value.Split('_');
            if (parts.Length == 2)
                return parts[0];
            throw new ArgumentException($"Pair {pair} is not valid");
        }
    }

    public class PairStringTypeConverter : TypeConverter
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
                return new PairString(value.ToString().ToUpper());
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}