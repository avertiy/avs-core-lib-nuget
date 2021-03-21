using AVS.CoreLib.Text;
using AVS.CoreLib.Trading.FormatProviders;
using AVS.CoreLib.Trading.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTradingCore(this IServiceCollection services)
        {
            services.AddTradingFormatters();
            services.AddTradingHelpers();
            return services;
        }

        public static IServiceCollection AddTradingFormatters(this IServiceCollection services)
        {
            X.FormatProvider.AppendFormatter(new PriceFormatter());
            X.FormatProvider.AppendFormatter(new PairStringFormatter());
            X.FormatProvider.AppendFormatter(new TradingEnumsFormatter());
            X.FormatProvider.AppendFormatter(new CurrencySymbolFormatter());
            return services;
        }
        
        public static IServiceCollection AddTradingHelpers(this IServiceCollection services)
        {
            services.AddSingleton<IRankHelper, RankHelper>();
            return services;
        }
    }
}
