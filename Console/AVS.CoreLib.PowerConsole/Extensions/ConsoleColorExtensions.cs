using System;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    /// <summary>
    /// ConsoleColor extensions
    /// </summary>
    public static class ConsoleColorExtensions
    {
        /// <summary>
        /// Converts <see cref="ConsoleColor"/> to ColorScheme string representation
        /// </summary>
        public static string ToColorSchemeString(this ConsoleColor color)
        {
            return "-" + color.ToString();
        }
    }
}