using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.TagProcessors;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class SwitchColorOutputWriter : OutputWriter
    {
        public SwitchColorOutputWriter(TextWriter writer, TagProcessor? tagProcessor = null) : base(writer, tagProcessor ?? new DummyTagProcessor())
        {
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ConsoleColor color)
        {
            var colorBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            base.Write(str, endLine, containsCTags);
            System.Console.ForegroundColor = colorBackup;
        }

        public override void Write(string str, bool endLine, bool? containsCTags, CTag tag)
        {
            var colors = tag.ToColors();
            this.Write(str, endLine, containsCTags, colors);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            var backup = ColorScheme.GetCurrentScheme();
            PowerConsole.ApplyColorScheme(scheme);

            var coloredStr = scheme.Colorize(str);
            base.Write(coloredStr, endLine, containsCTags);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }

        public override void Write(string str, bool endLine, bool? containsCTags, Colors colors)
        {
            var backup = ColorScheme.GetCurrentScheme();
            PowerConsole.ApplyColors(colors);

            if (containsCTags.HasValue && containsCTags.Value)
                base.Write(str, endLine, containsCTags);
            else
                base.Write(str, endLine);

            // restore scheme
            PowerConsole.ApplyColorScheme(backup);
        }
    }
}