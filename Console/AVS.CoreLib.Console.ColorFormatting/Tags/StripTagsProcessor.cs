using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    public class StripTagsProcessor : TagProcessor
    {
        public override void Process(StringBuilder sb)
        {
            var tagsCount = sb.StripTags();
        }
    }
}