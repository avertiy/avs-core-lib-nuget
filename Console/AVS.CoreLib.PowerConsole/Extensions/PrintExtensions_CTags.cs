using System;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public static partial class PrintExtensions
    {
        public static void Print(this IPrinter printer, string message, bool endLine, bool containsCTags)
        {
            if (!containsCTags)
                printer.Print(message, endLine);

            var text = printer.TagProcessor.Process(message);
            printer.Print(text, endLine);
        }

        public static void Print(this IPrinter printer, string message, ConsoleColor? color, bool endLine, bool containsCTags)
        {
            if (!containsCTags)
                printer.Print(message, color, endLine);

            var text = printer.TagProcessor.Process(message);
            printer.Print(text, color, endLine);
        }

        public static void Print(this IPrinter printer, FormattableString str, bool endLine, bool containsCTags)
        {
            var formattedString = printer.Format(str);

            if (!containsCTags)
                printer.Print(formattedString, endLine);

            var text = printer.TagProcessor.Process(formattedString);
            printer.Print(text, endLine);
        }
    }
}