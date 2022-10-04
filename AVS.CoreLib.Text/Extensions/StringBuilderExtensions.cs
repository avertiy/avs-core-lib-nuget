using System;
using System.Text;

namespace AVS.CoreLib.Text.Extensions
{
    /// <summary>
    /// StringBuilder extensions
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append X.Format(str)
        /// </summary>
        public static StringBuilder XAppend(this StringBuilder sb, FormattableString str)
        {
            return sb.Append(X.Format(str));
        }

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
}