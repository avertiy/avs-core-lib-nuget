using Microsoft.AspNetCore.Builder;

namespace AVS.CoreLib.WebApi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseDeveloperJsonExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DeveloperJsonExceptionMiddleware>();
        }
    }
}