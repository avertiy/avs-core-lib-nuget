using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.PowerConsole.TagProcessors
{
    public class StripTagsProcessor : TagProcessor
    {
        public override void Process(StringBuilder sb)
        {
            var tagsCount = sb.StripTags();
        }
    }
}