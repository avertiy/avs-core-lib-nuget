using System;
using System.Diagnostics;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Responses;

namespace AVS.CoreLib.REST.Responses
{
    public class Response : ResponseBase, IPropsContainer
    {
        /// <summary>
        /// Props might contain some metadata e.g. exchange name
        /// </summary>
        public virtual dynamic Props { get; set; }

        public virtual bool ShouldSerializeProps()
        {
            return Props != null;
        }

        public virtual dynamic GetData()
        {
            return null;
        }

        #region Create static methods
        public static Response Create(string source = null, string error = null, dynamic props = null)
        {
            return ResponseFactory.Instance.Create(source, error, props);
        }

        public static Response<T> Create<T>(string source = null, string error = null, dynamic props = null)
        {
            return ResponseFactory.Instance.Create<T>(source, error, props);
        }

        public static Response<T> Create<T>(T data, string source = null, string error = null, dynamic props = null)
        {
            return ResponseFactory.Instance.Create<T>(data, source, error, props);
        }

        public static IResponse<T> Create<T>(T data, Exception ex)
        {
            return ResponseFactory.Instance.Create<T>(data, ex);
        }

        public static Response<T> CreateCopy<T>(Response<T> response)
        {
            return ResponseFactory.Instance.Create<T>(response.Data, response.Source, response.Error, response.Props);
        }
        #endregion
    }

    [DebuggerDisplay("{ToString()}")]
    public class Response<T> : Response, IResponse<T>
    {
        public Response() { }

        public T Data { get; set; }

        public virtual TResponse To<TResponse>(Action<T, TResponse> onSuccess)
            where TResponse : IResponse, new()
        {
            var result = new TResponse { Error = Error, Source = Source };
            if (Success)
            {
                onSuccess(this.Data, result);
            }
            return result;
        }

        public virtual TResponse OnSuccess<TResponse>(Func<T, TResponse> onSuccess)
            where TResponse : IResponse, new()
        {
            if (Success)
                return onSuccess(Data);
            else
                return new TResponse { Error = Error, Source = Source };
        }

        public override string ToString()
        {
            return Success ? $"Response<{typeof(T).Name}> - OK" : $"Response<{typeof(T).Name}> - Fail [{Error}]";
        }

        public override dynamic GetData()
        {
            return Data;
        }
    }
}

