using System;
using System.Diagnostics;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="message">Message to be written to console output</param>
        /// <param name="color">Color of message text</param>
        /// <param name="timeFormat">Date and time format of the time written next to message in console output</param>
        public static void WriteLine(string message, ConsoleColor color, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            var scheme = new ColorScheme(color);
            scheme.Apply();
            Console.WriteLine(message);
            NewLineFlag = true;
            scheme.Restore();
        }

        public static void WriteLine(string message, ColorScheme scheme, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            scheme.Apply();
            Console.WriteLine(message);
            NewLineFlag = true;
            scheme.Restore();
        }


        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="message">Message to be written to console output</param>
        /// <param name="status">status of the message to be written to console output</param>
        /// <param name="timeFormat">Date and time format of the time written next to message in console output</param>
        public static void WriteLine(string message, MessageStatus status, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            WriteLine(message, ColorScheme.GetStatusColorScheme(status), timeFormat);
            if (BeepOnMessageStatus.HasValue && status == BeepOnMessageStatus.Value)
                Console.Beep();
        }

        public static void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;
            Console.WriteLine();
            NewLineFlag = true;
        }

        public static void Write(string value)
        {
            Console.Write(value);
            NewLineFlag = value.EndsWith("\r\n");
        }

        public static void Write(string value, ConsoleColor color)
        {
            var scheme = new ColorScheme(color);
            scheme.Apply();
            Console.Write(value);
            scheme.Restore();
            NewLineFlag = value.EndsWith("\r\n");
        }

        [Conditional("DEBUG")]
        public static void WriteDebug(string str, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            WriteLine(str, MessageStatus.Debug, timeFormat);
        }

        public static void WriteError(Exception ex, bool printStackTrace = true, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            Write($"\r\n {ex.GetType().Name}: ", ConsoleColor.DarkRed);
            WriteLine(ex.Message, MessageStatus.Error, timeFormat);
            if (printStackTrace)
                WriteLine(ex.Message, MessageStatus.Debug, null);
        }
    }
}
