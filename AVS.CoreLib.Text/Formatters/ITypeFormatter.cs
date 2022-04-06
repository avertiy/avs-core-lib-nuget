namespace AVS.CoreLib.Text.Formatters
{
    /// <summary>
    /// Type formatter interface
    /// </summary>
    public interface ITypeFormatter
    {
        /// <summary>
        /// string format qualifier
        /// </summary>
        string[] Qualifiers { get; }

        /// <summary>
        /// format argument 
        /// </summary>
        string Format(string format, object arg);
    }
}