#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Guards;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Represent a special case of an indirect projection     
    /// Its a bit complex but think of it as <see cref="Projection{T,TType}"/>where 
    /// two types do not relate to each other with inheritance i.e. TType niether inherit neither implement T
    /// <code>
    ///   //indirect projection through proxy
    ///   var projection = restResponse.IndirectProjection{IOrderResult, BinanceOrder}();
    ///   Response{IOrder} response = projection.MapWith{OrderBuilder}();
    /// </code>
    /// </summary>
    public class IndirectProjection<T, TType> : ProjectionBase
    {
        protected Action<TType>? _preProcess;
        protected Action<TType>? _postProcess;
        protected Action<T>? _postProcess2;

        [DebuggerStepThrough]
        public IndirectProjection(RestResponse response) : base(response)
        {
        }

        public IndirectProjection<T, TType> PreProcess(Action<TType> action)
        {
            _preProcess = action;
            return this;
        }

        public IndirectProjection<T, TType> PostProcess(Action<TType> action)
        {
            _postProcess = action;
            return this;
        }

        public IndirectProjection<T, TType> PostProcess(Action<T> action)
        {
            _postProcess2 = action;
            return this;
        }

        public T? InspectDeserialization<TProxy>(Action<JToken, IProxy<TType, T>> inspect, out Exception? err)
             where TProxy : class, IProxy<TType, T>, new()
        {
            try
            {
                var proxy = new TProxy();
                var token = LoadToken<JToken>();
                inspect(token, proxy);
                JsonHelper.Populate(token, proxy);
                var data = proxy!.Create();
                err = null;
                return data;
            }
            catch (Exception ex)
            {
                err = ex;
                return default;
            }
        }

        public Response<T> MapWith<TProxy>() where TProxy : class, IProxy<TType, T>, new()
        {
            try
            {
                var response = Response.Create<T>(Source, Error, Request);
                if (HasError)
                    return response;

                var proxy = new TProxy();
                if (IsEmpty)
                {
                    response.Data = proxy!.Create();
                }
                else
                {
                    var obj = Activator.CreateInstance<TType>();
                    _preProcess?.Invoke(obj);
                    var token = LoadToken<JToken>();
                    JsonHelper.Populate(token, obj);
                    _postProcess?.Invoke(obj);
                    proxy!.Add(obj);
                    var data = proxy.Create();
                    _postProcess2?.Invoke(data);
                    response.Data = data;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }
    }
}