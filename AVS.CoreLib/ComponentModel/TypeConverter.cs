using System;
using System.ComponentModel;
using System.Globalization;

namespace AVS.CoreLib.ComponentModel
{
    public abstract class TypeConverter<T> : TypeConverter
    {
        public abstract bool Parse(string str, out T value);

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context,
            CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                if (Parse(str, out T obj))
                {
                    return obj;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

    }
}
