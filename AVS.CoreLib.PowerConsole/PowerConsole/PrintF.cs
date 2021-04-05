using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// PrintF functionality might be rich & powerful with custom a formatting <see cref="IFormatProvider"/>
        /// see also AVS.CoreLib.Text.X.Format 
        /// </summary>
        /// <remarks>
        /// AVS.CoreLib.Text namespace contains X.Format function a power analog of string.Format
        /// that uses XFormatProvider in combination with ITextProcessor  
        /// </remarks>
        public static Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);

        /// <summary>
        /// Using color argument create a <see cref="ColorScheme"/> and apply it
        /// Format string calling <see cref="Format"/> delegate
        /// Create a <see cref="ColorFormattedString"/> and print it
        /// Restore initial color(s)
        /// </summary>
        /// <remarks>
        /// in case you use X.Format
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// </remarks>
        public static void PrintF(FormattableString str, ConsoleColor color, bool endLine = true)
        {
            var formattedString = Format(str);
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

            PrintF(new ColorFormattedString(formattedString), endLine);
            ColorSchemeReset();
        }

        /// <summary>
        /// Format string calling <see cref="Format"/> delegate
        /// than create a <see cref="ColorFormattedString"/> and print it
        /// </summary>
        /// <remarks>
        /// in case you use X.Format
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s)
        /// </remarks>
        public static void PrintF(FormattableString str, bool endLine = true)
        {
            var formattedString = Format(str);
            PrintF(new ColorFormattedString(formattedString), endLine);
        }

        public static void PrintF(int posX, int posY, FormattableString str, bool endLine = true)
        {
            var formattedString = Format(str);
            var rows = Regex.Matches(formattedString, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            PrintF(new ColorFormattedString(formattedString), endLine);
        }

        public static void PrintF(int posX, int posY, string str, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            PrintF(new ColorFormattedString(str), endLine);
        }

        public static void PrintF(int posX, int posY, string str, ConsoleColor color, bool endLine = true)
        {
            var rows = Regex.Matches(str, Environment.NewLine).Count;
            ClearRegion(posX, posY, rows);
            var scheme = new ColorScheme(color);
            ApplyColorScheme(scheme);
            PrintF(new ColorFormattedString(str), endLine);
            ColorSchemeReset();
            ClearLine();
        }

        /// <summary>
        /// Print color formatted string (does a console color magic) 
        /// e.g. $"{1:-Red} {"abc":--DarkGray}"
        /// where 
        /// -Red makes font color Red
        /// --DarkGray makes bg color DarkGray
        /// </summary>
        internal static void PrintF(ColorFormattedString str, bool endLine = true)
        {
            var scheme = ColorScheme.Current;

            foreach ((string, ColorScheme, string) tuple in str)
            {
                if (!string.IsNullOrEmpty(tuple.Item1))
                {
                    ColorScheme.ApplyScheme(scheme);
                    Write(tuple.Item1);
                }
                if (tuple.Item2 != null)
                    ColorScheme.ApplyScheme(tuple.Item2);

                if (!string.IsNullOrEmpty(tuple.Item3))
                    Write(tuple.Item3);
            }

            if (endLine)
                WriteLine();
            ColorScheme.ApplyScheme(scheme);
        }
    }
}
