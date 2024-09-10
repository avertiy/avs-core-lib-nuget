namespace AVS.CoreLib.Text.Formatters.GenericTypeFormatter
{
    /// <summary>
    /// Type formatter interface <seealso cref="GenericTypeFormatter"/>
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