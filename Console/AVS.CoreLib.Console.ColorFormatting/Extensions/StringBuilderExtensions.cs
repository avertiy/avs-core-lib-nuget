using System;
using System.Linq;
using System.Text;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Extensions
{
    /// <summary>
    /// StringBuilder extensions
    /// </summary>
    public static partial class StringBuilderExtensions
    {
        public static bool MatchTag(this StringBuilder sb, int index, out string tagName, out int closingTagIndex, int tagMaxLength = 20)
        {
            closingTagIndex = -1;
            tagName = null;
            if (sb[index] != '<')
                return false;

            var ind = sb.IndexOf('>', index + 1, tagMaxLength);
            if (ind == -1)
                return false;

            var tag = sb.ToString(index + 1, ind - index - 1);

            var closingTag = $"</{tag}>";

            var ind2 = sb.IndexOf(closingTag, index + tag.Length + 1);

            if (ind2 == -1)
                return false;

            closingTagIndex = ind2;
            tagName = tag;

            return true;
        }

        /// <summary>
        /// when console is resized when line ends with bgcolor
        /// the bg color will be stretched with the window that seems background color console issue. 
        /// </summary>
        internal static StringBuilder FixBgColorLineEnding(this StringBuilder sb, int index, string lineEnd = ".")
        {
            var rest = sb.Length - index;

            if (rest < 0 || rest > 10)
                return sb;

            if (sb.Length <= index || sb.IndexOf(Environment.NewLine, index) == index)
            {
                sb.Insert(index, lineEnd);
                return sb;
            }

            var restStr = sb.ToString(index, rest);
            restStr = restStr.Replace(AnsiCodes.RESET, "").Replace(Environment.NewLine, "");

            if (restStr.Length == 0)
                sb.Insert(index, lineEnd);

            return sb;
        }

        public static bool AnyTags(this StringBuilder sb)
        {
            var closingArrow = -1;
            var slash = -1;
            //simple check
            for (var i = 1; i < sb.Length - 1; i++)
            {
                var prev = sb[^(i + 1)];
                if (sb[^i] == '>')
                {
                    //case self-closing tag like <time/>
                    if (prev == '/')
                    {
                        closingArrow = sb.Length - i;
                        slash = sb.Length - (i + 1);
                        i++;
                        continue;
                    }
                    else if (char.IsLetter(prev) || char.IsDigit(prev))
                    {
                        closingArrow = sb.Length - i;
                        continue;
                    }
                }

                //case normal closing tag </time>
                string? tagCandidate = null;
                var openingArrow = -1;
                if (sb[^i] == '/' && closingArrow > 0 && prev == '<')
                {
                    slash = sb.Length - i;
                    openingArrow = sb.Length - (i + 1);
                    tagCandidate = sb.ToString(slash + 1, closingArrow - slash - 1);
                    var str = sb.ToString(0, slash);
                    if (IsValidTagCandidate(tagCandidate) && str.Contains($"<{tagCandidate}"))
                        return true;

                    slash = -1;
                    openingArrow = -1;
                    tagCandidate = null;
                }

                //self-closing tag case
                if (sb[^i] == '<' && closingArrow > 0 && slash + 1 == closingArrow)
                {
                    openingArrow = sb.Length - i;
                    tagCandidate = sb.ToString(openingArrow + 1, slash - openingArrow - 1);
                    if (IsValidTagCandidate(tagCandidate))
                        return true;
                }
            }

            return false;
        }

        private static bool IsValidTagCandidate(string str)
        {
            return str.Length <= 12 && str.All(x => char.IsLetter(x) || char.IsDigit(x) || x == '/' || x == '<' || x == '>') ||
                   (str.Length >= 9 && str.Length <= 17 && str.StartsWith("RGB:") && str.Contains(','));
        }

        internal static int StripTags(this StringBuilder sb, params string[] tags)
        {
            var counter = -1;
            bool validateTag(string tag)
            {
                return tags.Length > 0 ? tags.Any(x => x == tag) : IsValidTagCandidate(tag);
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
}