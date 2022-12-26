using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class PowerConsolePrinter : XPrinter, IPowerConsolePrinter
    {
        public PowerConsolePrinter(TextWriter writer, ColorMode mode = ColorMode.AnsiCodes) : base(writer, mode)
        {
        }

        public void SetCustomFormatter(Func<FormattableString, string> formatter, bool printF = true)
        {
            if (printF)
                XFormat = formatter;
            else
                Format = formatter;
        }

        /// <summary>
        /// colorize arguments of <see cref="FormattableString"/> kind of auto-highlight feature in color formatter for console logging
        /// </summary>
        public void Print(FormattableString str, ColorPalette palette, bool endLine)
        {
            throw new NotImplementedException();
            //var arguments = str.GetArguments();
            //var str2 = new FormattableString2(str.Format, arguments);
            //var preprocessor = FormatPreprocessor;

            //if (preprocessor is ColorFormatPreprocessor colorFormatPreprocessor)
            //    colorFormatPreprocessor.Palette = palette;

            //var formattedString = str2.ToString(X.FormatProvider, printer.FormatPreprocessor, X.TextProcessor);
            //var colorMarkupStr = new ColorMarkupString(formattedString);

            //printer.Print(colorMarkupStr, endLine);
        }

        public void Print(string str, CTag tag, bool endLine = true)
        {
            Writer.Write(str, endLine, false, tag);
        }
    }
}