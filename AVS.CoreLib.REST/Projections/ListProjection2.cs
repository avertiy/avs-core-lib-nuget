using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;
using Exception = System.Exception;

namespace AVS.CoreLib.REST.Projections
{
    //to-do this needs to be simplified may be split on 2 classes direct mapping and indirect cause it's quite complex and confusing

    /// <summary>
    /// Array projection helps to map json of array type structure i.e. [...] into <see cref="T"/> projection.
    /// </summary>
    /// <typeparam name="T">projection type</typeparam>
    /// <typeparam name="TItem">list item type </typeparam>
    /// <remarks>
    /// Projection could be either an interface or a concrete type, the same for <see cref="TItem"/>.
    /// In case projection is an interface(abstraction) the concrete type is needed
    /// it could be just an implementation for some complex cases consider to use either
    /// <see cref="MapThrough{TWrapper}"/> or <see cref="MapThrough{TWrapper,TItemType}"/> - an approach when Wrapper allows to add items by abstraction
    /// or consider to use proxy (aka object builder) <see cref="UseProxy{TProxy}"/> in case you need to filter items or do other operations over items
    /// proxy might help to eliminate routine and memory consumption for large arrays fi you do filtering or processing items without a need to keep them as an object 
    /// </remarks>
    public class ListProjection<T, TItem> : Projection 
        where T : class
    {
        protected Action<T> _postProcessAction;
        protected Action<T> _preProcessAction;
        protected Action<TItem> _itemAction;
        protected Func<TItem, bool> _where;
        protected IArrayProxy<T, TItem> _proxy;

        public ListProjection(RestResponse response) : base(response)
        {
        }

        public ListProjection<T, TItem> PreProcess(Action<T> action)
        {
            _preProcessAction = action;
            return this;
        }

        public ListProjection<T, TItem> PostProcess(Action<T> action)
        {
            _postProcessAction = action;
            return this;
        }

        public ListProjection<T, TItem> ForEach(Action<TItem> action)
        {
            _itemAction = action;
            return this;
        }

        public ListProjection<T, TItem> Where(Func<TItem, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        /// <summary>
        /// in case json can't be deserialized into a certain model directly or through wrapper,
        /// <see cref="TProxy"/> comes to the rescue, it has to implement <see cref="IArrayProxy{T,TItem}"/>
        /// <see cref="TProxy"/> will hold array items and then will produce the projection type <see cref="T"/>. 
        /// </summary>
        public ListProjection<T, TItem> UseProxy<TProxy>(Action<TProxy> initialize = null) 
            where TProxy : class, IArrayProxy<T, TItem>, new()
        {
            var builder = new TProxy();
            initialize?.Invoke(builder);
            _proxy = builder;
            return this;
        }

        #region MapThrough
        /// <summary>
        /// Map json into Response{T}.
        /// <see cref="TWrapper"/> is a wrapper over interface(abstraction) of <see cref="T"/> projection
        /// </summary>
        /// <typeparam name="TWrapper"> is a concrete projection type</typeparam>
        public Response<T> MapThrough<TWrapper>() where TWrapper : T, IListWrapper<TItem>, new()
        {
            return MapThrough<TWrapper, TItem>();
        }

        /// <summary>
        /// Map json into Response{T}.
        /// <see cref="TWrapper"/> is a wrapper over interface(abstraction) of <see cref="T"/> projection
        /// <see cref="TItemProjection"/> is a concrete type of interface(abstraction) of <see cref="TItem"/>  
        /// </summary>
        /// <typeparam name="TWrapper"> is a concrete projection type</typeparam>
        /// <typeparam name="TItemProjection"> is a concrete type of array items</typeparam>
        public Response<T> MapThrough<TWrapper, TItemProjection>()
            where TWrapper : T, IListWrapper<TItem>, new()
            where TItemProjection : TItem
        {
            var response = CreateResponse<T, TWrapper>();
            if (response.Success)
            {
                var data = new TWrapper();
                _preProcessAction?.Invoke(data);
                LoadToken<JArray, TWrapper, TItemProjection>(jArray =>
                {
                    foreach (var jToken in jArray)
                    {
                        var item = JsonHelper.Deserialize<TItemProjection>(jToken, typeof(TItemProjection));

                        _itemAction?.Invoke(item);

                        if (_where == null || _where(item))
                            data.Add(item);
                    }
                });
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        } 
        #endregion

        public Task<Response<T>> MapAsync()
        {
            return base.MapAsyncInternal(Map);
        }

        public Response<T> Map()
        {
            if (_proxy == null)
                throw new MapException("Proxy is not initialized", "You might need to use UseProxy<TBuilder>() method first") { JsonText = JsonText};

            var response = MapInternal<T>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = _proxy.Create();
                }
                else
                {
                    LoadToken<JArray, T>(jArray =>
                    {
                        foreach (var jToken in jArray)
                        {
                            var item = JsonHelper.Deserialize<TItem>(jToken, typeof(TItem));

                            _itemAction?.Invoke(item);

                            if (_where == null || _where(item))
                                _proxy.Add(item);
                        }
                    });

                    var data = _proxy.Create();
                    _postProcessAction?.Invoke(data);
                    response.Data = data;
                }
            });

            return response;
        }

