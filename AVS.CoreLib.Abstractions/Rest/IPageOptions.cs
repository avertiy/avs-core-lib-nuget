namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageOptions
    {
        /// <summary>
        /// 
        /// </summary>
        int Limit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int Offset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Sort { get; set; }
    }
}