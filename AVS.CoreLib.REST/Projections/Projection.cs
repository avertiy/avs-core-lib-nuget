#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Json;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Represent a simple type projection to deserialize json into T type 
    /// takes <see cref="RestResponse"/> as an input and produces <see cref="Response{T}"/> result
    /// <code>
    ///   // 1. T is a concrete type (direct projection)
    ///   var projection = restResponse.Projection{Order}();
    ///   Response{Order} response = projection.Map();
    ///   
    ///   // 2. T is an abstraction (direct projection)
    ///   var projection = restResponse.Projection{IOrder}();
    ///   Response{Order} response = projection.Map{BinanceOrder}();
    ///   
    ///   // 3. T is an abstraction/interface (projection via proxy)
    ///   var projection = restResponse.Projection{IOrderBook}();
    ///   Response{IOrderBook} response = projection.MapWith{OrderBookBuilder}();
    /// </code>
    /// </summary>
    public class Projection<T> : ProjectionBase
    {
        protected Action<T>? _preProcess;
        protected Action<T>? _postProcess;

        [DebuggerStepThrough]
        public Projection(RestResponse response) : base(response)
        {
        }

        public Projection<T> PreProcess(Action<T> action)
        {
            _preProcess = action;
            return this;
        }

        public Projection<T> PostProcess(Action<T> action)
        {
            _postProcess = action;
            return this;
        }

        public Projection<T> PostProcess<TType>(Action<TType> action) where TType : T
        {
            _postProcess = x => action((TType)x!);
            return this;
        }

        public T? InspectDeserialization(Action<JToken, T> inspect, out Exception? err)
        {
            try
            {
                var obj = Activator.CreateInstance<T>();
                var jToken = LoadToken<JToken>(JsonText);
                inspect(jToken, obj);
                NewtonsoftJsonHelper.Populate(jToken, obj);
                err = null;
                return obj;
            }
            catch (Exception ex)
            {
                err = ex;
                return default;
            }
        }

        public Response<T> Map()
        {
            try
            {
                var response = Response.Create<T>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var obj = Activator.CreateInstance<T>();
                _preProcess?.Invoke(obj);

                if (IsEmpty)
                {
                    response.Data = obj;
                }
                else
                {
                    //JsonHelper.Populate(obj, JsonText, _selectTokenPath);

                    var token = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(token, obj);
                    _postProcess?.Invoke(obj);
                    response.Data = obj;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }

        public Response<T> Map<TType>() where TType : T, new()
        {
            try
            {
                var response = Response.Create<T>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var obj = new TType();
                _preProcess?.Invoke(obj);

                if (IsEmpty)
                {
                    response.Data = obj;
                }
                else
                {
                    var token = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(token, obj);
                    _postProcess?.Invoke(obj);
                    response.Data = obj;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }

        public Response<T> MapWith<TProxy>(Action<TProxy>? configure = null) where TProxy : class, IProxy<T>, new()
        {
            try
            {
                var response = Response.Create<T>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var proxy = new TProxy();
                configure?.Invoke(proxy);

                if (!IsEmpty)
                {
                    var jToken = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(jToken, proxy);
                }

                var obj = proxy.Create();
                _postProcess?.Invoke(obj);
                response.Data = obj;
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }
    }

    /// <summary>
    /// Represent json projection when T is an abstraction, TImpl is an implementation of T
    /// <code>
    ///   // simple direct projection
    ///   var projection = restResponse.Projection{IOrder, Order}(); // where Order : IOrder
    ///   Response{IOrder} response = projection.Map();
    /// </code>
    /// </summary>
    public class Projection<T, TImpl> : ProjectionBase where TImpl : T
    {
        protected Action<TImpl>? _postProcess;
        protected Action<TImpl>? _preProcess;

        [DebuggerStepThrough]
        public Projection(RestResponse response) : base(response)
        {
        }

        public Projection<T, TImpl> PreProcess(Action<TImpl> action)
        {
            _preProcess = action;
            return this;
        }

        public Projection<T, TImpl> PostProcess(Action<TImpl> action)
        {
            _postProcess = action;
            return this;
        }

        public T? InspectDeserialization(Action<JToken, TImpl> inspect, out Exception? err)
        {
            try
            {
                var obj = Activator.CreateInstance<TImpl>();
                var jToken = LoadToken<JToken>(JsonText);
                inspect(jToken, obj);
                NewtonsoftJsonHelper.Populate(jToken, obj);
                err = null;
                return obj;
            }
            catch (Exception ex)
            {
                err = ex;
                return default;
            }
        }

        public Response<T> Map()
        {
            try
            {
                var response = Response.Create<T>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var obj = Activator.CreateInstance<TImpl>();
                _preProcess?.Invoke(obj);

                if (!IsEmpty)
                {
                    var jToken = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(jToken, obj);
                }

                _postProcess?.Invoke(obj);
                response.Data = obj;

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }
    }

    
}