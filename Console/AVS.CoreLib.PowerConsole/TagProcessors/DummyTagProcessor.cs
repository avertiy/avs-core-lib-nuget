using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.PowerConsole.TagProcessors
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
}