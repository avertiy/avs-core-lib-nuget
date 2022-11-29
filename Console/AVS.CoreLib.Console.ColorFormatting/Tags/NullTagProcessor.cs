namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    public class NullTagProcessor : ITagProcessor
    {
        public string Process(string input)
        {
            return input;
        }
    }
}