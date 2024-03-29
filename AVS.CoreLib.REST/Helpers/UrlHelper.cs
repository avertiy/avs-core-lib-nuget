﻿namespace AVS.CoreLib.REST.Helpers
{
    public static class UrlHelper
    {
        public static string Combine(string baseUrl, string queryString)
        {
            var url = baseUrl ?? "";
            if (queryString?.Length > 0)
            {
                // query string includes url part
                if (queryString.StartsWith("?") || queryString.StartsWith("/"))
                    url += queryString;
                else
                    url += (url.Contains("?") ? "&" : "?") + queryString;
            }

            return url;
        }
    }
}
