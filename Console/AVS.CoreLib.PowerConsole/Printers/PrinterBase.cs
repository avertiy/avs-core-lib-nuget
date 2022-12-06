using System.IO;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public abstract class PrinterBase
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written
        /// </summary>
        public bool NewLineFlag = false;

        public TextWriter Writer { get; }

        protected PrinterBase(TextWriter writer)
        {
            Writer = writer;
        }
        
        public virtual void Write(string str, bool endLine)
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

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;

            Writer.WriteLine();
            NewLineFlag = true;
        }
    }
}