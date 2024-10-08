﻿#nullable enable
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.REST.Helpers;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.REST.Clients
{
    public interface IRestTools
    {
        string Source { get; set; }
        string HttpClientName { get; set; }
        string? LastRequestUrl { get; }
        Task<RestResponse> SendRequest(IRequest request, CancellationToken ct = default);
    }

    /// <summary>
    /// RestTools based on HttpClient which is recommended instead of HttpWebRequest in .NET Core/.NET 5+
    /// </summary>
    public class RestTools : IRestTools
    {
        private readonly IRequestMessageBuilder _requestMessageBuilder;
        private readonly IHttpClientFactory _httpClientFactory;
        protected ILogger Logger { get; }
        protected IRateLimiter RateLimiter { get; }
        public string Source { get; set; } = nameof(RestTools);
        public string HttpClientName { get; set; } = "DefaultHttpClient";
        public string? LastRequestUrl { get; private set; }
        /// <summary>
        /// sometimes api source might return 422 error, we can make a few attempts to do the request
        /// </summary>
        public int RetryAttempts { get; set; } = 2;

        public RestTools(IRequestMessageBuilder requestMessageBuilder, IHttpClientFactory httpClientFactory, IRateLimiter rateLimiter, ILogger logger)
        {
            _requestMessageBuilder = requestMessageBuilder;
            _httpClientFactory = httpClientFactory;
            RateLimiter = rateLimiter;
            Logger = logger;
        }

        public async Task<RestResponse> SendRequest(IRequest request, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(HttpClientName);

                // 1. rate limit
                await RateLimiter.Execute(HttpClientName, request.RateLimit, ct);

                // 2. prepare http request message
                using var requestMessage = GetHttpRequestMessage(request);

                // 3. send request
                var responseMessage = await SendAsync(client, requestMessage, RetryAttempts, ct);

                // 4. prepare response
                var response = await PrepareResponse(request, responseMessage);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{nameof(SendRequest)} failed");
                throw;
            }
        }

        protected virtual async ValueTask<RestResponse> PrepareResponse(IRequest request, HttpResponseMessage responseMessage)
        {
            // 1. adjust rate limit by status code
            AdjustRateLimit(responseMessage);

            // 2. get content and/or error 
            var (content, error) = await GetContentAndError(responseMessage);

            var response = new RestResponse(Source, responseMessage.StatusCode)
            {
                Request = request,
                Content = content,
                Error = error,
            };

            OnResponseReady(response);
            return response;
        }

        protected virtual void AdjustRateLimit(HttpResponseMessage response)
        {
            RateLimiter.Adjust(response.StatusCode);
        }

        protected async ValueTask<(string content, string? error)> GetContentAndError(HttpResponseMessage responseMessage)
        {
            var content = string.Empty;
            var error = responseMessage.ReasonPhrase;
            // verify 429 status code
            if (!responseMessage.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(error))
                {
                    error = responseMessage.StatusCode == HttpStatusCode.TooManyRequests
                        ? "Too many requests"
                        : await responseMessage.Content.ReadAsStringAsync();
                }

                return (content, $"{responseMessage.StatusCode} - {error}");
            }

            content = await responseMessage.Content.ReadAsStringAsync();

            return ResponseHelper.ContainsError(content, out error)
                ? (string.Empty, error)
                : (content, error);
        }

        protected virtual void OnResponseReady(RestResponse response)
        {
            if (response.Error != null)
                return;

            if (string.IsNullOrEmpty(response.Content))
                return;

            var re = new Regex("(error|err-msg|error-message)\"?:\"?(?<error>.+?)\"", RegexOptions.IgnoreCase);
            var match = re.Match(response.Content);

            if (match.Success)
                response.Error = match.Groups["error"].Value;
        }

        protected virtual bool IsSuccess(HttpResponseMessage responseMessage, out string? error)
        {
            error = responseMessage.ReasonPhrase;
            if (responseMessage.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (string.IsNullOrEmpty(error))
                    error = $"{HttpStatusCode.TooManyRequests} - too many requests";

                return false;
            }

            return responseMessage.IsSuccessStatusCode;
        }

        protected HttpRequestMessage GetHttpRequestMessage(IRequest request)
        {
            return _requestMessageBuilder.Build(request);
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, int attempts, CancellationToken ct)
        {
            LastRequestUrl = request.RequestUri?.ToString();

            var attempt = 0;
            start:
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity && attempt < attempts)
            {
                attempt++;
                goto start;
            }

            return response;
        }
    }
}