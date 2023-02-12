using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Represents a keyed projection of json object with key/value pairs
    /// </summary>
    /// <typeparam name="T">The abstract/interface type of keyed collection, for example IBookTicker</typeparam>
    /// <typeparam name="TItem">The concrete type of item values in the collection, for example ExmoMarketData</typeparam>
    public class KeyedProjection<T, TItem> : Projection
    {
        protected Action<T> _postProcessAction;
        protected Action<T> _preProcessAction;
        protected Func<string, string> _preprocessKey;
        protected Action<string, TItem> _itemAction;
        protected Func<string, bool> _whereKey = null;
        protected Func<string, TItem, bool> _where = null;
        protected IKeyedCollectionProxy<T, TItem> _proxy;
        public KeyedProjection(string jsonText, string source = null) : base(jsonText, source)
        {
        }

        #region auxilary methods
        public KeyedProjection<T, TItem> PreProcess(Action<T> action)
        {
            _preProcessAction = action;
            return this;
        }
        public KeyedProjection<T, TItem> PostProcess(Action<T> action)
        {
            _postProcessAction = action;
            return this;
        }

        /// <summary>
        /// preprocess key
        /// </summary>
        public KeyedProjection<T, TItem> Key(Func<string, string> func)
        {
            _preprocessKey = func;
            return this;
        }

        public KeyedProjection<T, TItem> ForEach(Action<string, TItem> action)
        {
            _itemAction = action;
            return this;
        }

        public KeyedProjection<T, TItem> Where(Func<string, bool> predicate)
        {
            _whereKey = predicate;
            return this;
        }

        public KeyedProjection<T, TItem> Where(Func<string, TItem, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        /// <summary>
        /// setup deserialization proxy object <see cref="IKeyedCollectionProxy{T,TData}"/>
        /// </summary>
        public KeyedProjection<T, TItem> UseProxy<TProxy>(Action<TProxy> initialize = null) where TProxy : class, IKeyedCollectionProxy<T, TItem>, new()
        {
            var proxy = new TProxy();
            initialize?.Invoke(proxy);
            _proxy = proxy;
            return this;
        } 
        #endregion

        public Task<Response<T>> MapAsync()
        {
            return base.MapAsyncInternal(Map);
        }

        public Response<T> Map()
        {
            EnsureProxyInitialized();

            var response = MapInternal<T>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = _proxy.Create();
                }

                else
                {

                    LoadToken<JObject, T, TItem>(jObject =>
                    {
                        if (!jObject.HasValues)
                            return;

                        var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                        var itemType = typeof(TItem);
                        foreach (var kp in jObject)
                        {
                            var key = kp.Key;

                            if (_preprocessKey != null)
                                key = _preprocessKey.Invoke(kp.Key);

                            if (_whereKey != null && !_whereKey(key))
                                continue;

                            if (kp.Value != null)
                            {
                                var value = (TItem)serializer.Deserialize(kp.Value.CreateReader(), itemType);
                                if (_where != null && !_where(key, value))
                                    continue;

                                _itemAction?.Invoke(key, value);
                                _proxy.Add(key, value);
                            }
                        }
                    });

                    var data = _proxy.Create();
                    _postProcessAction?.Invoke(data);
                    response.Data = data;
                }
            });
            
            return response;
        }


        #region MapThrough
        /// <summary>
        /// Map json (represented by json object) into Response{T}
        /// where T is an interface(abstraction)
        /// <see cref="TWrapper"/> is a <see cref="IKeyValueWrapper{TKey,TValue}"/> is an implementation of <see cref="T"/>
        /// <see cref="TItem"/> is a value of key-value projection 
        /// </summary>
        public Response<T> MapThrough<TWrapper>()
            where TWrapper : T, IKeyValueWrapper<string, TItem>, new()
        {
            return MapThrough<TWrapper, TItem>();
        }

        /// <summary>
        /// Map json (represented by json object) into Response{T}
        /// where T is an interface(abstraction)
        /// <see cref="TWrapper"/> is a <see cref="IKeyValueWrapper{TKey,TValue}"/> is an implementation of <see cref="T"/>
        /// <see cref="TItem"/> is a value of key-value projection 
        /// </summary>
        public Response<T> MapThrough<TWrapper, TItemProjection>()
            where TWrapper : T, IKeyValueWrapper<string, TItem>, new()
            where TItemProjection : TItem
        {
            var response = CreateResponse<T, TWrapper>();
            if (response.Success)
            {
                var projection = new TWrapper();
                _preProcessAction?.Invoke(projection);

                LoadToken<JObject, TWrapper, TItem>(jObject =>
                {
                    if (!jObject.HasValues)
                        return;

                    var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                    var itemType = typeof(TItemProjection);
                    foreach (KeyValuePair<string, JToken> kp in jObject)
                    {
                        var key = kp.Key;

                        if (_preprocessKey != null)
                            key = _preprocessKey.Invoke(kp.Key);

                        if (_whereKey != null && !_whereKey(key))
                            continue;

                        var value = (TItem)serializer.Deserialize(kp.Value.CreateReader(), itemType);
                        if (_where != null && !_where(key, value))
                            continue;

                        _itemAction?.Invoke(key, value);
                        projection.Add(key, value);
                    }
                });

                _postProcessAction?.Invoke(projection);
                response.Data = projection;
            }
            return response;
        } 
        #endregion

        /// <summary>
        /// Parses json into Response`T 
        /// </summary>
        /// <typeparam name="TWrapper">The concrete type keyed collection of for example BookTicker</typeparam>
        /// <returns>response with data type of T</returns>
        public Response<T> Map<TWrapper>(Action<TWrapper, KeyValuePair<string, TItem>> addItem)
            where TWrapper : T, new()
        {
            var response = CreateResponse<T, TWrapper>();
            if (response.Success)
            {
                var projection = new TWrapper();
                _preProcessAction?.Invoke(projection);

                LoadToken<JObject, TWrapper, TItem>(jObject =>
                {
                    if (!jObject.HasValues)
                        return;

                    var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                    var itemType = typeof(TItem);
                    foreach (KeyValuePair<string, JToken> kp in jObject)
                    {
                        var key = kp.Key;

                        if (_preprocessKey != null)
                            key = _preprocessKey.Invoke(kp.Key);

                        if (_whereKey != null && !_whereKey(key))
                            continue;

                        var value = (TItem)serializer.Deserialize(kp.Value.CreateReader(), itemType);
                        if (_where != null && !_where(key, value))
                            continue;

                        _itemAction?.Invoke(key, value);

                        addItem?.Invoke(projection, new KeyValuePair<string, TItem>(key, value));
                    }
                });

                _postProcessAction?.Invoke(projection);
                response.Data = projection;
            }
            return response;
        }

        private void EnsureProxyInitialized()
        {
            if (_proxy == null)
                throw new AppException("Proxy is not initialized", "You might need to use UseProxy<TProxy>() method first");
        }

        [Obsolete("use UseProxy instead")]
        public KeyedProjection<T, TItem> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IKeyedCollectionProxy<T, TItem>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            _proxy = builder;
            return this;
        }
    }
}