using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Json;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    public class ArrayProjection<T, TItem> : Projection where T : class
    {
        protected Action<T> _postProcessAction;
        protected Action<T> _preProcessAction;
        protected Action<TItem> _itemAction;
        protected Func<TItem, bool> _where;
        protected IArrayProxy<T, TItem> _proxy;

        public ArrayProjection(string jsonText, string source = null) : base(jsonText, source)
        {
        }

        public ArrayProjection<T, TItem> PreProcess(Action<T> action)
        {
            _preProcessAction = action;
            return this;
        }

        public ArrayProjection<T, TItem> PostProcess(Action<T> action)
        {
            _postProcessAction = action;
            return this;
        }

        public ArrayProjection<T, TItem> ForEach(Action<TItem> action)
        {
            _itemAction = action;
            return this;
        }

        public ArrayProjection<T, TItem> Where(Func<TItem, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public ArrayProjection<T, TItem> UseProxy<TProxy>(Action<TProxy> initialize = null) 
            where TProxy : class, IArrayProxy<T, TItem>, new()
        {
            var builder = new TProxy();
            initialize?.Invoke(builder);
            _proxy = builder;
            return this;
        }

        [Obsolete("use UseProxy method instead")]
        public ArrayProjection<T, TItem> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IArrayProxy<T, TItem>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            _proxy = builder;
            return this;
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        public virtual Response<T> Map()
        {
            if (_proxy == null)
                throw new AppException("Proxy is not initialized", "You might need to use UseProxy<TBuilder>() method first");

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = _proxy.Create();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                LoadToken<JArray, T>(jArray =>
                {
                    foreach (JToken itemToken in jArray)
                    {
                        var item = JsonHelper.Deserialize<TItem>(itemToken, typeof(TItem));

                        _itemAction?.Invoke(item);

                        if (_where == null || _where(item))
                            _proxy.Add(item);
                    }
                });

                var data = _proxy.Create();
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public virtual Response<T> Map<TProjection>(Action<TProjection, TItem> addItem) where TProjection : T, new()
        {
            var response = CreateResponse<T, TProjection>();
            if (response.Success)
            {
                var data = new TProjection();
                _preProcessAction?.Invoke(data);
                LoadToken<JArray, TProjection, TItem>(jArray =>
                {
                    foreach (JToken itemToken in jArray)
                    {
                        var item = JsonHelper.Deserialize<TItem>(itemToken, typeof(TItem));

                        _itemAction?.Invoke(item);

                        if (_where == null || _where(item))
                            addItem?.Invoke(data, item);
                    }
                });
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public virtual Response<List<TItem>> MapList<TProjection>() where TProjection : TItem, new()
        {
            var response = CreateResponse<List<TItem>>();
            if (response.Success)
            {
                _preProcessAction?.Invoke(response.Data as T);
                LoadToken<JArray, TProjection, TItem>(jArray =>
                {
                    response.Data = JsonHelper.ParseList<TItem>(jArray, typeof(TProjection), _itemAction, _where);
                });
                _postProcessAction?.Invoke(response.Data as T);
            }
            return response;
        }

        public virtual Response<TItem> FirstOrDefault<TProjection>() where TProjection : TItem, new()
        {
            var response = CreateResponse<TItem, TProjection>();
            if (response.Success)
            {
                LoadToken(token =>
                {
                    if (token is JObject jObject)
                    {
                        var item = JsonHelper.Deserialize<TProjection>(jObject, typeof(TProjection));
                        response.Data = item;
                    }
                    else if (token is JArray jArray)
                    {
                        JToken itemToken = jArray.FirstOrDefault();
                        var item = JsonHelper.Deserialize<TItem>(itemToken, typeof(TProjection));
                        _itemAction?.Invoke(item);
                        response.Data = item;
                    }
                    else
                    {
                        throw new MapException($"Unexpected token type: {token.Type}");
                    }
                });
            }
            return response;
        }

        public virtual Response<TItem> FirstOrDefault<TProjection>(Func<TProjection, bool> predicate) where TProjection : TItem, new()
        {
            var response = CreateResponse<TItem, TProjection>();
            if (response.Success)
            {
                LoadToken(token =>
                {
                    if (token is JObject jObject)
                    {
                        var item = JsonHelper.Deserialize<TProjection>(jObject, typeof(TProjection));
                        if (predicate(item))
                            response.Data = item;
                    }
                    else if (token is JArray jArray)
                    {
                        foreach (var itemToken in jArray)
                        {
                            var item = JsonHelper.Deserialize<TProjection>(itemToken, typeof(TProjection));
                            if (predicate(item))
                            {
                                response.Data = item;
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new MapException($"Unexpected token type: {token.Type}");
                    }
                });
            }
            return response;
        }

        public object InspectDeserialization(Action<JArray, IArrayProxy<T, TItem>> inspect, Action<int, JToken> inspectItem, out Exception err)
        {
            EnsureProxyInitialized();
            try
            {
                LoadToken<JArray, T>(jArray =>
                {
                    inspect(jArray, _proxy);
                    var i = 0;
                    foreach (var itemToken in jArray)
                    {
                        inspectItem(i, itemToken);
                        i++;
                        var item = JsonHelper.Deserialize<TItem>(itemToken, typeof(TItem));

                        _itemAction?.Invoke(item);

                        if (_where == null || _where(item))
                            _proxy.Add(item);
                    }
                });
                var data = _proxy.Create();
                err = null;
                return data;
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }
        }

        private void EnsureProxyInitialized()
        {
            if (_proxy == null)
                throw new AppException("Proxy is not initialized", "You might need to use UseProxy<TProxy>() method first");
        }
    }
}