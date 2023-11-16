using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Projections;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class RestResponseListProjections
    {
        #region ListProjection<TItem>

        /// <summary>
        /// create <see cref="ListProjection{T}"/> to map json of array type structure into <see cref="List{TItem}"/>
        /// it is a strait mapping approach
        /// <code>
        ///  var projection = restResponse.ToList`KunaOrder`();   
        ///  List`KunaOrder` list = projection.Map();
        /// </code>
        /// </summary>
        /// <typeparam name="T">either an interface (IMarketData) or a concrete type (BinanceMarketData)</typeparam>
        /// <remarks>when <see cref="TItem"/> is an abstraction <see cref="ListProjection{TItem}.Map{TProjection}"/> should be used</remarks>
        public static ListProjection<T> ToList<T>(this RestResponse response) where T : class
        {
            return new ListProjection<T>(response);
        }

        [Obsolete("Try to use ToList<T, TItem>() i.e. specity container explicitly ToList<List<T>,T>")]
        public static ListProjection<List<T>, T> AsList<T>(this RestResponse response) where T : class
        {
            return new ListProjection<List<T>, T>(response);
        }

        #endregion

        /// <summary>
        /// Creates <see cref="ListProjection{T,TItem}"/>  to map json of array type structure        
        /// it is an indirect mapping approach used in combination with builder (or proxy type) that implements <see cref="IArrayProxy{T,TItem}"/>        
        /// Where T is a container of the list of TItems
        /// TItem is a concrete item type
        /// <code>
        /// var projection = restResponse.ToList`IOpenOrders, BinanceOrder`()
        ///     .UseProxy`OpenOrdersBuilder`();//builder produces an IOpenOrders object
        /// </code>
        /// </summary>
        public static ListProjection<T, TItem> ToList<T, TItem>(this RestResponse response)
            where T : class
        {
            return new ListProjection<T, TItem>(response);
        }

        

        [Obsolete("Try to use ToList<T, TItem>() instead")]
        public static ListProjection<T, TItem> AsList<T, TItem>(this RestResponse response)
            where T : class
            where TItem : class
        {
            return new ListProjection<T, TItem>(response);
        }
    }

    public static class RestResponseObjectProjectionExtensions
    {
        #region ObjectProjection<T>
        /// <summary>
        /// Create object projection <see cref="ObjectProjection{T}"/> to map json object to type <see cref="T"/> 
        /// <code>
        /// <see cref="RestResponse.Content"/> should represent a json object, e.g.:
        /// {
        ///     "prop1" = 123,
        ///     "prop2" = [],
        ///     "prop3" = { ... }
        /// }
        /// </code>
        /// Concrete type projection examples:
        /// <code>
        ///    //direct mapping 
        ///    var projection = jsonResult.AsObject`MyObject`()
        ///    Response`MyObject` response = projection.Map();
        /// 
        ///    //indirect mapping:
        ///    var projection = jsonResult.AsObject{MyObject}().UseProxy`MyProxy{MyObject}`();
        ///    Response`MyObject` response = projection.Map();
        /// </code>
        /// An interface projection (<seealso cref="AsObject{T, TProjection}"/> could be more convenient for interface projection): 
        /// <code>
        ///    //interface projection (direct mapping):
        ///    var projection = jsonResult.AsObject{IMyObject}()
        ///    Response`IMyObject` response = projection.Map{MyObject}();
        ///
        ///    //interface projection (indirect mapping):
        ///    var projection = jsonResult.AsObject{IMyObject}().UseProxy`MyProxy`();
        ///    Response`IMyObject` response = projection.Map`MyProjection`(); //here MyProxy create IMyObject from MyProjection
        /// </code>
        /// </summary>
        public static ObjectProjection<T> AsObject<T>(this RestResponse response)
        {
            return new ObjectProjection<T>(response);
        }
        #endregion

        #region ObjectProjection<T, TProjection>
        /// <summary>
        /// Create object projection <see cref="ObjectProjection{T,TProjection}"/> to map a json object to type <see cref="TProjection"/>.
        /// but return result as a <see cref="Response{T}"/>.
        /// 
        /// !!! the Map() method requires proxy (container < <seealso cref="IObjectProxy{T, TProjection}"/>)  !!!
        /// 
        /// It is case of indirect mapping, when <see cref="TProjection"/> neither implement, neither inherit type <see cref="T"/>.
        /// The <see cref="ObjectProjection{T,TProjection}.UseProxy{TProxy}"/> is required, the proxy creates <see cref="T"/> from <see cref="TProjection"/>:
        /// <code>
        ///    var projection = jsonResult.AsObject{IMyObject, ProjectionType}().UseProxy`MyProxy{IMyObject}`();
        ///    Response`IMyObject` response = projection.Map();
        /// </code>
        /// </summary>
        public static ObjectProjection<T, TProjection> AsObject<T, TProjection>(this RestResponse response)
            where TProjection : new()
        {
            return new ObjectProjection<T, TProjection>(response);
        } 
        #endregion
    }

    public static class RestResponseProjectionExtensions
    {
        public static BoolResponse ToBoolResponse(this RestResponse result)
        {
            if (!result.TryDeserialize(out BoolResponse response, out var error))
                response = new BoolResponse() { Error = error };

            response.Source = result.Source;
            return response;
        }

        public static Response<T> ToResponse<T>(this RestResponse result)
        {
            if (!result.TryDeserialize(out Response<T> response, out var error))
                response = new Response<T>() { Error = error };

            response.Source = result.Source;
            return response;
        }

        /// <summary>
        /// json object with key/value pairs to dictionary projection
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary, for example string</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary, it could be interface/abstraction, for example ICurrencyInfo</typeparam>
        public static DictionaryProjection<TKey, TValue> AsDictionary<TKey, TValue>(this RestResponse response) where TKey : class
        {
            return new DictionaryProjection<TKey, TValue>(response);
        }

        /// <summary>
        /// Represents a keyed projection of json object with key/value pairs
        /// </summary>
        /// <typeparam name="T">The abstract/interface type of keyed collection, for example IBookTicker</typeparam>
        /// <typeparam name="TItem">The concrete type of item values in the collection, for example ExmoMarketData</typeparam>
        public static KeyedProjection<T, TItem> AsKeyedProjection<T, TItem>(this RestResponse response) where T : class
        {
            return new KeyedProjection<T, TItem>(response);
        }
    }
}