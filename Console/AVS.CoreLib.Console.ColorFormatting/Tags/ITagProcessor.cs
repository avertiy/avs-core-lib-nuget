namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// tag processor might replace tags in input string with something else ansi-codes or strip tags
    /// </summary>
    public interface ITagProcessor
    {
        string Process(string input);
    }
}