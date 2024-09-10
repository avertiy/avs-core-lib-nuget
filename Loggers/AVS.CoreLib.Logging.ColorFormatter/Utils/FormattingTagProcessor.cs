using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public class FormattingTagProcessor : TagProcessor
{
    public string HeaderPadding { get; set; }

    public override void Process(StringBuilder sb)
    {
        //<H1>
        var tag = sb.ToString(0, 4);
        var closingTag = sb.ToString(sb.Length - 5, 5);

        if (tag == "<H1>" && closingTag == "</H1>")
        {
            if (!sb.StartsWith(Environment.NewLine))
                sb.Insert(0, Environment.NewLine);
            sb.Replace("<H1>", HeaderPadding ?? "<B> ======= ");
            sb.Replace("</H1>", HeaderPadding ?? " ======= </B>");
            return;
        }

        if (tag == "<H2>" && closingTag == "</H2>")
        {
            if (!sb.StartsWith(Environment.NewLine))
                sb.Insert(0, Environment.NewLine);
            sb.Replace("<H2>", "<B> >>>> ");
            sb.Replace("</H2>", "</B>");
        }
    }
}



