using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public interface IOutputWriter
    {
        void Write(string str, bool endLine = true);
        void Write(string str, bool endLine, bool? containsCTags);
        void Write(string str, bool endLine, bool? containsCTags, ConsoleColor? color);
        void Write(string str, bool endLine, bool? containsCTags, CTag tag);
        void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme);
        void Write(string str, bool endLine, bool? containsCTags, Colors colors);
        void WriteLine(string? str = null, bool voidMultipleEmptyLines = true);
    }

    public abstract class OutputWriter : IOutputWriter
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written
        /// </summary>
        protected bool NewLineFlag { get; set; }
        protected TextWriter Writer { get; }
        protected TagProcessor TagProcessor { get; set; }

        protected OutputWriter(TextWriter writer, TagProcessor tagProcessor)
        {
            Writer = writer;
            TagProcessor = tagProcessor;
        }

        public virtual void Write(string str, bool endLine = true)
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

        public virtual void Write(string str, bool endLine, bool? containsCTags)
        {
            var text = PreProcessText(str, containsCTags);
            Writer.Write(text, endLine);
        }

        public abstract void Write(string str, bool endLine, bool? containsCTags, ConsoleColor? color);
        public abstract void Write(string str, bool endLine, bool? containsCTags, CTag tag);
        public abstract void Write(string str, bool endLine, bool? containsCTags, ColorScheme scheme);
        public abstract void Write(string str, bool endLine, bool? containsCTags, Colors colors);

        

        public virtual void WriteLine(string? str = null, bool voidMultipleEmptyLines = true)
        {
            if (str == null)
            {
                if (voidMultipleEmptyLines && NewLineFlag)
                    return;

                Writer.WriteLine();
            }
            else
            {
                if (voidMultipleEmptyLines && NewLineFlag)
                    return;

                Writer.WriteLine(str);
            }
            NewLineFlag = true;
        }

        protected virtual string PreProcessText(string str, bool? containsCTags)
        {
            var text = str;
            if (containsCTags.HasValue && containsCTags.Value || !containsCTags.HasValue)
            {
                text = TagProcessor.Process(str);
            }

            return text;
        }

        public static IOutputWriter Create(TextWriter textWriter, ColorMode mode)
        {
            IOutputWriter writer;
            switch (mode)
            {
                case ColorMode.PlainText:
                    writer = new PlainTextOutputWriter(textWriter);
                    break;
                case ColorMode.AnsiCodes:
                    writer = new ColorOutputWriter(textWriter);
                    break;
                default:
                    writer = new SwitchColorOutputWriter(textWriter);
                    break;
            }

            return writer;

        }
    }
}