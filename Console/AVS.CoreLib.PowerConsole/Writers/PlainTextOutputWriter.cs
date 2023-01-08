using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class PlainTextOutputWriter : OutputWriter
    {
        public PlainTextOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? GetDefaultTagsProcessor())
        {
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ConsoleColor? color)
        {
            base.WriteInternal(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, CTag tag)
        {
            base.WriteInternal(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            base.WriteInternal(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, Colors colors)
        {
            base.WriteInternal(str, endLine, containsCTags);
        }

        public override void WriteColored(string str, PrintOptions options)
        {
            base.WriteInternal(str, options.EndLine, options.ColorTags);
        }

        protected static TagProcessor GetDefaultTagsProcessor()
        {
            return new AVS.CoreLib.PowerConsole.TagProcessors.StripTagsProcessor();
        }
    }
}