using AVS.CoreLib.Logging.ColorFormatter.Utils;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions
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

            if (background.HasValue)
            {
                writer.Write(AnsiCodesHelper.GetBackgroundColorEscapeCode(Console.BackgroundColor));
                //writer.Write(AnsiCodesHelper.DEFAULT_BACKGROUND_COLOR); // reset to the background color
            }

            // restore console colors
            if (foreground.HasValue)
            {
                writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(Console.ForegroundColor));
                //writer.Write(AnsiCodesHelper.DEFAULT_FOREGROUND_COLOR); // reset to default foreground color
            }
        }

        public static void WriteColored(this TextWriter writer, string message, ConsoleColor foreground)
        {
            writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(foreground));
            writer.Write(message);
            // restore console colors
            writer.Write(AnsiCodesHelper.GetForegroundColorEscapeCode(Console.ForegroundColor));
        }

        public static void WriteColorMarkupString(this TextWriter writer, ColorMarkupString str)
        {
            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                {
                    writer.Write(plainText);
                }

                if (!string.IsNullOrEmpty(coloredText))
                {
                    writer.WriteColored(coloredText, colorScheme);
                }
            }
        }

        private static void WriteColored(this TextWriter writer, string text, string colorScheme)
        {
            if (ColorMarkupHelper.TryParse(colorScheme, out var foreground, out var background))
            {   
                writer.WriteColored(text, background, foreground);
            }
            else
            {
                writer.Write(text);
            }
        }
    }
}
