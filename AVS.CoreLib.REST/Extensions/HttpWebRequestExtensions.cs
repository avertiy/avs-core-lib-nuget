using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AVS.CoreLib.REST.Extensions
{
    public static class HttpWebRequestExtensions
    {
        public static async Task<string> FetchResponseAsync(this HttpWebRequest request)
        {
            try
            {
                WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);
                using (response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");
                        using (StreamReader streamReader = new StreamReader(responseStream))
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