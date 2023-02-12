using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.REST.Extensions
{
    public static class HttpWebRequestExtensions
    {
        public static HttpWebRequest AcceptsJson(this HttpWebRequest request, string encoding = "gzip,deflate")
        {
            request.Headers[HttpRequestHeader.AcceptEncoding] = encoding;
            request.Headers.Add("Accepts", "application/json");
            return request;
        }

        public static void WriteBytes(this HttpWebRequest request, byte[] bytes)
        {
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static void WriteJson(this HttpWebRequest request, string json)
        {
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }
        }

        public static async Task<HttpWebResponse> GetHttpResponseAsync(this HttpWebRequest request)
        {
            try
            {
                var response = await request.GetResponseAsync().ConfigureAwait(false);
                return response as HttpWebResponse;
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }

        public static async Task<HttpWebResponse> FetchHttpResponseAsync(this HttpWebRequest request, int maxAttempts = 2)
        {
            int attempt = 0;
            start:
            var response = await request.GetHttpResponseAsync().ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.UnprocessableEntity && attempt++ < maxAttempts)
            {
                //try a few times
                goto start;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt++ < maxAttempts)
            {
                //try a few times
                Thread.Sleep(1000);
                goto start;
            }

            return response;
        }

        public static async Task<string> GetContentAsync(this HttpWebResponse response)
        {
            var contentType = response.ContentType;
            var length = response.ContentLength;

            await using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                    throw new NullReferenceException("The HttpWebRequest's response stream should not be null.");

                using (var streamReader = new StreamReader(responseStream))
                {
                    return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }
            }

        }

        public static string GetContent(this HttpWebResponse response)
        {
            var contentType = response.ContentType;
            var length = response.ContentLength;

            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                    throw new NullReferenceException("The HttpWebRequest's response stream should not be null.");

                using (var streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }

        }

        public static async Task<string> FetchResponseAsync(this HttpWebRequest request)
        {
            try
            {
                var response = await request.GetResponseAsync().ConfigureAwait(false);

                using (response)
                {
                    await using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"{{ \"error\": \"Request to {request.RequestUri} failed. {ex.Message}\" }}";
            }
        }
    }
}