using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AVS.CoreLib.Abstractions.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.FormatProcessors;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class Printer : PrinterBase, IPrinter
    {
        public Printer(TextWriter writer, bool cTags = true) : base(writer)
        {
            if (cTags)
            {
                var tagProcessor = new CompositeTagProcessor();
                tagProcessor.AddTagProcessor(new CTagProcessor());
                tagProcessor.AddTagProcessor(new RgbTagProcessor());
                TagProcessor = tagProcessor;
            }
            else
            {
                TagProcessor = new DummyTagProcessor();
            }
            FormatPreprocessor = new ColorFormatPreprocessor();
        }

        public virtual TagProcessor TagProcessor { get; }
        public virtual IFormatPreprocessor FormatPreprocessor { get; }
        public virtual Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);
        /// <summary>
        /// XFormat delegate based on <see cref="X.Format(FormattableString)"/> is used to convert <see cref="FormattableString"/> to string in PowerConsole.PrintF(..) methods
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by <see cref="X.TextProcessor"/>
        /// </summary>
        public virtual Func<FormattableString, string> XFormat { get; set; } = x => X.Format(x, X.FormatPreprocessor);
        public virtual void Print(string str, bool endLine)
        {
            Write(str, endLine);
        }

        public virtual void Print(string str, ConsoleColor? color, bool endLine)
        {
            if (color == null)
                Write(str, endLine);
            else
            {
                Write(str, color.Value, endLine);
            }
        }

        public virtual void Write(string str, ConsoleColor color, bool endLine)
        {
            var coloredStr = $"{AnsiCodes.Color(color)}{str}{AnsiCodes.RESET}";
            Writer.Write(coloredStr);
            if (endLine)
            {
                Writer.WriteLine();
                NewLineFlag = true;
            }
            else
                NewLineFlag = str.EndsWith('\n');
        }
    }

    public class PlainPrinter : PrinterBase, IPrinter
    {
        public PlainPrinter(TextWriter writer) : base(writer)
        {
            TagProcessor = new StripTagsProcessor();
            FormatPreprocessor = new PlainFormatPreprocessor();
        }

        public virtual TagProcessor TagProcessor { get; }
        public virtual IFormatPreprocessor FormatPreprocessor { get; }
        public virtual Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);
        /// <summary>
        /// XFormat delegate based on <see cref="X.Format(FormattableString)"/> is used to convert <see cref="FormattableString"/> to string in PowerConsole.PrintF(..) methods
        /// strings staring with @ symbol are treated as strings with expression(s) and processed by <see cref="X.TextProcessor"/>
        /// </summary>
        public virtual Func<FormattableString, string> XFormat { get; set; } = x => X.Format(x, X.FormatPreprocessor);
        public virtual void Print(string str, bool endLine)
        {
            Write(str, endLine);
        }

        public virtual void Print(string str, ConsoleColor? color, bool endLine)
        {
            if (color == null)
                Write(str, endLine);
            else
            {
                Write(str, color.Value, endLine);
            }
        }

        public virtual void Write(string str, ConsoleColor color, bool endLine)
        {
            var coloredStr = $"{AnsiCodes.Color(color)}{str}{AnsiCodes.RESET}";
            Writer.Write(coloredStr);
            if (endLine)
            {
                Writer.WriteLine();
                NewLineFlag = true;
            }
            else
                NewLineFlag = str.EndsWith('\n');
        }
    }
}