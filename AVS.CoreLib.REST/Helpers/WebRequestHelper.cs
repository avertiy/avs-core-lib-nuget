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
        private static readonly string _userAgent;
        static WebRequestHelper()
        {
            var asm = Assembly.GetExecutingAssembly().GetName();
            _userAgent = $"{asm.Name} v.{asm.Version.ToString(3)}";
        }

        public static HttpWebRequest ConstructHttpWebRequest(string method, string url, IWebProxy proxy, string contentType = "application/x-www-form-urlencoded")
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = _userAgent;
            request.Timeout = 15000;
            request.AcceptsJson("gzip,deflate");
            request.ContentType = contentType;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (proxy != null)
                request.Proxy = proxy;

            return request;
        }

        #region FetchResponse
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
        #endregion

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

        
    }
}