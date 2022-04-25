using System;
using AVS.CoreLib.Text.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Text.Extensions
{
    /// <summary>
    /// Service collection extension methods
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddCustomFormatters(this IServiceCollection services,
            params CustomFormatter[] formatters)
        {
            foreach (var customFormatter in formatters)
            {
                X.FormatProvider.AppendFormatter(customFormatter);
            } 
            
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddCustomFormatter<T>(this IServiceCollection services) where T: CustomFormatter, new()
        {
            X.FormatProvider.AppendFormatter(new T());
            return services;
        }

        /// <summary>
        /// Register formatter for type
        /// <see cref="CompositeFormatter"/>
        /// </summary>
        public static IServiceCollection AddFormatterForType<T>(this IServiceCollection services,
            string[] qualifiers, Func<string, T, string> formatter)
        {
            X.FormatProvider.ConfigureCompositeFormatter(x => x.AddTypeFormatter(qualifiers, formatter));
            return services;
        }

        /// <summary>
        /// Configure composite formatter (it used to register/remove type formatters)
        /// <see cref="CompositeFormatter"/>
        /// </summary>
        public static IServiceCollection ConfigureCompositeFormatter(this IServiceCollection services, Action<CompositeFormatter> configure)
        {
            X.FormatProvider.ConfigureCompositeFormatter(configure);
            return services;
        }
    }
}