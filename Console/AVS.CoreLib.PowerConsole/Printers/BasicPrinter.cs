using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.PowerConsole.Writers;

namespace AVS.CoreLib.PowerConsole.Printers
{
    /// <summary>
    /// simply write str to <see cref="System.Console.Out"/> stream
    /// </summary>
    public class BasicPrinter<TWriter> : IBasicPrinter where TWriter : class, IOutputWriter
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
            Writer.WriteLine(null, voidMultipleEmptyLines);
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