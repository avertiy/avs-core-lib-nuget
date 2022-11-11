namespace AVS.CoreLib.Abstractions.Text
{
    /// <summary>
    /// common interface for text processors of any kind to transform/translate/convert etc. the input string
    /// for example ColorTagProcessor will replace color tags with ansi-codes
    /// </summary>
    public interface ITextProcessor
    {
        /// <summary>
        /// Text Processor manipulates the input string according to text processor's purpose and returns the output
        /// </summary>
        string Process(string input);
    }
}