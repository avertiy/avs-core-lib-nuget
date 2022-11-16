using System;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    //seems not used
    public static class ResponseExtensions
    {
        public static IResponse<T> OnSuccess<T>(this IResponse response, Func<T> func, string errorMessage = null)
        {
            var newResponse = CreateResponse<T>(response, errorMessage);
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

        public static async Task<IResponse<T>> OnSuccess<T>(this IResponse response, Func<Task<T>> func, string errorMessage = null)
        {
            var newResponse = CreateResponse<T>(response, errorMessage);
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

        private static IResponse<T> CreateResponse<T>(IResponse response, string errorMessage = null)
        {
            if (response is IPropsContainer container)
            {
                return ResponseFactory.Instance.Create<T>(
                    default(T),
                    response.Source,
                    GetErrorText(response.Error, errorMessage),
                    container.Props);
            }
            else
            {
                return ResponseFactory.Instance.Create<T>(
                    default(T),
                    response.Source,
                    GetErrorText(response.Error, errorMessage));
            }
        }

        private static string GetErrorText(string error, string errorMessage)
        {
            if (errorMessage == null)
                return error;
            return $"{errorMessage}. {error}";
        }
    }
}