using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Projections
{
    public class MapResult
    {
        public string Source { get; set; }
        public string Error { get; set; }
        public bool Success => Error == null;

        public dynamic Data { get; set; }

        public Response<T> AsResponse<T>()
        {
            var response = new Response<T>() { Source = Source, Error = Error };
            if (Error == null)
                response.Data = (T)Data;
            return response;
        }

        public Response<T> AsResponse<T>(Func<dynamic, T> transform)
        {
            var response = new Response<T>() { Source = Source, Error = Error };
            if (Error == null)
                response.Data = transform(Data);
            return response;
        }

        public override string ToString()
        {
            return Error != null ? $"{Source} => Failed [{Error}]" : $"{Source} => OK [{Data}]";
        }
    }

    public sealed class MapResult<TKey, TValue> : MapResult
    {
        //public static implicit operator Response<IDictionary<TKey, TValue>>(MapResult<TKey, TValue> map)
        //{
        //    return Response.Create<IDictionary<TKey, TValue>>(map.Data, map.Error);
        //}

        public Response<T> AsResponse<T>(Func<IDictionary<TKey, TValue>, T> transform)
        {
            var response = new Response<T>() { Source = Source, Error = Error };
            if (Error == null)
                response.Data = transform(Data);
            return response;
        }
    }
}