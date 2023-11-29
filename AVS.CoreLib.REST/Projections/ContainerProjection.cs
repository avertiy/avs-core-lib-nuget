#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Represent a json mapper that map json array structure
    /// or key-value (json object) structure e.g. { "key": value, .. } or { "key": {..}, .. }, or { "key": [..], .. }
    /// T is a container 
    /// <code>
    ///     // json array structures:
    ///     // 1. when container implements ICollection{TItem}
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}();
    ///     
    ///     // 2. when container does not implement ICollection{TItem}
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}((x,item) => x.Add(item));
    ///     
    ///     // 3. when we use proxy to produce T data
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     // OpenOrdersBuilder: IListProxy{IOpenOrders, BinanceOrder} see IListProxy{out T, in TItem}
    ///     Response{IOpenOrders} result = projection.MapWith{OpenOrdersBuilder}();
    ///     
    ///     // json object structures (aka keyed projections):
    ///     // 1. when container implements IContainer{string, TItem}
    ///     var projection = response.ContainerProjection{IOpenOrders, List{ExmoOrder}}()
    ///     Response{IOpenOrders} result = projection.MapObject{OpenOrders}();
    ///     
    ///     // 2. when proxy type is used to produce T data
    ///     var projection = response.ContainerProjection{IOpenOrders, List{ExmoOrder}}()
    ///     Response{IOpenOrders} result = projection.MapObject{OpenOrders}();
    /// </code>
    /// </summary>
    /// <typeparam name="T">container usually an interface (abstraction)</typeparam>
    /// <typeparam name="TItem">concrete type to deserialize array item when mapping json array or value when mapping key-value structure</typeparam>
    public class ContainerProjection<T, TItem> : ProjectionBase
    {
        // common
        protected Action<T>? _preProcess;
        protected Action<T>? _postProcess;
        protected Func<TItem, bool>? _where;
        protected Action<TItem>? _itemAction;

        // object mapping (keyed cases)
        protected Action<string, TItem>? _keyValueAction;
        protected Func<string, string>? _preprocessKey;
        protected Func<string, bool>? _whereKey = null;
        protected Func<(string Key,TItem Value), bool>? _whereKeyValue = null;

        public ContainerProjection(RestResponse response) : base(response)
        {
        }

        #region Configure projection
        public ContainerProjection<T, TItem> PreProcess(Action<T> action)
        {
            _preProcess = action;
            return this;
        }

        public ContainerProjection<T, TItem> PostProcess(Action<T> action)
        {
            _postProcess = action;
            return this;
        }
        public ContainerProjection<T, TItem> Where(Func<TItem, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public ContainerProjection<T, TItem> ForEach(Action<TItem> action)
        {
            _itemAction = action;
            return this;
        }


        public ContainerProjection<T, TItem> ForEach(Action<string, TItem> action)
        {
            _keyValueAction = action;
            return this;
        }

        /// <summary>
        /// preprocess key
        /// </summary>
        public ContainerProjection<T, TItem> Key(Func<string, string> func)
        {
            _preprocessKey = func;
            return this;
        }

        public ContainerProjection<T, TItem> Where(Func<string, bool> predicate)
        {
            _whereKey = predicate;
            return this;
        }

        public ContainerProjection<T, TItem> Where(Func<(string Key,TItem Value), bool> predicate)
        {
            _whereKeyValue = predicate;
            return this;
        }

        #endregion

        /// <summary>
        /// Map json array when <typeparamref name="TContainer"/> implements <see cref="ICollection{TItem}"/>
        /// </summary>
        public Response<T> Map<TContainer>() 
            where TContainer : T, ICollection<TItem>, new()            
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;

            try
            {
                var data = new TContainer();
                _preProcess?.Invoke(data);

                if (IsEmpty)
                {
                    response.Data = data;
                }
                else
                {
                    var jArray = LoadToken<JArray>();
                    var i = 0;
                    var type = typeof(TItem);
                    foreach (JToken jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<TItem>(jToken, type);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            data.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i}", ex);
                        }
                    }
                    response.Data = data;
                }

                _postProcess?.Invoke(data);
                return response;
            }catch(Exception ex)
            {
                throw new MapException($"Mapping failed - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map json array when TConainer can't implement <see cref="ICollection{TItem}"/>
        /// </summary>
        public Response<T> Map<TContainer>(Action<TContainer, TItem> add) where TContainer : T, new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;

            try
            {

                var container = new TContainer();
                _preProcess?.Invoke(container);

                if (IsEmpty)
                {
                    response.Data = container;
                }
                else
                {
                    var jArray = LoadToken<JArray>();
                    var i = 0;
                    var type = typeof(TItem);
                    foreach (var jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<TItem>(jToken, type);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            add(container, item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i}", ex);
                        }
                    }
                    response.Data = container;
                }

                _postProcess?.Invoke(container);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"Mapping failed - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map json array with proxy/builder 
        /// </summary>
        public Response<T> MapWith<TProxy>(Action<TProxy>? configure = null)
            where TProxy : IProxy<T>, IContainer<TItem>, new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;

            try
            {
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
                    var type = typeof(TItem);
                    foreach (JToken jToken in jArray)
                    {
                        i++;
                        try
                        {
                            var item = JsonHelper.Deserialize<TItem>(jToken, type);
                            if (item == null)
                                continue;

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;
                            
                            proxy.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i}", ex);
                        }
                    }
                    response.Data = proxy.Create();
                }

                _postProcess?.Invoke(response.Data);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"Mapping failed - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map json object (key-value structure)
        /// </summary>
        public Response<T> MapObject<TContainer>()
            where TContainer : IContainer<string, TItem>, T, new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;

            try
            {
                var data = new TContainer();
                _preProcess?.Invoke(data);

                if (!IsEmpty)
                {
                    var jObject = LoadToken<JObject>();
                    if (jObject.HasValues)
                    {
                        var type = typeof(TItem);
                        foreach (KeyValuePair<string, JToken?> kp in jObject)
                        {
                            ProcessKeyValue(data, kp.Key, kp.Value, type);
                        }

                    }
                }

                response.Data = data;
                _postProcess?.Invoke(data);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"Mapping failed - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map json object (key-value structure) via proxy
        /// </summary>
        public Response<T> MapObjectWith<TProxy>(Action<TProxy>? configure = null)
            where TProxy : IProxy<T>, IContainer<string, TItem>, new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;

            try
            {
                var proxy = new TProxy();
                configure?.Invoke(proxy);
                if (!IsEmpty)
                {
                    var jObject = LoadToken<JObject>();
                    if (jObject.HasValues)
                    {
                        var type = typeof(TItem);
                        foreach (KeyValuePair<string, JToken?> kp in jObject)
                        {
                            ProcessKeyValue(proxy, kp.Key, kp.Value, type);
                        }
                    }
                }

                var data = proxy.Create();
                response.Data = data;
                _postProcess?.Invoke(data);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"Mapping failed - {ex.Message}", ex);
            }
        }

        private void ProcessKeyValue(IContainer<string, TItem> data, string key, JToken? jToken, Type itemType)            
        {
            if (jToken == null)
                throw new NullReferenceException($"value is null [key={key}]");

            try
            {
                if (_preprocessKey != null)
                    key = _preprocessKey(key);

                if (_whereKey != null && !_whereKey(key))
                    return;

                var item = JsonHelper.Deserialize<TItem>(jToken, itemType);

                if (item == null)
                    throw new NullReferenceException($"Deserialized {itemType.Name} value is null [key={key}; token:{jToken}]");

                _itemAction?.Invoke(item);

                if (_where != null && !_where(item))
                    return;

                if (_whereKeyValue != null && !_whereKeyValue((key, item)))
                    return;

                _keyValueAction?.Invoke(key, item);
                
                data.Add(key, item);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process {itemType.Name} item at key={key}", ex);
            }
        }
    } 
}