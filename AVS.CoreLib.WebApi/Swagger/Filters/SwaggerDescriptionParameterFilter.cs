using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AVS.CoreLib.WebApi.Swagger.Filters
{
    /// <summary>
    /// Allows to customize Swagger OpenApiParameter description using component model Description attribute
    /// just decorate your request properties with a Description attribute
    /// </summary>
    public class SwaggerDescriptionParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.In != ParameterLocation.Query || parameter.Description != null || context.PropertyInfo == null)
                return;

            var pi = context.PropertyInfo;
            var attr = pi.GetCustomAttribute<DescriptionAttribute>();
            parameter.Description = attr?.Description;
        }
    }
}