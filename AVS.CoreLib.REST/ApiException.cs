using System;
using System.Net;

namespace AVS.CoreLib.REST
{
    /// <summary>
    /// an exception that represents api request error 
    /// </summary>
    public class ApiException : Exception
    {   
        public HttpStatusCode? StatusCode { get; set; }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// returns http status code based on error message
        /// if message contains any code (401, 403 etc.) will return the code, otherwise 400
        /// </summary>
        public int? GetStatusCode()
        {
            if(StatusCode.HasValue)
                return (int) StatusCode.Value;

            if (Message.Contains("401"))
                return 401;
            if (Message.Contains("402"))
                return 402;
            if (Message.Contains("403"))
                return 403;
            if (Message.Contains("404"))
                return 404;
            if (Message.Contains("405"))
                return 405;
            if (Message.Contains("406"))
                return 406;
            if (Message.Contains("407"))
                return 407;
            if (Message.Contains("408"))
                return 408;
            if (Message.Contains("409"))
                return 409;
            if (Message.Contains("429"))
                return 429;
            if (Message.Contains("418"))
                return 418;

            return null;
        }
    }

    public class TooManyRequestsApiException : ApiException
    {
        public TooManyRequestsApiException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.TooManyRequests;
        }
    }
}