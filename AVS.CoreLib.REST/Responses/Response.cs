using System;
using System.Diagnostics;
using System.Net;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.REST.Responses
{
    //TODO this stuff with Response looks too complicated, this needs to be simplified to basic stuff without props etc. things

    public class Response : ResponseBase
    {
        #region Create static methods
        public static Response Create(string source, string error = null)
        {
            return new Response() { Source = source, Error = error };
        }

        public static Response<T> Create<T>(string source, string error)
        {
            return new Response<T>() { Source = source, Error = error};
        }

        public static Response<T> Create<T>(T data, string source, string error)
        {
            return new Response<T>() { Data = data, Source = source, Error = error };
        }

        public static Response<T> OK<T>(T data, string source, string error = null)
        {
            return new Response<T>() { Data = data, Source = source, Error = error };
        }

        public static IResponse<T> Failed<T>(Exception ex, string source)
        {
            return new Response<T>() { Source = source, Error = ex.Message };
        }

        #endregion
    }

    [DebuggerDisplay("{ToString()}")]
    public class Response<T> : IResponse<T>
    {
        public string Source { get; set; }
        public string Error { get; set; }
        public T Data { get; set; }
        public virtual bool Success => Error == null;

        public override string ToString()
        {
            if (Success)
            {
                var count = Data.GetCount();
                return count.HasValue ? $"Response<{typeof(T).Name}> - OK (#{count})" : $"Response<{typeof(T).Name}> - OK";
            }

            return $"Response<{typeof(T).Name}> - Failed ({Error})";
        }
    }
}

