using System.Text;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

/// <summary>
/// StringBuilder extensions
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// If StringBuilder content is not empty and the last character is neither a whitespace ' ', neither '\t' or '\n'
    /// append a whitespace ' '
    /// </summary>
    public static StringBuilder EnsureWhitespace(this StringBuilder sb)
    {
        var last = sb[^1];
        if (sb.Length == 0 || last == ' ' || last == '\t' || last == '\n')
        {
            return sb;
        }

        return sb.Append(' ');
    }
}