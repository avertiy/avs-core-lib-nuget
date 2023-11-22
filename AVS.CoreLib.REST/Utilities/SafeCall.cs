#nullable enable
using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Utilities
{
    /// <summary>
    /// util class to wrap async calls in try catch
    /// </summary>
    public static class SafeCall
    {
        public static async Task<IResponse<T>> Execute<T>(Func<Task<Response<T>>> func, string source, string? requestedUrl)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                return Response.Failed<T>(ex, source, requestedUrl);
            }
        }
        /// <summary>
        /// usage example: SafeCall.Execute(async () => await func(arg))
        /// </summary>
        public static async Task<IResponse<T>> Execute<T>(Func<Task<T>> func, string source, string? requestedUrl = null)
        {
            try
            {
                T data = await func();
                return Response.OK(data, source, requestedUrl);
            }
            catch (Exception ex)
            {
                return Response.Failed<T>(ex, source, requestedUrl);
            }
        }
    }
}