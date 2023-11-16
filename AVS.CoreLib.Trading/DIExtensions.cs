using AVS.CoreLib.Text;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Symbols;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Trading
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Add trading formatters core services
        /// </summary>
        public static IServiceCollection AddTradingCore(this IServiceCollection services)
        {
            X.FormatProvider.AddTradingFormatters();
            services.AddSingleton<IRankHelper, RankHelper>();
            services.AddSingleton<SymbolRegistry>();
            services.AddSingleton<ISymbolDescriptorService, SymbolDescriptorService>();
            return services;
        }

        //public static IServiceCollection AddMarketsConfigManager(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var configNodes = configuration.GetSection("markets").Get<MarketConfig[]>();
        //    var manager = new MarketsConfigManager(configNodes);
        //    services.AddSingleton<IMarketsConfigManager, MarketsConfigManager>(_ => manager);
        //    return services;
        //}
    }
}
