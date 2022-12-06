using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.FormatProviders;

namespace AVS.CoreLib.PowerConsole
{
    /// <summary>
    /// PowerConsole provides various extension methods to print to console
    /// supports coloring messages via ansi codes <see cref="AnsiCodes"/>,
    /// color tags <see cref="CTagProcessor"/>,
    /// extra formatting <see cref="XFormatProvider"/>.
    /// </summary>
    /// <remarks>If you need more rich and extensive console frameworks check out links below </remarks>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        private static IPrinter? _printer;
        /// <summary>
        /// Printed messages are forwarded to <see cref="AVS.CoreLib.PowerConsole.Printers.Printer"/>
        /// Printer provides all printing and writing features e.g. print array, processing color tags etc.
        /// </summary>
        public static IPrinter Printer
        {
            get => _printer ??= new Printer(System.Console.Out);
            set => _printer = value;
        }

        public static void Print(string str, bool endLine = true, bool colorTags = true)
        {
            Printer.Print(str, endLine, colorTags);
        }

        public static void Print(string str, CTag tag, bool endLine = true)
        {
            Printer.Print(str.WrapInTag(tag) , endLine, true);
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
        public static void Print(FormattableString str, bool endLine = true, bool colorTags = true)
        {
            Printer.Print(str, endLine, colorTags);
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
