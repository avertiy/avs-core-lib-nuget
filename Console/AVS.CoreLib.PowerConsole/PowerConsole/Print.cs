using System;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{

    /// <summary>
    /// PowerConsole represents simple extensions over standard .NET Console functionality
    /// If you need more rich and extensive console frameworks check out links below  
    /// </summary>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        private static IPrinter _printer;
        public static IPrinter Printer
        {
            get => _printer ??= new Printer(new ConsoleWriter());
            set => _printer = value;
        }

        public static void Print(string str, bool endLine = true)
        {
            Printer.Print(str, endLine);
        }

        public static void Print(string str, ConsoleColor color, bool endLine = true)
        {
            Printer.Print(str, color, endLine);
        }

        public static void Print(string str, ColorScheme scheme, bool endLine = true)
        {
            Printer.Print(str, scheme, endLine);
        }


        /// <summary>
        /// Format string by <see cref="Printers.Printer.Format"/> delegate and print it
        /// </summary>
        public static void Print(FormattableString str, bool endLine = true)
        {
            Printer.Print(str, endLine);
        }

        /// <summary>
        /// Format string by <see cref="Printers.Printer.Format"/> delegate and print it
        /// </summary>
        public static void Print(FormattableString str, ConsoleColor color, bool endLine = true)
        {
            Printer.Print(str, color, endLine);
        }

        public static void Print(FormattableString str, ColorPalette palette, bool endLine = true)
        {
            Printer.Print(str, palette, endLine);
        }

        public static void Print(FormattableString str, ConsoleColor[] colors, bool endLine = true)
        {
            Printer.Print(str, colors, endLine);
        }
    }
}
