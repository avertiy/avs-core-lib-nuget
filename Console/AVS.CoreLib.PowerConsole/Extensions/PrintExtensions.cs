using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.FormatProcessors;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using IPrinter = AVS.CoreLib.PowerConsole.Printers.IPowerConsolePrinter;
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
                printer.PrintLine();
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
                printer.PrintLine();
        }

        public static void Print(this IPrinter printer, IEnumerable<ColorString> messages)
        {
            foreach (var coloredText in messages)
                printer.Print(coloredText.Text, coloredText.Color, false);
        }
    }
}