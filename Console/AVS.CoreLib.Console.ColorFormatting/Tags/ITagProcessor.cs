namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// Process input string replacing tags, for example replace color tags with ansi-codes or removing tags from the input string to make it a plain text  
    /// </summary>
    public interface ITagProcessor
    {
        string Process(string input);
    }
}