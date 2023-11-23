#nullable enable
using AVS.CoreLib.REST.Projections;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions
{
    public static class DictionaryProjectionExtensions
    {
        public static BoolResponse ToBoolResponse(this RestResponse result)
        {
            if (!result.TryDeserialize(out BoolResponse response, out var error))
                response = new BoolResponse() { Error = error };

            response.Source = result.Source;
            return response;
        }

        ///// <summary>
        ///// json object with key/value pairs to dictionary projection
        ///// </summary>
        ///// <typeparam name="TKey">The type of keys in the dictionary, for example string</typeparam>
        ///// <typeparam name="TValue">The type of values in the dictionary, it could be interface/abstraction, for example ICurrencyInfo</typeparam>
        //public static DictionaryProjection<TKey, TValue> AsDictionary<TKey, TValue>(this RestResponse response) where TKey : class
        //{
        //    return new DictionaryProjection<TKey, TValue>(response);
        //}

        
    }
}