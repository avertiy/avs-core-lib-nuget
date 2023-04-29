using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Projections;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class JsonResultExtensions
    {
        public static BoolResponse ToBoolResponse(this JsonResult result)
        {
            if (!result.TryDeserialize(out BoolResponse response, out var error))
                response = new BoolResponse() { Error = error };

            response.Source = result.Source;
            return response;
        }

        public static Response<T> ToResponse<T>(this JsonResult result)
        {
            if (!result.TryDeserialize(out Response<T> response, out var error))
                response = new Response<T>() { Error = error };

            response.Source = result.Source;
            return response;
        }

        /// <summary>
        /// create <see cref="ListProjection{T}"/> to map json of array type structure into <see cref="List{TItem}"/>
        /// </summary>
        /// <typeparam name="TItem">either an interface (IMarketData) or a concrete type (BinanceMarketData)</typeparam>
        /// <remarks>when <see cref="TItem"/> is an abstraction <see cref="ListProjection{TItem}.Map{TProjection}"/> should be used</remarks>
        public static ListProjection<TItem> ToListProjection<TItem>(this JsonResult result) where TItem : class
        {
            return new ListProjection<TItem>(result.JsonText, result.Source, result.Error);
        }

        /// <summary>
        /// create <see cref="ObjectProjection{T}"/>
        /// </summary>
        public static ObjectProjection<T> AsObject<T>(this JsonResult result)
        {
            return new ObjectProjection<T>(result.JsonText, result.Source, result.Error);
        }

        /// <summary>
        /// create <see cref="ObjectProjection{T,TProjection}"/>
        /// </summary>
        public static ObjectProjection<T, TProjection> AsObject<T, TProjection>(this JsonResult result)
            where TProjection : new()
        {
            return new ObjectProjection<T, TProjection>(result.JsonText, result.Source, result.Error);
        }

        
        [Obsolete("use ToList<TInterface, TItem>()")]
        public static ListProjection<TInterface, TItem> AsArray<TInterface, TItem>(this JsonResult result)
            where TInterface : class
        {
            return new ListProjection<TInterface, TItem>(result.JsonText, result.Source, result.Error);
        }

        public static ListProjection<TInterface, TItem> ToList<TInterface, TItem>(this JsonResult result)
            where TInterface : class
        {
            return new ListProjection<TInterface, TItem>(result.JsonText, result.Source, result.Error);
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