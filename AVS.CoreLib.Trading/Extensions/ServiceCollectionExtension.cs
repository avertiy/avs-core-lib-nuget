using AVS.CoreLib.Text;
using AVS.CoreLib.Trading.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Trading.Extensions
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
            return services;
        }
    }
}
