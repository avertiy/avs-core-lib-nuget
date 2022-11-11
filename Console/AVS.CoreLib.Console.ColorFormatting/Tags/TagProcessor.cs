using System.Text;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    public abstract class TagProcessor : ITagProcessor
    {
        public virtual string Process(string input)
        {
            var sb = new StringBuilder(input);
            Process(sb);
            return sb.ToString();
        }

        public abstract void Process(StringBuilder sb);
    }
}