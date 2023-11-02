using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Clients;
using AVS.CoreLib.REST.RequestBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.REST
{
    public static class DIExtensions
    {
        /// <summary>
        /// Adds <see cref="IPublicRestClient"/> with dependent services incl. HttpClient (<see cref="System.Net.Http.IHttpClientFactory"/>                
        /// </summary>
        public static void AddREST(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IPublicRequestMessageBuilder, PublicRequestMessageBuilder>();            
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddTransient<IPublicRestClient, PublicRestClient>();

            //services.TryAddSingleton<IAuthenticator, Authenticator<HMACSHA512>>();
            //services.AddTransient<IRequestMessageBuilder, RequestMessageBuilder>();
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

        //[Obsolete("use AddREST()")]
        //public static void AddRestClient(this IServiceCollection services)
        //{
        //    services.TryAddScoped<IHttpRequestBuilder, HttpRequestBuilder>();
        //    services.TryAddScoped<IRestClient, RestClient>();
        //}
    }
}
