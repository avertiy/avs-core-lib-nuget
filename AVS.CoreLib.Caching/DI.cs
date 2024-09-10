using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Caching
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds as singletons a non distributed in memory implementation of <see cref="IMemoryCache"/> and <see cref="IXCacheManager"/>
        /// to the <see cref="IServiceCollection" />  
        /// </summary>
        [Obsolete("Use AddXCacheManager or AddCacheManager if you want a simpler version of CacheManager")]
        public static void AddCaching(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            // Set up configuration files.
            services.AddOptions();
            services.Configure<CachingOptions>(options => config.GetSection("Caching").Bind(options));
            services.AddSingleton<IXCacheManager, XCacheManager>();
        }

        public static void AddXCacheManager(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            // Set up configuration files.
            services.AddOptions();
            services.Configure<CachingOptions>(options => config.GetSection("Caching").Bind(options));
            services.AddSingleton<IXCacheManager, XCacheManager>();
        }

        /// <summary>
        /// Add <see cref="ICacheManager"/> and its implementation <see cref="CacheManager"/>
        /// </summary>
        /// <remarks>
        /// Comparing to <see cref="XCacheManager"/> it is simpler and strait forward,
        /// if your scenario is simple and you don't need control over cached objects like checking their timespan when the object has been cached, or clean cache by key prefixes.
        /// </remarks>
        public static void AddCacheManager(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            // Set up configuration files.
            services.AddSingleton<ICacheManager, CacheManager>();
        }

        /// <summary>
        /// Add <see cref="ICacheKeysBookkeeper"/> if you need to track keys that are stored in cache
        /// </summary>
        public static void AddCacheKeysBookkeeper(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICacheKeysBookkeeper, KeysBookkeeper>();
        }
    }
}