        /// <summary>
        /// Map json into Response{T}. T is usually an interface, so we use <see cref="TWrapper"/> as <see cref="T"/> interface implementation,
        /// </summary>
        /// <typeparam name="TWrapper">is an implementation of <see cref="T"/></typeparam>
        public Response<T> Map<TWrapper>(Action<TWrapper, TItem> addItem) where TWrapper : T, new()
        {
            var response = CreateResponse<T, TWrapper>();
            if (response.Success)
            {
                var data = new TWrapper();
                _preProcessAction?.Invoke(data);
                LoadToken<JArray, TWrapper, TItem>(jArray =>
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

        /// <summary>
        /// map into response of List{TItem}
        /// </summary>
        public Response<List<TItem>> MapList<TItemProjection>() where TItemProjection : TItem, new()
        {
            var response = CreateResponse<List<TItem>>();
            if (response.Success)
            {
                _preProcessAction?.Invoke(response.Data as T);
                LoadToken<JArray, TItemProjection, TItem>(jArray =>
                {
                    response.Data = JsonHelper.ParseList<TItem>(jArray, typeof(TItemProjection), _itemAction, _where);
                });
                _postProcessAction?.Invoke(response.Data as T);
            }
            return response;
        }

        public Response<TItem> FirstOrDefault<TItemProjection>() where TItemProjection : TItem, new()
        {
            var response = CreateResponse<TItem, TItemProjection>();
            if (response.Success)
            {
                LoadToken(token =>
                {
                    if (token is JObject jObject)
                    {
                        var item = JsonHelper.Deserialize<TItemProjection>(jObject, typeof(TItemProjection));
                        response.Data = item;
                    }
                    else if (token is JArray jArray)
                    {
                        JToken itemToken = jArray.FirstOrDefault();
                        var item = JsonHelper.Deserialize<TItem>(itemToken, typeof(TItemProjection));
                        _itemAction?.Invoke(item);
                        response.Data = item;
                    }
                    else
                    {
                        throw new MapException($"Unexpected token type: {token.Type}") { JsonText = JsonText };
                    }
                });
            }
            return response;
        }

        public Response<TItem> FirstOrDefault<TItemProjection>(Func<TItemProjection, bool> predicate) where TItemProjection : TItem, new()
        {
            var response = CreateResponse<TItem, TItemProjection>();
            if (response.Success)
            {
                LoadToken(token =>
                {
                    if (token is JObject jObject)
                    {
                        var item = JsonHelper.Deserialize<TItemProjection>(jObject, typeof(TItemProjection));
                        if (predicate(item))
                            response.Data = item;
                    }
                    else if (token is JArray jArray)
                    {
                        foreach (var itemToken in jArray)
                        {
                            var item = JsonHelper.Deserialize<TItemProjection>(itemToken, typeof(TItemProjection));
                            if (predicate(item))
                            {
                                response.Data = item;
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new MapException($"Unexpected token type: {token.Type}") { JsonText = JsonText };
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


        //[Obsolete("use UseProxy method instead")]
        //public ListProjection<T, TItem> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IArrayProxy<T, TItem>, new()
        //{
        //    var builder = new TBuilder();
        //    initialize?.Invoke(builder);
        //    _proxy = builder;
        //    return this;
        //}
    }
}