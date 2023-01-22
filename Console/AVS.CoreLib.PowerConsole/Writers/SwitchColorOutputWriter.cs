using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
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

        //public override void Write(string str, bool endLine, bool? containsCTags, ConsoleColor? color)
        //{
        //    if (color.HasValue)
        //    {
        //        var colorBackup = System.Console.ForegroundColor;
        //        System.Console.ForegroundColor = color.Value;
        //        base.WriteInternal(str, endLine, containsCTags);
        //        System.Console.ForegroundColor = colorBackup;
        //    }
        //    else
        //    {
        //        base.WriteInternal(str, endLine, containsCTags);
        //    }
        //}

        //public override void Write(string str, bool endLine, bool? containsCTags, CTag tag)
        //{
        //    var colors = tag.ToColors();
        //    this.Write(str, endLine, containsCTags, colors);
        //}

        //public override void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        //{
        //    var backup = ColorScheme.GetCurrentScheme();
        //    PowerConsole.ApplyColorScheme(scheme);

        //    base.WriteInternal(str, endLine, containsCTags);

        //    // restore scheme
        //    PowerConsole.ApplyColorScheme(backup);
        //}

        //public override void Write(string str, bool endLine, bool? containsCTags, Colors colors)
        //{
        //    var backup = ColorScheme.GetCurrentScheme();
        //    PowerConsole.ApplyColors(colors);

        //    if (containsCTags.HasValue && containsCTags.Value)
        //        base.WriteInternal(str, endLine, containsCTags);
        //    else
        //        base.WriteInternal(str, endLine);

        //    // restore scheme
        //    PowerConsole.ApplyColorScheme(backup);
        //}

        public override void WriteColored(string str, PrintOptions options)
        {
            var backup = ColorScheme.GetCurrentScheme();
            
            var colors = options.GetColors();
            PowerConsole.ApplyColors(colors);

            if (options.ColorTags is false)
                base.WriteInternal(str, options.EndLine);
            else
                base.WriteInternal(str, options.EndLine, options.ColorTags);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }
    }
}