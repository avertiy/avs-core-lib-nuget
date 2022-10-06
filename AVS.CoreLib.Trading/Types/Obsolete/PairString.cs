using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Types
{
    /// <summary>
    /// by default PairString supposed to be a "normalized pair" e.g. USDT_BTC, which is opposite to symbol format BTC_USDT
    /// this is historical issue, <see cref="Symbol"/> was introduced recently
    /// <see cref="Symbol"/> is less confusing due to many exchanges upgraded their api and operate with symbols
    /// </summary>
    [TypeConverter(typeof(PairStringTypeConverter))]
    [DebuggerDisplay("PairString: {Value}")]
    [Obsolete("use Symbol instead, don't forget the difference pair format is USDT_BTC, while symbol format is BTC_USDT")]
    public class PairString : IHasValue
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

        public string ToTradingPair()
        {
            return Value.Swap('_', '/');
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
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
                return new PairString(value.ToString().ToUpper());
            return base.ConvertFrom(context, culture, value);
        }
    }
}