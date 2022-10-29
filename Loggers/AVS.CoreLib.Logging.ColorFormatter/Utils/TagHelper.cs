using System.Text;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public static class TagHelper
{
    public const int TAG_MAX_LENGTH = 20;
    //private static Regex _regex = new Regex("<(\\w*)\b[^>]*>(.*?)</\\1>");

    public static bool MatchTag(this StringBuilder sb, int index, out string tagName, out int closingTagIndex)
    {
        closingTagIndex = -1;
        tagName = null;
        if (sb[index] != '<')
            return false;

        var ind = sb.IndexOf('>', index + 1, TAG_MAX_LENGTH);
        if (ind == -1)
            return false;

        var tag = sb.ToString(index + 1, ind - index - 1);

        var closingTag = $"</{tag}>";

        var ind2 = sb.IndexOf(closingTag, index+tag.Length+1);
        
        if (ind2 == -1)
            return false;

        closingTagIndex = ind2;
        tagName = tag;

        return true;
    }

    public static string[] GetColorTags()
    {
        var colors = Enum.GetValues<ConsoleColor>();
        var list = new List<string>()
        {
            "b", //bold/bright
            "u", //underline
            "r" //reverted
        };

        foreach (var color in colors)
        {
            list.Add(color.ToString());
            list.Add($"bg{color}");
        } 
        return list.ToArray();
    }

    public static string[] GetFormattingTags()
    {
        var tags = new []
        {
            "H1","H2","H3"
        };

        return tags;
    }

    public static bool HasTags(StringBuilder sb)
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
            string tagCandidate = null;
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
                if(IsValidTagCandidate(tagCandidate))
                    return true;
            }
        }

        return false;
    }

    internal static bool IsValidTagCandidate(string str)
    {
        return str.Length <= 12 && str.All(x => char.IsLetter(x) || char.IsDigit(x) || x == '/' || x=='<' || x=='>') || 
               (str.Length >=9 && str.Length <=17 && str.StartsWith("RGB:") && str.Contains(','));
    }

    public static bool HasTags(string str)
    {
        var closingArrow = -1;
        var slash = -1;
        //simple check
        for (var i = 1; i < str.Length - 1; i++)
        {
            var prev = str[^(i + 1)];
            if (str[^i] == '>')
            {
                //case self-closing tag like <time/>
                if (prev == '/')
                {
                    closingArrow = str.Length - i;
                    slash = str.Length - (i + 1);
                    i++;
                    continue;
                }else if (char.IsLetter(prev) || char.IsDigit(prev))
                {
                    closingArrow = str.Length - i;
                    continue;
                }
            }

            //case normal closing tag </time>
            string tagCandidate = null;
            var openingArrow = -1;
            if (str[^i] == '/' && closingArrow > 0 && prev == '<')
            {
                slash = str.Length - i;
                openingArrow = str.Length - (i + 1);
                tagCandidate = str.Substring(slash+1, closingArrow-slash-1);
                if(IsValidTagCandidate(tagCandidate) && str.Contains($"<{tagCandidate}"))
                    return true;

                slash = -1;
                openingArrow = -1;
                tagCandidate = null;
            }

            //self-closing tag case
            if (str[^i] == '<' && closingArrow > 0 && slash + 1 == closingArrow)
            {
                openingArrow = str.Length - i;
                tagCandidate = str.Substring(openingArrow + 1,  slash - openingArrow-1);
                if(IsValidTagCandidate(tagCandidate))
                    return true;
            }
        }

        return false;
    }

    public static bool HasAnyPairedTags(string str, params string[] tagNames)
    {
        foreach (var tagName in tagNames)
        {
            if (str.Contains($"<{tagName}>") && str.Contains($"</{tagName}>"))
                return true;
        }

        return false;
    }

    public static bool HasAnyTags(string str, params string[] tagNames)
    {
        foreach (var tagName in tagNames)
        {
            if(str.Contains($"<{tagName}>") && str.Contains($"</{tagName}>"))
                return true;

            if (str.Contains($"<{tagName}/>"))
                return true;
        }

        return false;
    }

    
    public static bool IsWrappedInTag(string str, string tagName)
    {
        return (str.StartsWith($"<{tagName}>") && str.EndsWith($"</{tagName}>"));
    }

    public static bool Contains(string str, string tagName, bool selfClosingTag = false)
    {
        return selfClosingTag ? str.Contains($"<{tagName}/>") : (str.Contains($"<{tagName}>") && str.Contains($"</{tagName}>"));
    }

    public static string StripTags(string str, params string[] tagNames)
    {
        var sb = new StringBuilder(str);
        foreach (var tagName in tagNames)
        {
            sb.Replace($"<{tagName}>", "");
            sb.Replace($"</{tagName}>", "");
            sb.Replace($"<{tagName}/>", "");
        }

        return sb.ToString();
    }

    

    public static string Trim(string str, string tag)
    {
        var start = tag.Length + 2;
        var len = str.Length - start - tag.Length - 3;
        return str.Substring(start, len);
    }
}

public static class StringExtensions
{
    public static bool ExactMatch(this string str, params string[] args)
    {
        return args.Any(x => x == str);
    }

}