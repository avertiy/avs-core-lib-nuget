using System;
using System.Globalization;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrintF
    {
        void PrintF(FormattableString str, bool endLine);
        void PrintF(FormattableString str, ConsoleColor color, bool endLine);
        void PrintF(FormattableString str, ColorScheme scheme, bool endLine);
    }

    public class PrintFPrinter : BasePrinter, IPrintF
    {
        /// <summary>
        /// Format delegate is used to convert <see cref="FormattableString"/> to string
        /// used by <see cref="PrintF(System.FormattableString,bool)"/>
        /// </summary>
        public static Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);
        public virtual void PrintF(FormattableString str, bool endLine)
        {
            var formattedString = Format(str);
            Print(formattedString, endLine);
        }

        public virtual void PrintF(FormattableString str, ConsoleColor color, bool endLine)
        {
            var formattedString = Format(str);
            Print(formattedString, color, endLine);
        }

        public virtual void PrintF(FormattableString str, ColorScheme scheme, bool endLine)
        {
            var formattedString = Format(str);
            Print(formattedString, scheme, endLine);
        }

        public PrintFPrinter(IConsoleWriter writer) : base(writer)
        {
        }
    }
}