using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using AVS.CoreLib.ConsoleColors;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

/// <summary>
/// StringBuilder extensions
/// </summary>
public static partial class StringBuilderExtensions
{
    /// <summary>
    /// when console is resized when line ends with bgcolor the bg color will be stretched with the window that seems background color console issue. 
    /// </summary>
    public static StringBuilder FixBgColorLineEnding(this StringBuilder sb, int index, string lineEnd = ".")
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
        restStr = restStr.Replace(AnsiCodes.RESET,"").Replace(Environment.NewLine,"");

        if (restStr.Length == 0)
            sb.Insert(index, lineEnd);

        return sb;
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
