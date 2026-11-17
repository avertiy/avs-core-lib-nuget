#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Dates;
using AVS.CoreLib.Types;

namespace AVS.CoreLib.Json
{
    /// <summary>
    /// <see cref="System.Text.Json.JsonSerializer"/> helper 
    /// </summary>
    public static class JSON
    {
        private static JsonSerializerOptions? _options;
        private static Lazy<JsonSerializerOptions> DefaultOptions { get; } = new(() => Setup());

        /// <summary>
        /// Setup <see cref="DefaultOptions"/> 
        /// (i) JsonSerializerDefaults.Web + AllowTrailingCommas
        /// (ii) Converters: <see cref="JsonStringEnumConverter"/>, <see cref="DateRangeJsonConverter"/>
        /// </summary>
        public static JsonSerializerOptions Setup(Action<JsonSerializerOptions>? setup = null)
        {
            _options ??= CreateJsonSerializerOptions();
            setup?.Invoke(_options);
            return _options;
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                WriteIndented = false
            };

            //options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateRangeJsonConverter());
            return options;
        }

        public static string ToJson(this object obj, JsonSerializerOptions? options = null)
        {
            var json = JsonSerializer.Serialize(obj, options ?? DefaultOptions.Value);
            return json;
        }

        public static string ToJson(this object obj, Type inputType, JsonSerializerOptions? options = null)
        {
            var json = JsonSerializer.Serialize(obj, inputType, options ?? DefaultOptions.Value);
            return json;
        }

        private static Lazy<JsonSerializerOptions> BriefJsonOptions { get; } = new(() =>
        {
            var options = CreateJsonSerializerOptions();
            options.Converters.Add(new PlainJsonConverter()
            {
                Mode = SerializationMode.Brief,
                MaxItemsCount = 3,
                MaxPropsCount = 10,
                BytesLimit = 500,
                DepthLimit = 2
            });
            return options;
        });

        public static string ToBriefJson(this object obj, JsonSerializerOptions? options = null)
        {
            var json = JsonSerializer.Serialize(obj, options ?? BriefJsonOptions.Value);
            return json;
        }

        public static string ToPlainJson(this object obj, bool writeIndented = false, int maxDepth = 2, SerializationMode mode = SerializationMode.Brief, int maxItemsCount = 0, int maxPropsCount = 0, int sizeLimit = 0)
        {
            var serializationOptions = CreateJsonSerializerOptions();
            serializationOptions.WriteIndented = writeIndented;
            serializationOptions.Converters.Add(new PlainJsonConverter()
            {
                Mode = mode,
                MaxItemsCount = maxItemsCount,
                MaxPropsCount = maxPropsCount,
                DepthLimit = maxDepth,
                BytesLimit = sizeLimit
            });
            var json = JsonSerializer.Serialize(obj, serializationOptions);
            return json;
        }

        public static string ToJsonSafe(this object obj, JsonSerializerOptions? options = null)
        {
            try
            {
                return JsonSerializer.Serialize(obj, options ?? DefaultOptions.Value);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #region Deserialization

        public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions.Value);
        }

        public static object? Deserialize(ref Utf8JsonReader reader, Type type, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize(ref reader, type, options ?? DefaultOptions.Value);
        }

        public static T? Deserialize<T, TType>(this string json, JsonSerializerOptions? options = null) where TType : class, T
        {
            return (T?)JsonSerializer.Deserialize<TType>(json, options ?? DefaultOptions.Value);
        }

        /// <summary>
        /// <code>
        ///     List{IBar} = json.DeserializeList{IBar,Bar}()
        /// </code>
        /// </summary>
        public static List<T>? DeserializeList<T, TType>(this string json, JsonSerializerOptions? options = null) where TType : class, T
        {
            var list = JsonSerializer.Deserialize<List<TType>>(json, options ?? DefaultOptions.Value);
            return list?.Select(x => (T)x).ToList();
        }

        /// <summary>
        /// <code>
        ///     List{Bar} = json.DeserializeList{Bar}()
        /// </code>
        /// </summary>
        public static List<T>? DeserializeList<T>(this string json, JsonSerializerOptions? options = null) where T : new()
        {
            return JsonSerializer.Deserialize<List<T>>(json, options ?? DefaultOptions.Value);
        }

        /// <summary>
        /// <code>
        ///     Dictionary{string,IBar} = json.DeserializeDict{IBar,Bar}()
        /// </code>
        /// </summary>
        public static Dictionary<string, T>? DeserializeDict<T, TType>(this string json, JsonSerializerOptions? options = null) where TType : class, T
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, TType>>(json, options ?? DefaultOptions.Value);
            return dict?.ToDictionary(x => x.Key, v => (T)v.Value);
        }

        /// <summary>
        /// <code>
        ///     Dictionary{string,Bar} = json.DeserializeDict{Bar}()
        /// </code>
        /// </summary>
        public static Dictionary<string, T>? DeserializeDict<T>(this string json, JsonSerializerOptions? options = null) where T : new()
        {
            return JsonSerializer.Deserialize<Dictionary<string, T>>(json, options ?? DefaultOptions.Value);
        }

        /// <summary>
        /// Helps to ensure deserialization was correct
        /// (current implementation validates only enum properties)
        /// </summary>
        public static void ValidateDeserialization<T>(this T[] data, string jsonText, IList<KeyValue> mappings, int count = 10) where T : class
        {
            using JsonDocument doc = JsonDocument.Parse(jsonText);
            var dataNode = doc.RootElement.GetProperty("data");

            var n = Math.Min(count, data.Length);
            var type = typeof(T);
            var props = type.GetProperties().Where(x => x.PropertyType.IsEnum).ToArray();

            for (var i = 0; i < n; i++)
            {
                var item = data[i];
                var jsonElement = dataNode[i];
                jsonElement.Validate(type, props, item, mappings);
            }
        }

        #endregion
    }
}