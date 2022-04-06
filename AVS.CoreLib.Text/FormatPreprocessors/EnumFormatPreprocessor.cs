using System;
using AVS.CoreLib.Text.Extensions;

namespace AVS.CoreLib.Text.FormatPreprocessors
{
    /// <summary>
    /// That's an example of the idea with format preprocessors
    /// Enum format preprocessor highlight enum values
    /// values matches to default enum value will be gray otherwise blue
    /// </summary>
    public class EnumFormatPreprocessor : IFormatPreprocessor
    {
        public ConsoleColor DefaultColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor Color { get; set; } = ConsoleColor.Cyan;
        protected virtual string GetFormat(Type enumType, int value)
        {
            var values = Enum.GetValues(enumType);
            if (value == (int)values.GetValue(0))
            {
                return DefaultColor.ToColorSchemeString();
            }

            return Color.ToColorSchemeString();
        }

        /// <inheritdoc />
        public string Process(string format, object argument)
        {
            if (string.IsNullOrEmpty(format))
            {
                var type = argument.GetType();
                if (type.IsEnum)
                {
                    return GetFormat(type, (int)argument);
                }
            }

            return format;
        }
    }
}