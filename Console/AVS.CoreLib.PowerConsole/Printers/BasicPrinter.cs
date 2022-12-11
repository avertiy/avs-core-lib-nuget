using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.PowerConsole.Writers;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IBasicPrinter
    {
        void Print(string str, bool endLine);

        void Print(FormattableString str, bool endLine);

        void PrintLine(bool voidMultipleEmptyLines = true);
        void PrintLine(string str, bool voidMultipleEmptyLines);
    }

    /// <summary>
    /// simply write str to <see cref="System.Console.Out"/> stream
    /// </summary>
    public class BasicPrinter<TWriter> : IBasicPrinter where TWriter : class, IBasicWriter
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written
        /// </summary>
        protected bool NewLineFlag { get; set; }
        protected TWriter Writer { get; set; }
        protected Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);

        public BasicPrinter(TWriter writer)
        {
            Writer = writer;
        }

        public void Print(string str, bool endLine)
        {
            Writer.Write(str, endLine);
        }

        public void Print(FormattableString str, bool endLine)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine);
        }

        public void PrintLine(bool voidMultipleEmptyLines = true)
        {
            Writer.WriteLine(voidMultipleEmptyLines);
        }

        public void PrintLine(string str, bool voidMultipleEmptyLines)
        {
            Writer.WriteLine(str, voidMultipleEmptyLines);
        }

        protected string FormatInternal(FormattableString str)
        {
            return Format?.Invoke(str) ?? str.ToString(CultureInfo.CurrentCulture);
        }
    }
}