using System;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class CTagsPrinter : FormatPrinter
    {
        public TagProcessor TagProcessor { get; set; }

        public CTagsPrinter(IConsoleWriter writer) : base(writer)
        {
            var tagProcessor = new CompositeTagProcessor();
            tagProcessor.AddTagProcessor(new CTagProcessor());
            tagProcessor.AddTagProcessor(new RgbTagProcessor());
            TagProcessor = tagProcessor;
        }

        public CTagsPrinter(IConsoleWriter writer, TagProcessor tagProcessor) : base(writer)
        {
            TagProcessor = tagProcessor;
        }

        public virtual void Print(string message, bool endLine, bool containsCTags)
        {
            if(!containsCTags)
                base.Print(message, endLine);

            var text = TagProcessor.Process(message);
            base.Print(message, endLine);
        }

        public virtual void Print(string message, ConsoleColor? color, bool endLine, bool containsCTags)
        {
            if (!containsCTags)
                base.Print(message, color, endLine);

            var text = TagProcessor.Process(message);
            base.Print(message, color, endLine);
        }

        public virtual void Print(FormattableString str, bool endLine, bool containsCTags)
        {
            var formattedString = Format(str);
            if (!containsCTags)
                base.Print(formattedString, endLine);

            var text = TagProcessor.Process(formattedString);
            Writer.Write(text, endLine);
        }

        protected void PrintCTags(string message, bool endLine)
        {
            var text = TagProcessor.Process(message);
            base.Print(message, endLine);
        }
    }

    public class ColorPrinter : CTagsPrinter
    {
        public IColorFormatProcessor FormatProcessor { get; set; }
        public ColorPrinter(IConsoleWriter writer) : base(writer)
        {
            FormatProcessor = new ColorFormatProcessor();
        }

        public ColorPrinter(IConsoleWriter writer, IColorFormatProcessor colorFormatProcessor, TagProcessor tagProcessor) : base(writer, tagProcessor)
        {
            FormatProcessor = colorFormatProcessor;
        }


        /// <summary>
        /// colorize arguments of <see cref="FormattableString"/> kind of auto-highlight feature in color formatter for console logging
        /// </summary>
        public virtual void Print(FormattableString str, ColorPalette palette, bool endLine)
        {
            //to-do combine or make common feature as it seems duplicate color formatter args auto-highlight
            var arguments = str.GetArguments();
            var str2 = new FormattableString2(str.Format, arguments);
            this.FormatProcessor.Palette = palette;
            var colorMarkupStr = new ColorMarkupString(str2.ToString(X.FormatProvider, this.FormatProcessor, X.TextProcessor));

            Print(colorMarkupStr, endLine);
        }

        public void Print(ColorMarkupString str, bool endLine)
        {
            //to-do..
            //approach needs to be changed to AnsiCodes..

            var currentScheme = ColorScheme.GetCurrentScheme();

            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                {
                    //ColorScheme.ApplyScheme(currentScheme);
                    Writer.Write(plainText, currentScheme, false);
                }

                // if scheme valid apply it
                if (!string.IsNullOrEmpty(coloredText) && ColorSchemeHelper.TryParse(colorScheme, out var scheme))
                {
                    Writer.Write(coloredText, scheme, false);
                }
            }

            if (endLine)
                Writer.WriteLine();
            //ColorScheme.ApplyScheme(currentScheme);
        }
    }
}