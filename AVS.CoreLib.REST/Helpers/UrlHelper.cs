namespace AVS.CoreLib.REST.Helpers
{
    public static class UrlHelper
    {
        public static string Combine(string baseUrl, string queryString)
        {
            var res = baseUrl;
            if (queryString.Length > 0)
            {
                if (queryString.StartsWith("?") || queryString.StartsWith("/"))
                    res += queryString;
                else
                    res += (baseUrl.Contains("?") ? "&" : "?") + queryString;
            }

            return res;
        }
    }
}
