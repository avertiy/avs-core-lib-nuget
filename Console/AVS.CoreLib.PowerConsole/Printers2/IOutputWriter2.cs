using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    public interface IOutputWriter2
    {
        void Write(string message);
        void Write(string message, bool endLine);

        void WriteLine(string message);
        void WriteLine(bool voidMultipleEmptyLines = true);
    }

    public class OutputWriter2 : IOutputWriter2
    {
        /// <summary>
        /// Indicates whether output text buffer ends with a new line (\r\n)
        /// </summary>
        protected bool NewLineFlag { get; set; }
        protected TextWriter Writer { get; }
        public OutputWriter2(TextWriter writer)
        {
            Writer = writer;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string message)
        {
            Writer.Write(message);
        }

        public void Write(string message, bool endLine)
        {
            if (endLine)
                Writer.WriteLine(message);
            else
                Writer.Write(message);
        }

        public void WriteLine(string message)
        {
            Writer.WriteLine(message);
            NewLineFlag = true;
        }

        //public virtual void WriteLine(MessageLevel level, string message)
        //{
        //    Writer.WriteLine(message);
        //    NewLineFlag = true;
        //}

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;
            Writer.WriteLine();
            NewLineFlag = true;
        }
    }
}