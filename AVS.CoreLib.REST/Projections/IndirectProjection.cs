﻿#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Represent a special case called indirect projection.
    /// This is a case when TResult (abstraction) and T (projection type) are not linked to each other with inheritance,
    /// and TContainer is produced with a help of TProxy <seealso cref="IProxy{T, TContainer}"/>
    /// <code>
    ///   //indirect projection through proxy
    ///   var projection = restResponse.IndirectProjection{IOrderResult, BinanceOrder}();
    ///   Response{IOrder} response = projection.MapWith{OrderBuilder}();
    /// </code>
    /// </summary>
    public class IndirectProjection<TResult, T> : ProjectionBase
    {
        protected Action<T>? _preProcess;
        protected Action<T>? _postProcess;
        protected Action<TResult>? _postProcess2;

        [DebuggerStepThrough]
        public IndirectProjection(RestResponse response) : base(response)
        {
        }

        public IndirectProjection<TResult, T> PreProcess(Action<T> action)
        {
            _preProcess = action;
            return this;
        }

        public IndirectProjection<TResult, T> PostProcess(Action<T> action)
        {
            _postProcess = action;
            return this;
        }

        public IndirectProjection<TResult, T> PostProcess(Action<TResult> action)
        {
            _postProcess2 = action;
            return this;
        }

        public TResult? InspectDeserialization<TProxy>(Action<JToken, IProxy<T, TResult>> inspect, out Exception? err)
             where TProxy : class, IProxy<T, TResult>, new()
        {
            try
            {
                var proxy = new TProxy();
                var token = LoadToken<JToken>(JsonText);
                inspect(token, proxy);
                NewtonsoftJsonHelper.Populate(token, proxy);
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

        public Response<TResult> MapWith<TProxy>(Action<TProxy>? configure = null) where TProxy : class, IProxy<T, TResult>, new()
        {
            try
            {
                var response = Response.Create<TResult>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                var proxy = new TProxy();
                configure?.Invoke(proxy);

                if (IsEmpty)
                {
                    response.Data = proxy!.Create();
                }
                else
                {
                    var obj = Activator.CreateInstance<T>();
                    _preProcess?.Invoke(obj);
                    var token = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(token, obj);
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

        public Response<TResult> MapWith(IProxy<T, TResult> proxy)
        {
            try
            {
                var response = Response.Create<TResult>(Source, content: JsonText, Error, Request);
                if (HasError)
                    return response;

                if (IsEmpty)
                {
                    response.Data = proxy!.Create();
                }
                else
                {
                    var obj = Activator.CreateInstance<T>();
                    _preProcess?.Invoke(obj);
                    var token = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(token, obj);
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

        public Response<TResult> MapWith<TMapper>() where TMapper : IMapper<T, TResult>, new()
        {
            try
            {
                var response = Response.Create<TResult>(Source, content: JsonText, Error, Request);

                if (HasError)
                    return response;
                
                if (!IsEmpty)
                {
                    var obj = Activator.CreateInstance<T>();
                    _preProcess?.Invoke(obj);
                    var token = LoadToken<JToken>(JsonText);
                    NewtonsoftJsonHelper.Populate(token, obj);
                    _postProcess?.Invoke(obj);

                    var mapper = new TMapper();
                    var data = mapper.Map(obj);
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

    //public class IndirectProjection2<TContainer, T> : ProjectionBase
    //{
    //    protected Action<T>? _preProcess;
    //    protected Action<T>? _postProcess;
    //    protected Action<TContainer>? _postProcess2;

    //    [DebuggerStepThrough]
    //    public IndirectProjection2(RestResponse response) : base(response)
    //    {
    //    }
    //}
}