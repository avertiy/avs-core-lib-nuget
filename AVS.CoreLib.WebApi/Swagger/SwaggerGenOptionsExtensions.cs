using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AVS.CoreLib.WebApi.Swagger
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void MapTypeAsString<T>(this SwaggerGenOptions swaggerGenOptions, string description = null)
        {
            swaggerGenOptions.MapType(typeof(T), () => new OpenApiSchema(){Type = "string", Description = description});
        }
    }
}