#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// special kind of list projection that provides map FristOrDefault method and other (in future)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerableProjection<T> : ProjectionBase, IEnumerable<T>
    {
        private Action<T>? _itemAction;
        public EnumerableProjection(RestResponse response) : base(response)
        {
        }

        public EnumerableProjection<T> ForEach(Action<T> action)
        {
            _itemAction = action;
            return this;
        }

        public Response<T> FirstOrDefault(Func<T, bool>? predicate = null)
        {
            try
            {
                var response = Response.Create<T>(Source, Error, Request);
                if (HasError || IsEmpty)
                    return response;

                var enumerator = GetEnumerator(typeof(T));
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    if (predicate == null || predicate(item))
                    {
                        response.Data = item;
                        break;
                    }
                }

                return response;                
            }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.{nameof(FirstOrDefault)} failed - {ex.Message}", ex);
            }
        }

        public Response<T> FirstOrDefault<TType>(Func<TType, bool>? predicate = null) where TType : T
        {
            try
            {
                var response = Response.Create<T>(Source, Error, Request);
                if (HasError || IsEmpty)
                    return response;

                var enumerator = GetEnumerator(typeof(TType));
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    if (predicate == null || predicate((TType)item!))
                    {
                        response.Data = item;
                        break;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetTypeName()}.{nameof(FirstOrDefault)} failed - {ex.Message}", ex);
            }
        }


        

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator(typeof(T));            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<T> GetEnumerator(Type itemType)
        {
            if (IsEmpty || HasError)
                yield break;

            var jArray = LoadToken<JArray>();
            var i = 0;

            foreach (var jToken in jArray)
            {
                T item;
                i++;
                try
                {
                    var obj = NewtonsoftJsonHelper.Deserialize<T>(jToken, itemType);
                    if (obj == null)
                        continue;

                    _itemAction?.Invoke(obj);
                    item = obj;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to process {itemType.Name} item at index={i} [jtoken: {jToken}]", ex);
                }

                yield return item;
            }
        }
    }
}