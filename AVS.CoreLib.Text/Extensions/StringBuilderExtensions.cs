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

        /// <summary>
        /// Returns the index of the start of the contents in a StringBuilder
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }
    }
}