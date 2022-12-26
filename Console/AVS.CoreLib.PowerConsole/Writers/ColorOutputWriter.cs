using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class ColorOutputWriter: OutputWriter
    {
        public ColorOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? GetDefaultTagsProcessor())
        {
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ConsoleColor color)
        {
            //if (color.HasValue)
            //{
            var coloredStr = color.Colorize(str);
            base.Write(coloredStr, endLine, containsCTags);
            //}
            //else
            //{
            //    Write(str, endLine, containsCTags);
            //}
        }

        public override void Write(string str, bool endLine, bool? containsCTags, CTag tag)
        {
            var coloredStr = tag.Colorize(str);
            base.Write(coloredStr, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            var coloredStr = scheme.Colorize(str);
            base.Write(coloredStr, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, Colors colors)
        {
            var coloredStr = colors.Colorize(str);
            base.Write(coloredStr, endLine, containsCTags);
        }

        protected static TagProcessor GetDefaultTagsProcessor()
        {
            var tagProcessor = new CompositeTagProcessor();
            tagProcessor.AddTagProcessor(new CTagProcessor());
            tagProcessor.AddTagProcessor(new RgbTagProcessor());
            return tagProcessor;
        }
    }
}