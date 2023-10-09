using System;
using System.Security.Cryptography;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVS.CoreLib.REST.Extensions
{
    public static class DIExtensions
    {
        /// <summary>
        /// Registers HttpClient (<see cref="System.Net.Http.IHttpClientFactory"/> and related services),
        /// <see cref="IPublicRequestMessageBuilder"/> and <see cref="IRequestMessageBuilder"/> a default implemenation        
        /// </summary>
        public static void AddREST(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IPublicRequestMessageBuilder, PublicRequestMessageBuilder>();
            services.TryAddSingleton<IAuthenticator, Authenticator<HMACSHA512>>();                        
            services.AddTransient<IRequestMessageBuilder, RequestMessageBuilder>();
            services.AddTransient<RestTools>();            
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

        [Obsolete("RestClient is obsolete, use RestTools")]
        public static void AddRestClient(this IServiceCollection services)
        {
            services.TryAddScoped<IHttpRequestBuilder, HttpRequestBuilder>();
            services.TryAddScoped<IRestClient, RestClient>();
        }
    }
}
