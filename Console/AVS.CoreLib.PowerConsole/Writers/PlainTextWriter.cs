using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class PlainTextWriter : BasicWriter, IColorWriter
    {
        public PlainTextWriter(TextWriter writer) : base(writer, new AVS.CoreLib.PowerConsole.TagProcessors.StripTagsProcessor())
        {
        }

        public void Write(string str, ConsoleColor? color, bool endLine, bool? containsCTags)
        {
            base.Write(str, endLine, containsCTags);
        }

        public void Write(string str, ColorScheme scheme, bool endLine, bool? containsCTags)
        {
            base.Write(str, endLine);
        }

        public void Write(string str, Colors colors, bool endLine, bool? containsCTags)
        {
            base.Write(str, endLine);
        }
    }
}