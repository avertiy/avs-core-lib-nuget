using System;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class Printer : IPrinter
    {
        public IColorFormatProcessor FormatProcessor { get; set; } = new ColorFormatProcessor();
        public void Print(FormattableString str, ColorPalette palette, bool endLine = true)
        {
            var arguments = str.GetArguments();
            var str2 = new FormattableString2(str.Format, arguments);
            this.FormatProcessor.Palette = palette;
            PowerConsole.Print(new ColorMarkupString(str2.ToString(X.FormatProvider, this.FormatProcessor, X.TextProcessor)), endLine);
        }
    }
}