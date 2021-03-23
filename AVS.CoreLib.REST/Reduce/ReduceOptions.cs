using System;
using System.ComponentModel;
using System.Globalization;

namespace AVS.CoreLib.REST.Reduce
{
    /// <summary>
    /// Represents pair of decimal Value & Factor
    /// Value is a threshold for Reducer function
    /// Factor is used to determine a dynamic threshold (when Value is 0)
    /// Reduce options are used by Reduce`T enumerable extension method 
    /// </summary>
    [TypeConverter(typeof(ReduceOptionsTypeConverter))]
    public readonly struct ReduceOptions
    {
        /// <summary>
        /// Indicates threshold, when 0 a dynamic threshold takes place <see cref="Factor"/>
        /// </summary>
        public decimal Value { get; }
        /// <summary>
        /// Indicates a dynamic threshold, that is calculated as Average x Factor
        /// </summary>
        public decimal Factor { get; }

        public bool IsDynamic => Value <= 0;

        public bool HasValue => Value > 0 || Factor > 0;

        public ReduceOptions(decimal value, decimal factor = 1)
        {
            if (factor < 0)
                throw new ArgumentOutOfRangeException(nameof(factor), "must be greater than 0");

            Factor = factor;
            Value = value;
        }

        public override string ToString()
        {
            if (IsDynamic)
                return $"x{Factor}";
            return $"{Value}";
        }

        public static bool TryParse(string str, out ReduceOptions options)
        {
            options = new ReduceOptions();
            if (string.IsNullOrEmpty(str))
                return false;

            if (str.StartsWith("x"))
            {
                if (decimal.TryParse(str.Substring(1), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value))
                {
                    options = new ReduceOptions(0, value);
                    return true;
                }
            }
            else
            {
                if (decimal.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value))
                {
                    options = new ReduceOptions(value);
                    return true;
                }
            }
            return false;
        }

        public static implicit operator decimal(ReduceOptions options) => options.Value;

        public static implicit operator ReduceOptions((decimal value, decimal factor) options)
        {
            return new ReduceOptions(options.value, options.factor);
        }

        public static implicit operator ReduceOptions(string value)
        {
            if (TryParse(value, out ReduceOptions options))
                return options;

            throw new Exception($"String '{value}' is not valid ReduceOptions value");
        }
    }

    public class ReduceOptionsTypeConverter : TypeConverter
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
                if (ReduceOptions.TryParse((string)value, out ReduceOptions options))
                {
                    return options;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}