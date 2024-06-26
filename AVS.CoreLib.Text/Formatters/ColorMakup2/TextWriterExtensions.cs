﻿using AVS.CoreLib.Logging.ColorFormatter.Utils;

namespace AVS.CoreLib.Logging.ColorFormatter.ColorMakup
{
    internal static class TextWriterExtensions
    {
        public static void WriteColored(this TextWriter writer, string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
            if (background.HasValue)
                writer.Write(AnsiCodesHelper.GetBackgroundColorEscapeCode(background.Value));

            if (foreground.HasValue)
                writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(foreground.Value));

            writer.Write(message);

            // restore console colors
            if (background.HasValue)
                writer.Write(AnsiCodesHelper.GetBackgroundColorEscapeCode(Console.BackgroundColor));

            if (foreground.HasValue)
                writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(Console.ForegroundColor));
        }

        public static void WriteColored(this TextWriter writer, string message, ConsoleColor foreground)
        {
            writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(foreground));
            writer.Write(message);
            // restore console colors
            writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(Console.ForegroundColor));
        }

        public static void WriteColorMarkupString(this TextWriter writer, ColorMarkupString2 str)
        {
            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                    writer.Write(plainText);

                if (!string.IsNullOrEmpty(coloredText))
                    writer.WriteColored(coloredText, colorScheme);
            }
        }

        private static void WriteColored(this TextWriter writer, string text, string colorScheme)
        {
            if (ColorMarkup2Helper.TryParse(colorScheme, out var foreground, out var background))
                writer.WriteColored(text, background, foreground);
            else
                writer.Write(text);
        }
    }
}
