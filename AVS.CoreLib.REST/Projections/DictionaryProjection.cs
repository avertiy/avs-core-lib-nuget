#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    public class DictionaryProjection<TValue> : ProjectionBase
    {
        private Action<IDictionary<string, TValue>>? _preProcess;
        private Action<IDictionary<string, TValue>>? _postProcess;
        private Func<string, string>? _preprocessKey;
        private Func<string, bool>? _filterByKey;
        protected Action<string, TValue>? _keyValueAction;

        public DictionaryProjection(RestResponse response) : base(response)
        {
        }

        public DictionaryProjection<TValue> PreProcess(Action<IDictionary<string, TValue>> action)
        {
            _preProcess = action;
            return this;
        }

        public DictionaryProjection<TValue> PostProcess(Action<IDictionary<string, TValue>> action)
        {
            _postProcess = action;
            return this;
        }

        /// <summary>
        /// preprocess key
        /// </summary>
        public DictionaryProjection<TValue> Key(Func<string, string> func)
        {
            _preprocessKey = func;
            return this;
        }

        public DictionaryProjection<TValue> FilterByKey(Func<string, bool> predicate)
        {
            _filterByKey = predicate;
            return this;
        }

        public DictionaryProjection<TValue> ForEach(Action<string, TValue> action)
        {
            _keyValueAction = action;
            return this;
        }

        #region Map methods
        /// <summary>
        /// Map json into dictionary with string key and T value
        /// </summary>        
        public Response<IDictionary<string, TValue>> Map()
        {
            var response = Response.Create<IDictionary<string, TValue>>(Source, Error, Request);
            if (HasError)
                return response;

            var dict = new Dictionary<string, TValue>();
            _preProcess?.Invoke(dict);

            var enumerator = GetEnumerator(typeof(TValue));
            while (enumerator.MoveNext())
            {
                var kp = enumerator.Current;
                _keyValueAction?.Invoke(kp.Key, kp.Value);
                dict.Add(kp.Key, kp.Value);
            }

            _postProcess?.Invoke(dict);
            response.Data = dict;

            return response;
        }

        /// <summary>
        /// Map json into dictionary with string key and T value
        /// </summary>        
        /// <typeparam name="TProjection">type to deserialize value part of the json object with key-value structure</typeparam>
        public Response<IDictionary<string, TValue>> Map<TProjection>() where TProjection : TValue
        {
            var response = Response.Create<IDictionary<string, TValue>>(Source, Error, Request);
            if (HasError)
                return response;

            var dict = new Dictionary<string, TValue>();
            _preProcess?.Invoke(dict);

            var enumerator = GetEnumerator(typeof(TProjection));
            while (enumerator.MoveNext())
            {
                var kp = enumerator.Current;
                _keyValueAction?.Invoke(kp.Key, kp.Value);
                dict.Add(kp.Key, kp.Value);
            }

            _postProcess?.Invoke(dict);
            response.Data = dict;

            return response;
        }

        /// <summary>
        /// Map json into dictionary with string key and T value
        /// </summary>        
        public Response<IDictionary<string, TValue>> MapWith<TProxy>()
            where TProxy : IProxy<IDictionary<string, TValue>>, IContainer<string, TValue>, new()
        {
            var response = Response.Create<IDictionary<string, TValue>>(Source, Error, Request);
            if (HasError)
                return response;

            var proxy = new TProxy();

            var enumerator = GetEnumerator(typeof(TValue));
            while (enumerator.MoveNext())
            {
                var kp = enumerator.Current;
                proxy.Add(kp.Key, kp.Value);
            }

            var dict = proxy.Create();
            _postProcess?.Invoke(dict);
            response.Data = dict;

            return response;
        }

        public Response<IDictionary<string, TValue>> MapWith<TProxy, TValueType>()
            where TProxy : IProxy<IDictionary<string, TValue>>, IContainer<string, TValue>, new()
            where TValueType : TValue
        {
            var response = Response.Create<IDictionary<string, TValue>>(Source, Error, Request);
            if (HasError)
                return response;

            var proxy = new TProxy();

            var enumerator = GetEnumerator(typeof(TValueType));
            while (enumerator.MoveNext())
            {
                var kp = enumerator.Current;
                proxy.Add(kp.Key, kp.Value);
            }

            var dict = proxy.Create();
            _postProcess?.Invoke(dict);
            response.Data = dict;

            return response;
        } 
        #endregion

        private IEnumerator<(string Key, TValue Value)> GetEnumerator(Type valueType)
        {
            if (IsEmpty || HasError)
                yield break;

            var jObject = LoadToken<JObject>();
            if(!jObject.HasValues)
                yield break;

            foreach (var kp in jObject)
            {
                var key = _preprocessKey == null ? kp.Key : _preprocessKey(kp.Key);
                TValue? value;
                try
                {
                    if (_filterByKey != null && !_filterByKey(key))
                        continue;

                    value = JsonHelper.Deserialize<TValue>(kp.Value, valueType);

                    if (value == null)
                        throw new NullReferenceException("value is null");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to process {valueType.Name} value [key={key}]", ex);
                }

                yield return (key, value);
            }
        }
    }


    //  DON`T DELETE CODE BELOW CONTAINS COMPLEX DICTIONARY MAPPINGS WHICH ARE NOT YET EXTRACTED 
    //  TODO add ContainerProjection.MapDictionary(..) methods
    ///// <summary>
    ///// Represents a dictionary projection of json object with key/value pairs
    ///// </summary>
    ///// <typeparam name="TKey">The type of keys in the dictionary, for example string</typeparam>
    ///// <typeparam name="TValue">The type of values in the dictionary, it could be interface/abstraction, for example ICurrencyInfo</typeparam>
    //public class DictionaryProjection<TKey, TValue> : ProjectionBase
    //    where TKey : class
    //{
    //    private Action<IDictionary<TKey, TValue>>? _preProcessAction;
    //    private Action<IDictionary<TKey, TValue>>? _postProcessAction;
    //    private Action<TKey, TValue>? _preprocessKeyValue = null;
    //    private Func<string, TKey>? _preprocessKey = null;
    //    private Func<TValue, TValue>? _preprocessValue = null;
    //    private Func<TKey, bool>? _where = null;

    //    [DebuggerStepThrough]
    //    public DictionaryProjection(RestResponse response) : base(response)
    //    {
    //    }

    //    public DictionaryProjection<TKey, TValue> PreProcess(Action<IDictionary<TKey, TValue>> action)
    //    {
    //        _preProcessAction = action;
    //        return this;
    //    }
    //    public DictionaryProjection<TKey, TValue> PostProcess(Action<IDictionary<TKey, TValue>> action)
    //    {
    //        _postProcessAction = action;
    //        return this;
    //    }

    //    /// <summary>
    //    /// preprocess key/value pair
    //    /// </summary>
    //    public DictionaryProjection<TKey, TValue> ForEach(Action<TKey, TValue> action)
    //    {
    //        _preprocessKeyValue = action;
    //        return this;
    //    }

    //    /// <summary>
    //    /// preprocess key
    //    /// </summary>
    //    public DictionaryProjection<TKey, TValue> Key(Func<string, TKey> keyConverter)
    //    {
    //        _preprocessKey = keyConverter;
    //        return this;
    //    }

    //    /// <summary>
    //    /// preprocess value
    //    /// </summary>
    //    public DictionaryProjection<TKey, TValue> Value(Func<TValue, TValue> valueFunc)
    //    {
    //        _preprocessValue = valueFunc;
    //        return this;
    //    }

    //    /// <summary>
    //    /// where predicate to filter not relevant items 
    //    /// </summary>
    //    public DictionaryProjection<TKey, TValue> Where(Func<TKey, bool> predicate)
    //    {
    //        _where = predicate;
    //        return this;
    //    }

    //    /// <summary>
    //    /// Map json into dictionary
    //    /// </summary>
    //    /// <typeparam name="TProjection">The projection (concrete) type of values in the dictionary, for example CurrencyInfo</typeparam>
    //    public virtual Response<IDictionary<TKey, TValue>> Map<TProjection>() where TProjection : TValue, new()
    //    {
    //        var response = CreateResponse<IDictionary<TKey, TValue>, Dictionary<TKey, TValue>>();
    //        if (response.Success)
    //        {
    //            var data = new Dictionary<TKey, TValue>();
    //            _preProcessAction?.Invoke(data);
    //            LoadToken<JObject, TProjection, TValue>(jObject =>
    //            {
    //                if (!jObject.HasValues)
    //                    return;

    //                var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
    //                var itemType = typeof(TProjection);
    //                var tValue = typeof(TValue);

    //                //simple case
    //                if (tValue.IsAssignableFrom(itemType))
    //                {
    //                    foreach (KeyValuePair<string, JToken> kp in jObject)
    //                    {
    //                        var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                        if (_where != null && !_where(key))
    //                            continue;

    //                        if (kp.Value.Type != JTokenType.Object)
    //                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                        var value = (TValue)serializer.Deserialize(kp.Value.CreateReader(), itemType);


    //                        value = _preprocessValue != null ? _preprocessValue(value) : value;

    //                        _preprocessKeyValue?.Invoke(key, value);
    //                        data.Add(key, value);
    //                    }
    //                }
    //                else
    //                {
    //                    //case when TValue is a collection/array/list of items
    //                    var genericType = Match(tValue, itemType);
    //                    MethodInfo addMethod = genericType.GetMethod("Add");

    //                    if (addMethod == null)
    //                        throw new Exception($"Add method not found");

    //                    foreach (KeyValuePair<string, JToken> kp in jObject)
    //                    {
    //                        var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                        if (_where != null && !_where(key))
    //                            continue;

    //                        if (kp.Value.Type != JTokenType.Array)
    //                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                        var list = ParseGenericType<TValue>((JArray)kp.Value, genericType, addMethod, itemType);


    //                        list = _preprocessValue != null ? _preprocessValue(list) : list;

    //                        _preprocessKeyValue?.Invoke(key, list);
    //                        data.Add(key, list);
    //                    }
    //                }
    //            });

    //            _postProcessAction?.Invoke(data);
    //            response.Data = data;
    //        }
    //        return response;
    //    }

    //    public Dictionary<TKey, TValue> InspectDeserialization<TProjection>(Action<JObject> inspect, Action<string, JToken> inspectItem, out Exception err)
    //    {
    //        try
    //        {
    //            Exception error = null;
    //            var data = new Dictionary<TKey, TValue>();

    //            LoadToken<JObject, TProjection, TValue>(jObject =>
    //            {
    //                inspect(jObject);
    //                var itemType = typeof(TProjection);
    //                var tValue = typeof(TValue);
    //                var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

    //                if (tValue.IsAssignableFrom(itemType))
    //                {
    //                    foreach (KeyValuePair<string, JToken> kp in jObject)
    //                    {
    //                        inspectItem(kp.Key, kp.Value);

    //                        var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                        if (_where != null && !_where(key))
    //                            continue;

    //                        if (kp.Value.Type != JTokenType.Object)
    //                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                        var value = (TValue)serializer.Deserialize(kp.Value.CreateReader(), itemType);


    //                        value = _preprocessValue != null ? _preprocessValue(value) : value;

    //                        _preprocessKeyValue?.Invoke(key, value);
    //                        data.Add(key, value);
    //                    }
    //                }
    //                else
    //                {
    //                    //case when TValue is a collection/array/list of items
    //                    var genericType = Match(tValue, itemType);
    //                    MethodInfo addMethod = genericType.GetMethod("Add");

    //                    if (addMethod == null)
    //                        throw new Exception($"Add method not found");

    //                    foreach (KeyValuePair<string, JToken> kp in jObject)
    //                    {
    //                        inspectItem(kp.Key, kp.Value);

    //                        var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                        if (_where != null && !_where(key))
    //                            continue;

    //                        if (kp.Value.Type != JTokenType.Array)
    //                            throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                        try
    //                        {
    //                            var list = ParseGenericType<TValue>((JArray)kp.Value, genericType, addMethod, itemType);
    //                            list = _preprocessValue != null ? _preprocessValue(list) : list;
    //                            _preprocessKeyValue?.Invoke(key, list);
    //                            data.Add(key, list);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            error = ex;
    //                            break;
    //                        }
    //                    }
    //                }
    //            });

    //            err = error;
    //            return data;
    //        }
    //        catch (Exception ex)
    //        {
    //            err = ex;
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// Maps json into specified type T
    //    /// </summary>
    //    /// <typeparam name="TResponse">Type inherited from Response &lt;IDictionary&lt;TKey, TValue>></typeparam>
    //    /// <typeparam name="TProjection">implementation of the TValue interface, in case TValue is IList&lt;IEntity> TProjection must be implementation of IEntity</typeparam>
    //    public virtual TResponse Map<TResponse, TProjection>()
    //        where TResponse : Response<IDictionary<TKey, TValue>>, new()
    //        where TProjection : new()
    //    {
    //        var response = new TResponse() { Source = Source, Error = Error };
    //        if (HasError)
    //            return response;

    //        if (IsEmpty)
    //        {
    //            response.Data = new Dictionary<TKey, TValue>();
    //            return response;
    //        }

    //        using (var stringReader = new StringReader(JsonText))
    //        using (var reader = new JsonTextReader(stringReader))
    //        {
    //            var token = JToken.Load(reader);

    //            if (token.Type == JTokenType.Object)
    //            {
    //                var jObject = (JObject)token;

    //                try
    //                {
    //                    response.Data = ParseDictionary<TProjection>(jObject);
    //                }
    //                catch (Exception ex)
    //                {
    //                    var msg =
    //                        $"ParseDictionary<{typeof(TKey).Name},{typeof(TValue).GetReadableName()}> with projection of {typeof(TProjection).GetReadableName()}> failed";
    //                    throw new MapException(msg, ex) { JsonText = JsonText };
    //                }

    //                return response;
    //            }

    //            throw new JsonReaderException($"Unexpected JToken type {token.Type}");
    //        }
    //    }

    //    public virtual TResponse Map<TResponse, TProjection, TData>(Func<IDictionary<TKey, TValue>, TData> transform)
    //        where TResponse : Response<TData>, new()
    //        where TProjection : new()
    //    {
    //        var response = new TResponse() { Source = Source, Error = Error };
    //        if(HasError) return response;

    //        if (IsEmpty)
    //        {
    //            response.Data = transform(new Dictionary<TKey, TValue>());
    //            return response;
    //        }

    //        using (var stringReader = new StringReader(JsonText))
    //        using (var reader = new JsonTextReader(stringReader))
    //        {
    //            var token = JToken.Load(reader);
    //            if (token.Type == JTokenType.Object)
    //            {
    //                var jObject = (JObject)token;
    //                IDictionary<TKey, TValue> data;
    //                try
    //                {
    //                    data = ParseDictionary<TProjection>(jObject);
    //                }
    //                catch (Exception ex)
    //                {
    //                    throw new MapException($"ParseDictionary<{typeof(TKey).Name},{typeof(TValue).GetReadableName()}> with projection of {typeof(TProjection).GetReadableName()}> failed", ex) { JsonText = JsonText };
    //                }

    //                response.Data = transform(data);
    //                return response;
    //            }
    //            throw new MapException($"Unexpected JToken type {token.Type}") { JsonText = JsonText };
    //        }
    //    }


    //    private IDictionary<TKey, TValue> ParseDictionary<TProjection>(JObject jObject)
    //    {
    //        var dictionary = new Dictionary<TKey, TValue>(jObject.Count);
    //        return ParseDictionary<TProjection, Dictionary<TKey, TValue>>(dictionary, jObject);
    //    }

    //    private T ParseDictionary<TProjection, T>(T dictionary, JObject jObject)
    //        where T : IDictionary<TKey, TValue>, new()
    //    {
    //        if (!jObject.HasValues)
    //            return dictionary;

    //        _preProcessAction?.Invoke(dictionary);

    //        CheckTKeyType();
    //        var itemType = typeof(TProjection);
    //        var tValue = typeof(TValue);

    //        var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

    //        if (tValue.IsAssignableFrom(itemType))
    //        {
    //            foreach (KeyValuePair<string, JToken> kp in jObject)
    //            {
    //                var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                if (_where != null && !_where(key))
    //                    continue;

    //                if (kp.Value.Type != JTokenType.Object)
    //                    throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                var value = (TValue)serializer.Deserialize(kp.Value.CreateReader(), itemType);


    //                value = _preprocessValue != null ? _preprocessValue(value) : value;

    //                _preprocessKeyValue?.Invoke(key, value);
    //                dictionary.Add(key, value);
    //            }
    //        }
    //        else
    //        {
    //            try
    //            {
    //                var genericType = Match(tValue, itemType);
    //                MethodInfo addMethod = genericType.GetMethod("Add");

    //                if (addMethod == null)
    //                    throw new Exception($"Add method not found");

    //                foreach (KeyValuePair<string, JToken> kp in jObject)
    //                {
    //                    var key = _preprocessKey == null ? ConvertToTKey(kp.Key) : _preprocessKey(kp.Key);
    //                    if (_where != null && !_where(key))
    //                        continue;

    //                    if (kp.Value.Type != JTokenType.Array)
    //                        throw new JsonReaderException($"Unexpected JToken type {kp.Value.Type}");

    //                    var list = ParseGenericType<TValue>((JArray)kp.Value, genericType, addMethod, itemType);


    //                    list = _preprocessValue != null ? _preprocessValue(list) : list;

    //                    _preprocessKeyValue?.Invoke(key, list);
    //                    dictionary.Add(key, list);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                throw new Exception($"ParseGenericType<{typeof(TKey).Name},{itemType.GetReadableName()}> failed", ex);
    //            }
    //        }

    //        _postProcessAction?.Invoke(dictionary);

    //        return dictionary;
    //    }

    //    internal static T ParseGenericType<T>(JArray jArray, Type genericType, MethodInfo addMethod, Type itemType)
    //    {
    //        if (addMethod == null)
    //            throw new ArgumentNullException(nameof(addMethod));

    //        var list = Activator.CreateInstance(genericType);

    //        if (!jArray.HasValues)
    //            return (T)list;

    //        var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

    //        foreach (JToken token in jArray)
    //        {
    //            if (token.Type != JTokenType.Object)
    //                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
    //            try
    //            {
    //                var value = serializer.Deserialize(token.CreateReader(), itemType);
    //                addMethod.Invoke(list, new[] { value });
    //            }
    //            catch (Exception ex)
    //            {
    //                throw new JsonException($"unable deserialize JToken [{token.ToString()}] into {itemType.GetReadableName()} ", ex);
    //            }
    //        }

    //        return (T)list;
    //    }

    //    internal static Type Match(Type tValue, Type tProjection)
    //    {
    //        if (!tValue.IsGenericType)
    //        {
    //            throw new ArgumentException($"{tValue.Name} must be a generic type");
    //        }

    //        var genericArgs = tValue.GetGenericArguments();

    //        if (genericArgs.Length != 1)
    //            throw new ArgumentException($"{tValue.Name} must have one generic argument");

    //        if (!genericArgs[0].IsInterface)
    //            throw new ArgumentException($"{genericArgs[0].Name} must be an interface");

    //        Type[] interfaces = tProjection.GetInterfaces();
    //        if (!interfaces.Contains(genericArgs[0]))
    //        {
    //            throw new ArgumentException($"{tProjection.Name} is expected to implement {genericArgs[0].Name}");
    //        }

    //        Type type = null;
    //        if (tValue.Name.StartsWith("IList") || tValue.Name.StartsWith("List"))
    //        {
    //            type = typeof(List<>);
    //        }
    //        else if (tValue.Name.StartsWith("ICollection") || tValue.Name.StartsWith("Collection"))
    //        {
    //            type = typeof(Collection<>);
    //        }

    //        if (type == null)
    //            throw new NotSupportedException($"{tValue.Name} must be assignable from List<> or Collection<>");

    //        var genericType = type.MakeGenericType(genericArgs[0]);
    //        return genericType;

    //    }

    //    internal static void CheckTKeyType()
    //    {
    //        var tKey = typeof(TKey);
    //        var tString = typeof(string);
    //        if (tKey == tString)
    //            return;

    //        throw new NotSupportedException($"The {tKey.Name} type of TKey is not supported");
    //    }

    //    internal static TKey ConvertToTKey(dynamic str)
    //    {
    //        return (TKey)str;
    //    }
    //}
}