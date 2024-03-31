using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class XPrinter : Printer, IXPrinter
    {
        /// <summary>
        /// XFormat delegate based on <see cref="X.Format(FormattableString)"/> is used to convert <see cref="FormattableString"/> to string in PowerConsole.PrintF(..) methods
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by <see cref="X.TextProcessor"/>
        /// </summary>
        public virtual Func<FormattableString, string> XFormat { get; set; } = x => X.Format(x, X.FormatPreprocessor);

        public XPrinter(TextWriter textWriter, ColorMode mode) : base(textWriter, mode)
        {
        }

        public void PrintF(FormattableString str, PrintOptions options)
        {
            string message;

            if (options.ColorPalette != null)
            {
                var str2 = str.Colorize(options.ColorPalette.Colors);
                message = XFormatInternal(str2);
            }
            else
            {
                message = XFormatInternal(str);
            }
            var text = options.TimeFormat == null ? message : this.AddTimestamp(message, SystemTime, options.TimeFormat);
            Writer.Write(text, options);
        }

        public void SetCustomFormatter(Func<FormattableString, string> formatter, bool printF = true)
        {
            if (printF)
                XFormat = formatter;
            else
                Format = formatter;
        }

        internal string XFormatInternal(FormattableString str)
        {
            //1. x-formatted string might contain color formatting: {text:-Color --BackgroundColor}
            //Table/Square/Header tag formatters are not implemented yet
            //but the idea (not implemented yet) to parse tags: 
            //<table><header>Column1|Col2|EmptyColumn</header><body><cell>{text:-Red}</cell>... </table> - prints table
            //<square size='5x5'>abc</square>  - prints text inside of square 5x5
            //<h1>title</h1>  - prints title with corresponding to h1 font-size and font-weight default setup
            //<h2>title</h2>  - prints title with corresponding to h2 font-size and font-weight default setup

            //text expressions also could be done with tags:
            //<expr>description:value</expr>
            //<table><thead>Column1|Column2</thead><tr><td>cell1</td><td>cell2</td></tr></body></table>

            //i.e. we need here somehow process line by line highlight etc.
            //XFormattedStringFactory.Create(formattedString) => ColorFormattedString or TableFormatted or something else
            //Print(new ColorMarkupString(formattedString), color, endLine);
            return XFormat?.Invoke(str) ?? str.ToString(CultureInfo.CurrentCulture);
        }

        //public void PrintF(FormattableString str, bool endLine = true)
        //{
        //    var text = XFormatInternal(str);
        //    Writer.Write(text, endLine);
        //}

        //public void PrintF(FormattableString str, bool endLine, bool? containsCTags)
        //{
        //    var text = XFormatInternal(str);
        //    Writer.Write(text, endLine, containsCTags);
        //}

        //public void PrintF(FormattableString str, bool endLine, bool? containsCTags, ConsoleColor? color)
        //{
        //    var text = XFormatInternal(str);
        //    Writer.Write(text, endLine, containsCTags, color);
        //}

        //public void PrintF(FormattableString str, bool endLine, bool? containsCTags, Colors colors)
        //{
        //    var text = XFormatInternal(str);
        //    Writer.Write(text, endLine, containsCTags, colors);
        //}

        //public void PrintF(FormattableString str, bool endLine, bool? containsCTags, ColorScheme scheme)
        //{
        //    var text = XFormatInternal(str);
        //    Writer.Write(text, endLine, containsCTags, scheme);
        //}
      }
}