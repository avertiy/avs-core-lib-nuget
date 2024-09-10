using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    public interface IColorOutputWriter : IOutputWriter2
    {
        void Write(string message, Colors colors);
        void Write(string message, bool endLine, Colors? colors);
        void WriteWithCTags(string message, bool endLine, Colors? colors);
        void WriteLine(string message, Colors? colors);
    }

    /// <summary>
    /// colored output writer with coloring based on ansi-codes 
    /// </summary>
    public class ColorOutputWriter : OutputWriter2, IColorOutputWriter
    {
        protected TagProcessor TagProcessor { get; set; }
        public ColorOutputWriter(TextWriter writer) : base(writer)
        {
            TagProcessor = GetDefaultTagsProcessor();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string message, Colors colors)
        {
            Write(colors.Colorize(message));
        }

        public void Write(string message, bool endLine, Colors? colors)
        {
            if (colors == null)
                Write(message, endLine);
            else
            {
                var text = colors.Value.Colorize(message);
                Write(text, endLine);
            }
        }

        public void WriteLine(string message, Colors? colors)
        {
            WriteLine(colors == null ? message : colors.Value.Colorize(message));
        }

        public void WriteWithCTags(string message, bool endLine, Colors? colors)
        {
            var text = TagProcessor.Process(message);
            if (colors.HasValue)
                text = colors.Value.Colorize(text);
            Write(text, endLine);
        }

        protected TagProcessor GetDefaultTagsProcessor()
        {
            var tagProcessor = new CompositeTagProcessor();
            tagProcessor.AddTagProcessor(new CTagProcessor());
            tagProcessor.AddTagProcessor(new RgbTagProcessor());
            return tagProcessor;
        }
    }
}