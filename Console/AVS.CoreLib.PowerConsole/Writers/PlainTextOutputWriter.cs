using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class PlainTextOutputWriter : OutputWriter
    {
        public PlainTextOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? GetDefaultTagsProcessor())
        {
        }

        public override void WriteColored(string str, PrintOptions options)
        {
            base.WriteTextWithColorTags(str, options.EndLine, options.ColorTags);
        }

        protected static TagProcessor GetDefaultTagsProcessor()
        {
            return new AVS.CoreLib.PowerConsole.TagProcessors.StripTagsProcessor();
        }
    }
}