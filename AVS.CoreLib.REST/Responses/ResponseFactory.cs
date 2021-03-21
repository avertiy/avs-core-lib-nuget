using System;
using AVS.CoreLib.Abstractions.Responses;

namespace AVS.CoreLib.REST.Responses
{
    public class ResponseFactory : IResponseFactory
    {
        public virtual IResponse Create(string source = null, string error = null, dynamic props = null)
        {
            return new Response() { Source = source, Error = error, Props = props };
        }

        public virtual IResponse<T> Create<T>(string source = null, string error = null, dynamic props = null)
        {
            return new Response<T>() { Error = error, Source = source, Props = props };
        }

        public virtual IResponse<T> Create<T>(T data, string source = null, string error = null, dynamic props = null)
        {
            return new Response<T>() { Data = data, Error = error, Source = source, Props = props };
        }

        public virtual IResponse<T> Create<T>(T data, Exception ex)
        {
            return new Response<T>() { Data = data, Error = ex.ToString() };
        }

        private static IResponseFactory _factory;
        public static IResponseFactory Instance
        {
            get => _factory ??= new ResponseFactory();
            set => _factory = value;
        }
    }
}