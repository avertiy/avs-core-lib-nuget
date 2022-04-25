using System;
using System.Text.RegularExpressions;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// XFormat delegate based on <see cref="X.Format(FormattableString)"/> is used to convert <see cref="FormattableString"/> to string in PowerConsole.PrintF(..) methods
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by <see cref="X.TextProcessor"/>
        /// </summary>
        public static Func<FormattableString, string> XFormat { get; set; } = x => X.Format(x, X.FormatPreprocessor);

        /// <summary>
        /// An input string formatted by <see cref="XFormat"/> delegate
        /// <see cref="ColorMarkupString"/> formatting is supported
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="color">console foreground color</param>
        /// <param name="endLine">end line or not</param>
        /// <remarks>
        /// After the printing is done the <see cref="ColorScheme"/> is reset <see cref="ColorSchemeReset"/>
        /// </remarks>
        public static void PrintF(FormattableString str, ConsoleColor color, bool endLine = true)
        {
            var formattedString = XFormat(str);
            var scheme = new ColorScheme(color);
            ApplyColorScheme(scheme);
            //1. x-formatted string might contain color formatting: $$text:--Color$
            //Table/Square/Header tag formatters are not implemented yet
            //but the idea is to parse tags: 
            //<table><header>Column1|Col2|EmptyColumn</header><body><cell>$$text:-Red$</cell>... </table> - prints table
            //<square size='5x5'>abc</square>  - prints text inside of square 5x5
            //<h1>title</h1>  - prints title with corresponding to h1 font-size and font-weight default setup
            //<h2>title</h2>  - prints title with corresponding to h2 font-size and font-weight default setup

            //text expressions also could be done with tags:
            //<expr>description:value</expr>
            //<table><thead>Column1|Column2</thead><tr><td>cell1</td><td>cell2</td></tr></body></table>

            //i.e. we need here somehow process line by line highlight etc.
            //XFormattedStringFactory.Create(formattedString) => ColorFormattedString or TableFormatted or something else

            Print(new ColorMarkupString(formattedString), endLine);
            ColorSchemeReset();
        }

        /// <summary>
        /// Format string calling <see cref="Format"/> delegate
        /// than create a <see cref="ColorMarkupString"/> and print it
        /// </summary>
        /// <remarks>
        /// in case you use X.Format
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// </remarks>
        public static void PrintF(FormattableString str, bool endLine = true)
        {
            var formattedString = XFormat(str);
            
            Print(new ColorMarkupString(formattedString), endLine);
        }

        public static void PrintF(int posX, int posY, FormattableString str, bool endLine = true)
        {
            var formattedString = XFormat(str);
            var rows = Regex.Matches(formattedString, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            Print(new ColorMarkupString(formattedString), endLine);
        }

        public static void PrintF(int posX, int posY, string str, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            Print(new ColorMarkupString(str), endLine);
        }

        public static void PrintF(int posX, int posY, string str, ConsoleColor color, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            var scheme = new ColorScheme(color);
            ApplyColorScheme(scheme);
            Print(new ColorMarkupString(str), endLine);
            ColorSchemeReset();
            ClearLine();
        }
    }
}
