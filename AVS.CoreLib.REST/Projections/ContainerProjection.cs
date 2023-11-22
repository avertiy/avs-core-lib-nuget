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
    /// T is a container <see cref="ICollection{TItem}"/>
    /// <code>
    ///     // 1. when container implements ICollection{TItem}
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}();
    ///     
    ///     // 2. when container does not implement ICollection{TItem}
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}((x,item) => x.Add(item));
    ///     
    ///     // 3. when we use proxy to produce container 
    ///     var projection = response.ContainerProjection{IOpenOrders, BinanceOrder}()
    ///     // OpenOrdersBuilder: IListProxy{IOpenOrders, BinanceOrder} see IListProxy{out T, in TItem}
    ///     Response{IOpenOrders} result = projection.MapWith{OpenOrdersBuilder}();
    /// </code>
    /// </summary>
    public class ContainerProjection<T, TItem> : ProjectionBase
    {
        protected Action<T>? _preProcess;
        protected Action<T>? _postProcess;
        protected Func<TItem, bool>? _where;
        protected Action<TItem>? _itemAction;

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
        #endregion

        /// <summary>
        /// when TConainer implements <see cref="ICollection{TItem}"/>
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

                            if (_where != null && !_where(item))
                                continue;

                            _itemAction?.Invoke(item);
                            data.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }
                    }
                    response.Data = data;
                }

                _postProcess?.Invoke(data);
                return response;
            }catch(Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.Map<{typeof(TContainer).Name}>() failed - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// when TConainer can't implement <see cref="ICollection{TItem}"/>
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

                            if (_where != null && !_where(item))
                                continue;

                            _itemAction?.Invoke(item);
                            add(container, item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }
                    }
                    response.Data = container;
                }

                _postProcess?.Invoke(container);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.Map<{typeof(TContainer).Name}>() failed - {ex.Message}", ex);
            }
        }

        public Response<T> MapWith<TProxy>(Action<TProxy>? configure = null)
            where TProxy : class, IProxy<TItem, T>, new()
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

                            if (_where != null && !_where(item))
                                continue;

                            _itemAction?.Invoke(item);
                            proxy.Add(item);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to process {type.Name} item at index={i} [jtoken: {jToken}]", ex);
                        }
                    }
                    response.Data = proxy.Create();
                }

                _postProcess?.Invoke(response.Data);
                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.Map<{typeof(TProxy).Name}>() failed - {ex.Message}", ex);
            }
        }
    } 
}