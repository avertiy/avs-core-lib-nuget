#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// ListProjection{T} represent json mapper 
    /// when json array [..] needs to be deserialized into <see cref="List{T}"/>.
    /// <code>
    ///    // 1. T is a concrete type
    ///    var projection = response.ListProjection{Order}();
    ///    Response{List{Order}} list = projection.Map();
    /// 
    ///    // 2. T is an abstraction/interface
    ///    var projection = response.ListProjection{IOrder}();
    ///    Response{List{IOrder}} list = projection.Map{Order}();
    ///     
    ///    // 3. indirect projection via proxy (concreate types)
    ///    var projection = response.ListProjection{Ticker}();
    ///    Response{List{Ticker}}  = projection.MapWith{TickerListBuilder}();    
    ///    
    ///    // 4. indirect projection via proxy (abstraction)
    ///    var projection = response.ListProjection{ITicker}();
    ///    Response{List{Ticker}} volume = projection.MapWith{TickerListBuilder, BinanceTicker}();  
    /// </code>
    /// </summary>
    public class ListProjection<T> : ProjectionBase
    {
        private Func<T, bool>? _where;
        private Action<T>? _itemAction;
        private Action<List<T>>? _postProcess;

        public ListProjection(RestResponse response) : base(response)
        {
        }

        public ListProjection<T> Where(Func<T, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public ListProjection<T> ForEach(Action<T> action)
        {
            _itemAction = action;
            return this;
        }

        public ListProjection<T> PostProcess(Action<List<T>> action)
        {
            _postProcess = action;
            return this;
        }

        public Response<List<T>> Map(int take = 0)
        {
            try
            {
                var response = Response.Create<List<T>>(Source, Error, Request);
                if (HasError)
                    return response;

                if (IsEmpty)
                {
                    response.Data = new List<T>();
                }
                else
                {
                    var jArray = LoadToken<JArray>();
                    var list = new List<T>(jArray.Count);
                    var type = typeof(T);
                    var i = 0;
                    foreach (var jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<T>(jToken, type);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            list.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }

                        if (take > 0 && i == take)
                            break;
                    }

                    _postProcess?.Invoke(list);
                    response.Data = list;
                }

                return response;
            }
            catch (MapException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MapException(ex, this);
            }
        }

        public Response<List<T>> Map<TType>(int take = 0)
            where TType : T
        {
            try
            {
                var response = Response.Create<List<T>>(Source, Error, Request);
                if (HasError)
                    return response;

                if (IsEmpty)
                {
                    response.Data = new List<T>();
                }
                else
                {
                    var jArray = LoadToken<JArray>();
                    var list = new List<T>(jArray.Count);
                    var type = typeof(TType);
                    var i = 0;
                    foreach (var jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<TType>(jToken, type);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            list.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }

                        if(take > 0 && i == take)
                            break;
                    }

                    _postProcess?.Invoke(list);
                    response.Data = list;
                }

                return response;
            }            
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.{nameof(MapWith)} failed - {ex.Message}", ex);
            }
        }

        public Response<List<T>> MapWith<TProxy>(Action<TProxy>? configure = null, int take = 0) where TProxy : IProxy<T, List<T>>, new()
        {
            return MapWith<TProxy, T>(configure, take);
        }

        public Response<List<T>> MapWith<TProxy, TType>(Action<TProxy>? configure = null, int take =0)            
            where TProxy : IProxy<T, List<T>>, new()
            where TType : T
        {
            try
            {
                var response = Response.Create<List<T>>(Source, Error, Request);
                if (HasError)
                    return response;

                var proxy = new TProxy();
                configure?.Invoke(proxy);
                if (IsEmpty)
                {
                    response.Data = proxy.Create();
                }
                else
                {
                    var jArray = LoadToken<JArray>();
                    var i = 0;
                    var itemType = typeof(TType);
                    foreach (var jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<T>(jToken, itemType);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            proxy!.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {itemType.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }

                        if (take > 0 && i == take)
                            break;
                    }

                    var data = proxy!.Create();
                    _postProcess?.Invoke(data);
                    response.Data = data;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.{nameof(MapWith)} failed - {ex.Message}", ex);
            }
        }
    }
}