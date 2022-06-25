using System;
using System.Linq;

namespace AVS.CoreLib.Text.Formatters.ColorMarkup
{
    /// <summary>
    /// format argument keeping color formatting i.e "{DateTime.Now:-Red --Yellow d}" => "{01/01/2020:-Red --Yellow}"
    /// it is assumed color format is at the begging
    /// </summary>
    public class ColorMarkupFormatter : CustomFormatter
    {
        /// <inheritdoc />
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            //format:-Color or --Color e.g -Red or --Red 
            var parts = format.Split(' ');
            var colors = parts.Where(x => x.StartsWith("-") && x.Length > 2).ToArray();

            // no color formats
            if (colors.Length == 0)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            var len = colors[0].Length;
            var colorScheme = colors[0];
            var nextFormat = "";
            if (colors.Length > 1)
            {
                len += colors[1].Length + 1;
                colorScheme = format.Substring(0, len);
            }

            nextFormat = format.Substring(len).TrimStart();
            var argStr = DefaultFormat(nextFormat, arg, formatProvider);

            if (argStr.Length == 0)
            {
                return argStr;
            }

            var str = $"{{{argStr}:{colorScheme}}}";
            return str;
        }

        /// <inheritdoc />
        protected override bool Match(string format)
        {
            return format.StartsWith("-");
        }
    }
}