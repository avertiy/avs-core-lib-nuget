using AVS.CoreLib.Abstractions.Json;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Clients;
using AVS.CoreLib.REST.RequestBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.REST
{
    public static class DIExtensions
    {
        /// <summary>
        /// Register:
        ///  1. HttpClient (<see cref="System.Net.Http.IHttpClientFactory"/>)
        ///  2. JsonSerializer <see cref="IJsonSerializer"/>
        ///  3. <see cref="PublicRestTools"/>
        /// </summary>
        public static void AddREST<TSerializer>(this IServiceCollection services) where TSerializer : class, IJsonSerializer
        {
            services.AddHttpClient();
            services.AddTransient<IPublicRequestMessageBuilder, PublicRequestMessageBuilder>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddTransient<PublicRestTools>();
            services.AddSingleton<IJsonSerializer, TSerializer>();
        }

        public static void AddHMACSHA512Authenticator(this IServiceCollection services, string publicKey, string privateKey)
        {
            //the default one HMACSHA512
            services.AddSingleton(x =>
            {
                return new HMACSHA512Authenticator(publicKey, privateKey);
            });
            services.AddSingleton(x => (IAuthenticator)x.GetService<HMACSHA512Authenticator>());
        }

        public static void AddHMACSHA256Authenticator(this IServiceCollection services, string publicKey, string privateKey)
        {
            //the default one HMACSHA512
            services.AddSingleton(x =>
            {
                return new HMACSHA256Authenticator(publicKey, privateKey);
            });
            services.AddSingleton(x => (IAuthenticator)x.GetService<HMACSHA256Authenticator>());
        }
    }
}
