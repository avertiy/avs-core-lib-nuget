using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        protected IKeyedCollectionBuilder<T, TItem> _builder;
        public KeyedProjection(string jsonText, string source = null) : base(jsonText, source)
        {
        }

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

        public KeyedProjection<T, TItem> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IKeyedCollectionBuilder<T, TItem>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            _builder = builder;
            return this;
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        public Response<T> Map()
        {
            if (_builder == null)
                throw new AppException("Builder is not initialized", "You might need to use UseBuilder<TBuilder>() method first");

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = _builder.Create();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {

                LoadToken<JObject, T, TItem>(jObject =>
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
                        _builder.Add(key, value);
                    }
                });

                var data = _builder.Create();
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        /// <summary>
        /// Parses json into Response`T 
        /// </summary>
        /// <typeparam name="TProjection">The concrete type keyed collection of for example BookTicker</typeparam>
        /// <returns>response with data type of T</returns>
        public Response<T> Map<TProjection>(Action<TProjection, KeyValuePair<string, TItem>> addItem) where TProjection : T, new()
        {
            var response = CreateResponse<T, TProjection>();
            if (response.Success)
            {
                var projection = new TProjection();
                _preProcessAction?.Invoke(projection);

                LoadToken<JObject, TProjection, TItem>(jObject =>
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
    }
}