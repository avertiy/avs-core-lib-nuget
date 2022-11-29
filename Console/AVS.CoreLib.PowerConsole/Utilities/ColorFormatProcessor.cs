using System;
using System.Linq;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.FormatPreprocessors;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    public interface IColorFormatProcessor : IFormatPreprocessor
    {
        ColorPalette Palette { get; set; }

        ConsoleColor StringColor { get; set; }
    }

    public class ColorFormatProcessor : IColorFormatProcessor, IFormatPreprocessor
    {
        public ColorPalette Palette { get; set; } = ColorPalette.BlueGreen;

        public ConsoleColor StringColor { get; set; } = ConsoleColor.DarkGray;

        /// <inheritdoc />
        public string Process(string format, object argument)
        {
            if (string.IsNullOrEmpty(format))
            {
                if (argument is string)
                    return StringColor.ToColorSchemeString();
                var type = argument.GetType();
                if (type.IsEnum)
                    return GetFormatForEnum(type, (int)argument);
                if (type.IsPrimitive)
                    return GetFormatForPrimitive(argument);
            }
            return format;
        }

        protected string GetFormatForPrimitive(object argument)
        {
            if (argument is bool flag)
                return GetFormatForBoolean(flag);
            var num = Compare(argument, 0);
            if (num == 0)
                return Palette[0].ToColorSchemeString();
            return num < 0 ? Palette[1].ToColorSchemeString() : Palette.Length > 2 ? Palette[2].ToColorSchemeString() : Palette.Last().ToColorSchemeString();
        }

        private static int Compare(object obj, int n)
        {
            if (obj is int num)
                return num.CompareTo(n);
            if (obj is long l)
                return l.CompareTo(n);
            if (obj is double d)
                return d.CompareTo(n);
            if (obj is decimal dec)
                return dec.CompareTo(n);
            return obj is short s ? s.CompareTo(n) : 0;
        }

        protected virtual string GetFormatForEnum(Type enumType, int value)
        {
            var values = Enum.GetValues(enumType);
            for (var index = 0; index < values.Length; ++index)
                if ((int)values.GetValue(index) == value)
                    return index <= Palette.Length ? Palette[index].ToColorSchemeString() : Palette[1].ToColorSchemeString();
            return Palette[0].ToColorSchemeString();
        }

        protected string GetFormatForBoolean(bool value) => Palette.Length <= 2 ? value ? Palette.First().ToColorSchemeString() : Palette.Last().ToColorSchemeString() : value ? Palette[1].ToColorSchemeString() : Palette[2].ToColorSchemeString();
    }
}