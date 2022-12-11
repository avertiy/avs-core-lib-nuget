using System;
using System.Text.RegularExpressions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// Format string with X.Format <see cref="XFormatProvider"/> 
        /// </summary>
        /// <remarks>
        /// in case you use X.Format
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// </remarks>
        public static void PrintF(FormattableString str, bool endLine = true)
        {
            Printer.PrintF(str, endLine);
        }

        /// <summary>
        /// An input string formatted by <see cref="XFormat"/> delegate
        /// <see cref="ColorMarkupString"/> formatting is supported
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="color">console foreground color</param>
        /// <param name="endLine">end line or not</param>
        /// <param name="containsTags">color tags <see cref="CTag"/></param>
        /// <remarks>
        /// After the printing is done the <see cref="ColorScheme"/> is reset <see cref="ColorSchemeReset"/>
        /// </remarks>
        public static void PrintF(FormattableString str, ConsoleColor color, bool endLine = true, bool containsTags= true)
        {
            Printer.PrintF(str, endLine, color, containsTags);
        }

        //public static void PrintF(int posX, int posY, FormattableString str, bool endLine = true)
        //{
        //    var formattedString = Printer.XFormat(str);
        //    var rows = Regex.Matches(formattedString, Environment.NewLine).Count;
        //    ClearRegion(posX, posY, rows);
        //    Printer.Print(new ColorMarkupString(formattedString), endLine);
        //}

        public static void PrintF(int posX, int posY, string str, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            Printer.Print(new ColorMarkupString(str), endLine);
        }

        public static void PrintF(int posX, int posY, string str, ConsoleColor color, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            Printer.Print(new ColorMarkupString(str), color, endLine);
            ClearLine();
        }
    }
}
