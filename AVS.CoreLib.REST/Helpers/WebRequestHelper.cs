using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AVS.CoreLib.REST.Extensions;

namespace AVS.CoreLib.REST.Helpers
{
    public static class WebRequestHelper
    {
        private static readonly string UserAgent;
        static WebRequestHelper()
        {
            var asm = Assembly.GetExecutingAssembly().GetName();
            UserAgent = $"{asm.Name} v.{asm.Version.ToString(3)}";
        }

        public static HttpWebRequest ConstructHttpWebRequest(string method, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = UserAgent;
            request.Timeout = 15000;
            request.AcceptsJson("gzip,deflate");
            request.ContentType = "application/x-www-form-urlencoded";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }

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

        public static string FetchResponse(this HttpWebRequest request)
        {
            int attempt = 0;
            start:
            var jsonString = request.FetchResponseAttempt();
            if (jsonString != null && jsonString.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }

            return jsonString;
        }

        public static async Task<string> FetchResponseAsync(HttpWebRequest request)
        {
            int attempt = 0;
            start:
            var jsonText = await request.FetchResponseAsync();
            if (jsonText != null && jsonText.Contains("The remote server returned an error: (422).") && attempt++ < 2)
            {
                //sometimes exchange returns 422 error, but on the second attempt it is ok
                goto start;
            }

            return jsonText;
        }

        [DebuggerStepThrough]
        private static string FetchResponseAttempt(this HttpWebRequest request)
        {
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    //var headers = response.Headers;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");
                        using (StreamReader streamReader = new StreamReader(responseStream))
                            return streamReader.ReadToEnd();
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