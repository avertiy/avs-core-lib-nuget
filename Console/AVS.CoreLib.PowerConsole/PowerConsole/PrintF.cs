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
        public static void PrintF(FormattableString str, PrintOptions? options = null)
        {
            Printer.PrintF(str, options ?? DefaultOptions);
        }

        public static void PrintF(FormattableString str, Action<PrintOptions> configureOptions)
        {
            var options = DefaultOptions.Clone();
            configureOptions(options);
            Printer.Print(str, options);
        }

        /// <summary>
        /// format and print string auto-highlighting arguments picking colors from color palette provided in <see cref="MultiColorPrintOptions"/>
        /// </summary>
        public static void PrintF(FormattableString str, MultiColorPrintOptions options)
        {
            Printer.Print(str, options);
        }

        public static void PrintF(FormattableString str, params ConsoleColor[] colors)
        {
            Printer.Print(str, new MultiColorPrintOptions(colors));
        }

        public static void PrintF(int posX, int posY, FormattableString str, PrintOptions? options = null)
        {
            var text = str.ToString();
            var rows = Regex.Matches(text, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            Printer.Print(new ColorMarkupString(text), options?? DefaultOptions);
            ClearLine();
        }
    }
}
