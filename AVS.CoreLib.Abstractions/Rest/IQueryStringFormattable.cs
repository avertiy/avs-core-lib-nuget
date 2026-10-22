namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryStringFormatable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        string ToQueryString(string format);
    }
}