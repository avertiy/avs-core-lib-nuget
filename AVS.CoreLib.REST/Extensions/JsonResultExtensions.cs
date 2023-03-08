using System.Collections.Generic;
using AVS.CoreLib.REST.Projections;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class JsonResultExtensions
    {
        public static BoolResponse AsBoolResponse(this JsonResult result)
        {
            var response = result.Deserialize<BoolResponse>();
            response.Source = result.Source;
            response.Error = result.Error;
            return response;
        }

        public static Response<T> DeserializeAsResponse<T>(this JsonResult result)
        {
            var response = result.Deserialize<Response<T>>();
            response.Source = result.Source;
            response.Error = result.Error;
            return response;
        }

        public static ObjectProjection<T> AsObject<T>(this JsonResult result)
        {
            return new ObjectProjection<T>(result.JsonText, result.Source, result.Error);
        }

        public static ObjectProjection<T, TProjection> AsObject<T, TProjection>(this JsonResult result)
            where TProjection : new()
        {
            return new ObjectProjection<T, TProjection>(result.JsonText, result.Source, result.Error);
        }

        /// <summary>
        /// <see cref="ListProjection{T}"/> map json of array type structure into <see cref="List{TItem}"/>
        /// </summary>
        public static ListProjection<TItem> AsListProjection<TItem>(this JsonResult result) where TItem : class
        {
            return new ListProjection<TItem>(result.JsonText, result.Source, result.Error);
        }

        public static ArrayProjection<TInterface, TItem> AsArray<TInterface, TItem>(this JsonResult result)
            where TInterface : class
        {
            return new ArrayProjection<TInterface, TItem>(result.JsonText, result.Source, result.Error);
        }

        public static ListProjection<List<T>, T> AsList<T>(this JsonResult result) where T : class
        {
            return new ListProjection<List<T>, T>(result.JsonText, result.Source, result.Error);
        }

        public static ListProjection<T, TItem> AsList<T, TItem>(this JsonResult result)
            where T : class
            where TItem : class
        {
            return new ListProjection<T, TItem>(result.JsonText, result.Source, result.Error);
        }

        /// <summary>
        /// json object with key/value pairs to dictionary projection
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary, for example string</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary, it could be interface/abstraction, for example ICurrencyInfo</typeparam>
        public static DictionaryProjection<TKey, TValue> AsDictionary<TKey, TValue>(this JsonResult result) where TKey : class
        {
            return new DictionaryProjection<TKey, TValue>(result.JsonText, result.Source, result.Error);
        }

        /// <summary>
        /// Represents a keyed projection of json object with key/value pairs
        /// </summary>
        /// <typeparam name="T">The abstract/interface type of keyed collection, for example IBookTicker</typeparam>
        /// <typeparam name="TItem">The concrete type of item values in the collection, for example ExmoMarketData</typeparam>
        public static KeyedProjection<T, TItem> AsKeyedProjection<T, TItem>(this JsonResult result) where T : class
        {
            return new KeyedProjection<T, TItem>(result.JsonText, result.Source, result.Error);
        }
    }
}