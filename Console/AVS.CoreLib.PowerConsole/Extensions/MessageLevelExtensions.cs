using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class MessageLevelExtensions
    {
        public static Colors GetColors(this MessageLevel messageLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return messageLevel switch
            {
                //MessageLevel.Trace => new Colors(ConsoleColor.DarkGray, null),
                MessageLevel.Debug => new Colors(ConsoleColor.DarkGray, null),
                MessageLevel.Default => new Colors(ConsoleColor.Gray, null),
                MessageLevel.Info => new Colors(ConsoleColor.Cyan, null),
                MessageLevel.Success => new Colors(ConsoleColor.Gray, ConsoleColor.Green),
                MessageLevel.Important => new Colors(ConsoleColor.Blue, ConsoleColor.Yellow),
                MessageLevel.Warning => new Colors(ConsoleColor.Magenta, null),
                MessageLevel.Error => new Colors(ConsoleColor.DarkRed, null),
                MessageLevel.Critical => new Colors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new Colors(null, null)
            };
        }

        public static string GetText(this MessageLevel messageLevel)
        {
            string text;
            switch (messageLevel)
            {
                case MessageLevel.Default:
                    text = "";
                    break;
                case MessageLevel.Debug:
                    text = "dbug";
                    break;
                case MessageLevel.Info:
                    text = "info";
                    break;
                case MessageLevel.Important:
                    text = "impt";
                    break;
                case MessageLevel.Success:
                    text = "succ";
                    break;
                case MessageLevel.Warning:
                    text = "warn";
                    break;
                case MessageLevel.Error:
                    text = "fail";
                    break;
                case MessageLevel.Critical:
                    text = "crit";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageLevel));
            }
            return text;
        }
    }
}