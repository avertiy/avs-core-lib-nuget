using System;
using System.Diagnostics;
using System.Net;
using AVS.CoreLib.Abstractions.Responses;

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
            return Success ? $"Response<{typeof(T).Name}> - OK" : $"Response<{typeof(T).Name}> - Failed ({Error})";
        }
    }

    

    //[DebuggerDisplay("{ToString()}")]
    //public class Response<T> : Response, IResponse<T>
    //{
    //    public Response() { }

    //    public T Data { get; set; }

    //    public TResponse To<TResponse>()
    //        where TResponse : IResponse<T>, new()
    //    {
    //        return new TResponse { Error = Error, Source = Source, Data = Data };
    //    }

    //    public virtual TResponse To<TResponse>(Action<T, TResponse> onSuccess)
    //        where TResponse : IResponse, new()
    //    {
    //        var result = new TResponse { Error = Error, Source = Source };
    //        if (Success)
    //        {
    //            onSuccess(this.Data, result);
    //        }
    //        return result;
    //    }

    //    public virtual TResponse OnSuccess<TResponse>(Func<T, TResponse> onSuccess)
    //        where TResponse : IResponse, new()
    //    {
    //        if (Success)
    //            return onSuccess(Data);
    //        else
    //            return new TResponse { Error = Error, Source = Source };
    //    }

    //    public override string ToString()
    //    {
    //        return Success ? $"Response<{typeof(T).Name}> - OK" : $"Response<{typeof(T).Name}> - Fail [{Error}]";
    //    }

    //    public override dynamic GetData()
    //    {
    //        return Data;
    //    }
    //}


}

