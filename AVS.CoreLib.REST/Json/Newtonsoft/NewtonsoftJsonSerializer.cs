#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AVS.CoreLib.Abstractions.Json;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Newtonsoft
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private static NullValueHandling NullValueHandling { get; set; } = NullValueHandling.Ignore;

        private static Lazy<JsonSerializer> LazySerializer => new Lazy<JsonSerializer>(
            () => new JsonSerializer() { NullValueHandling = NullValueHandling });
        internal static JsonSerializer Serializer => LazySerializer.Value;

        public string SerializeObject(object obj, Type? type = null)
        {
            return SerializeObjectInternal(obj, type, Serializer);
        }

        public object? DeserializeObject(string? json, Type type)
        {
            if (json == null)
                return null;

            using (var reader = new JsonTextReader(new StringReader(json)))
                return Serializer.Deserialize(reader, type);
        }

        public void Populate(object target, string json)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            using (var reader = new JsonTextReader(new StringReader(json)))
                Serializer.Populate(reader, target);
        }

        public string SerializeObject(object obj, Type? type = null, params Type[] converters)
        {
            if (converters == null || converters.Length == 0)
                return SerializeObject(obj);

            var conv = converters.Select(x => (JsonConverter)Activator.CreateInstance(x)!);
            var settings = new JsonSerializerSettings { Converters = conv.ToArray(), NullValueHandling = NullValueHandling };
            var serializer = JsonSerializer.CreateDefault(settings);
            return SerializeObjectInternal(obj, type, serializer);
        }

        private static string SerializeObjectInternal(object? value, Type? type, JsonSerializer serializer)
        {
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Serializer.Formatting;
                serializer.Serialize(jsonWriter, value, type);
            }

            return sw.ToString();
        }
    }
}