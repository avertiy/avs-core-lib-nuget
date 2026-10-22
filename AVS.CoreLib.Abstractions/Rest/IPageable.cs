namespace AVS.CoreLib.Abstractions.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageable
    {
        /// <summary>
        /// 
        /// </summary>
        void ApplyPageOptions(IPageOptions options);
    }
}