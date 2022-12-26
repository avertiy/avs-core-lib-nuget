using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.PowerConsole.Writers;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.PowerConsole.Printers
{
    //note PrintF methods by default should support ctags and color markup in text e.g. <Red>Hello {world:--bgYellow}</Red> 
    public class XPrinter : ColorPrinter, IXPrinter
    {
        /// <summary>
        /// XFormat delegate based on <see cref="X.Format(FormattableString)"/> is used to convert <see cref="FormattableString"/> to string in PowerConsole.PrintF(..) methods
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by <see cref="X.TextProcessor"/>
        /// </summary>
        public virtual Func<FormattableString, string> XFormat { get; set; } = x => X.Format(x, X.FormatPreprocessor);

        public XPrinter(TextWriter textWriter, ColorMode mode) : base(textWriter, mode)
        {
        }

        protected string XFormatInternal(FormattableString str)
        {
            return XFormat?.Invoke(str) ?? str.ToString(CultureInfo.CurrentCulture);
        }
        public void PrintF(FormattableString str, bool endLine, bool containsCTags = true)
        {
            var text = XFormatInternal(str);
            Writer.Write(text, endLine, containsCTags);
        }

        public void PrintF(FormattableString str, bool endLine, ConsoleColor? color, bool containsCTags = true)
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


            var text = XFormatInternal(str);

            if (color.HasValue)
                Writer.Write(text, endLine, containsCTags, color.Value);
            else
                Writer.Write(text, endLine, containsCTags);
        }

        public void PrintF(FormattableString str, bool endLine, Colors colors, bool containsCTags = true)
        {
            var text = XFormatInternal(str);
            Writer.Write(text, endLine, containsCTags, colors);
        }

        public void PrintF(FormattableString str, bool endLine, ColorScheme scheme, bool containsCTags = true)
        {
            var text = XFormatInternal(str);
            Writer.Write(text, endLine, containsCTags, scheme);
        }
    }
}