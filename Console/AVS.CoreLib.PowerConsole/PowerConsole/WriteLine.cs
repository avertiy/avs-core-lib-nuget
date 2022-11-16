using System;
using System.Diagnostics;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    using Console= System.Console;
    public static partial class PowerConsole
    {
        #region WriteLine
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

            ApplyColor(color);
            Console.WriteLine(message);
            NewLineFlag = true;
            ColorSchemeReset();
        }

        public static void WriteLine(string message, ColorScheme scheme, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            ApplyColorScheme(scheme);
            Console.WriteLine(message);
            NewLineFlag = true;
            ColorSchemeReset();
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

        public static void WriteLine(int posX, int posY, params string[] arr)
        {
            ClearRegion(posX, posY, arr.Length);
            foreach (var text in arr)
            {
                Console.WriteLine(text);
            }
        }

        public static void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;
            Console.WriteLine();
            NewLineFlag = true;
        } 
        #endregion

        public static void Write(string value)
        {
            Console.Write(value);
            NewLineFlag = value.EndsWith("\r\n");
        }

        public static void Write(string value, bool endLine)
        {
            Console.Write(value);
            NewLineFlag = value.EndsWith("\r\n");
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }

        public static void Write(string value, ConsoleColor color)
        {
            ApplyColor(color);
            Console.Write(value);
            ColorSchemeReset();
            NewLineFlag = value.EndsWith("\r\n");
        }

        public static void Write(string value, ConsoleColor? color, bool endLine = false)
        {
            if (color.HasValue)
            {
                ApplyColor(color.Value);
                Console.Write(value);
                ColorSchemeReset();
            }
            else
            {
                Console.Write(value);
            }
            
            NewLineFlag = value.EndsWith("\r\n");
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }

        public static void Write(string value, ColorScheme scheme, bool endLine = false)
        {
            ApplyColorScheme(scheme);
            Console.Write(value);
            ColorSchemeReset();
            NewLineFlag = value.EndsWith("\r\n");
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
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
                WriteLine(ex.StackTrace, MessageStatus.Debug, null);
        }

        public static void WriteEndLine(bool endLine)
        {
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }
    }
}
