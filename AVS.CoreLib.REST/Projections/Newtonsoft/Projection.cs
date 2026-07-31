#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.REST.Exceptions;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Projection{T} helps to map json into <see cref="Response{T}"/>
    /// <code>
    ///   // use cases:
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
    [Obsolete("Use Proj<T> impl.")]
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
    /// Projection{TAbstraction, TImplementation} helps to map json into  <see cref="Response{TAbstraction}"/>
    /// <code>
    ///   // use cases:
    ///   var projection = restResponse.Projection{IOrder, Order}(); // where Order : IOrder
    ///   Response{IOrder} response = projection.Map();
    /// </code>
    /// </summary>
    public class Projection<TAbstraction, TImplementation> : ProjectionBase where TImplementation : TAbstraction
    {
        protected Action<TImplementation>? _postProcess;
        protected Action<TImplementation>? _preProcess;

        [DebuggerStepThrough]
        public Projection(RestResponse response) : base(response)
        {
        }

        public Projection<TAbstraction, TImplementation> PreProcess(Action<TImplementation> action)
        {
            _preProcess = action;
            return this;
        }

        public Projection<TAbstraction, TImplementation> PostProcess(Action<TImplementation> action)
        {
            _postProcess = action;
            return this;
        }

        public TAbstraction? InspectDeserialization(Action<JToken, TImplementation> inspect, out Exception? err)
        {
            try
            {
                var obj = Activator.CreateInstance<TImplementation>();
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

        /// <summary>
        /// Deserializes json text to into <typeparamref name="TImplementation"/> and returns result as <see cref="Response{T}"/>
        /// </summary>
        public Response<TAbstraction> Map()
        {
            try
            {
                var response = Response.Create<TAbstraction>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var obj = Activator.CreateInstance<TImplementation>();
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