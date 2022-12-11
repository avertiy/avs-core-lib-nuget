using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public interface IBasicWriter
    {
        void Write(string str, bool endLine);
        void Write(string str, bool endLine, bool? containsCTags);
        void WriteLine(bool voidMultipleEmptyLines = true);
        void WriteLine(string str, bool voidMultipleEmptyLines);
    }

    public class BasicWriter : IBasicWriter
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written
        /// </summary>
        protected bool NewLineFlag { get; set; }
        protected TextWriter Writer { get; }
        protected TagProcessor TagProcessor { get; set; }

        public BasicWriter(TextWriter writer, TagProcessor tagProcessor)
        {
            Writer = writer;
            TagProcessor = tagProcessor;
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

        public void Write(string str, bool endLine)
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

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;
            Writer.WriteLine();
            NewLineFlag = true;
        }

        public void WriteLine(string str, bool voidMultipleEmptyLines)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;

            Writer.WriteLine(str);
            NewLineFlag = true;
        }
    }
}