using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

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

    
}
