using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    public static partial class StringBuilderExtensions
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

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the <paramref name="value"/> parameter if that string is found, 
        /// or -1 if it is not. If <paramref name="value"/> is <see cref="System.String.Empty"/>, the return value 
        /// is <paramref name="startIndex"/>.
        /// </returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
        {
            int index;
            var length = value.Length;
            var maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (var i = startIndex; i < maxSearchLength; ++i)
                {
                    if (char.ToLower(sb[i]) == char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (var i = startIndex; i < maxSearchLength; ++i)
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

        public static IEnumerable<(string value, int i)> IndexOf(this StringBuilder sb, string[] values,
            int startIndex = 0,
            bool ignoreCase = false)
        {
            int index;
            var length = values.Min(x => x.Length);
            var maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (var i = startIndex; i < maxSearchLength; i++)
                {
                    var current = char.ToLower(sb[i]);
                    foreach (var value in values)
                    {
                        if (current == char.ToLower(value[0]))
                        {
                            length = value.Length;
                            index = 1;
                            while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index])))
                                ++index;

                            if (index == length)
                                yield return (value, i);
                        }
                    }
                }

                yield break;
            }

            for (var i = startIndex; i < maxSearchLength; i++)
            {
                foreach (var value in values)
                {
                    if (sb[i] == value[0])
                    {
                        length = value.Length;
                        index = 1;
                        while ((index < length) && (sb[i + index] == value[index]))
                            ++index;

                        if (index == length)
                            yield return (value, i);
                    }
                }

            }
        }




        /// <summary>
        /// Reports the zero-based index position of the first occurrence of the specified Unicode
        /// character within this instance.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">start index</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex = 0)
        {
            var length = 1;
            var maxSearchLength = (sb.Length - length) + 1;

            for (var i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes all occurences of specified characters from <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="removeChars">A Unicode characters to remove.</param>
        /// <returns>
        /// Returns <see cref="System.Text.StringBuilder"/> without specified Unicode characters.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="removeChars"/> is null.</exception>
        public static StringBuilder Remove(this StringBuilder sb, params char[] removeChars)
        {
            for (int i = 0; i < sb.Length;)
            {
                if (removeChars.Any(ch => ch == sb[i]))
                    sb.Remove(i, 1);
                else
                    i++;
            }

            return sb;
        }

        /// <summary>
        /// returns index of line that matches value 
        /// </summary>
        public static int GetLineIndexOf(this StringBuilder sb, string value)
        {
            var offset = sb.IndexOf(value);
            var lineIndex = 0;
            var startIndex = 0;
            start:
            var ind = sb.IndexOf(Environment.NewLine, startIndex);

            if (ind == -1 || ind > offset)
                return lineIndex;

            startIndex =ind + Environment.NewLine.Length;
            lineIndex++;
            goto start;
        }

        /// <summary>
        /// returns index of line where offset points to
        /// e.g. if StringBuilder contains text of 3 lines "line1\r\nline2\r\nline3" offset = 10 (line2) the line index will be 1;   
        /// </summary>
        public static int GetLineIndexOf(this StringBuilder sb, int offset)
        {
            var lineIndex = 0;
            var startIndex = 0;
            start:
            var ind = sb.IndexOf(Environment.NewLine, startIndex);

            if (ind == -1 || ind > offset)
                return lineIndex;
           
            startIndex = ind + Environment.NewLine.Length;
            lineIndex++;
            goto start;
        }

        /// <summary>
        /// returns line by its index 
        /// </summary>
        public static string GetLineAt(this StringBuilder sb, int index)
        {
            var lineIndex = 0;
            var startIndex = 0;

            start:
            var ind = sb.IndexOf(Environment.NewLine, startIndex);

            if(index == lineIndex)
                return sb.ToString(startIndex, ind > -1? ind-startIndex : sb.Length - startIndex);

            if (ind == -1)
                return string.Empty;

            startIndex = ind + Environment.NewLine.Length;
            lineIndex++;
            goto start;
        }

        /// <summary>
        /// Reads a line from a specified index
        /// </summary>
        public static string ReadLine(this StringBuilder sb, int startIndex = 0)
        {
            var ind = sb.IndexOf(Environment.NewLine, startIndex);
            if (ind == -1)
               return sb.ToString(startIndex + Environment.NewLine.Length, sb.Length - startIndex);

            return sb.ToString(startIndex+ Environment.NewLine.Length, ind - startIndex);
        }

        public static string[] ReadAllLines(this StringBuilder sb, int startIndex = 0)
        {
            var lines = new List<string>();
            string line;
            start:
            var ind = sb.IndexOf(Environment.NewLine, startIndex);
            if (ind == -1)
            {
                line = sb.ToString(startIndex + Environment.NewLine.Length, sb.Length - startIndex);
                //if(!string.IsNullOrEmpty(line))
                //    lines.Add(line);
                return lines.ToArray();
            }
            
            line = sb.ToString(startIndex + Environment.NewLine.Length, ind - startIndex);
            lines.Add(line);
            startIndex += line.Length + Environment.NewLine.Length;
            goto start;
        }



        /// <summary>
        /// Removes the range of characters from the specified index to the end of <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> is less than zero, or <paramref name="startIndex"/> is greater
        /// than the length - 1 of this instance.
        /// </exception>
        public static StringBuilder Remove(this StringBuilder sb, int startIndex)
        {
            return sb.Remove(startIndex, sb.Length - startIndex);
        }

        private static bool IsBOMWhitespace(char c)
        {
            return false;
        }

        private static StringBuilder TrimHelper(this StringBuilder sb, int trimType)
        {
            int end = sb.Length - 1;
            int start = 0;
            if (trimType != 1)
            {
                start = 0;
                while (start < sb.Length)
                {
                    if (!char.IsWhiteSpace(sb[start]) && !IsBOMWhitespace(sb[start]))
                    {
                        break;
                    }

                    start++;
                }
            }

            if (trimType != 0)
            {
                end = sb.Length - 1;
                while (end >= start)
                {
                    if (!char.IsWhiteSpace(sb[end]) && !IsBOMWhitespace(sb[start]))
                    {
                        break;
                    }

                    end--;
                }
            }

            return sb.CreateTrimmedString(start, end);
        }

        private static StringBuilder CreateTrimmedString(this StringBuilder sb, int start, int end)
        {
            int length = (end - start) + 1;
            if (length == sb.Length)
            {
                return sb;
            }

            if (length == 0)
            {
                sb.Length = 0;
                return sb;
            }

            return sb.InternalSubString(start, end);
        }

        private static StringBuilder InternalSubString(this StringBuilder sb, int startIndex, int end)
        {
            sb.Length = end + 1;
            sb.Remove(0, startIndex);
            return sb;
        }

        private static StringBuilder TrimHelper(this StringBuilder sb, char[] trimChars, int trimType)
        {
            int end = sb.Length - 1;
            int start = 0;
            if (trimType != 1)
            {
                start = 0;
                while (start < sb.Length)
                {
                    int index = 0;
                    char ch = sb[start];
                    while (index < trimChars.Length)
                    {
                        if (trimChars[index] == ch)
                        {
                            break;
                        }

                        index++;
                    }

                    if (index == trimChars.Length)
                    {
                        break;
                    }

                    start++;
                }
            }

            if (trimType != 0)
            {
                end = sb.Length - 1;
                while (end >= start)
                {
                    int num4 = 0;
                    char ch2 = sb[end];
                    while (num4 < trimChars.Length)
                    {
                        if (trimChars[num4] == ch2)
                        {
                            break;
                        }

                        num4++;
                    }

                    if (num4 == trimChars.Length)
                    {
                        break;
                    }

                    end--;
                }
            }

            return sb.CreateTrimmedString(start, end);
        }

        /// <summary>
        /// Removes all leading occurrences of a set of characters specified in an array 
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters 
        /// that remains after all occurrences of the characters in the <paramref name="trimChars"/> parameter 
        /// are removed from the end of the current string. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            if ((trimChars != null) && (trimChars.Length != 0))
                return sb.TrimHelper(trimChars, 0);
            else
                return sb.TrimHelper(0);
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of characters specified in an array 
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters that remains 
        /// after all occurrences of the characters in the <paramref name="trimChars"/> parameter are removed 
        /// from the end of the current string. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            if ((trimChars != null) && (trimChars.Length != 0))
                return sb.TrimHelper(trimChars, 1);
            else
                return sb.TrimHelper(1);
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters 
        /// that remains after all white-space characters are removed 
        /// from the start and end of the current StringBuilder.
        /// </returns>
        public static StringBuilder Trim(this StringBuilder sb)
        {
            return sb.TrimHelper(2);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters specified in an array
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters that remains 
        /// after all occurrences of the characters in the <paramref name="trimChars"/> parameter are removed 
        /// from the end of the current StringBuilder. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder Trim(this StringBuilder sb, params char[] trimChars)
        {
            if ((trimChars != null) && (trimChars.Length != 0))
                return sb.TrimHelper(trimChars, 2);
            else
                return sb.TrimHelper(2);
        }




        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified Unicode
        /// character in this <see cref="System.Text.StringBuilder"/>. The search starts 
        /// at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, and <paramref name="count"/> 
        /// or <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is greater than 
        /// the length of this <see cref="System.Text.StringBuilder"/>.
        /// -or-The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, and <paramref name="count"/> 
        /// is greater than the length of this string minus <paramref name="startIndex"/>. 
        /// </exception>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex, int count)
        {
            if (sb.Length == 0 || count == 0)
                return -1;

            for (int i = startIndex; i < sb.Length && i < startIndex + count; i++)
            {
                if (sb[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }



        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the <paramref name="value"/> parameter if that string is found, 
        /// or -1 if it is not. If <paramref name="value"/> is <see cref="System.String.Empty"/>, 
        /// the return value is <paramref name="startIndex"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is 
        /// greater than the length of this instance.-or-<paramref name="count"/> is greater than the length of 
        /// this <see cref="System.Text.StringBuilder"/> minus <paramref name="startIndex"/>.
        /// </exception>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count,
            bool ignoreCase = false)
        {
            return IndexOfInternal(sb, value, startIndex, count, ignoreCase);
        }

        private static int IndexOfInternal(StringBuilder sb, string value, int startIndex, int count, bool ignoreCase)
        {
            if (value == string.Empty)
                return startIndex;
            if (sb.Length == 0 || count == 0 || startIndex + 1 + value.Length > sb.Length)
                return -1;

            int num3;
            int length = value.Length;
            int num2 = startIndex + count - value.Length;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i <= num2; i++)
                {
                    if (sb[i] == value[0])
                    {
                        num3 = 1;
                        while ((num3 < length) && (sb[i + num3] == value[num3]))
                        {
                            num3++;
                        }

                        if (num3 == length)
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                for (int j = startIndex; j <= num2; j++)
                {
                    if (char.ToLower(sb[j]) == char.ToLower(value[0]))
                    {
                        num3 = 1;
                        while ((num3 < length) && (char.ToLower(sb[j + num3]) == char.ToLower(value[num3])))
                        {
                            num3++;
                        }

                        if (num3 == length)
                        {
                            return j;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance 
        /// of any character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">anyOf is null.</exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf)
        {
            return sb.IndexOfAny(anyOf, 0, sb.Length);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is negative.-or-<paramref name="startIndex"/> is greater 
        /// than the number of characters in this instance.
        /// </exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex)
        {
            return sb.IndexOfAny(anyOf, startIndex, sb.Length - startIndex);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
        /// -or-<paramref name="count"/> + <paramref name="startIndex"/> is greater than the number of characters in this instance.
        /// </exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex, int count)
        {
            if (sb.Length == 0 || count == 0)
                return -1;

            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (anyOf.Any(ch => ch == sb[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified Unicode
        /// character within this instance.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        public static int LastIndexOf(this StringBuilder sb, char value)
        {
            return LastIndexOf(sb, value, sb.Length - 1, sb.Length);
        }

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of the specified Unicode character 
        /// in a substring within this instance. The search starts at a specified character position and 
        /// proceeds backward toward the beginning of the <see cref="System.Text.StringBuilder"/> 
        /// for a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">
        /// The starting position of the search. The search proceeds from <paramref name="startIndex"/> toward the beginning 
        /// of this instance.
        /// </param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, 
        /// and <paramref name="startIndex"/> is less than zero or greater than or equal to the length of this instance.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, char value, int startIndex)
        {
            return sb.LastIndexOf(value, startIndex, startIndex + 1);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified Unicode
        /// character in this <see cref="System.Text.StringBuilder"/>. The search starts 
        /// at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, 
        /// and <paramref name="startIndex"/> is less than zero or greater than or equal to the length of this instance.
        /// -or-The current instance <see cref="System.Text.StringBuilder.Length"/> 
        /// does not equal 0, and <paramref name="startIndex"/> - <paramref name="count"/> + 1 is less than zero.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, char value, int startIndex, int count)
        {
            if (sb.Length == 0 || count == 0)
                return -1;

            for (int i = startIndex; i > startIndex - count; i--)
            {
                if (sb[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string 
        /// in the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">The string to seek.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public static int LastIndexOf(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == string.Empty)
            {
                if (sb.Length == 0)
                    return 0;
                else
                    return sb.Length - 1;
            }

            if (sb.Length == 0)
                return -1;

            return LastIndexOfInternal(sb, value, sb.Length - 1, sb.Length, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// Parameter specifies the starting search position in the current <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        public static int LastIndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase = false)
        {
            return LastIndexOfInternal(sb, value, startIndex, startIndex + 1, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is less than 0 (zero) or greater than the length of this string.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, string value, int startIndex, int count,
            bool ignoreCase = false)
        {
            return LastIndexOfInternal(sb, value, startIndex, count, ignoreCase);
        }

        private static int LastIndexOfInternal(StringBuilder sb, string value, int startIndex, int count,
            bool ignoreCase)
        {
            if (value == string.Empty)
                return startIndex;
            if (sb.Length == 0 || count == 0 || startIndex + 1 - count + value.Length > sb.Length)
                return -1;

            int num3;
            int length = value.Length;
            int maxValueIndex = length - 1;
            int num2 = startIndex - count + value.Length;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i >= num2; i--)
                {
                    if (sb[i] == value[maxValueIndex])
                    {
                        num3 = 1;
                        while ((num3 < length) && (sb[i - num3] == value[maxValueIndex - num3]))
                        {
                            num3++;
                        }

                        if (num3 == length)
                        {
                            return i - num3 + 1;
                        }
                    }
                }
            }
            else
            {
                for (int j = startIndex; j >= num2; j--)
                {
                    if (char.ToLower(sb[j]) == char.ToLower(value[maxValueIndex]))
                    {
                        num3 = 1;
                        while ((num3 < length) &&
                               (char.ToLower(sb[j - num3]) == char.ToLower(value[maxValueIndex - num3])))
                        {
                            num3++;
                        }

                        if (num3 == length)
                        {
                            return j - num3 + 1;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance 
        /// of any character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf)
        {
            return sb.LastIndexOfAny(anyOf, sb.Length - 1, sb.Length);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is greater 
        /// than the number of characters in this instance.
        /// </exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex)
        {
            return sb.LastIndexOfAny(anyOf, startIndex, startIndex + 1);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
        /// -or-<paramref name="count"/> + <paramref name="startIndex"/> is greater than the number of characters in this instance.
        /// </exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex, int count)
        {
            if (sb.Length == 0 || count == 0)
                return -1;

            for (int i = startIndex; i > startIndex - count; i--)
            {
                if (anyOf.Any(ch => ch == sb[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether this instance of <see cref="System.Text.StringBuilder"/> starts with the specified string.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to compare.</param>
        /// <param name="value">The string to compare.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// true if the <paramref name="value"/> parameter matches the beginning of this string; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static bool StartsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            int length = value.Length;
            if (length > sb.Length)
                return false;

            if (ignoreCase == false)
            {
                for (int i = 0; i < length; i++)
                {
                    if (sb[i] != value[i])
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int j = 0; j < length; j++)
                {
                    if (char.ToLower(sb[j]) != char.ToLower(value[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether this instance of <see cref="System.Text.StringBuilder"/> ends with the specified string.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to compare.</param>
        /// <param name="value">The string to compare to the substring at the end of this instance.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// true if the <paramref name="value"/> parameter matches the beginning of this string; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static bool EndsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            int length = value.Length;
            int maxSBIndex = sb.Length - 1;
            int maxValueIndex = length - 1;
            if (length > sb.Length)
                return false;

            if (ignoreCase == false)
            {
                for (int i = 0; i < length; i++)
                {
                    if (sb[maxSBIndex - i] != value[maxValueIndex - i])
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int j = length - 1; j >= 0; j--)
                {
                    if (char.ToLower(sb[maxSBIndex - j]) != char.ToLower(value[maxValueIndex - j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase.</returns>
        public static StringBuilder ToLower(this StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToLower(sb[i]);
            }

            return sb;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase using specified culture.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="culture"/>is null.</exception>
        public static StringBuilder ToLower(this StringBuilder sb, CultureInfo culture)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToLower(sb[i], culture);
            }

            return sb;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase, using the casing rules of the invarian culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase using invariant culture.</returns>
        public static StringBuilder ToLowerInvariant(this StringBuilder sb)
        {
            return sb.ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase.</returns>
        public static StringBuilder ToUpper(this StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToUpper(sb[i]);
            }

            return sb;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase using specified culture.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="culture"/>is null.</exception>
        public static StringBuilder ToUpper(this StringBuilder sb, CultureInfo culture)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToUpper(sb[i], culture);
            }

            return sb;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase, using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase using invariant culture.</returns>
        public static StringBuilder ToUpperInvariant(this StringBuilder sb)
        {
            return sb.ToUpper(CultureInfo.InvariantCulture);
        }
    }

}