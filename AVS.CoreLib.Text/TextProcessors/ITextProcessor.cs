namespace AVS.CoreLib.Text.TextProcessors
{
    /// <summary>
    /// interface for text processor that might be used to process input string  
    /// </summary>
    public interface ITextProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string Process(string str);
    }
}