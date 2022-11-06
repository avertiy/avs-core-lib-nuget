using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.REST.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<string> ExecuteStringAsync(this HttpClient client, HttpRequestMessage request,
            CancellationToken cancellationToken, int attempts = 2)
        {
            start:
            attempts--;
            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                       .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                if (text != null && text.Contains("The remote server returned an error: (422).") && attempts > 0)
                    goto start;
                return text;
            }
        }
    }
}