using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public static partial class PrintExtensions
    {
        public static void Print(this IPrinter printer, FormattableString str, bool endLine)
        {
            var formattedString = printer.Format(str);
            printer.Print(formattedString, endLine);
        }

        public static void Print(this IPrinter printer, FormattableString str, ConsoleColor? color, bool endLine)
        {
            var formattedString = printer.Format(str);

            if (color == null)
                printer.Print(formattedString, endLine);
            else
                printer.Print(formattedString, color.Value, endLine);
        }

        public static void Print(this IPrinter printer, FormattableString str, ColorScheme scheme, bool endLine)
        {
            var formattedString = printer.Format(str);
            printer.Print(formattedString, scheme, endLine);
        }
    }
}