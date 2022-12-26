using System;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    public abstract class TagProcessor : ITagProcessor
    {
        public static int TagMaxLength { get; set; } = 20;
        public virtual string Process(string input)
        {
            var sb = new StringBuilder(input);
            Process(sb);
            return sb.ToString();
        }

        public abstract void Process(StringBuilder sb);

        /// <summary>
        /// go through string builder looking from open & close tags
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="fn">tagName, startIndex, endIndex</param>
        public static void Process(StringBuilder sb, Func<string, int, int, int> fn)
        {
            if (!sb.AnyTags())
                return;

            for (var i = 0; i < sb.Length - 3; i++)
            {
                if (!sb.MatchTag(i, out var tagName, out var closingTagIndex, TagMaxLength))
                    continue;

                var length = tagName.Length + 2;
                var end = closingTagIndex + length + 1;

                //tagName, startIndex,endIndex
                var shift = fn(tagName, i, end);

                if (shift < 0)
                    continue;

                i += shift;
                //return back cursor position
                if (i > length)
                    i -= (length - shift + 1);
            }
        }
    }
}