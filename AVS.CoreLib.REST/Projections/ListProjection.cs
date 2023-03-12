using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Map json of array type structure into <see cref="List{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">could be either a concrete type or an interface </typeparam>
    /// <example>
    /// Response{List{BinanceMarketData}} response = jsonResult.AsList{BinanceMarketData}().Map();
    /// Response{List{IMarketData}} response = jsonResult.AsList{IMarketData}().Map{BinanceMarketData}(); 
    /// </example>
    public class ListProjection<TItem> : Projection where TItem : class
    {
        private Action<List<TItem>> _postProcessAction;

        private Action<TItem> _itemAction;
        private Func<TItem, bool> _where;
        private IListProxy<TItem> _proxy;


        public ListProjection(string jsonText, string source, string error = null) : base(jsonText, source, error)
        {
        }

        public ListProjection<TItem> PostProcess(Action<List<TItem>> action)
        {
            _postProcessAction = action;
            return this;
        }

        public ListProjection<TItem> ForEach(Action<TItem> action)
        {
            _itemAction = action;
            return this;
        }

        public ListProjection<TItem> Where(Func<TItem, bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public ListProjection<TItem> UseProxy<TProxy>(Action<TProxy> initialize = null)
            where TProxy : class, IListProxy<TItem>, new()
        {
            var builder = new TProxy();
            initialize?.Invoke(builder);
            _proxy = builder;
            return this;
        }

        public Response<List<TItem>> Map()
        {
            var response = MapInternal<List<TItem>>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = _proxy == null ? new List<TItem>() : _proxy.Create();
                }
                else
                {
                    LoadToken<JArray, TItem>(jArray =>
                    {
                        foreach (var jToken in jArray)
                        {
                            var item = JsonHelper.Deserialize<TItem>(jToken, typeof(TItem));

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;

                            if (_proxy == null)
                                response.Data.Add(item);
                            else
                                _proxy.Add(item);

                        }
                    });

                    if (_proxy == null)
                    {
                        _postProcessAction?.Invoke(response.Data);
                    }
                    else
                    {
                        var data = _proxy.Create();
                        _postProcessAction?.Invoke(data);
                        response.Data = data;
                    }
                }
            });

            return response;
        }

        public Response<List<TItem>> Map<TProjection>()
            where TProjection : TItem
        {
            var response = MapInternal<List<TItem>>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = _proxy == null ? new List<TItem>() : _proxy.Create();
                }
                else
                {
                    LoadToken<JArray, TProjection>(jArray =>
                    {
                        foreach (var jToken in jArray)
                        {
                            var item = JsonHelper.Deserialize<TProjection>(jToken, typeof(TProjection));

                            _itemAction?.Invoke(item);

                            if (_where != null && !_where(item))
                                continue;

                            if (_proxy == null)
                                response.Data.Add(item);
                            else
                                _proxy.Add(item);

                        }
                    });

                    if (_proxy == null)
                    {
                        _postProcessAction?.Invoke(response.Data);
                    }
                    else
                    {
                        var data = _proxy.Create();
                        _postProcessAction?.Invoke(data);
                        response.Data = data;
                    }
                }
            });

            return response;
        }
    }

    ///// <summary>
    ///// this type is just a synonym of <see cref="ArrayProjection{T,TItem}"/>
    ///// </summary>
    //public class ListProjection<T, TItem> : ArrayProjection<T, TItem> where T : class
    //{
    //    [DebuggerStepThrough]
    //    public ListProjection(string jsonText, string source, string error = null) : base(jsonText, source, error)
    //    {
    //    }

    //    //[Obsolete("Seems MapResult could be avoided, so it will be removed")]
    //    //public virtual MapResult MapResult<TProjection>() where TProjection : TItem, new()
    //    //{
    //    //    var result = CreateMapResult<List<TItem>>();
    //    //    if (result.Success)
    //    //    {
    //    //        LoadToken<JArray, TProjection, TItem>(jArray =>
    //    //        {
    //    //            _preProcessAction?.Invoke(result.Data as T);
    //    //            result.Data = JsonHelper.ParseList<TItem>(jArray, typeof(TProjection), _itemAction, _where);
    //    //            _postProcessAction?.Invoke(result.Data as T);
    //    //        });
    //    //    }

    //    //    return result;
    //    //}
    //}
}