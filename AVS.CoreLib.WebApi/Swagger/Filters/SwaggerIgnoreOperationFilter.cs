using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AVS.CoreLib.WebApi.Swagger.Filters
{
    public class SwaggerIgnoreOperationFilter : IOperationFilter
    {
        private bool PropertySelector(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<SwaggerIgnoreAttribute>() != null)
                return true;
            if (pi.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                return true;
            if (pi.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() != null)
                return true;
            return false;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo == null)
                return;

            var ignoredProperties = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetProperties().Where(PropertySelector));

            foreach (var property in ignoredProperties)
            {
                var list = operation.Parameters.Where(p =>
                        (!p.Name.Equals(property.Name, StringComparison.InvariantCulture)))
                    .ToList();
                operation.Parameters = list;
            }
        }
    }
}
