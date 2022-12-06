using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class DummyTagProcessor : TagProcessor
    {
        public override string Process(string input)
        {
            return input;
        }

        public override void Process(StringBuilder sb)
        {   
        }
    }

    public class StripTagsProcessor : TagProcessor
    {
        public override void Process(StringBuilder sb)
        {
            var tagsCount = sb.StripTags();
        }
    }
}