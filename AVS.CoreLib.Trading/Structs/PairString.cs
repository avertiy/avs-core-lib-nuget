using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Structs
{
    /// <summary>
    /// by default PairString supposed to be a normalized pair with a base currency in first position and '_' separator e.g. USDT_BTC
    /// note here i confused base and quote currency meaning their meaning must be vise-versa 
    /// </summary>
    [TypeConverter(typeof(PairStringTypeConverter))]
    [DebuggerDisplay("PairString: {Value}")]
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

        /// <summary>
        /// under symbol mean base currency in a 2nd position
        /// </summary>
        /// <returns></returns>
        public string ToSymbol()
        {
            return Value.Swap('_');
        }
        public override string ToString()
        {
            return Value;
        }


        public static implicit operator string(PairString s)
        {
            return s.Value;
        }

        public static implicit operator PairString(string pair)
        {
            return new PairString(pair);
        }
    }

    public static class PairStringExtensions
    {
        public static string GetBaseCurrency(this PairString pair)
        {
            var parts = pair.Value.Split('_');
            if (parts.Length == 2)
                return parts[0];
            throw new ArgumentException($"Pair {pair} is not valid");
        }

        public static string GetQuoteCurrency(this PairString pair)
        {
            var parts = pair.Value.Split('_');
            if (parts.Length == 2)
                return parts[1];
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