using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.TagProcessors;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class SwitchColorOutputWriter : OutputWriter
    {
        public SwitchColorOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? new DummyTagProcessor())
        {
        }
       
        public override void WriteColored(string str, PrintOptions options)
        {
            var backup = ColorScheme.GetCurrentScheme();
            
            var colors = options.GetColors();
            PowerConsole.ApplyColors(colors);

            if (options.ColorTags is false)
                WriteInternal(str, options.EndLine);
            else
                WriteTextWithColorTags(str, options.EndLine);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }

        protected void WriteTextWithColorTags(string str, bool endLine)
        {
            Writer.Write(str);
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