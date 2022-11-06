using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Responses;

namespace AVS.CoreLib.REST.Http
{
    /// <summary>
    /// Response object interface
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// The response status code
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Whether the status code indicates a success status
        /// </summary>
        bool IsSuccessStatusCode { get; }

        /// <summary>
        /// The response headers
        /// </summary>
        HttpResponseHeaders ResponseHeaders { get; }

        /// <summary>
        /// Get the response stream
        /// </summary>
        /// <returns></returns>
        Task<Stream> GetResponseStreamAsync();

        /// <summary>
        /// Close the response
        /// </summary>
        void Close();
    }


    /// <summary>
    /// Response object, wrapper for HttpResponseMessage
    /// </summary>
    internal class Response : IResponse
    {
        private readonly HttpResponseMessage _response;

        /// <inheritdoc />
        public HttpStatusCode StatusCode => _response.StatusCode;

        /// <inheritdoc />
        public bool IsSuccessStatusCode => _response.IsSuccessStatusCode;

        /// <inheritdoc />
        public HttpResponseHeaders ResponseHeaders => _response.Headers;

        /// <summary>
        /// Create response for a http response message
        /// </summary>
        /// <param name="response">The actual response</param>
        public Response(HttpResponseMessage response)
        {
            this._response = response;
        }

        /// <inheritdoc />
        public async Task<Stream> GetResponseStreamAsync()
        {
            return await _response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Close()
        {
            _response.Dispose();
        }
    }
}