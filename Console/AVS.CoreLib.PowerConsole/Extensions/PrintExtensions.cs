using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public static partial class PrintExtensions
    {
        public static void Print(this IPrinter printer, string str, Colors colors, bool endLine)
        {
            var coloredStr = colors.Colorize(str);
            printer.Print(coloredStr, endLine);
        }

        public static void Print(this IPrinter printer, string str, ColorScheme scheme, bool endLine)
        {
            var coloredStr = scheme.Colorize(str);
            printer.Print(coloredStr, endLine);
        }

        /// <summary>
        /// colorize arguments of <see cref="FormattableString"/> kind of auto-highlight feature in color formatter for console logging
        /// </summary>
        public static void Print(this IPrinter printer, FormattableString str, ColorPalette palette, bool endLine)
        {
            //to-do combine or make common feature as it seems duplicate color formatter args auto-highlight
            var arguments = str.GetArguments();
            var str2 = new FormattableString2(str.Format, arguments);
            printer.FormatProcessor.Palette = palette;
            var colorMarkupStr = new ColorMarkupString(str2.ToString(X.FormatProvider, printer.FormatProcessor, X.TextProcessor));

            printer.Print(colorMarkupStr, endLine);
        }

        public static void Print(this IPrinter printer, ColorMarkupString str, bool endLine)
        {
            var currentScheme = ColorScheme.GetCurrentScheme();

            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                    printer.Print(plainText, currentScheme, false);

                // if scheme valid apply it
                if (!string.IsNullOrEmpty(coloredText) && ColorSchemeHelper.TryParse(colorScheme, out var scheme))
                    printer.Print(coloredText, scheme, false);
            }

            if (endLine)
                printer.WriteLine();
        }

        public static void Print(this IPrinter printer, ColorMarkupString str, ConsoleColor? color, bool endLine)
        {
            var colors = new Colors(color, null);

            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                    printer.Print(plainText, colors, false);

                // if scheme valid apply it
                if (!string.IsNullOrEmpty(coloredText) && ColorSchemeHelper.TryParse(colorScheme, out var scheme))
                    printer.Print(coloredText, scheme, false);
            }

            if (endLine)
                printer.WriteLine();
        }

        public static void Print(this IPrinter printer, IEnumerable<ColorString> messages)
        {
            foreach (var coloredText in messages)
                printer.Print(coloredText.Text, coloredText.Color, false);
        }
    }
}