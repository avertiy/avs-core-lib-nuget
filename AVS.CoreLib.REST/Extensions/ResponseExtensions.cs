using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class ResponseExtensions
    {
        public static IResponse<T> OnSuccess<T>(this IResponse response, Func<T> func, string errorMessage = null)
        {
            var newResponse = CopyResponse<T>(response, errorMessage);
            if (response.Success)
            {
                try
                {
                    newResponse.Data = func();
                }
                catch (Exception ex)
                {
                    newResponse.Error = GetErrorText($"Unhandled exception: {ex.Message}\r\n\r\n{ex.StackTrace}", errorMessage);
                }
            }
            return newResponse;
        }

        public static void ThrowOnError(this IResponse response)
        {
            if (string.IsNullOrEmpty(response.Error))
                return;

            throw new ApiException(response);
        }

        [Obsolete("looks obsolete")]
        public static void OnError(this IResponse response, Action<string> action)
        {
            var error = response.Error;
            if (string.IsNullOrEmpty(error))
                return;

            action(error);
        }

        [Obsolete("looks obsolete")]
        public static async Task<IResponse<T>> OnSuccess<T>(this IResponse response, Func<Task<T>> func, string errorMessage = null)
        {
            var newResponse = CopyResponse<T>(response, errorMessage);
            if (response.Success)
            {
                try
                {
                    newResponse.Data = await func();
                }
                catch (Exception ex)
                {
                    response.Error = GetErrorText($"Unhandled exception: {ex.Message}\r\n\r\n{ex.StackTrace}", errorMessage);
                }
            }
            return newResponse;
        }

        private static IResponse<T> CopyResponse<T>(IResponse response, string errorMessage = null)
        {
            return Response.Create(default(T),
                    response.Source,
                    GetErrorText(response.Error, errorMessage), response.Request);            
        }

        private static string GetErrorText(string error, string errorMessage)
        {
            if (errorMessage == null)
                return error;
            return $"{errorMessage}. {error}";
        }
    }
}