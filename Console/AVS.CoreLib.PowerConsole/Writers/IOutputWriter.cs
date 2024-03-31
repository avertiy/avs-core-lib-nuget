using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public interface IOutputWriter
    {
        void Write(string str, bool endLine = true);
        void Write(string message, bool endLine, bool colorTags);
        void Write(string str, PrintOptions options);
        
        void WriteLine(string? str, PrintOptions options);
        void WriteLine(bool voidMultipleEmptyLines = true);
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

        public void Write(string str, bool endLine = true)
        {
            Writer.Write(str);
            if (endLine)
            {
                Writer.WriteLine();
                NewLineFlag = true;
            }
            else
            {
                NewLineFlag = str.EndsWith('\n');
            }
        }

        public void Write(string message, PrintOptions options)
        {
            if (options.HasColors)
            {
                WriteColored(message, options);
            }
            else
            {
                WriteTextWithColorTags(message, options.EndLine, options.ColorTags);
            }
        }

        public void Write(string message, bool endLine, bool colorTags)
        {
            var text = PreProcessText(message, colorTags);
            WriteInternal(text, endLine);
        }

        protected void WriteInternal(string str, bool endLine)
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

        protected void WriteTextWithColorTags(string str, bool endLine, bool? containsCTags)
        {
            var text = PreProcessText(str, containsCTags);
            WriteInternal(text, endLine);
        }

        public abstract void WriteColored(string str, PrintOptions options);

        public void WriteLine(string? str, PrintOptions options)
        {
            if (str == null)
            {
                if (options.VoidEmptyLines && NewLineFlag)
                    return;

                Writer.WriteLine();
            }
            else
            {
                if (options.HasColors)
                {
                    WriteColored(str, options);
                }
                else
                {
                    Writer.WriteLine(str);
                }
            }
            NewLineFlag = true;
        }

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;

            Writer.WriteLine();
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