using System;
using System.Linq;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.FormatPreprocessors;

namespace AVS.CoreLib.PowerConsole.Printers
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
                    return this.StringColor.ToColorSchemeString();
                Type type = argument.GetType();
                if (type.IsEnum)
                    return this.GetFormatForEnum(type, (int)argument);
                if (type.IsPrimitive)
                    return this.GetFormatForPrimitive(argument);
            }
            return format;
        }

        protected string GetFormatForPrimitive(object argument)
        {
            if (argument is bool flag)
                return this.GetFormatForBoolean(flag);
            var num = Compare(argument, 0);
            if (num == 0)
                return this.Palette[0].ToColorSchemeString();
            return num < 0 ? this.Palette[1].ToColorSchemeString() : (this.Palette.Length > 2 ? this.Palette[2].ToColorSchemeString() : this.Palette.Last<ConsoleColor>().ToColorSchemeString());
        }

        private static int Compare(object obj, int n)
        {
            if (obj is int num)
                return num.CompareTo(n);
            if (obj is long l)
                return l.CompareTo((long)n);
            if (obj is double d)
                return d.CompareTo((double)n);
            if (obj is decimal dec)
                return dec.CompareTo((decimal)n);
            return obj is short s ? s.CompareTo((object)n) : 0;
        }

        protected virtual string GetFormatForEnum(Type enumType, int value)
        {
            Array values = Enum.GetValues(enumType);
            for (int index = 0; index < values.Length; ++index)
            {
                if ((int)values.GetValue(index) == value)
                    return index <= this.Palette.Length ? this.Palette[index].ToColorSchemeString() : this.Palette[1].ToColorSchemeString();
            }
            return this.Palette[0].ToColorSchemeString();
        }

        protected string GetFormatForBoolean(bool value) => this.Palette.Length <= 2 ? (value ? this.Palette.First<ConsoleColor>().ToColorSchemeString() : this.Palette.Last<ConsoleColor>().ToColorSchemeString()) : (value ? this.Palette[1].ToColorSchemeString() : this.Palette[2].ToColorSchemeString());
    }
}