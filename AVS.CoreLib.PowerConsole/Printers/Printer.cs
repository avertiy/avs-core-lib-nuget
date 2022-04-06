using System;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.FormatPreprocessors;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class Printer : IPrinter
    {
        public void Print(FormattableString str, ColorPalette palette, bool endLine = true)
        {
            var args = str.GetArguments();
            var str2 = new FormattableString2(str.Format, args);
            IFormatPreprocessor formatPreprocessor = null;
            if (palette.Colors.Length == 2)
            {
                formatPreprocessor = new EnumFormatPreprocessor() { DefaultColor =  palette[0], Color = palette[1]};
            }

            var formattedString = str2.ToString(X.FormatProvider, formatPreprocessor, X.TextProcessor);
            var colorMarkupString = new ColorMarkupString(formattedString);
            PowerConsole.Print(colorMarkupString, endLine);
        }
    }
}