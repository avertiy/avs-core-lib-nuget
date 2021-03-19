using System;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static ColorScheme Scheme => ColorScheme.Current;

        /// <summary>
        /// Indicates whether new line (\r\n) has been just written 
        /// </summary>
        public static bool NewLineFlag = false;

        /// <summary>
        /// Status of the message will produce beep sound when written to console
        /// <see cref="WriteLine(string,MessageStatus,string)"/>
        /// </summary>
        public static MessageStatus? BeepOnMessageStatus { get; set; } = null;

        public static void ClearLine(int left = 0)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - left));
            Console.SetCursorPosition(left, currentLineCursor);
        }
    }
}
