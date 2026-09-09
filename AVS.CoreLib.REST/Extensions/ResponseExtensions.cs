using AVS.CoreLib.Abstractions.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    /// <summary>
    /// IReseponse extensions
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Throws <see cref="ApiException"/> when response contains Error
        /// </summary>
        public static void ThrowOnError(this IResponse response)
        {
            if (string.IsNullOrEmpty(response.Error))
                return;

            var message = response.Source == null ? response.Error : $"{response.Source} => {response.Error}";
            throw new ApiException(message, response.Source, response.Request) { RawContent = response.RawContent };
        }
    }
}