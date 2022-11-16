using System;
using System.Globalization;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class BasePrinter
    {
        public IConsoleWriter Writer { get; }

        public BasePrinter(IConsoleWriter writer)
        {
            Writer = writer;
        }

        public virtual void Print(string str, bool endLine)
        {
            Writer.Write(str, endLine);
        }

        public virtual void Print(string str, ConsoleColor? color, bool endLine)
        {
            if(color == null)
                Writer.Write(str, endLine);
            else
                Writer.Write(str, color.Value, endLine);
        }

        public virtual void Print(string str, ConsoleColor color, bool endLine)
        {
            Writer.Write(str, color, endLine);
        }

        public virtual void Print(string str, Colors colors, bool endLine)
        {
            Writer.Write(str, colors, endLine);
        }
        public virtual void Print(string str, ColorScheme scheme, bool endLine)
        {
            Writer.Write(str, scheme, endLine);
        }
    }

    public class FormatPrinter : BasePrinter
    {
        /// <summary>
        /// Format delegate is used to convert <see cref="FormattableString"/> to string
        /// used by <see cref="Print(System.FormattableString, bool)"/>
        /// </summary>
        public static Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);

        public FormatPrinter(IConsoleWriter writer) : base(writer)
        {
        }

        public virtual void Print(FormattableString str, bool endLine)
        {
            var formattedString = Format(str);
            Writer.Write(formattedString, endLine);
        }

        public virtual void Print(FormattableString str, ConsoleColor color, bool endLine)
        {
            var formattedString = Format(str);
            Writer.Write(formattedString, color, endLine);
        }

        public virtual void Print(FormattableString str, ColorScheme scheme, bool endLine)
        {
            var formattedString = Format(str);
            Writer.Write(formattedString, scheme, endLine);
        }
    }
}