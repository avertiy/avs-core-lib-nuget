using System;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVS.CoreLib.REST.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Scoped IRestClient and IRequestBuilder
        /// </summary>
        /// <param name="services"></param>
        [Obsolete("Looks obsolete")]
        public static void AddRestClient(this IServiceCollection services)
        {
            services.TryAddScoped<IHttpRequestBuilder, HttpRequestBuilder>();            
            services.TryAddScoped<IRestClient, RestClient>();
        }

        //not used
        public static void AddRestTools(this IServiceCollection services)
        {
            services.TryAddScoped<IRequestMessageBuilder, RequestMessageBuilder>();
            services.TryAddScoped<RestTools>();

            //the default one HMACSHA512
            services.TryAddScoped<IAuthenticator, HMACSHA512Authenticator>();
            services.TryAddScoped<HMACSHA512Authenticator, HMACSHA512Authenticator>();
            services.TryAddScoped<HMACSHA256Authenticator, HMACSHA256Authenticator>();
        }
    }
}
