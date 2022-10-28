using System.Text;
using AVS.CoreLib.Logging.ColorFormatter.Utils;

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

    /// <summary>
    /// Returns the index of the start of the contents in a StringBuilder
    /// </summary>
    /// <param name="sb">StringBuilder</param>
    /// <param name="value">The string to find</param>
    /// <param name="startIndex">The starting index.</param>
    /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
    /// <returns></returns>
    public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
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

    internal static int StripTags(this StringBuilder sb, params string[] tags)
    {
        var counter = -1;
        bool validateTag(string tag)
        {
            return tags.Length > 0 ? tags.Any(x => x == tag) : TagHelper.IsValidTagCandidate(tag);
        }

        var start = 0;
        start:
        var ind = sb.IndexOf('<', start);
        if (ind == -1)
            return counter;

        //<time> or <time/> or </time>
        var ind2 = sb.IndexOf('>', ind);
        if (ind2 == -1)
            return counter;

        var tag = sb.ToString(ind, ind2 - ind);

        if (!validateTag(tag))
        {
            start = ind2;
            goto start;
        }

        // self closing tag time/
        if (tag.EndsWith("/"))
        {
            sb.Replace($"<{tag}>", "");
            counter++;
            start = ind;
            goto start;
        }

        var endTagInd = sb.IndexOf($"</{tag}>", ind2);

        if (endTagInd > 0)
        {
            counter++;
            sb.Replace($"<{tag}>", "");
            sb.Replace($"</{tag}>", "");
            start = ind;
            goto start;
        }

        start = ind2;
        goto start;
    }
}
