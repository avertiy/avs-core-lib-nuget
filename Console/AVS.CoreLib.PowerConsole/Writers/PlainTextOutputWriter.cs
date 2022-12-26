using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class PlainTextOutputWriter : OutputWriter
    {
        public PlainTextOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? GetDefaultTagsProcessor())
        {
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ConsoleColor color)
        {
            base.Write(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, CTag tag)
        {
            base.Write(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            base.Write(str, endLine, containsCTags);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, Colors colors)
        {
            base.Write(str, endLine, containsCTags);
        }

        protected static TagProcessor GetDefaultTagsProcessor()
        {
            return new AVS.CoreLib.PowerConsole.TagProcessors.StripTagsProcessor();
        }
    }
}