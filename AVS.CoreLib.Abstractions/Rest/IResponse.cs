#nullable enable

namespace AVS.CoreLib.Abstractions.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Binance, ByBit etc.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// the request payload
        /// </summary>
        object? Request { get; }

        /// <summary>
        /// indicates whether the response is succesfull or not
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// error when response failed
        /// </summary>
        string? Error { get; }

        /// <summary>
        /// Raw content
        /// </summary>
        string? RawContent { get; }
    }

    /// <summary>
    /// Response with T Data
    /// </summary>
    public interface IResponse<T> : IResponse
    {
        /// <summary>
        /// 
        /// </summary>
        T? Data { get; set; }
    }    
}