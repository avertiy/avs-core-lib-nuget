using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TimeFrameEnum = AVS.CoreLib.Trading.Enums.TimeFrame;
namespace AVS.CoreLib.Trading.Structs
{
    /// <summary>
    /// why i have created this struct??
    /// </summary>
    [TypeConverter(typeof(TimeFrameTypeConverter))]
    [Obsolete("seems unnesessary struct")]
    public readonly struct TimeFrame
    {
        public static Dictionary<string, int> Literals = new Dictionary<string, int>()
        {
            {"5m", 300},
            {"15m", 900},
            {"30m", 1800},
            {"1h", 3600},
            {"2h", 7200},
            {"4h", 14400},
            {"12h", 43200},
            {"24h", 86400},
            {"1d", 86400},
            {"7d", 86400 * 7},
            {"1M", 86400 * 30}
        };
        public static string SupportedLiterals => string.Join(", ", Literals.Keys);

        public int Value { get; }

        public TimeFrame(int value)
        {
            if (value % 60 != 0)
                throw new ArgumentException("TimeFrame value is invalid", nameof(value));
            Value = value;
        }

        public override string ToString()
        {
            foreach (var kp in Literals)
                if (kp.Value == Value)
                    return kp.Key;
            return Value.ToString();
        }

        public static bool TryParse(string str, out TimeFrame timeFrame)
        {
            timeFrame = new TimeFrame(300);
            if (string.IsNullOrEmpty(str))
                return false;

            if (Literals.ContainsKey(str))
            {
                timeFrame = new TimeFrame(Literals[str]);
                return true;
            }
            return false;
        }

        public static implicit operator int(TimeFrame timeFrame)
        {
            return timeFrame.Value;
        }

        public static implicit operator TimeFrame(TimeFrameEnum value)
        {
            return new TimeFrame((int)value);
        }

        public static implicit operator TimeFrameEnum(TimeFrame timeFrame)
        {
            return (TimeFrameEnum)timeFrame.Value;
        }
    }

    public class TimeFrameTypeConverter : TypeConverter
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
                if (TimeFrame.TryParse((string)value, out var timeFrame))
                    return timeFrame;
            return base.ConvertFrom(context, culture, value);
        }
    }
}