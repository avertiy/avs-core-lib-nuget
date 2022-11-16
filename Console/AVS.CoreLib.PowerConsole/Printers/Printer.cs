using System;
using System.Collections.Generic;
using System.Globalization;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    using Console = System.Console;

    public interface IPrinter
    {
        public IConsoleWriter Writer { get; }
        void Print(string str, bool endLine);
        void Print(string str, ConsoleColor? color, bool endLine);
        void Print(string str, ConsoleColor color, bool endLine);
        void Print(string str, Colors colors, bool endLine);
        void Print(string str, ColorScheme scheme, bool endLine);
        void Print(FormattableString str, bool endLine);
        void Print(FormattableString str, ConsoleColor color, bool endLine);
        void Print(FormattableString str, ColorScheme scheme, bool endLine);
        void Print(string message, bool endLine, bool containsCTags);
        void Print(string message, ConsoleColor? color, bool endLine, bool containsCTags);
        void Print(FormattableString str, bool endLine, bool containsCTags);

        /// <summary>
        /// colorize arguments of <see cref="FormattableString"/> kind of auto-highlight feature in color formatter for console logging
        /// </summary>
        void Print(FormattableString str, ColorPalette palette, bool endLine);
        void Print(ColorMarkupString str, bool endLine);
    }

    public class Printer : ColorPrinter, IPrinter
    {
        public Printer(IConsoleWriter writer) : base(writer)
        {
        }

       
    }
}