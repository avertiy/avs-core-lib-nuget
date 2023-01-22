using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class ColorOutputWriter: OutputWriter
    {
        public ColorOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? GetDefaultTagsProcessor())
        {
        }

        public override void WriteColored(string str, PrintOptions options)
        {
            var coloredStr = Colorize(str, options);
            base.WriteInternal(coloredStr, options.EndLine, options.ColorTags);
        }

        protected static TagProcessor GetDefaultTagsProcessor()
        {
            var tagProcessor = new CompositeTagProcessor();
            tagProcessor.AddTagProcessor(new CTagProcessor());
            tagProcessor.AddTagProcessor(new RgbTagProcessor());
            return tagProcessor;
        }

        protected string Colorize(string text, PrintOptions options)
        {
            if (options.Color.HasValue)
            {
                return options.Color.Value.Colorize(text);
            }

            if (options.Scheme.HasValue)
            {
                return options.Scheme.Value.Colorize(text);
            }

            if (options.Colors.HasValue)
            {
                return options.Colors.Value.Colorize(text);
            }

            if (options.CTag.HasValue)
            {
                return options.CTag.Value.Colorize(text);
            }

            return text;
        }
    }
}