using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMemoryCacheManager(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            // Set up configuration files.
            services.AddOptions();
            services.Configure<CachingOptions>(options => config.GetSection("Caching").Bind(options));
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
        }
    }
}