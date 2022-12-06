using System;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// replace color tags <see cref="CTag"/> with ansi codes
    /// </summary>
    public class CTagProcessor : TagProcessor
    {
        public bool FixBgColorLineEnding { get; set; } = true;
        public int TagMaxLength { get; set; } = 20;

        public override void Process(StringBuilder sb)
        {
            if (!sb.AnyTags())
                return;

            for (var i = 0; i < sb.Length - 3; i++)
            {
                if (!sb.MatchTag(i, out var tagName, out var closingTagIndex, TagMaxLength) || !Enum.TryParse(tagName, out CTag tag))
                    continue;

                var length = tagName.Length + 2;
                var end = closingTagIndex + length + 1;

                if (FixBgColorLineEnding && tagName.StartsWith("bg"))
                    sb.FixBgColorLineEnding(end);

                // fix bg color at the end of line 
                if (end == sb.Length - 1 || end == sb.Length)
                    sb.Append(' ');

                sb.Replace($"</{tagName}>", AnsiCodes.RESET, closingTagIndex, length + 1);
                var ansiCode = tag.ToAnsiCode();
                sb.Replace($"<{tagName}>", ansiCode, i, length);

                i += ansiCode.Length;
                //return back cursor position
                if (i > length)
                    i -= (length - ansiCode.Length + 1);
            }
        }
    }
}