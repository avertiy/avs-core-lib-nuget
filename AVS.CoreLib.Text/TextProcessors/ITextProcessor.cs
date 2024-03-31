namespace AVS.CoreLib.Text.TextProcessors
{
    /// <summary>
    /// common interface for text processors of any kind to transform/translate/convert etc. the input string
    /// for example ColorTagProcessor will replace color tags with ansi-codes
    /// </summary>
    public interface ITextProcessor
    {
        /// <summary>
        /// Text Processor manipulates somehow by the input string and returns the output
        /// 
        /// </summary>
        string Process(string input);
    }
}