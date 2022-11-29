using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class PrintFExtensions
    {
        /// <summary>
        /// An input string formatted by <see cref="IPrinter.XFormat"/> delegate
        /// <see cref="ColorMarkupString"/> formatting is supported
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="str">input string</param>
        /// <param name="color">console foreground color</param>
        /// <param name="endLine">end line or not</param>
        public static void PrintF(this IPrinter printer, FormattableString str, ConsoleColor? color, bool endLine = true)
        {
            //1. x-formatted string might contain color formatting: {text:-Color --BackgroundColor}
            //Table/Square/Header tag formatters are not implemented yet
            //but the idea is to parse tags: 
            //<table><header>Column1|Col2|EmptyColumn</header><body><cell>{text:-Red}</cell>... </table> - prints table
            //<square size='5x5'>abc</square>  - prints text inside of square 5x5
            //<h1>title</h1>  - prints title with corresponding to h1 font-size and font-weight default setup
            //<h2>title</h2>  - prints title with corresponding to h2 font-size and font-weight default setup

            //text expressions also could be done with tags:
            //<expr>description:value</expr>
            //<table><thead>Column1|Column2</thead><tr><td>cell1</td><td>cell2</td></tr></body></table>

            //i.e. we need here somehow process line by line highlight etc.
            //XFormattedStringFactory.Create(formattedString) => ColorFormattedString or TableFormatted or something else

            var formattedString = printer.XFormat(str);
            printer.Print(new ColorMarkupString(formattedString), color, endLine);
        }

        /// <summary>
        /// Format input string with <see cref="XFormatProvider"/>,
        /// then creates <see cref="ColorMarkupString"/> and print it
        /// </summary>
        /// <remarks>
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// <see cref="TextExpressionProcessor"/>
        /// </remarks>
        public static void PrintF(this IPrinter printer, FormattableString str, bool endLine = true)
        {
            var formattedString = printer.XFormat(str);
            printer.Print(new ColorMarkupString(formattedString), endLine);
        }
    }
}