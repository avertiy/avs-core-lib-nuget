using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Caching
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds as singletons a non distributed in memory implementation of <see cref="IMemoryCache"/> and <see cref="ICacheManager"/>
        /// to the <see cref="IServiceCollection" />  
        /// </summary>
        public static void AddCaching(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            // Set up configuration files.
            services.AddOptions();
            services.Configure<CachingOptions>(options => config.GetSection("Caching").Bind(options));
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
        }
    }
}